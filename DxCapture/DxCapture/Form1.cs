using SlimDX.Direct3D9;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace DxCapture
{
    public partial class Form1 : Form
    {
        int i = 1;
        Int16 frame = 15;//30 frames
        Stopwatch sw = new Stopwatch();
        String fileName = "test.txt";
        String log = "log.txt";
        bool notFirst = false;
        Surface s;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            sw.Start();
            this.Screenshot();
            sw.Stop();
            double t = sw.Elapsed.TotalMilliseconds * 0.001;
            textBox1.Text = (t.ToString("G3") + " s");
        }

        private void Screenshot()
        {
            String tmp = "data" + i + ".jpg";
            try
            {
                DxScreenCapture sc = new DxScreenCapture();
                s = sc.CaptureScreen();
                Surface.ToFile(s, tmp, ImageFileFormat.Jpg);
            }
            catch (SlimDX.Direct3D9.Direct3D9Exception err)
            {
               Console.WriteLine(err.Message);
            }
            finally
            {
                s.Dispose();
            }
        }

        private void startBtn_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            timer1.Start();
        }

        private void stopBtn_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ExeScreenSaveJpg();
        }

        private void ExeScreenSaveJpg()
        {
            String content = File.ReadAllText(fileName);
            if (content == "canvas")
            {
                sw.Start();
                this.Screenshot();
                // last frame
                if (i%frame ==0)
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
                    textBox1.Text += ss + "." + ms + " s ";
                    Console.WriteLine(ss + "." + ms + " s ");
                    using (StreamWriter outfile = new StreamWriter(log, true))
                    {
                        outfile.Write(i + "fps: " + ss + "." + ms + " s, ");
                    }
                }
                if (i == frame)
                {
                    i = 0;
                    notFirst = true;
                }
                i = i + 1;
            }
        }
    }
}
