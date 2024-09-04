using System;

public interface IRaceEventHandler
{
    void AddEvent(PitEvent pitEvent);

    void AddEvent(PlayerFinishedLapEvent lapEvent);

    void AddEvent(SessionReloadEvent sessionReloadEvent);

    void SetCurrentLapTime(TimeSpan currentLapTime);

    TimeSpan GetElapsedSessionTime();

    void ReinitPitEventStore(string sessionId);

    void ReinitPlayerFinishedLapEventStore(string sessionId);

    EntryPitData GetPitDataByEntryId(string entryId);
}