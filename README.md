# ソケット通信を用いたC#のGUI部分
## 課題名：C++,python,C# 連携映像処理
氏名：白井　正真

## 仕様
- 画像入力(WEBカメラ)はC++，画像処理（オブジェクト検出）はPython，GUI（表示・操作）はC#を使い，ソケット通信によりリア
ルタイム連携するアプリケーション
- ソケット通信にはライブラリのZeroMQ(NetMQ)を用いる
- C++（カメラ入力）→ Python（画像処理）→ C#（GUI表示）の流れで画像データを送信する．

## コードの流れ
1. カメラキャプチャーを行って，左側のpictureBoxに出力する
2. ポート番号とボーレートをComboBoxから受け取った場合にシリアル接続を開始する
3. 右側のpictureBoxにセンサ値を出力する
4. マウス感度を選択したら，空中マウスを開始する
5. ボタンを押すことで空中マウスを終わりにできる

## 開発環境
- Windows 11 WSL2
- Visual studio 2022
- NETMQ(C#のZeroMQ)

## 実行方法
NugetからNETMQをインストールしておけば大丈夫
Visual Studio 2022でデバックすればよい

## 工夫した点
- シリアル通信ができない場合やカメラ起動ができない場合のエラー処理を行っている
- シリアル通信のパラメータをGUI上で設定できるようにしている
- マウス感度をGUI上で設定できるようにしている
- 空中マウス起動はシリアル通信中でないとできないようにしている
- 上のラベルに次に推奨される操作を記入することで操作性を向上している

## 参考資料
- Qiita C#フォームプログラムでのシリアル通信の仕方(https://qiita.com/mag2/items/d15bc3c9d66ce0c8f6b1)
- [VisualStudioの教科書] C#ツールボックス一覧 (https://www.kyoukasho.net/entry/c-sharp-toolbox)
- Teratail pictureboxにbitmapを使用して折れ線グラフを表示するアプリを作成したい。(https://teratail.com/questions/37484)

