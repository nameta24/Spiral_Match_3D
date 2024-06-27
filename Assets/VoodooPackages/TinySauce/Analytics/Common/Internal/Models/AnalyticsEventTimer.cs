using System;

namespace Voodoo.Tiny.Sauce.Internal.Analytics
{
    public class AnalyticsEventTimer
    {
        private DateTime? _startTime;
        private DateTime? _pauseTime;
        private TimeSpan _pauseDuration = TimeSpan.Zero;
        private TimeSpan _duration = TimeSpan.Zero;
        private bool? _pauseStatus;
        private TimerStatus? _timerStatus;

        private enum TimerStatus
        {
            Started,
            Stopped,
            Paused,
        }

        public void Start()
        {
            _timerStatus = TimerStatus.Started;
            _startTime = DateTime.Now;
            //reset timer
            _pauseTime = null;
            _pauseDuration = TimeSpan.Zero;
            _duration = TimeSpan.Zero;
        }

        public void Stop()
        {
            _timerStatus = TimerStatus.Stopped;
            DateTime now = DateTime.Now;
            if (_startTime != null && now > _startTime) {
                _duration = (TimeSpan) (now - _startTime) - _pauseDuration;
            } else {
                _duration = TimeSpan.Zero;
            }
            //reset timer
            _pauseDuration = TimeSpan.Zero;
            _pauseTime = null;
            _startTime = null;
        }

        public void Pause()
        {
            if (_timerStatus != TimerStatus.Started) return;
            _timerStatus = TimerStatus.Paused;
            _pauseTime = DateTime.Now;
        }

        public void Resume()
        {
            if (_timerStatus != TimerStatus.Paused || _pauseTime == null) return;
            _timerStatus = TimerStatus.Started;
            DateTime now = DateTime.Now;
            //add the new pause duration
            if (now > _pauseTime) {
                _pauseDuration += (TimeSpan) (now - _pauseTime);
            }
        }

        public int GetDuration() => (int) GetDurationTimeSpan().TotalMilliseconds;
        private TimeSpan GetDurationTimeSpan() => _duration.TotalMilliseconds >= 0 ? _duration : TimeSpan.Zero;
        
    }
}