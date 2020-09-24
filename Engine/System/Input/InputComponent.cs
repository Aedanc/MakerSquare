using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Engine.Components;
using Ultraviolet.Core.Splinq;
using Ultraviolet.Input;

namespace Engine.System.Input
{
    public delegate void ActionDelegate(Entity entity, Dictionary<Entity, string> contextRefs);

    public enum OnKey
    {
        PRESSED,
        RELEASED,
        UP,
        DOWN
    }


    [Serializable]
    public class InputBindingBundle
    {
        [NonSerialized]
        public InputAction act;

        public List<OnKey> handlers;
        public List<Key> keyBinds;

        public Dictionary<Entity, string> contextRefs;

        public ActionDelegate codeUp;
        public ActionDelegate codeDown;

        public string name;
    }

    [Serializable]
    public class InputComponent : Component, LoadComponent
    {
        private List<InputBindingBundle> _actions;

        public InputComponent(Entity entity) : base(entity)
        {
            _actions = new List<InputBindingBundle>();
        }

        private bool IsKeyAlreadyBinded(Key keybind, string name)
        {
            if (_actions.Count == 0) return false;
            foreach (var action in _actions)
            {
                if (action.name != name) continue;
                foreach (var key in action.keyBinds)
                    if (key == keybind)
                        return true;
            }
            return false;
        }

        private void AddCodeToKeyAlreadyBinded(string name, OnKey handler, ActionDelegate code)
        {
            var inputBindingBundle = _actions.Find(i => i.name == name);
            if (inputBindingBundle == null)
                throw new NullReferenceException("No InputBindingBundle found\n");
            if (!inputBindingBundle.handlers.Any(i => i == handler))
                inputBindingBundle.handlers.Add(handler);
            if (handler == OnKey.UP || handler == OnKey.RELEASED) 
                inputBindingBundle.codeUp = code;
            else 
                inputBindingBundle.codeDown = code;
        }

        private void CreateNewInputBindingBundle(string name, OnKey handler, Key keybind, 
            Dictionary<Entity, string> contextRefs, ActionDelegate code)
        {
            InputBindingBundle input = new InputBindingBundle();

            input.name = name;
            input.act = InputManager.CreateAction(name);
            input.keyBinds = new List<Key>();
            input.keyBinds.Add(keybind);
            input.handlers = new List<OnKey>();
            input.handlers.Add(handler);
            input.contextRefs = contextRefs;
            if (handler == OnKey.UP || handler == OnKey.RELEASED) 
                input.codeUp = code;
            else 
                input.codeDown = code;
            if (input.act != null) 
                input.act.Primary = InputManager.CreateKeyboardBinding(keybind);
            _actions.Add(input);
        }

        public void AddAction(string name, OnKey handler, Key keybind,
            Dictionary<Entity, string> contextRefs, ActionDelegate code)
        {
            if (IsKeyAlreadyBinded(keybind, name)) AddCodeToKeyAlreadyBinded(name, handler, code);
            else CreateNewInputBindingBundle(name, handler, keybind, contextRefs, code);
        }

        public void CheckActions()
        {
            if (Entity == null)
                return;
            foreach (var action in _actions)
            {
                foreach (var handler in action.handlers)
                {
                    switch (handler)
                    {
                        case OnKey.PRESSED:
                            if (action.act.IsPressed())
                                action.codeDown(Entity, action.contextRefs);
                            break;
                        case OnKey.RELEASED:
                            if (action.act.IsReleased())
                                action.codeUp(Entity, action.contextRefs);
                            break;
                        case OnKey.DOWN:
                            if (action.act.IsDown())
                                action.codeDown(Entity, action.contextRefs);
                            break;
                        case OnKey.UP:
                            if (action.act.IsUp())
                                action.codeUp(Entity, action.contextRefs);
                            break;
                    }                    
                }
            }
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            //LoadData();
        }

        public void LoadData()
        {
            foreach (var act in _actions)
            {
                act.act = InputManager.CreateAction(act.name);
                if (act.act != null)
                    act.act.Primary = InputManager.CreateKeyboardBinding(act.keyBinds[0]);
            }
        }
    }
}