using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ZhangZiShiRssRead
{
    public class MainViewModel : ViewModelBase, IUpdateLayout
    {
        private const string ALL = "ALL";

        private FileStoreHelper fileStoreHelper = new FileStoreHelper();
        private RssReader rssReader = new RssReader();

        private List<Item> AllItemList { get; set; } = new List<Item>();
        public ObservableCollection<Item> Items { get; set; } = new ObservableCollection<Item>();

        private List<string> categoryList;
        public List<string> CategoryList
        {
            get
            { return categoryList; }
            set
            { Set(ref categoryList, value); }
        }

        private string selectedCategory;

        public string SelectedCategory
        {
            get { return selectedCategory; }
            set
            {
                Set(ref selectedCategory, value);
                InitItems(AllItemList, selectedCategory);
                IsOpen = false;
            }
        }

        private bool isOpen;

        public bool IsOpen
        {
            get { return isOpen; }
            set
            {
                isOpen = value;
                this.RaisePropertyChanged();
            }
        }

        private bool isLoading;

        public bool IsLoading
        {
            get { return isLoading; }
            set { Set(ref isLoading, value); }
        }

        private Item selectItem;

        public event EventHandler UpdateLayoutEvent;

        public Item SelectedItem
        {
            get
            {
                return selectItem;
            }
            set
            {
                var oldValue = selectItem;
                Set(ref selectItem, value);
                if (selectItem != oldValue)
                {
                    ContentEncoded = SelectedItem?.ContentEncoded;
                    this.UpdateLayoutEvent?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private string contentEncoded;
        public string ContentEncoded
        {
            get
            {
                return contentEncoded;
            }
            set
            {
                Set(ref contentEncoded, value);
            }
        }


        public ICommand OpenPaneCommand { get; set; }
        public ICommand GoBackCommand { get; set; }
        public ICommand SyncCommand { get; set; }

        public MainViewModel()
        {
            InitData();
            OpenPaneCommand = new RelayCommand(OpenPane);
            GoBackCommand = new RelayCommand(GoBack);
            SyncCommand = new RelayCommand(InitData);
            Messenger.Default.Register<GenericMessage<string>>(this, HandleMessage);
        }

        private void HandleMessage(GenericMessage<string> message)
        {
            if (message.Content == Const.ReloadItemList)
            {
                InitData();
            }
            else if (message.Content == Const.GoBack)
            {
                GoBack();
            }
        }

        private void GoBack()
        {
            SelectedItem = null;
        }

        private void OpenPane()
        {
            IsOpen = true;
        }

        private async void InitData()
        {
            await LoadLocalDataAsync();
            InitItems(AllItemList, ALL);

            IsLoading = true;
            var isSuccess = await LoadRemoteDataAsync();
            if (isSuccess)
            {
                InitItems(AllItemList, ALL);
            }
            IsLoading = false;

            CategoryList = AllItemList.Select(_ => _.Category).Distinct().ToList();
            CategoryList.Insert(0, ALL);
        }

        private void InitItems(List<Item> allItemList, string category)
        {
            var itemList = new List<Item>();
            if (category == ALL)
            {
                itemList = allItemList;
            }
            else
            {
                itemList = allItemList.Where(_ => _.Category == category).ToList();
            }

            Items.Clear();
            foreach (var item in itemList)
            {
                Items.Add(item);
            }
        }

        private async Task<bool> LoadRemoteDataAsync()
        {
            var rssContent = await rssReader.DownloadRssString();
            var rssNode = rssReader.GetRssNode(rssContent);
            if (rssReader.HasNewItems(rssNode) == false)
            {
                return false;
            }
            
            var isSuccess = await fileStoreHelper.SaveRssFileAsync(rssContent);
            if (isSuccess)
            {
                AllItemList = rssReader.ParseRss(rssNode);
                return true;
            }

            return false;
        }

        private async Task<bool> LoadLocalDataAsync()
        {
            var rssContent = await fileStoreHelper.ReadRssFileAsync();
            if (string.IsNullOrEmpty(rssContent))
            {
                return false;
            }

            var rssNode = rssReader.GetRssNode(rssContent);

            AllItemList = rssReader.ParseRss(rssNode);
            return true;
        }  
    }
}
