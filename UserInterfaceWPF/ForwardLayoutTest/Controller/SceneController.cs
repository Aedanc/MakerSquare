using MakerSquare.FrontFacingECS;
using MakerSquare.SavingSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using FS = MakerSquare.FileSystem;

namespace ForwardLayoutTest.Controller
{
    public class SceneController
    {
        private MainWindow _window;
        private EntitiesController _entitiesController;
        private FS.Manager _fsManager;
        private BinaryFormatter _formatter;

        public SceneController(MainWindow window)
        {
            _window = window;
            _entitiesController = _window.entitiesController;
            _fsManager = FS.Manager.Instance;
            _formatter = new BinaryFormatter();
        }

        public void LoadScene(string name)
        {
            try
            {
                var scene = LoadFFScene(name);
                var rect = new System.Windows.Shapes.Rectangle();
                foreach (var item in _entitiesController.CanvasValues())
                    item.Select();

                _entitiesController.DeleteSelectedEntities();
                _window.RemoveEntitiesFromList(_window.dataListEntities);

                foreach (Entities.Entity entity in scene.entities)
                {					
                    entity.DeserializeData();
                    entity.originalImage.Name = "_" + entity.entity_id.ToString();
                    entity.canvas_image_data.image_id = entity.originalImage.Name;			    
					_entitiesController.AddCanvasEntity(entity.Name, entity);
					_entitiesController.generator.Update();
					_window.AddEntityToList(entity);					
                    Canvas.SetLeft(entity.originalImage, entity.Transform.x);
                    Canvas.SetTop(entity.originalImage, entity.Transform.y);                    
                }                
            }
            catch (Exception exception)
            {
#if DEBUG
                throw exception;
#else
                MessageBox.Show("Il semblerait qu'il y ait eu une erreur durant l'ouverture de votre scène.", "Erreur d'Ouverture de scène");
#endif
            }
            finally
            {
                _entitiesController.UnselectAll();
            }
        }

        public void SaveSceneAs(string name)
        {
            try
            {
                FFScene scene = new FFScene(_entitiesController.GetCanvasEntities());
                SaveFFScene(scene, name);
            }
            catch (Exception exception)
            {
#if DEBUG
                throw exception;
#else
                MessageBox.Show("Il semblerait qu'il y ait eu une erreur durant la sauvegarde de votre scène.", "Erreur de Sauvegarde");
#endif
            }
        }

        private FFScene LoadFFScene(string scene_name)
        {
            var filename = Path.Combine(scene_name);
            var FFScene = new FFScene(new List<FFEntity>());
            try
            {
                var file = File.Open(filename, FileMode.Open);
                try
                {
                    FFScene = (FFScene)_formatter.Deserialize(file);
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

            return FFScene;            
        }

        private void SaveFFScene(FFScene scene, string scene_name)
        {
            var filename = scene_name;
            var FFScene = new FFScene(_entitiesController.GetCanvasEntities());
            try
            {
                var file = File.Open(filename, FileMode.Create);
                try
                {
                    _formatter.Serialize(file, FFScene);
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
    }
}
