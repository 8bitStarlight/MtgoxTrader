//****************************************************************************             
//
// @File: v0.cs
// @owner: iamapi 
//    
// Notes:
//	
// @EndHeader@
//****************************************************************************
using System;
using System.Collections.Generic;
using System.Configuration;
using MtGoxTrader.Model;

namespace MtGoxTrader.MtGoxAPIClient
{
    /// <summary>
    /// This class implements MtGoxAPIV0 of the MtGox API as specified at https://en.bitcoin.it/wiki/MtGox/API#Methods_API_version_0
    /// </summary>
    public class MtGoxAPIV0
    {
        private string baseURL = "https://mtgox.com/api/";
        
        public string apiKey;

        public string apiSecret;

        public MtGoxAPIV0()
        {
        }

        /// <summary>
        /// 0/data/getTrades.php 
        /// </summary>
        public List<MtGoxTrade> getTrades(string sinceTradeID)
        {
            try
            {
                string url = (this.baseURL) + "0/data/getTrades.php";
                string postData = "";
                if (sinceTradeID != "")
                    url += "?since=" + sinceTradeID;
                string responseStr = (new MtGoxNetTools()).DoAuthenticatedAPIPost(url, apiKey, apiSecret, postData);
                return MtGoxTrade.getObjects(responseStr);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 0/data/getDepth.php 
        /// </summary>
        public MtGoxDepthInfo getDepth(MtGoxCurrencySymbol currency)
        {
            try
            {   
                string url = (this.baseURL) + "0/data/getDepth.php?currency=" + currency.ToString();
                string postData = "";
                string responseStr = (new MtGoxNetTools()).DoAuthenticatedAPIPost(url, apiKey, apiSecret, postData);
                
                MtGoxDepthInfo returnValue = MtGoxDepthInfo.getObjects(responseStr);
                
                return returnValue;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 0/getFunds.php 
        /// </summary>
        public double getFunds()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 0/buyBTC.php 
        /// </summary>
        public List<MtGoxOrder> buyBTC(double amount, MtGoxCurrencySymbol currency, double price = 0.0)
        {
            try
            {
                string url = (this.baseURL) + "0/buyBTC.php";
                string postData;
                if (price == 0.0)
                    postData = "amount=" + amount + "&currency=" + currency.ToString();
                else
                    postData = "amount=" + amount + "&price=" + price + "&currency=" + currency.ToString();
                string responseStr = (new MtGoxNetTools()).DoAuthenticatedAPIPost(url, apiKey, apiSecret, postData);
                return MtGoxOrder.getObjects(responseStr);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 0/sellBTC.php 
        /// </summary>
        public List<MtGoxOrder> sellBTC(double amount, MtGoxCurrencySymbol currency, double price = 0.0)
        {
            try
            {
                string url = (this.baseURL) + "0/sellBTC.php";
                string postData;
                if (price == 0.0)
                    postData = "amount=" + amount + "&currency=" + currency.ToString();
                else
                    postData = "amount=" + amount + "&price=" + price + "&currency=" + currency.ToString();
                string responseStr = (new MtGoxNetTools()).DoAuthenticatedAPIPost(url, apiKey, apiSecret, postData);
                return MtGoxOrder.getObjects(responseStr);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 0/getOrders.php 
        /// </summary>
        public List<MtGoxOrder> getOrders(MtGoxOrderType type, MtGoxOrderStatus status, int oid = 0)
        {
            try
            {
                string url = (this.baseURL) + "0/getOrders.php";
                string postData = "";
                postData = "oid=" + oid;
                switch (type)
                {
                    case MtGoxOrderType.Buy:
                        postData += "&type=1";
                        break;
                    case MtGoxOrderType.Sell:
                        postData += "&type=2";
                        break;
                }
                switch (status)
                {
                    case MtGoxOrderStatus.Active:
                        postData += "&status=1";
                        break;
                    case MtGoxOrderStatus.NotSufficientFunds:
                        postData += "&status=2";
                        break;
                }
                string responseStr = (new MtGoxNetTools()).DoAuthenticatedAPIPost(url, apiKey, apiSecret, postData);
                return MtGoxOrder.getObjects(responseStr);
            }
            catch (Exception ex)
            {
                return null;
            }
        }        

        /// <summary>
        /// 0/cancelOrder.php 
        /// </summary>
        public List<MtGoxOrder> cancelOrder(string oid, MtGoxOrderType type)
        {
            try
            {
                string url = (this.baseURL) + "0/cancelOrder.php";
                string postData;
                int t = 0;
                switch (type)
                {
                    case MtGoxOrderType.Buy:
                        t = 2;
                        break;
                    case MtGoxOrderType.Sell:
                        t = 1;
                        break;
                }
                postData = "oid=" + oid + "&type=" + t;
                string responseStr = (new MtGoxNetTools()).DoAuthenticatedAPIPost(url, apiKey, apiSecret, postData);
                return MtGoxOrder.getObjects(responseStr);
            }
            catch (Exception ex)
            {
                return null;
            }
        }    

        /// <summary>
        /// 0/history_[CUR].csv
        /// </summary>
        public string history_CUR(MtGoxCurrencySymbol currency)
        {
            try
            {
                string url = (this.baseURL) + "0/history_" + currency.ToString() + ".csv";
                string postData = "";
                string responseStr = (new MtGoxNetTools()).DoAuthenticatedAPIPost(url, apiKey, apiSecret, postData);
                return responseStr;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 0/info.php
        /// </summary>
        public MtGoxHistoryItem info()
        {
            try
            {
                string url = (this.baseURL) + "0/info.php";
                string postData = "";
                string responseStr = (new MtGoxNetTools()).DoAuthenticatedAPIPost(url, apiKey, apiSecret, postData);
                return MtGoxHistoryItem.getObjects(responseStr);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 0/ticker
        /// </summary>
        public MtGoxTickerItem ticker()
        {
            try
            {
                string url = (this.baseURL) + "0/ticker.php";
                string postData = "";
                string responseStr = (new MtGoxNetTools()).DoAuthenticatedAPIPost(url, apiKey, apiSecret, postData);
                return MtGoxTickerItem.getObjects(responseStr);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }

}
