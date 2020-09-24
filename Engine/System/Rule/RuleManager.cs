using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.System.Rule
{
    [Serializable]
    public class RuleManager
    {
        public static Queue<EngineMessage> EngineAction;
        static Dictionary<RuleComponent, EngineMessage> RemoveEngineList;
        static List<RuleComponent> WatcherList;
        static Dictionary<RuleComponent, RuleComponent.ActionFunction> RemoveList;
   
        public static void Initialize()
        {
            WatcherList = new List<RuleComponent>();
            RemoveList = new Dictionary<RuleComponent, RuleComponent.ActionFunction>();
            RemoveEngineList = new Dictionary<RuleComponent, EngineMessage>();
            EngineAction = new Queue<EngineMessage>();
        }

        public static void UpdateRule()
        {
            foreach (Entity entity in EntityManager.GetAllEntities())
            {
                var comp = entity.GetComponent<RuleComponent>();
                if (comp != null && comp.WatcherSet == true )
                {
                    AddWatcher(comp);
                }
            }
            if (RemoveList.Count != 0 || RemoveEngineList.Count != 0)
            {
                foreach (KeyValuePair<RuleComponent, RuleComponent.ActionFunction> remove in RemoveList)
                {
                    foreach (RuleComponent rule in WatcherList)
                    {
                        rule.listWatcherVariable.Remove(remove.Value);
                    }
                    WatcherList.Remove(remove.Key);
                }
                foreach (KeyValuePair<RuleComponent, EngineMessage> removeengine in RemoveEngineList)
                {
                    foreach (RuleComponent rule in WatcherList)
                    {
                        rule.listWatcherEngine.Remove(removeengine.Value);

                    }
                    WatcherList.Remove(removeengine.Key);
                }
                RemoveList.Clear();
                RemoveEngineList.Clear();
            }

            foreach (RuleComponent watcher in WatcherList)
            {
                foreach (KeyValuePair<RuleComponent.ActionFunction, Object[]> kvp in watcher.listWatcherVariable)
                {
                    watcher.WatcherExecution((string)kvp.Value[0], kvp.Value[1],
                            (string)kvp.Value[2], (RuleComponent.ActionFunction)kvp.Value[3],
                            (string)kvp.Value[4], (Entity)kvp.Value[5],
                            (string)kvp.Value[6]);
                }
                foreach (KeyValuePair<EngineMessage, Object[]> kvp in watcher.listWatcherEngine)
                {
                    watcher.WatcherEngineExecution((EngineMessage)kvp.Key,
                    (RuleComponent.ActionFunction)kvp.Value[0], (string)kvp.Value[1],
                    (Entity)kvp.Value[2], (string)kvp.Value[3]);
                }
            }
            EngineAction.Clear();
        }

        public static void AddWatcher(RuleComponent del)
        {
            foreach (RuleComponent rule in WatcherList)
            {
                if (rule.Entity.Name == del.Entity.Name)
                    return;
            }
            WatcherList.Add(del);
        }

        public static void RemoveWatcher(RuleComponent del, RuleComponent.ActionFunction action)
        {
            RemoveList.Add(del, action);
        }

        public static void RemoveEngineWatcher(RuleComponent del, EngineMessage msg)
        {
            RemoveEngineList.Add(del, msg);
        }

        [Serializable]
        public struct EngineMessage
        {
            public Entity entitybase;
            public Entity entityFocus;
            public ActionEngine Action;
        }

        [Serializable]
       public enum ActionEngine { Collision, Input, Graphics, Physics, Movement, Audio };
    }
}
