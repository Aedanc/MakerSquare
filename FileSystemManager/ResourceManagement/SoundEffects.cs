using System;
using System.IO;
using System.Xml;

namespace MakerSquare
{
    namespace FileSystem
    {
        public partial class Manager
        {
            public string SoundEffectDirName { get { return "SoundEffects"; } }

            public void AddSoundEffect(string soundEffect_path, string soundEffect_name)
            {
                AddGenericAsset(SoundEffectDirName, soundEffect_path, soundEffect_name);
            }

            public void RemoveSoundEffect(string soundEffect_name)
            {
                RemoveGenericAsset(SoundEffectDirName, soundEffect_name);
            }
        }
    }
}