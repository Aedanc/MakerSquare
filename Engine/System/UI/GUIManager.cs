using Ultraviolet;
using Ultraviolet.Content;
using Ultraviolet.ImGuiViewProvider.Bindings;
using System.Collections.Generic;
using System;

namespace Engine.System.UI
{
    public static class GUIManager
    {
        private static UltravioletContext _context;
        private static ContentManager _uvContentManager;
        private static GUIScreen _currentScreen;
        private static List<Entity> _screen_entities;

        private static bool to_be_flushed = false;

        public static void Initialize(UltravioletContext context, ContentManager uvContentManager)
        {
            _context = context;
            _uvContentManager = uvContentManager;
            _screen_entities = new List<Entity>();
        }

        public static ContentManager GetContentManager()
        {
            return _uvContentManager;
        }

        public static void LoadScreenEntities(List<Entity> components)
        {
            _screen_entities = components;
        }

        public static void DisplayGuiScreen()
        {
            _currentScreen = new GUIScreen(_uvContentManager, _screen_entities);
            var screens = _context.GetUI().GetScreens();
            _context.GetUI().GetScreens().Open(_currentScreen, TimeSpan.Zero);
        }

        public static void FlushGuiScreen()
        {
            to_be_flushed = true;
            if (_currentScreen != null)
                _currentScreen.DeleteEntities();
        }

        public static void Update()
        {
            if (to_be_flushed && _currentScreen != null && _currentScreen.Empty())
            {                
                _currentScreen = new GUIScreen(_uvContentManager, new List<Entity>());
                _context.GetUI().GetScreens().Open(_currentScreen, TimeSpan.Zero);
                if (_screen_entities != null)
                    _screen_entities.Clear();
                to_be_flushed = false;
            }
        }

        public static void FetchGuiComponents()
        {
            foreach (Entity entity in EntityManager.GetAllEntities())
            {
                foreach (var comp_type in GUIManager.UIComponentsTypes)
                {
                    var func = typeof(Entity).GetMethod("GetComponent").MakeGenericMethod(comp_type);
                    UIComponent comp = (UIComponent)func.Invoke(entity, null);
                    if (comp != null)
                        _screen_entities.Add(entity);
                }
            }
        }

        public static Type[] UIComponentsTypes = { typeof(ImageButtonComponent), typeof(UITextComponent) };
    }

    public class ImGuiFlagsPresets
    {
        public static ImGuiWindowFlags InvisibleWindow = ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoNav | ImGuiWindowFlags.NoMove |
            ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoTitleBar;
    }
}
