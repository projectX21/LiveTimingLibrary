using System;

public abstract class RaceEvent
{
    protected static readonly char s_recoveryFilePatternDelimiter = ';';

    public RaceEventType Type { get; protected set; }

    public TimeSpan ElapsedTime { get; set; }

    public RaceEvent() { }

    public RaceEvent(RaceEventType type)
    {
        Type = type;
    }

    public RaceEvent(RaceEventType type, TimeSpan elapsedTime)
    {
        Type = type;
        ElapsedTime = elapsedTime;
    }

    public abstract string ToRecoveryFileFormat();

    public static RaceEventType GetRaceEventTypeFromLine(string line)
    {
        var tokens = SplitLine(line);
        return RaceEventTypeConverter.ToEnum(tokens[0]);
    }

    protected static string[] SplitLine(string line)
    {
        return line.Split(s_recoveryFilePatternDelimiter);
    }

    protected static TimeSpan ToTimeSpan(string value)
    {
        return TimeSpan.Parse(value);
    }

    protected static int ToInt(string value)
    {
        try
        {
            return int.Parse(value);
        }
        catch (FormatException)
        {
            throw new Exception($"RaceEvent::ToInt(): cannot convert value to int: {value}!");
        }
    }

}