using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace ZhangZiShiRssRead
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private GridLength zeroGridLength = new GridLength(0);
        private GridLength oneStarGridLength = new GridLength(1,GridUnitType.Star);
        private GridLength fourStarGridLength = new GridLength(4, GridUnitType.Star);
        private GridLength sixStarGridLength = new GridLength(6, GridUnitType.Star);

        public MainPage()
        {
            this.InitializeComponent();
            this.SizeChanged += MainPage_SizeChanged;

            var vm = this.DataContext as MainViewModel;
            vm.UpdateLayoutEvent += UpdateSplitViewLayout;

            SystemNavigationManager.GetForCurrentView().BackRequested += (sender, e) =>
            {
                if (vm.SelectedItem != null)
                {
                    vm.SelectedItem = null;
                    e.Handled = true;
                }
            };

            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                StatusBar.GetForCurrentView().BackgroundColor = Colors.Transparent;
                StatusBar.GetForCurrentView().ForegroundColor = Colors.Black;
            }
        }

        private void UpdateSplitViewLayout(object sender, EventArgs e)
        {
            UpdateLayout(this.ActualWidth);
        }

        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateLayout(e.NewSize.Width);
        }

        private void UpdateLayout(double newWidth)
        {
            if (newWidth <= 800)
            {
                this.splitView.DisplayMode = SplitViewDisplayMode.Overlay;
                this.borderMiddle.Width = 0;
                if (listViewItems.SelectedItem == null)
                {
                    columnRight.Width = zeroGridLength;
                    columnLeft.Width = oneStarGridLength;
                    columnRightBar.Width = zeroGridLength;
                    columnLeftBar.Width = oneStarGridLength;
                }
                else
                {
                    columnLeft.Width = zeroGridLength;
                    columnRight.Width = oneStarGridLength;
                    columnLeftBar.Width = zeroGridLength;
                    columnRightBar.Width = oneStarGridLength;
                }
            }
            else
            {
                columnLeft.Width = fourStarGridLength;
                columnRight.Width = sixStarGridLength;
                columnLeftBar.Width = fourStarGridLength;
                columnRightBar.Width = sixStarGridLength;
                this.splitView.DisplayMode = SplitViewDisplayMode.CompactOverlay;
                this.borderMiddle.Width = 48;
            }
        }
    }
}
