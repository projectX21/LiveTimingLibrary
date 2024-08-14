using System;

public class PitEvent : RaceEvent
{
    public string EntryId { get; }

    public int LapNumber { get; }

    public PitEvent(RaceEventType type, string entryId, int lapNumber)
    {
        Type = type;
        EntryId = entryId;
        LapNumber = lapNumber;
    }

    public PitEvent(RaceEventType type, string entryId, int lapNumber, TimeSpan elapsedTime) : base(type, elapsedTime)
    {
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

        if (tokens.Length != 4)
        {
            throw new Exception("PitEvent(): Cannot init PitEvent! Invalid line:  " + recoveryFileFormat);
        }

        Type = RaceEventTypeConverter.ToEnum(tokens[0]);
        EntryId = tokens[1];
        LapNumber = ToInt(tokens[2]);
        ElapsedTime = ToTimeSpan(tokens[3]);
    }

    public override string ToRecoveryFileFormat()
    {
        return RaceEventTypeConverter.FromEnum(Type) + s_recoveryFilePatternDelimiter
         + EntryId + s_recoveryFilePatternDelimiter
         + LapNumber + s_recoveryFilePatternDelimiter
         + ElapsedTime;
    }

    public override string ToString()
    {
        return $"[ type: {RaceEventTypeConverter.FromEnum(Type)}, EntryId: {EntryId}, LapNumber: {LapNumber}, ElapsedTime: {ElapsedTime} ]";
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
        return Type == other.Type &&
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