using Moq;

public class TestableStatusDataBaseConverterTest
{
    [Fact]
    public void TestFromStatusDataBase()
    {
        var result = TestableStatusDataBaseConverter.FromStatusDataBase(CreateStatusDataBaseMock().Object);
        Assert.Equal(CreateExpectedResult(), result);
    }

    public static Mock<IStatusDataBase> CreateStatusDataBaseMock(string sessionName = "SessionName")
    {
        var mock = new Mock<IStatusDataBase>();
        mock.SetupGet(m => m.SessionName).Returns(sessionName);
        mock.SetupGet(m => m.CurrentLap).Returns(5);
        mock.SetupGet(m => m.CurrentLapTime).Returns(TimeSpan.Parse("00:01:12.4010000"));
        mock.SetupGet(m => m.Opponents).Returns([
            TestableOpponentConverterTest.CreateOpponentMock("Name 1", "107").Object,
            TestableOpponentConverterTest.CreateOpponentMock("Name 2", "999").Object,
            TestableOpponentConverterTest.CreateOpponentMock("Name 3", "1").Object
        ]);

        return mock;
    }

    public static TestableStatusDataBase CreateExpectedResult(string sessionName = "SessionName")
    {
        return new TestableStatusDataBase
        {
            SessionName = sessionName,
            CurrentLap = 5,
            CurrentLapTime = TimeSpan.Parse("00:01:12.4010000"),
            Opponents = [
                TestableOpponentConverterTest.CreateExpectedResult("Name 1", "107"),
                TestableOpponentConverterTest.CreateExpectedResult("Name 2", "999"),
                TestableOpponentConverterTest.CreateExpectedResult("Name 3", "1")
            ]
        };
    }
}
