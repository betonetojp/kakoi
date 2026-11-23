# kakoi

Tiny nostr client for windows.

## 概要 (Overview)
`kakoi` は、コンパクトなグリッド表示でタイムラインの閲覧、投稿、リアクション、返信、リポスト、引用ができる Windows 向け Nostr クライアントです。

Microsoft Excel との双方向リアルタイム連携（タイムライン閲覧・投稿・返信・リアクション・リポスト）や、「伺か」(SSP) へのイベント通知連携（DirectSSTP）に対応しています。

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
| `F5` | Microsoft Excel 双方向連携の ON / OFF 切替（起動時はデフォルト OFF） |
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
  - ※秘密鍵は Windows 資格情報マネージャー（Win32 `advapi32.dll`）に DPAPI で暗号化されて安全に保存されます。
- **Show avatar**: ユーザーアイコンの表示・ダウンロードの有無を切り替えます（Windows 標準 WIC + `System.Drawing` を使用し、PNG / JPEG / GIF / BMP / WebP に対応）。
- **Show only followees**: フォロイーの投稿のみを表示します。
- **Show non-followees**: フォローしていないユーザーの投稿も表示します。
- **Show reposts only from followees**: フォロイーからのリポストのみを表示します。
- **Show only selected languages**: 言語フィルタを有効にし、指定した言語（日本語、英語など）の投稿のみを表示します（非フォロイーの投稿に適用）。
- **Minimize to system tray**: 有効にすると、ウィンドウを閉じても終了せずにタスクトレイに最小化されます（トレイアイコン右クリックから Quit で終了）。
- **Add client tag**: 投稿に `client` タグを付加します。

### 2. リレー設定
- 画面右下の **リストアイコンボタン（📄）** をクリックすると、リレー設定画面が開きます。
- 接続先リレーの追加・編集・有効 / 無効の切り替えが可能です（設定は `relays.json` に保存されます）。

### 3. Microsoft Excel 双方向連携 (F5キー)
起動時はデフォルト OFF です。`F5` キーを押すと Excel の新規ワークブックが開き、直近のタイムライン（最大 50 件）が初期流し込みされたうえで、以降の投稿がリアルタイムに反映されます。再度 `F5` を押すか Excel を閉じると安全に連携を解除します。

- **1行目（固定入力バー）**:
  - `A1`: モード表示（`投稿` / `返信`）
  - `B1`: 返信先表示（`Delete` でクリアすると返信モード解除）
  - `C1`: **投稿入力欄** — テキストを入力して `Enter` で Nostr へ投稿・返信できます
  - `D1`: 送信ステータス表示
- **2行目（ヘッダー行・ウィンドウ枠固定）**: `時刻` / `名前` / `内容` / `反応`
- **3行目以降（タイムライン・最大 500 行保持）**:
  - 新着イベントが 3 行目へ自動挿入されます（セル編集中はキューに溜めて編集完了後に自動反映）。
  - **`D` 列（反応）に文字を入力して `Enter`**:
    - `+` や任意の絵文字（例: `❤️`, `🔥`）→ その投稿へ **リアクション** を送信
    - `b` または `rp` → その投稿を **リポスト**
    - `r` または `返信`（あるいは行の **ダブルクリック**）→ 1 行目を **返信モード** にセット（`C1` から返信送信可能）

### 4. 「伺か」(SSP) 連携（DirectSSTP）
タイムラインの投稿を「伺か」(SSP) にリアルタイムで流すことができます。
- [SSP](https://ssp.shillest.net/) / [keshiki](https://keshiki.nobody.jp/)
- [GhostSpeaker](https://github.com/apxxxxxxe/GhostSpeaker) と [棒読みちゃん](https://chi.usamimi.info/Program/Application/BouyomiChan/) を組み合わせることで、タイムラインの音声読み上げも可能です。
- 「伺か」(SSP) 用ゴースト「[nostalk](https://github.com/nikolat/nostalk)」の Nostr イベント通知仕様 (Nostr/0.4) に対応し、アバター画像 URL をゴースト側に送信できます。

---

## 各種設定ファイル
アプリの実行フォルダ内に以下の設定ファイルが保存されます：
- `kakoi.config`: ウィンドウ位置、カラー設定、表示オプション等の基本設定（XML形式）
- `relays.json`: 接続先リレー情報
- `users.json`: プロフィール情報のキャッシュ（表示名、petname、ミュート設定等）
- `emojis.json`: リアクションで使用する絵文字リスト
- `clients.json`: クライアントタグに応じた時間の背景色マッピング
- `keywords.json`: キーワード通知設定

---

## 利用ライブラリ / NuGet パッケージ
- [Microsoft.Web.WebView2](https://www.nuget.org/packages/Microsoft.Web.WebView2)
- [NBitcoin.Secp256k1](https://www.nuget.org/packages/NBitcoin.Secp256k1)
- [NTextCat](https://www.nuget.org/packages/NTextCat)
- Microsoft.Office.Interop.Excel (COM 参照)
- [NNostr](https://github.com/Kukks/NNostr) (`NNostr.Client` から利用ファイルのみを `kakoi/Nostr` に整理して同梱)
- [DirectSSTPTester](https://github.com/nikolat/DirectSSTPTester) (`SSTPLib` を利用)
