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
            if (FormWindowState.Normal != WindowState)
            {
                // 最小化最大化状態の時、元の位置と大きさを保存
                ((FormMain)Owner)._formWebLocation = RestoreBounds.Location;
                ((FormMain)Owner)._formWebSize = RestoreBounds.Size;
            }
            else
            {
                ((FormMain)Owner)._formWebLocation = Location;
                ((FormMain)Owner)._formWebSize = Size;
            }
        }

        //private void FormWeb_DoubleClick(object sender, EventArgs e)
        //{
        //    Opacity = 1.0;
        //}
    }
}
