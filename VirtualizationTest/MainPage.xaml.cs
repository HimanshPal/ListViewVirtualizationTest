using Newtonsoft.Json;
using PropertyChanged;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using VirtualizationTest.Extensions;
using VirtualizationTest.Services;
using VirtualizationTest.Models;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace VirtualizationTest
{
    [ImplementPropertyChanged]
    public sealed partial class MainPage : Page
    {
        //int number;
        //private int offset;
        private bool IsBusy;
        private const int Range = 50; 
        private const int LowerLimit = 0;
        private const int UpperLimit = 500;
        private const int StartingIndex = 250;
        private int CurrentUpIndex = StartingIndex;
        private int CurrentDownIndex = StartingIndex + Range;
        public const string ErrorMessage = "Something Went Wrong.";
        public const string Url = "http://jsonplaceholder.typicode.com/comments?_start=<startIndex>&_end=<endIndex>";
       
        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            
            ToastService = new ToastService();
            this.LayoutRoot.DataContext = this;
        }

        public ToastService ToastService { get; set; }
         
        public ObservableCollection<CommentsModel> CommentsCollection { get; set; }


        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            await StatusBarService.ShowProgressBar();
            var url = Url.Replace("<startIndex>", StartingIndex.ToString()).Replace("<endIndex>", CurrentDownIndex.ToString());
            await GetCommentsAsync(url);
            await StatusBarService.HideProgressBar();
        }

        private void listView_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("listView ActualHeight: " + listView.ActualHeight);
            Debug.WriteLine("scrollViewer ActualHeight: " + scrollViewer.ActualHeight);
            Debug.WriteLine("MyScrollViewer ActualHeight: " + scrollViewer.ExtentHeight);

            scrollViewer.ViewChanged += OnViewChanged;
        }

        private async void OnViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            ScrollViewer scrollViewer = (ScrollViewer)sender;
            Debug.WriteLine("ScrollViewer VerticalOffset: " + scrollViewer.VerticalOffset);
            if ((scrollViewer.VerticalOffset > 0.75 * scrollViewer.ScrollableHeight) & !IsBusy)
            {
                if (CurrentDownIndex < 500)
                {
                    IsBusy = true;
                    await StatusBarService.ShowProgressBar();
                    await GetMoreCommentsBasedOnDirectionOfScrollingAsync(false);
                    await StatusBarService.HideProgressBar();
                }

            }
            else if ((scrollViewer.VerticalOffset < 0.1 * scrollViewer.ScrollableHeight) & !IsBusy)
            {
                if (CurrentUpIndex > 0)
                {
                    IsBusy = true;
                    await StatusBarService.ShowProgressBar();
                    await GetMoreCommentsBasedOnDirectionOfScrollingAsync(true);
                    await StatusBarService.HideProgressBar();
                }

            }
        }

        private async Task GetCommentsAsync(string url)
        {
            var model = await GetHttpResponseAsync(url);
            if (model != null)
            {
                CommentsCollection = model;
                listView.ScrollIntoView(CommentsCollection.ElementAt(25));
            }
        }

        private async Task GetMoreCommentsBasedOnDirectionOfScrollingAsync(bool isUpward = false)
        {
            string url = string.Empty;
            if (isUpward)
            {
                var currentIndex = CurrentUpIndex;
                CurrentUpIndex -= Range;

                if (CurrentUpIndex < 0)
                {
                    //ToastService.ShowToast("ListFinshed Upward");
                    IsBusy = false;
                    return;
                }
                url = Url.Replace("<startIndex>", CurrentUpIndex.ToString()).Replace("<endIndex>", currentIndex.ToString());             
            }
            else
            {
                var currentIndex = CurrentDownIndex;
                CurrentDownIndex += Range;

                if (currentIndex >= 500)
                {
                    //ToastService.ShowToast("ListFinshed Downward");
                    IsBusy = false;
                    return;
                } 
                
                url = Url.Replace("<startIndex>", currentIndex.ToString()).Replace("<endIndex>", CurrentDownIndex.ToString());
            }

            var model = await GetHttpResponseAsync(url);
            if (model != null)
            {
                if (isUpward)
                {
                    var list = model.Reverse();

                    foreach (var comment in list)
                        CommentsCollection.Insert(0, comment);
                }
                else
                {
                    foreach (var comment in model)
                        CommentsCollection.Add(comment);
                }
            }
            IsBusy = false;
        }

        private async Task<ObservableCollection<CommentsModel>> GetHttpResponseAsync(string url)
        {
            ObservableCollection<CommentsModel> model = null;
            try
            {
                var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(new Uri(url));
                var payload = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode && !string.IsNullOrEmpty(payload))
                {
                    model = JsonConvert.DeserializeObject<ObservableCollection<CommentsModel>>(payload);
                    if(model == null) ToastService.ShowToast("Error", ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception: " + ex.Message);
            }

            return model;
        }
    
        private async void UpAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            //var x = scrollViewer.ChangeView(null, 0, null);
            //Debug.WriteLine("Result of ChangeView: " + x);
            Debug.WriteLine("ExtentHeight: " + scrollViewer.ExtentHeight);
            Debug.WriteLine("ViewportHeight: " + scrollViewer.ViewportHeight);


            await scrollViewer.ScrollToVerticalOffsetWithAnimation(0);
        }

        private async void DownAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            //var x = scrollViewer.ChangeView(null, scrollViewer.ExtentHeight, null);
            //Debug.WriteLine("Result of ChangeView: " + x);
            Debug.WriteLine("ExtentHeight: " + scrollViewer.ExtentHeight);
            Debug.WriteLine("ViewportHeight: " + scrollViewer.ViewportHeight);

            await scrollViewer.ScrollToVerticalOffsetWithAnimation(scrollViewer.ExtentHeight);
        }
    }
}
