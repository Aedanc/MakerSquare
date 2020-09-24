using System;
using System.Diagnostics;
using Engine;
using Engine.System.Collision;
using Engine.System.Rule;
using Microsoft.Xna.Framework;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;


namespace EngineDemo
{
    [Serializable]
    public class InvisibleWall : Entity
    {
        public InvisibleWall(Vector2 pos, Vector2 size, bool destroyBallHandler = false) : base(true)
        {
            //Initial Placement
            Transform.x = pos.X;
            Transform.y = pos.Y;
            
            //Component
            AddComponent(new RuleComponent(this));
            var builder = CollisionComponentBuilder.CreateNew();
            builder.Init(this, size)
                .SetBodyType(BodyType.Static)
                .SetCategoriesCollision(Category.Cat2)
                .SetFriction(0f)
                .SetCollidesWith(Category.All);
            if (destroyBallHandler)
                builder.AddOnCollisionEventHandler(delegate(Fixture sender, Fixture other, Contact contact)
                {
                    EntityManager.GetAllEntities().Find(i => i.Guid == (Guid)other.Body.Tag).marked_for_deletion = true;
                    ((Breakout)EntityManager.GetEntity("Breakout")).nbBall -= 1;
                    return true;
                });
            AddComponent(builder.Build());
        }
    }
}