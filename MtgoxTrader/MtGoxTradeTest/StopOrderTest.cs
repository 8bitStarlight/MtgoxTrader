using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MtGoxTrader.Trader;
using MtGoxTrader.MtGoxAPIClient;

namespace MtGoxTradeTest
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class StopOrderTest
    {
        public StopOrderTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void SaveToFile()
        {
            List<StopOrder> orderList = new List<StopOrder>();
            StopOrder order = new StopOrder();
            
            order.Amount = 1;
            order.Currency = MtGoxCurrencySymbol.USD;
            order.ExecuteTime = System.DateTime.Now;
            order.OrderTime = System.DateTime.Now;
            order.Price = 2;
            order.Status = StopOrder.OrderStatus.Pending;
            order.Type = StopOrder.OrderType.SellStop;
            string folder = System.AppDomain.CurrentDomain.BaseDirectory.TrimEnd(new char[] { System.IO.Path.DirectorySeparatorChar });
            string fileName = Path.Combine(Path.GetDirectoryName(folder), "test.dat");
            StopOrder order1 = order;
            order1.Amount = 2;
            order1.Price = 4;
            orderList.Add(order);
            orderList.Add(order1);
            StopOrderHelper.SaveToFile(orderList, fileName);

            List<StopOrder> orderList2 = StopOrderHelper.LoadFromFile(fileName);
            Assert.AreEqual(orderList2.Count, 2);
            StopOrder order2 = orderList2[0];
            Assert.AreEqual(order.Amount, order2.Amount);
            Assert.AreEqual(order.Currency, order2.Currency);
            Assert.AreEqual(order.ExecuteTime, order2.ExecuteTime);
            Assert.AreEqual(order.OrderTime, order2.OrderTime);
            Assert.AreEqual(order.Price, order2.Price);
            Assert.AreEqual(order.Status, order2.Status);
            Assert.AreEqual(order.Type, order2.Type);
            //
            // TODO: Add test logic here
            //
        }

        [TestMethod]
        public void Execute()
        {
            List<StopOrder> orderList = new List<StopOrder>();
            StopOrder order = new StopOrder();

            order.Amount = 1;
            order.Currency = MtGoxCurrencySymbol.USD;
            order.ExecuteTime = System.DateTime.Now;
            order.OrderTime = System.DateTime.Now;
            order.Price = 2;
            order.Status = StopOrder.OrderStatus.Pending;
            order.Type = StopOrder.OrderType.SellStop;
            order.Execute(1.99, 1.98);

        }
    }
}
