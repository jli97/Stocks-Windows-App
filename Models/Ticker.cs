using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stocks_Windows_App.Models
{
    public class Ticker
    {
        public string Name { get; set; }
    }
    public class Watchlist
    {
        public static List<Ticker> GetTickers()
        {
            var tickers = new List<Ticker>();

            tickers.Add(new Ticker { Name = "AAPL" });
            tickers.Add(new Ticker { Name = "MSFT" });
            
            return tickers;
        }
    }
}
 