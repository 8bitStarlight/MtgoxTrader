//****************************************************************************             
//
// @File: Configuration.cs
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
using System.Configuration;
using System.IO;
using MtGoxTrader.MtGoxAPIClient;
using MtGoxTrader.Model;

namespace MtGoxTrader.Trader
{
    public partial class Configure : Form
    {
        private string configrFullPath = null;
        public static Config MtGoxConfig = null;
        private bool needClose = false;
        public static int Expiration = 0;
        public Configure(int expire = 0)
        {
            Expiration = expire;
            InitializeComponent();
            Init();
            string configFolder = Path.Combine(Application.StartupPath, Consts.DataFolder);
            configrFullPath = Path.Combine(configFolder, Consts.ConfigFileName);
            try
            {
                if (!Directory.Exists(configFolder))
                {
                    Directory.CreateDirectory(configFolder);
                }
            }
            catch (Exception ex)
            {
            }
            this.AcceptButton = this.btnOk;
            this.comboCurrency.DataSource = Enum.GetValues(typeof(MtGoxCurrencySymbol));
            showContent();
        }

        private void Init()
        {
            this.btnOk.Text = ResourceFactory.GetString("ConfigurationFinished");
            this.Text = ResourceFactory.GetString("ConfigTitle");

        }

        private void showContent()
        {
            MtGoxConfig = new Config();
            try
            {
                MtGoxConfig = ConfigHelper.LoadFromFile(this.configrFullPath);
                this.txtKey.Text = MtGoxConfig.Key;
                this.txtSecret.Text = MtGoxConfig.Secret;
                this.comboCurrency.SelectedItem = MtGoxConfig.Currency;
            }
            catch(Exception e)
            {
                
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (checkField())
            {
                MtGoxConfig.Key = this.txtKey.Text.Trim();
                MtGoxConfig.Secret = this.txtSecret.Text.Trim();
                MtGoxConfig.Currency = (MtGoxCurrencySymbol)this.comboCurrency.SelectedItem;
                try
                {
                    ConfigHelper.SaveToFile(MtGoxConfig, this.configrFullPath);
                }
                catch (Exception ex)
                {

                }
                needClose = true;
                this.Close();
            }
        }

        private bool checkField()
        {
            bool bStatus = true;
            this.lblErrorMsg.Visible = false;
            this.lblErrorMsg.Text = "";

            do
            {
                if (this.txtKey.Text.Trim().Equals(string.Empty) || this.txtSecret.Text.Trim().Equals(string.Empty))
                {
                    this.lblErrorMsg.Text = ResourceFactory.GetString("ConfigurationError");
                    this.lblErrorMsg.Visible = true;
                    bStatus = false;
                }
                else
                {
                    bStatus = true;

                }           
            } while (false);
            return bStatus;
        }

        private void Configure_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (needClose)
            {
                Form1 form1 = new Form1();
                form1.MtGoxConfig = MtGoxConfig;
                if (form1.Init())
                {
                    form1.Show();
                }
                else
                {
                    MessageBox.Show("Please check your api key and secret!");
                    needClose = false;
                    Application.Exit();
                }
            }            
            else
            {
                Application.Exit();
            }
        }
    }
}
