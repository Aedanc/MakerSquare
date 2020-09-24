using ForwardLayoutTest.Classes;
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
using Entities;
using MakerSquare.FrontFacingECS;
using ForwardLayoutTest.Controller;

namespace ForwardLayoutTest.XAML.Components
{
    [Serializable]
    public class InputComponentSerializedData : ISerializedComponent
    {
        public Dictionary<string, MakerSquare.FrontFacingECS.Tuple<string, string>> actions;

        public void SetComponentData(Entity entity)
        {
            var component = new InputComponentControl(entity);

            for (int i = 1; i < actions.Count; i++)
                component.KeyActionsPanel.Children.Add(DeepCopy<StackPanel>.DeepCopyMethod(component.ActionPanel));

            int actionPanel = 0;
            foreach (var action in actions)
            {
                AddAction(actionPanel, action, component);
                actionPanel++;
            }

            entity.AddComponent(component);
        }

        private void AddAction(int actionPanel, KeyValuePair<string, MakerSquare.FrontFacingECS.Tuple<string, string>> action,
            InputComponentControl component)
        {
            StackPanel child = (StackPanel)component.KeyActionsPanel.Children[actionPanel];
            ComboBox keyBox = (ComboBox)child.Children[1];
            ComboBox keyStateBox = (ComboBox)child.Children[2];
            ComboBox methodBox = (ComboBox)child.Children[3];

            string keyBoxValue = action.Key.Substring("Ultraviolet.Input.Key.".Length);
            string keyStateBoxValue = action.Value.Item1.Substring("OnKey.".Length);
            string methodBoxValue = action.Value.Item2;
                    
            int i = 0;
            foreach (ComboBoxItem item in keyBox.Items)
            {
                if (item.ToString().Substring("System.Windows.Controls.ComboBoxItem: ".Length) == keyBoxValue)
                {
                    keyBox.SelectedIndex = i;
                    break;
                }
                i++;
            }

            i = 0;
            foreach (ComboBoxItem item in keyStateBox.Items)
            {
                if (item.ToString().Substring("System.Windows.Controls.ComboBoxItem: ".Length) == keyStateBoxValue)
                {
                    keyStateBox.SelectedIndex = i;
                    break;
                }
                i++;
            }

            i = 0;
            foreach (ComboBoxItem item in methodBox.Items)
            {
                if (item.ToString().Substring("System.Windows.Controls.ComboBoxItem: ".Length) == methodBoxValue)
                {
                    methodBox.SelectedIndex = i;
                    break;
                }
                i++;
            }
        }
    }

    /// <summary>
    /// Logique d'interaction pour InputComponentControl.xaml
    /// </summary>
    public partial class InputComponentControl : UserControl, IComponent
    {
        public Entity selectedEntity { get; set; }
        public List<FFComponent> engineData { get; set; }

        Dictionary<string, MakerSquare.FrontFacingECS.Tuple<string, string, string>> actions;
        StackPanel basePanel;

        public InputComponentControl(Entity entity)
        {
            InitializeComponent();
            engineData = new List<FFComponent>();
            selectedEntity = entity;
            actions = new Dictionary<string, MakerSquare.FrontFacingECS.Tuple<string, string, string>>();
            basePanel = DeepCopy<StackPanel>.DeepCopyMethod(ActionPanel);
        }

        private void AddActionButtonClick(object sender, RoutedEventArgs e)
        {
            KeyActionsPanel.Children.Add(DeepCopy<StackPanel>.DeepCopyMethod(basePanel));
        }

        public void FillData()
        {
            actions.Clear();

            foreach (StackPanel child in KeyActionsPanel.Children)
            {
                ComboBox keyBox = (ComboBox)child.Children[1];
                ComboBox keyStateBox = (ComboBox)child.Children[2];
                ComboBox methodBox = (ComboBox)child.Children[3];
               
                string key = "Ultraviolet.Input.Key." + ((ComboBoxItem)keyBox.SelectedItem).Name;
                string keyState = "OnKey." + ((ComboBoxItem)keyStateBox.SelectedItem).Name;
                string invertedState = "OnKey.";
                string method = ((ComboBoxItem)methodBox.SelectedItem).Name;

                if (keyState.Contains("UP"))
                    invertedState += "DOWN";
                else if (keyState.Contains("DOWN"))
                    invertedState += "UP";
                else if (keyState.Contains("PRESSED"))
                    invertedState += "RELEASED";
                else if (keyState.Contains("RELEASED"))
                    invertedState += "PRESSED";

                actions.Add(key, new MakerSquare.FrontFacingECS.Tuple<string, string, string>(keyState, invertedState, method));
            }

            engineData.Add(new FFInput(selectedEntity, actions));
            engineData.Add(new FFMovement(selectedEntity));
        }
        public void deleteComponent(object sender, RoutedEventArgs e)
        {
            var CMC = ContextMenuController.GetInstance();
            CMC.DeleteComponent(this);
        }

        public void AddSerializedComponent(Entity entity)
        {
            var data = new InputComponentSerializedData();
            data.actions = new Dictionary<string, MakerSquare.FrontFacingECS.Tuple<string, string>>();
            foreach (StackPanel child in KeyActionsPanel.Children)
            {
                ComboBox keyBox = (ComboBox)child.Children[1];
                ComboBox keyStateBox = (ComboBox)child.Children[2];
                ComboBox methodBox = (ComboBox)child.Children[3];
                string key = "Ultraviolet.Input.Key." + keyBox.Text;
                string keyState = "OnKey." + keyStateBox.Text;
                string method = methodBox.Text;

                data.actions.Add(key, new MakerSquare.FrontFacingECS.Tuple<string, string>(keyState, method));
            }

            entity._serializedComponents.Add(data);
        }
    }
}
