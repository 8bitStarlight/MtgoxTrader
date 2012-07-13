//****************************************************************************             
//
// @File: Language.cs
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
    public partial class Language : Form
    {
        public string Culture { get; set; }
        public Language(string culture)
        {
            InitializeComponent();
            Culture = culture;
            this.comboBox1.Items.Clear();
            this.comboBox1.Items.Add("English");
            this.comboBox1.Items.Add("中文");
            if (Culture == Consts.enLanguage)
                this.comboBox1.SelectedIndex = 0;
            else
                this.comboBox1.SelectedIndex = 1;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.SelectedIndex == 0)
                Culture = Consts.enLanguage;
            else if (this.comboBox1.SelectedIndex == 1)
                Culture = Consts.cnLanguage;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
