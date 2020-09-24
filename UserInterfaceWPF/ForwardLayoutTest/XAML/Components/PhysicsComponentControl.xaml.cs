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
    /// <summary>
    /// Logique d'interaction pour PhysicsComponentControl.xaml
    /// </summary>
    public partial class PhysicsComponentControl : UserControl, IComponent
    {
        public Entity selectedEntity { get; set; }
        public FFComponent engineData { get; set; }

        public PhysicsComponentControl(Entity entity)
        {
            InitializeComponent();

            selectedEntity = entity;
        }

        public void FillData()
        {
            //engineData = new FFPhysics(selectedEntity);
        }
        public void deleteComponent(object sender, RoutedEventArgs e)
        {
            var CMC = ContextMenuController.GetInstance();
            CMC.DeleteComponent(this);
        }
    }
}
