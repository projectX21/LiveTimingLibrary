using System.Collections.Generic;

public interface IRaceEventRecoveryFile
{
    void AddEvent(RaceEvent raceEvent);

    List<PitEvent> ReadPitEvents(string sessionId);

    List<PlayerFinishedLapEvent> ReadPlayerFinishedLapEvents(string sessionId);
}