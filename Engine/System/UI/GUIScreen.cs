using System.Collections.Generic;
using Ultraviolet.Content;
using Ultraviolet.UI;
using Ultraviolet.ImGuiViewProvider;
using Ultraviolet.ImGuiViewProvider.Bindings;
using Ultraviolet;
using Engine.Components;
using System;

namespace Engine.System.UI
{
    public class GUIScreen : UIScreen, IImGuiPanel
    {
        private ContentManager _content;
        private List<Entity> _entities;
        private List<UIComponent> _components;

        bool to_flush = false;

        public GUIScreen(ContentManager globalContent, List<Entity> entities)
            : base("Content/Resources/UI", "GUIScreen", globalContent)
        {
            _content = globalContent;
            _entities = entities;
            _components = new List<UIComponent>();
        }


        public void ImGuiRegisterResources(ImGuiView view)
        {            
            view.Fonts.RegisterDefault();
            foreach (var entity in _entities)
            {
                foreach (var type in GUIManager.UIComponentsTypes)
                {
                    var func = typeof(Entity).GetMethod("GetComponent").MakeGenericMethod(type);
                    UIComponent comp = (UIComponent)func.Invoke(entity, null);
                    if (comp != null)
                    {
                        comp.RegisterData(view);
                        _components.Add(comp);
                    }
                }
            }
        }

        public void DeleteEntities()
        {
            to_flush = true;
        }

        public bool Empty()
        {
            return _entities.Count == 0;
        }

        private void _DeleteEntities()
        {
            _entities.Clear();
            _components.Clear();
            to_flush = false;
        }

        public void ImGuiUpdate(UltravioletTime time)
        {
            if (to_flush)
            {
                _DeleteEntities();
                return;
            }
            var style = ImGui.GetStyle();
            style.WindowBorderSize = 0;
            style.WindowRounding = 0;
            style.WindowPadding = new Vector2(0, 0);
            ImGui.SetNextWindowSize(new Vector2(0, 0));
            
            foreach (var component in _components)
            {
                component.ImGuiUpdate(time);
            }
        }

        public void ImGuiDraw(UltravioletTime time)
        {
        }
    }
}
