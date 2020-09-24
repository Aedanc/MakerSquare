using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;
using System.Windows.Input;
using Entities;
using System.Collections.Generic;
using System.Diagnostics;

namespace ForwardLayoutTest
{
    public partial class SearchBar : INotifyPropertyChanged
    {
        public List<Entity> DataListEntities = new List<Entity>();
        public List<Entity> DisplayDataListEntities = new List<Entity>();
        public MainWindow mainWindow;

        private string _SearchText;
        public string SearchText {
            get { return this._SearchText; }
            set { if (_SearchText != value)
                {
                    _SearchText = value;
                    changeDisplay();
                }
            }
        }

        public void changeDisplay()
        {
            if (DisplayDataListEntities != null)
                DisplayDataListEntities.RemoveRange(0, DisplayDataListEntities.Count);
            for (int i = 0; i < DataListEntities.Count; ++i)
            {
                if (DataListEntities[i].Name.Contains(_SearchText))
                {
                    DisplayDataListEntities.Add(DataListEntities[i]);
                    Debug.WriteLine(DataListEntities[i].Name);
                    Debug.WriteLine(DataListEntities.Count);
                }
            }
            if (mainWindow != null)
                mainWindow.EntitiesRefresh();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

    }
}
