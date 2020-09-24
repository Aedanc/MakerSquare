
//    //APU Monogame, APU XNA :(


//using Microsoft.Xna.Framework.Input;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Engine.System.Input
//{
//    using LowlevelKeyboardState = Microsoft.Xna.Framework.Input.KeyboardState;
//    /// <summary>
//    ///  Keyboard input management
//    /// </summary>
//    public class KeyboardState
//    {
//        internal KeyboardState()
//        {
//            _currentState = Keyboard.GetState();
//            _previousState = _currentState;
//        }

//        private LowlevelKeyboardState _currentState;
//        private LowlevelKeyboardState _previousState;
//        public bool IsKeyDown(Keys key) => _currentState.IsKeyDown(key);

//        public bool IsKeyUp(Keys key) => _currentState.IsKeyUp(key);

//        public bool IsKeyPressed(Keys key) => IsKeyDown(key) && _previousState.IsKeyUp(key);

//        public bool IsKeyReleased(Keys key) => IsKeyUp(key) && _previousState.IsKeyDown(key);
//        public bool Shift() => IsKeyDown(Keys.LeftShift) || IsKeyDown(Keys.RightShift);

//        public bool Alt() => IsKeyDown(Keys.LeftAlt);

//        public bool Control() => IsKeyDown(Keys.LeftControl) || IsKeyDown(Keys.RightControl);

//        /// <summary>
//        /// Get the current keyboard state and checks pressed keys
//        /// </summary>
//        internal void Update()
//        {
//            _previousState = _currentState;
//            _currentState = Keyboard.GetState();
     
//        }
//    }
//}
