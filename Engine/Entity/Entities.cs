using System;
using System.Collections.Generic;
using Ultraviolet;
using Engine.Components;

namespace Engine
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

        public Vector2 ToVector2()
        {
            return new Vector2(x, y);
        }

        public Microsoft.Xna.Framework.Vector2 ToVector2MX()
        {
            return new Microsoft.Xna.Framework.Vector2(x, y);
        }

        public float x;
        public float scaleX;
        public float y;
        public float scaleY;        
        public float depth;
    };

    [Serializable]
    public class Entity
    {
        protected List<Component> _components;
        protected List<Entity> Children;
        public Position Transform;

        public string Name { get; set; }
        public Guid Guid { get; }

        public bool marked_for_deletion = false;
        public bool loaded_before_engine { get; private set; }

        public List<Component> Components {
            get { return _components; }
            private set { value = _components; }
        }


        public Entity(bool loaded_before_engine_ = false) {
            _components = new List<Component>();
            Children = new List<Entity>();
            Guid = Guid.NewGuid();
            Transform = new Position(0, 0, 0);
            loaded_before_engine = loaded_before_engine_;
        }

        public Entity(Position pos, bool loaded_before_engine_ = false)
        {
            _components = new List<Component>();
            Children = new List<Entity>();
            Guid = Guid.NewGuid();
            Transform = pos;
            loaded_before_engine = loaded_before_engine_;
        }

        public void AddComponent(Component component)
        {
            _components.Add(component);
        }

        public void DeleteComponent(Component component)
        {
            _components.Remove(component);
        }

        public void DeleteAllComponent()
        {
            _components.Clear();
        }

        public T GetComponent<T>()
        {
            foreach (object comp in _components)
            {
                if (comp.GetType() == typeof(T))
                    return (T)comp;
            }
            return default(T);
        }   
    }
}