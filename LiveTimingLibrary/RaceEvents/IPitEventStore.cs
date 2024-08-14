public interface IPitEventStore
{
    EntryPitData GetPitDataByEntryId(string entryId);

    void Reset();

    void Add(PitEvent pitEvent);

    void ValidateNewEvent(PitEvent pitEvent);
}