namespace SharpDX.Samples
{
    public class DemoConfiguration
    {
        public DemoConfiguration() : this("SharpDX Demo") {
        }

        public DemoConfiguration(string title) : this(title, 1080, 720)
        {
        }

        public DemoConfiguration(string title, int width, int height)
        {
            Title = title;
            Width = width;
            Height = height;
            WaitVerticalBlanking = false;
        }

        public string Title {
            get;
            set;
        }

        public int Width {
            get;
            set;
        }

        public int Height {
            get;
            set;
        }

        public bool WaitVerticalBlanking
        {
            get; set;
        }
    }
}
