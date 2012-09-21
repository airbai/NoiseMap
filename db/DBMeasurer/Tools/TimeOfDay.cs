namespace DBMeasurer.Tools
{
    using System;

    public class TimeOfDay
    {
        public static readonly TimeSpan t1 = DateTime.Parse("6:00").get_TimeOfDay();
        public static readonly TimeSpan t2 = DateTime.Parse("8:00").get_TimeOfDay();
        public static readonly TimeSpan t3 = DateTime.Parse("11:00").get_TimeOfDay();
        public static readonly TimeSpan t4 = DateTime.Parse("13:00").get_TimeOfDay();
        public static readonly TimeSpan t5 = DateTime.Parse("19:00").get_TimeOfDay();

        public static string GetTimeOfDay(DateTime time)
        {
            TimeSpan span = time.get_TimeOfDay();
            if (span < t5)
            {
                if (span >= t4)
                {
                    return "下午";
                }
                if (span >= t3)
                {
                    return "中午";
                }
                if (span >= t2)
                {
                    return "上午";
                }
                if (span >= t1)
                {
                    return "早晨";
                }
            }
            return "夜间";
        }
    }
}

