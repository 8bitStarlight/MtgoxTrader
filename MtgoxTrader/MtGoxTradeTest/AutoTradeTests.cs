//****************************************************************************             
//
// @File: AutoTradeTests.cs
// @owner: iamapi 
//    
// Notes:
//	
// @EndHeader@
//****************************************************************************
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MtGoxTrader.Model;
using MtGoxTrader.TradeStrategy;

namespace MtGoxTradeTest
{
    [TestClass]
    public class AutoTradeTests
    {
        private MtGoxDepthInfo depth;
        private List<MtGoxTrade> tradeListInFiveMin;
        private List<MtGoxTrade> tradeListInOneMin;
        private MtGoxTickerItem ticker;
        [TestInitialize]
        public void Initialize()
        {
            depth = new MtGoxDepthInfo();
            tradeListInFiveMin = new List<MtGoxTrade>();
            tradeListInOneMin = new List<MtGoxTrade>();
            ticker = new MtGoxTickerItem();

            for (int i = 0; i < 100; i++)
            {
                MtGoxAsk a = new MtGoxAsk();
                a.amount = i + 1;
                a.price = i + 1;
                depth.asks.Add(a);
                MtGoxBid b = new MtGoxBid();
                b.amount = i + 1;
                b.price = i + 1;
                depth.bids.Add(b);
                MtGoxTrade t = new MtGoxTrade();
                t.amount = i + 1;
                t.price = i + 1;
                tradeListInFiveMin.Add(t);
                tradeListInOneMin.Add(t);
            }
            ticker.last = 5;
            
        }
        [TestMethod]
        public void TestAutoTradePriceHigher()
        {
            AutoTradePriceHigher at = (AutoTradePriceHigher)AutoTradeRuleFactory.CreateAutoTradeRule(0);
            Assert.IsTrue(at.ShouldExecute(depth, tradeListInOneMin, tradeListInFiveMin, ticker, 4.99));
        }
        [TestMethod]
        public void TestAutoTradePriceLower()
        {
            AutoTradePriceLower at = (AutoTradePriceLower)AutoTradeRuleFactory.CreateAutoTradeRule(1);
            Assert.IsTrue(at.ShouldExecute(depth, tradeListInOneMin, tradeListInFiveMin, ticker, 5.01));
        }
        [TestMethod]
        public void TestAutoTradeFiveMinIncreaseHigher()
        {
            tradeListInFiveMin[0].price = 4.9;
            tradeListInFiveMin[99].price = 5.1;
            AutoTradeFiveMinIncreaseHigher at = (AutoTradeFiveMinIncreaseHigher)AutoTradeRuleFactory.CreateAutoTradeRule(2);
            Assert.IsTrue(at.ShouldExecute(depth, tradeListInOneMin, tradeListInFiveMin, ticker, 1.9));
        }
        [TestMethod]
        public void TestAutoTradeFiveMinDropHigher()
        {
            tradeListInFiveMin[0].price = 5.1;
            tradeListInFiveMin[99].price = 4.9;
            AutoTradeFiveMinDropHigher at = (AutoTradeFiveMinDropHigher)AutoTradeRuleFactory.CreateAutoTradeRule(3);
            Assert.IsTrue(at.ShouldExecute(depth, tradeListInOneMin, tradeListInFiveMin, ticker, 1.9));
        }
        [TestMethod]
        public void TestAutoTradeFiveMinDealAmountHigher()
        {
            AutoTradeFiveMinDealAmountHigher at = (AutoTradeFiveMinDealAmountHigher)AutoTradeRuleFactory.CreateAutoTradeRule(4);
            Assert.IsTrue(at.ShouldExecute(depth, tradeListInOneMin, tradeListInFiveMin, ticker, 5000));
        }
        [TestMethod]
        public void TestAutoTradeFiveMinAmountPerTradeHigher()
        {
            AutoTradeFiveMinAmountPerTradeHigher at = (AutoTradeFiveMinAmountPerTradeHigher)AutoTradeRuleFactory.CreateAutoTradeRule(5);
            Assert.IsTrue(at.ShouldExecute(depth, tradeListInOneMin, tradeListInFiveMin, ticker, 99));
        }
        
        [TestMethod]
        public void TestAutoTradeSellAmountIn10Higher()
        {
            AutoTradeSellAmountIn10Higher at = (AutoTradeSellAmountIn10Higher)AutoTradeRuleFactory.CreateAutoTradeRule(10);
            Assert.IsTrue(at.ShouldExecute(depth, tradeListInOneMin, tradeListInFiveMin, ticker, 50));
        }
        
    }
}
