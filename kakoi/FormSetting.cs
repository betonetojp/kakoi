using NTextCat.Commons;
using System.Diagnostics;

namespace kakoi
{
    public partial class FormSetting : Form
    {
        internal FormPostBar? PostBarForm { get; set; }
        public FormSetting()
        {
            InitializeComponent();

            // ボタンの画像をDPIに合わせて表示
            using var graphics = CreateGraphics();
            float scale = graphics.DpiX / 96f;
            int size = (int)(16 * scale);
            if (scale < 2.0f)
            {
                buttonLogOut.Image = new Bitmap(Properties.Resources.icons8_log_out_16, size, size);
            }
            else
            {
                buttonLogOut.Image = new Bitmap(Properties.Resources.icons8_log_out_32, size, size);
            }
        }

        private void FormSetting_Load(object sender, EventArgs e)
        {
            labelOpacity.Text = $"{trackBarOpacity.Value}%";
        }

        private void FormSetting_Shown(object sender, EventArgs e)
        {
            checkBoxTopMost.Focus();
        }

        private void FormSetting_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }

        private void TrackBarOpacity_Scroll(object sender, EventArgs e)
        {
            labelOpacity.Text = $"{trackBarOpacity.Value}%";
            if (Owner != null && PostBarForm != null)
            {
                Owner.Opacity = trackBarOpacity.Value / 100.0;
                PostBarForm.Opacity = Owner.Opacity;
            }
        }

        private void LinkLabelIcons8_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabelIcons8.LinkVisited = true;
            var app = new ProcessStartInfo
            {
                FileName = "https://icons8.com",
                UseShellExecute = true
            };
            Process.Start(app);
        }

        private void LinkLabelVersion_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabelVersion.LinkVisited = true;
            var app = new ProcessStartInfo
            {
                FileName = "https://github.com/betonetojp/kakoi",
                UseShellExecute = true
            };
            Process.Start(app);
        }

        private void TextBoxNsec_Leave(object sender, EventArgs e)
        {
            textBoxNpub.Text = textBoxNsec.Text.GetNpub();
            if (!textBoxNpub.Text.IsNullOrEmpty())
            {
                textBoxNsec.Enabled = false;
            }
        }

        private void ButtonLogOut_Click(object sender, EventArgs e)
        {
            textBoxNsec.Enabled = true;
            textBoxNsec.Text = string.Empty;
            textBoxNpub.Text = string.Empty;
        }
    }
}
