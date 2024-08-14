using System;

public class GapCalculator
{
    public static string Calc(SessionType sessionType, TestableOpponent behind, TestableOpponent inFront)
    {
        // Don't calculate the gap when any of the two entries are null
        if (behind == null || inFront == null)
        {
            return null;
        }

        return sessionType == SessionType.Race
            ? CalcRaceGap(behind, inFront)
            : CalcBestLapGap(behind, inFront);
    }

    public static string CalcRaceGap(TestableOpponent behind, TestableOpponent inFront)
    {
        // Don't calculate the gap when any of the two entries is in the pits
        if (behind.IsInPit || inFront.IsInPit)
        {
            return null;
        }


        var diffLaps = inFront.CurrentLap - behind.CurrentLap;

        if (NormalizeTrackPosition(behind.TrackPositionPercent) > NormalizeTrackPosition(inFront.TrackPositionPercent))
        {
            diffLaps--;
        }

        if (diffLaps > 0)
        {
            return $"+{diffLaps}L";
        }
        else
        {
            var gap = NormalizeGapToLeader(behind.GapToLeader) - NormalizeGapToLeader(inFront.GapToLeader);
            return gap > 0.0 ? $"+{TimeSpanFormatter.Format(TimeSpan.FromSeconds(gap))}" : "";
        }
    }

    public static string CalcBestLapGap(TestableOpponent behind, TestableOpponent inFront)
    {
        var behindBestLap = behind.BestTimes.GetByLapFragmentType(LapFragmentType.FULL_LAP);
        var inFrontBestLap = inFront.BestTimes.GetByLapFragmentType(LapFragmentType.FULL_LAP);

        if (behindBestLap == null || inFrontBestLap == null)
        {
            return null;
        }

        TimeSpan gap = ((TimeSpan)behindBestLap).Subtract((TimeSpan)inFrontBestLap);
        return gap.Milliseconds > 0 ? $"+{TimeSpanFormatter.Format(gap)}" : "";
    }

    public static bool IsFasterThan(TimeSpan? time1, TimeSpan? time2)
    {
        if (time1 != null && time2 != null)
        {
            return time1?.TotalMilliseconds < time2?.TotalMilliseconds;
        }
        else if (time1 != null)
        {
            return true;
        }

        return false;
    }

    private static double NormalizeTrackPosition(double? trackPos)
    {
        if (trackPos != null && trackPos < 1)
        {
            return trackPos ?? 0.0; // why is the compiler not smart enough??
        }

        return 0.0;
    }

    private static double NormalizeGapToLeader(double? gapToLeader)
    {
        if (gapToLeader == null)
        {
            return 0.0;
        }

        // same games have negative gaps to the leader, but they should always be positive in this code
        return (double)gapToLeader * (gapToLeader < 0 ? -1 : 1);
    }
}