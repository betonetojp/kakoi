namespace kakoi
{
    public class ImeStatus : IMessageFilter
    {
        public ImeStatus()
        {
            // コンストラクター内でメッセージフィルターとして自身を登録
            Application.AddMessageFilter(this);
        }

        /// <summary>
        /// 日本語入力中かどうか
        /// </summary>
        public bool Compositing
        {
            get; private set;
        }

        // アプリケーションがウィンドウメッセージを処理する前に呼び出される
        public bool PreFilterMessage(ref Message m)
        {
            const int WmStartComposition = 0x10D;
            const int WmEndComposition = 0x10E;
            switch (m.Msg)
            {
                case WmStartComposition:
                    // 日本語入力の開始
                    Compositing = true;
                    break;
                case WmEndComposition:
                    // 日本語入力の終了
                    Compositing = false;
                    break;
            }
            return false;
        }
    }
}
