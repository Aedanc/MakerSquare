using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using Engine;
using Engine.System.Audio;
using Engine.System.Graphics;
using Engine.System.UI;
using MakerSquare.FileSystem;
using MakerSquare.SavingSystem;
using Microsoft.Xna.Framework;
using Game = Engine.Core.Game;
using Vector4 = Ultraviolet.Vector4;

namespace EngineDemo
{    
    class Program
    {
        static void AddSantaInResources(Manager filesystem)
        {
            string image_root = @"DemoAssets\";
            string[] image_names = {"tile002.png", "tile003.png",
                "tile004.png", "tile005.png", "tile006.png", "tile007.png", "tile008.png", "tile009.png"};

            List<BitmapImage> frames = new List<BitmapImage>();

            foreach (var image in image_names)
            {
                BitmapImage bImage = new BitmapImage(new Uri(Path.Combine(image_root, image), UriKind.Relative));
                frames.Add(bImage);
            }

            filesystem.AddSprite(frames, "Santa", 200);
            frames.Clear();

            // Ball sprite
            var ball = new BitmapImage(new Uri(Path.Combine(image_root, "little_circle.png"), UriKind.Relative));
            frames.Add(ball);
            filesystem.AddSprite(frames, "Ball", 1);
            frames.Clear();

            // Bat sprite
            var bat = new BitmapImage(new Uri(Path.Combine(image_root, "bat.png"), UriKind.Relative));
            frames.Add(bat);
            filesystem.AddSprite(frames, "Bat", 1);
            frames.Clear();

            // Background sprite
            var bg = new BitmapImage(new Uri(Path.Combine(image_root, "cloud_background_small.png"), UriKind.Relative));
            frames.Add(bg);
            filesystem.AddSprite(frames, "CloudBackgroundSmall", 1);
            frames.Clear();


            // Bricks sprite
            var bricks = new BitmapImage(new Uri(Path.Combine(image_root, "SuperMarioBrick.png"), UriKind.Relative));
            frames.Add(bricks);
            filesystem.AddSprite(frames, "Brick", 1);
            frames.Clear();
            
            var brickVert = new BitmapImage(new Uri(Path.Combine(image_root, "SuperMarioBrickVert.png"), UriKind.Relative));
            frames.Add(brickVert);
            filesystem.AddSprite(frames, "BrickVert", 1);
            frames.Clear();
            
            var bigCircle = new BitmapImage(new Uri(Path.Combine(image_root, "circle.png"), UriKind.Relative));
            frames.Add(bigCircle);
            filesystem.AddSprite(frames, "BigCircle", 1);
            frames.Clear();
            
            var littleCirle =  new BitmapImage(new Uri(Path.Combine(image_root, "little_circle.png"), UriKind.Relative));
            frames.Add(littleCirle);
            filesystem.AddSprite(frames, "LittleCircle", 1);
            frames.Clear();
        }


        static void AddBallInResources(Manager filesystem)
        {
            string image_root = @"DemoAssets\";
            string[] image_names = {"little_circle.png"};
            List<BitmapImage> frames = new List<BitmapImage>();
        }

        static void AddSabatonInResources(Manager filesystem)
        {
            string song_root = @"DemoAssets\";
            string sabaton = "SabatonTheLastStand.mp3";
            string nyancat = "NyanCat.mp3";
            filesystem.AddSong(Path.Combine(song_root, sabaton), "SabatonTheLastStand");
            filesystem.AddSong(Path.Combine(song_root, nyancat), "NyanCat");
        }


        public static void CreateGameScene(Game game)
        {
            var bg_ent = new Entity(true);
            bg_ent.Transform.x = 0;
            bg_ent.Transform.y = 0;
            bg_ent.AddComponent(new SpriteComponent(bg_ent, "CloudBackgroundSmall", true));
            bg_ent.AddComponent(new AudioComponent(bg_ent, "NyanCat", false, false, true));

            Bat bat = new Bat();
            Ball ball = new Ball();
            Breakout breakout = new Breakout(1, 0, ball);

        /*    breakout.CreateBricks(200, 200, "Brick", 100);
            breakout.CreateBricks(200, 400, "Brick", 100);
            breakout.CreateBricks(200, 600, "Brick", 100);
            breakout.CreateBricks(200, 800, "Brick", 100);
            */

           var invisibleWallLeft = new InvisibleWall(new Vector2(0, 0), new Vector2(5, 980));
            var invisibleWallRight = new InvisibleWall(new Vector2(640, 0), new Vector2(5, 980));
            var invisibleWallTop =  new InvisibleWall(new Vector2(0, 0), new Vector2(1240, 5));
            var invisibleWallDown = new InvisibleWall(new Vector2(0, 480), new Vector2(1240, 5), true);


            var bricks = BrickArenaBuilder.CreateNew().Init().Build();
            var santa_ent = SantaBuilder.CreateNew().Init(game).Build();
               
            var camera_ent = new Entity(true);

            /*    var cameraInput = new InputComponent(camera_ent);
                ActionDelegate cam_up = (entity, dict) => { CameraManager.MoveCameraY(-1); };
                ActionDelegate cam_down = (entity, dict) => { CameraManager.MoveCameraY(1); };
                ActionDelegate cam_right = (entity, dict) => { CameraManager.MoveCameraX(1); };
                ActionDelegate cam_left = (entity, dict) => { CameraManager.MoveCameraX(-1); };
    
                cameraInput.AddAction("cam_up", OnKey.DOWN, Key.Z, null, cam_up);
                cameraInput.AddAction("cam_down", OnKey.DOWN, Key.S, null, cam_down);
                cameraInput.AddAction("cam_right", OnKey.DOWN, Key.D, null, cam_right);
                cameraInput.AddAction("cam_left", OnKey.DOWN, Key.Q, null, cam_left);
                camera_ent.AddComponent(cameraInput);*/

            var save_manager = new SavedFilesManager(Manager.Instance);
            var PreSave = new List<Entity> {bg_ent, 
                camera_ent, invisibleWallTop, invisibleWallDown, invisibleWallLeft, invisibleWallRight, breakout, ball, bat};
            //  PreSave.AddRange(bricks);
            PreSave.AddRange(breakout.wall);
            var scene = new SavedScene();
            scene.entities = PreSave;
            save_manager.SaveScene(scene, "game_scene");
        }

        public static void CreateMenuScene(Game game)
        {
            var jouer_entity = new Entity(true);
            jouer_entity.Name = "jouer";
            jouer_entity.Transform.x = 300;
            jouer_entity.Transform.y = 200;
            var jouer_cmp = new Engine.System.UI.ImageButtonComponent(jouer_entity);
            var jouer_img_asset = new Engine.System.UI.ImageAsset();
            jouer_img_asset.ContentName = "bouton_jouer";
            jouer_cmp.onclick.action = (entity, context) => { Embed.LoadScene("game_scene", Engine.Core.Game.Instance); };

            jouer_cmp.AddAsset(jouer_img_asset);
            jouer_entity.AddComponent(jouer_cmp);

            var quitter_entity = new Entity(true);
            quitter_entity.Name = "quitter";
            quitter_entity.Transform.x = 288;
            quitter_entity.Transform.y = 275;
            var quitter_cmp = new ImageButtonComponent(quitter_entity);
            var quitter_img_asset = new ImageAsset();
            quitter_img_asset.ContentName = "bouton_quitter";
            quitter_cmp.AddAsset(quitter_img_asset);
            jouer_cmp.onclick.contextRefs = null;
            quitter_cmp.onclick.action = (entity, context) => { Game.Instance.Exit(); };
            quitter_entity.AddComponent(quitter_cmp);

            var text_entity = new Entity(true);
            text_entity.Name = "text";
            text_entity.Transform.x = 230;
            text_entity.Transform.y = 100;
            var text_cmp = new UITextComponent(text_entity);
            text_cmp.Text = "Makers² : le jeu démo !";
            text_cmp.Color = new Vector4(0, 0, 0, 1);
            text_cmp.FontSize = 36;


            var font_asset = new FontAsset();
            font_asset.ContentName = "coolvetica";
            text_cmp.AddAsset(font_asset);
            text_entity.AddComponent(text_cmp);

            var save_manager = new SavedFilesManager(Manager.Instance);
            var PreSave = new List<Entity> { jouer_entity, quitter_entity, text_entity };            
            var scene = new SavedScene();
            scene.entities = PreSave;
            save_manager.SaveScene(scene, "menu_scene");
        }

        static void Main()
        {
            var temp_file = Path.Combine(Directory.GetCurrentDirectory(), "Content");
            if (Directory.Exists(temp_file))
                Directory.Delete(temp_file, true);

            Manager.Initialize(temp_file);
            Manager filesystem = Manager.Instance;
            filesystem.AddFont(@"DemoAssets\coolvetica rg.ttf", "coolvetica");            
            filesystem.AddTexture(@"DemoAssets\bouton_jouer.png", "bouton jouer");
            filesystem.AddTexture(@"DemoAssets\bouton_quitter.png", "bouton quitter");

            AddSabatonInResources(filesystem);
            AddSantaInResources(filesystem);
                        
            using (var game = Game.Instance)
            {
                CreateMenuScene(game);
                CreateGameScene(game);

                Embed.LoadScene("game_scene", game);
                game.Run();
            }
            Directory.Delete(temp_file, true);
        }
    }
}