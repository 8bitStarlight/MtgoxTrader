//****************************************************************************             
//
// @File: MtGoxWalletBalance.cs
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
    public class MtGoxWalletBalance
    {
        public double value;

        public Int64 value_int;

        public string display;

        public MtGoxCurrencySymbol currency;

        public MtGoxWalletBalance()
        { }

        public MtGoxWalletBalance(double value, Int64 value_int, string display, MtGoxCurrencySymbol currency)
        {
            this.value = value;
            this.value_int = value_int;
            this.display = display;
            this.currency = currency;
        }

        public static MtGoxWalletBalance getObjects(string jsonData)
        {
            throw new NotImplementedException();
        }
    }
}
