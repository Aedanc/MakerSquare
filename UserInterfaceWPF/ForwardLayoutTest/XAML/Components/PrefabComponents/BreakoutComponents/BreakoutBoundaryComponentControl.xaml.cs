using Entities;
using ForwardLayoutTest.Classes;
using ForwardLayoutTest.Controller;
using MakerSquare.FrontFacingECS;
using MakerSquare.FrontFacingECS.PrefabEntities;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ForwardLayoutTest.XAML.Components
{

    [Serializable]
    public class BreakoutBoundarySerializedData : ISerializedComponent
    {
        public System.Tuple<int, int> size;
        public bool breaks_ball;

        public void SetComponentData(Entity entity)
        {
            var breakout = new BreakoutBoundaryComponentControl(entity);
            
            entity.AddComponent(breakout);
        }
    }

    public partial class BreakoutBoundaryComponentControl : UserControl, IComponent
    {
        public Entity selectedEntity { get; set; }
        public List<FFComponent> engineData { get; set; }

        public BreakoutBoundaryComponentControl(Entity entity)
        {
            InitializeComponent();
            engineData = new List<FFComponent>();
            selectedEntity = entity;
        }

        public void AddSerializedComponent(Entity entity)
        {
            var data = new BreakoutBoundarySerializedData();
            data.breaks_ball = DestroyCheckBox.IsChecked.Value;
            data.size = new System.Tuple<int, int>(Int32.Parse(SizeXTextBox.Text),
                Int32.Parse(SizeYTextBox.Text));
            entity._serializedComponents.Add(data);
        }

        public void FillData()
        {
            var data = new FFBreakoutBoundaryComponent(selectedEntity);
            data.destroys_ball = DestroyCheckBox.IsChecked.Value;
            data.size_x = Int32.Parse(SizeXTextBox.Text);
            data.size_y = Int32.Parse(SizeYTextBox.Text);            
            engineData.Add(data);
        }

        public void DeleteComponent(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Êtes vous sûr de vouloir supprimer les composants et entités en rapport avec le préfabriqué de Casse-Briques ?", "Suppression", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                PrefabController.Instance.DeletePrefab(PrefabEnum.BREAKOUT);
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}