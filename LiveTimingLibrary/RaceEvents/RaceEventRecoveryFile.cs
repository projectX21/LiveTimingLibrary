using System;
using System.Collections.Generic;
using System.IO;

public class RaceEventRecoveryFile : IRaceEventRecoveryFile
{
    private readonly IRaceEventRecoveryFileEventSelector<PitEvent> _pitEventSelector;

    private readonly IRaceEventRecoveryFileEventSelector<PlayerFinishedLapEvent> _finishedLapEventSelector;

    private readonly string _filePath;

    private static readonly string s_defaultFilePath = @"C:\Users\chris\Documents\simhub\race-event-recovery.csv";

    public RaceEventRecoveryFile(IRaceEventRecoveryFileEventSelector<PitEvent> pitEventSelector, IRaceEventRecoveryFileEventSelector<PlayerFinishedLapEvent> lapEventSelector)
    {
        _filePath = s_defaultFilePath;
        _pitEventSelector = pitEventSelector;
        _finishedLapEventSelector = lapEventSelector;
        SimHub.Logging.Current.Debug($"RaceEventRecoveryFile(): use {_filePath} as the recovery file path");
    }

    public RaceEventRecoveryFile(IRaceEventRecoveryFileEventSelector<PitEvent> pitEventSelector, IRaceEventRecoveryFileEventSelector<PlayerFinishedLapEvent> lapEventSelector, string path)
    {
        _pitEventSelector = pitEventSelector;
        _finishedLapEventSelector = lapEventSelector;
        _filePath = path;
        SimHub.Logging.Current.Debug($"RaceEventRecoveryFile(): use {_filePath} as the recovery file path");
    }

    public void AddEvent(RaceEvent raceEvent)
    {
        Write(raceEvent.ToRecoveryFileFormat());
    }

    public List<PitEvent> ReadPitEvents()
    {
        var pitEvents = _pitEventSelector.SelectSpecificEvents(_filePath);
        SimHub.Logging.Current.Debug($"RaceEventRecoveryFile::ReadPitEvents(): found {pitEvents.Count} events");
        return pitEvents;
    }

    public List<PlayerFinishedLapEvent> ReadPlayerFinishedLapEvents()
    {
        var playerFinishedLapEvents = _finishedLapEventSelector.SelectSpecificEvents(_filePath);
        SimHub.Logging.Current.Debug($"RaceEventRecoveryFile::ReadPlayerFinishedLapEvents(): found {playerFinishedLapEvents.Count} events");
        return playerFinishedLapEvents;
    }

    private void Write(string recoveryFileFormat)
    {
        SimHub.Logging.Current.Debug($"RaceEventRecoveryFile::Write(): write '{recoveryFileFormat}' into recovery file");
        File.AppendAllText(_filePath, recoveryFileFormat + Environment.NewLine);
    }
}