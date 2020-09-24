using System;
using Engine.Components;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Engine.System.Collision;
using Engine.System.Rule;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;

namespace Engine.System.Rule
{
    [Serializable]
    public class RuleComponent : Component
    {
        //Faut retravailler les boucles, faire en fonctions des noms des entités ça serait plus facile pour tout identifier ? Réfléchir sur la pertinance
        //Changer la liste de watcher de variable, , on peut pas associer plusieurs fois la même fonction.

        public Dictionary<RuleManager.EngineMessage, Object[]> listWatcherEngine;
        Dictionary<string, Tuple<Type, object, FieldInfo>> EntityVarTable;

        public Dictionary<ActionFunction, Object[]> listWatcherVariable;
        public bool WatcherSet;

        public delegate void ActionFunction(string EntityWatched, Entity EntityWatcher, string VarName);

        public RuleComponent(Entity entity) : base(entity)
        {
            EntityVarTable = new Dictionary<string, Tuple<Type, object, FieldInfo>>();
            listWatcherVariable = new Dictionary<ActionFunction, object[]>();
            listWatcherEngine = new Dictionary<RuleManager.EngineMessage, object[]>();
            WatcherSet = false;
        }
    
        public object GetVariableinVarTable(string varName)
        {
            foreach (KeyValuePair<string, Tuple<Type, object, FieldInfo>> kvp in EntityVarTable)
            {
                if (kvp.Key.Equals(varName))
                    return (kvp.Value.Item2);
            }
            return (null);
        }

        public void GetValueEntity(string name)
        {
            Entity entity = EntityManager.GetEntity(name);
            FieldInfo[] myFieldInfo = entity.GetType().GetFields();

            EntityVarTable.Clear();
            for (int i = 0; i < myFieldInfo.Length; i++)
            {
                EntityVarTable.Add(myFieldInfo[i].Name, new Tuple<Type, object, FieldInfo>(myFieldInfo[i].FieldType, myFieldInfo[i].GetValue(entity), myFieldInfo[i]));
            }
     }

        public void SetVariablevalue(string EntityName, string VarName, object Value)
        {
            try
            {
                Entity entity = EntityManager.GetEntity(EntityName);
                GetValueEntity(EntityName);

                foreach (KeyValuePair<string, Tuple<Type, object, FieldInfo>> kvp in EntityVarTable)
                {
                    if (kvp.Key.Equals(VarName))
                        kvp.Value.Item3.SetValue(entity, Value);
                }
            }
            catch (InvalidOperationException e)
            {
                
            }
        }

        public void RemoveWatcher(ActionFunction del)
        {
            RuleManager.RemoveWatcher(this, del);
        }

        //fonction qui regarde en permanence une variable d'une entité et qui applique une fonction en fonction
        public void WatcherVarEntity(string variableName, object targetvalue, string entityName, ActionFunction  del, string targetDel, Entity watcher, string valueDel)
        {
            listWatcherVariable.Add(del, new Object[7] { variableName, targetvalue, entityName, del, targetDel, watcher, valueDel });
            WatcherSet = true;
        }

        public void WatcherExecution(string variableName, object targetvalue, string entityName, ActionFunction del, string targetDel, Entity watcher, string valueDel)
        {
            //try catch exception + exception name
            GetValueEntity(entityName);
            object valueVariable = null;
            foreach (KeyValuePair<string, Tuple<Type, object, FieldInfo>> kvp in EntityVarTable)
            {
                if (kvp.Key.Equals(variableName))
                    valueVariable = kvp.Value.Item2;
            }
            if (valueVariable.Equals(targetvalue))
                del(targetDel, watcher, valueDel);
        }

        public void WatcherEngineExecution(RuleManager.EngineMessage msg, ActionFunction del, string targetDel, Entity watcher, string valueDel)
        {
            for (int i = 0; i < RuleManager.EngineAction.Count; i++)
            {
                var tmp = RuleManager.EngineAction.ElementAt(i);
                  if (tmp.Equals(msg))
                {
                    del(targetDel, watcher, valueDel);
                    return;
                }
            }
        }

        public void WatcherActionEngine(RuleManager.EngineMessage msg, ActionFunction del, string targetDel, Entity watcher, string valueDel)
        {
            Entity.GetComponent<CollisionComponent>().AddOnCollisionHandler(delegate (Fixture sender, Fixture other, Contact contact)
            {
                var tmp = new RuleManager.EngineMessage();
                tmp.Action = RuleManager.ActionEngine.Collision;
                tmp.entitybase = EntityManager.GetAllEntities().Find(i => i.Guid == (Guid)sender.Body.Tag);
                tmp.entityFocus = EntityManager.GetAllEntities().Find(i => i.Guid == (Guid)other.Body.Tag);
                RuleManager.EngineAction.Enqueue(tmp);
                return true;
            });
            listWatcherEngine.Add(msg, new Object[4] { del, targetDel, watcher, valueDel });
            WatcherSet = true;
        }

         [Serializable]
        public struct Tuple<T1, T2, T3>
        {
            public readonly T1 Item1;
            public readonly T2 Item2;
            public T3 Item3;

            public Tuple(T1 item1, T2 item2, T3 item3)
            {
                Item1 = item1;
                Item2 = item2;
                Item3 = item3;
            }
        }
    }
}
