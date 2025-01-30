using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NetMQ;
using NetMQ.Sockets;

namespace ss2411_GUI
{
    public partial class Form1 : Form
    {
        private bool isReceiving = false;
        private PublisherSocket cppPublisher;    // C++用のPUBソケット
        private PublisherSocket pythonPublisher; // Python用のPUBソケット

        private int cameraFrameCount = 0; // カメラフレーム数
        private Stopwatch stopwatch = new Stopwatch(); // タイマー
        private System.Windows.Forms.Timer fpsTimer; // FPS計測用タイマー

        private string captureMode = "";
        public Form1()
        {
            InitializeComponent();

            // C#からC++への画像送信用のPUBソケットを作成
            cppPublisher = new PublisherSocket();
            cppPublisher.Bind("tcp://*:5557");

            // C#からPythonへの画像送信用のPUBソケットを作成
            pythonPublisher = new PublisherSocket();
            pythonPublisher.Bind("tcp://*:5561");

            // FPS計測用のタイマーを正しく初期化（1秒ごとに更新）
            fpsTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            fpsTimer.Tick += FpsTimer_Tick;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void Form1_Closing(object sender, FormClosingEventArgs e)
        {
            // フォーム終了時に受信を停止
            isReceiving = false;

            // プログラム終了時にC++とPythonに通知
            cppPublisher.SendFrame("END");   // C++に終了通知
            pythonPublisher.SendFrame("END"); // Pythonに終了通知

            // ソケットを閉じる
            cppPublisher.Close();
            pythonPublisher.Close();
        }

        private void ReceiveImagesFromPython()
        {
            using (var subscriber = new SubscriberSocket())
            {
                // サブスクライブ設定
                subscriber.Connect("tcp://localhost:5556");
                subscriber.Subscribe("");  // 全てのメッセージを受信

                while (isReceiving)
                {
                    try
                    {
                        // 画像データを受信
                        byte[] frameData = subscriber.ReceiveFrameBytes();
                        using (var ms = new MemoryStream(frameData))
                        {
                            // バイトデータを画像に変換
                            Image image = Image.FromStream(ms);

                            // PictureBoxに画像を表示
                            pictureBox1.Invoke((MethodInvoker)(() =>
                            {
                                pictureBox1.Image = image;
                            }));

                            // フレーム数をカウント
                            cameraFrameCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("受信エラー: " + ex.Message);
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!isReceiving)
            {
                isReceiving = true;
                captureMode = comboBox1.SelectedItem.ToString();

                if (captureMode == "USBカメラ")
                {
                    cppPublisher.SendFrame("START");
                }
                else if (captureMode == "Dummy")
                {
                    // Pythonに通知
                    cppPublisher.SendFrame("STARTDummy");
                }
                else
                {
                    MessageBox.Show("キャプチャモードを選択してください");
                    return;
                }

                // 画像受信のためのスレッドを開始
                Task.Run(() => ReceiveImagesFromPython());

                // FPS計測を開始
                cameraFrameCount = 0;
                stopwatch.Restart(); // 計測開始
                fpsTimer.Start();

                button1.Text = "停止";
                label1.Text = "受信中です．停止ボタンで停止できます";
            }
            else
            {
                // 画像受信を停止
                isReceiving = false;
                cppPublisher.SendFrame("STOP");

                // FPS計測を停止
                fpsTimer.Stop();
                label2.Text = "FPS: 0.00"; // 停止時に FPS をリセット

                button1.Text = "開始";
                label1.Text = "停止しました.開始ボタンで再開できます";
            }
        }

        private void FpsTimer_Tick(object sender, EventArgs e)
        {
            // 経過時間を取得
            double elapsedSeconds = stopwatch.Elapsed.TotalSeconds;

            if (elapsedSeconds > 0)
            {
                // FPS を計算（小数点以下2桁）
                double cameraFps = cameraFrameCount / elapsedSeconds;

                // ラベルの更新
                label2.Text = $"カメラ : {cameraFps:F2} FPS";
            }

            // フレームカウントをリセット
            cameraFrameCount = 0;

            // Stopwatchをリセット
            stopwatch.Restart();
        }
    }
}
