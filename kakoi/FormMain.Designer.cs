namespace kakoi
{
    partial class FormMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            buttonStart = new Button();
            buttonStop = new Button();
            textBoxPost = new TextBox();
            buttonPost = new Button();
            buttonSetting = new Button();
            checkBoxPostBar = new CheckBox();
            buttonRelayList = new Button();
            labelRelays = new Label();
            toolTipRelays = new ToolTip(components);
            comboBoxEmoji = new ComboBox();
            emojiBindingSource = new BindingSource(components);
            dataGridViewNotes = new DataGridView();
            time = new DataGridViewTextBoxColumn();
            name = new DataGridViewTextBoxColumn();
            note = new DataGridViewTextBoxColumn();
            id = new DataGridViewTextBoxColumn();
            pubkey = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)emojiBindingSource).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridViewNotes).BeginInit();
            SuspendLayout();
            // 
            // buttonStart
            // 
            buttonStart.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonStart.Image = Properties.Resources.icons8_start_16;
            buttonStart.Location = new Point(211, 12);
            buttonStart.Name = "buttonStart";
            buttonStart.Size = new Size(23, 23);
            buttonStart.TabIndex = 3;
            buttonStart.UseVisualStyleBackColor = true;
            buttonStart.Click += ButtonStart_Click;
            // 
            // buttonStop
            // 
            buttonStop.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonStop.Enabled = false;
            buttonStop.Image = Properties.Resources.icons8_stop_16;
            buttonStop.Location = new Point(240, 12);
            buttonStop.Name = "buttonStop";
            buttonStop.Size = new Size(23, 23);
            buttonStop.TabIndex = 4;
            buttonStop.UseVisualStyleBackColor = true;
            buttonStop.Click += ButtonStop_Click;
            // 
            // textBoxPost
            // 
            textBoxPost.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBoxPost.BorderStyle = BorderStyle.FixedSingle;
            textBoxPost.Enabled = false;
            textBoxPost.Location = new Point(33, 246);
            textBoxPost.MaxLength = 1024;
            textBoxPost.Name = "textBoxPost";
            textBoxPost.PlaceholderText = "Hello Nostr!";
            textBoxPost.Size = new Size(230, 23);
            textBoxPost.TabIndex = 8;
            textBoxPost.KeyDown += TextBoxPost_KeyDown;
            // 
            // buttonPost
            // 
            buttonPost.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonPost.Enabled = false;
            buttonPost.Image = Properties.Resources.icons8_create_16;
            buttonPost.Location = new Point(269, 246);
            buttonPost.Name = "buttonPost";
            buttonPost.Size = new Size(23, 23);
            buttonPost.TabIndex = 9;
            buttonPost.UseVisualStyleBackColor = true;
            buttonPost.Click += ButtonPost_Click;
            // 
            // buttonSetting
            // 
            buttonSetting.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonSetting.Image = Properties.Resources.icons8_setting_16;
            buttonSetting.Location = new Point(269, 12);
            buttonSetting.Name = "buttonSetting";
            buttonSetting.Size = new Size(23, 23);
            buttonSetting.TabIndex = 5;
            buttonSetting.UseVisualStyleBackColor = true;
            buttonSetting.Click += ButtonSetting_Click;
            // 
            // checkBoxPostBar
            // 
            checkBoxPostBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            checkBoxPostBar.AutoSize = true;
            checkBoxPostBar.Location = new Point(12, 251);
            checkBoxPostBar.Name = "checkBoxPostBar";
            checkBoxPostBar.Size = new Size(15, 14);
            checkBoxPostBar.TabIndex = 7;
            toolTipRelays.SetToolTip(checkBoxPostBar, "Toggle post bar");
            checkBoxPostBar.UseVisualStyleBackColor = true;
            checkBoxPostBar.CheckedChanged += CheckBoxPostBar_CheckedChanged;
            // 
            // buttonRelayList
            // 
            buttonRelayList.Image = Properties.Resources.icons8_list_16;
            buttonRelayList.Location = new Point(12, 12);
            buttonRelayList.Name = "buttonRelayList";
            buttonRelayList.Size = new Size(23, 23);
            buttonRelayList.TabIndex = 1;
            buttonRelayList.UseVisualStyleBackColor = true;
            buttonRelayList.Click += ButtonRelayList_Click;
            // 
            // labelRelays
            // 
            labelRelays.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            labelRelays.AutoEllipsis = true;
            labelRelays.ForeColor = SystemColors.GrayText;
            labelRelays.Location = new Point(41, 16);
            labelRelays.Name = "labelRelays";
            labelRelays.Size = new Size(98, 15);
            labelRelays.TabIndex = 0;
            labelRelays.Text = "Relay info";
            labelRelays.MouseClick += FormMain_MouseClick;
            labelRelays.MouseDoubleClick += FormMain_MouseDoubleClick;
            // 
            // comboBoxEmoji
            // 
            comboBoxEmoji.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            comboBoxEmoji.DataSource = emojiBindingSource;
            comboBoxEmoji.DisplayMember = "Content";
            comboBoxEmoji.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxEmoji.Font = new Font("Segoe UI Emoji", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            comboBoxEmoji.Location = new Point(145, 12);
            comboBoxEmoji.Name = "comboBoxEmoji";
            comboBoxEmoji.Size = new Size(60, 24);
            comboBoxEmoji.TabIndex = 2;
            toolTipRelays.SetToolTip(comboBoxEmoji, "Reaction content");
            comboBoxEmoji.ValueMember = "Content";
            // 
            // dataGridViewNotes
            // 
            dataGridViewNotes.AllowUserToAddRows = false;
            dataGridViewNotes.AllowUserToDeleteRows = false;
            dataGridViewNotes.AllowUserToOrderColumns = true;
            dataGridViewNotes.AllowUserToResizeRows = false;
            dataGridViewNotes.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridViewNotes.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridViewNotes.BackgroundColor = Color.White;
            dataGridViewNotes.BorderStyle = BorderStyle.Fixed3D;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.Font = new Font("Yu Gothic UI", 9F);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dataGridViewNotes.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewNotes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewNotes.ColumnHeadersVisible = false;
            dataGridViewNotes.Columns.AddRange(new DataGridViewColumn[] { time, name, note, id, pubkey });
            dataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = SystemColors.Window;
            dataGridViewCellStyle5.Font = new Font("Yu Gothic UI", 9F);
            dataGridViewCellStyle5.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = Color.HotPink;
            dataGridViewCellStyle5.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = DataGridViewTriState.False;
            dataGridViewNotes.DefaultCellStyle = dataGridViewCellStyle5;
            dataGridViewNotes.GridColor = Color.HotPink;
            dataGridViewNotes.Location = new Point(12, 41);
            dataGridViewNotes.MultiSelect = false;
            dataGridViewNotes.Name = "dataGridViewNotes";
            dataGridViewNotes.ReadOnly = true;
            dataGridViewNotes.RowHeadersVisible = false;
            dataGridViewNotes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewNotes.Size = new Size(280, 199);
            dataGridViewNotes.StandardTab = true;
            dataGridViewNotes.TabIndex = 6;
            dataGridViewNotes.CellDoubleClick += DataGridViewNotes_CellDoubleClick;
            dataGridViewNotes.CellMouseClick += DataGridViewNotes_CellMouseClick;
            dataGridViewNotes.KeyDown += DataGridViewNotes_KeyDown;
            dataGridViewNotes.MouseEnter += Control_MouseEnter;
            dataGridViewNotes.MouseLeave += Control_MouseLeave;
            // 
            // time
            // 
            time.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.TopRight;
            dataGridViewCellStyle2.Format = "t";
            dataGridViewCellStyle2.NullValue = null;
            time.DefaultCellStyle = dataGridViewCellStyle2;
            time.HeaderText = "time";
            time.Name = "time";
            time.ReadOnly = true;
            time.Width = 5;
            // 
            // name
            // 
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.TopLeft;
            name.DefaultCellStyle = dataGridViewCellStyle3;
            name.HeaderText = "name";
            name.Name = "name";
            name.ReadOnly = true;
            name.Width = 60;
            // 
            // note
            // 
            note.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.TopLeft;
            note.DefaultCellStyle = dataGridViewCellStyle4;
            note.HeaderText = "note";
            note.Name = "note";
            note.ReadOnly = true;
            // 
            // id
            // 
            id.HeaderText = "id";
            id.Name = "id";
            id.ReadOnly = true;
            id.Visible = false;
            // 
            // pubkey
            // 
            pubkey.HeaderText = "pubkey";
            pubkey.Name = "pubkey";
            pubkey.ReadOnly = true;
            pubkey.Visible = false;
            // 
            // FormMain
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(304, 281);
            Controls.Add(comboBoxEmoji);
            Controls.Add(dataGridViewNotes);
            Controls.Add(labelRelays);
            Controls.Add(buttonRelayList);
            Controls.Add(checkBoxPostBar);
            Controls.Add(buttonSetting);
            Controls.Add(buttonPost);
            Controls.Add(textBoxPost);
            Controls.Add(buttonStop);
            Controls.Add(buttonStart);
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            MinimumSize = new Size(200, 200);
            Name = "FormMain";
            SizeGripStyle = SizeGripStyle.Show;
            StartPosition = FormStartPosition.Manual;
            Text = "kakoi";
            TopMost = true;
            FormClosing += FormMain_FormClosing;
            Load += FormMain_Load;
            KeyDown += FormMain_KeyDown;
            MouseClick += FormMain_MouseClick;
            MouseDoubleClick += FormMain_MouseDoubleClick;
            ((System.ComponentModel.ISupportInitialize)emojiBindingSource).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridViewNotes).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button buttonStart;
        private Button buttonStop;
        private Button buttonPost;
        private Button buttonSetting;
        internal TextBox textBoxPost;
        internal CheckBox checkBoxPostBar;
        private Button buttonRelayList;
        private Label labelRelays;
        private ToolTip toolTipRelays;
        private DataGridView dataGridViewNotes;
        private DataGridViewTextBoxColumn time;
        private DataGridViewTextBoxColumn name;
        private DataGridViewTextBoxColumn note;
        private DataGridViewTextBoxColumn id;
        private DataGridViewTextBoxColumn pubkey;
        private ComboBox comboBoxEmoji;
        private BindingSource emojiBindingSource;
    }
}
