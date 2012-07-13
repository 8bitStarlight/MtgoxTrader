//****************************************************************************             
//
// @File: About.cs
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

namespace MtGoxTrader.Trader
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
            this.label1.Text = "Version: 1.0.0.1\r\nIf you enjoy this software,\r\n support its development by donating to\r\n1LBwLBgz6CfvBuDTwkr9kzEYc7RyGk1SU8";
        }
    }
}
