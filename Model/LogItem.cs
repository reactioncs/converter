using System;

namespace ImageConverter.Model
{
    public class LogItem
    {
        public DateTime Time { get; set; } = DateTime.Now;
        public DateTime TimePrevious { get; set; }
        public string Message { get; set; } = "";

        public int Span
        {
            get => (int)new TimeSpan(Time.Ticks - TimePrevious.Ticks).TotalMilliseconds;
        }
    }
}
