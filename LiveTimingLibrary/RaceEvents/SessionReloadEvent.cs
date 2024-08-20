using System;

public class SessionReloadEvent : RaceEvent
{
    public int LapNumber { get; }

    public SessionReloadEvent(string sessionId, int lapNumber)
    {
        SessionId = sessionId;
        Type = RaceEventType.SessionReload;
        LapNumber = lapNumber;
    }

    public SessionReloadEvent(string sessionId, int lapNumber, TimeSpan elapsedTime)
    {
        SessionId = sessionId;
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

        if (tokens.Length != 4)
        {
            throw new Exception("SessionReloadEvent(): Cannot create SessionReloadEvent from recovery file! Invalid line:  " + recoveryFileFormat);
        }

        SessionId = tokens[0];
        Type = RaceEventType.SessionReload;
        LapNumber = ToInt(tokens[2]);
        ElapsedTime = ToTimeSpan(tokens[3]);
    }

    public override string ToRecoveryFileFormat()
    {
        return SessionId + s_recoveryFilePatternDelimiter
          + RaceEventTypeConverter.FromEnum(Type) + s_recoveryFilePatternDelimiter
          + LapNumber + s_recoveryFilePatternDelimiter
          + ElapsedTime;
    }

    public override string ToString()
    {
        return $"[ sessionId: {SessionId}, type: {RaceEventTypeConverter.FromEnum(Type)}, LapNumber: {LapNumber}, ElapsedTime: {ElapsedTime} ]";
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
        return SessionId == other.SessionId &&
               Type == other.Type &&
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