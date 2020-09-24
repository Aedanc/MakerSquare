// This file is here to gather all functions that, would
// they not be here, cause circular dependencies.

using Engine;
using MakerSquare.SavingSystem;

namespace EngineDemo
{
    public class Embed
    {
        public static void LoadScene(string scene_name, Engine.Core.Game game, bool additively = false)
        {
            var save_manager = new SavedFilesManager(MakerSquare.FileSystem.Manager.Instance);
            var scene = new SavedScene();
            save_manager.LoadScene(out scene, scene_name);

            if (!additively)
                EntityManager.UnloadEntities();

            EntityManager.AddListEntityInList(scene.entities);
            if (game.IsActive)
                EntityManager.PostContentLoad();            
        }
    }
}
