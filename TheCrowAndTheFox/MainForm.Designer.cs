using SharpDX;
using SharpDX.Direct3D11; // Direct3D
using SharpDX.DXGI;
using SharpDX.Direct2D1; // Direct2D
using System.Windows.Forms;
using Device = SharpDX.Direct3D11.Device;
using DeviceContext3D = SharpDX.Direct3D11.DeviceContext; // Alias for Direct3D DeviceContext
using RenderTarget = SharpDX.Direct2D1.RenderTarget; 
using Factory = SharpDX.Direct2D1.Factory;
using SharpDX.Direct3D;
using TheCrowAndTheFox.Models;

namespace TheCrowAndTheFox
{
	public partial class MainForm : Form
	{
		private Device device;
		private DeviceContext3D context; 
		private SwapChain swapChain;
		private RenderTargetView renderTargetView;
		private Factory d2dFactory;
		private RenderTarget d2dRenderTarget;
		private Label label1;

		private Game game;
		private Player player;
		private Timer timer;

		public MainForm()
		{
			InitializeComponent();
			InitializeDirect3D();
			InitializeDirect2D();
			//player = new Player(); 
			//game = new Game(player); 
			this.KeyDown += OnKeyDown; 
		}

		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				Application.Exit();

			if (e.KeyCode == Keys.A)
				player.MoveLeft();

			if (e.KeyCode == Keys.D)
				player.MoveRight();
		}

		private void InitializeDirect3D()
		{
			var swapChainDescription = new SwapChainDescription()
			{
				BufferCount = 1,
				ModeDescription = new ModeDescription(ClientSize.Width, ClientSize.Height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
				IsWindowed = true,
				OutputHandle = this.Handle,
				SampleDescription = new SampleDescription(1, 0),
				SwapEffect = SwapEffect.Discard,
				Usage = Usage.RenderTargetOutput,
			};

			Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.BgraSupport, swapChainDescription, out device, out swapChain);

			var backBuffer = swapChain.GetBackBuffer<Texture2D>(0);
			renderTargetView = new RenderTargetView(device, backBuffer);
			context = device.ImmediateContext;
			context.OutputMerger.SetRenderTargets(renderTargetView);
		}

		private void InitializeDirect2D()
		{
			d2dFactory = new Factory();
			var properties = new HwndRenderTargetProperties
			{
				Hwnd = this.Handle,
				PixelSize = new Size2(ClientSize.Width, ClientSize.Height),
				PresentOptions = PresentOptions.Immediately,
			};
			d2dRenderTarget = new WindowRenderTarget(d2dFactory, new RenderTargetProperties(), properties);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			Render();
		}

		private void Render()
		{
			if (context == null || d2dRenderTarget == null)
			{
				return;
			}

			context.ClearRenderTargetView(renderTargetView, Color.White);
			
			d2dRenderTarget.BeginDraw();
			d2dRenderTarget.Clear(Color.White); 

			game.Update();
			game.Render(d2dRenderTarget);
			Text = player.Score.ToString();

			d2dRenderTarget.EndDraw();
			swapChain.Present(1, PresentFlags.None);
			Invalidate(); 
		}

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			swapChain?.Dispose();
			renderTargetView?.Dispose();
			context?.Dispose();
			device?.Dispose();
			d2dRenderTarget?.Dispose();
			d2dFactory?.Dispose();
			base.OnFormClosing(e);
		}

	}
}