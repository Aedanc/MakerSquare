using System;
using Ultraviolet;
using Ultraviolet.ImGuiViewProvider;
using Ultraviolet.ImGuiViewProvider.Bindings;

namespace Engine.System.UI
{
    [Serializable]
    public class UITextComponent : UIComponent
    {
        public string Text { get; set; }
        public float FontSize { get; set; }
        public Vector4 Color { get; set; }

        public UITextComponent(Entity entity) : base(entity)
        {
            Text = "Texte par défaut";
            FontSize = 12;
            Color = new Vector4(1, 1, 1, 1);
        }

        public override void ImGuiUpdate(UltravioletTime time)
        {
            FontAsset asset = GetFirstAssetOfType<FontAsset>();
            ImGui.SetNextWindowBgAlpha(0);
            ImGui.SetNextWindowPos(Entity.Transform.ToVector2());
            if (ImGui.Begin(Text, ImGuiFlagsPresets.InvisibleWindow))
            {
                ImGui.PushFont(asset.ImGuiID);
                ImGui.TextColored(Color, Text);
                ImGui.PopFont();
                ImGui.End();
            }
            
        }

        public override void RegisterData(ImGuiView view)
        {
            FontAsset asset = GetFirstAssetOfType<FontAsset>();           
            asset.ImGuiID = view.Fonts.RegisterFromAssetTTF(
                ContentManagement.ContentManager.GetUVContent(),
                "FreeTypeFonts\\" + asset.ContentName, FontSize);
        }
    }
}
