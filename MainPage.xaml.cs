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
using System.Drawing;
using AngleSharp.Dom;
using AngleSharp.Text;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Stocks_Windows_App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private List<Stock> Watch_List;
        private ObservableCollection<Stock> SelectedStock;
        private Stock CurrentTicker;
        Uri Complete_Uri;
        private event EventHandler NavigationCompleted;
        private Dictionary<string, TextBlock> Financial_Dict = new Dictionary<string, TextBlock>();
        private string[,] Financial_Data = new string[9, 2];
        bool Get_FinancialData_Initial = true;

        public MainPage()
        {
            this.InitializeComponent();
            Watch_List = Watchlist.GetTickers();
            SelectedStock = new ObservableCollection<Stock>();
            SelectedStock.Add(Watch_List[0]);
            Title.Text = Watch_List[0].Ticker;

            Get_Chart(Watch_List[0].Ticker);

            Get_FinancialData(Watch_List[0].Ticker);


        }



        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var selectedTicker = (Stock)e.ClickedItem;
            Title.Text = selectedTicker.Ticker;

            Get_Chart(selectedTicker.Ticker);
            Get_FinancialData(selectedTicker.Ticker);
            
        }
        // Financial Data Methods

        private async void Get_FinancialData(string ticker)
        {
            if(Get_FinancialData_Initial == true)
            {
                Financial_Dict.Add("Previous Close", Prev_Value);
                Financial_Dict.Add("Open", Open_Value);
                Financial_Dict.Add("Day&#x27;s Range", Range_Value);
                Financial_Dict.Add("Volume", Vol_Value);
                Financial_Dict.Add("PE Ratio (TTM)", PE_Value);
                Financial_Dict.Add("Market Cap", Mkt_Cap_Value);
                Financial_Dict.Add("52 Week Range", Range_Value);
                Financial_Dict.Add("Forward Dividend &amp; Yield", Dividend_Value);
                Financial_Dict.Add("Beta (5Y Monthly)", Beta_Value);
                Get_FinancialData_Initial = false;
            }
           
            await Get_Html_Async(ticker);

            //Populates the financial stats 
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

        //Search bar methods
        private async void Search_Box_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (Search_Box.Text.Length != 0)
            {
                Canvas.SetZIndex(Main_Rect_2, 1);

                //Scrape, populate listview and set zindex to 0

                string input = Search_Box.Text;
                string site_url = "https://ca.finance.yahoo.com/lookup/equity?s=" + input;
                HttpClient httpClient = new HttpClient();

                var html = await httpClient.GetStringAsync(site_url);

                var html_document = new HtmlDocument();
                html_document.LoadHtml(html);

                var summary_html = html_document.DocumentNode.Descendants("table").Where(node => node.GetAttributeValue("class", "").Equals("lookup-table W(100%) Pos(r) BdB Bdc($tableBorderGray) smartphone_Mx(20px)")).ToList();
                var table_html = html_document.DocumentNode.Descendants("tr").Where(node => node.GetAttributeValue("data-reactid", "").Contains("")).ToList();

                List<Stock> search_list = new List<Stock>();

                bool first = true;
                
                foreach (var node in table_html)
                {
                    //Skips first node
                    if(first == true)
                    {
                        first = false;
                        continue;
                    }
                   

                    string ticker = "";
                    for(int i = 0; i<node.InnerText.Length; i++)
                    {
                        if (node.InnerText[i].IsLowercaseAscii()) break;
                        ticker = ticker + node.InnerText[i];
                    }
                    ticker = ticker.Remove(ticker.Length - 1); //Removes first letter of name
                    search_list.Add(new Stock { Ticker = ticker });

                    Master_ListView.ItemsSource = search_list;
                    Canvas.SetZIndex(Main_Rect_2, 0);
                }
                


            }
            else
            {
                Canvas.SetZIndex(Main_Rect_2, 0);
                Master_ListView.ItemsSource = Watch_List;
            }

        }

        // Charting Methods
        private async void Get_Chart(string ticker)
        {
            string BaseUri = "https://www.tradingview.com/chart/?symbol=";
            Complete_Uri = new Uri(BaseUri + ticker);
            Web_Chart.Navigate(Complete_Uri);
        }

        private async void Web_Chart_LoadCompleted(object sender, NavigationEventArgs e)
        {
            await Web_Chart.InvokeScriptAsync("eval", new string[] { "document.getElementById(\"header-toolbar-fullscreen\").click()" });
        }

        //Notification Methods

    }
}
