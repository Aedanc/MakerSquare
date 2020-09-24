using Engine.Components;
using System;
using System.Runtime.Serialization;
using Ultraviolet;
using Ultraviolet.Graphics.Graphics2D;

namespace Engine.System.Graphics
{
    [Serializable]
    public class SpriteComponent : Component, LoadComponent
    {
        [NonSerialized]
        private SpriteAnimationController _controller;
        public SpriteAnimationController Controller { get { return _controller; } private set { _controller = value; } }

        [NonSerialized]
        private Sprite _sprite;
        public Sprite Sprite { get { return _sprite; } private set { _sprite = value; } }
        public string Name { get; private set; }

        public SpriteComponent(Entity entity, string name, bool loadOnEngineInit=false) : base(entity)
        {
            Name = name;
            Controller = new SpriteAnimationController();
            if (!loadOnEngineInit)
                LoadData();
        }

        public void LoadData()
        {
            if (Sprite == null)
                LoadSprite();
        }
        
        private void LoadSprite()
        {
            Sprite = ContentManagement.ContentManager.LoadSprite(Name);
            Controller = Sprite[Name].Controller;
        }        
    }
}
