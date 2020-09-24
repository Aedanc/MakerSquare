using Entities;
using MakerSquare.Compiler;
using MakerSquare.FileSystem;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ForwardLayoutTest.XAML
{
    /// <summary>
    /// Logique d'interaction pour BuildWindow.xaml
    /// </summary>
    public partial class BuildWindow : Window
    {
        string fileName = "";
        string filePath = "";

        MainWindow mw;
        Compiler compiler;

        public BuildWindow(MainWindow mw)
        {
            InitializeComponent();

            this.mw = mw;
            compiler = new Compiler();

            Init();
        }

        public void Init()
        {            
            compiler.AddDependency(@"Engine.dll");
            compiler.AddDependency(@"SavingSystem.dll");
            compiler.AddDependency(@"FileSystemManager.dll");
            compiler.AddDependency(@"Ultraviolet.Shims.Desktop.dll");
            compiler.AddDependency(@"Ultraviolet.BASS.dll");
            compiler.AddDependency(@"Ultraviolet.Core.dll");
            compiler.AddDependency(@"Ultraviolet.OpenGL.dll");
            compiler.AddDependency(@"Ultraviolet.OpenGL.Bindings.dll");
            compiler.AddDependency(@"Ultraviolet.SDL2.dll");
            compiler.AddDependency(@"Ultraviolet.ImGuiViewProvider.dll");
            compiler.AddDependency(@"Ultraviolet.FreeType2.dll");
            compiler.AddDependency(@"Ultraviolet.dll");
            compiler.AddDependency(@"MonoGame.Framework.dll");
            compiler.AddDependency(@"Aether.Physics2D.dll");
        }

        private void BuildPathBrowseClick(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            CommonFileDialogResult result = dialog.ShowDialog();

            try
            {
                filePath = dialog.FileName;
                PathTextField.Text = filePath;
            }
            catch (Exception ex)
            {
            }
        }

        private void BuildButtonClick(object sender, RoutedEventArgs e)
        {
            fileName = FileTextField.Text;

            if (filePath.Length == 0)
                MessageBox.Show("Erreur: Veuillez entrer un chemin");
            else if (fileName.Length == 0)
                MessageBox.Show("Error: Veuillez entrer un nom");
            else
            {
                try
                {
                    mw.entitiesController.SaveEntitiesData();
                    FFScene scene = new FFScene(mw.entitiesController.GetCanvasEntities());
                    Codegen.Codegen.CodegenScene(Manager.Instance, scene.entities, compiler);

                    compiler.SetResourceFolder(Manager.Instance.ProjectDir);
                    compiler.SetExecutableName(fileName);
                    compiler.SetExecPath(filePath);

                    compiler.CompileToPortableExec();

                    var current_working_dir = Directory.GetCurrentDirectory();
                    var filepath = GetFilePath();
                    var process_working_dir = filepath.Substring(0, filepath.LastIndexOf("\\"));

                    Directory.SetCurrentDirectory(process_working_dir);
                    Process.Start(GetFilePath());

                    foreach (Entity entity in mw.entitiesController.GetCanvasEntities())
                    {
                        entity.GetComponents().ForEach(x => x.engineData.Clear());
                    }

                    Directory.SetCurrentDirectory(current_working_dir);

                }
                catch (Exception exception)
                {
                #if DEBUG
                    throw exception;
                #else
                    MessageBox.Show("Il semblerait qu'il y ait eu une erreur à la compilation.", "Erreur de Compilation");
                #endif
                }
            }
        }

        public string GetFilePath()
        {
            string resultPapth = "";

            resultPapth += filePath + "\\";
            resultPapth += fileName + ".exe";
            return resultPapth;
        }

        private void FileNameValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[a-zA-Z0-9]*$");
            e.Handled = !regex.IsMatch(e.Text);
        }
    }
}
