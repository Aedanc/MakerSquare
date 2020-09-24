using MakerSquare.FileSystem;
using System;
using System.Collections.Generic;

namespace MakerSquare.FrontFacingECS
{
    [Serializable]
    public class FFEntity
    {
        [Serializable]
        public struct Position
        {
            public Position(float x_, float y_)
            {
                x = x_;
                y = y_;
                scaleX = 1;
                scaleY = 1;
                depth = 0;
            }

            public Position(float x_, float y_, float depth_)
            {
                x = x_;
                y = y_;
                scaleX = 1;
                scaleY = 1;
                depth = depth_;
            }            

            public float x;
            public float scaleX;
            public float y;
            public float scaleY;
            public float depth;
        };

        protected List<FFComponent> _components;
        protected List<FFEntity> Children;
        public Position Transform;
        public string Name { get; set; }
        public string EntityTemplate = null;

        public List<FFComponent> Components
        {
            get { return _components; }
            private set { value = _components; }
        }

        public FFEntity(string name)
        {
            _components = new List<FFComponent>();
            Children = new List<FFEntity>();
            Transform = new Position(0, 0, 0);
            Name = name;
        }

    }

    [Serializable]
    public abstract class FFComponent
    {
        public FFEntity Entity { get; private set; }

        public FFComponent(FFEntity entity)
        {
            Entity = entity;
        }

        public abstract string WriteComponentAddition();
    }

    [Serializable]
    public class FFSpriteComponent : FFComponent
    {
        public string resourceName { get; set; }

        public FFSpriteComponent(FFEntity entity, string resourceName) : base(entity)
        {
            this.resourceName = resourceName;
        }

        public override string WriteComponentAddition()
        {
            return String.Format("AddComponent(new Engine.System.Graphics.SpriteComponent(this, \"{0}\", onLaunch))", resourceName);
        }
    }

    [Serializable]
    public class FFAudioComponent : FFComponent
    {
        public  string resourceName { get; set; }
        public bool loop;
        public bool playable;

        public FFAudioComponent(FFEntity entity, string resourceName, bool playable) : base(entity)
        {
            this.resourceName = resourceName;
            this.playable = playable;
        }

        public override string WriteComponentAddition()
        {
            string returnString = "";
            returnString += String.Format("AddComponent(new Engine.System.Audio.AudioComponent(this, \"{0}\", ", resourceName);
            if (loop == false)
                returnString += "false, ";
            else
                returnString = "true, ";
            if (playable == false)
                returnString += "false, ";
            else
                returnString += "true, ";
            returnString += "onLaunch))";
            return returnString;
        }
    }

    [Serializable]
    public class FFInput : FFComponent
    {
        //add pause
        //Dictionary<key designed, tuple<Key State, function name>>
        public Dictionary<string, Tuple<string, string, string>> KeyList;
        public FFInput(FFEntity entity, Dictionary<string, Tuple<string, string, string>> KeyList) : base(entity)
        {
            this.KeyList = KeyList;
        }

        public override string WriteComponentAddition()
        {
            string PreString = "";
            String ReturnString = "var input = new Engine.System.Input.InputComponent(this);\n";

            foreach (KeyValuePair<string, Tuple<string, string, string>> entry in KeyList)
            {
                switch (entry.Value.Item3)
                {
                    case "move_up":
                        PreString += "Engine.System.Input.ActionDelegate move_up_down = (entity, dict) => { GetComponent<Engine.System.Movement.MovementComponent>().MoveToUp(); };\nEngine.System.Input.ActionDelegate move_up_up = (entity, dict) => { GetComponent<Engine.System.Movement.MovementComponent>().StopMoveToDirection(Engine.System.Movement.MovementComponent.DirectionMovement.UP); };\n";
                        break;
                    case "move_down":
                        PreString += "Engine.System.Input.ActionDelegate move_down_down = (entity, dict) => { GetComponent<Engine.System.Movement.MovementComponent>().MoveToDown(); };\n Engine.System.Input.ActionDelegate move_down_up = (entity, dict) => { GetComponent<Engine.System.Movement.MovementComponent>().StopMoveToDirection(Engine.System.Movement.MovementComponent.DirectionMovement.DOWN);};\n";
                        break;
                    case "move_right":
                        PreString += "Engine.System.Input.ActionDelegate move_right_down = (entity, dict) => { GetComponent<Engine.System.Movement.MovementComponent>().MoveToRight();};\n Engine.System.Input.ActionDelegate move_right_up = (entity, dict) => { GetComponent<Engine.System.Movement.MovementComponent>().StopMoveToDirection(Engine.System.Movement.MovementComponent.DirectionMovement.RIGHT);};\n";
                        break;
                    case "move_left":
                        PreString += "Engine.System.Input.ActionDelegate move_left_down = (entity, dict) => { GetComponent<Engine.System.Movement.MovementComponent>().MoveToLeft();};\nEngine.System.Input.ActionDelegate move_left_up = (entity, dict) => { GetComponent<Engine.System.Movement.MovementComponent>().StopMoveToDirection(Engine.System.Movement.MovementComponent.DirectionMovement.LEFT);};\n";
                        break;
                    case "quit":
                        PreString += "Engine.System.Input.ActionDelegate quit_down = (entity, dict) => Environment.Exit(0);\nEngine.System.Input.ActionDelegate quit_up = (entity, dict) => Environment.Exit(0);\n";
                        break;
                    default:
                        break;
                }
                if (entry.Value.Item1 == "DOWN" || entry.Value.Item1 == "RELEASED")
                {
                    ReturnString += String.Format("input.AddAction(\"{0}\", Engine.System.Input.{1}, {2}, null, {3});\n", entry.Value.Item3,
                        entry.Value.Item1, entry.Key, entry.Value.Item3 + "_down");
                    ReturnString += String.Format("input.AddAction(\"{0}\", Engine.System.Input.{1}, {2}, null, {3});\n", entry.Value.Item3,
                     entry.Value.Item2, entry.Key, entry.Value.Item3 + "_up");
                }
                    ReturnString += String.Format("input.AddAction(\"{0}\", Engine.System.Input.{1}, {2}, null, {3});\n", entry.Value.Item3,
                        entry.Value.Item1, entry.Key, entry.Value.Item3 + "_down");
                ReturnString += String.Format("input.AddAction(\"{0}\", Engine.System.Input.{1}, {2}, null, {3});\n", entry.Value.Item3,
                    entry.Value.Item2, entry.Key, entry.Value.Item3 + "_up");
            }
            PreString += ReturnString + "AddComponent(input);\n";
            return PreString;
        }
    }

    [Serializable]
    public class FFMovement : FFComponent
    {
        public FFMovement(FFEntity entity) : base(entity)
        {
        }

        public override string WriteComponentAddition()
        {
            return String.Format("AddComponent(Engine.System.Collision.MovementComponentBuilder.CreateNew().Init(this).SetForceAmount(3000).Build())");
        }
    }

    [Serializable]
    public class FFRule : FFComponent
    {
        public FFRule(FFEntity entity) : base(entity)
        {
        }

        public override string WriteComponentAddition()
        {
            return String.Format("AddComponent(new Engine.System.Rule.RuleComponent(this))");
        }
    }


    [Serializable]
    public class FFCollision : FFComponent
    {
        Tuple<int, int> Vector;

        public FFCollision(FFEntity entity, Tuple<int, int> vector) : base(entity)
        {
            this.Vector = vector;
        }

        public override string WriteComponentAddition()
        {
            return String.Format("AddComponent(Engine.System.Collision.CollisionComponentBuilder.CreateNew().Init(this, new Microsoft.Xna.Framework.Vector2({0},{1})).SetRestitution(1f).SetCategoriesCollision(tainicom.Aether.Physics2D.Dynamics.Category.Cat1).SetCollidesWith(tainicom.Aether.Physics2D.Dynamics.Category.All).SetBodyType(tainicom.Aether.Physics2D.Dynamics.BodyType.Dynamic).Build())", Vector.Item1, Vector.Item2);
        }
    }

    [Serializable]
    public struct Tuple<T1, T2>
    {
        public T1 Item1;
        public T2 Item2;

        public Tuple(T1 t1, T2 t2)
        {
            Item1 = t1;
            Item2 = t2;
        }
    }

    [Serializable]
    public struct Tuple<T1, T2, T3>
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;

        public Tuple(T1 t1, T2 t2, T3 t3)
        {
            Item1 = t1;
            Item2 = t2;
            Item3 = t3;
        }
    }
}