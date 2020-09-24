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
    public class RuleComponentSerializedData : ISerializedComponent
    {
        public void SetComponentData(Entity entity)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Logique d'interaction pour RuleComponentControl.xaml
    /// </summary>
    /// 
    public partial class RuleComponentControl : UserControl, IComponent
    {
        public Entity selectedEntity { get; set; }
        public List<FFComponent> engineData { get; set; }

        public RuleComponentControl(Entity entity)
        {
            InitializeComponent();
            engineData = new List<FFComponent>();
            selectedEntity = entity;
        }

        public void FillData()
        {
            engineData.Add(new FFRule(selectedEntity));
        }
        public void deleteComponent(object sender, RoutedEventArgs e)
        {
            var CMC = ContextMenuController.GetInstance();
            CMC.DeleteComponent(this);
        }

        public void AddSerializedComponent(Entity entity)
        {
            throw new NotImplementedException();
        }
    }
}
