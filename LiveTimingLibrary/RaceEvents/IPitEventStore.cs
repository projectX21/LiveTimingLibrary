using System;

public interface IPitEventStore
{
    EntryPitData GetPitDataByEntryId(string entryId);

    void Reset();

    void Add(PitEvent pitEvent);

    // void ValidateNewEvent(PitEvent pitEvent);

    Boolean IsAddable(PitEvent pitEvent);
}