using System;

public class SessionReloadEvent : RaceEvent
{
    public int LapNumber { get; }

    public SessionReloadEvent(int lapNumber)
    {
        Type = RaceEventType.SessionReload;
        LapNumber = lapNumber;
    }

    public SessionReloadEvent(int lapNumber, TimeSpan elapsedTime)
    {
        Type = RaceEventType.SessionReload;
        LapNumber = lapNumber;
        ElapsedTime = elapsedTime;
    }

    public SessionReloadEvent(string recoveryFileFormat)
    {
        if (!Matches(recoveryFileFormat))
        {
            throw new Exception("SessionReloadEvent(): Cannot parse line: " + recoveryFileFormat + " into SessionReloadEvent!");
        }

        var tokens = SplitLine(recoveryFileFormat);

        if (tokens.Length != 3)
        {
            throw new Exception("SessionReloadEvent(): Cannot create SessionReloadEvent from recovery file! Invalid line:  " + recoveryFileFormat);
        }

        Type = RaceEventType.SessionReload;
        LapNumber = ToInt(tokens[1]);
        ElapsedTime = ToTimeSpan(tokens[2]);
    }

    public override string ToRecoveryFileFormat()
    {
        return RaceEventTypeConverter.FromEnum(Type) + s_recoveryFilePatternDelimiter
          + LapNumber + s_recoveryFilePatternDelimiter
          + ElapsedTime;
    }

    public override string ToString()
    {
        return $"[ type: {RaceEventTypeConverter.FromEnum(Type)}, LapNumber: {LapNumber}, ElapsedTime: {ElapsedTime} ]";
    }

    public override bool Equals(object obj)
    {
        // If parameter is null return false.
        if (obj == null)
        {
            return false;
        }

        // If parameter cannot be cast to Point return false.
        if (!(obj is SessionReloadEvent other))
        {
            return false;
        }

        // Return true if the fields match:
        return Type == other.Type &&
               LapNumber == other.LapNumber &&
               ElapsedTime == other.ElapsedTime;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static bool Matches(string line)
    {
        return GetRaceEventTypeFromLine(line) == RaceEventType.SessionReload;
    }
}