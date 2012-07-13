//****************************************************************************             
//
// @File: MtGoxHistoryItem.cs
// @owner: iamapi 
//    
// Notes:
//	
// @EndHeader@
//****************************************************************************
using System.Collections.Generic;
using System;
using System.Web.Script.Serialization;


namespace MtGoxTrader.Model
{    
    public class MtGoxHistoryItem
    {

        public string Login { get; set; }

        public int Index { get; set; }

        public List<string> Rights { get; set; }

        public string Language { get; set; }

        public DateTime Created { get; set; }

        public DateTime Last_Login { get; set; }

        public List<MtGoxWallet> Wallets { get; set; }

        public double Trade_Fee { get; set; }

        public MtGoxHistoryItem()
        { }
        
        /// <summary>
        /// Parse the JSON data returned by the 0/info.php method
        /// </summary>        
        public static MtGoxHistoryItem getObjects(string jsonDataStr)
        {
            string json = jsonDataStr;
            var serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new[] { new DynamicJsonConverter() });
            dynamic obj = serializer.Deserialize(json, typeof(object));
            MtGoxHistoryItem item = new MtGoxHistoryItem();
            item.Login = obj.Login.ToString();
            item.Index = int.Parse(obj.Index.ToString());
            item.Rights = new List<string>();
            for (int i = 0; i < obj.Rights.Count; i++)
            {
                item.Rights.Add(obj.Rights[i].ToString());
            }
            item.Language = obj.Language.ToString();
            item.Created = DateTime.Parse(obj.Created.ToString());
            item.Last_Login = DateTime.Parse(obj.Last_Login.ToString());
            item.Wallets = new List<MtGoxWallet>();
            try
            {
                item.Wallets.Add(MtGoxHistoryItem.BuildWallet("USD", obj.Wallets.USD));
            }
            catch(Exception ex) 
            { }
            // AUD
            try
            {
                item.Wallets.Add(MtGoxHistoryItem.BuildWallet("AUD", obj.Wallets.AUD));
            }
            catch (Exception ex)
            { }
            // CAD
            try
            {
                item.Wallets.Add(MtGoxHistoryItem.BuildWallet("CAD", obj.Wallets.CAD));
            }
            catch (Exception ex)
            { }
            // CHF
            try
            {
                item.Wallets.Add(MtGoxHistoryItem.BuildWallet("CHF", obj.Wallets.CHF));
            }
            catch (Exception ex)
            { }
            // CNY
            try
            {
                item.Wallets.Add(MtGoxHistoryItem.BuildWallet("CNY", obj.Wallets.CNY));
            }
            catch (Exception ex)
            { }
            // DKK
            try
            {
                item.Wallets.Add(MtGoxHistoryItem.BuildWallet("DKK", obj.Wallets.DKK));
            }
            catch (Exception ex)
            { }
            // EUR
            try
            {
                item.Wallets.Add(MtGoxHistoryItem.BuildWallet("EUR", obj.Wallets.EUR));
            }
            catch (Exception ex)
            { }
            // GBP
            try
            {
                item.Wallets.Add(MtGoxHistoryItem.BuildWallet("GBP", obj.Wallets.GBP));
            }
            catch (Exception ex)
            { }
            // HKD
            try
            {
                item.Wallets.Add(MtGoxHistoryItem.BuildWallet("HKD", obj.Wallets.HKD));
            }
            catch (Exception ex)
            { }
            // JPY
            try
            {
                item.Wallets.Add(MtGoxHistoryItem.BuildWallet("JPY", obj.Wallets.JPY));
            }
            catch (Exception ex)
            { }
            // NZD
            try
            {
                item.Wallets.Add(MtGoxHistoryItem.BuildWallet("NZD", obj.Wallets.NZD));
            }
            catch (Exception ex)
            { }
            // PLN
            try
            {
                item.Wallets.Add(MtGoxHistoryItem.BuildWallet("PLN", obj.Wallets.PLN));
            }
            catch (Exception ex)
            { }
            // RUB
            try
            {
                item.Wallets.Add(MtGoxHistoryItem.BuildWallet("RUB", obj.Wallets.RUB));
            }
            catch (Exception ex)
            { }
            // SEK
            try
            {
                item.Wallets.Add(MtGoxHistoryItem.BuildWallet("SEK", obj.Wallets.SEK));
            }
            catch (Exception ex)
            { }
            // SGD
            try
            {
                item.Wallets.Add(MtGoxHistoryItem.BuildWallet("SGD", obj.Wallets.SGD));
            }
            catch (Exception ex)
            { }
            // THB
            try
            {
                item.Wallets.Add(MtGoxHistoryItem.BuildWallet("THB", obj.Wallets.THB));
            }
            catch (Exception ex)
            { }
            // BTC
            try
            {
                item.Wallets.Add(MtGoxHistoryItem.BuildWallet("BTC", obj.Wallets.BTC));
            }
            catch (Exception ex)
            { }
            item.Trade_Fee = double.Parse(obj.Trade_Fee.ToString());
            return item;
        }

        /// <summary>
        /// Helper method to build wallet objects
        /// </summary>       
        private static MtGoxWallet BuildWallet(string walletName, dynamic obj)
        {
            if(obj == null)
                throw new Exception("No wallet for this currency");
            MtGoxCurrencySymbol name = (MtGoxCurrencySymbol)Enum.Parse(typeof(MtGoxCurrencySymbol), walletName, true);
            MtGoxWalletBalance bal = new MtGoxWalletBalance(double.Parse(obj.Balance.value), Int64.Parse(obj.Balance.value_int), obj.Balance.display, (MtGoxCurrencySymbol)Enum.Parse(typeof(MtGoxCurrencySymbol), obj.Balance.currency, true));
            int ops = obj.Operations;
            MtGoxWithdrawalLimit dwd = null;
            if (obj.Daily_Withdraw_Limit != null)
            {
                dwd = new MtGoxWithdrawalLimit(double.Parse(obj.Daily_Withdraw_Limit.value), Int64.Parse(obj.Daily_Withdraw_Limit.value_int), obj.Daily_Withdraw_Limit.display, (MtGoxCurrencySymbol)Enum.Parse(typeof(MtGoxCurrencySymbol), obj.Daily_Withdraw_Limit.currency, true));
            }
            MtGoxWithdrawalLimit mwd = null;
            if (obj.Monthly_Withdraw_Limit != null)
            {
                mwd = new MtGoxWithdrawalLimit(double.Parse(obj.Monthly_Withdraw_Limit.value), Int64.Parse(obj.Monthly_Withdraw_Limit.value_int), obj.Monthly_Withdraw_Limit.display, (MtGoxCurrencySymbol)Enum.Parse(typeof(MtGoxCurrencySymbol), obj.Monthly_Withdraw_Limit.currency, true));
            }
            MtGoxWithdrawalLimit maxwd = null;
            if (obj.Max_Withdraw != null)
            {
                maxwd = new MtGoxWithdrawalLimit(double.Parse(obj.Max_Withdraw.value), Int64.Parse(obj.Max_Withdraw.value_int), obj.Max_Withdraw.display, (MtGoxCurrencySymbol)Enum.Parse(typeof(MtGoxCurrencySymbol), obj.Max_Withdraw.currency, true));
            }
            MtGoxWallet wal = new MtGoxWallet(name, bal, ops, dwd, mwd, maxwd);
            return wal;
        }
    }
}
