using System;
using Ultraviolet;
using Ultraviolet.Audio;
using Ultraviolet.Core;
using Ultraviolet.Graphics;
using Ultraviolet.Graphics.Graphics2D;

namespace Engine.System.ContentManagement
{
    public static class ContentManager
    {        
        private static Ultraviolet.Content.ContentManager content;
        private static UltravioletContext context;        

        public static void Initialize(UltravioletContext context_, Ultraviolet.Content.ContentManager content_)
        {
            content = content_;
            context = context_;
            Contract.Require(content, nameof(content));                        
            var manifests = context.GetContent().Manifests;
        }

        public static Ultraviolet.Content.ContentManager GetUVContent()
        {
            return content;
        }

        public static Texture2D LoadTexture2D(string name)
        {
            return content.Load<Texture2D>("Textures\\" + name + ".png");
        }

        public static UltravioletFont LoadFont(string name)
        {
            return content.Load<UltravioletFont>("TrueTypeFonts\\" + name + ".uvmeta");
        }

        public static Sprite LoadSprite(string name)
        {
            return content.Load<Sprite>("Sprites\\" + name + ".sprite");
        }

        public static Song LoadSong(string name)
        {            
            return content.Load<Song>("Songs\\" + name + ".wav");
        }
  
    }
}
