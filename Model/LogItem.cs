using System;

namespace Converter.Model
{
    internal class LogItem
    {
        public DateTime Time { get; set; }
        public string Message { get; set; }

        public LogItem()
        {
            Time = DateTime.Now;
            Message = "";
        }
    }
}
