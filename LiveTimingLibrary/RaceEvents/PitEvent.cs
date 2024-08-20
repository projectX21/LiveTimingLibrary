using System;

public class PitEvent : RaceEvent
{
    public string EntryId { get; }

    public int LapNumber { get; }

    public PitEvent(string sessionId, RaceEventType type, string entryId, int lapNumber)
    {
        SessionId = sessionId;
        Type = type;
        EntryId = entryId;
        LapNumber = lapNumber;
    }

    public PitEvent(string sessionId, RaceEventType type, string entryId, int lapNumber, TimeSpan elapsedTime) : base(sessionId, type, elapsedTime)
    {
        SessionId = sessionId;
        EntryId = entryId;
        LapNumber = lapNumber;
    }

    public PitEvent(string recoveryFileFormat)
    {
        if (!Matches(recoveryFileFormat))
        {
            throw new Exception("PitEvent(): Cannot parse line: " + recoveryFileFormat + " into PitEvent!");
        }

        var tokens = SplitLine(recoveryFileFormat);

        if (tokens.Length != 5)
        {
            throw new Exception("PitEvent(): Cannot init PitEvent! Invalid line:  " + recoveryFileFormat);
        }

        SessionId = tokens[0];
        Type = RaceEventTypeConverter.ToEnum(tokens[1]);
        EntryId = tokens[2];
        LapNumber = ToInt(tokens[3]);
        ElapsedTime = ToTimeSpan(tokens[4]);
    }

    public override string ToRecoveryFileFormat()
    {
        return SessionId + s_recoveryFilePatternDelimiter
         + RaceEventTypeConverter.FromEnum(Type) + s_recoveryFilePatternDelimiter
         + EntryId + s_recoveryFilePatternDelimiter
         + LapNumber + s_recoveryFilePatternDelimiter
         + ElapsedTime;
    }

    public override string ToString()
    {
        return $"[ sessionId: {SessionId}, type: {RaceEventTypeConverter.FromEnum(Type)}, EntryId: {EntryId}, LapNumber: {LapNumber}, ElapsedTime: {ElapsedTime} ]";
    }

    public override bool Equals(object obj)
    {
        // If parameter is null return false.
        if (obj == null)
        {
            return false;
        }

        // If parameter cannot be cast to Point return false.
        if (!(obj is PitEvent other))
        {
            return false;
        }

        // Return true if the fields match:
        return SessionId == other.SessionId &&
               Type == other.Type &&
               EntryId == other.EntryId &&
               LapNumber == other.LapNumber &&
               ElapsedTime == other.ElapsedTime;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static bool Matches(string line)
    {
        var eventType = GetRaceEventTypeFromLine(line);
        return eventType == RaceEventType.PitIn || eventType == RaceEventType.PitOut;
    }
}