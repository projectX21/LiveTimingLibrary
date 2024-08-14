public class EntryPitData
{
    public string EntryId { get; }

    public int TotalPitStops { get; } = 0;

    public int TotalPitDuration { get; } = 0;

    public int? LastPitDuration { get; }

    public int? LapNumberLastPitOut { get; }

    public EntryPitData(string entryId, int totalPitStops, int totalPitDuration, int? lastPitDuration, int? lapNumberLastPitOut)
    {
        EntryId = entryId;
        TotalPitStops = totalPitStops;
        TotalPitDuration = totalPitDuration;
        LastPitDuration = lastPitDuration;
        LapNumberLastPitOut = lapNumberLastPitOut;
    }

    public override bool Equals(object obj)
    {
        // If parameter is null return false.
        if (obj == null)
        {
            return false;
        }

        // If parameter cannot be cast to Point return false.
        if (!(obj is EntryPitData other))
        {
            return false;
        }

        // Return true if the fields match:
        return EntryId == other.EntryId &&
               TotalPitStops == other.TotalPitStops &&
               TotalPitDuration == other.TotalPitDuration &&
               LastPitDuration == other.LastPitDuration &&
               LapNumberLastPitOut == other.LapNumberLastPitOut;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

}