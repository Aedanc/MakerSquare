using Engine.System.Audio;
using Engine.System.Camera;
using Engine.System.Collision;
using Engine.System.Graphics;
using Engine.System.Input;
using Engine.System.Movement;
using Engine.System.Rule;
using Engine.System.UI;
using tainicom.Aether.Physics2D.Dynamics;
using Ultraviolet;
using Ultraviolet.Content;
using Ultraviolet.Core;
using Ultraviolet.FreeType2;
using Ultraviolet.ImGuiViewProvider;
using Ultraviolet.OpenGL;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Engine.Core
{
    public class Game : UltravioletApplication
    {
        private ContentManager content;

        private static Game _instance;

        public World world = new World(Vector2.Zero);

        private Game() : base("MK²", "XUNTOS") { }

        public static Game Instance
        {
            get
            {
                if (_instance == null)
                {

                    _instance = new Game();
                }
                return _instance;
            }
        }


        protected override UltravioletContext OnCreatingUltravioletContext()
        {            
            var configuration = new OpenGLUltravioletConfiguration();
            PopulateConfiguration(configuration);
            configuration.Plugins.Add(new ImGuiPlugin());
            configuration.Plugins.Add(new FreeTypeFontPlugin());
            return new OpenGLUltravioletContext(this, configuration);
        }        

        protected override void OnLoadingContent()
        {
           content = ContentManager.Create("Content\\Resources");
           System.ContentManagement.ContentManager.Initialize(Ultraviolet, content);            
           GUIManager.Initialize(Ultraviolet, content);
           GraphicManager.Initialize(Ultraviolet);
           CollisionManager.Initialize(world);
           CameraManager.Initialize(Ultraviolet);
           AudioManager.Initialize(Ultraviolet);
           InputManager.Initialize();
           MovementManager.Initialize();
           RuleManager.Initialize();
           base.OnLoadingContent();
           EntityManager.PostContentLoad();
           GUIManager.FetchGuiComponents();
           GUIManager.DisplayGuiScreen();
        }

        protected override void OnUpdating(UltravioletTime time)
        {
            EntityManager.RemoveMarkedEntities();
            GUIManager.Update();
            CollisionManager.SetCurrentWorld(world);
            AudioManager.FetchSongComponents();
            AudioManager.PlaySong();
          
            CollisionManager.FetchCollisionComponent();
            CollisionManager.UpdateComponents();
            MovementManager.FetchMovementComponent();
            MovementManager.UpdateMovement();
            InputManager.OnUpdateEffectInputs();
            CollisionManager.OnUpdateCollision(time);
            MovementManager.UpdateCorrectionMovement();
            CollisionManager.UpdatePositionEntities();
            GraphicManager.FetchSpriteComponents();
            GraphicManager.UpdateSprites(time);
            RuleManager.UpdateRule();
            base.OnUpdating(time);
        }

        protected override void OnDrawing(UltravioletTime time)
        {
            GraphicManager.DrawSprites(time);
            base.OnDrawing(time);
        }

        protected void LoadContentManifests()
        {            
            var uvContent = Ultraviolet.GetContent();            
            Contract.Require(content, nameof(content));
            
            var contentManifestFiles = content.GetAssetFilePathsInDirectory("Manifests");
            uvContent.Manifests.Load(contentManifestFiles);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
        }

    }
}
