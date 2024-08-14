using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("LiveTimingLibrary.Tests")]
public class LiveTimingPluginProcessor
{
    public IGameProcessor GameProcessor { get; internal set; }

    private readonly IPropertyManager _propertyManager;

    private readonly IRaceEventHandler _raceEventHandler;

    private readonly IRaceEntryProcessor _raceEntryProcessor;

    private Guid _lastSessionId;

    public LiveTimingPluginProcessor(IPropertyManager propertyManager, IRaceEventHandler raceEventHandler, IRaceEntryProcessor raceEntryProcessor)
    {
        _propertyManager = propertyManager;
        _raceEventHandler = raceEventHandler;
        _raceEntryProcessor = raceEntryProcessor;

    }

    public void DataUpdate(TestableGameData gameData, Guid sessionId)
    {
        if (gameData.NewData == null)
        {
            SimHub.Logging.Current.Debug("LiveTimingPluginProcessor::DataUpdate(): NewData isn't filled");
            return;
        }

        if (GameProcessor?.CurrentGameName != gameData.GameName)
        {
            SimHub.Logging.Current.Info($"LiveTimingPluginProcessor::DataUpdate(): GameName has changed {GameProcessor?.CurrentGameName} to {gameData.GameName}");
            GameProcessor = new GameProcessor(_propertyManager, _raceEventHandler, _raceEntryProcessor, gameData.GameName);
        }

        SetSessionId(gameData, sessionId);
        GameProcessor.Run(gameData);

        _lastSessionId = sessionId;
    }

    private void SetSessionId(TestableGameData gameData, Guid sessionId)
    {
        if (gameData.NewData != null)
        {
            gameData.NewData.SessionId = sessionId;
        }

        if (gameData.OldData != null)
        {
            gameData.OldData.SessionId = _lastSessionId;
        }
    }
}