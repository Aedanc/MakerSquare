using Entities;
using ForwardLayoutTest.Classes;
using ForwardLayoutTest.Controller;
using MakerSquare.FileSystem;
using MakerSquare.FrontFacingECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ForwardLayoutTest.XAML.Components
{
    [Serializable]
    public class AudioComponentSerializedData : ISerializedComponent
    {
        public string sound_name;
        public bool plays_on_start;

        public void SetComponentData(Entity entity)
        {            
            var audio = new AudioComponentControl(entity);            
            audio.ChangeSong(sound_name);            
            audio.SetPlayOnStart(plays_on_start);
            entity.AddComponent(audio);
        }
    }

    /// <summary>
    /// Logique d'interaction pour AudioComponent.xaml
    /// </summary>
    public partial class AudioComponentControl : UserControl, IComponent
    {
        public Entity selectedEntity { get; set; }
        public List<FFComponent> engineData { get; set; }

        private bool bPlayOnStart;
        AssetSearcher assetSearcher;
        string fileName;
        Window window;

        public AudioComponentControl(Entity entity)
        {
            InitializeComponent();
            engineData = new List<FFComponent>();
            selectedEntity = entity;
        }

        private void SearchAudioFileClick(object sender, RoutedEventArgs e)
        {
            assetSearcher = new AssetSearcher(EFileType.SOUND, SoundAssetButtonClick);

            window = new Window
            {
                Title = "Select Asset",
                Content = assetSearcher,
                Height = 450,
                Width = 330,
            };
            window.ShowDialog();
        }

        public void SoundAssetButtonClick(object sender, RoutedEventArgs e)
        {
            Button obj = sender as Button;

            ChangeSong(obj.Name);
            window.Close();
        }

        public void FillData()
        {
            bPlayOnStart = PlayCheckBox.IsChecked.Value;
            engineData.Add(new FFAudioComponent(selectedEntity, fileName, bPlayOnStart));
        }

        public void deleteComponent(object sender, RoutedEventArgs e)
        {
            var CMC = ContextMenuController.GetInstance();
            CMC.DeleteComponent(this);
        }

        public void AddSerializedComponent(Entity entity)
        {
            var data = new AudioComponentSerializedData();
            data.sound_name = fileName;
            data.plays_on_start = bPlayOnStart;
            entity._serializedComponents.Add(data);
        }

        public void ChangeSong(string song_name)
        {            
            fileName = song_name;
            TextFileName.Text = song_name;
            TextFileName.Visibility = Visibility.Visible;
        }

        public void SetPlayOnStart(bool play_on_start)
        {
            bPlayOnStart = play_on_start;
            PlayCheckBox.IsChecked = play_on_start;            
        }

    }
}
