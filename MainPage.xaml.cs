using Stocks_Windows_App.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Stocks_Windows_App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private List<Ticker> Tickers;
        private ObservableCollection<Ticker> SelectedTickers;
        private Ticker CurrentTicker;
        private string BaseUri = "https://www.tradingview.com/chart/?symbol=";
        private Uri Complete_Uri;

        public MainPage()
        {
            this.InitializeComponent();
            Tickers = Watchlist.GetTickers();
            SelectedTickers = new ObservableCollection<Ticker>();
            SelectedTickers.Add(Tickers[0]);

            Complete_Uri = new Uri(BaseUri + Tickers[0].Name);
            
            Web_Chart.Navigate(Complete_Uri);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var selectedTicker = (Ticker)e.ClickedItem;
            Ticker_Name.Text = selectedTicker.Name;

            Complete_Uri = new Uri(BaseUri + selectedTicker.Name);
            Web_Chart.Navigate(Complete_Uri);
        }
    }
}
