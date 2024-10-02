◆ 動作環境

Windows11 22H2 (x64)
.NET 8.0
※ランタイムが必要です。インストールしていない場合は初回起動時の案内に従ってください。


◆ kakoi.exe

nokakoiをグリッド表示にしてリアクションできるようにしたものです。
イベントをダブルクリックまたはカーソルキー右でリアクションを送信します。
イベントを右クリックまたはカーソルキー左でWebビューで開きます。

その他はnokakoiと同じです。
https://github.com/betonetojp/nokakoi

ESCキー 設定画面
F9キー  コンテンツの折り返し表示の切り替え（余白ダブルクリックでも動作）
F10キー ユーザーミュートとキーワード通知の設定画面（余白右クリックでも動作）
F11キー メイン画面の表示と非表示
F12キー ポストバーの表示と非表示


◆ 更新履歴

2024/10/03 v0.2.2
名前の列幅を保存するようにしました。
eタグを含む投稿の背景色を変更するようにしました。
clientタグにkakoiとlumilumiを含む投稿の背景色を変更するようにしました。

2024/09/30 v0.2.1
clientタグ修正

2024/09/23 v0.2.0
イベントをWebビューで開けるようにしました。
イベントを右クリックまたはカーソルキー左でWebビューで開きます。
※現状ではF10キーで開く設定画面のキーワード通知のURLを使用します。
　https://lumilumi.vercel.app/ がおすすめです。

2024/09/16 v0.1.1
リアクションを選択できるようにしました。
項目の変更はemojis.jsonを編集してください。

2024/09/09 v0.1.0
設定項目を整理しました。

2024/09/08 v0.0.1
初公開


◆ 利用NuGetパッケージ

NTextCat
https://github.com/ivanakcheurov/ntextcat

◆ Nostrクライアントライブラリ

NNostr
https://github.com/Kukks/NNostr
内のNNostr.Client Ver0.0.49を一部変更して利用しています。


◆ DirectSSTP送信ライブラリ

DirectSSTPTester
https://github.com/nikolat/DirectSSTPTester
内のSSTPLib Ver4.0.0を利用しています。
