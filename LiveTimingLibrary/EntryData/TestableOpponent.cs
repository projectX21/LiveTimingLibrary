using System;

public class TestableOpponent
{
    public string Id
    {
        get
        {
            return CarNumber?.Length > 0
                ? CarNumber
                : Name + "-" + TeamName + "-" + CarName;
        }
    }

    public string Manufacturer { get; set; }

    public string Name { get; set; }

    public string TeamName { get; set; }

    public bool IsInPit { get; set; }

    public int Position { get; set; }

    public string CarName { get; set; }

    public string CarClass { get; set; }

    public bool IsPlayer { get; set; }

    public double? TrackPositionPercent { get; set; }

    public int? CurrentLap { get; set; }

    public TimeSpan? CurrentLapTime { get; set; }

    public string CarNumber { get; set; }

    public double? GapToLeader { get; set; }

    public TestableSectorTimes CurrentTimes { get; set; }

    public TestableSectorTimes LastTimes { get; set; }

    public TestableSectorTimes BestTimes { get; set; }

    public string FrontTyreCompound { get; set; }

    public string RearTyreCompound { get; set; }

    public int? CurrentSector { get; set; }

    public int? StartPosition { get; set; }

    public double? FuelCapacity { get; set; }

    public double? FuelLoad { get; set; }

    public TimeSpan? GetCurrentLapFragmentTime(LapFragmentType lapFragment)
    {
        return CurrentTimes != null ? CurrentTimes?.GetByLapFragmentType(lapFragment) : null;
    }

    public TimeSpan? GetLastLapFragmentTime(LapFragmentType lapFragment)
    {
        return LastTimes != null ? LastTimes?.GetByLapFragmentType(lapFragment) : null;
    }

    public TimeSpan? GetFastestFragmentTime(LapFragmentType lapFragmentType)
    {
        var best = BestTimes?.GetByLapFragmentType(lapFragmentType);
        var current = CurrentTimes?.GetByLapFragmentType(lapFragmentType);
        var last = LastTimes?.GetByLapFragmentType(lapFragmentType);

        if (best == null && current == null && last == null)
        {
            return null;
        }

        if (GapCalculator.IsFasterThan(last, best))
        {
            return GapCalculator.IsFasterThan(current, last) ? current : last;
        }
        else
        {
            return GapCalculator.IsFasterThan(current, best) ? current : best;
        }
    }

    public override bool Equals(object obj)
    {
        // If parameter is null return false.
        if (obj == null)
        {
            return false;
        }

        // If parameter cannot be cast to Point return false.
        if (!(obj is TestableOpponent other))
        {
            return false;
        }

        // Return true if the fields match:
        return Id == other.Id &&
               Manufacturer == other.Manufacturer &&
               Name == other.Name &&
               TeamName == other.TeamName &&
               IsInPit == other.IsInPit &&
               Position == other.Position &&
               CarName == other.CarName &&
               CarClass == other.CarClass &&
               IsPlayer == other.IsPlayer &&
               TrackPositionPercent == other.TrackPositionPercent &&
               CurrentLap == other.CurrentLap &&
               CurrentLapTime == other.CurrentLapTime &&
               CarNumber == other.CarNumber &&
               GapToLeader == other.GapToLeader &&
               CurrentTimes.Equals(other.CurrentTimes) &&
               LastTimes.Equals(other.LastTimes) &&
               BestTimes.Equals(other.BestTimes) &&
               FrontTyreCompound == other.FrontTyreCompound &&
               RearTyreCompound == other.RearTyreCompound &&
               CurrentSector == other.CurrentSector &&
               StartPosition == other.StartPosition &&
               FuelCapacity == other.FuelCapacity &&
               FuelLoad == other.FuelLoad;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public void Log()
    {
        SimHub.Logging.Current.Info("ID:             " + Id);
        SimHub.Logging.Current.Info("Name:           " + Name);
        SimHub.Logging.Current.Info("TeamName:       " + TeamName);
        SimHub.Logging.Current.Info("IsInPit:        " + IsInPit);
        SimHub.Logging.Current.Info("Position:       " + Position);
        SimHub.Logging.Current.Info("Manufacturer:   " + Manufacturer);
        SimHub.Logging.Current.Info("CarName:        " + CarName);
        SimHub.Logging.Current.Info("CarClass:       " + CarClass);
        SimHub.Logging.Current.Info("IsPlayer:       " + IsPlayer);
        SimHub.Logging.Current.Info("TrackPosition%: " + TrackPositionPercent);
        SimHub.Logging.Current.Info("CurrentLap:     " + CurrentLap);
        SimHub.Logging.Current.Info("CurrentLapTime: " + CurrentLapTime);
        SimHub.Logging.Current.Info("CurrentSector:  " + CurrentSector);
        SimHub.Logging.Current.Info("CarNumber:      " + CarNumber);
        SimHub.Logging.Current.Info("GapToLeader:    " + GapToLeader);
        SimHub.Logging.Current.Info("CL S1:          " + CurrentTimes.Sector1);
        SimHub.Logging.Current.Info("CL S2:          " + CurrentTimes.Sector2);
        SimHub.Logging.Current.Info("CL S3:          " + CurrentTimes.Sector3);
        SimHub.Logging.Current.Info("CL LT:          " + CurrentTimes.FullLap);
        SimHub.Logging.Current.Info("LL S1:          " + LastTimes.Sector1);
        SimHub.Logging.Current.Info("LL S2:          " + LastTimes.Sector2);
        SimHub.Logging.Current.Info("LL S3:          " + LastTimes.Sector3);
        SimHub.Logging.Current.Info("LL LT:          " + LastTimes.FullLap);
        SimHub.Logging.Current.Info("B S1:           " + BestTimes.Sector1);
        SimHub.Logging.Current.Info("B S2:           " + BestTimes.Sector2);
        SimHub.Logging.Current.Info("B S3:           " + BestTimes.Sector3);
        SimHub.Logging.Current.Info("B LT:           " + BestTimes.FullLap);
        SimHub.Logging.Current.Info("F Tyre:         " + FrontTyreCompound);
        SimHub.Logging.Current.Info("R Tyre:         " + RearTyreCompound);
        SimHub.Logging.Current.Info("CurrentSector:  " + CurrentSector);
        SimHub.Logging.Current.Info("StartPosition:  " + StartPosition);
        SimHub.Logging.Current.Info("Fuel capacity:  " + FuelCapacity);
        SimHub.Logging.Current.Info("Fuel load:      " + FuelLoad);
    }

    public void LogShort()
    {
        SimHub.Logging.Current.Info("ID:             " + Id);
        SimHub.Logging.Current.Info("Position:       " + Position);
        SimHub.Logging.Current.Info("CurrentLap:     " + CurrentLap);
        SimHub.Logging.Current.Info("GapToLeader:    " + GapToLeader);
        SimHub.Logging.Current.Info("----------------------------------");
    }
}