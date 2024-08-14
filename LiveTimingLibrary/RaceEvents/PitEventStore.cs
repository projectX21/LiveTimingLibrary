using System;
using System.Collections.Generic;
using System.Linq;

public class PitEventStore : IPitEventStore
{
    private Dictionary<string, List<PitEvent>> _entryCache = new Dictionary<string, List<PitEvent>>();

    public EntryPitData GetPitDataByEntryId(string entryId)
    {
        return new EntryPitData(
            entryId,
            CalcTotalNumberOfPitStops(entryId),
            CalcTotalPitDuration(entryId),
            CalcLastPitDuration(entryId),
            CalcLapNumberLastPitOut(entryId)
        );
    }

    public void Reset()
    {
        SimHub.Logging.Current.Debug("PitEventStore::Reset(): clear the event store");
        _entryCache = new Dictionary<string, List<PitEvent>>();
    }

    public void Add(PitEvent pitEvent)
    {
        if (!_entryCache.ContainsKey(pitEvent.EntryId))
        {
            _entryCache.Add(pitEvent.EntryId, new List<PitEvent>());
        }

        ValidateNewEvent(pitEvent);

        SimHub.Logging.Current.Debug($"PitEventStore::Add(): add event: {pitEvent}");
        _entryCache[pitEvent.EntryId].Add(pitEvent);
    }

    public void ValidateNewEvent(PitEvent pitEvent)
    {
        var entryEvents = _entryCache[pitEvent.EntryId];

        if (pitEvent.Type == RaceEventType.PitIn)
        {
            // a PitIn event is only addable when there aren't any pit events yet, or the last one in the current data is a PitOut event
            if (entryEvents.Count > 0 && entryEvents[entryEvents.Count - 1].Type != RaceEventType.PitOut)
            {
                throw new Exception($"PitEventStore::ValidateNewEvent(): PitIn event is not addable: {pitEvent}! Current size of pit events: {entryEvents.Count}, last event type: {RaceEventTypeConverter.FromEnum(entryEvents[entryEvents.Count - 1].Type)}");
            }
        }
        else if (pitEvent.Type == RaceEventType.PitOut)
        {
            // a PitOut event is only addable when there are at least one pit event yet and the last one in the current data is a PitIn event

            if (entryEvents.Count == 0 || entryEvents[entryEvents.Count - 1].Type != RaceEventType.PitIn)
            {
                throw new Exception($"PitEventStore::ValidateNewEvent(): PitOut event is not addable: {pitEvent}! Current size of pit events: {entryEvents.Count}, last event type: {RaceEventTypeConverter.FromEnum(entryEvents[entryEvents.Count - 1].Type)}");
            }

        }
    }

    private int CalcTotalNumberOfPitStops(string entryId)
    {
        return _entryCache.ContainsKey(entryId)
            ? _entryCache[entryId].FindAll(pitEvent => pitEvent.Type == RaceEventType.PitIn).Count
            : 0;
    }

    private int CalcTotalPitDuration(string entryId)
    {
        int currentTotalPitDuration = 0;

        if (_entryCache.ContainsKey(entryId))
        {
            PitEvent lastPitOut = null;

            foreach (PitEvent pitEvent in _entryCache[entryId].AsEnumerable().Reverse())
            {
                if (pitEvent.Type == RaceEventType.PitOut)
                {
                    lastPitOut = pitEvent;
                }
                else if (pitEvent.Type == RaceEventType.PitIn)
                {
                    if (lastPitOut != null)
                    {
                        var pitDuration = (int)lastPitOut.ElapsedTime.TotalSeconds - (int)pitEvent.ElapsedTime.TotalSeconds;
                        currentTotalPitDuration += pitDuration;
                    }
                }
            }
        }

        return currentTotalPitDuration;
    }

    private int? CalcLastPitDuration(string entryId)
    {
        if (_entryCache.ContainsKey(entryId))
        {
            var entryPitEvents = _entryCache[entryId];
            var indexLastPitOut = entryPitEvents.FindLastIndex(e => e.Type == RaceEventType.PitOut);

            if (indexLastPitOut > 0)
            {
                var lastPitIn = entryPitEvents[indexLastPitOut - 1];
                var lastPitOut = entryPitEvents[indexLastPitOut];
                return (int)lastPitOut.ElapsedTime.TotalSeconds - (int)lastPitIn.ElapsedTime.TotalSeconds;
            }
        }

        return null;
    }

    private int? CalcLapNumberLastPitOut(string entryId)
    {
        return _entryCache.ContainsKey(entryId)
            ? _entryCache[entryId].FindLast(e => e.Type == RaceEventType.PitOut)?.LapNumber
            : null;
    }
}