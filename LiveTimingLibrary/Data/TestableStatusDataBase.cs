using System;
using System.Linq;

public class TestableStatusDataBase
{
    public Guid SessionId { get; set; }

    public string SessionName { get; set; }

    public int CurrentLap { get; set; }

    public TimeSpan CurrentLapTime { get; set; }

    public TestableOpponent[] Opponents { get; set; }


    public override bool Equals(object obj)
    {
        // If parameter is null return false.
        if (obj == null)
        {
            return false;
        }

        // If parameter cannot be cast to Point return false.
        if (!(obj is TestableStatusDataBase other))
        {
            return false;
        }

        // Return true if the fields match:
        return SessionId == other.SessionId &&
               SessionName == other.SessionName &&
               CurrentLap == other.CurrentLap &&
               CurrentLapTime == other.CurrentLapTime &&
               Enumerable.SequenceEqual(Opponents, other.Opponents);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

}