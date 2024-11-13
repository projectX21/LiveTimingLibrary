using System;
using System.Collections.Generic;

public class PlayerFinishedLapEventStore : IPlayerFinishedLapEventStore
{
    public TimeSpan CurrentLapTime { get; set; } = TimeSpan.Zero;

    private readonly List<PlayerFinishedLapEvent> _lapEvents = new List<PlayerFinishedLapEvent>();

    public TimeSpan CalcTotalElapsedTimeAfterLastCompletedLap()
    {
        var totalTime = TimeSpan.Zero;

        foreach (var lap in _lapEvents)
        {
            totalTime = totalTime.Add(lap.LapTime);
            SimHub.Logging.Current.Debug($"PlayerFinishedLapEventStore::TotalElapsedTimeAfterLastCompletedLap(): added lap (#{lap.LapNumber}, time: {lap.LapTime}), new total time: {totalTime}");
        }

        SimHub.Logging.Current.Debug($"PlayerFinishedLapEventStore::TotalElapsedTimeAfterLastCompletedLap(): final total time: {totalTime}");
        return totalTime;
    }

    public TimeSpan CalcTotalElapsedTimeWithCurrentLapTime()
    {
        var totalTime = TimeSpan.Zero;

        foreach (var lap in _lapEvents)
        {
            totalTime = totalTime.Add(lap.LapTime);
            SimHub.Logging.Current.Debug($"PlayerFinishedLapEventStore::TotalElapsedTimeWithCurrentLapTime(): added lap (#{lap.LapNumber}, time: {lap.LapTime}), new total time: {totalTime}");
        }

        totalTime = totalTime.Add(CurrentLapTime);
        SimHub.Logging.Current.Debug($"PlayerFinishedLapEventStore::TotalElapsedTimeWithCurrentLapTime(): added current lap time {CurrentLapTime}, final total time: {totalTime}");

        return totalTime;
    }

    public TimeSpan CalcReloadTime(int currentLapNumber)
    {
        SimHub.Logging.Current.Debug($"PlayerFinishedLapEventStore::CalcReloadTime(): search for laps with lap number less than ${currentLapNumber}");
        var totalTime = TimeSpan.Zero;

        foreach (var lap in _lapEvents)
        {
            if (lap.LapNumber < currentLapNumber)
            {
                totalTime = totalTime.Add(lap.LapTime);
                SimHub.Logging.Current.Debug($"PlayerFinishedLapEventStore::CalcReloadTime(): added lap (#{lap.LapNumber}, time: {lap.LapTime}), new total time: {totalTime}");
            }

        }

        totalTime = totalTime.Add(CurrentLapTime);
        SimHub.Logging.Current.Debug($"PlayerFinishedLapEventStore::CalcReloadTime(): added current lap time {CurrentLapTime}, final total time: {totalTime}");

        return totalTime;
    }

    public void Reset()
    {
        SimHub.Logging.Current.Debug("PlayerFinishedLapEventStore::Reset(): clear the event store");
        CurrentLapTime = TimeSpan.Zero;
        _lapEvents.Clear();
    }

    public void Add(PlayerFinishedLapEvent finishedLapEvent)
    {
        if (IsAddable(finishedLapEvent))
        {
            SimHub.Logging.Current.Debug($"PlayerFinishedLapEventStore::Add(): add event: {finishedLapEvent}");
            _lapEvents.Add(finishedLapEvent);
        }
    }

    /*
        public void ValidateNewEvent(PlayerFinishedLapEvent finishedLapEvent)
        {
            if (_lapEvents.Count == 0 && finishedLapEvent.LapNumber != 1)
            {
                throw new Exception("PlayerFinishedLapEventStore::ValidateNewEvent(): PlayerFinishedLapEvent is not addable!. First element must have the lap number 1!");
            }
            else if (_lapEvents.Count > 0
                    && finishedLapEvent.LapNumber != (_lapEvents[_lapEvents.Count - 1].LapNumber + 1))
            {
                throw new Exception($"PlayerFinishedLapEventStore::ValidateNewEvent(): PlayerFinishedLapEvent is not addable!. New lap number {finishedLapEvent.LapNumber} is not the consecutive one in the current data (last lap number in store: {_lapEvents[_lapEvents.Count - 1].LapNumber})!");
            }
        }
    */

    public Boolean IsAddable(PlayerFinishedLapEvent finishedLapEvent)
    {
        if (_lapEvents.Count == 0 && finishedLapEvent.LapNumber != 1)
        {
            SimHub.Logging.Current.Warn("PlayerFinishedLapEventStore::ValidateNewEvent(): PlayerFinishedLapEvent is not addable!. First element must have the lap number 1!");
            return false;
        }
        else if (_lapEvents.Count > 0
                && finishedLapEvent.LapNumber != (_lapEvents[_lapEvents.Count - 1].LapNumber + 1))
        {
            SimHub.Logging.Current.Warn($"PlayerFinishedLapEventStore::ValidateNewEvent(): PlayerFinishedLapEvent is not addable!. New lap number {finishedLapEvent.LapNumber} is not the consecutive one in the current data (last lap number in store: {_lapEvents[_lapEvents.Count - 1].LapNumber})!");
            return false;
        }

        return true;
    }
}