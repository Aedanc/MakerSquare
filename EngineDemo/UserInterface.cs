using System.Diagnostics;
using System;
using Engine;
using Engine.System.UI;
using Ultraviolet;

namespace EngineDemo
{
    [Serializable]
    public class UserInterface : Entity
    {
        public UserInterface(int Score) : base(true)
        {
            Name = "Score";

            var text_cmp = new UITextComponent(this);
            Transform.x = 550;
            Transform.y = 10;
            text_cmp.Text = "Score " + Score.ToString();
            text_cmp.Color = new Vector4(75, 75, 0, 1);
            text_cmp.FontSize = 26;
            var font_asset = new FontAsset();
            font_asset.ContentName = "coolvetica rg.ttf";

            text_cmp.AddAsset(font_asset);
            AddComponent(text_cmp);
            Debug.WriteLine("User Interface constructor called");
        }
    }
}
