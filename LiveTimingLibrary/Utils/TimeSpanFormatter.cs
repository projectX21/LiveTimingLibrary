using System;

public class TimeSpanFormatter
{
    public static string Format(TimeSpan? value)
    {
        if (value == null)
        {
            return "";
        }

        var hours = value?.Hours ?? 0;
        var minutes = value?.Minutes ?? 0;
        var seconds = value?.Seconds ?? 0;
        var millis = value?.Milliseconds ?? 0;

        if (hours > 0)
        {
            return hours + ":" + minutes.ToString("D2") + ":" + seconds.ToString("D2") + "." + millis.ToString("D3");
        }
        else if (minutes > 0)
        {
            return minutes.ToString() + ":" + seconds.ToString("D2") + "." + millis.ToString("D3");
        }
        else
        {
            return seconds.ToString() + "." + millis.ToString("D3");
        }
    }
}