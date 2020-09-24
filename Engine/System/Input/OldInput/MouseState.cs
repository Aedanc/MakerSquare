
////APU Monogame, APU XNA :(

////using Microsoft.Xna.Framework;
////using Microsoft.Xna.Framework.Input;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Engine.System.Input
//{
//    using LowlevelMouseState = Microsoft.Xna.Framework.Input.MouseState;
//    /// <summary>
//    ///  Mouse Input management.
//    /// </summary>
//    /// 
//    public enum MouseButton
//    {
//        Left,
//        Right,
//        Middle,
//        XButton1,
//        XButton2
//    };

//    public class MouseState
//    {
//        private LowlevelMouseState _currentState;
//        private LowlevelMouseState _previousState;

//        internal MouseState()
//        {
//           _currentState = Mouse.GetState();
//           _previousState = _currentState;
//        }

//        public Point Cursor {
//            get {
//                return _currentState.Position;
//            }
//        }

//        public bool IsButtonPressed(MouseButton button)
//        {
//            switch (button)
//            {
//                case MouseButton.Left:
//                    return _currentState.LeftButton == ButtonState.Pressed;
//                case MouseButton.Right:
//                    return _currentState.RightButton == ButtonState.Pressed;
//                case MouseButton.Middle:
//                    return _currentState.MiddleButton == ButtonState.Pressed;
//                case MouseButton.XButton1:
//                    return _currentState.XButton1 == ButtonState.Pressed;
//                case MouseButton.XButton2:
//                    return _currentState.XButton2 == ButtonState.Pressed;
//            }
//            return false;
//        }

//        public bool IsButtonClicked(MouseButton button)
//        {
//            switch (button)
//            {
//                case MouseButton.Left:
//                    //return _currentState.LeftButton == ButtonState.Pressed && _previousState.LeftButton == ButtonState.Released;
//                case MouseButton.Right:
//                    return _currentState.RightButton == ButtonState.Pressed && _previousState.RightButton == ButtonState.Released;
//                case MouseButton.Middle:
//                    return _currentState.MiddleButton == ButtonState.Pressed && _previousState.MiddleButton == ButtonState.Released;
//                case MouseButton.XButton1:
//                    return _currentState.XButton1 == ButtonState.Pressed && _previousState.XButton1 == ButtonState.Released;
//                case MouseButton.XButton2:
//                    return _currentState.XButton2 == ButtonState.Pressed && _previousState.XButton2 == ButtonState.Released;
//            }
//            return false;
//        }

//        public bool IsButtonReleased(MouseButton button)
//        {
//            switch (button)
//            {
//                case MouseButton.Left:
//                    return _currentState.LeftButton == ButtonState.Released && _previousState.LeftButton == ButtonState.Pressed;
//                case MouseButton.Right:
//                    return _currentState.RightButton == ButtonState.Released && _previousState.RightButton == ButtonState.Pressed;
//                case MouseButton.Middle:
//                    return _currentState.MiddleButton == ButtonState.Released && _previousState.MiddleButton == ButtonState.Pressed;
//                case MouseButton.XButton1:
//                    return _currentState.XButton1 == ButtonState.Released && _previousState.XButton1 == ButtonState.Pressed;
//                case MouseButton.XButton2:
//                    return _currentState.XButton2 == ButtonState.Released && _previousState.XButton2 == ButtonState.Pressed;
//            }
//            return false;
//        }

//        internal void Update()
//        {
//            _previousState = _currentState;
//            _currentState = Mouse.GetState();
//        }


//    }
//}
