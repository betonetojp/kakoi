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
            if (Owner == null)
            {
                return;
            }

            var mainForm = (FormMain)Owner;
            if (FormWindowState.Normal != WindowState)
            {
                // 最小化最大化状態の時、元の位置と大きさを保存
                mainForm._formWebLocation = RestoreBounds.Location;
                mainForm._formWebSize = RestoreBounds.Size;
            }
            else
            {
                mainForm._formWebLocation = Location;
                mainForm._formWebSize = Size;
            }
        }

        //private void FormWeb_DoubleClick(object sender, EventArgs e)
        //{
        //    Opacity = 1.0;
        //}
    }
}
