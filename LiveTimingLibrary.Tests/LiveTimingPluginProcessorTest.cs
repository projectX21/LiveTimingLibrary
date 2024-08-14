using Moq;

public class LiveTimingPluginProcessorTest
{
    [Fact]
    public void TestSetSessionId()
    {
        var mockPropertyManager = new Mock<IPropertyManager>();
        var mockRaceEventHandler = new Mock<IRaceEventHandler>();
        var mockRaceEntryProcessor = new Mock<IRaceEntryProcessor>();
        LiveTimingPluginProcessor processor = new(mockPropertyManager.Object, mockRaceEventHandler.Object, mockRaceEntryProcessor.Object);

        // For the first time only the SessionId on the NewData should be set
        var gameData = new TestableGameData
        {
            GameRunning = true,
            GameName = "Test",
            NewData = new TestableStatusDataBase
            {
                SessionName = "Race",
                CurrentLap = 2,
                CurrentLapTime = TimeSpan.Parse("00:00:01.4100000"),
                Opponents = []
            },
            OldData = new TestableStatusDataBase()
        };
        processor.DataUpdate(gameData, Guid.Parse("00000000-1234-3456-6789-000000000000"));
        Assert.Equal(Guid.Parse("00000000-1234-3456-6789-000000000000"), gameData.NewData.SessionId);
        Assert.Equal(Guid.Empty, gameData.OldData.SessionId);

        // now in the second DataUpdate the SessionId of the previous DataUpdate should be the one of the OldData
        processor.DataUpdate(gameData, Guid.Parse("00000000-1234-3456-6789-000000000001"));
        Assert.Equal(Guid.Parse("00000000-1234-3456-6789-000000000001"), gameData.NewData.SessionId);
        Assert.Equal(Guid.Parse("00000000-1234-3456-6789-000000000000"), gameData.OldData.SessionId);
    }

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
        processor.DataUpdate(gameData, Guid.Parse("00000000-1234-3456-6789-000000000000"));
        mockGameProcessor.Verify(m => m.Run(It.IsAny<TestableGameData>()), Times.Never());

        // NewData is filled now, therefore the Run() method of the gameProcessor should be called
        gameData.NewData = new TestableStatusDataBase();
        processor.DataUpdate(gameData, Guid.Parse("00000000-1234-3456-6789-000000000000"));
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
                CurrentLap = 2,
                CurrentLapTime = TimeSpan.Parse("00:00:01.4100000"),
                Opponents = []
            },
            OldData = new TestableStatusDataBase()
        };
        processor.DataUpdate(gameData, Guid.Parse("00000000-1234-3456-6789-000000000000"));
        Assert.Equal("Test", processor.GameProcessor.CurrentGameName);
        var gameProcessor = processor.GameProcessor;

        gameData.GameName = "Test2";
        processor.DataUpdate(gameData, Guid.Parse("00000000-1234-3456-6789-000000000000"));
        Assert.Equal("Test2", processor.GameProcessor.CurrentGameName);

        // should create new instance when game name changes
        Assert.NotEqual(gameProcessor, processor.GameProcessor);
    }
}