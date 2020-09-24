using Entities;
using System;
using System.Diagnostics;

namespace Engine
public class EntityManager
{
    // Create List of type Entities that contains every Entities in the scene
    public EntityManager() {}

    protected List<Entity> EntityList = new List<Entity>();

    public void AddEntityInList(Entity Entity)
    {
        EntityList.Add(Entity);
        Debug.WriteLine("Entity has been added");
    }

    public Entity GetEntity(string Name)
    {
        foreach (Entity Entity in EntityList)
        {
            if (Name == Entity.Name)
            {
                return Entity;
            }
        }

        throw new System.InvalidOperationException("Name does not exist.");
    }
}