using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace MakerSquare
{
    namespace FileSystem
    {
        public partial class Manager
        {
            public string SpriteDirName { get { return "Sprites"; } }
            public string FramesDirName { get { return "Textures"; } }

            private void CreateDotSpriteFile(string sprite_name, uint duration, uint originX, uint originY, uint nb_frames)
            {
                var doc = new XmlDocument();
                doc.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n" +
                            "<Sprite>\n" +
                            "<Animations>\n" +
                            "</Animations>\n" +
                            "</Sprite>");
                var animations = doc.SelectSingleNode("//Animations");
                var anim_name = doc.CreateElement("Animation");
                anim_name.SetAttribute("Name", sprite_name);
                var frame_data = doc.CreateElement("Frames");
                frame_data.SetAttribute("Atlas", "Textures/" + sprite_name);
                frame_data.SetAttribute("Duration", duration.ToString());
                frame_data.SetAttribute("OriginX", originX.ToString());
                frame_data.SetAttribute("OriginY", originY.ToString());
                var frame_length_size = nb_frames.ToString().Length;
                for (uint i = 1; i <= nb_frames; i++)
                {
                    var elem = doc.CreateElement("Frame");
                    elem.SetAttribute("AtlasCell", String.Format("{0}_" + String.Format("{{1:D{0}}}", frame_length_size), sprite_name, i));
                    frame_data.AppendChild(elem);
                }
                anim_name.AppendChild(frame_data);
                animations.AppendChild(anim_name);
                doc.Save(Path.Combine(ProjectDir, ResourceDirName, SpriteDirName) + "\\" + sprite_name + ".sprite");
            }
            
            private void CreateSpriteXMLFile(string sprite_name)
            {
                var doc = new XmlDocument();

                if (!Directory.Exists(Path.Combine(ProjectDir, ResourceDirName, SpriteDirName, FramesDirName)))
                    Directory.CreateDirectory(Path.Combine(ProjectDir, ResourceDirName, SpriteDirName, FramesDirName));

                doc.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n" +
                            "<TextureAtlas>\n" +
                            "<Metadata>\n" +
                            "</Metadata>\n" +
                            "<Images>\n" +
                            String.Format("<Include>{0}_*.png</Include>\n", sprite_name) +
                            "</Images>\n" +
                            "</TextureAtlas>");

                doc.Save(Path.Combine(ProjectDir, ResourceDirName, SpriteDirName, FramesDirName) +  "\\" + sprite_name + ".xml");
            }

            private void PopulateFrameDirectory(List<BitmapImage> frames, string sprite_name)
            {
                var frame_length_size = frames.Count.ToString().Length;

                for (int i = 0; i < frames.Count; i++)
                {
                    var file_name = Path.Combine(ProjectDir, ResourceDirName, SpriteDirName, FramesDirName) + "\\" 
                        + String.Format("{0}_" + String.Format("{{1:D{0}}}", frame_length_size) + ".png", sprite_name, i + 1);

                    PngBitmapEncoder PngBitmapEncoder = new PngBitmapEncoder();
                    PngBitmapEncoder.Frames.Add(BitmapFrame.Create(frames[i]));
                    using (FileStream fileStream = new FileStream(file_name, FileMode.Create))
                    {
                        PngBitmapEncoder.Save(fileStream);
                        fileStream.Flush();
                        fileStream.Close();
                    }
                }
            }

            public void AddSprite(List<BitmapImage> frames, string sprite_name, uint sprite_duration)
            {                
                CreateDotSpriteFile(sprite_name, sprite_duration, 0, 0, (uint)frames.Count);
                CreateSpriteXMLFile(sprite_name);
                PopulateFrameDirectory(frames, sprite_name);
                AddGlobalAsset(SpriteDirName, sprite_name);

                //Adding file to virtual directory system
                var file_name = Path.Combine(ProjectDir, ResourceDirName, SpriteDirName, FramesDirName) + "\\"
                                + String.Format("{0}_" + String.Format("{{1:D{0}}}", frames.Count.ToString().Length) + ".png", sprite_name, 1);
                Root["Images"].AddFile(file_name, sprite_name);
            }

            public void AddSprite(string sprite_path, string sprite_name)
            {
                List<BitmapImage> frames = new List<BitmapImage>();
                frames.Add(new BitmapImage(new Uri(sprite_path, UriKind.Absolute)));
                uint sprite_duration = 9999;
                CreateDotSpriteFile(sprite_name, sprite_duration, 0, 0, (uint)frames.Count);
                CreateSpriteXMLFile(sprite_name);
                PopulateFrameDirectory(frames, sprite_name);
                AddGlobalAsset(SpriteDirName, sprite_name);

                //Adding file to virtual directory system
                var file_name = Path.Combine(ProjectDir, ResourceDirName, SpriteDirName, FramesDirName) + "\\"
                                + String.Format("{0}_" + String.Format("{{1:D{0}}}", frames.Count.ToString().Length) + ".png", sprite_name, 1);
                Root["Images"].AddFile(file_name, sprite_name);
                this.SaveVirtualDirectories(Root);
            }

            public void RemoveSprite(string sprite_name)
            {
                var path = Path.Combine(ProjectDir, ResourceDirName, SpriteDirName);
                File.Delete(path + sprite_name + ".sprite");
                foreach (var file in Directory.EnumerateFiles(path, String.Format("{0}.*", sprite_name)))
                {
                    File.Delete(file);
                }
                RemoveGenericAsset(SpriteDirName, sprite_name);
            }
        }
    }
}