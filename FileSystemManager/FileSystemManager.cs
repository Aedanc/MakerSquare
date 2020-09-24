using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;

namespace MakerSquare
{
    namespace FileSystem
    {
        public enum EFileType
        {
            NONE = -1,
            SOUND,
            SPRITE,
            FONT
        }

        public partial class Manager
        {
            public string ProjectDir { get; protected set; }

            public string ResourceDirName { get { return "Resources"; } }
            public string AssetDirName { get { return "Content"; } }
            public string PrefabsDirName { get { return "Prefabs"; } }
            public string SceneDirName { get { return "Scenes"; } }

            public VirtualDirectory Root { get; }
                       
            private static Manager instance;

            public static void Initialize(string project_dir)
            {
                instance = new Manager(project_dir);
            }

            public static Manager Instance
            {
                get
                {
                    return instance;
                }
            }

            private Manager(string project_dir)
            {            
                ProjectDir = project_dir;

                if (!isProjectFolder(project_dir))
                {
                    CreateDirectoryStructure(project_dir);
                    CreateVirtualDirectories(project_dir);
                    ManifestCreation();
                    CreateEditorFiles();
                }
                else
                {
                    GlobalManifestPath = Path.Combine(ProjectDir, ManifestDir, "Global.manifest");
                }
                Root = GetVirtualDirectories(project_dir);        
            }            

            protected string GetRelativePathTo(string path, string relative_dir)
            {
                var uri_base = new Uri(relative_dir);
                var uri_path = new Uri(path);

                return uri_base.MakeRelativeUri(uri_path).ToString();
            }

            private bool isProjectFolder(string directory)
            {
                return Directory.Exists(directory) && File.Exists(Path.Combine(directory, ManifestDir, "Global.manifest"));
            }

            private void CreateVirtualDirectories(string directory)
            {
                var vir_fs = new VirtualDirectory("/");
                vir_fs.directories.Add(new VirtualDirectory("Images", vir_fs));
                vir_fs.directories.Add(new VirtualDirectory("Musiques", vir_fs));
                vir_fs.directories.Add(new VirtualDirectory("Polices", vir_fs));
                var formatter = new BinaryFormatter();
                var file = File.Open(Path.Combine(directory, ".vir_fs"), FileMode.Create);
                try
                {
                    formatter.Serialize(file, vir_fs);
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
                finally
                {
                    file.Close();
                }                                
            }

            private VirtualDirectory GetVirtualDirectories(string directory)
            {
                var formatter = new BinaryFormatter();                
                var file = File.Open(Path.Combine(directory, ".vir_fs"), FileMode.Open);
                VirtualDirectory fs;
                try
                {
                    fs = (VirtualDirectory)formatter.Deserialize(file);
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
                finally
                {
                    file.Close();
                }
                return fs;
            }

            private void SaveVirtualDirectories(VirtualDirectory root)
            {
                var formatter = new BinaryFormatter();
                var file = File.Open(Path.Combine(ProjectDir, ".vir_fs"), FileMode.Open);
                try
                {
                    formatter.Serialize(file, root);
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
                finally
                {
                    file.Close();
                }
            }

            private void CreateDirectoryStructure(string directory)
            {
                try
                {
                    Directory.CreateDirectory(directory);
                    Directory.CreateDirectory(Path.Combine(directory, "Prefabs"));
                    Directory.CreateDirectory(Path.Combine(directory, "Scenes"));
                    var dir = Directory.CreateDirectory(Path.Combine(directory, "Resources"));
                    AddUIScreen(directory);
                }
                catch (Exception e)
                {
                    throw new CannotCreateProjectDirException("Could not create the project directory.", e);
                }
            }

            private void AddUIScreen(string directory)
            {
                Directory.CreateDirectory(Path.Combine(directory, @"Resources\UI"));
                var fs = File.OpenWrite(Path.Combine(directory, @"Resources\UI\GUIScreen.xml"));
                String str = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<UIPanelDefinition>\n  <View />\n</UIPanelDefinition>";
                var encoding = new UTF8Encoding();
                fs.Write(encoding.GetBytes(str), 0, encoding.GetByteCount(str));
                fs.Close();
            }            

            public List<VirtualFile> SearchForFileType(EFileType type)
            {
                List<VirtualFile> files = new List<VirtualFile>();
                Regex reg = null;

                if (type == EFileType.FONT)
                    reg = new Regex(@"\.ttf|\.otf|\.woff");
                else if (type == EFileType.SPRITE)
                    reg = new Regex(@"\.jpeg|\.png|\.bmp|\.jpg");
                else if (type == EFileType.SOUND)
                    reg = new Regex(@"\.mp3|\.wav|\.flac");

                if (reg == null)
                    return files;

                foreach (var file in Root.files)
                {
                    if (reg.IsMatch(file.RealFilePath))
                        files.Add(file);
                }
                foreach (var dir in Root.directories)
                {
                    foreach (var file in dir.files)
                    {
                        if (reg.IsMatch(file.RealFilePath))
                            files.Add(file);
                    }
                }

                return files;
            }
        }
    }
}