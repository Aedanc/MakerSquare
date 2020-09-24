using System;
using System.IO;
using System.Xml;

namespace MakerSquare
{
    namespace FileSystem
    {
        public partial class Manager
        {
            public string FontDirName { get { return "FreeTypeFonts"; } }

            public void AddFont(string font_path, string font_name)
            {
                AddGenericAsset(FontDirName, font_path, font_name);
                Root["Polices"].AddFile(Path.Combine(ProjectDir, ResourceDirName, SongDirName, font_name + Path.GetExtension(font_path)), font_name);
                this.SaveVirtualDirectories(Root);
            }

            public void RemoveFont(string font_name)
            {
                File.Delete(Path.Combine(ProjectDir, ResourceDirName, FontDirName, font_name));
            }

        }
    }
}