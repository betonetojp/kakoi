namespace kakoi
{
    public partial class FormWeb : Form
    {
        public FormWeb()
        {
            InitializeComponent();
        }

        private void FormWeb_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (FormWindowState.Normal != WindowState)
            {
                // 最小化最大化状態の時、元の位置と大きさを保存
                Setting.WebLocation = RestoreBounds.Location;
                Setting.WebSize = RestoreBounds.Size;
            }
            else
            {
                Setting.WebLocation = Location;
                Setting.WebSize = Size;
            }
        }

        private void FormWeb_DoubleClick(object sender, EventArgs e)
        {
            Opacity = 1.0;
        }
    }
}
