using System;

public class PlayerFinishedLapEvent : RaceEvent
{
    public int LapNumber { get; }

    public TimeSpan LapTime { get; }

    public PlayerFinishedLapEvent(int lapNumber, TimeSpan lapTime)
    {
        Type = RaceEventType.PlayerFinishedLap;
        LapNumber = lapNumber;
        LapTime = lapTime;
    }

    public PlayerFinishedLapEvent(int lapNumber, TimeSpan lapTime, TimeSpan elapsedTime)
    {
        Type = RaceEventType.PlayerFinishedLap;
        LapNumber = lapNumber;
        LapTime = lapTime;
        ElapsedTime = elapsedTime;
    }

    public PlayerFinishedLapEvent(string recoveryFileFormat)
    {
        if (!Matches(recoveryFileFormat))
        {
            throw new Exception("PlayerFinishedLapEvent(): Cannot parse line: " + recoveryFileFormat + " into PlayerFinishedLapEvent!");
        }

        var tokens = SplitLine(recoveryFileFormat);

        if (tokens.Length != 4)
        {
            throw new Exception("PlayerFinishedLapEvent(): Cannot create PlayerFinishedLapEvent from recovery file! Invalid line:  " + recoveryFileFormat);
        }

        Type = RaceEventType.PlayerFinishedLap;
        LapNumber = ToInt(tokens[1]);
        LapTime = ToTimeSpan(tokens[2]);
        ElapsedTime = ToTimeSpan(tokens[3]);
    }

    public override string ToRecoveryFileFormat()
    {
        return RaceEventTypeConverter.FromEnum(Type) + s_recoveryFilePatternDelimiter
         + LapNumber + s_recoveryFilePatternDelimiter
         + LapTime + s_recoveryFilePatternDelimiter
         + ElapsedTime;
    }

    public override string ToString()
    {
        return $"[ type: {RaceEventTypeConverter.FromEnum(Type)}, LapNumber: {LapNumber}, LapTime: {LapTime}, ElapsedTime: {ElapsedTime} ]";
    }


    public override bool Equals(object obj)
    {
        // If parameter is null return false.
        if (obj == null)
        {
            return false;
        }

        // If parameter cannot be cast to Point return false.
        if (!(obj is PlayerFinishedLapEvent other))
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
        return GetRaceEventTypeFromLine(line) == RaceEventType.PlayerFinishedLap;
    }
}