using System.Linq;
using AcTools.Utils.Helpers;

public class GameProcessor : IGameProcessor
{
    public string CurrentGameName { get; private set; }

    protected readonly IPropertyManager _propertyManager;

    protected readonly IRaceEventHandler _raceEventHandler;

    protected readonly IRaceEntryProcessor _raceEntryProcessor;

    protected TestableGameData _currentGameData;

    protected SessionType _sessionType;

    private string _lastSessionId;

    public GameProcessor(IPropertyManager propertyManager, IRaceEventHandler handler, IRaceEntryProcessor processor, string currentGameName)
    {
        _propertyManager = propertyManager;
        _raceEventHandler = handler;
        _raceEntryProcessor = processor;
        CurrentGameName = currentGameName;
    }

    public virtual TestableOpponent[] GetEntries()
    {
        return _currentGameData.NewData.Opponents
            .Sort((a, b) => a.Position - b.Position)
            .ToArray();
    }

    public void Run(TestableGameData gameData)
    {
        if (!gameData.GameRunning)
        {
            SimHub.Logging.Current.Debug("GameProcessor::Run(): Omit calculation cycle");
            return;
        }

        _currentGameData = gameData;
        UpdateSessionType();
        UpdateCurrentLapTimeInLapEventStore();
        var entries = GetEntries().Select(e => NormalizeEntryData(e, FindOldDataById(e.Id))).ToArray();

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
            ProcessEntries(entries);
        }

        _lastSessionId = gameData.NewData.SessionId;
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
        _raceEventHandler.ReinitPitEventStore();
        _raceEventHandler.ReinitPlayerFinishedLapEventStore();
    }

    private bool WasSessionReloaded()
    {
        return _currentGameData.OldData != null && _currentGameData.NewData != null &&
            (
                _currentGameData.OldData.CurrentLap > _currentGameData.NewData.CurrentLap ||
                (
                    _currentGameData.OldData.CurrentLap == _currentGameData.NewData.CurrentLap
                    && _currentGameData.OldData.CurrentLapTime.TotalSeconds > _currentGameData.NewData.CurrentLapTime.TotalSeconds
                )
            )
        ;
    }

    private void HandleSessionReload()
    {
        _raceEventHandler.AddEvent(new SessionReloadEvent(_currentGameData.NewData.CurrentLap));
    }

    private void UpdateSessionType()
    {
        _sessionType = SessionTypeConverter.ToEnum(_currentGameData.NewData.SessionName);
        _propertyManager.Add(PropertyManagerConstants.SESSION_TYPE, _sessionType.ToString());
    }

    private void UpdateCurrentLapTimeInLapEventStore()
    {
        SimHub.Logging.Current.Debug($"GameProcessor::UpdateCurrentLapTimeInLapEventStore(): Set current lap time on RaceEventHandler to {_currentGameData.NewData.CurrentLapTime}");
        _raceEventHandler.SetCurrentLapTime(_currentGameData.NewData.CurrentLapTime);
    }

    private void ProcessEntries(TestableOpponent[] entries)
    {
        var fastestSectorTimes = new FastestFragmentTimesStore(entries);

        for (var i = 0; i < entries.Length; i++)
        {
            _raceEntryProcessor.Process(
                _sessionType,
                entries[i],
                FindOldDataById(entries[i].Id),
                i > 0 ? entries[0] : null,
                i > 0 ? entries[i - 1] : null,
                fastestSectorTimes
            );
        }
    }

    private TestableOpponent FindOldDataById(string id)
    {
        if (_currentGameData.OldData?.Opponents != null && _currentGameData.OldData.Opponents.Length > 0)
        {
            var result = _currentGameData.OldData.Opponents.Where(o => o.Id == id);

            if (result.Count() > 0)
            {
                return result.First();
            }
        }

        SimHub.Logging.Current.Info($"GameProcessor::FindOldDataById(): Could not find OldData for entry with id: {id}");
        return null;
    }

    private TestableOpponent NormalizeEntryData(TestableOpponent newData, TestableOpponent oldData)
    {
        if (newData.CurrentLap == oldData?.CurrentLap && newData.CurrentSector < oldData?.CurrentSector)
        {
            SimHub.Logging.Current.Info($"GameProcessor::NormalizeEntryData(): OldData is newer than NewData for entry with id: {newData.Id}");
            return oldData;
        }

        return newData;
    }
}