//****************************************************************************             
//
// @File: MtGoxWallet.cs
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
    public class MtGoxWallet
    {
        public MtGoxCurrencySymbol name { get; set; }

        public MtGoxWalletBalance balance { get; set; }
        
        public int Operations { get; set; }
        
        public MtGoxWithdrawalLimit Daily_Withdraw_Limit { get; set; }
        
        public MtGoxWithdrawalLimit Monthly_Withdraw_Limit { get; set; }
        
        public MtGoxWithdrawalLimit Max_Withdraw { get; set; }

        public MtGoxWallet(MtGoxCurrencySymbol name, MtGoxWalletBalance balance, int Operations, MtGoxWithdrawalLimit Daily_Withdraw_Limit, MtGoxWithdrawalLimit Monthly_Withdraw_Limit, MtGoxWithdrawalLimit Max_Withdraw)
        {
            this.name = name;
            this.balance = balance;
            this.Operations = Operations;
            this.Daily_Withdraw_Limit = Daily_Withdraw_Limit;
            this.Monthly_Withdraw_Limit = Monthly_Withdraw_Limit;
            this.Max_Withdraw = Max_Withdraw;
        }

        public MtGoxWallet()
        { }

        public static MtGoxWallet getObjects(string jsonData)
        {
            throw new NotImplementedException();
        }
    }
}
