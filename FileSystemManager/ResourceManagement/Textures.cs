using System;
using System.IO;
using System.Xml;

namespace MakerSquare
{
    namespace FileSystem
    {
        public partial class Manager
        {
            public string TextureDirName { get { return "Textures"; } }

            public void AddTexture(string texture_path, string texture_name)
            {
                AddGenericAsset(TextureDirName, texture_path, texture_name);
            }

            public void RemoveTexture(string texture_name)
            {
                RemoveGenericAsset(TextureDirName, texture_name);
            }
        }
    }
}