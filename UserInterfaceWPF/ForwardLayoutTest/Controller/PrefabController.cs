using Entities;
using ForwardLayoutTest.XAML.Components;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace ForwardLayoutTest.Controller
{
    public enum PrefabEnum
    {
        BREAKOUT
    }

    public class PrefabMetadata
    {
        public PrefabMetadata(PrefabEnum prefab_, MenuItem menuItem_)
        {
            prefab = prefab_;
            menuItem = menuItem_;
            entities = new List<Entity>();
        }

        public PrefabEnum prefab;
        public MenuItem menuItem;
        public List<Entity> entities; 
    }

    public class PrefabController
    {
        EntitiesController _entitiesController;
        MainWindow _mainWindow;
        List<PrefabMetadata> _metadatas;
        static bool _initialized;

        private static PrefabController _instance;
        public static PrefabController Instance
        {
            get
            {
                if (!_initialized)
                    return null;
                else
                    return _instance;
            }
        }

        public static void Init(MainWindow window)
        {
            if (!_initialized)
            {
                _instance = new PrefabController(window);
                _initialized = true;
            }
        }
        
        private PrefabController(MainWindow window)
        {
            _mainWindow = window;
            _entitiesController = window.entitiesController;

            _metadatas = new List<PrefabMetadata> {
                new PrefabMetadata (PrefabEnum.BREAKOUT, _mainWindow.BreakoutMenuEntry),
            };
        }

        public void CreatePrefab(PrefabEnum prefab)
        {
            switch (prefab)
            {
                case PrefabEnum.BREAKOUT:
                    CreateBreakout();
                    break;
                default:
                    break;
            }
        }        

        public void DeletePrefab(PrefabEnum prefab)
        {
            var prefab_entities = _metadatas.First(x => x.prefab == prefab).entities;
            prefab_entities.ForEach(x => _entitiesController.DeleteEntity(x));
            _mainWindow.RemoveEntitiesFromList(prefab_entities);        
            _metadatas.First(x => x.prefab == prefab).menuItem.IsChecked = false;
        }

        public PrefabMetadata GetMetadata(PrefabEnum prefab)
        {
            return _metadatas.Find(x => x.prefab == prefab);
        }

        private void CreateBreakout()
        {
            Entity breakoutManager = new Entity("Gestionnaire_CasseBriques");
            breakoutManager.entity_id = _entitiesController.generator.NewID();
            breakoutManager.originalImage = new Image();
            breakoutManager.AddComponent(new BreakoutComponentControl(breakoutManager));            
            breakoutManager.originalImage.Name = "_" + breakoutManager.entity_id.ToString();
            breakoutManager.canvas_image_data.image_id = breakoutManager.originalImage.Name;
            _entitiesController.AddCanvasEntity(breakoutManager.Name, breakoutManager);
            _entitiesController.generator.Update();
            _mainWindow.AddEntityToList(breakoutManager);
            Canvas.SetLeft(breakoutManager.originalImage, breakoutManager.Transform.x);
            Canvas.SetTop(breakoutManager.originalImage, breakoutManager.Transform.y);
            breakoutManager.SetImage("pack://application:,,,/Src/letter-e124.png");

            Entity breakoutBat = new Entity("Balle_CasseBriques");
            breakoutBat.entity_id = _entitiesController.generator.NewID();
            breakoutBat.originalImage = new Image();
            breakoutBat.AddComponent(new BreakoutBatComponentControl(breakoutBat));            
            breakoutBat.originalImage.Name = "_" + breakoutBat.entity_id.ToString();
            breakoutBat.canvas_image_data.image_id = breakoutBat.originalImage.Name;
            _entitiesController.AddCanvasEntity(breakoutBat.Name, breakoutBat);
            _entitiesController.generator.Update();
            _mainWindow.AddEntityToList(breakoutBat);
            Canvas.SetLeft(breakoutBat.originalImage, breakoutBat.Transform.x);
            Canvas.SetTop(breakoutBat.originalImage, breakoutBat.Transform.y);
            breakoutBat.SetImage("pack://application:,,,/Src/letter-e124.png");

            Entity breakoutBall = new Entity("Batte_CasseBriques");
            breakoutBall.entity_id = _entitiesController.generator.NewID();
            breakoutBall.originalImage = new Image();
            breakoutBall.AddComponent(new BreakoutBallComponentControl(breakoutBall));            
            breakoutBall.originalImage.Name = "_" + breakoutBall.entity_id.ToString();
            breakoutBall.canvas_image_data.image_id = breakoutBall.originalImage.Name;
            _entitiesController.AddCanvasEntity(breakoutBall.Name, breakoutBall);
            _entitiesController.generator.Update();
            _mainWindow.AddEntityToList(breakoutBall);
            Canvas.SetLeft(breakoutBall.originalImage, breakoutBall.Transform.x);
            Canvas.SetTop(breakoutBall.originalImage, breakoutBall.Transform.y);
            breakoutBall.SetImage("pack://application:,,,/Src/letter-e124.png");


            GetMetadata(PrefabEnum.BREAKOUT).entities.Add(breakoutBall);
            GetMetadata(PrefabEnum.BREAKOUT).entities.Add(breakoutBat);
            GetMetadata(PrefabEnum.BREAKOUT).entities.Add(breakoutManager);
        }
    }
}
