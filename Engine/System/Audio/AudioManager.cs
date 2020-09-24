using System.Collections.Generic;
using System.Diagnostics;
using Ultraviolet;
using Ultraviolet.Audio;

namespace Engine.System.Audio
{
    public class AudioManager
    {
        private static List<AudioComponent> _songs = new List<AudioComponent>();
        private static UltravioletContext _context;

    
        public static void Initialize(UltravioletContext context)
        {
            _context = context;
        }

        public static void AddAudioComponent(AudioComponent audioComponent)
        {
            _songs.Add(audioComponent);
        }

        public static void PlaySong()
        {
            foreach (var song in _songs)
                if (song.Playable && song.Played != true)
                    song.Play();
        }

        public static void FetchSongComponents()
        {
            foreach (Entity entity in EntityManager.GetAllEntities())
            {
                var comp = entity.GetComponent<AudioComponent>();
                if (comp != null && comp.Added != true)
                {
                    comp.Added = true;
                    AddAudioComponent(comp);
                }
            }
        }
    }
}
