using System;

public class RaceEventHandler : IRaceEventHandler
{
    private readonly IRaceEventRecoveryFile _recoveryFile;

    private readonly IPitEventStore _pitEventStore;

    private readonly IPlayerFinishedLapEventStore _lapEventStore;

    public RaceEventHandler(IRaceEventRecoveryFile file, IPitEventStore pitStore, IPlayerFinishedLapEventStore lapStore)
    {
        _recoveryFile = file;
        _pitEventStore = pitStore;
        _lapEventStore = lapStore;
    }

    public void AddEvent(PitEvent pitEvent)
    {
        pitEvent.ElapsedTime = _lapEventStore.CalcTotalElapsedTimeWithCurrentLapTime();
        SimHub.Logging.Current.Debug($"RaceEventHandler::AddEvent(): add PitEvent: {pitEvent}");

        _recoveryFile.AddEvent(pitEvent);
        _pitEventStore.Add(pitEvent);
    }

    public void AddEvent(PlayerFinishedLapEvent lapEvent)
    {
        lapEvent.ElapsedTime = _lapEventStore.CalcTotalElapsedTimeAfterLastCompletedLap() + lapEvent.LapTime;
        SimHub.Logging.Current.Debug($"RaceEventHandler::AddEvent(): add PlayerFinishedLapEvent: {lapEvent}");

        _recoveryFile.AddEvent(lapEvent);
        _lapEventStore.Add(lapEvent);
    }

    public void AddEvent(SessionReloadEvent sessionReloadEvent)
    {
        sessionReloadEvent.ElapsedTime = _lapEventStore.CalcReloadTime(sessionReloadEvent.LapNumber);
        SimHub.Logging.Current.Debug($"RaceEventHandler::AddEvent(): add SessionReloadEvent: {sessionReloadEvent}");

        _recoveryFile.AddEvent(sessionReloadEvent);
        ReinitPitEventStore(sessionReloadEvent.SessionId);
        ReinitPlayerFinishedLapEventStore(sessionReloadEvent.SessionId);
    }

    public void SetCurrentLapTime(TimeSpan currentLapTime)
    {
        _lapEventStore.CurrentLapTime = currentLapTime;
    }

    public TimeSpan GetElapsedSessionTime()
    {
        return _lapEventStore.CalcTotalElapsedTimeWithCurrentLapTime();
    }

    public void ReinitPitEventStore(string sessionId)
    {
        _pitEventStore.Reset();
        _recoveryFile.ReadPitEvents(sessionId).ForEach(_pitEventStore.Add);
    }

    public void ReinitPlayerFinishedLapEventStore(string sessionId)
    {
        _lapEventStore.Reset();
        _recoveryFile.ReadPlayerFinishedLapEvents(sessionId).ForEach(_lapEventStore.Add);
    }

    public EntryPitData GetPitDataByEntryId(string entryId)
    {
        return _pitEventStore.GetPitDataByEntryId(entryId);
    }
}