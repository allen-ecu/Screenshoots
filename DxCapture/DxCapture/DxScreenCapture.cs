using SlimDX.Direct3D9;
using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace DxCapture
{
    public class DxScreenCapture
    {
        Device d;
        int width = Screen.PrimaryScreen.Bounds.Width;
        int height = Screen.PrimaryScreen.Bounds.Height;
        public DxScreenCapture()
        {
            SlimDX.Configuration.AddResultWatch(ResultCode.NotAvailable, SlimDX.ResultWatchFlags.AlwaysIgnore);
            PresentParameters pp = new PresentParameters();
            pp.Windowed = true;
            pp.SwapEffect = SwapEffect.Discard;
            d = new Device(new Direct3D(), 0, DeviceType.Hardware, IntPtr.Zero, CreateFlags.MixedVertexProcessing, pp);
        }

        public Surface CaptureScreen()
        {
            Surface s = Surface.CreateOffscreenPlain(d, width , height, Format.A8R8G8B8, Pool.SystemMemory);
            d.GetFrontBufferData(0, s);
            d.Dispose();
            return s;
        }
    }
}
