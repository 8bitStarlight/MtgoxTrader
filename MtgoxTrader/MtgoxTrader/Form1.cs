//****************************************************************************             
//
// @File: Form1.cs
// @owner: iamapi 
//    
// Notes:
//	
// @EndHeader@
//****************************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.Resources;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.IO;
using System.Configuration;
using System.Diagnostics;
using MtGoxTrader.MtGoxAPIClient;
using MtGoxTrader.Model;
using MtGoxTrader.TradeStrategy;

namespace MtGoxTrader.Trader
{
    public partial class Form1 : Form
    {
        private DataTable dtAccount;
        private DataTable dtTrade;
        private int showOrderNO = Consts.DefaultShowOrderNo;
        private int separate = 0;
        private int separateMicro = 0;
        private MtGoxDepthInfo currentInfo = null;
        private MtGoxTickerItem currentTicker = null;
        private List<MtGoxTrade> currenttradeListInOneMin = null;
        private List<MtGoxTrade> currenttradeListInFiveMin = null;
        private System.Threading.Timer tickerTimer = null;
        private System.Threading.Timer formTimer = null;
        AutoResetEvent tickerEvent;
        AutoResetEvent formEvent;
        private int lastRefreshTime = Consts.DefaultRefreshTime;

        public static MtGoxAPIV0 mtgoxV0 = null;
        private UserInfo userInfo = null;
        private HistoryInfo historyFund = null;
        private HistoryInfo historyBtc = null;
        private List<MtGoxOrder> orders = null;
        private string tradeHistoryFund;
        private string tradeHistoryBtc;
        private string lastMaxTid = string.Empty;
        private string lastMinTid = string.Empty;        
        private string stopOrderFullPath = null;
        MtGoxHistoryItem currentHistory;

        private string configFolder;

        private List<StopOrder> stopOrderList;

        private AutoTradeSettings currentAutoTradeSettings;
        private List<AutoTradeSettings> autoTradeSettingsList;

        public Config MtGoxConfig = null;

        public MtGoxCurrencySymbol Currency = MtGoxCurrencySymbol.USD;

        public Form1()
        {
            InitializeComponent();
            InitializeData();
            InitializeControl();
            LoadSettings(null, null);
            MtGoxConfig = new Config();
            mtgoxV0 = new MtGoxAPIV0();
        }

        public bool Init()
        {
            bool bReturn = true;
            try
            {             
                mtgoxV0.apiKey = MtGoxConfig.Key;
                mtgoxV0.apiSecret = MtGoxConfig.Secret;
                Currency = MtGoxConfig.Currency;
                userInfo = new UserInfo();
                userInfo.Currency = Currency;

                historyFund = new HistoryInfo();
                historyBtc = new HistoryInfo();
                currentHistory = mtgoxV0.info();
                if (currentHistory == null)
                {
                    bReturn = false;
                    return bReturn;
                }

                this.LoadHistory();
                this.loadUserInfo();
                this.loadOrders();
                tickerTimer = new System.Threading.Timer(GetRealtimeTrade, null, Consts.DefaultRefreshTime * 1000, Consts.DefaultRefreshTime * 1000);
                formTimer = new System.Threading.Timer(BindRealtimeTrade, null, Consts.DefaultRefreshTime * 1000 + System.DateTime.Now.Month * 100 + System.DateTime.Now.Year, Consts.DefaultRefreshTime * 1000);
                this.GetRealtimeTrade(null);
                this.BindUserInfo();                
            }
            catch (Exception ex)
            {
                bReturn = false;
                Trace.WriteLine(string.Format("{0} \r\n stack:{1}", Utils.GetDetailedException(ex), Utils.GetStackTrace(ex)));
            }
            return bReturn;
        }

        private void InitializeControl()
        {
            this.tabAccount.Text = ResourceFactory.GetString("AccountTab");
            this.tabTicker.Text = ResourceFactory.GetString("TickerTab");
            this.tabAutoTrade.Text = ResourceFactory.GetString("TabTradeStrategy");
            this.cbAlwaysInFront.Text = ResourceFactory.GetString("AlwaysInFront");
            this.notifyIcon1.Text = ResourceFactory.GetString("Minimized");
            this.menuItemShow.Text = ResourceFactory.GetString("MenuItemShow");
            this.menuItemClose.Text = ResourceFactory.GetString("MenuItemClose");
            this.lblWarnBuyPrice.Text = ResourceFactory.GetString("WarnBuyPrice");
            this.lblWarnSellPrice.Text = ResourceFactory.GetString("WarnSellPrice");
            this.lblWordWarning2.Text = ResourceFactory.GetString("Warning");
            this.lblWordWarning.Text = ResourceFactory.GetString("Warning");
            this.lblRefreshTime.Text = ResourceFactory.GetString("LabelRefreshTime");
            this.lblSeconds.Text = ResourceFactory.GetString("Seconds");
            this.label1.Text = ResourceFactory.GetString("SendBtcToMe");
            this.lblHelp.Text = ResourceFactory.GetString("LabelHelp");
            this.comboBuyType.Items.Clear();
            this.comboBuyType.Items.AddRange(new object[] {
            ResourceFactory.GetString("BuyCustomize"),
            ResourceFactory.GetString("BuyStop")});
            this.lblBuyType.Text = ResourceFactory.GetString("BuyType");
            this.lblBuyCount.Text = ResourceFactory.GetString("BuyAmount");
            this.lblAvailBuyCount.Text = ResourceFactory.GetString("AvailBuyCount");
            this.btnBuy.Text = global::MtGoxTrader.Trader.ResourceFactory.GetString("Buy");
            this.lblFundAmount.Text = ResourceFactory.GetString("AvailFundAmount");
            this.lblBuyPrice.Text = ResourceFactory.GetString("BuyPrice");
            this.comboSellType.Items.Clear();
            this.comboSellType.Items.AddRange(new object[] {
            ResourceFactory.GetString("SellCustomize"),
            ResourceFactory.GetString("SellMarket")});
            this.lblSellType.Text = ResourceFactory.GetString("SellType");
            this.lblSellPrice.Text = ResourceFactory.GetString("SellPrice");
            this.btnSell.Text = global::MtGoxTrader.Trader.ResourceFactory.GetString("Sell");
            this.lblSellCount.Text = ResourceFactory.GetString("SellAmount");
            this.lblAvailSellCount.Text = ResourceFactory.GetString("AvailSellCount");
            this.lblBtcAmount.Text = ResourceFactory.GetString("AvailBtcAmount");
            this.lblShowNumber.Text = ResourceFactory.GetString("ShowDepth");
            this.lblTradeMinAmount.Text = ResourceFactory.GetString("LabelTradeMinAmount");
            this.tabTrade.Text = global::MtGoxTrader.Trader.ResourceFactory.GetString("TradeTab");
            this.tabHistory.Text = global::MtGoxTrader.Trader.ResourceFactory.GetString("HistoryTab");
            this.btnExportBtc.Text = global::MtGoxTrader.Trader.ResourceFactory.GetString("Export");
            this.btnExportFund.Text = global::MtGoxTrader.Trader.ResourceFactory.GetString("Export");
            this.tabTradesHistory.Text = global::MtGoxTrader.Trader.ResourceFactory.GetString("TradeHistoryTab");
            this.btnTradeTime.Text = global::MtGoxTrader.Trader.ResourceFactory.GetString("ButtonTradeTime");
            this.linkNext.Text = ResourceFactory.GetString("LinkNext");
            this.lblAutoTradeNotReady.Text = ResourceFactory.GetString("AutoTradeNotReady");
            this.Text = ResourceFactory.GetString("SoftwareName");
            this.btnAccountRefresh.Text = ResourceFactory.GetString("Refresh");
            this.lblMusic.Text = ResourceFactory.GetString("Sound");
            ToolStripMenuItem FileMenu = new ToolStripMenuItem(ResourceFactory.GetString("FileMenu"));
            ToolStripMenuItem SaveSettingsSubMenu = new ToolStripMenuItem(ResourceFactory.GetString("SaveSettingsSubMenu"));
            SaveSettingsSubMenu.Click += SaveSettings;
            ToolStripMenuItem LoadSettingsSubMenu = new ToolStripMenuItem(ResourceFactory.GetString("LoadSettingsSubMenu"));
            LoadSettingsSubMenu.Click += LoadSettings;
            ToolStripMenuItem QuitSubMenu = new ToolStripMenuItem(ResourceFactory.GetString("QuitSubMenu"));
            QuitSubMenu.Click += CloseForm;
            FileMenu.DropDownItems.Add(SaveSettingsSubMenu);
            FileMenu.DropDownItems.Add(LoadSettingsSubMenu);
            FileMenu.DropDownItems.Add(QuitSubMenu);            

            ToolStripMenuItem LanguageMenu = new ToolStripMenuItem(ResourceFactory.GetString("LanguageMenu"));
            ToolStripMenuItem LanguageSubMenu = new ToolStripMenuItem(ResourceFactory.GetString("LanguageSubMenu"));
            LanguageSubMenu.Click += SetLanguage;
            LanguageMenu.DropDownItems.Add(LanguageSubMenu);

            ToolStripMenuItem HelpMenu = new ToolStripMenuItem(ResourceFactory.GetString("HelpMenu"));
            ToolStripMenuItem AboutSubMenu = new ToolStripMenuItem(ResourceFactory.GetString("AboutSubMenu"));
            AboutSubMenu.Click += AboutClick;
            HelpMenu.DropDownItems.Add(AboutSubMenu);

            this.menuStrip1.Items.Clear();
            this.menuStrip1.Items.Add(FileMenu);
            this.menuStrip1.Items.Add(LanguageMenu);
            this.menuStrip1.Items.Add(HelpMenu);

            dtAccount.Columns.Clear();
            dtAccount.Columns.Add(ResourceFactory.GetString("FundAmount"));
            dtAccount.Columns.Add(ResourceFactory.GetString("Currency"));
            dtAccount.Columns.Add(ResourceFactory.GetString("BtcAmount"));
            dtAccount.Columns.Add(ResourceFactory.GetString("NeweastPrice"));
            dtAccount.Columns.Add(ResourceFactory.GetString("Cost"));
            dtAccount.Columns.Add(ResourceFactory.GetString("MarketValue"));
            dtAccount.Columns.Add(ResourceFactory.GetString("Profit"));
            dtAccount.Columns.Add(ResourceFactory.GetString("ProfitRate"));

            dtTrade.Columns.Clear();
            dtTrade.Columns.Add(ResourceFactory.GetString("TradeTime"), typeof(DateTime));
            dtTrade.Columns.Add(ResourceFactory.GetString("TradePrice"), typeof(Double));
            dtTrade.Columns.Add(ResourceFactory.GetString("TradeAmount"), typeof(Double));
            dtTrade.Columns.Add(ResourceFactory.GetString("TradeType"));

            DataGridViewTextBoxColumn oidColumn = new DataGridViewTextBoxColumn();
            oidColumn.Visible = false;
            oidColumn.Name = "OID";
            oidColumn.HeaderText = "oid";
            dgTrade.Columns.Clear();
            dgTrade.Columns.Add(oidColumn);
            dgTrade.Columns.Add(ResourceFactory.GetString("OrderType"), ResourceFactory.GetString("OrderType"));
            dgTrade.Columns.Add(ResourceFactory.GetString("OrderAmount"), ResourceFactory.GetString("OrderAmount"));
            dgTrade.Columns.Add(ResourceFactory.GetString("OrderPrice"), ResourceFactory.GetString("OrderPrice"));
            dgTrade.Columns.Add(ResourceFactory.GetString("OrderStatus"), ResourceFactory.GetString("OrderStatus"));
            dgTrade.Columns.Add(ResourceFactory.GetString("OrderTotal"), ResourceFactory.GetString("OrderTotal"));
            dgTrade.Columns.Add(ResourceFactory.GetString("OrderTime"), ResourceFactory.GetString("OrderTime"));
            DataGridViewButtonColumn buttonColumn = new DataGridViewButtonColumn();
            buttonColumn.HeaderText = ResourceFactory.GetString("OrderAction");
            buttonColumn.Text = ResourceFactory.GetString("CancelAction");
            buttonColumn.Name = ResourceFactory.GetString("OrderAction");
            this.dgTrade.Columns.Add(buttonColumn);

            this.dgHistoryFund.Columns.Clear();
            this.dgHistoryFund.Columns.Add("Index", "");
            this.dgHistoryFund.Columns.Add("Date", ResourceFactory.GetString("Date"));
            this.dgHistoryFund.Columns.Add("Type", ResourceFactory.GetString("Type"));
            this.dgHistoryFund.Columns.Add("Info", ResourceFactory.GetString("Info"));
            this.dgHistoryFund.Columns.Add("Value", ResourceFactory.GetString("Value"));
            this.dgHistoryFund.Columns.Add("Balance", ResourceFactory.GetString("Balance"));

            this.dgHistoryBtc.Columns.Clear();
            this.dgHistoryBtc.Columns.Add("Index", "");
            this.dgHistoryBtc.Columns.Add("Date", ResourceFactory.GetString("Date"));
            this.dgHistoryBtc.Columns.Add("Type", ResourceFactory.GetString("Type"));
            this.dgHistoryBtc.Columns.Add("Info", ResourceFactory.GetString("Info"));
            this.dgHistoryBtc.Columns.Add("Value", ResourceFactory.GetString("Value"));
            this.dgHistoryBtc.Columns.Add("Balance", ResourceFactory.GetString("Balance"));

            this.dgTradeMicro.Columns.Clear();
            this.dgTradeMicro.Columns.Add(ResourceFactory.GetString("TradeHeadPrice"), ResourceFactory.GetString("TradeHeadPrice"));
            this.dgTradeMicro.Columns.Add(ResourceFactory.GetString("TradeHeadAmount"), ResourceFactory.GetString("TradeHeadAmount"));

            this.dgTrades.Columns.Clear();
            this.dgTrades.Columns.Add("Price", ResourceFactory.GetString("MarketSummaryHeadPrice"));
            this.dgTrades.Columns.Add("Amount", ResourceFactory.GetString("MarketSummaryHeadAmount"));
            this.dgTrades.Columns.Add("BtcAmount", ResourceFactory.GetString("MarketSummaryHeadBtcAmount"));
            this.dgTrades.Columns.Add("FundAmount", ResourceFactory.GetString("MarketSummaryHeadFundAmount"));

            this.btnAddRules.Text = ResourceFactory.GetString("Add");
            this.btnAutoTradeDeleteRules.Text = ResourceFactory.GetString("Delete");
            this.btnAutoTradeGenerateRules.Text = ResourceFactory.GetString("Generate");
            this.btnConfirm.Text = ResourceFactory.GetString("Confirm");
            this.btnAutoTradeCancel.Text = ResourceFactory.GetString("Cancel");
            this.cbWarning.Text = ResourceFactory.GetString("WarningSound");
            this.cbAutoTrade.Text = ResourceFactory.GetString("AutomaticTrade");
            
            this.comboAutoTradeStrategy.Items.Clear();
            string[] nameList = Enum.GetNames(typeof(AutoTradeRules.TradeRules));
            foreach (string s in nameList)
            {
                this.comboAutoTradeStrategy.Items.Add(ResourceFactory.GetString(s));
            }

            this.comboAutoTradeType.Items.Clear();
            this.comboAutoTradeType.Items.AddRange(new object[] {
            ResourceFactory.GetString("Buy"),
            ResourceFactory.GetString("Sell")});          
           
            this.dgAutoTrade.Columns.Clear();
            DataGridViewTextBoxColumn idColumn = new DataGridViewTextBoxColumn();
            idColumn.Visible = false;
            idColumn.Name = "OID";
            idColumn.HeaderText = "oid";
            this.dgAutoTrade.Columns.Add(idColumn);
            this.dgAutoTrade.Columns.Add("AutoTradeRuleCondition", ResourceFactory.GetString("AutoTradeRuleCondition"));
            this.dgAutoTrade.Columns.Add("TradeType", ResourceFactory.GetString("TradeType"));
            this.dgAutoTrade.Columns.Add("TradeAmount", ResourceFactory.GetString("TradeAmount"));            
            this.dgAutoTrade.Columns.Add(ResourceFactory.GetString("OrderTime"), ResourceFactory.GetString("OrderTime"));
            this.dgAutoTrade.Columns.Add(ResourceFactory.GetString("OrderStatus"), ResourceFactory.GetString("OrderStatus"));
            this.dgAutoTrade.Columns.Add(ResourceFactory.GetString("WarningSound"), ResourceFactory.GetString("WarningSound"));
            DataGridViewButtonColumn deleteColumn = new DataGridViewButtonColumn();
            deleteColumn.HeaderText = ResourceFactory.GetString("AutoTradeDelete");
            deleteColumn.Text = ResourceFactory.GetString("AutoTradeDelete");
            deleteColumn.Name = ResourceFactory.GetString("AutoTradeDelete");
            this.dgAutoTrade.Columns.Add(deleteColumn);
            dgAutoTrade.Columns[1].Width = 150;            
        }

        private void InitializeData()
        {
            tickerEvent = new AutoResetEvent(false);
            formEvent = new AutoResetEvent(false);
            dtAccount = new DataTable();
            dtTrade = new DataTable();
            this.lblAutoTradeRules.Text = string.Empty;
            this.lblAutoTradePercent.Visible = false;
            this.lblWarning.Text = "";
            currentAutoTradeSettings = new AutoTradeSettings();
            autoTradeSettingsList = new List<AutoTradeSettings>();
            try
            {
                stopOrderList = new List<StopOrder>();
                configFolder = Path.Combine(Application.StartupPath, Consts.DataFolder);
                this.stopOrderFullPath = Path.Combine(configFolder, Consts.StopOrderFileName);
                
                if (!Directory.Exists(configFolder))
                {
                    Directory.CreateDirectory(configFolder);
                }
                List<StopOrder> orderList = (List<StopOrder>)Utils.LoadFromFile(this.stopOrderFullPath, typeof(List<StopOrder>));
                if (orderList != null)
                    this.stopOrderList = orderList;
            }
            catch
            {
            }

            try
            {
                this.comboAlarm.Items.Add("");
                this.comboAutoTradeWarningSound.Items.Add("");
                DirectoryInfo folder = new DirectoryInfo(Path.Combine(Application.StartupPath, Consts.SoundFolder));
                foreach (FileInfo file in folder.GetFiles())
                {
                    if (Consts.SoundTypes.Contains(file.Extension.ToLower()))
                    {
                        this.comboAlarm.Items.Add(file.Name);
                        this.comboAutoTradeWarningSound.Items.Add(file.Name);
                    }
                }
                this.comboAlarm.SelectedIndex = 0;
                this.comboAutoTradeWarningSound.SelectedIndex = 0;
            }
            catch
            {

            }
        }        

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            reload();
        }

        private void loadUserInfo()
        {
            try
            {                
                MtGoxWallet walletBTC = null, walletFund = null;
                for (int i = 0; i < currentHistory.Wallets.Count; i++)
                {
                    if (currentHistory.Wallets[i].name == MtGoxCurrencySymbol.BTC)
                    {
                        walletBTC = currentHistory.Wallets[i];
                    }
                    else if (currentHistory.Wallets[i].name == Currency)
                    {
                        walletFund = currentHistory.Wallets[i];
                    }
                    if (walletBTC != null && walletFund != null)
                        break;
                }

                userInfo.Fee = currentHistory.Trade_Fee;
                userInfo.BtcBalance = walletBTC.balance.value;
                userInfo.FundBalance = walletFund.balance.value;
                userInfo.BtcBalanceExclude = walletBTC.balance.value + historyBtc.TotalWithdraw - historyBtc.TotalDeposit;
                userInfo.FundBalanceExclude = walletFund.balance.value + historyFund.TotalWithdraw - historyFund.TotalDeposit;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("{0} \r\n stack:{1}", Utils.GetDetailedException(ex), Utils.GetStackTrace(ex)));
            }
        }

        private void LoadHistory()
        {
            try
            {
                tradeHistoryFund = mtgoxV0.history_CUR(Currency);
                tradeHistoryBtc = mtgoxV0.history_CUR(MtGoxCurrencySymbol.BTC);
                historyFund.History = tradeHistoryFund;
                HistoryHelper.analyzeHistory(ref historyFund);

                historyBtc.History = tradeHistoryBtc;
                HistoryHelper.analyzeHistory(ref historyBtc);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("{0} \r\n stack:{1}", Utils.GetDetailedException(ex), Utils.GetStackTrace(ex)));
            }
        }

        private void BindUserInfo()
        {           
            dtAccount.Clear();
            DataRow dr = dtAccount.NewRow();
            dr[ResourceFactory.GetString("BtcAmount")] = string.Format("{0:0.####}", userInfo.BtcBalance);
            dr[ResourceFactory.GetString("FundAmount")] = string.Format("{0:0.##}", userInfo.FundBalance);
            dr[ResourceFactory.GetString("Currency")] = Currency.ToString();
            userInfo.LastPrice = currentTicker.last;
            dr[ResourceFactory.GetString("NeweastPrice")] = string.Format("{0:0.####}", userInfo.LastPrice);
            dr[ResourceFactory.GetString("Cost")] = string.Format("{0:0.####}", userInfo.GetCost());
            dr[ResourceFactory.GetString("MarketValue")] = string.Format("{0:0.##}", userInfo.GetMarketValue());
            dr[ResourceFactory.GetString("Profit")] = string.Format("{0:0.##}", userInfo.GetProfit());
            dr[ResourceFactory.GetString("ProfitRate")] = string.Format("{0:0.####}%", 100 * userInfo.GetProfitRate());
            dtAccount.Rows.Add(dr);
            this.dataGridView1.DataSource = dtAccount;
        }

        private string alertSellerLowest()
        {
            string alert = string.Empty;
            double lowestSellPrice = Consts.DefaultLowestSellPrice;
            if (!Double.TryParse(txtLowestSellPrice.Text.Trim(), out lowestSellPrice))
                return alert;
            if (currentInfo != null && currentInfo.asks.Count > 0)
            {
                if (currentInfo.asks[0].price < lowestSellPrice)
                    alert += ResourceFactory.GetString("CurrentSellPrice") + " " + currentInfo.asks[0].price + " " + ResourceFactory.GetString("Below") + lowestSellPrice;                
            }           
            
            return alert;
        }

        private string alertBuyerHighest()
        {
            string alert = string.Empty;
            double highestBuyPrice = Consts.DefaultHighestBuyPrice;
            if (!Double.TryParse(txtHighestBuyPrice.Text.Trim(), out highestBuyPrice))
                return alert;

            if (currentInfo != null && currentInfo.bids.Count > 0)
            {
                if (currentInfo.bids[0].price > highestBuyPrice)
                    alert += ResourceFactory.GetString("CurrentBuyPrice") + " " + currentInfo.bids[0].price + " " + ResourceFactory.GetString("Above") + highestBuyPrice;
            }
            return alert;
        }        

        private void GetRealtimeTrade(object state)
        {
            try
            {
                tickerTimer.Change(int.MaxValue, int.MaxValue);
                currentInfo = mtgoxV0.getDepth(Currency);
                currentTicker = mtgoxV0.ticker();
                DateTime fiveMinuteAgo = System.DateTime.Now - TimeSpan.FromMinutes(5);
                string tidFive = getJasonDate(fiveMinuteAgo) + "000000";
                currenttradeListInFiveMin = mtgoxV0.getTrades(tidFive);
                DateTime oneMinuteAgo = System.DateTime.Now - TimeSpan.FromMinutes(1);
                string tidOne = getJasonDate(oneMinuteAgo) + "000000";
                currenttradeListInOneMin = mtgoxV0.getTrades(tidOne);
                executeAutoTrade();                
            }            
            catch(Exception ex)
            {
                Trace.WriteLine(string.Format("{0} \r\n stack:{1}", Utils.GetDetailedException(ex), Utils.GetStackTrace(ex)));
            }
            finally
            {
                tickerTimer.Change(lastRefreshTime * 1000, lastRefreshTime * 1000);
                formEvent.Set();
            }
        }

        private void BindRealtimeTrade(object state)
        {
            try
            {
                formTimer.Change(int.MaxValue, int.MaxValue);
                formEvent.WaitOne();
                BindList();

                int refreshTime;
                if (!int.TryParse(this.txtRefresh.Text.Trim(), out refreshTime))
                    refreshTime = Consts.DefaultRefreshTime;

                lastRefreshTime = refreshTime;                    
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("{0} \r\n stack:{1}", Utils.GetDetailedException(ex), Utils.GetStackTrace(ex)));
            }
            finally
            {
                tickerTimer.Change(lastRefreshTime * 1000, lastRefreshTime * 1000);
                formTimer.Change(lastRefreshTime * 1000, lastRefreshTime * 1000);
            }
        }

        private delegate void BindTradeDelegate();

        private void BindList()
        {
            if (this.dgTrades.InvokeRequired)
            {
                BindTradeDelegate sd = new BindTradeDelegate(BindList);
                dgTrades.Invoke(sd);
                return;
            }

            double minAmount;
            if (!Double.TryParse(this.txtOrderMinAmount.Text.Trim(), out minAmount))
            {
                minAmount = Consts.DefaultShowMinAmount;
            }

            if (!int.TryParse(this.comboShowNumber.SelectedItem.ToString(), out showOrderNO))
            {
                this.showOrderNO = Consts.DefaultShowOrderNo;
            }
            this.lblWarning.Text = "";
            this.lblFundAmount.Text = ResourceFactory.GetString("AvailFundAmount") + ": " + userInfo.FundBalance.ToString();          
            this.lblBtcAmount.Text = ResourceFactory.GetString("AvailBtcAmount") + ": " + userInfo.BtcBalance.ToString();
            this.lblAvailSellCount.Text = ResourceFactory.GetString("AvailSellCount") + ": " + userInfo.BtcBalance.ToString();

            int count = 0;
            string display = "";
            this.dgTrades.Rows.Clear();
            this.dgTradeMicro.Rows.Clear();
            this.lbTicker.Items.Clear();

            this.lbTicker.Items.Add(ResourceFactory.GetString("CurrentPrice") + ": " + currentTicker.last.ToString().PadRight(20)
                + ResourceFactory.GetString("Volume") + ": " + currentTicker.vol.ToString());

            this.lbTicker.Items.Add("");            

            this.lbTicker.Items.Add(ResourceFactory.GetString("High") + ": " + currentTicker.high.ToString().PadRight(20)
                + ResourceFactory.GetString("Low") + ": " + currentTicker.low.ToString().PadRight(20));
            this.lbTicker.Items.Add(""); 
            this.lbTicker.Items.Add(ResourceFactory.GetString("Avg") + ": " + currentTicker.avg.ToString());                

            int bidsCountMicro = 0;
            separateMicro = 0;
            for (int i = 0; i < currentInfo.bids.Count; i++)
            {
                if (currentInfo.bids[i].amount >= minAmount)
                {
                    bidsCountMicro++;
                    separateMicro++;
                    this.dgTradeMicro.Rows.Add(string.Format("{0:0.####}", currentInfo.bids[i].price), string.Format("{0:0.##}", currentInfo.bids[i].amount));
                }
                if (bidsCountMicro >= this.showOrderNO)
                    break;
            }

            //
            this.dgTradeMicro.Rows.Insert(0, "", "");
            bidsCountMicro += 1;

            for (int i = 0; i < currentInfo.asks.Count; i++)
            {
                if (currentInfo.asks[i].amount >= minAmount)
                {
                    count++;
                    this.dgTradeMicro.Rows.Insert(0, string.Format("{0:0.####}", currentInfo.asks[i].price), string.Format("{0:0.##}", currentInfo.asks[i].amount));
                }
                if (count >= this.showOrderNO)
                    break;
            }
            
            
            int bidsCount = 0;
            separate = 0;
            count = 0;
            for (int i = 0; i < currentInfo.bidGroup.Count; i++)
            {
                if (currentInfo.bidGroup[i].Amount >= minAmount)
                {
                    bidsCount++;
                    separate++;
                    display = padString(string.Format("{0:0.00}", currentInfo.bidGroup[i].Price), 18) + padString(string.Format("{0:0.00}", currentInfo.bidGroup[i].Amount) + "(" + currentInfo.bidGroup[i].ItemNO + ")", 25)
                        + padString(string.Format("{0:0.0}", currentInfo.bidGroup[i].BtcCount), 25) + string.Format("{0:0.0}", currentInfo.bidGroup[i].FundCount);
                    this.dgTrades.Rows.Add(string.Format("{0:0.00}", currentInfo.bidGroup[i].Price), string.Format("{0:0.00}", currentInfo.bidGroup[i].Amount) + "(" + currentInfo.bidGroup[i].ItemNO + ")", 
                        string.Format("{0:0.0}", currentInfo.bidGroup[i].BtcCount), string.Format("{0:0.0}", currentInfo.bidGroup[i].FundCount));
                }
                if (bidsCount >= this.showOrderNO)
                    break;
            }

            //
            this.dgTrades.Rows.Insert(0, "", "", "", "");
            bidsCount += 1;

            for (int i = 0; i < currentInfo.askGroup.Count; i++)
            {
                if (currentInfo.askGroup[i].Amount >= minAmount)
                {
                    count++;
                    display = padString(string.Format("{0:0.00}", currentInfo.askGroup[i].Price), 18) + padString(string.Format("{0:0.00}", currentInfo.askGroup[i].Amount) + "(" + currentInfo.askGroup[i].ItemNO + ")", 25)
                        + padString(string.Format("{0:0.0}", currentInfo.askGroup[i].BtcCount), 25) + string.Format("{0:0.0}", currentInfo.askGroup[i].FundCount);
                    this.dgTrades.Rows.Insert(0, string.Format("{0:0.00}", currentInfo.askGroup[i].Price), string.Format("{0:0.00}", currentInfo.askGroup[i].Amount) + "(" + currentInfo.askGroup[i].ItemNO + ")", 
                        string.Format("{0:0.0}", currentInfo.askGroup[i].BtcCount), string.Format("{0:0.0}", currentInfo.askGroup[i].FundCount));
                }
                if (count >= this.showOrderNO)
                    break;
            }

            string alertSell = alertSellerLowest();
            string alertBuy = alertBuyerHighest();

            if (alertSell != string.Empty || alertBuy != string.Empty)
            {
                this.lblWarning.Text += (alertSell == string.Empty ? string.Empty : (alertSell + "\r\n")) + alertBuy;
                if (this.comboAlarm.SelectedItem != null && this.comboAlarm.SelectedItem.ToString() != string.Empty)
                {
                    string folder = Path.Combine(Application.StartupPath, Consts.SoundFolder);
                    SoundPlayer player = new SoundPlayer(Path.Combine(folder, this.comboAlarm.SelectedItem.ToString()));
                    player.Play();
                }
            }
        }

        #region orders

        private void loadOrders()
        {
            orders = mtgoxV0.getOrders(MtGoxOrderType.Sell, MtGoxOrderStatus.Active);
        }

        private void BindOrders()
        {
            if (orders == null)
                return;
            this.dgTrade.Rows.Clear();            

            foreach (MtGoxOrder sellOrder in orders)
            {
                dgTrade.Rows.Add(sellOrder.oid, sellOrder.type, sellOrder.amount, sellOrder.price, sellOrder.real_status, sellOrder.price * sellOrder.amount,
                    formatDate(sellOrder.date), ResourceFactory.GetString("CancelAction"));
            }
            foreach (StopOrder stopOrder in stopOrderList)
            {
                if (stopOrder.Status == StopOrder.OrderStatus.Pending)
                {
                    dgTrade.Rows.Add(stopOrder.OrderId, stopOrder.Type.ToString(), stopOrder.Amount, stopOrder.Price, stopOrder.Status.ToString(),
                        stopOrder.Amount * stopOrder.Price, stopOrder.OrderTime, ResourceFactory.GetString("CancelAction"));
                }
                else
                {
                    dgTrade.Rows.Add(stopOrder.OrderId, stopOrder.Type.ToString(), stopOrder.Amount, stopOrder.Price, stopOrder.Status.ToString(),
                        stopOrder.Amount * stopOrder.Price, stopOrder.OrderTime, "Executed at " + stopOrder.ExecuteTime);
                }
            }
        }        

        private void dgTrade_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 7 && dgTrade.Rows[e.RowIndex].Cells[0].Value != null)
                {
                    string type = dgTrade.Rows[e.RowIndex].Cells[1].Value.ToString();
                    string status = dgTrade.Rows[e.RowIndex].Cells[4].Value.ToString();
                    if (status == StopOrder.OrderStatus.Executed.ToString())
                        return;
                    if (type == StopOrder.OrderType.SellStop.ToString() || type == StopOrder.OrderType.BuyStop.ToString())
                    {
                        StopOrder order = this.stopOrderList.Where(t => t.OrderId.Equals(Guid.Parse(dgTrade.Rows[e.RowIndex].Cells[0].Value.ToString()))).FirstOrDefault();
                        this.stopOrderList.Remove(order);
                        Utils.SaveToFile(this.stopOrderList, this.stopOrderFullPath);
                        loadOrders();
                        BindOrders();
                    }
                    else
                    {
                        mtgoxV0.cancelOrder((string)dgTrade.Rows[e.RowIndex].Cells[0].Value, (MtGoxOrderType)dgTrade.Rows[e.RowIndex].Cells[1].Value);
                        loadOrders();
                        BindOrders();
                        loadUserInfo();
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("{0} \r\n stack:{1}", Utils.GetDetailedException(ex), Utils.GetStackTrace(ex)));
            }
        }
        #endregion
        

        #region history
        private void BindHistory()
        {
            try
            {
                using (TextReader textReader = new StringReader(tradeHistoryFund))
                {
                    LumenWorks.Framework.IO.Csv.CsvReader reader = new LumenWorks.Framework.IO.Csv.CsvReader(textReader, true);
                    LumenWorks.Framework.IO.Csv.CsvReader.RecordEnumerator record = reader.GetEnumerator();
                    this.dgHistoryFund.Rows.Clear();

                    while (record.MoveNext())
                    {
                        this.dgHistoryFund.Rows.Add(record.Current);
                    }
                }

                using (TextReader textReader = new StringReader(tradeHistoryBtc))
                {
                    LumenWorks.Framework.IO.Csv.CsvReader reader = new LumenWorks.Framework.IO.Csv.CsvReader(textReader, true);
                    LumenWorks.Framework.IO.Csv.CsvReader.RecordEnumerator record = reader.GetEnumerator();
                    this.dgHistoryBtc.Rows.Clear();

                    while (record.MoveNext())
                    {
                        this.dgHistoryBtc.Rows.Add(record.Current);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("{0} \r\n stack:{1}", Utils.GetDetailedException(ex), Utils.GetStackTrace(ex)));
            }
        }
        #endregion



        private void lbTrades_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;
            try
            {
                string vKey = ((ListBox)sender).Items[e.Index].ToString();
                e.DrawBackground();
                e.Graphics.DrawString(vKey, e.Font, new SolidBrush(e.Index > separate ? Color.Red : Color.Green), e.Bounds);
                e.DrawFocusRectangle();
            }
            catch(Exception ex)
            {
            }
        }

        private void lbTradeMicro_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;
            try
            {
                string vKey = ((ListBox)sender).Items[e.Index].ToString();
                e.DrawBackground();
                e.Graphics.DrawString(vKey, e.Font, new SolidBrush(e.Index > separateMicro ? Color.Red : Color.Green), e.Bounds);
                e.DrawFocusRectangle();
            }
            catch (Exception ex)
            {
            }
        }

        private void btnBuy_Click(object sender, EventArgs e)
        {
            double price = 0;
            double amount = 0;

            try
            {
                if (this.comboBuyType.SelectedIndex == 0)
                {
                    price = Double.Parse(this.txtBuyPrice.Text.Trim());
                }
                else
                {
                    currentTicker = mtgoxV0.ticker();
                    price = currentTicker.sell + Consts.MarketTol;
                }
                amount = Double.Parse(this.txtBuyCount.Text.Trim());
            }
            catch(Exception ex)
            {
                MessageBox.Show(ResourceFactory.GetString("InputError"), ResourceFactory.GetString("InputError"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }            
            
            List<MtGoxOrder> order = mtgoxV0.buyBTC(amount, Currency, price);
            if (order != null)
            {
                MessageBox.Show(ResourceFactory.GetString("OrderSucceed"), ResourceFactory.GetString("OrderSucceed"), MessageBoxButtons.OK);
            }
        }

        private void btnSell_Click(object sender, EventArgs e)
        {
            double price = 0;
            double amount = 0;
            try
            {
                amount = Double.Parse(this.txtSellCount.Text.Trim());
                price = Double.Parse(this.txtSellPrice.Text.Trim());
                if (this.comboSellType.SelectedIndex == 0)
                {
                    
                }
                else
                {                    
                    if (this.comboSellType.SelectedIndex == 1)
                    {
                        currentTicker = mtgoxV0.ticker();
                        price = currentTicker.buy - Consts.MarketTol;
                    }
                    else if (this.comboSellType.SelectedIndex == 2)
                    {
                        StopOrder stopOrder = new StopOrder();
                        stopOrder.Amount = amount;
                        stopOrder.Currency = Currency;
                        stopOrder.OrderTime = System.DateTime.Now;
                        stopOrder.Price = price;
                        stopOrder.Status = StopOrder.OrderStatus.Pending;
                        stopOrder.Type = StopOrder.OrderType.SellStop;
                        this.stopOrderList.Add(stopOrder);
                        Utils.SaveToFile(this.stopOrderList, this.stopOrderFullPath);
                        return;
                    }
                }
                
            }
            catch
            {
                MessageBox.Show(ResourceFactory.GetString("InputError"), ResourceFactory.GetString("InputError"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            List<MtGoxOrder> order = mtgoxV0.sellBTC(amount, Currency, price);
            if (order != null)
            {
                MessageBox.Show(ResourceFactory.GetString("OrderSucceed"), ResourceFactory.GetString("OrderSucceed"), MessageBoxButtons.OK);
            }
        }       

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                exportFileDialog.Filter = "CSV|*.csv|Text|*.txt";
                exportFileDialog.FilterIndex = 0;
                exportFileDialog.FileName = "history_" + Currency.ToString() + ".csv";
                if (exportFileDialog.ShowDialog() == DialogResult.OK && exportFileDialog.FileName != string.Empty)
                {
                    SaveFile(exportFileDialog.FileName, tradeHistoryFund);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void btnExportBtc_Click(object sender, EventArgs e)
        {
            exportFileDialog.Filter = "CSV|*.csv|Text|*.txt";
            exportFileDialog.FilterIndex = 0;
            exportFileDialog.FileName = "history_" + MtGoxCurrencySymbol.BTC.ToString() + ".csv";
            if (exportFileDialog.ShowDialog() == DialogResult.OK && exportFileDialog.FileName != string.Empty)
            {
                SaveFile(exportFileDialog.FileName, tradeHistoryBtc);
            }            
        }

        private void SaveFile(string fileName, string tradeHistory)
        {
            try
            {
                Stream stream = File.OpenWrite(fileName);

                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(tradeHistory);
                }
            }

            catch (IOException ex)
            {
                
            }
        }

        private void txtBuyPrice_Leave(object sender, EventArgs e)
        {
            double price = 0;
            if (Double.TryParse(this.txtBuyPrice.Text.Trim(), out price))
            {
                this.lblAvailBuyCount.Text = ResourceFactory.GetString("AvailBuyCount") + ": " + userInfo.FundBalance / price;
            }
            else
                this.lblAvailBuyCount.Text = ResourceFactory.GetString("AvailBuyCount") + ": ";
        }  
      
        private void BindTradeHistory(string tid)
        {
            dtTrade.Clear();

            List<MtGoxTrade> tradeList = mtgoxV0.getTrades(tid);
            if (tradeList == null || tradeList.Count == 0)
            {
                this.linkNext.Enabled = false;
                return;
            }
            
            int count = 0;
            foreach (MtGoxTrade trade in tradeList)
            {
                DataRow dr = dtTrade.NewRow();
                dr[ResourceFactory.GetString("TradeTime")] = formatDate(trade.date);
                dr[ResourceFactory.GetString("TradePrice")] = trade.price;
                dr[ResourceFactory.GetString("TradeAmount")] = trade.amount;
                dr[ResourceFactory.GetString("TradeType")] = trade.trade_type.ToString();
                dtTrade.Rows.Add(dr);
                if (count == 0)
                {
                    lastMinTid = trade.tid;
                }
                count++;
                if (count >= 100)
                {
                    lastMaxTid = trade.tid;
                    break;
                }
            }
            if (tradeList.Count >= 100)
            {
                this.linkNext.Enabled = true;
            }
            else
            {
                this.linkNext.Enabled = false;
            }         
            this.dgTradeHistory.DataSource = dtTrade;            
        }       

        private void linkNext_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            BindTradeHistory(lastMaxTid);
        }

        private void btnTradeTime_Click(object sender, EventArgs e)
        {
            DateTime tradeTime = this.pickerTradeTime.Value;
            lastMaxTid = lastMinTid = getJasonDate(tradeTime) + "000000";
            BindTradeHistory(lastMaxTid);
        }

        private DateTime formatDate(int jasonDate)
        {
            DateTime baseDate = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            return baseDate.AddSeconds(jasonDate);
        }

        private int getJasonDate(DateTime date)
        {
            DateTime newDate = date.ToUniversalTime();
            DateTime baseDate = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            return (int)(newDate.Subtract(baseDate).TotalSeconds);
        }

        private void btnAccountRefresh_Click(object sender, EventArgs e)
        {
            if (this.tabTradeStrategy.SelectedTab == tabAccount)
            {
                loadUserInfo();
                BindUserInfo();
            }
            else if (this.tabTradeStrategy.SelectedTab == tabTrade)
            {
                loadOrders();
                BindOrders();
            }
            else if (this.tabTradeStrategy.SelectedTab == tabTicker)
            {
                GetRealtimeTrade(null);
                BindRealtimeTrade(null);
            }
            else if (this.tabTradeStrategy.SelectedTab == tabHistory)
            {
                LoadHistory();
                BindHistory();                
            }
            else if (this.tabTradeStrategy.SelectedTab == tabTradesHistory)
            {
               
                //BindTradeHistory();
            }            
        }

        private void btnTradeRefresh_Click(object sender, EventArgs e)
        {
            loadOrders();
            BindOrders();
        }

        private void comboBuyType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBuyType.SelectedIndex == 1)
            {
                this.txtBuyPrice.Text = "";
                this.txtBuyPrice.Enabled = false;
            }
            else
            {
                this.txtBuyPrice.Enabled = true;
            }
        }

        private void comboSellType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboSellType.SelectedIndex == 1)
            {
                this.txtSellPrice.Text = "";
                this.txtSellPrice.Enabled = false;
            }
            else
            {
                this.txtSellPrice.Enabled = true;
            }
        }

        private string padString(string input, int padding)
        {
            string returnValue = input;
            if (input.Length >= padding)
                return returnValue;
            else
            {
                int len = padding - input.Length;
                for (int i = 0; i < len; i++)
                {
                    returnValue += " ";
                }
            }
            return returnValue;
        }        

        private void linkMail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string strMailTo = "mailto:mtgoxtrade@gmail.com";
        }

        private void btnQuickRefresh_Click(object sender, EventArgs e)
        {
            GetRealtimeTrade(null);
            BindRealtimeTrade(null);
        }

        private void CloseForm(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
            Application.Exit();
        }

        private void HideForm()
        {
            this.Hide();
        }

        private void ShowForm()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;
            e.Cancel = true;
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.ShowForm();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            notifyIcon1.BalloonTipText = ResourceFactory.GetString("Minimized");

            if (FormWindowState.Minimized == this.WindowState)
            {
                this.Hide();
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(500);                
            }
            else if (FormWindowState.Normal == this.WindowState)
            {
                notifyIcon1.Visible = false;
            }
        }

        private void menuItemShow_Click(object sender, EventArgs e)
        {
            this.ShowForm();
        }

        private void menuItemClose_Click(object sender, EventArgs e)
        {
            this.CloseForm(null, null);
        }        

        private void cbAlwaysInFront_CheckedChanged_1(object sender, EventArgs e)
        {
            if (this.cbAlwaysInFront.Checked)
            {
                this.TopMost = true;
            }
            else
                this.TopMost = false;
        }

        private void SaveSettings(object sender, EventArgs e)
        {
            Settings settings = new Settings();
            try
            {
                int refreshTime;
                if (!int.TryParse(this.txtRefresh.Text.Trim(), out refreshTime))
                {
                    refreshTime = Consts.DefaultRefreshTime;

                }
                double minAmount;
                if (!Double.TryParse(this.txtOrderMinAmount.Text.Trim(), out minAmount))
                {
                    minAmount = Consts.DefaultShowMinAmount;
                }
                int showOrders;
                if (!int.TryParse(this.comboShowNumber.SelectedItem.ToString(), out showOrders))
                {
                    showOrders = Consts.DefaultShowOrderNo;
                }
                double highestBuyPrice;
                if (!Double.TryParse(txtHighestBuyPrice.Text.Trim(), out highestBuyPrice))
                {
                    highestBuyPrice = Consts.DefaultHighestBuyPrice;
                }
                double lowestSellPrice;
                if (!Double.TryParse(txtLowestSellPrice.Text.Trim(), out lowestSellPrice))
                {
                    lowestSellPrice = Consts.DefaultLowestSellPrice;
                }

                settings.RefreshTime = refreshTime;
                settings.ShowMinAmount = minAmount;
                settings.ShowOrderNO = showOrders;
                settings.WarnHighBuyPrice = highestBuyPrice;
                settings.WarnLowSellPrice = lowestSellPrice;
                settings.Language = ResourceFactory.Culture;
                settings.AutoTrade = autoTradeSettingsList;
                string settingsFileFullPath = Path.Combine(configFolder, Consts.SettingsFileName);
                Utils.SaveToFile(settings, settingsFileFullPath);
                MessageBox.Show(ResourceFactory.GetString("SettingsSaved"));
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("{0} \r\n stack:{1}", Utils.GetDetailedException(ex), Utils.GetStackTrace(ex)));
            }            
        }

        private void LoadSettings(object sender, EventArgs e)
        {
            try
            {
                string settingsFileFullPath = Path.Combine(configFolder, Consts.SettingsFileName);
                Settings settings = (Settings)Utils.LoadFromFile(settingsFileFullPath, typeof(Settings));
                if (settings == null)
                {
                    settings = new Settings();
                    settings.RefreshTime = Consts.DefaultRefreshTime;
                    settings.ShowMinAmount = Consts.DefaultShowMinAmount;
                    settings.ShowOrderNO = Consts.DefaultShowOrderNo;
                    settings.WarnHighBuyPrice = Consts.DefaultHighestBuyPrice;
                    settings.WarnLowSellPrice = Consts.DefaultLowestSellPrice;
                    settings.Language = ResourceFactory.Culture;
                    Utils.SaveToFile(settings, settingsFileFullPath);
                }
                this.txtRefresh.Text = settings.RefreshTime.ToString();
                this.txtOrderMinAmount.Text = settings.ShowMinAmount.ToString();
                if (settings.ShowOrderNO == 5)
                    this.comboShowNumber.SelectedIndex = 0;
                else if (settings.ShowOrderNO == 10)
                    this.comboShowNumber.SelectedIndex = 1;
                else if (settings.ShowOrderNO == 20)
                    this.comboShowNumber.SelectedIndex = 2;
                this.txtHighestBuyPrice.Text = settings.WarnHighBuyPrice.ToString();
                this.txtLowestSellPrice.Text = settings.WarnLowSellPrice.ToString();
                if(settings.AutoTrade != null)
                    this.autoTradeSettingsList = settings.AutoTrade;                
                if (settings.Language != ResourceFactory.Culture)
                {
                    if (settings.Language == Consts.cnLanguage)
                    {
                        ResourceFactory.ResourceCulture = ResourceFactory.CnCultureInfo;
                        ResourceFactory.ResourceMan = ResourceFactory.CnResourceManager;
                        Thread.CurrentThread.CurrentUICulture = ResourceFactory.CnCultureInfo;
                    }
                    else
                    {
                        ResourceFactory.ResourceCulture = ResourceFactory.EnCultureInfo;
                        ResourceFactory.ResourceMan = ResourceFactory.EnResourceManager;
                        Thread.CurrentThread.CurrentUICulture = ResourceFactory.EnCultureInfo;
                    }
                    ResourceFactory.Culture = settings.Language;
                    InitializeControl();
                    reload();                    
                }
            }
            catch (Exception ex)
            {
            }

        }

        private void LoadAutoTradeSettings()
        {
            this.dgAutoTrade.Rows.Clear();
            foreach (AutoTradeSettings autoTrade in this.autoTradeSettingsList)
            {
                string ruleCondition = string.Empty;
                foreach (RuleSettings rule in autoTrade.Rules)
                {
                    string amount = ShouldAddPercent(rule.RuleIndex) ? rule.RuleCondition + "%" : rule.RuleCondition + "";
                    ruleCondition += ResourceFactory.GetString(Enum.GetName(typeof(AutoTradeRules.TradeRules), rule.RuleIndex)) + " " + amount + "    \r\n";
                }
                string type = autoTrade.TradeType == 0 ? ResourceFactory.GetString("Buy") : ResourceFactory.GetString("Sell");
                this.dgAutoTrade.Rows.Add(autoTrade.OrderId, ruleCondition, type, autoTrade.TradeAmount, autoTrade.OrderTime, ResourceFactory.GetString(autoTrade.Status.ToString()), autoTrade.Sound, ResourceFactory.GetString("AutoTradeDelete"));
            }
        }

        private void SetLanguage(object sender, EventArgs e)
        {
            Language dialog = new Language(ResourceFactory.Culture);
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK && dialog.Culture != ResourceFactory.Culture)
            {
                if (dialog.Culture == Consts.cnLanguage)
                {
                    ResourceFactory.ResourceCulture = ResourceFactory.CnCultureInfo;
                    ResourceFactory.ResourceMan = ResourceFactory.CnResourceManager;
                    Thread.CurrentThread.CurrentUICulture = ResourceFactory.CnCultureInfo;
                }
                else
                {
                    ResourceFactory.ResourceCulture = ResourceFactory.EnCultureInfo;
                    ResourceFactory.ResourceMan = ResourceFactory.EnResourceManager;
                    Thread.CurrentThread.CurrentUICulture = ResourceFactory.EnCultureInfo;
                }
                ResourceFactory.Culture = dialog.Culture;
                InitializeControl();
                reload();
            }
        }

        private void AboutClick(object sender, EventArgs e)
        {
            About about = new About();
            about.ShowDialog();
        }

        private void btnAutoTradeGenerateRules_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentAutoTradeSettings.Rules.Count > 0)
                {
                    this.gbAutoTradeExecution.Enabled = true;
                }
            }
            catch 
            { 
            }            
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                currentAutoTradeSettings.Trade = this.cbAutoTrade.Checked;
                if (currentAutoTradeSettings.Trade)
                {
                    currentAutoTradeSettings.TradeType = this.comboAutoTradeType.SelectedIndex == 0 ? AutoTradeSettings.OrderType.Buy : AutoTradeSettings.OrderType.Sell;
                    currentAutoTradeSettings.TradeAmount = Double.Parse(this.txtAutoTradeAmount.Text.Trim());
                }
                currentAutoTradeSettings.Warn = this.cbWarning.Checked;
                if (currentAutoTradeSettings.Warn)
                {
                    currentAutoTradeSettings.Sound = this.comboAutoTradeWarningSound.SelectedItem.ToString();
                }
                currentAutoTradeSettings.OrderTime = DateTime.Now;
                currentAutoTradeSettings.Status = AutoTradeSettings.OrderStatus.Pending;
                autoTradeSettingsList.Add(currentAutoTradeSettings);
                currentAutoTradeSettings = new AutoTradeSettings();
                this.gbAutoTradeExecution.Enabled = false;
                this.lblAutoTradeRules.Text = string.Empty;                
            }
            catch (Exception ex)
            {
            }
            LoadAutoTradeSettings();
        }

        private void btnAddRules_Click(object sender, EventArgs e)
        {
            try
            {
                int index = this.comboAutoTradeStrategy.SelectedIndex;
                if (index < 0)
                    return;
                if (this.currentAutoTradeSettings.Rules.Where(r => r.RuleIndex == index).Count() != 0)
                {
                    MessageBox.Show(ResourceFactory.GetString("AutoTradeRuleExisted"));
                    return;
                }
                RuleSettings rules = new RuleSettings();
                rules.RuleIndex = index;
                rules.RuleCondition = Double.Parse(this.txtAutoTradeStrategyAmount.Text.Trim());
                this.currentAutoTradeSettings.Rules.Add(rules);
                string amount = ShouldAddPercent(this.comboAutoTradeStrategy.SelectedIndex) ? this.txtAutoTradeStrategyAmount.Text.Trim() + "%" : this.txtAutoTradeStrategyAmount.Text.Trim();
                this.lblAutoTradeRules.Text += this.comboAutoTradeStrategy.SelectedItem.ToString() + " " + amount + "\r\n";
            }
            catch(Exception ex)
            {
                Trace.WriteLine(string.Format("{0} \r\n stack:{1}", Utils.GetDetailedException(ex), Utils.GetStackTrace(ex)));
            }
        }

        private void btnAutoTradeDeleteRules_Click(object sender, EventArgs e)
        {
            try
            {                
                int index = this.comboAutoTradeStrategy.SelectedIndex;
                bool bFound = false;
                for (int i = 0; i < this.currentAutoTradeSettings.Rules.Count; i++)
                {
                    if (index == this.currentAutoTradeSettings.Rules[i].RuleIndex)
                    {
                        bFound = true;
                        this.currentAutoTradeSettings.Rules.RemoveAt(i);
                        break;
                    }
                }
                if (!bFound)
                    return;

                this.lblAutoTradeRules.Text = string.Empty;
                foreach(RuleSettings rule in this.currentAutoTradeSettings.Rules)
                {
                    string amount = ShouldAddPercent(rule.RuleIndex) ? rule.RuleCondition + "%" : rule.RuleCondition + "";
                    this.lblAutoTradeRules.Text += ResourceFactory.GetString(Enum.GetName(typeof(AutoTradeRules.TradeRules), rule.RuleIndex)) + " " + amount + "\r\n";
                }                    
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("{0} \r\n stack:{1}", Utils.GetDetailedException(ex), Utils.GetStackTrace(ex)));
            }
        }

        private void btnAutoTradeCancel_Click(object sender, EventArgs e)
        {
            this.currentAutoTradeSettings = new AutoTradeSettings();
            this.gbAutoTradeExecution.Enabled = false;
            this.lblAutoTradeRules.Text = string.Empty;
        }

        private void dgAutoTrade_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 7 && dgAutoTrade.Rows[e.RowIndex].Cells[0].Value != null)
                {
                    this.autoTradeSettingsList.RemoveAt(e.RowIndex);
                    LoadAutoTradeSettings();
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("{0} \r\n stack:{1}", Utils.GetDetailedException(ex), Utils.GetStackTrace(ex)));
            }
        }

        private void executeAutoTrade()
        {
            try
            {
                SmpleTradeStrategy strategy = new SmpleTradeStrategy();
                strategy.SoundFolder = Path.Combine(Application.StartupPath, Consts.SoundFolder);
                strategy.ComputeNewOrders(currentInfo, currenttradeListInOneMin, currenttradeListInFiveMin, currentTicker, userInfo, mtgoxV0, autoTradeSettingsList);                                
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("{0} \r\n stack:{1}", Utils.GetDetailedException(ex), Utils.GetStackTrace(ex)));
            }
        }

        private void comboAutoTradeStrategy_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ShouldAddPercent(comboAutoTradeStrategy.SelectedIndex))
                this.lblAutoTradePercent.Visible = true;
            else
                this.lblAutoTradePercent.Visible = false;                   
        }

        private void reload()
        {
            if (this.tabTradeStrategy.SelectedTab == tabAccount)
            {
                BindUserInfo();
            }
            else if (this.tabTradeStrategy.SelectedTab == tabTrade)
            {
                BindOrders();
            }
            else if (this.tabTradeStrategy.SelectedTab == tabTicker)
            {
                if (currentInfo == null)
                {
                    GetRealtimeTrade(null);
                    BindRealtimeTrade(null);
                }
            }
            else if (this.tabTradeStrategy.SelectedTab == tabHistory)
            {
                BindHistory();
            }
            else if (this.tabTradeStrategy.SelectedTab == tabTradesHistory)
            {
                //BindTradeHistory();
            }
            else if (this.tabTradeStrategy.SelectedTab == tabAutoTrade)
            {
                LoadAutoTradeSettings();
            }
        }

        private bool ShouldAddPercent(int index)
        {
            bool should = false;
            switch (index)
            {
                case 2:
                case 3:
                case 6:
                case 7:
                    should = true;
                    break;
                default:
                    should =  false;
                    break;
            }
            return should;
        }
    }
}
