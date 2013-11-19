using OpenTK.Graphics.OpenGL;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Screenshot
{
    public partial class Form1 : Form
    {
        bool loaded = false;

        public Form1()
        {
            InitializeComponent();
        }

        Stopwatch sw = new Stopwatch(); // available to all event handlers

        // Load Resources here
        private void glControl1_Load(object sender, EventArgs e)
        {
            loaded = true;
            GL.ClearColor(Color.SkyBlue); // Yey! .NET Colors can be used directly!
            SetupViewport();
            Application.Idle += Application_Idle; // press TAB twice after +=
            sw.Start(); // start at application boot
        }

        void Application_Idle(object sender, EventArgs e)
        {
            double milliseconds = ComputeTimeSlice();
            Accumulate(milliseconds);
            Animate(milliseconds);
        }

        float rotation = 0;
        private void Animate(double milliseconds)
        {
            float deltaRotation = (float)milliseconds /24.0f;
            rotation += deltaRotation;
            glControl1.Invalidate();
        }

        double accumulator = 0;
        int idleCounter = 0;
        private void Accumulate(double milliseconds)
        {
            idleCounter++;
            accumulator += milliseconds;
            if (accumulator > 1000)
            {
                label1.Text = idleCounter.ToString()+" FPS";
                accumulator -= 1000;
                idleCounter = 0; // don't forget to reset the counter!
            }
        }

        private double ComputeTimeSlice()
        {
            sw.Stop();
            double timeslice = sw.Elapsed.TotalMilliseconds;
            sw.Reset();
            sw.Start();
            return timeslice;
        }

        private void SetupViewport()
        {
            int w = glControl1.Width;
            int h = glControl1.Height;
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, w, 0, h, -1, 1); // Bottom-left corner pixel has coordinate (0, 0)
            GL.Viewport(0, 0, w, h); // Use all of the glControl painting area
        }

        // Window Paint here
        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (!loaded)
                return;

            drawShapes();

            glControl1.SwapBuffers();
        }

        private void screenshotCanvas(String path)
        {
            int width = glControl1.Width;
            int height = glControl1.Height;
            GL.PixelStore(PixelStoreParameter.PackAlignment, 1.0f);
            byte[] desBuffer = new byte[width * height * 3];//initialize a managed array

            int size = Marshal.SizeOf(desBuffer[0]) * desBuffer.Length;
            IntPtr pnt = Marshal.AllocHGlobal(size);//initilize an unmanaged array

            byte[] TGAheader = { 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0 };//prepare for the tga headers
            byte[] header = { (byte)(width % 256), (byte)(width / 256), (byte)(height % 256), (byte)(height / 256), 24, 0 };
            

            if (!System.IO.File.Exists(path))
            {
                try
                {
                    //GL.ReadPixels only read the pixels from the glControl i.e. the painted canvas
                    GL.ReadPixels(0, 0, width, height, PixelFormat.Bgr, PixelType.UnsignedByte, pnt);//read pixel dataa from back cache
                    Marshal.Copy(pnt, desBuffer, 0, desBuffer.Length);//copy data from unsafe array to safe array

                    using (FileStream fs = File.Create(path))
                    {
                        fs.Write(TGAheader, 0, 12);
                        fs.Write(header, 0, 6);
                        fs.Write(desBuffer, 0, desBuffer.Length);
                        fs.Close();
                    }
                }
                catch (Exception err)
                {
                    Console.WriteLine(err.Message);
                }
                finally
                {
                    Marshal.FreeHGlobal(pnt);
                }
            }
            else
            {
                Console.WriteLine("File \"{0}\" already exists.", path);
            }
        }

        private void drawShapes()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.Translate(0, 0, 0);

            if (glControl1.Focused)
                GL.Color3(Color.Yellow);
            else
                GL.Color3(Color.Blue);
            GL.Rotate(rotation, OpenTK.Vector3.UnitZ); // OpenTK has this nice Vector3 class!
            GL.Begin(BeginMode.Quads);
            GL.Vertex2(10 + x, 20 + y);
            GL.Vertex2(100 + x, 20 + y);
            GL.Vertex2(100 + x, 100 + y);
            GL.Vertex2(10 + x, 100 + y);
            GL.End();

            GL.Color3(Color.LawnGreen);
            GL.Begin(BeginMode.Triangles);
            GL.Vertex2(110 + z, 30 + w);
            GL.Vertex2(190 + z, 30 + w);
            GL.Vertex2(190 + z, 110 + w);
            GL.End();
        }

        int x = 0, y = 0, z= 0, w = 0;
        private void glControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D)
            {
                x++;
                glControl1.Invalidate();
            }
            if (e.KeyCode == Keys.A)
            {
                x--;
                glControl1.Invalidate();
            }
            if (e.KeyCode == Keys.S)
            {
                y--;
                glControl1.Invalidate();
            }
            if (e.KeyCode == Keys.W)
            {
                y++;
                glControl1.Invalidate();
            }

            if (e.KeyCode == Keys.J)
            {
                z--;
                glControl1.Invalidate();
            }
            if (e.KeyCode == Keys.L)
            {
                z++;
                glControl1.Invalidate();
            }
            if (e.KeyCode == Keys.K)
            {
                w--;
                glControl1.Invalidate();
            }
            if (e.KeyCode == Keys.I)
            {
                w++;
                glControl1.Invalidate();
            }
        }
        // Window Resize here
        private void glControl1_Resize(object sender, EventArgs e)
        {
            SetupViewport();
            glControl1.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String path = "screenshot.tga";//save path
            screenshotCanvas(path);
        }
    }
}
