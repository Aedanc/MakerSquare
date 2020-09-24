using System;
using System.Collections.Generic;
using System.Diagnostics;
using Engine;
using Engine.System.Collision;
using Engine.System.Graphics;
using Engine.System.Input;
using Engine.System.Movement;
using Engine.System.Rule;
using Microsoft.Xna.Framework;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;

namespace EngineDemo
{
    [Serializable]
    class Bat : Entity
    {
        public Bat() : base(true)
        {
            // Initial placement
            Transform.x = 400;
            Transform.y = 150;
            // Main Components
            AddComponent(new RuleComponent(this));
            // var input = new InputComponent(this);
            
            AddComponent(MovementComponentBuilder.CreateNew().Init(this).SetForceAmount(3000).Build());
            AddComponent(new SpriteComponent(this, "Bat", true));
            List<OnCollisionEventHandler> fixtureOnCollisionEventHandlers = new List<OnCollisionEventHandler>();
            fixtureOnCollisionEventHandlers.Add(delegate(Fixture sender, Fixture other, Contact contact)
            {
                Vector2 linear = other.Body.LinearVelocity;
                if (linear.Y >= 0)
                    other.Body.LinearVelocity = new Vector2(-100, -70);
                else
                    other.Body.LinearVelocity = new Vector2(-100, 70);
                return true;
            });
            fixtureOnCollisionEventHandlers.Add(delegate(Fixture sender, Fixture other, Contact contact)
            {
                Vector2 linear = other.Body.LinearVelocity;
                if (linear.Y >= 0)
                    other.Body.LinearVelocity = new Vector2(0, -170);
                else
                    other.Body.LinearVelocity = new Vector2(0, 170);
                return true;
            });
            fixtureOnCollisionEventHandlers.Add(delegate(Fixture sender, Fixture other, Contact contact)
            {
                Vector2 linear = other.Body.LinearVelocity;
                if (linear.Y >= 0)
                    other.Body.LinearVelocity = new Vector2(100, -70);
                else
                    other.Body.LinearVelocity = new Vector2(100, 70);
                return true;
            });
            AddComponent(CollisionComponentBuilder.CreateNew()
                .Init(this, new Vector2(153, 40))
                .SetBodyType(BodyType.Dynamic)
                .SetCollidesWith(Category.All)
                .SetCategoriesCollision(Category.Cat3)
                .SetHitBoxDivide(3)
                .SetFixtureOnCollisionEventHandlers(fixtureOnCollisionEventHandlers)
                .SetFriction(0f)
                .Build());
            
            var input = new InputComponent(this);
            

            ActionDelegate move_up_down = (entity, dict) => {
                GetComponent<MovementComponent>().MoveToUp();
            };
            
            ActionDelegate move_up_up = (entity, dict) =>
            {
                GetComponent<MovementComponent>().StopMoveToDirection(MovementComponent.DirectionMovement.UP);
            };
            ActionDelegate move_down_down = (entity, dict) => {
                GetComponent<MovementComponent>().MoveToDown();
            };
            
            ActionDelegate move_down_up = (entity, dict) =>
            {
                GetComponent<MovementComponent>().StopMoveToDirection(MovementComponent.DirectionMovement.DOWN);
            };
            ActionDelegate move_right_down = (entity, dict) => {
                GetComponent<MovementComponent>().MoveToRight();
            };
            
            ActionDelegate move_right_up = (entity, dict) =>
            {
                GetComponent<MovementComponent>().StopMoveToDirection(MovementComponent.DirectionMovement.RIGHT);
            };
            ActionDelegate move_left_down = (entity, dict) => {
                GetComponent<MovementComponent>().MoveToLeft();
            };
            
            ActionDelegate move_left_up = (entity, dict) =>
            {
                GetComponent<MovementComponent>().StopMoveToDirection(MovementComponent.DirectionMovement.LEFT);
            };

            ActionDelegate launch_ball = (entity, dict) =>
            {
                EntityManager.GetEntity("Breakout").GetComponent<RuleComponent>().WatcherVarEntity("score", 0, "Breakout", SpawnBall, "Ball", this, "none");
            };

            ActionDelegate exit = (entity, dict) => Engine.Core.Game.Instance.Exit();
            input.AddAction("move_right", OnKey.DOWN, Ultraviolet.Input.Key.Right, null, move_right_down);
            input.AddAction("move_right", OnKey.UP, Ultraviolet.Input.Key.Right, null, move_right_up);
            input.AddAction("move_left", OnKey.DOWN, Ultraviolet.Input.Key.Left, null, move_left_down);
            input.AddAction("move_left", OnKey.UP, Ultraviolet.Input.Key.Left, null, move_left_up);
            input.AddAction("exit", OnKey.PRESSED, Ultraviolet.Input.Key.Escape, null, exit);
            input.AddAction("launch_ball", OnKey.PRESSED, Ultraviolet.Input.Key.Space, null, launch_ball);
            AddComponent(input);


            // Mouvement mechanics not implemented yet.
            // Waiting for update from physics part before tweaking it
        }

        public void SpawnBall(string EntityWatched, Entity EntityWatcher, string VarName)
        {
            Debug.WriteLine("SpawnBall");
            var mvt = EntityManager.GetEntity("Ball").GetComponent<MovementComponent>();
            mvt.forceAmount = 3000;
            mvt.Direction = new Vector2(0, -1);
            EntityManager.GetEntity("Breakout").GetComponent<RuleComponent>().RemoveWatcher(SpawnBall);
        }
    }
}