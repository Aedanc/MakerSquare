using Engine.Components;
using System;
using Ultraviolet.Audio;


namespace Engine.System.Audio
{
    [Serializable]
    public class AudioComponent : Component, LoadComponent
    {
        [NonSerialized]
        private Song _song;
        public Song Song { get { return _song; } private set { _song = value; } }
        public string Name { get; private set; }
        public bool Loop { get; private set; }
        [NonSerialized]
        public SongPlayer player;
        public bool Playable = false;
        public bool Played = false;
        public bool Added = false;

        public AudioComponent(Entity entity, string name, bool loop, bool playable, bool loadOnEngineInit = false) : base(entity)
        {
            Name = name;
            Loop = loop;
            Playable = playable;
            if (!loadOnEngineInit)
                LoadData();
        }

        public void LoadData()
        {
            if (Song == null)
                LoadSong();
        }

        private void LoadSong()
        {
            Song = ContentManagement.ContentManager.LoadSong(Name);
        }

        public void Play()
        {
            Played = true;
            player = SongPlayer.Create();
            player.Play(Song, Loop);
        }
    }
}
