using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ListViewXamarin
{
    public class ListViewModel : INotifyPropertyChanged
    {
        #region Fields

        private dynamic items;
        private bool isDownloaded;
        #endregion

        #region Properties

        public dynamic Items
        {
            get { return items; }
            set
            {
                items = value;
                OnPropertyChanged("Items");
            }
        }
        public DataServices DataService { get; set; }
        #endregion

        #region Constructor

        public ListViewModel()
        {
            DataService = new DataServices();
            GetDataAsync();
        }
        #endregion

        #region Methods
        private async void GetDataAsync()
        {
            isDownloaded = await DataService.DownloadJsonAsync();
            if (isDownloaded)
            {
                var localFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var fileText = File.ReadAllText(Path.Combine(localFolder, "data.json"));
                //Read data from the local path and set it to the collection bound to the ListView.
                Items = JsonConvert.DeserializeObject<dynamic>(fileText);
            }
        }
        #endregion

        #region Interface Member

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string name)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
