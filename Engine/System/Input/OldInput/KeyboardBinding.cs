//using Microsoft.Xna.Framework.Input;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Engine.System.Input
//{
//    public enum KeyEvent
//    {
//        OnPressed,
//        OnReleased,
//        OnDown,
//    }
//    public class KeyboardBinding : IBinding
//    {
//        public InputType Type => InputType.Keyboard;
//        private List<Keys> _keys;
//        private KeyEvent _keyEventType;


//        public KeyboardBinding(KeyEvent keyEvent, params Keys[] keys)
//        {
//            _keyEventType = keyEvent;
//            _keys = new List<Keys>(keys);
//        }

//        public bool ValidateInput(InputState inputState)
//        {
//            bool validation = false;
//            switch (_keyEventType)
//            {
//                case KeyEvent.OnPressed:
//                    foreach (var k in _keys)
//                    {
//                        if (inputState.Keyboard.IsKeyUp(k))
//                            return false;
//                        if (inputState.Keyboard.IsKeyPressed(k))
//                            validation = true;
//                    }
//                    break;
//                case KeyEvent.OnReleased:
//                    foreach (var k in _keys)
//                    {
//                        if (inputState.Keyboard.IsKeyDown(k))
//                            return false;
//                        if (inputState.Keyboard.IsKeyReleased(k))
//                            validation = true;
//                    }
//                    break;
//                case KeyEvent.OnDown:
//                    return _keys.All(k => inputState.Keyboard.IsKeyDown(k));
//            }
//            return validation;
//        }
//    }
//}
