//****************************************************************************             
//
// @File: MtGoxWithdrawalLimit.cs
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

namespace MtGoxTrader.Model
{    
    public class MtGoxWithdrawalLimit
    {
        public double value { get; set; }

        public Int64 value_int { get; set; }

        public string display { get; set; }

        public MtGoxCurrencySymbol currency { get; set; }

        public MtGoxWithdrawalLimit()
        {}

        public MtGoxWithdrawalLimit(double value, Int64 value_int, string display, MtGoxCurrencySymbol currency)
        {
            this.value = value;
            this.value_int = value_int;
            this.display = display;
            this.currency = currency;
        }

        public static MtGoxWithdrawalLimit getObjects(string jsonData)
        {
            return null;
        }
    }
}
