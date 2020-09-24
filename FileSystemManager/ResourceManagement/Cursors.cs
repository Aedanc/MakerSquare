using System.Windows.Media.Imaging;
using System.IO;
using System.Xml;
using System;

namespace MakerSquare
{
    namespace FileSystem
    {
        public partial class Manager
        {
            public string CursorDirName { get { return "Cursors"; } }            

            protected XmlNode CreateXmlCursor(string cursor_path, string cursor_name, XmlDocument doc)
            {                
                doc.DocumentElement.SetAttribute("Image", Path.GetFileNameWithoutExtension(cursor_path));
                var child = doc.CreateElement("Cursor");
                using (FileStream fs = new FileStream(cursor_path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    BitmapSource img = BitmapFrame.Create(fs);
                    BitmapMetadata md = (BitmapMetadata)img.Metadata;
                    child.SetAttribute("Name", "Normal");
                    child.SetAttribute("Position", "0 0");
                    child.SetAttribute("Size", img.PixelHeight + " " + img.PixelWidth);
                    child.SetAttribute("Hotspot", "0 0");
                }
                return child;
            }

            public void AddCursorImage(string cursor_path, string cursor_name)
            {
                File.Copy(cursor_path, Path.Combine(ProjectDir, ResourceDirName, CursorDirName, cursor_name + Path.GetExtension(cursor_path)));                
                var CursorXmlPath = Path.Combine(ProjectDir, ResourceDirName, CursorDirName, cursor_name + ".xml");                
                var doc = new XmlDocument();
                doc.PreserveWhitespace = true;                
                doc.LoadXml("<?xml version=\"1.0\"?> \n" +
                            "<Cursors>\n</Cursors>");
                var node = CreateXmlCursor(cursor_path, cursor_name, doc);
                doc.DocumentElement.AppendChild(node);
                doc.Save(CursorXmlPath);
                AddGlobalAsset("Cursors", cursor_name);
            }

            public void RemoveCursor(string cursor_name)
            {
                var path = Path.Combine(ProjectDir, ResourceDirName, CursorDirName);                
                foreach (var file in Directory.EnumerateFiles(path, String.Format("{0}.*", cursor_name)))
                {
                    File.Delete(file);
                }
                RemoveGlobalAsset("Cursors", cursor_name);
            }
        }
    }
}