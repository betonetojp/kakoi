# kakoi

Tiny nostr client for windows.

## 概要 (Overview)
`kakoi` は、コンパクトなグリッド表示でタイムラインの閲覧、投稿、リアクション、返信、リポスト、引用ができる Windows 向け Nostr クライアントです。

Google Gemini AI によるタイムラインまとめ機能、Microsoft Excel へのリアルタイム出力機能、「伺か」(SSP) へのイベント通知連携（DirectSSTP）に対応しています。

---

## 動作環境 (Environment)
- Windows 10 / 11 (x64)
- [.NET 8.0 デスクトップランタイム](https://dotnet.microsoft.com/download/dotnet/8.0)

---

## ショートカットキー & 操作一覧 (Shortcuts & Mouse)

### 画面表示・各種ウィンドウ
| キー / 操作 | 動作 |
| :--- | :--- |
| `ESC` | 基本設定画面（FormSetting）を開く |
| `F1` / `F12` | ポストバー（投稿欄）の表示 / 非表示 |
| `F2` | 「時間（time）」列の表示 / 非表示切替 |
| `F3` | 「アバター（avatar）」列の表示 / 非表示切替 |
| `F4` | 「名前（name）」列の表示 / 非表示切替 |
| `F5` | Gemini AI タイムラインまとめ・対話画面（FormAI）を開く / 閉じる |
| `F9` / `Z` / **余白ダブルクリック** | コンテンツの折り返し（Wrap）表示の ON / OFF 切替 |
| `F10` / **余白右クリック** | マニアクス画面（FormManiacs: ユーザー管理・キーワード通知設定）を開く |
| `F11` | メイン画面の表示 / 非表示 |
| `Ctrl + Shift + A` | **グローバルホットキー**: kakoi メイン画面をアクティブ化 |

### タイムライン操作・アクション
| キー / 操作 | 動作 |
| :--- | :--- |
| `W` / `↑` | 1行上に移動 |
| `S` / `↓` | 1行下に移動 |
| `Shift + W` | 最上行へ移動 |
| `Shift + S` | 最下行へ移動 |
| `1` ～ `0` | 送信するリアクション絵文字を選択 |
| `F` / `→` / **ダブルクリック** | 選択中のイベントへリアクションを送信 |
| `R` | 返信（Reply） |
| `B` | リポスト（Repost） |
| `Q` | 引用（Quote） |
| `A` / `←` / **右クリック** | Web ビュー（WebView2）で投稿を開く |
| `C` | Web ビューを閉じる |

> [!NOTE]
> 返信・引用をキャンセルする場合は、ポストバーを一度閉じてください。

---

## タイムラインの表示仕様
タイムライン本文中の特殊な要素は以下のように絵文字アイコンで省略表示されます（ツールチップで全文や詳細を確認できます）：
- `［💬 ユーザー名 ...］`: 返信先ユーザー
- `［🗒️］`: 引用イベント
- `［🖼️］`: 画像 URL
- `［🔗］`: Web リンク URL
- `［👤ユーザー名］`: メンションされたユーザー

---

## 初期設定と使い方 (Usage)

### 1. ログイン・基本設定 (ESCキー)
- **Private key**: 投稿機能を使用するには、設定画面で Nostr 秘密鍵（`nsec1...`）を入力してログインします。
  - ※秘密鍵は Windows 資格情報マネージャーに DPAPI で暗号化されて安全に保存されます。
- **Show avatar**: ユーザーアイコンの表示・ダウンロードの有無を切り替えます。
- **Show only followees**: フォロイーの投稿のみを表示します。
- **Show non-followees**: フォローしていないユーザーの投稿も表示します。
- **Show reposts only from followees**: フォロイーからのリポストのみを表示します。
- **Show only selected languages**: 言語フィルタを有効にし、指定した言語（日本語、英語など）の投稿のみを表示します（非フォロイーの投稿に適用）。
- **Minimize to system tray**: 有効にすると、ウィンドウを閉じても終了せずにタスクトレイに最小化されます（トレイアイコン右クリックから Quit で終了）。
- **Add client tag**: 投稿に `client` タグを付加します。

### 2. リレー設定
- 画面右下の **リストアイコンボタン（📄）** をクリックすると、リレー設定画面が開きます。
- 接続先リレーの追加・編集・有効 / 無効の切り替えが可能です（設定は `relays.json` に保存されます）。

### 3. Gemini タイムラインまとめ (F5キー)
F5キーを押して AI 画面を開きます。
- **Gemini API Key**: [Google AI Studio](https://aistudio.google.com/apikey) で取得した API キーを入力します（Windows 資格情報に保存）。
- **Model**: 使用する Gemini モデル名（例: `gemini-2.0-flash` など）を設定します。モデル変更時も会話履歴は保持されます。
- **Summarize**: 直近の取得投稿をまとめて要約を表示します。
- **Chat**: 任意のプロンプトを入力して Gemini と直接対話が可能です。

### 4. 「伺か」(SSP) 連携（DirectSSTP）
タイムラインの投稿を「伺か」(SSP) にリアルタイムで流すことができます。
- [SSP](https://ssp.shillest.net/) / [keshiki](https://keshiki.nobody.jp/)
- [GhostSpeaker](https://github.com/apxxxxxxe/GhostSpeaker) と [棒読みちゃん](https://chi.usamimi.info/Program/Application/BouyomiChan/) を組み合わせることで、タイムラインの音声読み上げも可能です。
- 「伺か」(SSP) 用ゴースト「[nostalk](https://github.com/nikolat/nostalk)」の Nostr イベント通知仕様 (Nostr/0.4) に対応し、アバター画像をゴースト側に送信できます。

### 5. Microsoft Excel 連携
Microsoft Excel がインストールされている環境では、kakoi 起動時に自動的に Excel の新規ワークブックが立ち上がり、受信したタイムライン（時刻、名前、本文）がリアルタイムにシートへ書き込まれます。
- **1列目 (time)**: 投稿時刻（`hh:mm:ss` 形式）
- **2列目 (name)**: 送信者名（列幅: 10）
- **3列目 (note)**: 投稿本文（列幅: 60、自動折り返し全体表示）
- 新しいイベントを受信するごとに最上行（1行目）へ新しい行が挿入され、常に最新の投稿が上に並びます。
- Excel がインストールされていない環境や、利用中に Excel 側が閉じられた場合も自動でエラーを捕捉し、安全にリソースを解放して kakoi 本体の動作を継続します。またアプリ終了時には Excel プロセスも自動的にクリーンアップされます。

---

## 各種設定ファイル
アプリの実行フォルダ内に以下の設定ファイルが保存されます：
- `kakoi.config`: ウィンドウ位置、カラー設定、表示オプション等の基本設定（XML形式）
- `relays.json`: 接続先リレー情報
- `users.json`: プロフィール情報のキャッシュ（表示名、petname、ミュート設定等）
- `emojis.json`: リアクションで使用する絵文字リスト
- `clients.json`: クライアントタグに応じた時間の背景色マッピング
- `AI.json`: AI 設定

---

## 利用ライブラリ / NuGet パッケージ
- [CredentialManagement](https://www.nuget.org/packages/CredentialManagement)
- [Google_GenerativeAI](https://www.nuget.org/packages/Google_GenerativeAI)
- [Microsoft.Web.WebView2](https://www.nuget.org/packages/Microsoft.Web.WebView2)
- [NTextCat](https://www.nuget.org/packages/NTextCat)
- [SkiaSharp](https://www.nuget.org/packages/SkiaSharp)
- [Svg.Skia](https://www.nuget.org/packages/Svg.Skia)
- Microsoft.Office.Interop.Excel (COM 参照)
- [NNostr](https://github.com/Kukks/NNostr) (NNostr.Client を一部変更して同梱)
- [DirectSSTPTester](https://github.com/nikolat/DirectSSTPTester) (SSTPLib を利用)
