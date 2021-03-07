using System;

namespace TinyBlazorAdmin.Data
{
    public class Schedule
    {
        public DateTime Start { get; set; } = DateTime.MinValue;
        public DateTime End { get; set; } = DateTime.MaxValue;

        public string AlternativeUrl { get; set; } = "";
        public string Cron { get; set; } = "0 0 0 0 0";

        public int DurationMinutes { get; set; } = 0;

    }
}
