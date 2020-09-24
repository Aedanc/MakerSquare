using System;
using System.Collections.Generic;
using Ultraviolet;
using Ultraviolet.Core;
using Ultraviolet.ImGuiViewProvider;
using Ultraviolet.ImGuiViewProvider.Bindings;

namespace Engine.System.UI
{
    public delegate void ImageButtonDelegate(Entity entity, Dictionary<string, object> contextRefs);


    [Serializable]
    public class InputBindingBundle
    {
        public Dictionary<string, object> contextRefs;
        public ImageButtonDelegate action;
        
    }

    [Serializable]
    public class ImageButtonComponent : UIComponent
    {
        public InputBindingBundle onclick;


        public ImageButtonComponent(Entity entity) : base(entity)
        {
            onclick = new InputBindingBundle();
            onclick.action = (a, b) => { };
            onclick.contextRefs = new Dictionary<string, object>();
        }

        public override void ImGuiUpdate(UltravioletTime time)
        {
            ImageAsset asset = GetFirstAssetOfType<ImageAsset>();
            ImGui.SetNextWindowBgAlpha(0);
            var vec = new Vector2(asset.Tex2D.Width * Entity.Transform.scaleX,
                    asset.Tex2D.Height * Entity.Transform.scaleY);
            ImGui.SetNextWindowPos(Entity.Transform.ToVector2());
            ImGui.SetNextWindowSize(vec);
            if (ImGui.Begin(asset.ContentName, ImGuiFlagsPresets.InvisibleWindow))
            {
                if (ImGui.ImageButton(asset.ImGuiID, vec,
                    Vector2.Zero, Vector2.One, 0, Vector4.Zero))
                    onclick.action.Invoke(this.Entity, onclick.contextRefs);
                ImGui.End();
            }
        }

        public override void RegisterData(ImGuiView view)
        {
            ImageAsset asset = GetFirstAssetOfType<ImageAsset>();
            asset.Tex2D = ContentManagement.ContentManager.LoadTexture2D(asset.ContentName);
            asset.ImGuiID = new IntPtr(view.Textures.Register(asset.Tex2D)); 
        }
    }
}
