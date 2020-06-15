using Stocks_Windows_App.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Threading;
using System.Xml;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Net.Http;
using HtmlAgilityPack;

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
        Uri Complete_Uri;
        private event EventHandler NavigationCompleted;
        private Dictionary<string, TextBlock> Financial_Dict = new Dictionary<string, TextBlock>();
        private string[,] Financial_Data = new string[9, 2];


        public MainPage()
        {
            this.InitializeComponent();
            Tickers = Watchlist.GetTickers();
            SelectedTickers = new ObservableCollection<Ticker>();
            SelectedTickers.Add(Tickers[0]);
            Ticker_Name.Text = Tickers[0].Name;

            Get_Chart(Tickers[0].Name);

            Financial_Dict.Add("Previous Close", Prev_Value);
            Financial_Dict.Add("Open", Open_Value);
            Financial_Dict.Add("Day&#x27;s Range", Range_Value);
            Financial_Dict.Add("Volume", Vol_Value);
            Financial_Dict.Add("PE Ratio (TTM)", PE_Value);
            Financial_Dict.Add("Market Cap", Mkt_Cap_Value);
            Financial_Dict.Add("52 Week Range", Range_Value);
            Financial_Dict.Add("Forward Dividend &amp; Yield", Dividend_Value);
            Financial_Dict.Add("Beta (5Y Monthly)", Beta_Value);

            Get_FinancialData(Tickers[0].Name);

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

            Get_Chart(selectedTicker.Name);
            Get_FinancialData(selectedTicker.Name);
            
        }
        // Financial Data Methods

        private async void Get_FinancialData(string ticker)
        { 

            await Get_Html_Async(ticker);

            for(int i = 0; i<Financial_Data.Length/2; i++)
            {
                string key = Financial_Data[i, 0];
                TextBlock target = Financial_Dict[key];
                target.Text = Financial_Data[i, 1];
            }   


        }
        private async Task Get_Html_Async(string ticker) //Scrapes Yahoo Finance
        {
            
            string site_url = "https://ca.finance.yahoo.com/quote/" + ticker;
            HttpClient httpClient = new HttpClient();

            var html = await httpClient.GetStringAsync(site_url);

            var html_document = new HtmlDocument();
            html_document.LoadHtml(html);

            var summary_html = html_document.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("data-test", "").Equals("quote-summary-stats")).ToList();

            var data_html = html_document.DocumentNode.Descendants("tr").ToList();
            
            int idx = 0;
            foreach (var cur in data_html)
            {
                foreach (var key in Financial_Dict.Keys)
                {
                    if (cur.InnerText.Contains(key))
                    {
                        var value = cur.InnerText.Split(new String[] { key }, StringSplitOptions.None);
                        
                        if (value[0].Equals(""))
                        {
                            Financial_Data[idx, 0] = key;
                            Financial_Data[idx, 1] = value[1];
                            idx++;
                        }

               
                    }
                }
            }

        }

            
        // Charting Methods
        private async void Get_Chart(string ticker)
        {
            string BaseUri = "https://www.tradingview.com/chart/?symbol=";
            Complete_Uri = new Uri(BaseUri + ticker);
            Web_Chart.Navigate(Complete_Uri);
            await System.Threading.Tasks.Task.Delay(10000); //Fix this later, figure out how to get webview to wait before exeucting javascript
            //await Web_Chart.InvokeScriptAsync("eval", new string[] { "document.getElementById(\"header-toolbar-fullscreen\").click()" });
        }

        private async void Test_Click(object sender, RoutedEventArgs e)
        {
            await Web_Chart.InvokeScriptAsync("eval", new string[] { "document.getElementById(\"header-toolbar-fullscreen\").click()" }); 
        }
    }
}
