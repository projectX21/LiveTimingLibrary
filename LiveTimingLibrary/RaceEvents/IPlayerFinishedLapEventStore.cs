using System;

public interface IPlayerFinishedLapEventStore
{
    TimeSpan CurrentLapTime { get; set; }

    TimeSpan CalcTotalElapsedTimeAfterLastCompletedLap();

    TimeSpan CalcTotalElapsedTimeWithCurrentLapTime();

    TimeSpan CalcReloadTime(int currentLapNumber);

    void Reset();

    void Add(PlayerFinishedLapEvent finishedLapEvent);

    // void ValidateNewEvent(PlayerFinishedLapEvent finishedLapEvent);

    Boolean IsAddable(PlayerFinishedLapEvent finishedLapEvent);
}