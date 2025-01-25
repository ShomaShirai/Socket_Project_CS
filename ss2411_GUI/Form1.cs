using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
        private bool isReceiving = true;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 画像受信のためのスレッドを開始
            Task.Run(() => ReceiveImages());
        }

        private void Form1_Closing(object sender, FormClosingEventArgs e)
        {
            // フォーム終了時に受信を停止
            isReceiving = false;
        }

        private void ReceiveImages()
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
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("受信エラー: " + ex.Message);
                    }
                }
            }
        }
    }
}
