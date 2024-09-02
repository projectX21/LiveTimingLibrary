using Moq;

public class LiveTimingPluginProcessorTest
{
    [Fact]
    public void TestGameProcessorExecution()
    {
        var mockPropertyManager = new Mock<IPropertyManager>();
        var mockRaceEventHandler = new Mock<IRaceEventHandler>();
        var mockRaceEntryProcessor = new Mock<IRaceEntryProcessor>();
        LiveTimingPluginProcessor processor = new(mockPropertyManager.Object, mockRaceEventHandler.Object, mockRaceEntryProcessor.Object);

        var mockGameProcessor = new Mock<IGameProcessor>();
        mockGameProcessor.SetupGet(m => m.CurrentGameName).Returns("Test");
        processor.GameProcessor = mockGameProcessor.Object;

        // NewData is null, therefore the Run() method of the gameProcessor shouldn't be called
        var gameData = new TestableGameData
        {
            GameRunning = true,
            GameName = "Test",
            OldData = new TestableStatusDataBase()
        };
        processor.DataUpdate(gameData);
        mockGameProcessor.Verify(m => m.Run(It.IsAny<TestableGameData>()), Times.Never());

        // NewData is filled now, therefore the Run() method of the gameProcessor should be called
        gameData.NewData = new TestableStatusDataBase();
        processor.DataUpdate(gameData);
        mockGameProcessor.Verify(m => m.Run(It.IsAny<TestableGameData>()), Times.Once());
    }

    [Fact]
    public void TestCurrentGameChange()
    {
        var mockPropertyManager = new Mock<IPropertyManager>();
        var mockRaceEventHandler = new Mock<IRaceEventHandler>();
        var mockRaceEntryProcessor = new Mock<IRaceEntryProcessor>();
        LiveTimingPluginProcessor processor = new(mockPropertyManager.Object, mockRaceEventHandler.Object, mockRaceEntryProcessor.Object);

        // Should be the default GameProcessor
        var gameData = new TestableGameData
        {
            GameRunning = true,
            GameName = "Test",
            NewData = new TestableStatusDataBase
            {
                SessionName = "Race",
                TrackName = "Testtrack",
                Opponents = [
                    new TestableOpponent
                    {
                        IsPlayer = true,
                        CurrentLap = 2,
                        CurrentSector = 3,
                        CurrentLapTime = TimeSpan.Parse("00:00:01.4100000"),
                    }
                ]
            },
            OldData = new TestableStatusDataBase()
        };
        processor.DataUpdate(gameData);
        Assert.Equal("Test", processor.GameProcessor.CurrentGameName);
        var gameProcessor = processor.GameProcessor;

        gameData.GameName = "Test2";
        processor.DataUpdate(gameData);
        Assert.Equal("Test2", processor.GameProcessor.CurrentGameName);

        // should create new instance when game name changes
        Assert.NotEqual(gameProcessor, processor.GameProcessor);
    }
}