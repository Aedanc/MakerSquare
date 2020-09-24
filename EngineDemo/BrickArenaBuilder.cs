using System.Collections.Generic;
using System.Diagnostics;
using Engine;
using Engine.System.Collision;
using Engine.System.Graphics;
using Microsoft.Xna.Framework;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;

namespace EngineDemo
{
    public sealed class BrickArenaBuilder
    {
        private List<Entity> _bricks;
        
        private BrickArenaBuilder() {}
        
        public static BrickArenaBuilder CreateNew()
        {
            return new BrickArenaBuilder();
        }

        public BrickArenaBuilder Init()
        {
            _bricks = new List<Entity>();
            return this;
        }
        
        public static void CreateArena(List<Entity> bricks)
        {
           
            for (int x = 0; x <= 600; x += 60)
            {
                bricks.Add(CreateBricks(x, 450, false, BodyType.Static, Category.All, Category.Cat1));
                bricks.Add(CreateBricks(x, 0, false, BodyType.Static, Category.All, Category.Cat1));
            }
            for (int y = 0; y <= 420; y += 60)
            {
                bricks.Add(CreateBricks(0, y, true, BodyType.Static, Category.All, Category.Cat1));
                bricks.Add(CreateBricks(610, y, true, BodyType.Static, Category.All, Category.Cat1));
            }
        }
        
        public List<Entity> Build()
        {
            //  CreateArena(_bricks);
            _bricks.Add(CreateBricks(400, 200, false, BodyType.Static, Category.All, Category.Cat1));
            return _bricks;
        }
        
        private static Entity CreateBricks(int x, int y,
            bool vert,
            BodyType bodyType,
            Category collidesWith,
            Category collision,
            Vector2 force = default(Vector2),
            Vector2 linearVelocity = default(Vector2),
            float mass = 2,
            bool gravity = false, float restitution = 1f)
        {
            var entity = new Entity(true);
            entity.Transform.x = x;
            entity.Transform.y = y;
            //      entity.Name = "brick" + nbNameBrick;
            Debug.WriteLine(entity.Name);
            //    nbNameBrick += 1;
            Vector2 hitbox;
            if (vert)
            {
                entity.AddComponent(new SpriteComponent(entity, "BrickVert", true));
                hitbox = new Vector2(30, 60);
            } else
            {
                entity.AddComponent(new SpriteComponent(entity, "Brick", true));
                hitbox = new Vector2(60, 30);                
            }
            
            var fixtureCollisionEventHandlers = new List<OnCollisionEventHandler>();
            fixtureCollisionEventHandlers.Add(delegate(Fixture sender, Fixture other, Contact contact) {  
                other.Body.LinearVelocity = new Vector2(-100, -70);
                return true;
            });
            fixtureCollisionEventHandlers.Add(delegate(Fixture sender, Fixture other, Contact contact) {  
                other.Body.LinearVelocity = new Vector2(0, -170 );
                return true;
            });
            fixtureCollisionEventHandlers.Add(delegate(Fixture sender, Fixture other, Contact contact) {  
                other.Body.LinearVelocity = new Vector2(100, -70);
                return true;
            });

            CollisionComponent component = CollisionComponentBuilder.CreateNew()
                .Init(entity, hitbox)
                .SetBodyType(BodyType.Dynamic)
                .SetCollidesWith(collidesWith)
                .SetCategoriesCollision(collision)
                .SetIgnoreGravity(gravity)
                //  .SetIgnoreCCD(true)
                //.ApplyForce(force)
                //    .SetMass(mass)
                //  .SetInertia(0)
                //    .SetLinearVelocity(linearVelocity)
                .SetHitBoxDivide(3)
                .SetRestitution(restitution)
                .SetFixtureOnCollisionEventHandlers(fixtureCollisionEventHandlers)
                .Build();
            entity.AddComponent(component);
            
            return entity;
        }
    }
}