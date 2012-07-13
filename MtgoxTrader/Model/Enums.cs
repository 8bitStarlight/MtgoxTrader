//****************************************************************************             
//
// @File: Enums.cs
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

    /// <summary>
    /// Order types
    /// </summary>
    public enum MtGoxOrderType
    {
        Buy,
        Sell
    }
    
    /// <summary>
    /// Currency symbols currently supported by MtGox 
    /// </summary>
    public enum MtGoxCurrencySymbol
    {
        USD,
        AUD,
        CAD,
        CHF,
        CNY,
        DKK,
        EUR,
        GBP,
        HKD,
        JPY,
        NZD,
        PLN,
        RUB,
        SEK,
        SGD,
        THB,
        BTC
    }

    /// <summary>
    /// Order status
    /// </summary>
    public enum MtGoxOrderStatus
    {
        Active,
        NotSufficientFunds,
        Pending,
        Invalid
    }

    /// <summary>
    /// MtGox trade types
    /// </summary>
    public enum MtGoxTradeType
    {
        bid,
        ask
    }    
}
