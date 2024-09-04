using System;
using log4net.Config;

public class EntryProgress
{
    private string _entryId;

    private int _lapNumber;

    private int _miniSector;

    private TimeSpan _elapsedTime;

    // only used for ordering the entries when my custom comparison by lapNumber/miniSector/elapsedTime is identical
    private int _simHubPosition;

    public EntryProgress(string entryId, int lapNumber, int trackPos, TimeSpan elapsedTime, int simHubPosition)
    {
        _entryId = entryId;
        _lapNumber = lapNumber;
        _miniSector = trackPos;
        _elapsedTime = elapsedTime;
        _simHubPosition = simHubPosition;
    }

    public string GetEntryId()
    {
        return _entryId;
    }

    public int GetLapNumber()
    {
        return _lapNumber;
    }

    public int GetMiniSector()
    {
        return _miniSector;
    }

    public TimeSpan GetElapsedTime()
    {
        return _elapsedTime;
    }

    public int GetSimHubPosition()
    {
        return _simHubPosition;
    }

    public bool IdenticalLapNumberAndMiniSector(EntryProgress other)
    {
        return IdenticalLapNumberAndMiniSector(other._lapNumber, other._miniSector);
    }

    public bool IdenticalLapNumberAndMiniSector(int lapNumber, int miniSector)
    {
        return _lapNumber == lapNumber && _miniSector == miniSector;
    }

    public int CompareTo(EntryProgress other)
    {
        var diffLapNumber = _lapNumber - other._lapNumber;
        var diffMiniSector = _miniSector - other._miniSector;
        var diffElapsedTime = other._elapsedTime.CompareTo(_elapsedTime);

        if (diffLapNumber != 0)
        {
            return diffLapNumber;
        }
        else if (diffMiniSector != 0)
        {
            return diffMiniSector;
        }
        else if (diffElapsedTime != 0)
        {
            return diffElapsedTime;
        }
        else
        {
            return other._simHubPosition - _simHubPosition;
        }
    }


    public override bool Equals(object obj)
    {
        // If parameter is null return false.
        if (obj == null)
        {
            return false;
        }

        // If parameter cannot be cast to Point return false.
        if (!(obj is EntryProgress other))
        {
            return false;
        }

        // Return true if the fields match:
        return _entryId == other._entryId &&
               _lapNumber == other._lapNumber &&
               _miniSector == other._miniSector &&
               _elapsedTime == other._elapsedTime &&
               _simHubPosition == other._simHubPosition;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }


    public override string ToString()
    {
        return $"EntryProgress: entry id: {_entryId}, lap number: {_lapNumber}, mini sector: {_miniSector}, elapsed time: {_elapsedTime}, simhub pos: {_simHubPosition}";
    }
}