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

        // Game isn't running, therefore the Run() method of the gameProcessor shouldn't be called
        var gameData = new TestableGameData
        {
            GameName = "Test",
            GameRunning = false
        };
        processor.DataUpdate(gameData);
        mockGameProcessor.Verify(m => m.Run(It.IsAny<TestableGameData>()), Times.Never());

        // Game is paused, therefore the Run() method of the gameProcessor shouldn't be called
        gameData = new TestableGameData
        {
            GameName = "Test",
            GamePaused = true
        };
        processor.DataUpdate(gameData);
        mockGameProcessor.Verify(m => m.Run(It.IsAny<TestableGameData>()), Times.Never());

        // No opponents, therefore the Run() method of the gameProcessor shouldn't be called
        gameData = new TestableGameData
        {
            GameName = "Test",
            GameRunning = true,
            Opponents = []
        };
        processor.DataUpdate(gameData);
        mockGameProcessor.Verify(m => m.Run(It.IsAny<TestableGameData>()), Times.Never());

        // ACC has the special case, where only the driver itself exists in the opponent list while the initialisation runs.
        gameData = new TestableGameData
        {
            GameName = "Test",
            GameRunning = true,
            Opponents = [
                new TestableOpponent
                {
                    IsPlayer = true
                }
            ]
        };
        processor.DataUpdate(gameData);
        mockGameProcessor.Verify(m => m.Run(It.IsAny<TestableGameData>()), Times.Never());

        // we have two opponents and the Run() method should run now
        gameData = new TestableGameData
        {
            GameName = "Test",
            GameRunning = true,
            Opponents = [
                new TestableOpponent
                {
                    IsPlayer = true
                },
                new TestableOpponent
                {
                    IsPlayer = false
                }
            ]
        };
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
            GamePaused = false,
            GameName = "Test",
            SessionName = "Race",
            TrackName = "Testtrack",
            Opponents = [
                    new TestableOpponent
                    {
                        CarNumber = "107",
                        IsPlayer = true,
                        CurrentLap = 2,
                        CurrentSector = 3,
                        CurrentLapTime = TimeSpan.Parse("00:00:01.4100000"),
                    },
                    new TestableOpponent
                    {
                        CarNumber = "108",
                        IsPlayer = false
                    }
                ]
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