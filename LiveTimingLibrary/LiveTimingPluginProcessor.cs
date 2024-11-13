using System;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("LiveTimingLibrary.Tests")]
public class LiveTimingPluginProcessor
{
    public IGameProcessor GameProcessor { get; internal set; }

    private readonly IPropertyManager _propertyManager;

    private readonly IRaceEventHandler _raceEventHandler;

    private readonly IRaceEntryProcessor _raceEntryProcessor;

    public LiveTimingPluginProcessor(IPropertyManager propertyManager, IRaceEventHandler raceEventHandler, IRaceEntryProcessor raceEntryProcessor)
    {
        _propertyManager = propertyManager;
        _raceEventHandler = raceEventHandler;
        _raceEntryProcessor = raceEntryProcessor;

    }

    public void DataUpdate(TestableGameData gameData)
    {
        if (!gameData.GameRunning || gameData.GamePaused)
        {
            SimHub.Logging.Current.Debug("GameProcessor::Run(): Omit calculation cycle");
            return;
        }
        else if (gameData.Opponents?.Count() <= 1)
        {
            SimHub.Logging.Current.Debug("LiveTimingPluginProcessor::DataUpdate(): Opponents aren't set correctly");
            return;
        }

        if (GameProcessor?.CurrentGameName != gameData.GameName)
        {
            SimHub.Logging.Current.Info($"LiveTimingPluginProcessor::DataUpdate(): GameName has changed {GameProcessor?.CurrentGameName} to {gameData.GameName}");
            GameProcessor = new GameProcessor(_propertyManager, _raceEventHandler, _raceEntryProcessor, new EntryProgressStore(gameData.GameName), gameData.GameName, SessionIdGenerator.Generate(gameData));
        }

        GameProcessor.Run(gameData);
    }
}