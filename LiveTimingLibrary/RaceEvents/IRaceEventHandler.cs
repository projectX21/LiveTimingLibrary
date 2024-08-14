using System;

public interface IRaceEventHandler
{
    void AddEvent(PitEvent pitEvent);

    void AddEvent(PlayerFinishedLapEvent lapEvent);

    void AddEvent(SessionReloadEvent sessionReloadEvent);

    void SetCurrentLapTime(TimeSpan currentLapTime);

    void ReinitPitEventStore();

    void ReinitPlayerFinishedLapEventStore();

    EntryPitData GetPitDataByEntryId(string entryId);
}