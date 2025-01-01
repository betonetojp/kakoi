namespace kakoi
{
    partial class FormAI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAI));
            label1 = new Label();
            textBoxApiKey = new TextBox();
            textBoxAnswer = new TextBox();
            buttonSummarize = new Button();
            textBoxChat = new TextBox();
            buttonChat = new Button();
            checkBoxInitialized = new CheckBox();
            numericUpDownNumberOfPosts = new NumericUpDown();
            label2 = new Label();
            textBoxPrompt = new TextBox();
            textBoxPromptForEveryMessage = new TextBox();
            label3 = new Label();
            label4 = new Label();
            linkLabelGetApiKey = new LinkLabel();
            ((System.ComponentModel.ISupportInitialize)numericUpDownNumberOfPosts).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 16);
            label1.Name = "label1";
            label1.Size = new Size(87, 15);
            label1.TabIndex = 0;
            label1.Text = "Gemini API Key";
            // 
            // textBoxApiKey
            // 
            textBoxApiKey.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxApiKey.BorderStyle = BorderStyle.FixedSingle;
            textBoxApiKey.Location = new Point(12, 34);
            textBoxApiKey.Name = "textBoxApiKey";
            textBoxApiKey.PasswordChar = '*';
            textBoxApiKey.Size = new Size(370, 23);
            textBoxApiKey.TabIndex = 5;
            // 
            // textBoxAnswer
            // 
            textBoxAnswer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            textBoxAnswer.BorderStyle = BorderStyle.FixedSingle;
            textBoxAnswer.Location = new Point(402, 41);
            textBoxAnswer.Multiline = true;
            textBoxAnswer.Name = "textBoxAnswer";
            textBoxAnswer.ScrollBars = ScrollBars.Vertical;
            textBoxAnswer.Size = new Size(370, 284);
            textBoxAnswer.TabIndex = 8;
            // 
            // buttonSummarize
            // 
            buttonSummarize.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonSummarize.Location = new Point(697, 12);
            buttonSummarize.Name = "buttonSummarize";
            buttonSummarize.Size = new Size(75, 23);
            buttonSummarize.TabIndex = 4;
            buttonSummarize.Text = "Summarize";
            buttonSummarize.UseVisualStyleBackColor = true;
            buttonSummarize.Click += ButtonSummarize_Click;
            // 
            // textBoxChat
            // 
            textBoxChat.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            textBoxChat.BorderStyle = BorderStyle.FixedSingle;
            textBoxChat.Location = new Point(402, 331);
            textBoxChat.Name = "textBoxChat";
            textBoxChat.Size = new Size(289, 23);
            textBoxChat.TabIndex = 9;
            textBoxChat.KeyDown += TextBoxChat_KeyDown;
            // 
            // buttonChat
            // 
            buttonChat.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonChat.Location = new Point(697, 331);
            buttonChat.Name = "buttonChat";
            buttonChat.Size = new Size(75, 23);
            buttonChat.TabIndex = 10;
            buttonChat.Text = "Chat";
            buttonChat.UseVisualStyleBackColor = true;
            buttonChat.Click += ButtonChat_Click;
            // 
            // checkBoxInitialized
            // 
            checkBoxInitialized.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            checkBoxInitialized.AutoSize = true;
            checkBoxInitialized.Location = new Point(612, 15);
            checkBoxInitialized.Name = "checkBoxInitialized";
            checkBoxInitialized.Size = new Size(79, 19);
            checkBoxInitialized.TabIndex = 3;
            checkBoxInitialized.Text = "Initialazed";
            checkBoxInitialized.UseVisualStyleBackColor = true;
            checkBoxInitialized.CheckedChanged += CheckBoxInitialized_CheckedChanged;
            // 
            // numericUpDownNumberOfPosts
            // 
            numericUpDownNumberOfPosts.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            numericUpDownNumberOfPosts.Location = new Point(523, 12);
            numericUpDownNumberOfPosts.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            numericUpDownNumberOfPosts.Name = "numericUpDownNumberOfPosts";
            numericUpDownNumberOfPosts.Size = new Size(46, 23);
            numericUpDownNumberOfPosts.TabIndex = 2;
            numericUpDownNumberOfPosts.TextAlign = HorizontalAlignment.Right;
            numericUpDownNumberOfPosts.Value = new decimal(new int[] { 1000, 0, 0, 0 });
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Location = new Point(402, 16);
            label2.Name = "label2";
            label2.Size = new Size(115, 15);
            label2.TabIndex = 0;
            label2.Text = "Max of posts to read";
            // 
            // textBoxPrompt
            // 
            textBoxPrompt.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBoxPrompt.BorderStyle = BorderStyle.FixedSingle;
            textBoxPrompt.Location = new Point(12, 85);
            textBoxPrompt.Multiline = true;
            textBoxPrompt.Name = "textBoxPrompt";
            textBoxPrompt.ScrollBars = ScrollBars.Vertical;
            textBoxPrompt.Size = new Size(370, 163);
            textBoxPrompt.TabIndex = 6;
            textBoxPrompt.Text = resources.GetString("textBoxPrompt.Text");
            // 
            // textBoxPromptForEveryMessage
            // 
            textBoxPromptForEveryMessage.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBoxPromptForEveryMessage.BorderStyle = BorderStyle.FixedSingle;
            textBoxPromptForEveryMessage.Location = new Point(12, 269);
            textBoxPromptForEveryMessage.Multiline = true;
            textBoxPromptForEveryMessage.Name = "textBoxPromptForEveryMessage";
            textBoxPromptForEveryMessage.ScrollBars = ScrollBars.Vertical;
            textBoxPromptForEveryMessage.Size = new Size(370, 85);
            textBoxPromptForEveryMessage.TabIndex = 7;
            textBoxPromptForEveryMessage.Text = "全体で140文字以内にしてください。\r\n【タイムライン】がない場合は新着投稿がない旨を伝えてください。\r\n以下、【タイムライン】\r\n\r\n";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 67);
            label3.Name = "label3";
            label3.Size = new Size(78, 15);
            label3.TabIndex = 0;
            label3.Text = "Initial prompt";
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label4.AutoSize = true;
            label4.Location = new Point(12, 251);
            label4.Name = "label4";
            label4.Size = new Size(165, 15);
            label4.TabIndex = 0;
            label4.Text = "Prompt for every summarizing";
            // 
            // linkLabelGetApiKey
            // 
            linkLabelGetApiKey.AutoSize = true;
            linkLabelGetApiKey.Location = new Point(105, 16);
            linkLabelGetApiKey.Name = "linkLabelGetApiKey";
            linkLabelGetApiKey.Size = new Size(68, 15);
            linkLabelGetApiKey.TabIndex = 1;
            linkLabelGetApiKey.TabStop = true;
            linkLabelGetApiKey.Text = "Get API Key";
            linkLabelGetApiKey.LinkClicked += LinkLabelGetApiKey_LinkClicked;
            // 
            // FormAI
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 361);
            Controls.Add(linkLabelGetApiKey);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(textBoxPromptForEveryMessage);
            Controls.Add(textBoxPrompt);
            Controls.Add(label2);
            Controls.Add(numericUpDownNumberOfPosts);
            Controls.Add(checkBoxInitialized);
            Controls.Add(buttonChat);
            Controls.Add(textBoxChat);
            Controls.Add(buttonSummarize);
            Controls.Add(textBoxAnswer);
            Controls.Add(textBoxApiKey);
            Controls.Add(label1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(800, 400);
            Name = "FormAI";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Gemini Test";
            FormClosing += FormAI_FormClosing;
            ((System.ComponentModel.ISupportInitialize)numericUpDownNumberOfPosts).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox textBoxApiKey;
        private TextBox textBoxAnswer;
        private Button buttonSummarize;
        private TextBox textBoxChat;
        private Button buttonChat;
        private CheckBox checkBoxInitialized;
        private NumericUpDown numericUpDown1;
        private Label label2;
        private TextBox textBoxPrompt;
        private TextBox textBoxPromptForEveryMessage;
        private Label label3;
        private Label label4;
        internal NumericUpDown numericUpDownNumberOfPosts;
        private LinkLabel linkLabelGetApiKey;
    }
}