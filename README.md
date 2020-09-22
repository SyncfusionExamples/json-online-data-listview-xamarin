# How to bind online JSON data to Xamarin.Forms ListView (SfListView)

You can fetch data from a web server and store it on a local server for Xamarin.Forms [SfListView](https://help.syncfusion.com/xamarin/listview/overview). You can refer to the document regarding offline JSON with ListView from [here](https://www.syncfusion.com/kb/11173/how-to-bind-json-data-to-xamarin-forms-listview-sflistview).

You can also refer the following article.

https://www.syncfusion.com/kb/11943/how-to-bind-online-json-data-to-xamarin-forms-listview-sflistview

**XAML**

Create Xamarin.Forms application with **SfListView**.
``` xml
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:local="clr-namespace:ListViewXamarin"
             xmlns:syncfusion="clr-namespace:Syncfusion.ListView.XForms;assembly=Syncfusion.SfListView.XForms"
             xmlns:data="clr-namespace:Syncfusion.DataSource;assembly=Syncfusion.DataSource.Portable"
             x:Class="ListViewXamarin.MainPage" Padding="{OnPlatform iOS='0,40,0,0'}">
    <ContentPage.BindingContext>
        <local:ListViewModel/>
    </ContentPage.BindingContext>
    
	 <ContentPage.Content>
        <StackLayout>
            <Grid BackgroundColor="#2196F3" HeightRequest="40">
                <Label Text="To Do Items" x:Name="headerLabel"  TextColor="White" FontAttributes="Bold" VerticalOptions="Center" HorizontalOptions="Center" />
            </Grid>
            <syncfusion:SfListView x:Name="listView" ItemSize="60" GroupHeaderSize="50" BackgroundColor="#FFE8E8EC" ItemsSource="{Binding Items}">
                <syncfusion:SfListView.ItemTemplate >
                    <DataTemplate>
                        ...
                    </DataTemplate>
                </syncfusion:SfListView.ItemTemplate>
                <syncfusion:SfListView.DataSource>
                    <data:DataSource>
                        <data:DataSource.GroupDescriptors>
                            <data:GroupDescriptor PropertyName="userId"/>
                        </data:DataSource.GroupDescriptors>
                    </data:DataSource>
                </syncfusion:SfListView.DataSource>
                <syncfusion:SfListView.GroupHeaderTemplate>
                    <DataTemplate>
                       ...
                    </DataTemplate>
                </syncfusion:SfListView.GroupHeaderTemplate>
            </syncfusion:SfListView>
        </StackLayout>
    </ContentPage.Content>
</ContentPage>
```
**C#**

Fetch data and download from the web server.

1. Using the [HTTPClient.GetAsync](https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient.getasync?view=netcore-3.1) method, retrieve data from the web server for the specified URL.

2. The [HTTPContent.ReadAsStreamAsync](https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpcontent.readasstreamasync?view=netcore-3.1) returns the data in stream format.

3. Use [Environment.GetFolderPath](https://docs.microsoft.com/en-us/dotnet/api/system.environment.getfolderpath?view=netcore-3.1) method to create a local path to store the retrieved data and combine 4. the path with the file name using [Path.Combine](https://docs.microsoft.com/en-us/dotnet/api/system.io.path.combine?view=netcore-3.1) method.

5. The [FileInfo.OpenWrite](https://docs.microsoft.com/en-us/dotnet/api/system.io.fileinfo.openwrite?view=netcore-3.1#System_IO_FileInfo_OpenWrite) creates a write only stream and write to the local file stream using [CopyToAsync](https://docs.microsoft.com/en-us/dotnet/api/system.io.stream.copytoasync?view=netcore-3.1#System_IO_Stream_CopyToAsync_System_IO_Stream_) method.

``` c#
namespace ListViewXamarin
{
    public class DataServices
    {
        private static HttpClient httpClient;

        public DataServices()
        {
            httpClient = new HttpClient();
        }

        /// <summary>
        /// Fetches data from web server and write it to the local file.
        /// </summary>
        /// <returns>Returns true, if data fetched from webserver and downloaded to the local location.</returns>
        public async Task<bool> DownloadJsonAsync()
        {
            try
            {
                var url = "https://jsonplaceholder.typicode.com/todos"; //Set your REST API url here

                //Sends request to retrieve data from the web service for the specified Uri
                var response = await httpClient.GetAsync(url);
                using (var stream = await response.Content.ReadAsStreamAsync()) //Reads data as stream
                {
                    //Gets the path to the specified folder
                    var localFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);

                    var newpath = Path.Combine(localFolder, "data.json"); // Combine path with the file name

                    var fileInfo = new FileInfo(newpath);

                    //Creates a write-only file stream
                    using (var fileStream = fileInfo.OpenWrite())
                    {
                        await stream.CopyToAsync(fileStream); //Reads data from the current stream and write to destination stream (fileStream)
                    }
                }
            }
            catch (OperationCanceledException e)
            {
                System.Diagnostics.Debug.WriteLine(@"ERROR {0}", e.Message);
                return false;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(@"ERROR {0}", e.Message);
                return false;
            }
            return true;
        }
    }
}
```

**C#**

Read data from local file and set it to collection.

``` c#
namespace ListViewXamarin
{
    public class ListViewModel : INotifyPropertyChanged
    {
        private dynamic items;
        private bool isDownloaded;

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

        public ListViewModel()
        {
            DataService = new DataServices();
            GetDataAsync();
        }

        private async void GetDataAsync()
        {
            isDownloaded = await DataService.DownloadJsonAsync();
            if (isDownloaded)
            {
                var localFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var fileText = File.ReadAllText(Path.Combine(localFolder, "data.json"));
                //Read data from the local file path and set it to the collection bound to the ListView.
                Items = JsonConvert.DeserializeObject<dynamic>(fileText);
            }
        }
    }
}
```
