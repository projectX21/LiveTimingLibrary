using System;

public interface IFastestFragmentTimesStore
{
    string GetCurrentLapFragmentTimeIndicator(TestableOpponent entry, LapFragmentType lapFragmentType);

    string GetLastLapFragmentTimeIndicator(TestableOpponent entry, LapFragmentType lapFragmentType);

    string GetFragmentTimeIndicator(TestableOpponent entry, TimeSpan? checkTime, LapFragmentType lapFragmentType);

    bool IsFastestInClass(string carClass, LapFragmentType lapFragmentType, TimeSpan? checkTime);

    bool IsFastestForEntry(string entryId, LapFragmentType lapFragmentType, TimeSpan? checkTime);
}