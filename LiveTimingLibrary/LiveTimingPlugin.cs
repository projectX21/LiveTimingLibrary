using GameReaderCommon;
using SimHub.Plugins;

namespace LiveTimingLibrary
{
    [PluginDescription("Live Timing")]
    [PluginAuthor("Cebra")]
    [PluginName("Live Timing")]
    public class LiveTimingPlugin : IPlugin, IDataPlugin
    {
        public PluginManager PluginManager { get; set; }

        private LiveTimingPluginProcessor _processor;

        public void Init(PluginManager pluginManager) { }

        public void DataUpdate(PluginManager pluginManager, ref GameData gameData)
        {
            if (_processor == null)
            {
                var propertyManager = new PropertyManager();

                var raceEventHandler = new RaceEventHandler(
                    new RaceEventRecoveryFile(new RaceEventRecoveryFileEventSelector<PitEvent>(), new RaceEventRecoveryFileEventSelector<PlayerFinishedLapEvent>()),
                    new PitEventStore(),
                    new PlayerFinishedLapEventStore()
                );

                var raceEntryProcessor = new RaceEntryProcessor(propertyManager, raceEventHandler);

                _processor = new LiveTimingPluginProcessor(propertyManager, raceEventHandler, raceEntryProcessor);
            }

            _processor.DataUpdate(
                TestableGameDataConverter.FromGameData(gameData),
                gameData.SessionId
            );
        }

        public void End(PluginManager pluginManager) { }
    }
}