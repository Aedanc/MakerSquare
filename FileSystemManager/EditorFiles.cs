using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace MakerSquare.FileSystem
{
    public partial class Manager
    {
        public class EditorPreferences
        {
            public string LastSceneOpened = "";
        }

        public string ConfigFile { get { return ".config"; } }
        public string EditorScenesDirName { get { return "EditorScenes"; } }

        public EditorPreferences editorPreferences;

        public void CreateEditorFiles()
        {
            Directory.CreateDirectory(Path.Combine(ProjectDir, EditorScenesDirName));
            var file = File.Create(Path.Combine(ProjectDir, ConfigFile));
            file.Close();
            SavePreferences();
        }

        public void SavePreferences()
        {
            var json = JsonConvert.SerializeObject(editorPreferences);
            using (FileStream file = File.Open(Path.Combine(ProjectDir, ConfigFile), FileMode.Truncate))
            {
                file.Write(Encoding.UTF8.GetBytes(json), 0, json.Length);
            }
        }

        public void SaveEditorScene(byte[] scene, string scene_name)
        {
            using (FileStream file = File.Open(Path.Combine(ProjectDir, ConfigFile, scene_name), FileMode.Truncate))
            {
                file.Write(scene, 0, scene.Length);
            }
        }

        public byte[] GetEditorScene(string scene_name)
        {
            var scene = new List<byte>();
            using (FileStream file = File.Open(Path.Combine(ProjectDir, ConfigFile, scene_name), FileMode.Truncate))
            {
                var array = new byte[file.Length];
                file.Read(array, 0, (int)file.Length);
                scene.AddRange(array);
            }
            return scene.ToArray();
        }
    }
}
