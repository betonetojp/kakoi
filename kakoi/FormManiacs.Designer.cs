﻿namespace kakoi
{
    partial class FormManiacs
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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormManiacs));
            dataGridViewUsers = new DataGridView();
            mute = new DataGridViewCheckBoxColumn();
            last_activity = new DataGridViewTextBoxColumn();
            petname = new DataGridViewTextBoxColumn();
            display_name = new DataGridViewTextBoxColumn();
            name = new DataGridViewTextBoxColumn();
            pubkey = new DataGridViewTextBoxColumn();
            nip05 = new DataGridViewTextBoxColumn();
            picture = new DataGridViewTextBoxColumn();
            created_at = new DataGridViewTextBoxColumn();
            buttonSave = new Button();
            checkBoxBalloon = new CheckBox();
            checkBoxOpenFile = new CheckBox();
            labelFileName = new Label();
            textBoxFileName = new TextBox();
            textBoxKeywords = new TextBox();
            labelKeywords = new Label();
            buttonDelete = new Button();
            buttonReload = new Button();
            checkBoxMuteMostr = new CheckBox();
            checkBoxReaction = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)dataGridViewUsers).BeginInit();
            SuspendLayout();
            // 
            // dataGridViewUsers
            // 
            dataGridViewUsers.AllowUserToAddRows = false;
            dataGridViewUsers.AllowUserToDeleteRows = false;
            dataGridViewUsers.AllowUserToResizeRows = false;
            dataGridViewUsers.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridViewUsers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewUsers.Columns.AddRange(new DataGridViewColumn[] { mute, last_activity, petname, display_name, name, pubkey, nip05, picture, created_at });
            dataGridViewUsers.Location = new Point(12, 12);
            dataGridViewUsers.Name = "dataGridViewUsers";
            dataGridViewUsers.RowHeadersVisible = false;
            dataGridViewUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewUsers.ShowCellToolTips = false;
            dataGridViewUsers.Size = new Size(440, 291);
            dataGridViewUsers.StandardTab = true;
            dataGridViewUsers.TabIndex = 1;
            // 
            // mute
            // 
            mute.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            mute.HeaderText = "Mute";
            mute.MinimumWidth = 20;
            mute.Name = "mute";
            mute.SortMode = DataGridViewColumnSortMode.Automatic;
            mute.Width = 60;
            // 
            // last_activity
            // 
            last_activity.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            last_activity.HeaderText = "Last activity";
            last_activity.MinimumWidth = 20;
            last_activity.Name = "last_activity";
            last_activity.ReadOnly = true;
            last_activity.Width = 94;
            // 
            // petname
            // 
            petname.HeaderText = "petname";
            petname.MinimumWidth = 20;
            petname.Name = "petname";
            // 
            // display_name
            // 
            display_name.HeaderText = "display_name";
            display_name.MinimumWidth = 20;
            display_name.Name = "display_name";
            display_name.ReadOnly = true;
            // 
            // name
            // 
            name.HeaderText = "name";
            name.MinimumWidth = 20;
            name.Name = "name";
            name.ReadOnly = true;
            // 
            // pubkey
            // 
            pubkey.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            pubkey.HeaderText = "pubkey";
            pubkey.MinimumWidth = 20;
            pubkey.Name = "pubkey";
            pubkey.ReadOnly = true;
            pubkey.Width = 71;
            // 
            // nip05
            // 
            nip05.HeaderText = "nip05";
            nip05.MinimumWidth = 20;
            nip05.Name = "nip05";
            nip05.ReadOnly = true;
            nip05.Width = 110;
            // 
            // picture
            // 
            picture.HeaderText = "picture";
            picture.MinimumWidth = 20;
            picture.Name = "picture";
            picture.Width = 110;
            // 
            // created_at
            // 
            created_at.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle1.NullValue = null;
            created_at.DefaultCellStyle = dataGridViewCellStyle1;
            created_at.HeaderText = "created_at";
            created_at.MinimumWidth = 20;
            created_at.Name = "created_at";
            created_at.ReadOnly = true;
            created_at.Width = 86;
            // 
            // buttonSave
            // 
            buttonSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonSave.Location = new Point(377, 406);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new Size(75, 23);
            buttonSave.TabIndex = 10;
            buttonSave.Text = "Save";
            buttonSave.UseVisualStyleBackColor = true;
            buttonSave.Click += ButtonSave_Click;
            // 
            // checkBoxBalloon
            // 
            checkBoxBalloon.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            checkBoxBalloon.AutoSize = true;
            checkBoxBalloon.Location = new Point(170, 356);
            checkBoxBalloon.Name = "checkBoxBalloon";
            checkBoxBalloon.Size = new Size(129, 19);
            checkBoxBalloon.TabIndex = 6;
            checkBoxBalloon.Text = "Balloon notification";
            checkBoxBalloon.UseVisualStyleBackColor = true;
            // 
            // checkBoxOpenFile
            // 
            checkBoxOpenFile.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            checkBoxOpenFile.AutoSize = true;
            checkBoxOpenFile.Location = new Point(170, 381);
            checkBoxOpenFile.Name = "checkBoxOpenFile";
            checkBoxOpenFile.Size = new Size(142, 19);
            checkBoxOpenFile.TabIndex = 8;
            checkBoxOpenFile.Text = "Open URL notification";
            checkBoxOpenFile.UseVisualStyleBackColor = true;
            // 
            // labelFileName
            // 
            labelFileName.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            labelFileName.AutoSize = true;
            labelFileName.Location = new Point(170, 410);
            labelFileName.Name = "labelFileName";
            labelFileName.Size = new Size(28, 15);
            labelFileName.TabIndex = 0;
            labelFileName.Text = "URL";
            // 
            // textBoxFileName
            // 
            textBoxFileName.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            textBoxFileName.BorderStyle = BorderStyle.FixedSingle;
            textBoxFileName.Location = new Point(204, 406);
            textBoxFileName.Name = "textBoxFileName";
            textBoxFileName.Size = new Size(155, 23);
            textBoxFileName.TabIndex = 9;
            textBoxFileName.Text = "https://lumilumi.vercel.app/";
            // 
            // textBoxKeywords
            // 
            textBoxKeywords.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            textBoxKeywords.BorderStyle = BorderStyle.FixedSingle;
            textBoxKeywords.Location = new Point(12, 356);
            textBoxKeywords.Multiline = true;
            textBoxKeywords.Name = "textBoxKeywords";
            textBoxKeywords.ScrollBars = ScrollBars.Vertical;
            textBoxKeywords.Size = new Size(152, 73);
            textBoxKeywords.TabIndex = 5;
            // 
            // labelKeywords
            // 
            labelKeywords.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            labelKeywords.AutoSize = true;
            labelKeywords.Location = new Point(12, 338);
            labelKeywords.Name = "labelKeywords";
            labelKeywords.Size = new Size(108, 15);
            labelKeywords.TabIndex = 0;
            labelKeywords.Text = "Keywords (per line)";
            // 
            // buttonDelete
            // 
            buttonDelete.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            buttonDelete.Location = new Point(12, 309);
            buttonDelete.Name = "buttonDelete";
            buttonDelete.Size = new Size(75, 23);
            buttonDelete.TabIndex = 2;
            buttonDelete.Text = "Delete";
            buttonDelete.UseVisualStyleBackColor = true;
            buttonDelete.Click += ButtonDelete_Click;
            // 
            // buttonReload
            // 
            buttonReload.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonReload.Location = new Point(377, 309);
            buttonReload.Name = "buttonReload";
            buttonReload.Size = new Size(75, 23);
            buttonReload.TabIndex = 4;
            buttonReload.Text = "Reload";
            buttonReload.UseVisualStyleBackColor = true;
            buttonReload.Click += ButtonReload_Click;
            // 
            // checkBoxMuteMostr
            // 
            checkBoxMuteMostr.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            checkBoxMuteMostr.AutoSize = true;
            checkBoxMuteMostr.Location = new Point(170, 313);
            checkBoxMuteMostr.Name = "checkBoxMuteMostr";
            checkBoxMuteMostr.Size = new Size(147, 19);
            checkBoxMuteMostr.TabIndex = 3;
            checkBoxMuteMostr.Text = "Mute posts from Mostr";
            checkBoxMuteMostr.UseVisualStyleBackColor = true;
            // 
            // checkBoxReaction
            // 
            checkBoxReaction.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            checkBoxReaction.AutoSize = true;
            checkBoxReaction.Location = new Point(305, 356);
            checkBoxReaction.Name = "checkBoxReaction";
            checkBoxReaction.Size = new Size(72, 19);
            checkBoxReaction.TabIndex = 7;
            checkBoxReaction.Text = "Reaction";
            checkBoxReaction.UseVisualStyleBackColor = true;
            // 
            // FormManiacs
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(464, 441);
            Controls.Add(checkBoxReaction);
            Controls.Add(checkBoxMuteMostr);
            Controls.Add(buttonReload);
            Controls.Add(buttonDelete);
            Controls.Add(labelKeywords);
            Controls.Add(textBoxKeywords);
            Controls.Add(textBoxFileName);
            Controls.Add(labelFileName);
            Controls.Add(checkBoxOpenFile);
            Controls.Add(checkBoxBalloon);
            Controls.Add(buttonSave);
            Controls.Add(dataGridViewUsers);
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            MinimumSize = new Size(480, 480);
            Name = "FormManiacs";
            SizeGripStyle = SizeGripStyle.Show;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Mute and keyword notification";
            FormClosing += FormManiacs_FormClosing;
            Load += FormManiacs_Load;
            KeyDown += FormManiacs_KeyDown;
            ((System.ComponentModel.ISupportInitialize)dataGridViewUsers).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dataGridViewUsers;
        private Button buttonSave;
        private CheckBox checkBoxBalloon;
        private CheckBox checkBoxOpenFile;
        private Label labelFileName;
        private TextBox textBoxFileName;
        private TextBox textBoxKeywords;
        private Label labelKeywords;
        private Button buttonDelete;
        private Button buttonReload;
        private CheckBox checkBoxMuteMostr;
        private CheckBox checkBoxReaction;
        private DataGridViewCheckBoxColumn mute;
        private DataGridViewTextBoxColumn last_activity;
        private DataGridViewTextBoxColumn petname;
        private DataGridViewTextBoxColumn display_name;
        private DataGridViewTextBoxColumn name;
        private DataGridViewTextBoxColumn pubkey;
        private DataGridViewTextBoxColumn nip05;
        private DataGridViewTextBoxColumn picture;
        private DataGridViewTextBoxColumn created_at;
    }
}