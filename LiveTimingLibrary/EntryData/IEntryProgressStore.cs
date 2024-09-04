using System.Collections.Generic;

public interface IEntryProgressStore
{
    bool UseCustomGapCalculation();

    EntryProgress GetLastEntryProgress(string entryId);

    EntryProgress GetProgress(string entryId, int lapNumber, int miniSector);

    EntryProgress GetLatestProgress(string entryId, int miniSector);

    List<EntryProgress> GetAllProgressesForEntry(string entryId);

    void AddIfNotAlreadyExists(EntryProgress entryRaceProgress);

    List<string> GetEntryIdsSortedByProgress();

    string CalcGap(string idInFront, string idBehind);
}