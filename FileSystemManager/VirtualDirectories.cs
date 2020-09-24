using System;
using System.Collections.Generic;

namespace MakerSquare.FileSystem
{
    [Serializable]
    public class VirtualDirectory
    {
        public VirtualDirectory(string dir_name, VirtualDirectory parent = null)
        {
            name = dir_name;
            this.parent = parent;
        }

        public string name;
        public VirtualDirectory parent = null;
        public List<VirtualDirectory> directories = new List<VirtualDirectory>();
        public List<VirtualFile> files = new List<VirtualFile>();

        public void AddFile(string real_path, string display_name)
        {
            var tmp_parent = parent;
            string virtual_path = name;
            if (name != "/")
                virtual_path = virtual_path + "/";
            while (tmp_parent != null)
            {
                if (tmp_parent.name != "/")
                    virtual_path = virtual_path.Insert(0, "/");
                virtual_path = virtual_path.Insert(0, tmp_parent.name);
                tmp_parent = tmp_parent.parent;
            }
            virtual_path = virtual_path.Insert(virtual_path.Length, display_name);
            files.Add(new VirtualFile(real_path, virtual_path, display_name));
        }

        public override string ToString() { return name; }

        public VirtualDirectory this[string name]
        {
            get { return directories.Find(dir => dir.name == name); }
        }
    }

    [Serializable]
    public class VirtualFile
    {
        public VirtualFile(string real_path, string virtual_path, string display_name)
        {
            RealFilePath = real_path;
            VirtualFilePath = virtual_path;
            FileDisplayName = display_name;
        }

        public string RealFilePath;
        public string VirtualFilePath;
        public string FileDisplayName;

        public override string ToString() { return RealFilePath; }
    }
}
