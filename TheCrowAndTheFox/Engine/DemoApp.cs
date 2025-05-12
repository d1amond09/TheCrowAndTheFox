using System.Windows.Forms;
using SharpDX.Windows;
using System;

namespace SharpDX.Samples
{
    public abstract class DemoApp
    {
        private readonly DemoTime clock = new DemoTime();
        private FormWindowState _currentFormWindowState;
        private bool _disposed;
        private Form _form;
        private float _frameAccumulator;
        private int _frameCount;
        private DemoConfiguration _demoConfiguration;

        ~DemoApp()
        {
            if (!_disposed)
            {
                Dispose(false);
                _disposed = true;
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                Dispose(true);
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposeManagedResources)
        {
            if (disposeManagedResources)
            {
				_form?.Dispose();
            }
        }

        protected IntPtr DisplayHandle
        {
            get
            {
                return _form.Handle;
            }
        }

        public DemoConfiguration Config
        {
            get
            {
                return _demoConfiguration;
            }
        }

        public float FrameDelta { get; private set; }

        public float FramePerSecond { get; private set; }

        protected virtual Form CreateForm(DemoConfiguration config)
        {
            return new RenderForm(config.Title)
            {
                ClientSize = new System.Drawing.Size(config.Width, config.Height),
				WindowState = FormWindowState.Maximized,
				FormBorderStyle = FormBorderStyle.None,
				IsFullscreen = true,
            };
        }

        public void Run()
        {
            Run(new DemoConfiguration());
        }

        public void Run(DemoConfiguration demoConfiguration)
        {
            _demoConfiguration = demoConfiguration ?? new DemoConfiguration();
            _form = CreateForm(_demoConfiguration);
            Initialize(_demoConfiguration);

            bool isFormClosed = false;
            bool formIsResizing = false;

            _form.MouseClick += HandleMouseClick;
            _form.KeyDown += HandleKeyDown;
            _form.KeyUp += HandleKeyUp;
            _form.Resize += (o, args) =>
            {
                if (_form.WindowState != _currentFormWindowState)
                {
                    HandleResize(o, args);
                }

                _currentFormWindowState = _form.WindowState;
            };

            _form.ResizeBegin += (o, args) => { formIsResizing = true; };
            _form.ResizeEnd += (o, args) =>
            {
                formIsResizing = false;
                HandleResize(o, args);
            };

            _form.Closed += (o, args) => { isFormClosed = true; };

            LoadContent();

            clock.Start();
            BeginRun();
            RenderLoop.Run(_form, () =>
            {
                if (isFormClosed)
                {
                    return;
                }

                OnUpdate();
                if (!formIsResizing)
                    Render();
            });

            UnloadContent();
            EndRun();

            Dispose();
        }

        protected abstract void Initialize(DemoConfiguration demoConfiguration);

        protected virtual void LoadContent()
        {
        }

        protected virtual void UnloadContent()
        {
        }

        protected virtual void Update(DemoTime time)
        {
        }

        protected virtual void Draw(DemoTime time)
        {
        }

        protected virtual void BeginRun()
        {
        }

        protected virtual void EndRun()
        {
        }

        protected virtual void BeginDraw()
        {
        }

        protected virtual void EndDraw()
        {
        }

        public void Exit()
        {
            _form.Close();
        }

        private void OnUpdate()
        {
            FrameDelta = (float)clock.Update();
            Update(clock);
        }

        protected System.Drawing.Size RenderingSize
        {
            get
            {
                return _form.ClientSize;
            }
        }

        private void Render()
        {
            _frameAccumulator += FrameDelta;
            ++_frameCount;
            if (_frameAccumulator >= 1.0f)
            {
                FramePerSecond = _frameCount / _frameAccumulator;

                _form.Text = _demoConfiguration.Title + " - FPS: " + FramePerSecond + $" | score: ";

				_frameAccumulator = 0.0f;  
                _frameCount = 0;
            }

            BeginDraw();
            Draw(clock);
            EndDraw();
        }

        protected virtual void MouseClick(MouseEventArgs e)
        {
        }

        protected virtual void KeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Exit();
        }

        protected virtual void KeyUp(KeyEventArgs e)
        {
        }

        private void HandleMouseClick(object sender, MouseEventArgs e)
        {
            MouseClick(e);
        }

        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            KeyDown(e);
        }

        private void HandleKeyUp(object sender, KeyEventArgs e)
        {
            KeyUp(e);
        }

        private void HandleResize(object sender, EventArgs e)
        {
            if (_form.WindowState == FormWindowState.Minimized)
            {
                return;
            }
        }
    }
}