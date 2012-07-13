namespace MtGoxTrader.Trader
{
    partial class Configure
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Configure));
            this.lblAPIKey = new System.Windows.Forms.Label();
            this.txtKey = new System.Windows.Forms.TextBox();
            this.lblAPISecert = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.lblErrorMsg = new System.Windows.Forms.Label();
            this.txtSecret = new System.Windows.Forms.TextBox();
            this.lblCurrency = new System.Windows.Forms.Label();
            this.comboCurrency = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // lblAPIKey
            // 
            this.lblAPIKey.AutoSize = true;
            this.lblAPIKey.Location = new System.Drawing.Point(4, 8);
            this.lblAPIKey.Name = "lblAPIKey";
            this.lblAPIKey.Size = new System.Drawing.Size(82, 13);
            this.lblAPIKey.TabIndex = 0;
            this.lblAPIKey.Text = "MtGox API Key:";
            // 
            // txtKey
            // 
            this.txtKey.Location = new System.Drawing.Point(104, 6);
            this.txtKey.Name = "txtKey";
            this.txtKey.Size = new System.Drawing.Size(220, 20);
            this.txtKey.TabIndex = 1;
            // 
            // lblAPISecert
            // 
            this.lblAPISecert.AutoSize = true;
            this.lblAPISecert.Location = new System.Drawing.Point(4, 50);
            this.lblAPISecert.Name = "lblAPISecert";
            this.lblAPISecert.Size = new System.Drawing.Size(95, 13);
            this.lblAPISecert.TabIndex = 2;
            this.lblAPISecert.Text = "MtGox API Secret:";
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(104, 171);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 25);
            this.btnOk.TabIndex = 4;
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // lblErrorMsg
            // 
            this.lblErrorMsg.AutoSize = true;
            this.lblErrorMsg.ForeColor = System.Drawing.Color.Red;
            this.lblErrorMsg.Location = new System.Drawing.Point(100, 121);
            this.lblErrorMsg.Name = "lblErrorMsg";
            this.lblErrorMsg.Size = new System.Drawing.Size(0, 13);
            this.lblErrorMsg.TabIndex = 5;
            this.lblErrorMsg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtSecret
            // 
            this.txtSecret.Location = new System.Drawing.Point(104, 38);
            this.txtSecret.Multiline = true;
            this.txtSecret.Name = "txtSecret";
            this.txtSecret.Size = new System.Drawing.Size(220, 60);
            this.txtSecret.TabIndex = 6;
            // 
            // lblCurrency
            // 
            this.lblCurrency.AutoSize = true;
            this.lblCurrency.Location = new System.Drawing.Point(4, 110);
            this.lblCurrency.Name = "lblCurrency";
            this.lblCurrency.Size = new System.Drawing.Size(49, 13);
            this.lblCurrency.TabIndex = 7;
            this.lblCurrency.Text = "Currency";
            // 
            // comboCurrency
            // 
            this.comboCurrency.FormattingEnabled = true;
            this.comboCurrency.Items.AddRange(new object[] {
            "USD",
            "AUD",
            "CAD",
            "CHF",
            "CNY",
            "DKK",
            "EUR",
            "GBP",
            "HKD",
            "JPY",
            "NZD",
            "PLN",
            "RUB",
            "SEK",
            "SGD",
            "THB",
            "BTC"});
            this.comboCurrency.Location = new System.Drawing.Point(104, 108);
            this.comboCurrency.Name = "comboCurrency";
            this.comboCurrency.Size = new System.Drawing.Size(60, 21);
            this.comboCurrency.TabIndex = 8;
            // 
            // Configure
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(350, 202);
            this.Controls.Add(this.comboCurrency);
            this.Controls.Add(this.lblCurrency);
            this.Controls.Add(this.txtSecret);
            this.Controls.Add(this.lblErrorMsg);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.lblAPISecert);
            this.Controls.Add(this.txtKey);
            this.Controls.Add(this.lblAPIKey);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Configure";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Configure_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblAPIKey;
        private System.Windows.Forms.TextBox txtKey;
        private System.Windows.Forms.Label lblAPISecert;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Label lblErrorMsg;
        private System.Windows.Forms.TextBox txtSecret;
        private System.Windows.Forms.Label lblCurrency;
        private System.Windows.Forms.ComboBox comboCurrency;
    }
}