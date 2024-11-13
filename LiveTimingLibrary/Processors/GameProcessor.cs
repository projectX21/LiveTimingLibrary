using System;
using System.Linq;
using AcTools.Utils.Helpers;

public class GameProcessor : IGameProcessor
{
    public string CurrentGameName { get; private set; }

    protected readonly IPropertyManager _propertyManager;

    protected readonly IRaceEventHandler _raceEventHandler;

    protected readonly IRaceEntryProcessor _raceEntryProcessor;

    protected readonly IEntryProgressStore _entryProgressStore;

    protected TestableGameData _currentGameData;

    protected TestableGameData _previousGameData;

    protected TestableOpponent _currentPlayerData;

    protected TestableOpponent _previousPlayerData;

    protected SessionType _sessionType;

    public GameProcessor(IPropertyManager propertyManager, IRaceEventHandler handler, IRaceEntryProcessor processor, IEntryProgressStore progressStore, string currentGameName, string sessionId)
    {
        _propertyManager = propertyManager;
        _raceEventHandler = handler;
        _raceEntryProcessor = processor;
        CurrentGameName = currentGameName;
        _entryProgressStore = progressStore;

        _propertyManager.ResetAll();
        _raceEventHandler.ReinitPitEventStore(sessionId);
        _raceEventHandler.ReinitPlayerFinishedLapEventStore(sessionId);
    }

    public virtual TestableOpponent[] GetEntries()
    {
        return _currentGameData.Opponents?
            .Sort((a, b) => a.Position - b.Position)
            .ToArray();
    }

    public void Run(TestableGameData gameData)
    {
        _currentGameData = gameData;
        var entries = GetEntries();
        _currentPlayerData = FindPlayer(entries);

        /*
         * ACC has the strange behavior, that it will change the lap number some milliseconds too late when the lap time is already reseted to 0.
         * As a result some calculations afterwards won't work in the correct way and it's better to skip them completely.
         * For example the session relation handling will detect a session reload, because as in the example below, the lap time of lap 2 has changed from 98 to 0.
         *
         * Example:
         * [2024-11-13 21:56:08,387] INFO - Lap: 2 - Lap time 98.087
         * [2024-11-13 21:56:08,404] INFO - Lap: 2 - Lap time 0.012
         * [2024-11-13 21:56:08,420] INFO - Lap: 3 - Lap time 0.027
         */
        if (_currentPlayerData.CurrentLapTime?.TotalSeconds < 0.1)
        {
            return;
        }

        UpdateSessionType();
        UpdateCurrentLapTimeInLapEventStore();

        if (HasSessionIdChanged())
        {
            HandleSessionIdChange();
        }
        else if (WasSessionReloaded())
        {
            HandleSessionReload();
        }
        else
        {
            CreateEventWhenPlayerFinishedLap();
            ProcessEntries(entries);
        }

        _previousGameData = _currentGameData;
        _previousPlayerData = _currentPlayerData;
    }

    private bool HasSessionIdChanged()
    {
        // Don't use OldData.SessionId here, because then it will never notificate for a session change, because the old data isn't filled initially.
        return _previousGameData?.SessionId?.Length > 0 && _previousGameData.SessionId != (_currentGameData.SessionId ?? "");
    }

    private void HandleSessionIdChange()
    {
        SimHub.Logging.Current.Info($"GameProcessor::HandleSessionIdChange(): Session ID has changed from: {_previousGameData.SessionId} to: {_currentGameData.SessionId}. Reset all properties, reinit stores for pit events and player finished lap events");
        _propertyManager.ResetAll();
        _raceEventHandler.ReinitPitEventStore(_currentGameData.SessionId);
        _raceEventHandler.ReinitPlayerFinishedLapEventStore(_currentGameData.SessionId);
    }

    private bool WasSessionReloaded()
    {
        if (_sessionType == SessionType.Race && _previousPlayerData?.CurrentLap != null && _previousPlayerData?.CurrentLapTime != null && _currentPlayerData != null)
        {
            if (_previousPlayerData.CurrentLap > _currentPlayerData.CurrentLap)
            {
                SimHub.Logging.Current.Info($"GameProcessor::WasSessionReloaded(): Reload detected - previous data lap number: {_previousPlayerData.CurrentLap}, current data lap number: {_currentPlayerData.CurrentLap}");
                return true;
            }
            else if (_previousPlayerData.CurrentLap == _currentPlayerData.CurrentLap
                      && _previousPlayerData.CurrentLapTime?.TotalSeconds > _currentPlayerData.CurrentLapTime?.TotalSeconds)
            {
                SimHub.Logging.Current.Info($"GameProcessor::WasSessionReloaded(): Reload detected - same lap number {_currentPlayerData.CurrentLap} - previous data lap time: {_previousPlayerData.CurrentLapTime?.TotalSeconds}, current data lap time: {_currentPlayerData.CurrentLapTime?.TotalSeconds}");
                return true;
            }
        }

        return false;
    }

    private void HandleSessionReload()
    {
        if (_currentPlayerData?.CurrentLap != null)
        {
            _raceEventHandler.AddEvent(new SessionReloadEvent(_currentGameData.SessionId, (int)_currentPlayerData.CurrentLap));
        }
    }

    private void UpdateSessionType()
    {
        _sessionType = SessionTypeConverter.ToEnum(_currentGameData.SessionName);
        _propertyManager.Add(PropertyManagerConstants.SESSION_TYPE, _sessionType.ToString());
    }

    private void UpdateCurrentLapTimeInLapEventStore()
    {
        if (_currentPlayerData?.CurrentLapTime != null)
        {
            SimHub.Logging.Current.Debug($"GameProcessor::UpdateCurrentLapTimeInLapEventStore(): Set current lap time on RaceEventHandler to {_currentPlayerData.CurrentLapTime}");
            _raceEventHandler.SetCurrentLapTime((System.TimeSpan)_currentPlayerData.CurrentLapTime);
        }
    }

    private void CreateEventWhenPlayerFinishedLap()
    {
        if (_sessionType != SessionType.Race)
        {
            return;
        }

        var oldData = FindOldDataById(_currentPlayerData.Id);

        if (oldData == null)
        {
            return;
        }

        if (oldData.CurrentLap < _currentPlayerData.CurrentLap && _currentPlayerData.CurrentLap > 1)
        {
            SimHub.Logging.Current.Info($"GameProcessor::CreateEventWhenPlayerFinishedLap(): Lap changed from {oldData.CurrentLap} to {_currentPlayerData.CurrentLap}");

            _raceEventHandler.AddEvent(
                new PlayerFinishedLapEvent(
                    _currentGameData.SessionId,
                    _currentPlayerData.CurrentLap - 1 ?? 1,
                    _currentPlayerData.LastTimes.GetByLapFragmentType(LapFragmentType.FULL_LAP) ?? TimeSpan.Zero
                )
            );
        }
    }

    private void ProcessEntries(TestableOpponent[] entries)
    {
        var fastestSectorTimes = new FastestFragmentTimesStore(entries);

        if (_sessionType == SessionType.Race && _entryProgressStore.UseCustomGapCalculation())
        {
            entries = PrepareCustomScoring(entries);
        }

        for (var i = 0; i < entries.Length; i++)
        {
            _raceEntryProcessor.Process(
                _currentGameData.SessionId,
                _sessionType,
                i + 1,
                entries[i],
                FindOldDataById(entries[i].Id),
                i > 0 ? entries[0] : null,
                i > 0 ? entries[i - 1] : null,
                fastestSectorTimes,
                _entryProgressStore
            );
        }
    }

    private TestableOpponent FindPlayer(TestableOpponent[] entries)
    {
        var result = entries.Where(o => o.IsPlayer == true);

        if (result.Count() > 0)
        {
            return result.First();
        }

        throw new Exception($"GameProcessor::FindPlayer(): Could not find player data");
    }

    private TestableOpponent FindOldDataById(string id)
    {
        if (_previousGameData?.Opponents != null && _previousGameData.Opponents.Length > 0)
        {
            var result = _previousGameData.Opponents.Where(o => o.Id == id);

            if (result.Count() > 0)
            {
                return result.First();
            }
        }

        SimHub.Logging.Current.Debug($"GameProcessor::FindOldDataById(): Could not find OldData for entry with id: {id}");
        return null;
    }

    private TestableOpponent[] PrepareCustomScoring(TestableOpponent[] entries)
    {
        var totalElapsedTime = _raceEventHandler.GetElapsedSessionTime();

        for (var i = 0; i < entries.Length; i++)
        {
            AddToEntryRaceProgressStore(entries[i], totalElapsedTime);
        }

        var sortedEntryIds = _entryProgressStore.GetEntryIdsSortedByProgress();

        return entries.Sort((a, b) =>
        {
            var aIndex = sortedEntryIds.FindIndex(e => e == a.Id);
            var bIndex = sortedEntryIds.FindIndex(e => e == b.Id);
            return aIndex - bIndex;
        }).ToArray();
    }

    private void AddToEntryRaceProgressStore(TestableOpponent entry, TimeSpan totalElapsedTime)
    {
        if (entry == null || entry.TrackPositionPercent == null || totalElapsedTime == TimeSpan.Zero)
        {
            return;
        }

        var miniSector = (int)(entry.TrackPositionPercent * 30);
        SimHub.Logging.Current.Debug($"Process entry with id: {entry.Id} on position: {entry.Position}, gap to leader: {entry.GapToLeader}, mini sector: {miniSector}");
        _entryProgressStore.AddIfNotAlreadyExists(new EntryProgress(entry.Id, entry.CurrentLap ?? 1, miniSector, totalElapsedTime, entry.Position));
    }
}