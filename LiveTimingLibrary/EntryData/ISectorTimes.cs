using System;

public interface ISectorTimes
{
    TimeSpan? GetSectorSplit(int sectorIndex);

    TimeSpan? GetLapTime();
}