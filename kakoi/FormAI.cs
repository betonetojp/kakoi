using GenerativeAI;
using GenerativeAI.Types;
using System.Diagnostics;

namespace kakoi
{
    public partial class FormAI : Form
    {
        internal FormMain? MainForm { get; set; }
        private const string ApiKeyTarget = "kakoi_ApiKey";
        private GenerativeModel? _model;
        private string _currentModelName = string.Empty;
        private ChatSession? _chat;
        internal bool IsInitialized = false;
        private ChatSessionBackUpData? _chatSessionBackUpData;

        public FormAI()
        {
            InitializeComponent();
            LoadApiKey();
            LoadAISettings();
            // textBoxModelが空の時はデフォルト値を設定
            if (string.IsNullOrEmpty(textBoxModel.Text))
            {
                textBoxModel.Text = "gemini-2.0-flash";
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
                if (!IsInitialized)
                {
                    _model = null;
                    _currentModelName = string.Empty;
                    _chatSessionBackUpData = null;
                }
                InitializeModel(apiKey);
                if (_model == null)
                {
                    return;
                }

                var notesContent = MainForm.GetNotesContent();
                if (!IsInitialized)
                {
                    _chat = _model.StartChat();
                    IsInitialized = true;
                    checkBoxInitialized.Invoke((MethodInvoker)(() => checkBoxInitialized.Checked = IsInitialized));
                    notesContent = textBoxPrompt.Text + textBoxPromptForEveryMessage.Text + notesContent;
                }
                else if (_chat == null)
                {
                    if (_chatSessionBackUpData != null)
                    {
                        _chat = _model.StartChat(_chatSessionBackUpData);
                    }
                    else
                    {
                        _chat = _model.StartChat();
                    }
                }

                if (_chat != null)
                {
                    var result = new GenerateContentResponse();
                    try
                    {
                        result = await _chat.GenerateContentAsync(textBoxPromptForEveryMessage.Text + notesContent);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                    finally
                    {
                        DisplayResult(result.Text());
                    }
                }
            }
        }
        private async Task SendMessageAsync(string message)
        {
            textBoxAnswer.Text = string.Empty;

            var apiKey = textBoxApiKey.Text;
            InitializeModel(apiKey);
            if (_model == null)
            {
                return;
            }

            if (!IsInitialized)
            {
                _chat = _model.StartChat();
                IsInitialized = true;
                checkBoxInitialized.Invoke((MethodInvoker)(() => checkBoxInitialized.Checked = IsInitialized));
            }
            else if (_chat == null)
            {
                if (_chatSessionBackUpData != null)
                {
                    _chat = _model.StartChat(_chatSessionBackUpData);
                }
                else
                {
                    _chat = _model.StartChat();
                }
            }

            if (_chat != null)
            {
                var result = new GenerateContentResponse();
                try
                {
                    result = await _chat.GenerateContentAsync(message);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                finally
                {
                    DisplayResult(result.Text());
                    textBoxChat.Invoke((MethodInvoker)(() =>
                    {
                        textBoxChat.Text = string.Empty;
                        textBoxChat.Focus();
                    }));
                }
            }
        }

        private void InitializeModel(string apiKey)
        {
            try
            {
                var modelName = textBoxModel.Invoke(() => textBoxModel.Text.Trim());
                if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(modelName))
                {
                    _model = null;
                    _currentModelName = string.Empty;
                    return;
                }

                if (_model == null || _currentModelName != modelName)
                {
                    // 既存のセッションがあれば会話履歴をバックアップして新モデルに引き継ぐ
                    if (_chat != null)
                    {
                        try
                        {
                            _chatSessionBackUpData = _chat.CreateChatSessionBackUpData();
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"チャット履歴バックアップ失敗: {ex.Message}");
                        }
                    }

                    _model = new GenerativeModel(apiKey, modelName);
                    _currentModelName = modelName;

                    // 会話履歴があれば新モデルでセッションを復元・継続
                    if (_chatSessionBackUpData?.History != null && _chatSessionBackUpData.History.Count > 0)
                    {
                        try
                        {
                            _chat = _model.StartChat(_chatSessionBackUpData);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"新モデルでのセッション復元失敗: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                _model = null;
                _currentModelName = string.Empty;
                if (MainForm != null)
                {
                    MainForm.LastCreatedAt = DateTimeOffset.MinValue;
                    MainForm.LatestCreatedAt = DateTimeOffset.MinValue;
                }
            }
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
