using System.Diagnostics;

namespace SharpDX.Samples
{
    public class DemoTime
    {
		private readonly Stopwatch _stopwatch;
        private double _lastUpdate;

        public DemoTime()
        {
            _stopwatch = new Stopwatch();
        }

        public void Start()
        {
            _stopwatch.Start();
            _lastUpdate = 0;
        }

        public void Stop()
        {
            _stopwatch.Stop();
        }

        public double Update()
        {
            double now = ElapseTime;
            double updateTime = now - _lastUpdate;
            _lastUpdate = now;
            return updateTime;
        }

        public double ElapseTime
        {
            get
            {
                return _stopwatch.ElapsedMilliseconds * 0.001;
            }
        }
    }
}
