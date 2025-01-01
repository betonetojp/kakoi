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
        private bool _isInitialized = false;

        public FormAI()
        {
            InitializeComponent();
            LoadApiKey();
            LoadAISettings();
        }

        private async void ButtonSummarize_Click(object sender, EventArgs e)
        {
            if (!_isInitialized)
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

                if (!_isInitialized)
                {
                    _chat = _model?.StartChat(new StartChatParams());
                    _isInitialized = true;
                    checkBoxInitialized.Checked = _isInitialized;
                    //notesContent = _prompt + _promptForEveryMessage + notesContent;
                    notesContent = textBoxPrompt.Text + textBoxPromptForEveryMessage.Text + notesContent;
                }

                if (_chat != null)
                {
                    string? result = null;
                    try
                    {
                        //result = await _chat.SendMessageAsync(_promptForEveryMessage + notesContent);
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

            if (!_isInitialized)
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
            _model ??= new GenerativeModel(apiKey, "gemini-1.5-flash");
            //_model ??= new GenerativeModel(apiKey, "gemini-2.0-flash-exp");
        }

        private void DisplayResult(string? result)
        {
            if (result == null)
            {
                textBoxAnswer.Text = "電波が悪いみたいです。";
                _isInitialized = false;
                checkBoxInitialized.Checked = _isInitialized;
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
                Prompt = textBoxPrompt.Text,
                PromptForEveryMessage = textBoxPromptForEveryMessage.Text
            };
            Tools.SaveAISettings(settings);
        }

        private void LoadAISettings()
        {
            var settings = Tools.LoadAISettings();
            numericUpDownNumberOfPosts.Value = settings.NumberOfPosts;
            textBoxPrompt.Text = settings.Prompt;
            textBoxPromptForEveryMessage.Text = settings.PromptForEveryMessage;
        }

        private void CheckBoxInitialized_CheckedChanged(object sender, EventArgs e)
        {
            _isInitialized = checkBoxInitialized.Checked;
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
            SaveApiKey(textBoxApiKey.Text);
            SaveAISettings();
        }
    }
}
