namespace kakoi
{
    partial class FormSetting
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSetting));
            textBoxNokakoiKey = new TextBox();
            textBoxPassword = new TextBox();
            trackBarOpacity = new TrackBar();
            checkBoxTopMost = new CheckBox();
            label1 = new Label();
            label2 = new Label();
            checkBoxAddClient = new CheckBox();
            label4 = new Label();
            linkLabelIcons8 = new LinkLabel();
            checkBoxShowOnlyJapanese = new CheckBox();
            labelOpacity = new Label();
            checkBoxShowOnlyFollowees = new CheckBox();
            label3 = new Label();
            checkBoxSendDSSTP = new CheckBox();
            linkLabelVersion = new LinkLabel();
            checkBoxShowAvatar = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)trackBarOpacity).BeginInit();
            SuspendLayout();
            // 
            // textBoxNokakoiKey
            // 
            textBoxNokakoiKey.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxNokakoiKey.BorderStyle = BorderStyle.FixedSingle;
            textBoxNokakoiKey.ImeMode = ImeMode.Disable;
            textBoxNokakoiKey.Location = new Point(88, 112);
            textBoxNokakoiKey.MaxLength = 136;
            textBoxNokakoiKey.Name = "textBoxNokakoiKey";
            textBoxNokakoiKey.Size = new Size(184, 23);
            textBoxNokakoiKey.TabIndex = 6;
            // 
            // textBoxPassword
            // 
            textBoxPassword.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxPassword.BorderStyle = BorderStyle.FixedSingle;
            textBoxPassword.ImeMode = ImeMode.Disable;
            textBoxPassword.Location = new Point(88, 141);
            textBoxPassword.MaxLength = 256;
            textBoxPassword.Name = "textBoxPassword";
            textBoxPassword.PasswordChar = '*';
            textBoxPassword.PlaceholderText = "password";
            textBoxPassword.Size = new Size(184, 23);
            textBoxPassword.TabIndex = 7;
            // 
            // trackBarOpacity
            // 
            trackBarOpacity.Location = new Point(152, 31);
            trackBarOpacity.Maximum = 100;
            trackBarOpacity.Minimum = 20;
            trackBarOpacity.Name = "trackBarOpacity";
            trackBarOpacity.Size = new Size(120, 45);
            trackBarOpacity.TabIndex = 2;
            trackBarOpacity.TickFrequency = 20;
            trackBarOpacity.Value = 100;
            trackBarOpacity.Scroll += TrackBarOpacity_Scroll;
            // 
            // checkBoxTopMost
            // 
            checkBoxTopMost.AutoSize = true;
            checkBoxTopMost.Location = new Point(12, 12);
            checkBoxTopMost.Name = "checkBoxTopMost";
            checkBoxTopMost.Size = new Size(101, 19);
            checkBoxTopMost.TabIndex = 1;
            checkBoxTopMost.Text = "Always on top";
            checkBoxTopMost.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(152, 13);
            label1.Name = "label1";
            label1.Size = new Size(48, 15);
            label1.TabIndex = 0;
            label1.Text = "Opacity";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 114);
            label2.Name = "label2";
            label2.Size = new Size(70, 15);
            label2.TabIndex = 0;
            label2.Text = "nokakoi key";
            // 
            // checkBoxAddClient
            // 
            checkBoxAddClient.AutoSize = true;
            checkBoxAddClient.Checked = true;
            checkBoxAddClient.CheckState = CheckState.Checked;
            checkBoxAddClient.Location = new Point(172, 170);
            checkBoxAddClient.Name = "checkBoxAddClient";
            checkBoxAddClient.Size = new Size(100, 19);
            checkBoxAddClient.TabIndex = 9;
            checkBoxAddClient.Text = "Add client tag";
            checkBoxAddClient.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label4.AutoSize = true;
            label4.ForeColor = SystemColors.GrayText;
            label4.Location = new Point(99, 237);
            label4.Name = "label4";
            label4.Size = new Size(126, 15);
            label4.TabIndex = 0;
            label4.Text = "Monochrome icons by";
            // 
            // linkLabelIcons8
            // 
            linkLabelIcons8.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            linkLabelIcons8.AutoSize = true;
            linkLabelIcons8.Location = new Point(231, 237);
            linkLabelIcons8.Name = "linkLabelIcons8";
            linkLabelIcons8.Size = new Size(41, 15);
            linkLabelIcons8.TabIndex = 11;
            linkLabelIcons8.TabStop = true;
            linkLabelIcons8.Text = "Icons8";
            linkLabelIcons8.LinkClicked += LinkLabelIcons8_LinkClicked;
            // 
            // checkBoxShowOnlyJapanese
            // 
            checkBoxShowOnlyJapanese.AutoSize = true;
            checkBoxShowOnlyJapanese.Location = new Point(12, 87);
            checkBoxShowOnlyJapanese.Name = "checkBoxShowOnlyJapanese";
            checkBoxShowOnlyJapanese.Size = new Size(162, 19);
            checkBoxShowOnlyJapanese.TabIndex = 5;
            checkBoxShowOnlyJapanese.Text = "Show only Japanese posts";
            checkBoxShowOnlyJapanese.UseVisualStyleBackColor = true;
            // 
            // labelOpacity
            // 
            labelOpacity.Location = new Point(231, 13);
            labelOpacity.Name = "labelOpacity";
            labelOpacity.Size = new Size(41, 15);
            labelOpacity.TabIndex = 0;
            labelOpacity.Text = "100%";
            labelOpacity.TextAlign = ContentAlignment.TopRight;
            // 
            // checkBoxShowOnlyFollowees
            // 
            checkBoxShowOnlyFollowees.AutoSize = true;
            checkBoxShowOnlyFollowees.Location = new Point(12, 62);
            checkBoxShowOnlyFollowees.Name = "checkBoxShowOnlyFollowees";
            checkBoxShowOnlyFollowees.Size = new Size(134, 19);
            checkBoxShowOnlyFollowees.TabIndex = 4;
            checkBoxShowOnlyFollowees.Text = "Show only followees";
            checkBoxShowOnlyFollowees.UseVisualStyleBackColor = true;
            checkBoxShowOnlyFollowees.CheckedChanged += checkBoxShowOnlyFollowees_CheckedChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 143);
            label3.Name = "label3";
            label3.Size = new Size(57, 15);
            label3.TabIndex = 0;
            label3.Text = "password";
            // 
            // checkBoxSendDSSTP
            // 
            checkBoxSendDSSTP.AutoSize = true;
            checkBoxSendDSSTP.ForeColor = SystemColors.ControlDark;
            checkBoxSendDSSTP.Location = new Point(12, 170);
            checkBoxSendDSSTP.Name = "checkBoxSendDSSTP";
            checkBoxSendDSSTP.Size = new Size(88, 19);
            checkBoxSendDSSTP.TabIndex = 8;
            checkBoxSendDSSTP.Text = "Send DSSTP";
            checkBoxSendDSSTP.UseVisualStyleBackColor = true;
            // 
            // linkLabelVersion
            // 
            linkLabelVersion.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            linkLabelVersion.AutoSize = true;
            linkLabelVersion.Location = new Point(12, 237);
            linkLabelVersion.Name = "linkLabelVersion";
            linkLabelVersion.Size = new Size(44, 15);
            linkLabelVersion.TabIndex = 10;
            linkLabelVersion.TabStop = true;
            linkLabelVersion.Text = "v0.3.0b";
            linkLabelVersion.LinkClicked += LinkLabelVersion_LinkClicked;
            // 
            // checkBoxShowAvatar
            // 
            checkBoxShowAvatar.AutoSize = true;
            checkBoxShowAvatar.Location = new Point(12, 37);
            checkBoxShowAvatar.Name = "checkBoxShowAvatar";
            checkBoxShowAvatar.Size = new Size(90, 19);
            checkBoxShowAvatar.TabIndex = 3;
            checkBoxShowAvatar.Text = "Show avatar";
            checkBoxShowAvatar.UseVisualStyleBackColor = true;
            // 
            // FormSetting
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(284, 261);
            Controls.Add(checkBoxShowAvatar);
            Controls.Add(linkLabelVersion);
            Controls.Add(checkBoxSendDSSTP);
            Controls.Add(label3);
            Controls.Add(checkBoxShowOnlyFollowees);
            Controls.Add(labelOpacity);
            Controls.Add(checkBoxShowOnlyJapanese);
            Controls.Add(linkLabelIcons8);
            Controls.Add(label4);
            Controls.Add(checkBoxAddClient);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(checkBoxTopMost);
            Controls.Add(trackBarOpacity);
            Controls.Add(textBoxPassword);
            Controls.Add(textBoxNokakoiKey);
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(300, 300);
            Name = "FormSetting";
            ShowInTaskbar = false;
            SizeGripStyle = SizeGripStyle.Show;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Setting";
            TopMost = true;
            Load += FormSetting_Load;
            Shown += FormSetting_Shown;
            KeyDown += FormSetting_KeyDown;
            ((System.ComponentModel.ISupportInitialize)trackBarOpacity).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        internal TextBox textBoxNokakoiKey;
        internal TextBox textBoxPassword;
        internal TrackBar trackBarOpacity;
        internal CheckBox checkBoxTopMost;
        private Label label1;
        private Label label2;
        internal CheckBox checkBoxAddClient;
        private Label label4;
        private LinkLabel linkLabelIcons8;
        internal CheckBox checkBoxShowOnlyJapanese;
        private Label labelOpacity;
        internal CheckBox checkBoxShowOnlyFollowees;
        private Label label3;
        internal CheckBox checkBoxSendDSSTP;
        private LinkLabel linkLabelVersion;
        internal CheckBox checkBoxShowAvatar;
    }
}