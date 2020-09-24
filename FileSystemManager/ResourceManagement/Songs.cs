using System.IO;
using NAudio.Wave;

namespace MakerSquare
{
    namespace FileSystem
    {
        public partial class Manager
        {
            public string SongDirName { get { return "Songs"; } }

            private void ConvertAndMoveMusic(string song_path, string asset_path)
            {
                if (song_path.EndsWith(".mp3"))
                {
                    using (Mp3FileReader reader = new Mp3FileReader(song_path))                 
                        WaveFileWriter.CreateWaveFile(asset_path, reader);                    
                }
                else
                    File.Copy(song_path, asset_path);
            }

            public void AddSong(string song_path, string song_name)
            {
                var asset_path = Path.Combine(ProjectDir, ResourceDirName, SongDirName, song_name + ".wav");                
                ConvertAndMoveMusic(song_path, asset_path);                
                AddGlobalAsset(SongDirName, song_name + ".wav");
                Root["Musiques"].AddFile(asset_path, song_name);
                this.SaveVirtualDirectories(Root);
            }

            public void RemoveSong(string song_name)
            {
                RemoveGenericAsset(SongDirName, song_name);
            }

        }
    }
}