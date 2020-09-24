using Engine.Components;
using Ultraviolet.ImGuiViewProvider;
using Ultraviolet.ImGuiViewProvider.Bindings;
using Ultraviolet.Graphics;
using Ultraviolet.Graphics.Graphics2D;
using System.Collections.Generic;
using Ultraviolet;
using System;

namespace Engine.System.UI
{
    [Serializable]
    public class UIAsset
    {
        public string ContentName;
    }

    [Serializable]
    public class ImageAsset : UIAsset
    {
        [NonSerialized]
        public Texture2D Tex2D;
        [NonSerialized]
        public IntPtr ImGuiID;
    }

    [Serializable]
    public class FontAsset : UIAsset
    {
        [NonSerialized]
        public UltravioletFont Font;
        [NonSerialized]
        public ImFontPtr ImGuiID;
    }

    [Serializable]
    public abstract class UIComponent : Component
    {
        protected List<UIAsset> assets;

        public UIComponent(Entity entity) : base(entity)
        {
            assets = new List<UIAsset>();
        }

        public abstract void RegisterData(ImGuiView view);
        public abstract void ImGuiUpdate(UltravioletTime time);

        public void AddAsset(UIAsset asset)
        {
            assets.Add(asset);
        }

        public T GetFirstAssetOfType<T>()
        {
            foreach (object asset in assets)
            {
                if (asset.GetType() == typeof(T))
                    return (T)asset;
            }
            return default(T);
        }
    }
}
