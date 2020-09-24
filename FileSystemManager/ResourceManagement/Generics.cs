using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace MakerSquare
{
    namespace FileSystem
    {
        public partial class Manager
        {
            public bool CheckNameAvailability(string name)
            {
                if (name.Length == 0)
                    return false;
                var doc = XDocument.Load(Path.Combine(ProjectDir, ManifestDir, "Global.Manifest"));                


                var nodes = doc.Descendants().Where(x => x.Name.LocalName == "Asset");
                return !nodes.Any(x => x.Attribute("Name").Value == name);                
            }

            private void AddGenericAsset(string asset_directory_name, string asset_path, string asset_name)
            {                
                File.Copy(asset_path, 
                    Path.Combine(ProjectDir, ResourceDirName, asset_directory_name, asset_name + Path.GetExtension(asset_path)));               
                AddGlobalAsset(asset_directory_name, asset_name + Path.GetExtension(asset_path));
            }

            private void RemoveGenericAsset(string asset_directory_name, string asset_name)
            {
                var path = Path.Combine(ProjectDir, ResourceDirName, asset_directory_name);
                foreach (var file in Directory.EnumerateFiles(path, String.Format("{0}.*", asset_name)))
                {
                    File.Delete(file);
                }
                RemoveGlobalAsset(asset_directory_name, asset_name);
            }
        }
    }
}
