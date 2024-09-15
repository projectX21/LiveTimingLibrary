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

    protected SessionType _sessionType;

    private string _lastSessionId;

    private int? _lastCurrentLap;

    private TimeSpan? _lastCurrentLapTime;

    public GameProcessor(IPropertyManager propertyManager, IRaceEventHandler handler, IRaceEntryProcessor processor, IEntryProgressStore progressStore, string currentGameName, string sessionId)
    {
        _propertyManager = propertyManager;
        _raceEventHandler = handler;
        _raceEntryProcessor = processor;
        CurrentGameName = currentGameName;
        _entryProgressStore = progressStore;

        _raceEventHandler.ReinitPitEventStore(sessionId);
        _raceEventHandler.ReinitPlayerFinishedLapEventStore(sessionId);
    }

    public virtual TestableOpponent[] GetEntries()
    {
        return _currentGameData.NewData.Opponents
            .Sort((a, b) => a.Position - b.Position)
            .ToArray();
    }

    public void Run(TestableGameData gameData)
    {
        _currentGameData = gameData;
        var entries = GetEntries().Select(e => NormalizeEntryData(e, FindOldDataById(e.Id))).ToArray();
        _currentPlayerData = FindPlayer(entries);

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

        _lastSessionId = gameData.NewData.SessionId;
        _lastCurrentLap = _currentPlayerData.CurrentLap;
        _lastCurrentLapTime = _currentPlayerData.CurrentLapTime;
        _previousGameData = _currentGameData;
    }

    private bool HasSessionIdChanged()
    {
        // Don't use OldData.SessionId here, because then it will never notificate for a session change, because the old data isn't filled initially.
        return _lastSessionId?.Length > 0 && _lastSessionId != (_currentGameData.NewData?.SessionId ?? "");
    }

    private void HandleSessionIdChange()
    {
        SimHub.Logging.Current.Info($"GameProcessor::HandleSessionIdChange(): Session ID has changed from: {_lastSessionId} to: {_currentGameData.NewData.SessionId}. Reset all properties, reinit stores for pit events and player finished lap events");
        _propertyManager.ResetAll();
        _raceEventHandler.ReinitPitEventStore(_currentGameData.NewData.SessionId);
        _raceEventHandler.ReinitPlayerFinishedLapEventStore(_currentGameData.NewData.SessionId);
    }

    private bool WasSessionReloaded()
    {
        return _sessionType == SessionType.Race && _lastCurrentLap != null && _lastCurrentLapTime != null && _currentPlayerData != null &&
            (
                _lastCurrentLap > _currentPlayerData.CurrentLap ||
                (
                    _lastCurrentLap == _currentPlayerData.CurrentLap
                    && _lastCurrentLapTime?.TotalSeconds > _currentPlayerData.CurrentLapTime?.TotalSeconds
                )
            )
        ;
    }

    private void HandleSessionReload()
    {
        if (_currentPlayerData?.CurrentLap != null)
        {
            _raceEventHandler.AddEvent(new SessionReloadEvent(_currentGameData.NewData.SessionId, (int)_currentPlayerData.CurrentLap));
        }
    }

    private void UpdateSessionType()
    {
        _sessionType = SessionTypeConverter.ToEnum(_currentGameData.NewData.SessionName);
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
        if (_currentPlayerData == null)
        {
            return;
        }

        var oldData = FindOldDataById(_currentPlayerData.Id);

        if (oldData == null)
        {
            return;
        }

        if (oldData.CurrentLap < _currentPlayerData.CurrentLap && _currentPlayerData.CurrentLap > 0)
        {
            _raceEventHandler.AddEvent(
                new PlayerFinishedLapEvent(
                    _currentGameData.NewData.SessionId,
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
                _currentGameData.NewData.SessionId,
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
        if (_previousGameData?.NewData?.Opponents != null && _previousGameData.NewData.Opponents.Length > 0)
        {
            var result = _previousGameData.NewData.Opponents.Where(o => o.Id == id);

            if (result.Count() > 0)
            {
                return result.First();
            }
        }

        SimHub.Logging.Current.Debug($"GameProcessor::FindOldDataById(): Could not find OldData for entry with id: {id}");
        return null;
    }

    private TestableOpponent NormalizeEntryData(TestableOpponent newData, TestableOpponent oldData)
    {
        if (newData.CurrentLap == oldData?.CurrentLap
            && (newData.CurrentSector < oldData?.CurrentSector || newData.CurrentLapTime?.TotalSeconds < oldData?.CurrentLapTime?.TotalSeconds))
        {
            SimHub.Logging.Current.Debug($"GameProcessor::NormalizeEntryData(): OldData is newer than NewData for entry with id: {newData.Id}");
            return oldData;
        }

        return newData;
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