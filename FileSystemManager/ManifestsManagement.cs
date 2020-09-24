using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MakerSquare
{
    namespace FileSystem
    {
        public partial class Manager
        {
            public string ManifestDir { get { return Path.Combine(ResourceDirName, "Manifests"); } }
            public string GlobalManifestPath { get; private set; }

            protected XmlWriterSettings writerSettings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace,
            };

            protected Dictionary<string, string> _ContentGroups = new Dictionary<string, string>
                {   {"FreeTypeFonts", "Ultraviolet.FreeType2.FreeTypeFont, Ultraviolet.FreeType2" },
                    {"Sprites", "Ultraviolet.Graphics.Graphics2D.Sprite" },
                    {"Textures", "Ultraviolet.Graphics.Texture2D" },
                    {"Songs", "Ultraviolet.Audio.Song" },
                    {"SoundEffects", "Ultraviolet.Audio.SoundEffect" },
                    {"Cursors", "Ultraviolet.CursorCollection" },
                    {"Effects", "Ultraviolet.Graphics.Effect" }};

            protected void CreateDefaultManifest()
            {                
                using (XmlWriter writer = XmlWriter.Create(GlobalManifestPath, writerSettings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("ContentGroup");

                    writer.WriteStartAttribute("Name");
                    writer.WriteRaw("GlobalContent");
                    writer.WriteEndAttribute();

                    foreach (var each in _ContentGroups)
                    {
                        writer.WriteStartElement("ContentGroup");
                        writer.WriteStartAttribute("Name");
                        writer.WriteRaw(each.Key);
                        writer.WriteEndAttribute();
                        writer.WriteStartAttribute("Directory");
                        writer.WriteRaw(each.Key);
                        writer.WriteEndAttribute();
                        Directory.CreateDirectory(Path.Combine(ProjectDir, ResourceDirName, each.Key));
                        writer.WriteStartAttribute("Type");
                        writer.WriteRaw(each.Value);
                        writer.WriteEndAttribute();
                        writer.WriteFullEndElement();                        
                    }

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }

            protected void ManifestCreation()
            {
                try
                {
                    GlobalManifestPath = Path.Combine(ProjectDir, ManifestDir, "Global.manifest");
                    if (!Directory.Exists(Path.Combine(ProjectDir, ManifestDir)))
                        Directory.CreateDirectory(Path.Combine(ProjectDir, ManifestDir));                    
                    CreateDefaultManifest();
                }
                catch (Exception e)
                {
                    throw new CannotCreateDefaultManifestException("Manifest creation crashed", e);
                }
            }            

            protected void AddGlobalAsset(string directory, string asset_name)
            {
                var doc = new XmlDocument();               
                doc.Load(GlobalManifestPath);
                var global_node = doc.SelectSingleNode(String.Format("//ContentGroup[@Directory='{0}']", directory));
                var new_node = doc.CreateElement("Asset");
                new_node.InnerText = asset_name;                
                new_node.SetAttribute("Name", Regex.Replace(asset_name, @"\s+", ""));                                
                global_node.AppendChild(new_node);
                doc.Save(GlobalManifestPath);
            }
            
            protected void RemoveGlobalAsset(string directory, string asset_name)
            {
                var doc = new XmlDocument();
                doc.Load(GlobalManifestPath);
                var global_node = doc.SelectSingleNode(String.Format("//ContentGroup[@Directory='{0}']", directory));                
                var asset = global_node.SelectSingleNode(String.Format("//Asset[@Name='{0}']", Regex.Replace(asset_name, @"\s+", "")));
                global_node.RemoveChild(asset);
                doc.Save(GlobalManifestPath);
            }
        }
    }
}
