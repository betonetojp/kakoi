﻿namespace kakoi
{
    partial class FormWeb
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWeb));
            webView2 = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)webView2).BeginInit();
            SuspendLayout();
            // 
            // webView2
            // 
            webView2.AllowExternalDrop = true;
            webView2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            webView2.CreationProperties = null;
            webView2.DefaultBackgroundColor = Color.White;
            webView2.Location = new Point(12, 12);
            webView2.Name = "webView2";
            webView2.Size = new Size(360, 337);
            webView2.TabIndex = 0;
            webView2.ZoomFactor = 1D;
            // 
            // FormWeb
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(384, 361);
            Controls.Add(webView2);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(320, 320);
            Name = "FormWeb";
            SizeGripStyle = SizeGripStyle.Show;
            StartPosition = FormStartPosition.Manual;
            Text = "kakoi";
            FormClosing += FormWeb_FormClosing;
            ((System.ComponentModel.ISupportInitialize)webView2).EndInit();
            ResumeLayout(false);
        }

        #endregion

        internal Microsoft.Web.WebView2.WinForms.WebView2 webView2;
    }
}