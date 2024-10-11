using nokakoiCrypt;
using System.Diagnostics;

namespace kakoi
{
    public partial class FormSetting : Form
    {
        internal FormPostBar? PostBarForm { get; set; }
        //internal FormWeb? WebForm { get; set; }
        public FormSetting()
        {
            InitializeComponent();
            textBoxNokakoiKey.PlaceholderText = NokakoiCrypt.NokakoiTag + " . . .";
        }

        private void FormSetting_Load(object sender, EventArgs e)
        {
            labelOpacity.Text = $"{trackBarOpacity.Value}%";
        }

        private void FormSetting_Shown(object sender, EventArgs e)
        {
            textBoxPassword.Focus();
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
            if (null != Owner && null != PostBarForm)
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
    }
}
