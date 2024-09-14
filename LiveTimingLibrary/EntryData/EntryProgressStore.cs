using System.Collections.Generic;
using System.Linq;
using AcTools.Utils.Helpers;

public class EntryProgressStore : IEntryProgressStore
{
    private readonly Dictionary<string, List<EntryProgress>> _entryRaceProgressStore;
    private readonly string _gameName;

    public EntryProgressStore(string gameName)
    {
        _gameName = gameName;
        _entryRaceProgressStore = new Dictionary<string, List<EntryProgress>>();
    }

    public bool UseCustomGapCalculation()
    {
        return _gameName == "AssettoCorsaCompetizione";
    }

    public EntryProgress GetLastEntryProgress(string entryId)
    {
        if (_entryRaceProgressStore.ContainsKey(entryId))
        {
            return _entryRaceProgressStore[entryId].Last();
        }

        return null;
    }

    public EntryProgress GetProgress(string entryId, int lapNumber, int miniSector)
    {
        if (_entryRaceProgressStore.ContainsKey(entryId))
        {
            foreach (var progress in _entryRaceProgressStore[entryId].AsEnumerable().Reverse())
            {
                if (progress.GetLapNumber() == lapNumber && progress.GetMiniSector() == miniSector)
                {
                    return progress;
                }
            }
        }

        return null;
    }

    public EntryProgress GetLatestProgress(string entryId, int miniSector)
    {
        if (_entryRaceProgressStore.ContainsKey(entryId))
        {
            foreach (var progress in _entryRaceProgressStore[entryId].AsEnumerable().Reverse())
            {
                if (progress.GetMiniSector() == miniSector)
                {
                    return progress;
                }
            }
        }

        return null;
    }

    public List<EntryProgress> GetAllProgressesForEntry(string entryId)
    {
        return _entryRaceProgressStore.ContainsKey(entryId)
           ? _entryRaceProgressStore[entryId]
           : null;
    }

    public void AddIfNotAlreadyExists(EntryProgress entryRaceProgress)
    {
        if (!UseCustomGapCalculation())
        {
            return;
        }

        if (!_entryRaceProgressStore.ContainsKey(entryRaceProgress.GetEntryId()))
        {
            _entryRaceProgressStore[entryRaceProgress.GetEntryId()] = new List<EntryProgress>();
        }

        var entryProgresses = _entryRaceProgressStore[entryRaceProgress.GetEntryId()];

        if (!entryProgresses.Any(progress => progress.IdenticalLapNumberAndMiniSector(entryRaceProgress)))
        {
            SimHub.Logging.Current.Debug($"Add race process: {entryRaceProgress}");
            entryProgresses.Add(entryRaceProgress);
            ReorgEntryProgresses(entryProgresses);
        }
    }

    public List<string> GetEntryIdsSortedByProgress()
    {
        return _entryRaceProgressStore
            .ToArray()
            .Sort((a, b) => b.Value.Last().CompareTo(a.Value.Last()))
            .Select(entry => entry.Key).ToList();
    }

    public string CalcGap(string idInFront, string idBehind)
    {
        var lastProgressEntryBehind = GetLastEntryProgress(idBehind);

        // Cannot calculate a gap if the entry behind has not progress yet
        if (lastProgressEntryBehind == null)
        {
            return null;
        }

        var progressEntryInFront = GetLatestProgress(idInFront, lastProgressEntryBehind.GetMiniSector());

        // Same as above, both progress must exist to calculate a gap
        if (progressEntryInFront == null)
        {
            return null;
        }

        // behind is actually in front of InFront
        if (lastProgressEntryBehind.CompareTo(progressEntryInFront) > 0)
        {
            return null;
        }

        var diffInLaps = progressEntryInFront.GetLapNumber() - lastProgressEntryBehind.GetLapNumber();

        if (diffInLaps > 0)
        {
            // entry InFront has more laps and the latest mini sector is the same, but the entry behind has crossed it earlier.
            // Therefore we have to decrement one lap
            if (progressEntryInFront.GetElapsedTime().TotalSeconds > lastProgressEntryBehind.GetElapsedTime().TotalSeconds)
            {
                diffInLaps -= 1;

                if (diffInLaps == 0)
                {
                    // behind isn't lapped yet, therefore we have to fetch the progress of entry in front with the same lap number and mini sector as the entry behind
                    progressEntryInFront = GetProgress(idInFront, lastProgressEntryBehind.GetLapNumber(), lastProgressEntryBehind.GetMiniSector());

                    // TODO Test
                    if (progressEntryInFront == null)
                    {
                        return null;
                    }
                }
            }
        }

        if (diffInLaps > 0)
        {
            return GapCalculator.ToLapGap(diffInLaps);
        }
        else
        {
            var gap = lastProgressEntryBehind.GetElapsedTime().TotalSeconds - progressEntryInFront.GetElapsedTime().TotalSeconds;

            // only display positive gaps
            return gap >= 0 ? GapCalculator.ToTimeGap(gap) : null;
        }
    }

    private void ReorgEntryProgresses(List<EntryProgress> entryProgresses)
    {
        while (entryProgresses.Count() > 90)
        {
            entryProgresses.RemoveAt(0);
        }
    }
}