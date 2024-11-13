using System.Linq;

public class TestableGameData
{
    public string GameName { get; set; }

    public bool GameRunning { get; set; }

    public bool GamePaused { get; set; }

    public string TrackName { get; set; }

    public string SessionName { get; set; }

    public string SessionId
    {
        get
        {
            return SessionIdGenerator.Generate(this);
        }
    }

    public TestableOpponent[] Opponents { get; set; }

    // public TestableStatusDataBase NewData { get; set; }

    public override bool Equals(object obj)
    {
        // If parameter is null return false.
        if (obj == null)
        {
            return false;
        }

        // If parameter cannot be cast to Point return false.
        if (!(obj is TestableGameData other))
        {
            return false;
        }

        // Return true if the fields match:
        return GameName == other.GameName &&
               GameRunning == other.GameRunning &&
               GamePaused == other.GamePaused &&
               TrackName == other.TrackName &&
               SessionId == other.SessionId &&
               SessionName == other.SessionName &&
               Enumerable.SequenceEqual(Opponents, other.Opponents);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

}