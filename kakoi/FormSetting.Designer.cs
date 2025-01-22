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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSetting));
            textBoxNsec = new TextBox();
            trackBarOpacity = new TrackBar();
            checkBoxTopMost = new CheckBox();
            label1 = new Label();
            checkBoxAddClient = new CheckBox();
            label4 = new Label();
            linkLabelIcons8 = new LinkLabel();
            checkBoxShowOnlySelectedLanguage = new CheckBox();
            labelOpacity = new Label();
            checkBoxShowOnlyFollowees = new CheckBox();
            label3 = new Label();
            checkBoxSendDSSTP = new CheckBox();
            linkLabelVersion = new LinkLabel();
            checkBoxGetAvatar = new CheckBox();
            checkBoxShowRepostsOnlyFromFollowees = new CheckBox();
            checkBoxMinimizeToTray = new CheckBox();
            label2 = new Label();
            textBoxNpub = new TextBox();
            buttonLogOut = new Button();
            toolTipLogOut = new ToolTip(components);
            checkBoxDAN = new CheckBox();
            checkBoxDEU = new CheckBox();
            checkBoxENG = new CheckBox();
            checkBoxFRA = new CheckBox();
            checkBoxITA = new CheckBox();
            checkBoxJPN = new CheckBox();
            checkBoxKOR = new CheckBox();
            checkBoxNLD = new CheckBox();
            checkBoxNOR = new CheckBox();
            checkBoxPOR = new CheckBox();
            checkBoxRUS = new CheckBox();
            checkBoxSPA = new CheckBox();
            checkBoxSWE = new CheckBox();
            checkBoxZHO = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)trackBarOpacity).BeginInit();
            SuspendLayout();
            // 
            // textBoxNsec
            // 
            textBoxNsec.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxNsec.BorderStyle = BorderStyle.FixedSingle;
            textBoxNsec.ImeMode = ImeMode.Disable;
            textBoxNsec.Location = new Point(82, 212);
            textBoxNsec.MaxLength = 256;
            textBoxNsec.Name = "textBoxNsec";
            textBoxNsec.PasswordChar = '*';
            textBoxNsec.PlaceholderText = "nsec1...";
            textBoxNsec.Size = new Size(221, 23);
            textBoxNsec.TabIndex = 21;
            textBoxNsec.Leave += TextBoxNsec_Leave;
            // 
            // trackBarOpacity
            // 
            trackBarOpacity.Location = new Point(212, 31);
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
            label1.Location = new Point(212, 13);
            label1.Name = "label1";
            label1.Size = new Size(48, 15);
            label1.TabIndex = 0;
            label1.Text = "Opacity";
            // 
            // checkBoxAddClient
            // 
            checkBoxAddClient.AutoSize = true;
            checkBoxAddClient.Checked = true;
            checkBoxAddClient.CheckState = CheckState.Checked;
            checkBoxAddClient.ForeColor = SystemColors.ControlText;
            checkBoxAddClient.Location = new Point(97, 270);
            checkBoxAddClient.Name = "checkBoxAddClient";
            checkBoxAddClient.Size = new Size(100, 19);
            checkBoxAddClient.TabIndex = 25;
            checkBoxAddClient.Text = "Add client tag";
            checkBoxAddClient.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label4.AutoSize = true;
            label4.ForeColor = SystemColors.GrayText;
            label4.Location = new Point(139, 297);
            label4.Name = "label4";
            label4.Size = new Size(126, 15);
            label4.TabIndex = 0;
            label4.Text = "Monochrome icons by";
            // 
            // linkLabelIcons8
            // 
            linkLabelIcons8.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            linkLabelIcons8.AutoSize = true;
            linkLabelIcons8.Location = new Point(271, 297);
            linkLabelIcons8.Name = "linkLabelIcons8";
            linkLabelIcons8.Size = new Size(41, 15);
            linkLabelIcons8.TabIndex = 28;
            linkLabelIcons8.TabStop = true;
            linkLabelIcons8.Text = "Icons8";
            linkLabelIcons8.LinkClicked += LinkLabelIcons8_LinkClicked;
            // 
            // checkBoxShowOnlySelectedLanguage
            // 
            checkBoxShowOnlySelectedLanguage.AutoSize = true;
            checkBoxShowOnlySelectedLanguage.ForeColor = SystemColors.ControlText;
            checkBoxShowOnlySelectedLanguage.Location = new Point(12, 112);
            checkBoxShowOnlySelectedLanguage.Name = "checkBoxShowOnlySelectedLanguage";
            checkBoxShowOnlySelectedLanguage.Size = new Size(286, 19);
            checkBoxShowOnlySelectedLanguage.TabIndex = 6;
            checkBoxShowOnlySelectedLanguage.Text = "Show only selected language from non-followees";
            checkBoxShowOnlySelectedLanguage.UseVisualStyleBackColor = true;
            // 
            // labelOpacity
            // 
            labelOpacity.Location = new Point(291, 13);
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
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 214);
            label3.Name = "label3";
            label3.Size = new Size(64, 15);
            label3.TabIndex = 0;
            label3.Text = "Private key";
            // 
            // checkBoxSendDSSTP
            // 
            checkBoxSendDSSTP.AutoSize = true;
            checkBoxSendDSSTP.ForeColor = SystemColors.ControlText;
            checkBoxSendDSSTP.Location = new Point(203, 270);
            checkBoxSendDSSTP.Name = "checkBoxSendDSSTP";
            checkBoxSendDSSTP.Size = new Size(88, 19);
            checkBoxSendDSSTP.TabIndex = 26;
            checkBoxSendDSSTP.Text = "Send DSSTP";
            checkBoxSendDSSTP.UseVisualStyleBackColor = true;
            // 
            // linkLabelVersion
            // 
            linkLabelVersion.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            linkLabelVersion.AutoSize = true;
            linkLabelVersion.Location = new Point(12, 297);
            linkLabelVersion.Name = "linkLabelVersion";
            linkLabelVersion.Size = new Size(37, 15);
            linkLabelVersion.TabIndex = 27;
            linkLabelVersion.TabStop = true;
            linkLabelVersion.Text = "v0.7.0";
            linkLabelVersion.LinkClicked += LinkLabelVersion_LinkClicked;
            // 
            // checkBoxGetAvatar
            // 
            checkBoxGetAvatar.AutoSize = true;
            checkBoxGetAvatar.Checked = true;
            checkBoxGetAvatar.CheckState = CheckState.Checked;
            checkBoxGetAvatar.Location = new Point(12, 270);
            checkBoxGetAvatar.Name = "checkBoxGetAvatar";
            checkBoxGetAvatar.Size = new Size(79, 19);
            checkBoxGetAvatar.TabIndex = 24;
            checkBoxGetAvatar.Text = "Get avatar";
            checkBoxGetAvatar.UseVisualStyleBackColor = true;
            // 
            // checkBoxShowRepostsOnlyFromFollowees
            // 
            checkBoxShowRepostsOnlyFromFollowees.AutoSize = true;
            checkBoxShowRepostsOnlyFromFollowees.ForeColor = SystemColors.ControlText;
            checkBoxShowRepostsOnlyFromFollowees.Location = new Point(12, 87);
            checkBoxShowRepostsOnlyFromFollowees.Name = "checkBoxShowRepostsOnlyFromFollowees";
            checkBoxShowRepostsOnlyFromFollowees.Size = new Size(203, 19);
            checkBoxShowRepostsOnlyFromFollowees.TabIndex = 5;
            checkBoxShowRepostsOnlyFromFollowees.Text = "Show reposts only from followees";
            checkBoxShowRepostsOnlyFromFollowees.UseVisualStyleBackColor = true;
            // 
            // checkBoxMinimizeToTray
            // 
            checkBoxMinimizeToTray.AutoSize = true;
            checkBoxMinimizeToTray.Location = new Point(12, 37);
            checkBoxMinimizeToTray.Name = "checkBoxMinimizeToTray";
            checkBoxMinimizeToTray.Size = new Size(150, 19);
            checkBoxMinimizeToTray.TabIndex = 3;
            checkBoxMinimizeToTray.Text = "Minimize to system tray";
            checkBoxMinimizeToTray.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 243);
            label2.Name = "label2";
            label2.Size = new Size(61, 15);
            label2.TabIndex = 15;
            label2.Text = "Public key";
            // 
            // textBoxNpub
            // 
            textBoxNpub.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxNpub.BorderStyle = BorderStyle.FixedSingle;
            textBoxNpub.Location = new Point(82, 241);
            textBoxNpub.Name = "textBoxNpub";
            textBoxNpub.PlaceholderText = "npub1...";
            textBoxNpub.ReadOnly = true;
            textBoxNpub.Size = new Size(250, 23);
            textBoxNpub.TabIndex = 23;
            // 
            // buttonLogOut
            // 
            buttonLogOut.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonLogOut.Image = Properties.Resources.icons8_log_out_16;
            buttonLogOut.Location = new Point(309, 212);
            buttonLogOut.Name = "buttonLogOut";
            buttonLogOut.Size = new Size(23, 23);
            buttonLogOut.TabIndex = 22;
            toolTipLogOut.SetToolTip(buttonLogOut, "Log out");
            buttonLogOut.UseVisualStyleBackColor = true;
            buttonLogOut.Click += ButtonLogOut_Click;
            // 
            // checkBoxDAN
            // 
            checkBoxDAN.AutoSize = true;
            checkBoxDAN.Location = new Point(32, 137);
            checkBoxDAN.Name = "checkBoxDAN";
            checkBoxDAN.Size = new Size(51, 19);
            checkBoxDAN.TabIndex = 7;
            checkBoxDAN.Text = "DAN";
            checkBoxDAN.UseVisualStyleBackColor = true;
            // 
            // checkBoxDEU
            // 
            checkBoxDEU.AutoSize = true;
            checkBoxDEU.Location = new Point(92, 137);
            checkBoxDEU.Name = "checkBoxDEU";
            checkBoxDEU.Size = new Size(48, 19);
            checkBoxDEU.TabIndex = 8;
            checkBoxDEU.Text = "DEU";
            checkBoxDEU.UseVisualStyleBackColor = true;
            // 
            // checkBoxENG
            // 
            checkBoxENG.AutoSize = true;
            checkBoxENG.Location = new Point(152, 137);
            checkBoxENG.Name = "checkBoxENG";
            checkBoxENG.Size = new Size(49, 19);
            checkBoxENG.TabIndex = 9;
            checkBoxENG.Text = "ENG";
            checkBoxENG.UseVisualStyleBackColor = true;
            // 
            // checkBoxFRA
            // 
            checkBoxFRA.AutoSize = true;
            checkBoxFRA.Location = new Point(212, 137);
            checkBoxFRA.Name = "checkBoxFRA";
            checkBoxFRA.Size = new Size(47, 19);
            checkBoxFRA.TabIndex = 10;
            checkBoxFRA.Text = "FRA";
            checkBoxFRA.UseVisualStyleBackColor = true;
            // 
            // checkBoxITA
            // 
            checkBoxITA.AutoSize = true;
            checkBoxITA.Location = new Point(272, 137);
            checkBoxITA.Name = "checkBoxITA";
            checkBoxITA.Size = new Size(43, 19);
            checkBoxITA.TabIndex = 11;
            checkBoxITA.Text = "ITA";
            checkBoxITA.UseVisualStyleBackColor = true;
            // 
            // checkBoxJPN
            // 
            checkBoxJPN.AutoSize = true;
            checkBoxJPN.Location = new Point(32, 162);
            checkBoxJPN.Name = "checkBoxJPN";
            checkBoxJPN.Size = new Size(46, 19);
            checkBoxJPN.TabIndex = 12;
            checkBoxJPN.Text = "JPN";
            checkBoxJPN.UseVisualStyleBackColor = true;
            // 
            // checkBoxKOR
            // 
            checkBoxKOR.AutoSize = true;
            checkBoxKOR.Location = new Point(92, 162);
            checkBoxKOR.Name = "checkBoxKOR";
            checkBoxKOR.Size = new Size(49, 19);
            checkBoxKOR.TabIndex = 13;
            checkBoxKOR.Text = "KOR";
            checkBoxKOR.UseVisualStyleBackColor = true;
            // 
            // checkBoxNLD
            // 
            checkBoxNLD.AutoSize = true;
            checkBoxNLD.Location = new Point(152, 162);
            checkBoxNLD.Name = "checkBoxNLD";
            checkBoxNLD.Size = new Size(49, 19);
            checkBoxNLD.TabIndex = 14;
            checkBoxNLD.Text = "NLD";
            checkBoxNLD.UseVisualStyleBackColor = true;
            // 
            // checkBoxNOR
            // 
            checkBoxNOR.AutoSize = true;
            checkBoxNOR.Location = new Point(212, 162);
            checkBoxNOR.Name = "checkBoxNOR";
            checkBoxNOR.Size = new Size(51, 19);
            checkBoxNOR.TabIndex = 15;
            checkBoxNOR.Text = "NOR";
            checkBoxNOR.UseVisualStyleBackColor = true;
            // 
            // checkBoxPOR
            // 
            checkBoxPOR.AutoSize = true;
            checkBoxPOR.Location = new Point(272, 162);
            checkBoxPOR.Name = "checkBoxPOR";
            checkBoxPOR.Size = new Size(49, 19);
            checkBoxPOR.TabIndex = 16;
            checkBoxPOR.Text = "POR";
            checkBoxPOR.UseVisualStyleBackColor = true;
            // 
            // checkBoxRUS
            // 
            checkBoxRUS.AutoSize = true;
            checkBoxRUS.Location = new Point(32, 187);
            checkBoxRUS.Name = "checkBoxRUS";
            checkBoxRUS.Size = new Size(47, 19);
            checkBoxRUS.TabIndex = 17;
            checkBoxRUS.Text = "RUS";
            checkBoxRUS.UseVisualStyleBackColor = true;
            // 
            // checkBoxSPA
            // 
            checkBoxSPA.AutoSize = true;
            checkBoxSPA.Location = new Point(92, 187);
            checkBoxSPA.Name = "checkBoxSPA";
            checkBoxSPA.Size = new Size(47, 19);
            checkBoxSPA.TabIndex = 18;
            checkBoxSPA.Text = "SPA";
            checkBoxSPA.UseVisualStyleBackColor = true;
            // 
            // checkBoxSWE
            // 
            checkBoxSWE.AutoSize = true;
            checkBoxSWE.Location = new Point(152, 187);
            checkBoxSWE.Name = "checkBoxSWE";
            checkBoxSWE.Size = new Size(49, 19);
            checkBoxSWE.TabIndex = 19;
            checkBoxSWE.Text = "SWE";
            checkBoxSWE.UseVisualStyleBackColor = true;
            // 
            // checkBoxZHO
            // 
            checkBoxZHO.AutoSize = true;
            checkBoxZHO.Location = new Point(212, 187);
            checkBoxZHO.Name = "checkBoxZHO";
            checkBoxZHO.Size = new Size(51, 19);
            checkBoxZHO.TabIndex = 20;
            checkBoxZHO.Text = "ZHO";
            checkBoxZHO.UseVisualStyleBackColor = true;
            // 
            // FormSetting
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(344, 321);
            Controls.Add(checkBoxZHO);
            Controls.Add(checkBoxSWE);
            Controls.Add(checkBoxSPA);
            Controls.Add(checkBoxRUS);
            Controls.Add(checkBoxPOR);
            Controls.Add(checkBoxNOR);
            Controls.Add(checkBoxNLD);
            Controls.Add(checkBoxKOR);
            Controls.Add(checkBoxJPN);
            Controls.Add(checkBoxITA);
            Controls.Add(checkBoxFRA);
            Controls.Add(checkBoxENG);
            Controls.Add(checkBoxDEU);
            Controls.Add(checkBoxDAN);
            Controls.Add(buttonLogOut);
            Controls.Add(textBoxNpub);
            Controls.Add(label2);
            Controls.Add(checkBoxMinimizeToTray);
            Controls.Add(checkBoxGetAvatar);
            Controls.Add(linkLabelVersion);
            Controls.Add(checkBoxSendDSSTP);
            Controls.Add(label3);
            Controls.Add(checkBoxShowOnlyFollowees);
            Controls.Add(labelOpacity);
            Controls.Add(checkBoxShowRepostsOnlyFromFollowees);
            Controls.Add(checkBoxShowOnlySelectedLanguage);
            Controls.Add(linkLabelIcons8);
            Controls.Add(label4);
            Controls.Add(checkBoxAddClient);
            Controls.Add(label1);
            Controls.Add(checkBoxTopMost);
            Controls.Add(trackBarOpacity);
            Controls.Add(textBoxNsec);
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(360, 360);
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
        internal TextBox textBoxNsec;
        internal TrackBar trackBarOpacity;
        internal CheckBox checkBoxTopMost;
        private Label label1;
        internal CheckBox checkBoxAddClient;
        private Label label4;
        private LinkLabel linkLabelIcons8;
        internal CheckBox checkBoxShowOnlySelectedLanguage;
        private Label labelOpacity;
        internal CheckBox checkBoxShowOnlyFollowees;
        private Label label3;
        internal CheckBox checkBoxSendDSSTP;
        private LinkLabel linkLabelVersion;
        internal CheckBox checkBoxGetAvatar;
        internal CheckBox checkBoxShowRepostsOnlyFromFollowees;
        internal CheckBox checkBoxMinimizeToTray;
        private Label label2;
        internal TextBox textBoxNpub;
        private Button buttonLogOut;
        private ToolTip toolTipLogOut;
        internal CheckBox checkBoxDAN;
        internal CheckBox checkBoxDEU;
        internal CheckBox checkBoxENG;
        internal CheckBox checkBoxFRA;
        internal CheckBox checkBoxITA;
        internal CheckBox checkBoxJPN;
        internal CheckBox checkBoxKOR;
        internal CheckBox checkBoxNLD;
        internal CheckBox checkBoxNOR;
        internal CheckBox checkBoxPOR;
        internal CheckBox checkBoxRUS;
        internal CheckBox checkBoxSPA;
        internal CheckBox checkBoxSWE;
        internal CheckBox checkBoxZHO;
    }
}