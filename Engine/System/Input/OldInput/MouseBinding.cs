//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Engine.System.Input
//{

//    public enum MouseEvent
//    {
//        OnClick,
//        OnHold,
//        OnReleased,
//    }

//    public class MouseBinding : IBinding
//    {

//        private MouseEvent _mouseEventType;
//        private MouseButton _button;

//        public MouseBinding(MouseEvent eventType, MouseButton button)
//        {
//            _mouseEventType = eventType;
//            _button = button;
//        }

//        public InputType Type => InputType.Mouse;

//        public bool ValidateInput(InputState inputState)
//        {
//            switch (_mouseEventType)
//            {
//                case MouseEvent.OnClick:
//                    return inputState.Mouse.IsButtonClicked(_button);
//                case MouseEvent.OnHold:
//                    return inputState.Mouse.IsButtonPressed(_button);
//                case MouseEvent.OnReleased:
//                    return inputState.Mouse.IsButtonReleased(_button);
//            }
//            return false;
//        }
//    }
//}
