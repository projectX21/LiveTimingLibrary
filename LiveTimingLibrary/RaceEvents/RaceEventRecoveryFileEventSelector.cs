using System;
using System.Collections.Generic;
using System.IO;

public class RaceEventRecoveryFileEventSelector<T> : IRaceEventRecoveryFileEventSelector<T> where T : RaceEvent
{
    public List<T> SelectSpecificEvents(string filePath)
    {
        if (!File.Exists(filePath))
        {
            SimHub.Logging.Current.Debug("RaceEventRecoveryFileEventSelector::SelectSpecificEvents(): no events yet");
            return new List<T>();
        }

        var lines = File.ReadAllLines(filePath);
        var events = new List<T>();
        TimeSpan? elapsedTimeLastReload = null;

        // read the recovery file in reverse order
        for (var i = lines.Length - 1; i >= 0; i--)
        {
            var raceEvent = ToRaceEvent(lines[i]);

            if (raceEvent is SessionReloadEvent @sessionEvent)
            {
                elapsedTimeLastReload = @sessionEvent.ElapsedTime;
                SimHub.Logging.Current.Debug($"RaceEventRecoveryFileEventSelector::SelectSpecificEvents(): set elapsedTimeLastReload to: {elapsedTimeLastReload}");
            }
            else if (raceEvent is T @specificEvent)
            {
                if (elapsedTimeLastReload == null || @specificEvent.ElapsedTime < elapsedTimeLastReload)
                {
                    SimHub.Logging.Current.Debug($"RaceEventRecoveryFileEventSelector::SelectSpecificEvents(): add PitEvent: {lines[i]}");
                    events.Add(@specificEvent);
                }
            }
        }

        // the file was read in reverse order, therefore it has to reversed again to have the initial order.
        events.Reverse();
        return events;
    }

    private RaceEvent ToRaceEvent(string line)
    {
        if (PitEvent.Matches(line))
        {
            return new PitEvent(line);
        }
        else if (SessionReloadEvent.Matches(line))
        {
            return new SessionReloadEvent(line);
        }
        else if (PlayerFinishedLapEvent.Matches(line))
        {
            return new PlayerFinishedLapEvent(line);
        }
        else
        {
            throw new Exception("RaceEventRecoveryFileEventSelector::ToRaceEvent(): cannot parse line into RaceEvent: " + line);
        }
    }
}