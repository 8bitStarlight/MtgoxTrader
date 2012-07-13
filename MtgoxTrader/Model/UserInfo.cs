//****************************************************************************             
//
// @File: UserInfo.cs
// @owner: iamapi 
//    
// Notes:
//	
// @EndHeader@
//****************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MtGoxTrader.Model;

namespace MtGoxTrader.Model
{
    public class UserInfo
    {
        public double BtcBalance
        {
            get;
            set;
        }
        public double FundBalance
        {
            get;
            set;
        }
        public MtGoxCurrencySymbol Currency
        {
            get;
            set;
        }       
        
        public double BtcBalanceExclude
        {
            get;
            set;
        }
        public double FundBalanceExclude
        {
            get;
            set;
        }

        public double LastPrice
        {
            get;
            set;
        }

        public double Fee
        {
            get;
            set;
        }
        
        public double GetCost()
        {
            return -FundBalanceExclude / BtcBalanceExclude;                      
        }
        public double GetMarketValue()
        {
            return BtcBalance * LastPrice;            
        }
        public double GetProfit()
        {
            return FundBalanceExclude + BtcBalanceExclude * LastPrice;            
        }
        public double GetProfitRate()
        {
            return GetProfit() / (BtcBalanceExclude * LastPrice);          
        }
    }
}
