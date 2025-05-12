using SharpDX.Direct2D1;
using SharpDX.DXGI;

using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Factory = SharpDX.Direct2D1.Factory;

namespace SharpDX.Samples
{
    public class Direct2D1WinFormDemoApp : DemoApp
    {
        public Factory Factory2D { get; private set; }
        public DirectWrite.Factory FactoryDWrite { get; private set; }
        public WindowRenderTarget RenderTarget2D { get; private set; }
        public SolidColorBrush SceneColorBrush { get; private set; }

        protected override void Initialize(DemoConfiguration demoConfiguration)
        {
            Factory2D = new Factory();
            FactoryDWrite = new DirectWrite.Factory();

			HwndRenderTargetProperties properties = new HwndRenderTargetProperties
			{
				Hwnd = DisplayHandle,
				PixelSize = new Size2(demoConfiguration.Width, demoConfiguration.Height),
				PresentOptions = PresentOptions.None
			};

			RenderTarget2D = new WindowRenderTarget(Factory2D, new RenderTargetProperties(new PixelFormat(Format.Unknown, AlphaMode.Premultiplied)), properties)
			{
				AntialiasMode = AntialiasMode.PerPrimitive
			};

			SceneColorBrush = new SolidColorBrush(RenderTarget2D, Color.White);
        }

        protected override void BeginDraw()
        {
            base.BeginDraw();
            RenderTarget2D.BeginDraw();
        }

        protected override void EndDraw()
        {
            RenderTarget2D.EndDraw();
            base.EndDraw();
        }
    }
}
