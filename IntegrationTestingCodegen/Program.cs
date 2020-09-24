using MakerSquare.Compiler;
using MakerSquare.FileSystem;
using MakerSquare.FrontFacingECS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;
using Codegen;

namespace Program
{ 
    class Program
    {
        static void AddSantaInResources(Manager filesystem)
        {
            string image_root = @"..\..\..\EngineDemo\Resources\";
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
            var bg = new BitmapImage(new Uri(Path.Combine(image_root, "background.jpg"), UriKind.Relative));
            frames.Add(bg);
            filesystem.AddSprite(frames, "Background_Snow", 1);
        }

        public static void AddDependencies(Compiler compiler)
        {
            compiler.AddDependency(@"Engine.dll");
            compiler.AddDependency(@"SavingSystem.dll");
            compiler.AddDependency(@"FileSystemManager.dll");
            compiler.AddDependency(@"Ultraviolet.Shims.Desktop.dll");
            compiler.AddDependency(@"Ultraviolet.BASS.dll");
            compiler.AddDependency(@"Ultraviolet.Core.dll");
            compiler.AddDependency(@"Ultraviolet.OpenGL.dll");
            compiler.AddDependency(@"Ultraviolet.OpenGL.Bindings.dll");
            compiler.AddDependency(@"Ultraviolet.SDL2.dll");
            compiler.AddDependency(@"Ultraviolet.ImGuiViewProvider.dll");
            compiler.AddDependency(@"Ultraviolet.FreeType2.dll");
            compiler.AddDependency(@"Ultraviolet.dll");
        }

        static void Main(string[] args)
        {
            try
            {
                //setup the environment
                Manager.Initialize(Path.Combine(Directory.GetCurrentDirectory(), "Content"));
                Manager fileManager = Manager.Instance;
                //AddSantaInResources(fileManager);
                //MK2Path path;
                //path.RelativePath = "Resources/Sprites/Santa.sprite";
                //path.ResourceFolder = "Sprites";
                //path.ResourceName = "Santa";

                Compiler compiler = new Compiler();

                //Generate the Santa.cs in Prefabs
                FFEntity entity = new FFEntity("Klaus");
                entity.Transform.x = 100;
                entity.Transform.y = 100;
                entity.Components.Add(new FFSpriteComponent(entity, "Santa"));
                var FFEntities = new List<FFEntity>();
                FFEntities.Add(entity);

                Codegen.Codegen.CodegenScene(fileManager, FFEntities, compiler);

                //Compile with the demo main : AddFiles should be made automatically.
                
                AddDependencies(compiler);
                compiler.SetExecutableName("test_codegen");
                compiler.SetExecPath(".");
                compiler.CompileToPortableExec();

                Process.Start("test_codegen.exe");
            }
            //Cleanup in case of error
            catch (System.Exception e)
            {                
                throw e;
            }
        }
    }
}
