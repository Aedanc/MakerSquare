using System;
using Engine;
using Engine.System.Rule;
using Engine.System.Collision;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace EngineDemo
{
    [Serializable]
    public class Brick : Entity
    {
        public int ScoreGiven;

        public Brick(int posx, int posy, string NameSprite, int Score, Breakout breakout) : base(true)
        {
            // initial placement
            Transform.x = posx;
            Transform.y = posy;
            AddComponent(new Engine.System.Graphics.SpriteComponent(this, NameSprite, true));
            AddComponent(new RuleComponent(this));
            AddComponent(CollisionComponentBuilder.CreateNew()
                .Init(this, new Vector2(60, 30))
                .SetBodyType(BodyType.Static)
                .SetCollidesWith(Category.Cat1)
                .SetCategoriesCollision(Category.Cat2)
                .SetFriction(0f)
                .AddOnCollisionEventHandler(delegate(Fixture sender, Fixture other, Contact contact)
                {
                    other.Body.LinearVelocity = new Vector2(other.Body.LinearVelocity.X * 400, other.Body.LinearVelocity.Y * 400);
                    return true;
                })
                .Build());
            breakout.wall.Add(this);
            breakout.nbBrick += 1;
            Name = "Brick" + breakout.nbBrick;
            ScoreGiven = Score;
            var msg = new RuleManager.EngineMessage();
            msg.entitybase = EntityManager.GetEntity("Ball");
            msg.entityFocus = this;
            msg.Action = RuleManager.ActionEngine.Collision;

            GetComponent<RuleComponent>().WatcherActionEngine(msg, BreakBricks, "brick", this, "breakout");
        }
       
        public void AddScore(string EntityWatched, Entity EntityWatcher, string VarName)
        {
            ((Breakout)EntityWatcher).score += ScoreGiven;
            Console.WriteLine("Score : " + ((Breakout)EntityWatcher).score);
       }

        public void BreakBricks(string EntityWatched, Entity EntityWatcher, string VarName)
        {
            this.AddScore(EntityWatched, EntityWatcher, VarName);
            ((Breakout)EntityWatcher).nbBrick -= 1;
            this.marked_for_deletion = true;
        }

    }
}
