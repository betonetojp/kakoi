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

        private readonly string _prompt =
            "口調は「みたいですよ」「ですね」みたいな感じで発言してください。\r\n" +
            "マークダウン記法は使わないでください。\r\n" +
            "HTMLは使わないでください。\r\n" +
            "！記号はなるべく使わないでください。\r\n" +
            "ツイッターではないので、ツイートではなく投稿と表現してください。\r\n" +
            "まず、「みなさんこんなことを」「あくまでもうわさですけど」「今の話題は」のどれかに続けて" +
            "【タイムライン】の要約を5件以内で箇条書きで紹介してください。\r\n" +
            "-箇条書きには『・』を使用してください。" +
            "最後に、「印象的なのは」「目を惹いたのは」「興味深いのは」のどれかに続けて" +
            "一番面白かった投稿に皮肉やユーモアを交えた感想を添えて紹介してください。\r\n" +
            "-投稿者の名前も織り込んでください。\r\n" +
            "投稿内の［💬 人名］は投稿者から人名へのリプライを表しています。\r\n" +
            "投稿内の［👤人名］は投稿者から人名へのメンションを表しています。\r\n" +
            "投稿内の［🗒️］は引用リポストを表しています。\r\n" +
            "投稿内の［🖼️］は画像リンクを表しています。\r\n" +
            "投稿内の［🔗］はURLリンクを表しています。\r\n" +
            "【タイムライン】が与えられた時は、毎回このように要約してください。\r\n";

        // 毎回のプロンプト
        private readonly string _promptForEveryMessage =
            "全体で140文字以内にしてください。\r\n" +
            "以下、【タイムライン】\r\n\r\n";

        public FormAI()
        {
            InitializeComponent();
            LoadApiKey();
        }

        private async void ButtonSummarize_Click(object sender, EventArgs e)
        {
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
            SaveApiKey(apiKey);

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
            SaveApiKey(apiKey);

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
    }
}
