using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stocks_Windows_App.Models
{
    public class Stock
    {
        public string Ticker { get; set; }
        public string Exchange { get; set; }
    }
    public class Watchlist
    {
        public static List<Stock> GetTickers()
        {
            var tickers = new List<Stock>();

            tickers.Add(new Stock { Ticker = "AAPL" });
            tickers.Add(new Stock { Ticker = "MSFT" });
            
            return tickers;
        }
    }

    public class Searchlist
    {
        public static List<Stock> GetSearch()
        {
            var tickers = new List<Stock>();

            return tickers;
        }
    }
}
 