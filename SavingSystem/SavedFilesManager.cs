using Engine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MakerSquare
{
    namespace SavingSystem
    {
        public class SavedFilesManager
        {
            private string _project_dir;
            private BinaryFormatter _formatter;

            public SavedFilesManager(FileSystem.Manager fs_manager)
            {
                _formatter = new BinaryFormatter();
                _project_dir = fs_manager.ProjectDir;
            }

            private void LoadSerializableItem<T>(out T item, string filename)
            {
                try
                {
                    var file = File.Open(filename, FileMode.Open);
                    try
                    {
                        item = (T)_formatter.Deserialize(file);
                    }
                    catch (Exception e)
                    {
                        throw new CannotLoadFileException("Could not deserialize the file", e);
                    }
                    finally
                    {
                        file.Close();
                    }
                }
                catch (Exception e)
                {
                    throw new CannotLoadFileException("Could not open the file", e);
                }
            }

            private void SaveSerializableItem<T>(T item, string filename)
            {
                try
                {
                    var file = File.Open(filename, FileMode.Create);
                    try
                    {
                        _formatter.Serialize(file, item);
                    }
                    catch (Exception e)
                    {
                        throw new CannotSaveFileException("Could not serialize the file", e);
                    }
                    finally
                    {

                        file.Close();
                    }
                }
                catch (Exception e)
                {
                    throw new CannotSaveFileException("Could not open the file", e);
                }
            }           

            public void LoadScene(out SavedScene scene, string scene_name)
            {
                LoadSerializableItem(out scene, Path.Combine(_project_dir, "Scenes", scene_name + ".scene"));
            }

            public void SaveScene(SavedScene scene, string scene_name)
            {
                SaveSerializableItem(scene, Path.Combine(_project_dir, "Scenes", scene_name + ".scene"));
            }

            public void LoadPrefab(out Entity entity, string prefab_name)
            {
                LoadSerializableItem(out entity, Path.Combine(_project_dir, "Prefabs", prefab_name + ".prefab"));
            }

            public void SavePrefab(Entity entity, string prefab_name)
            {
                SaveSerializableItem(entity, Path.Combine(_project_dir, "Prefabs", prefab_name + ".prefab"));
            }

            public void LoadConfig(out Config config)
            {
                LoadSerializableItem(out config, Path.Combine(_project_dir, "MakersSquare.config"));
            }

            public void SaveConfig(Config config)
            {
                SaveSerializableItem(config, Path.Combine(_project_dir, "MakersSquare.config"));
            }
        }
    }
}
