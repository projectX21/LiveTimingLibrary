public class TestableGameData
{
    public string GameName { get; set; }

    public bool GameRunning { get; set; }

    public TestableStatusDataBase OldData { get; set; }

    public TestableStatusDataBase NewData { get; set; }

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
               OldData.Equals(other.OldData) &&
               NewData.Equals(other.NewData);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

}