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
            DataGridViewCellStyle dataGridViewCellStyle6 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
            buttonStart = new Button();
            buttonStop = new Button();
            buttonSetting = new Button();
            checkBoxPostBar = new CheckBox();
            buttonRelayList = new Button();
            labelRelays = new Label();
            toolTipRelays = new ToolTip(components);
            comboBoxEmoji = new ComboBox();
            emojiBindingSource = new BindingSource(components);
            dataGridViewNotes = new DataGridView();
            time = new DataGridViewTextBoxColumn();
            avatar = new DataGridViewImageColumn();
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
            buttonStart.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonStart.Image = Properties.Resources.icons8_start_16;
            buttonStart.Location = new Point(211, 246);
            buttonStart.Name = "buttonStart";
            buttonStart.Size = new Size(23, 23);
            buttonStart.TabIndex = 5;
            buttonStart.UseVisualStyleBackColor = true;
            buttonStart.Click += ButtonStart_Click;
            // 
            // buttonStop
            // 
            buttonStop.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonStop.Enabled = false;
            buttonStop.Image = Properties.Resources.icons8_stop_16;
            buttonStop.Location = new Point(240, 246);
            buttonStop.Name = "buttonStop";
            buttonStop.Size = new Size(23, 23);
            buttonStop.TabIndex = 6;
            buttonStop.UseVisualStyleBackColor = true;
            buttonStop.Click += ButtonStop_Click;
            // 
            // buttonSetting
            // 
            buttonSetting.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonSetting.Image = Properties.Resources.icons8_setting_16;
            buttonSetting.Location = new Point(269, 246);
            buttonSetting.Name = "buttonSetting";
            buttonSetting.Size = new Size(23, 23);
            buttonSetting.TabIndex = 7;
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
            checkBoxPostBar.TabIndex = 2;
            toolTipRelays.SetToolTip(checkBoxPostBar, "Toggle post bar");
            checkBoxPostBar.UseVisualStyleBackColor = true;
            checkBoxPostBar.CheckedChanged += CheckBoxPostBar_CheckedChanged;
            // 
            // buttonRelayList
            // 
            buttonRelayList.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonRelayList.Image = Properties.Resources.icons8_list_16;
            buttonRelayList.Location = new Point(182, 246);
            buttonRelayList.Name = "buttonRelayList";
            buttonRelayList.Size = new Size(23, 23);
            buttonRelayList.TabIndex = 4;
            buttonRelayList.UseVisualStyleBackColor = true;
            buttonRelayList.Click += ButtonRelayList_Click;
            // 
            // labelRelays
            // 
            labelRelays.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            labelRelays.AutoEllipsis = true;
            labelRelays.ForeColor = SystemColors.GrayText;
            labelRelays.Location = new Point(87, 251);
            labelRelays.Name = "labelRelays";
            labelRelays.Size = new Size(89, 15);
            labelRelays.TabIndex = 0;
            labelRelays.Text = "Relay info";
            labelRelays.TextAlign = ContentAlignment.TopRight;
            labelRelays.MouseClick += FormMain_MouseClick;
            labelRelays.MouseDoubleClick += FormMain_MouseDoubleClick;
            // 
            // comboBoxEmoji
            // 
            comboBoxEmoji.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            comboBoxEmoji.DataSource = emojiBindingSource;
            comboBoxEmoji.DisplayMember = "Content";
            comboBoxEmoji.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxEmoji.Font = new Font("Segoe UI Emoji", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            comboBoxEmoji.Location = new Point(33, 246);
            comboBoxEmoji.Name = "comboBoxEmoji";
            comboBoxEmoji.Size = new Size(48, 24);
            comboBoxEmoji.TabIndex = 3;
            toolTipRelays.SetToolTip(comboBoxEmoji, "Reaction content");
            comboBoxEmoji.ValueMember = "Content";
            comboBoxEmoji.SelectionChangeCommitted += comboBoxEmoji_SelectionChangeCommitted;
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
            dataGridViewNotes.Columns.AddRange(new DataGridViewColumn[] { time, avatar, name, note, id, pubkey });
            dataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = SystemColors.Window;
            dataGridViewCellStyle6.Font = new Font("Yu Gothic UI", 9F);
            dataGridViewCellStyle6.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle6.SelectionBackColor = Color.DeepPink;
            dataGridViewCellStyle6.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = DataGridViewTriState.False;
            dataGridViewNotes.DefaultCellStyle = dataGridViewCellStyle6;
            dataGridViewNotes.GridColor = Color.DeepPink;
            dataGridViewNotes.Location = new Point(12, 12);
            dataGridViewNotes.MultiSelect = false;
            dataGridViewNotes.Name = "dataGridViewNotes";
            dataGridViewNotes.ReadOnly = true;
            dataGridViewNotes.RowHeadersVisible = false;
            dataGridViewNotes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewNotes.Size = new Size(280, 228);
            dataGridViewNotes.StandardTab = true;
            dataGridViewNotes.TabIndex = 1;
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
            time.SortMode = DataGridViewColumnSortMode.NotSortable;
            time.Width = 5;
            // 
            // avatar
            // 
            avatar.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.TopCenter;
            dataGridViewCellStyle3.NullValue = resources.GetObject("dataGridViewCellStyle3.NullValue");
            dataGridViewCellStyle3.Padding = new Padding(2);
            avatar.DefaultCellStyle = dataGridViewCellStyle3;
            avatar.HeaderText = "avatar";
            avatar.Name = "avatar";
            avatar.ReadOnly = true;
            avatar.Width = 5;
            // 
            // name
            // 
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.TopLeft;
            name.DefaultCellStyle = dataGridViewCellStyle4;
            name.HeaderText = "name";
            name.Name = "name";
            name.ReadOnly = true;
            name.SortMode = DataGridViewColumnSortMode.NotSortable;
            name.Width = 60;
            // 
            // note
            // 
            note.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.TopLeft;
            note.DefaultCellStyle = dataGridViewCellStyle5;
            note.HeaderText = "note";
            note.Name = "note";
            note.ReadOnly = true;
            note.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // id
            // 
            id.HeaderText = "id";
            id.Name = "id";
            id.ReadOnly = true;
            id.SortMode = DataGridViewColumnSortMode.NotSortable;
            id.Visible = false;
            // 
            // pubkey
            // 
            pubkey.HeaderText = "pubkey";
            pubkey.Name = "pubkey";
            pubkey.ReadOnly = true;
            pubkey.SortMode = DataGridViewColumnSortMode.NotSortable;
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
            Controls.Add(buttonStop);
            Controls.Add(buttonStart);
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            MinimumSize = new Size(240, 240);
            Name = "FormMain";
            SizeGripStyle = SizeGripStyle.Show;
            StartPosition = FormStartPosition.Manual;
            Text = "kakoi";
            TopMost = true;
            FormClosing += FormMain_FormClosing;
            Load += FormMain_Load;
            Shown += FormMain_Shown;
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
        private Button buttonSetting;
        internal CheckBox checkBoxPostBar;
        private Button buttonRelayList;
        private Label labelRelays;
        private ToolTip toolTipRelays;
        private DataGridView dataGridViewNotes;
        private ComboBox comboBoxEmoji;
        private BindingSource emojiBindingSource;
        private DataGridViewTextBoxColumn time;
        private DataGridViewImageColumn avatar;
        private DataGridViewTextBoxColumn name;
        private DataGridViewTextBoxColumn note;
        private DataGridViewTextBoxColumn id;
        private DataGridViewTextBoxColumn pubkey;
    }
}
