using System;

public interface IOpponent
{
    string Manufacturer { get; set; }

    string Name { get; set; }

    string TeamName { get; set; }

    bool IsCarInPit { get; set; }

    bool IsCarInPitLane { get; set; }

    bool StandingStillInPitLane { get; set; }

    int Position { get; set; }

    string CarName { get; set; }

    string CarClass { get; set; }

    bool IsPlayer { get; set; }

    double? TrackPositionPercent { get; set; }

    int? CurrentLap { get; set; }

    TimeSpan? CurrentLapTime { get; set; }

    int? CurrentSector { get; set; }

    string CarNumber { get; set; }

    double? GaptoLeader { get; set; }

    TimeSpan BestLapTime { get; set; }

    string FrontTyreCompound { get; set; }

    string RearTyreCompound { get; set; }

    int? StartPosition { get; set; }

    ISectorTimes CurrentLapSectorTimes { get; set; }

    ISectorTimes LastLapSectorTimes { get; set; }

    ISectorTimes BestSectorSplits { get; set; }
}