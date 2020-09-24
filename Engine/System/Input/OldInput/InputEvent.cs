//using Engine.Event;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Engine.System.Input
//{
//    public class InputEvent
//    {
//        IBinding[] Bindings;

//        public AEvent Event;

//        /// <summary>
//        /// Detect if the current input state matches the 
//        /// </summary>
//        internal void DetectEventTrigger(InputState inputs)
//        {
//            foreach (var bind in Bindings)
//            {
//                if (bind.ValidateInput(inputs))
//                {
//                    Event.OnInput(bind.Type, inputs);
//                    break;
//                }
//            }        
//        }
//    }
//}
