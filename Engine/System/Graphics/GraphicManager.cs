using System.Collections.Generic;
using System.Diagnostics;
using Engine.System.Collision;
using Ultraviolet;
using Ultraviolet.Content;
using Ultraviolet.Graphics;
using Ultraviolet.Graphics.Graphics2D;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Engine.System.Graphics
{
    public static partial class GlobalSpriteID
    {
        public static readonly AssetID Background_Snow;
    }

    public static class GraphicManager
    {
        private static bool _debugHitbox;
        private static UltravioletContext _context;
        private static List<SpriteComponent> _spritesToUpdate = new List<SpriteComponent>();
        private static List<SpriteComponent> _spritesToDraw = new List<SpriteComponent>();
        private static SpriteBatch _spriteBatch;

        public static void Initialize(UltravioletContext context)
        {
            _context = context;
            _spriteBatch = SpriteBatch.Create();
        }

        public static void QueueSpriteForUpdate(SpriteComponent sprite)
        {
            if (!_spritesToUpdate.Contains(sprite))
                _spritesToUpdate.Add(sprite);
        }

        public static void QueueSpriteForDrawing(SpriteComponent sprite)
        {
            if (!_spritesToDraw.Contains(sprite))
                _spritesToDraw.Add(sprite);
        }

        public static void DrawSprites(UltravioletTime time)
        {
            _spriteBatch.Begin(SpriteSortMode.BackToFront, Ultraviolet.Graphics.BlendState.AlphaBlend);
            foreach (var sprite in _spritesToDraw)
            {
                _spriteBatch.DrawSprite(sprite.Controller, sprite.Entity.Transform.ToVector2(), null, null, Color.White,
                    0, SpriteEffects.None, sprite.Entity.Transform.depth);
                if (_debugHitbox)
                    DisplayHitbox(sprite);
            }
            _spriteBatch.End();
            _spritesToDraw.Clear();
        }

        private static void DisplayHitbox(SpriteComponent sprite)
        {
            CollisionComponent component = sprite.Entity.GetComponent<CollisionComponent>();
            if (component != null)
            {
                if (component.debugTexture == null)
                {
                    component.debugTexture = Texture2D.CreateTexture(component.hitBoxWidth,component.hitBoxHeight, TextureOptions.SrgbColor);
                    component.debugTexture.SetData(component.colorDebug);
                }
                _spriteBatch.Draw(component.debugTexture, sprite.Entity.Transform.ToVector2(), Color.White);
            }
        } 

        public static void DebugHitbox()
        {
            if (_debugHitbox)
                _debugHitbox = false;
            else
                _debugHitbox = true;
        }

        public static void UpdateSprites(UltravioletTime time)
        {
            foreach (var sprite in _spritesToUpdate)
            {
                sprite.Controller.Update(time);
            }
            _spritesToUpdate.Clear();
        }

        public static void FetchSpriteComponents()
        {
            foreach (Entity entity in EntityManager.GetAllEntities())
            {
                var comp = entity.GetComponent<SpriteComponent>();
                if (comp != null)
                {
                    QueueSpriteForDrawing(comp);
                    QueueSpriteForUpdate(comp);
                }
            }
        }
    }
}