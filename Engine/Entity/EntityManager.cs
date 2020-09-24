using Engine.Components;
using Engine.System.Collision;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine
{
    // The other part can be found in the Project SavingSystem,
    // and contains the LoadScene function (and related).
    public partial class EntityManager
    {
        protected static List<Entity> PreloadEntities = new List<Entity>();
        protected static List<Entity> EntityList = new List<Entity>();        

        static public void AddEntityInList(Entity Entity)
        {
            EntityList.Add(Entity);         
        }

        static public void AddPreInitEntity(Entity entity)
        {
            PreloadEntities.Add(entity);
        }

        static public void AddListEntityInList(List<Entity> entities)
        {
            foreach (var entity in entities)
            {
                if (entity.loaded_before_engine)
                    PreloadEntities.Add(entity);
                else
                    EntityList.Add(entity);
            }
        }

        static public void PostContentLoad()
        {
            foreach (Entity entity in PreloadEntities)
            {
                foreach (Component component in entity.Components)
                {
                    if (component is LoadComponent)
                        ((LoadComponent)component).LoadData();
                }
                EntityList.Add(entity);
            }
            PreloadEntities.Clear();
        }

        static public List<Entity> GetAllEntities()
        {
            return EntityList;
        }

        static public Entity GetEntity(string Name)
        {
            foreach (Entity Entity in EntityList)
            {
                if (Name == Entity.Name)
                {
                    return Entity;
                }
            }

            throw new InvalidOperationException("Name does not exist.");
        }

        static public void UnloadEntities()
        {
            EntityList.ForEach((entity) => { entity.marked_for_deletion = true; });
            System.UI.GUIManager.FlushGuiScreen();
        }

        static public void RemoveMarkedEntities()
        {
            foreach (var Entity in EntityList)
            {
                if (Entity.marked_for_deletion)
                {
                    var collisionComponent = Entity.GetComponent<CollisionComponent>();
                    if (collisionComponent != null)
                        CollisionManager.RemoveBody(collisionComponent.body);
                    CollisionManager.RemoveCollisionComponent(Entity);
                    Entity.DeleteAllComponent();
                }
            }
            EntityList.RemoveAll((entity) => { return entity.marked_for_deletion == true; });
        }
    }
}