using System;

public class TestableOpponentConverter
{
    public static TestableOpponent FromOpponent(IOpponent o)
    {
        return new TestableOpponent
        {
            Manufacturer = o.Manufacturer,
            Name = o.Name,
            TeamName = o.TeamName,
            IsInPit = o.IsCarInPit || o.IsCarInPitLane || o.StandingStillInPitLane,
            Position = o.Position,
            CarName = o.CarName,
            CarClass = o.CarClass,
            IsPlayer = o.IsPlayer,
            TrackPositionPercent = o.TrackPositionPercent,
            CurrentLap = o.CurrentLap,
            CurrentLapTime = NormalizeTimeSpan(o.CurrentLapTime),
            CurrentSector = o.CurrentSector,
            CarNumber = o.CarNumber,
            GapToLeader = o.GaptoLeader,
            CurrentTimes = new TestableSectorTimes()
            {
                Sector1 = NormalizeTimeSpan(o.CurrentLapSectorTimes?.GetSectorSplit(1)),
                Sector2 = NormalizeTimeSpan(o.CurrentLapSectorTimes?.GetSectorSplit(2)),
            },
            LastTimes = new TestableSectorTimes()
            {
                Sector1 = NormalizeTimeSpan(o.LastLapSectorTimes?.GetSectorSplit(1)),
                Sector2 = NormalizeTimeSpan(o.LastLapSectorTimes?.GetSectorSplit(2)),
                Sector3 = NormalizeTimeSpan(o.LastLapSectorTimes?.GetSectorSplit(3)),
                FullLap = NormalizeTimeSpan(o.LastLapSectorTimes?.GetLapTime())
            },

            BestTimes = new TestableSectorTimes()
            {
                Sector1 = NormalizeTimeSpan(o.BestSectorSplits?.GetSectorSplit(1)),
                Sector2 = NormalizeTimeSpan(o.BestSectorSplits?.GetSectorSplit(2)),
                Sector3 = NormalizeTimeSpan(o.BestSectorSplits?.GetSectorSplit(3)),
                FullLap = NormalizeTimeSpan(o.BestLapTime)
            },
            FrontTyreCompound = o.FrontTyreCompound,
            RearTyreCompound = o.RearTyreCompound,
            StartPosition = o.StartPosition
        };
    }

    private static TimeSpan? NormalizeTimeSpan(TimeSpan? value)
    {
        return value != null && value?.TotalMilliseconds > 0 ? value : null;
    }
}