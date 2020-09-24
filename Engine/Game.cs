using Ultraviolet;
using Ultraviolet.OpenGL;
using Ultraviolet.Content;
using Ultraviolet.Audio;
using Ultraviolet.Core;
using System;
using Engine.System.Audio;
using FileSystemManager;

namespace Engine
{
    
    public class Game : UltravioletApplication
    {
        ContentManager contentManager;
        SongPlayer songPlayer;
        Song song;

        public Game() : base("Makers²", "Makers² Engine")
        {
        }

        protected override UltravioletContext OnCreatingUltravioletContext()
        {
            return new OpenGLUltravioletContext(this);
        }

        //Replace With correct Manager
        protected override void OnLoadingContent()
        {
            Console.WriteLine("Something");
            FileSystemManager fileSystemManager = new FileSystemManager("E:\rendu\\EIP\\MakersSquare\\AComponent");
            this.contentManager = ContentManager.Create("Content");
            LoadContentManifests(contentManager);
            AudioManager audioManager = new AudioManager();
            Song song = this.contentManager.Load<Song>(GlobalSongID.SabatonTheLastStand);
            audioManager.addSong(song);
           

           // this.song = TODO load song
            songPlayer = SongPlayer.Create();
            songPlayer.Play(song);

            base.OnLoadingContent();
        }

        protected override void OnUpdating(UltravioletTime time)
        {
            songPlayer.Update(time);

            if (Ultraviolet.GetInput().GetActions().ExitApplication.IsPressed())
            {
                Exit();
            }
            base.OnUpdating(time);
        }

        protected override void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                SafeDispose.Dispose(songPlayer);
                SafeDispose.Dispose(contentManager);
            }
            base.Dispose(disposing);
        }

        protected override void LoadContentManifests(ContentManager content)
        {
            base.LoadContentManifests(content);
            Ultraviolet.GetContent().Manifests["Global"]["Songs"].PopulateAssetLibrary(typeof(GlobalSongID));
        }
    }


}
