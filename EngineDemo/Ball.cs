using Engine;
using Engine.System.Rule;
using Engine.System.Movement;
using System;
using Engine.System.Collision;
using tainicom.Aether.Physics2D.Dynamics;


namespace EngineDemo
{
    [Serializable]
    public class Ball : Entity
    {     
        public Ball() : base(true)
        {
            Name = "Ball";
            // Initial placement
            Transform.x = 400;
            Transform.y = 300;
            // Components
         
            AddComponent(new Engine.System.Graphics.SpriteComponent(this, "Ball", true));
            AddComponent(CollisionComponentBuilder.CreateNew()
                .Init(this, 25)
                .SetRestitution(1f)
                .SetCategoriesCollision(Category.Cat1)
                .SetCollidesWith(Category.All)
                .SetBodyType(BodyType.Dynamic)
                .SetFriction(0f)
                .Build());
            var mvt = new MovementComponent(this, false);
            AddComponent(new RuleComponent(this));
            AddComponent(mvt);
            
            
            //activation of the function to remove life
     /*       var msg = new RuleManager.EngineMessage();
            msg.entitybase = this;
            msg.entityFocus = borderscreen;
            msg.Action = RuleManager.ActionEngine.Collision;
            rulebrick.WatcherActionEngine(msg, removeLife, "Breakout", this, "nbBall");*/
        }
        public void RemoveLife(string EntityWatched, Entity EntityWatcher, string VarName)
        {
            GetComponent<RuleComponent>().SetVariablevalue(EntityWatched, VarName, (int)(GetComponent<RuleComponent>().GetVariableinVarTable(VarName)) - 1);
        }
    }
}
