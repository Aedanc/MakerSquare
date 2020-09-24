using System;

namespace Engine.Components
{
    public interface LoadComponent
    {
        void LoadData();
    }

    [Serializable]
    public class Component
    {
        public Entity Entity { get; private set; }

        public Component(Entity entity)
        {
            Entity = entity;
        }
    }
}
