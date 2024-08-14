using System;

public class TestableSectorTimes
{
    public TimeSpan? Sector1 { get; set; }

    public TimeSpan? Sector2 { get; set; }

    public TimeSpan? Sector3 { get; set; }

    public TimeSpan? FullLap { get; set; }

    public TimeSpan? GetByLapFragmentType(LapFragmentType type)
    {
        switch (type)
        {
            case LapFragmentType.SECTOR_1: return Sector1;
            case LapFragmentType.SECTOR_2: return Sector2;
            case LapFragmentType.SECTOR_3: return Sector3;
            case LapFragmentType.FULL_LAP: return FullLap;
        }

        return null;
    }

    public override bool Equals(object obj)
    {
        // If parameter is null return false.
        if (obj == null)
        {
            return false;
        }

        // If parameter cannot be cast to Point return false.
        if (!(obj is TestableSectorTimes other))
        {
            return false;
        }

        // Return true if the fields match:
        return Sector1 == other.Sector1 &&
               Sector2 == other.Sector2 &&
               Sector3 == other.Sector3 &&
               FullLap == other.FullLap;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}