using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace GrCapture
{
    public partial class Form1 : Form
    {
        int i = 1;
        Stopwatch sw = new Stopwatch();
        String fileName = "test.txt";
        String log = "log.txt";
        Int16 frame = 15;//15f/s
        bool notFirst = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void ScreenShot()
        {
            String tmp = "data" + i + ".jpg";
            Rectangle bounds = Screen.GetBounds(Point.Empty);
            // {X=0,Y=0,Width=1366,Height=768}
            using (Bitmap bmp = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                }
                bmp.Save(tmp, ImageFormat.Jpeg);
                bmp.Dispose();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            String content = File.ReadAllText(fileName);
            if (content == "canvas")
            {
                // delete all files if already exist
                /*
                if (notFirst)
                {
                    for (int j = 1; j < totalFrames + 1; j = j + 1)
                    {
                        File.Delete("data" + j + ".jpg");
                    }
                    notFirst = false;
                }
                */
                sw.Start();
                ScreenShot();
                // every 'frame',do sth
                if (i%frame  == 0)
                {
                    using (StreamWriter outfile = new StreamWriter(fileName))
                    {
                        outfile.Write("screenshot");
                    }
                    sw.Stop();
                    TimeSpan ts = sw.Elapsed;
                    string ss = ts.Seconds.ToString();
                    string ms = ts.Milliseconds.ToString();
                    sw.Reset();
                    Console.WriteLine(ss+"."+ms+" s" );
                    textBox1.Text += ss+"."+ms+" s ";
                    using (StreamWriter outfile = new StreamWriter(log, true))
                    {
                        outfile.Write(i + "fps: "+ss+"."+ms+" s, ");
                    }
                }
                // when frame, do sth
                if (i == frame)
                {
                    i = 0;
                    notFirst = true;
                }
                i = i + 1;
            }
        }

        private void stopBtn_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            sw.Stop();
        }

        private void startBtn_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            timer1.Start();
        }

    }
}
