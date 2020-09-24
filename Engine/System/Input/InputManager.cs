using Engine.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ultraviolet;
using Ultraviolet.Input;

namespace Engine.System.Input
{
    public class Actions : InputActionCollection
    {
        public Actions(UltravioletContext uv)
            : base(uv)
        {
        }

        public static Actions Instance { get; } = CreateSingleton<Actions>();

        public InputAction SendAction(string name)
        {
            return CreateAction(name);
        }

        public InputBinding GetBinding(Key keybind)
        {
            return CreateKeyboardBinding(keybind);
        }
    }

    public class InputManager
    {
        private static UltravioletContext _uvContext;
        private static List<InputComponent> _components = new List<InputComponent>();
        private static Actions _actions;

        public static void Initialize()
        {
            _uvContext = UltravioletContext.DemandCurrent();
            _actions = Actions.Instance;
        }

        public static void OnUpdateEffectInputs()
        {
            foreach (var entity in EntityManager.GetAllEntities())
            {
                var comp = entity.GetComponent<InputComponent>();
                if (comp != null)
                {
                    comp.CheckActions();
                }
            }
        }

        public static InputAction CreateAction(string name)
        {
            if (_uvContext == null)
                return null;                       

            return _actions.SendAction(name);
        }

        public static InputBinding CreateKeyboardBinding(Key keybind)
        {
            return _actions.GetBinding(keybind);
        }
    }
}
