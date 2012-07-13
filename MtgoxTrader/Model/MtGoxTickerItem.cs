//****************************************************************************             
//
// @File: MtGoxTickerItem.cs
// @owner: iamapi 
//    
// Notes:
//	
// @EndHeader@
//****************************************************************************
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace MtGoxTrader.Model
{    
    public class MtGoxTickerItem
    {
        
        public double high { get; set; }
        
        public double low { get; set; }
        
        public double avg { get; set; }
        
        public double vwap { get; set; }
        
        public double vol { get; set; }
        
        public double last { get; set; }
        
        public double buy { get; set; }
        
        public double sell { get; set; }

        public double last_all { get; set; }

        public double last_local { get; set; }

        public MtGoxTickerItem()
        { }

        public MtGoxTickerItem(double high, double low, double avg, double vwap, double vol, double last_all, double last_local, double last, double buy, double sell)
        {
            this.avg = avg;
            this.buy = buy;
            this.high = high;
            this.last = last;
            this.low = low;
            this.sell = sell;
            this.vol = vol;
            this.vwap = vwap;
            this.last_all = last_all;
            this.last_local = last_local;
        }

        public static MtGoxTickerItem getObjects(string jsonDataStr)
        { 
            string json = jsonDataStr;
            var serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new[] { new DynamicJsonConverter() });
            dynamic obj = serializer.Deserialize(json, typeof(object));
            MtGoxTickerItem item = new MtGoxTickerItem();
            item.avg = double.Parse(obj.ticker.avg.ToString());
            item.buy = double.Parse(obj.ticker.buy.ToString());
            item.high = double.Parse(obj.ticker.high.ToString());
            item.last = double.Parse(obj.ticker.last.ToString());
            item.low = double.Parse(obj.ticker.low.ToString());
            item.sell = double.Parse(obj.ticker.sell.ToString());
            item.vol = double.Parse(obj.ticker.vol.ToString());
            item.vwap = double.Parse(obj.ticker.vwap.ToString());
            item.last_all = double.Parse(obj.ticker.last_all.ToString());
            item.last_local = double.Parse(obj.ticker.last_local.ToString());
            return item;
        }            
    }
}
