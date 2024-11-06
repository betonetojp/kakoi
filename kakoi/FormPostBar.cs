using NNostr.Client;
using System.Diagnostics;

namespace kakoi
{
    public partial class FormPostBar : Form
    {
        internal FormMain? MainForm { get; set; }
        internal NostrEvent? RootEvent { get; set; }
        internal bool IsQuote { get; set; }
        private Point _mousePoint;
        private double _tempOpacity = 1.00;

        public FormPostBar()
        {
            InitializeComponent();

            // ボタンの画像をDPIに合わせて表示
            float scale = CreateGraphics().DpiX / 96f;
            int size = (int)(16 * scale);
            if (scale < 2.0f)
            {
                buttonPost.Image = new Bitmap(Properties.Resources.icons8_create_16, size, size);
                buttonPicture.Image = new Bitmap(Properties.Resources.icons8_picture_16, size, size);
            }
            else
            {
                buttonPost.Image = new Bitmap(Properties.Resources.icons8_create_32, size, size);
                buttonPicture.Image = new Bitmap(Properties.Resources.icons8_picture_32, size, size);
            }
        }

        private void ButtonPost_Click(object sender, EventArgs e)
        {
            if (null != MainForm)
            {
                //MainForm.textBoxPost.Text = textBoxPost.Text;
                MainForm.ButtonPost_Click(RootEvent, IsQuote);
                IsQuote = false;
            }
        }

        private void FormPostBar_MouseDown(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                _mousePoint = new Point(e.X, e.Y);
            }
        }

        private void FormPostBar_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                Left += e.X - _mousePoint.X;
                Top += e.Y - _mousePoint.Y;
            }
        }

        private void FormPostBar_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (null != MainForm)
                {
                    MainForm.checkBoxPostBar.Checked = false;
                    MainForm.dataGridViewNotes.Focus();
                }
                Visible = false;
                e.Cancel = true;
            }
        }

        private void FormPostBar_Shown(object sender, EventArgs e)
        {
            // モーダル解除
            Close();
        }

        private void FormPostBar_DoubleClick(object sender, EventArgs e)
        {
            if (null != MainForm)
            {
                if (FormWindowState.Minimized == MainForm.WindowState)
                {
                    MainForm.WindowState = FormWindowState.Normal;
                }
                else
                {
                    MainForm.WindowState = FormWindowState.Minimized;
                }
            }
        }

        private void FormPostBar_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                FormPostBar_DoubleClick(sender, e);
            }
        }

        private void TextBoxPost_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == (Keys.Enter | Keys.Control))
            {
                e.SuppressKeyPress = true; // エンターキーを無効化
                ButtonPost_Click(sender, e);
            }
        }

        private void FormPostBar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F11)
            {
                FormPostBar_DoubleClick(sender, e);
                // フォーカスキープ
                Activate();
            }
            if (e.KeyCode == Keys.F12 || e.KeyCode == Keys.F1)
            {
                Close();
            }
        }

        private void ButtonPicture_Click(object sender, EventArgs e)
        {
            var app = new ProcessStartInfo
            {
                FileName = Setting.PictureUploadUrl,
                UseShellExecute = true
            };
            Process.Start(app);
        }

        #region 透明解除処理
        // マウス入った時
        private void Control_MouseEnter(object sender, EventArgs e)
        {
            _tempOpacity = Opacity;
            Opacity = 1.00;
        }

        // マウス出た時
        private void Control_MouseLeave(object sender, EventArgs e)
        {
            Opacity = _tempOpacity;
        }
        #endregion

        private void FormPostBar_Activated(object sender, EventArgs e)
        {
            //var result = textBoxPost.Focus();
            if (null != RootEvent && !IsQuote)
            {
                textBoxPost.BackColor = Tools.HexToColor(Setting.ReplyColor);
                buttonPost.BackColor = Tools.HexToColor(Setting.ReplyColor);
            }
            else
            {
                // デフォルトの色に戻す
                textBoxPost.BackColor = SystemColors.Window;
                buttonPost.BackColor = SystemColors.Control;
            }
        }

        // 非表示になる時クリア
        private void FormPostBar_VisibleChanged(object sender, EventArgs e)
        {
            if (!Visible)
            {
                textBoxPost.Text = string.Empty;
                RootEvent = null;
                textBoxPost.PlaceholderText = string.Empty;
                IsQuote = false;
            }
        }
    }
}
