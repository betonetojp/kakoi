using GenerativeAI.Methods;
using GenerativeAI.Models;
using GenerativeAI.Types;
using System.Diagnostics;

namespace kakoi
{
    public partial class FormAI : Form
    {
        internal FormMain? MainForm { get; set; }
        private const string ApiKeyTarget = "kakoi_ApiKey";
        private GenerativeModel? _model;
        private ChatSession? _chat;
        internal bool IsInitialized = false;

        public FormAI()
        {
            InitializeComponent();
            LoadApiKey();
            LoadAISettings();
            // textBoxModelが空の時はデフォルト値を設定
            if (string.IsNullOrEmpty(textBoxModel.Text))
            {
                textBoxModel.Text = "gemini-1.5-flash";
            }
        }

        private async void ButtonSummarize_Click(object sender, EventArgs e)
        {
            if (!IsInitialized)
            {
                if (MainForm != null)
                {
                    MainForm.LastCreatedAt = DateTimeOffset.MinValue;
                    MainForm.LatestCreatedAt = DateTimeOffset.MinValue;
                }
            }
            await SummarizeNotesAsync();
        }

        private async void ButtonChat_Click(object sender, EventArgs e)
        {
            await SendMessageAsync(textBoxChat.Text);
        }

        private async Task SummarizeNotesAsync()
        {
            textBoxAnswer.Text = string.Empty;

            var apiKey = textBoxApiKey.Text;

            if (MainForm != null)
            {
                var notesContent = MainForm.GetNotesContent();
                InitializeModel(apiKey);

                if (!IsInitialized)
                {
                    _chat = _model?.StartChat(new StartChatParams());
                    IsInitialized = true;
                    checkBoxInitialized.Checked = IsInitialized;
                    notesContent = textBoxPrompt.Text + textBoxPromptForEveryMessage.Text + notesContent;
                }

                if (_chat != null)
                {
                    string? result = null;
                    try
                    {
                        result = await _chat.SendMessageAsync(textBoxPromptForEveryMessage.Text + notesContent);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                    finally
                    {
                        DisplayResult(result);
                    }
                }
            }
        }
        private async Task SendMessageAsync(string message)
        {
            textBoxAnswer.Text = string.Empty;

            var apiKey = textBoxApiKey.Text;
            InitializeModel(apiKey);

            if (!IsInitialized)
            {
                _chat = _model?.StartChat(new StartChatParams());
            }

            if (_chat != null)
            {
                string? result = null;
                try
                {
                    result = await _chat.SendMessageAsync(message);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                finally
                {
                    DisplayResult(result);
                    textBoxChat.Text = string.Empty;
                    textBoxChat.Focus();
                }
            }
        }

        private void InitializeModel(string apiKey)
        {
            _model ??= new GenerativeModel(apiKey, textBoxModel.Text);
            //_model ??= new GenerativeModel(apiKey, "gemini-2.0-flash-exp");
        }

        private void DisplayResult(string? result)
        {
            if (result == null)
            {
                textBoxAnswer.Text = "電波が悪いみたいです。";
                IsInitialized = false;
                checkBoxInitialized.Checked = IsInitialized;
            }
            else
            {
                textBoxAnswer.Text = result.Replace("\n", "\r\n");
            }
        }

        private void TextBoxChat_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // エンターキーを無効化
                ButtonChat_Click(sender, e);
            }
        }

        private static void SaveApiKey(string apiKey)
        {
            Tools.SaveApiKey(ApiKeyTarget, apiKey);
        }

        private void LoadApiKey()
        {
            try
            {
                var apiKey = Tools.LoadApiKey(ApiKeyTarget);
                if (!string.IsNullOrEmpty(apiKey))
                {
                    textBoxApiKey.Text = apiKey;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void SaveAISettings()
        {
            var settings = new AISettings
            {
                NumberOfPosts = (int)numericUpDownNumberOfPosts.Value,
                Model = textBoxModel.Text,
                Prompt = textBoxPrompt.Text,
                PromptForEveryMessage = textBoxPromptForEveryMessage.Text
            };
            Tools.SaveAISettings(settings);
        }

        private void LoadAISettings()
        {
            var settings = Tools.LoadAISettings();
            numericUpDownNumberOfPosts.Value = settings.NumberOfPosts;
            textBoxModel.Text = settings.Model;
            textBoxPrompt.Text = settings.Prompt;
            textBoxPromptForEveryMessage.Text = settings.PromptForEveryMessage;
        }

        private void CheckBoxInitialized_CheckedChanged(object sender, EventArgs e)
        {
            IsInitialized = checkBoxInitialized.Checked;
        }

        private void LinkLabelGetApiKey_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabelGetApiKey.LinkVisited = true;
            var app = new ProcessStartInfo
            {
                FileName = "https://aistudio.google.com/apikey",
                UseShellExecute = true
            };
            Process.Start(app);
        }

        private void FormAI_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
            }
            SaveApiKey(textBoxApiKey.Text);
            SaveAISettings();
            Hide();
        }

        private void FormAI_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                Close();
            }
        }
    }
}
