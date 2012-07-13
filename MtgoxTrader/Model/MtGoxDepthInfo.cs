//****************************************************************************             
//
// @File: MtGoxDepthInfo.cs
// @owner: iamapi 
//    
// Notes:
//	
// @EndHeader@
//****************************************************************************
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System;


namespace MtGoxTrader.Model
{   
    public class MtGoxDepthInfo
    {
        private const double epsilon = 0.0000001;

        public List<DepthInfoItem> askGroup { get; set; }

        public List<DepthInfoItem> bidGroup { get; set; }

        public List<MtGoxAsk> asks { get; set; }

        public List<MtGoxBid> bids { get; set; }

        public MtGoxDepthInfo()
        {
            this.asks = new List<MtGoxAsk>();
            this.bids = new List<MtGoxBid>();
            this.askGroup = new List<DepthInfoItem>();
            this.bidGroup = new List<DepthInfoItem>();
        }        
       
        public static MtGoxDepthInfo getObjects(string jsonDataStr)
        {
            MtGoxDepthInfo depthInfo = new MtGoxDepthInfo();
            string json = jsonDataStr;
            var serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new[] { new DynamicJsonConverter() });
            dynamic obj = serializer.Deserialize(json, typeof(object));  
            double itemPrice = 0, itemAmount = 0, lastPrice = 0, amount = 0, btcCount = 0, fundCount = 0;
            int itemNO = 0;
            for (int i = 0; i < obj.asks.Count; i++)
            {
                MtGoxAsk aa = new MtGoxAsk();
                aa.price = double.Parse(obj.asks[i][0].ToString());
                aa.amount = double.Parse(obj.asks[i][1].ToString());
                depthInfo.asks.Add(aa);
                itemPrice = aa.price;
                itemAmount = aa.amount;
                itemPrice = Math.Round(itemPrice, 2);
                if ( i == 0 || Math.Abs(lastPrice - itemPrice) < epsilon)
                {
                    amount += itemAmount;
                    itemNO++;
                    btcCount += itemAmount;
                    fundCount += itemPrice * itemAmount;
                }
                else
                {
                    DepthInfoItem a = new DepthInfoItem();
                    a.Price = lastPrice;
                    a.ItemNO = itemNO;
                    a.FundCount = fundCount;
                    a.BtcCount = btcCount;
                    a.Amount = amount;
                    depthInfo.askGroup.Add(a);
                    itemNO = 0;
                    amount = 0;
                }
                lastPrice = itemPrice;
                if (i > 1500)
                    break;
            }

            itemPrice = 0;
            itemAmount = 0; lastPrice = 0; amount = 0; btcCount = 0; fundCount = 0;
            itemNO = 0;
            for (int i = obj.bids.Count - 1, j = 0; i >= 0; i--, j++)
            {
                MtGoxBid bb = new MtGoxBid();
                bb.price = double.Parse(obj.bids[i][0].ToString());
                bb.amount = double.Parse(obj.bids[i][1].ToString());
                depthInfo.bids.Add(bb);
                itemPrice = bb.price;
                itemAmount = bb.amount;
                itemPrice = Math.Round(itemPrice, 2);
                if (i == obj.bids.Count - 1 || Math.Abs(lastPrice - itemPrice) < epsilon)
                {
                    amount += itemAmount;
                    itemNO++;
                    btcCount += itemAmount;
                    fundCount += itemPrice * itemAmount;
                }
                else
                {
                    DepthInfoItem a = new DepthInfoItem();
                    a.Price = lastPrice;
                    a.ItemNO = itemNO;
                    a.FundCount = fundCount;
                    a.BtcCount = btcCount;
                    a.Amount = amount;
                    depthInfo.bidGroup.Add(a);
                    itemNO = 0;
                    amount = 0;
                }
                lastPrice = itemPrice;
                if (j > 1500)
                    break;                
            }
            return depthInfo;
        }
    }
}
