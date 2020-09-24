using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FS = MakerSquare.FileSystem;
using System.Text.RegularExpressions;

namespace ForwardLayoutTest.XAML
{
    /// <summary>
    /// Interaction logic for FileAddingDialog.xaml
    /// </summary>
    public partial class FileAddingDialog : Window
    {
        private FileExplorer explorer;

        private string _resourceDir = "";

        private Microsoft.Win32.OpenFileDialog _dialog;

        private Dictionary<int, string> _filterToDirectory = new Dictionary<int, string>{
            { 1, FS.Manager.Instance.SpriteDirName },
            { 2, FS.Manager.Instance.SongDirName },
            { 3, FS.Manager.Instance.FontDirName }};


        private Dictionary<int, string> _filterToFRDirectory = new Dictionary<int, string>{
            { 1, "Images" },
            { 2, "Musiques" },
            { 3, "Polices" }};


        public FileAddingDialog()
        {
            InitializeComponent();
        }

        public FileAddingDialog(FileExplorer explorer)
        {
            InitializeComponent();
            this.explorer = explorer;
        }

        private void FileButton_Click(object sender, RoutedEventArgs e)
        {
            _dialog = new Microsoft.Win32.OpenFileDialog();
            _dialog.Filter =
            "Images (*.jpg; *.jpeg; *.bmp; *.png)|*.jpg; *.jpeg; *.bmp; *.png;|" +
            "Musique (*.mp3; *.wav;)|*.mp3; *.wav;|" +
            "Polices d'écriture (*.ttf; *.otf; *.woff)|*.ttf; *.otf; *.woff;";

            if ((bool)_dialog.ShowDialog())
                FileTextField.Text = _dialog.FileName;
            else
                FileTextField.Text = "Clique sur le bouton à droite pour choisir un fichier !";
        }

        private void FileTextField_TextChanged(object sender, TextChangedEventArgs e)
        {
            _resourceDir = "";
            if (!File.Exists(FileTextField.Text))
                return;
            _resourceDir = _filterToDirectory[_dialog.FilterIndex];
            CheckNameAvailability();
        }

        private Regex mask_regex = new Regex(@"[^A-z0-9+\-\*_]");
        private bool CheckNameAvailability()
        {
            //Ensure caret position
            var caret_idx = NameTextField.CaretIndex;
            var old_length = NameTextField.Text.Length;
            NameTextField.Text = mask_regex.Replace(NameTextField.Text, "");
            NameTextField.CaretIndex = caret_idx - (old_length - NameTextField.Text.Length);

            var result = FS.Manager.Instance.CheckNameAvailability(NameTextField.Text);            
            if (!result)
            {
                if (NameTextField.Text.Length == 0)
                    NameValidationField.Content = "Nom vide.";
                else
                    NameValidationField.Content = "Nom déjà pris.";
                NameValidationField.Foreground = Brushes.Red;
                AddResourceButton.IsEnabled = false;
            }
            else
            {
                NameValidationField.Content = "Nom valide.";
                NameValidationField.Foreground = Brushes.Green;
                AddResourceButton.IsEnabled = File.Exists(FileTextField.Text);
            }
            return false;
        }

        private void NameTextField_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckNameAvailability();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            explorer.AddFile(FileTextField.Text, NameTextField.Text, _filterToDirectory[_dialog.FilterIndex]);            
            MessageBox.Show(this, String.Format("Ressource ajoutée avec succès dans le dossier \"{0}\" !", _filterToFRDirectory[_dialog.FilterIndex]));
            this.Close();
        }
    }
}
