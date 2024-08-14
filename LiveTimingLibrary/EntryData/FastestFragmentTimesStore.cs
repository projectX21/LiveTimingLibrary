using System;
using System.Collections.Generic;
using System.Linq;

public class FastestFragmentTimesStore : IFastestFragmentTimesStore
{
    private readonly Dictionary<string, TestableSectorTimes> _fastestByClass;
    private readonly Dictionary<string, TestableSectorTimes> _fastestByEntry;

    public FastestFragmentTimesStore(TestableOpponent[] entries)
    {
        _fastestByClass = CreateFastestByClass(entries);
        _fastestByEntry = CreateFastestByEntry(entries);
    }

    public string GetCurrentLapFragmentTimeIndicator(TestableOpponent entry, LapFragmentType lapFragmentType)
    {
        return GetFragmentTimeIndicator(entry, entry.GetCurrentLapFragmentTime(lapFragmentType), lapFragmentType);
    }

    public string GetLastLapFragmentTimeIndicator(TestableOpponent entry, LapFragmentType lapFragmentType)
    {
        return GetFragmentTimeIndicator(entry, entry.GetLastLapFragmentTime(lapFragmentType), lapFragmentType);
    }

    public string GetFragmentTimeIndicator(TestableOpponent entry, TimeSpan? checkTime, LapFragmentType lapFragmentType)
    {
        if (checkTime != null && checkTime?.TotalMilliseconds > 0)
        {
            if (IsFastestInClass(entry.CarClass, lapFragmentType, checkTime))
            {
                return "CLASS_BEST";
            }
            else if (IsFastestForEntry(entry.Id, lapFragmentType, checkTime))
            {
                return "ENTRY_BEST";
            }
        }

        return "";
    }

    public bool IsFastestInClass(string carClass, LapFragmentType lapFragmentType, TimeSpan? checkTime)
    {
        var filledClassName = GetFilledCarClassName(carClass);

        if (!_fastestByClass.ContainsKey(filledClassName))
        {
            throw new Exception("Could not find car class for determining fastest lap fragment time: " + carClass + "!");
        }

        return _fastestByClass[filledClassName].GetByLapFragmentType(lapFragmentType) == checkTime;
    }

    public bool IsFastestForEntry(string entryId, LapFragmentType lapFragmentType, TimeSpan? checkTime)
    {
        if (!_fastestByEntry.ContainsKey(entryId))
        {
            throw new Exception("Could not find entry for determining fastest lap fragment time: " + entryId + "!");
        }

        return _fastestByEntry[entryId].GetByLapFragmentType(lapFragmentType) == checkTime;
    }

    private Dictionary<string, TestableSectorTimes> CreateFastestByClass(TestableOpponent[] entries)
    {
        var fastestByClass = new Dictionary<string, TestableSectorTimes>();

        foreach (var carClass in entries.Select(e => GetFilledCarClassName(e.CarClass)).Distinct())
        {
            fastestByClass.Add(carClass,
                new TestableSectorTimes
                {
                    Sector1 = GetFastestFragmentTimeInClass(entries, carClass, LapFragmentType.SECTOR_1),
                    Sector2 = GetFastestFragmentTimeInClass(entries, carClass, LapFragmentType.SECTOR_2),
                    Sector3 = GetFastestFragmentTimeInClass(entries, carClass, LapFragmentType.SECTOR_3),
                    FullLap = GetFastestFragmentTimeInClass(entries, carClass, LapFragmentType.FULL_LAP),
                }
            );
        }

        return fastestByClass;
    }

    private Dictionary<string, TestableSectorTimes> CreateFastestByEntry(TestableOpponent[] entries)
    {
        var fastestByEntry = new Dictionary<string, TestableSectorTimes>();

        foreach (var entry in entries)
        {
            fastestByEntry.Add(
                entry.Id,
                new TestableSectorTimes
                {
                    Sector1 = entry.GetFastestFragmentTime(LapFragmentType.SECTOR_1),
                    Sector2 = entry.GetFastestFragmentTime(LapFragmentType.SECTOR_2),
                    Sector3 = entry.GetFastestFragmentTime(LapFragmentType.SECTOR_3),
                    FullLap = entry.GetFastestFragmentTime(LapFragmentType.FULL_LAP),
                }
            );
        }

        return fastestByEntry;
    }

    private static TimeSpan? GetFastestFragmentTimeInClass(TestableOpponent[] entries, string carClass, LapFragmentType lapFragmentType)
    {
        return entries.Where(e => GetFilledCarClassName(e.CarClass) == carClass).Select(e => e.GetFastestFragmentTime(lapFragmentType)).Min();
    }

    private static string GetFilledCarClassName(string carClass)
    {
        return carClass?.Length > 0 ? carClass : "unknown";
    }


}