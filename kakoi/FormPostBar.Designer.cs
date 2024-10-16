namespace kakoi
{
    partial class FormPostBar
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPostBar));
            textBoxPost = new TextBox();
            buttonPost = new Button();
            buttonPicture = new Button();
            SuspendLayout();
            // 
            // textBoxPost
            // 
            textBoxPost.AcceptsReturn = true;
            textBoxPost.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBoxPost.BorderStyle = BorderStyle.FixedSingle;
            textBoxPost.Enabled = false;
            textBoxPost.Location = new Point(12, 12);
            textBoxPost.MaxLength = 1024;
            textBoxPost.Multiline = true;
            textBoxPost.Name = "textBoxPost";
            textBoxPost.PlaceholderText = "kakoi";
            textBoxPost.ScrollBars = ScrollBars.Vertical;
            textBoxPost.Size = new Size(251, 52);
            textBoxPost.TabIndex = 1;
            textBoxPost.KeyDown += TextBoxPost_KeyDown;
            textBoxPost.MouseEnter += Control_MouseEnter;
            textBoxPost.MouseLeave += Control_MouseLeave;
            // 
            // buttonPost
            // 
            buttonPost.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonPost.Enabled = false;
            buttonPost.Image = Properties.Resources.icons8_create_16;
            buttonPost.Location = new Point(269, 12);
            buttonPost.Name = "buttonPost";
            buttonPost.Size = new Size(23, 23);
            buttonPost.TabIndex = 2;
            buttonPost.UseVisualStyleBackColor = true;
            buttonPost.Click += ButtonPost_Click;
            // 
            // buttonPicture
            // 
            buttonPicture.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonPicture.Image = Properties.Resources.icons8_picture_16;
            buttonPicture.Location = new Point(269, 41);
            buttonPicture.Name = "buttonPicture";
            buttonPicture.Size = new Size(23, 23);
            buttonPicture.TabIndex = 3;
            buttonPicture.UseVisualStyleBackColor = true;
            buttonPicture.Click += ButtonPicture_Click;
            // 
            // FormPostBar
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(304, 76);
            ControlBox = false;
            Controls.Add(buttonPicture);
            Controls.Add(buttonPost);
            Controls.Add(textBoxPost);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            MaximizeBox = false;
            MaximumSize = new Size(480, 240);
            MinimizeBox = false;
            MinimumSize = new Size(200, 92);
            Name = "FormPostBar";
            ShowIcon = false;
            ShowInTaskbar = false;
            SizeGripStyle = SizeGripStyle.Show;
            StartPosition = FormStartPosition.Manual;
            TopMost = true;
            Activated += FormPostBar_Activated;
            FormClosing += FormPostBar_FormClosing;
            Shown += FormPostBar_Shown;
            DoubleClick += FormPostBar_DoubleClick;
            KeyDown += FormPostBar_KeyDown;
            MouseClick += FormPostBar_MouseClick;
            MouseDown += FormPostBar_MouseDown;
            MouseMove += FormPostBar_MouseMove;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        internal Button buttonPost;
        internal TextBox textBoxPost;
        private Button buttonPicture;
    }
}