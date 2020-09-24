using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine;
using Engine.Components;
using Engine.System.Rule;

namespace EngineDemo
{
    [Serializable]
    public class Test1 : Entity
    {
        public int nbAdd;
        public int HP;
        //TODO Fix missing/wrong code 
      //  public RuleManager.EngineMessage msg { get; set; }

        public Test1() : base(true)
        {
            Name = "test";
            nbAdd = 0;
            HP = 0;
            AddComponent(new RuleComponent(this));
        } 

        public void LossHp(string EntityWatched, Entity EntityWatcher, string VarName)
        {
            /*
            Console.WriteLine((int)(GetComponent<RuleComponent>().GetVariableinVarTable("HP")));
            GetComponent<RuleComponent>().SetVariablevalue(EntityWatched, VarName, (int)(GetComponent<RuleComponent>().GetVariableinVarTable("HP")) - 10);
            */
            HP = HP - 10;
            Console.WriteLine(HP);

        }


        public void RemoveAdd(string EntityWatched, Entity EntityWatcher, string VarName)
        {
            GetComponent<RuleComponent>().RemoveWatcher(this.AddHp);
        }

        public void AddHp(string EntityWatched, Entity EntityWatcher, string VarName)
        {
            nbAdd += 1;
            Console.WriteLine((int)(GetComponent<RuleComponent>().GetVariableinVarTable("HP")));
            GetComponent<RuleComponent>().SetVariablevalue(EntityWatched, VarName, (int)(GetComponent<RuleComponent>().GetVariableinVarTable("HP")) + 10);
        }
    }
}
