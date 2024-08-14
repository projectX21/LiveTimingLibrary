/*
Not able to test it

using GameReaderCommon;

public class Test
{
    public string Name { get; set; } = "Test";
}

public class TestableStatusDataBaseConverterTest
{
    [Fact]
    public void TestFromStatusDataBase()
    {
        var result = TestableStatusDataBaseConverter.FromStatusDataBase(CreateStatusData());
        Assert.Equal(CreateExpectedResult(), result);
    }

    public static StatusDataBase CreateStatusData(string sessionName = "SessionName")
    {
        return new StatusData<Test>(new GameUnitSettings())
        {
            SessionTypeName = sessionName,
            CurrentLap = 5,
            CurrentLapTime = TimeSpan.Parse("00:01:12.4010000"),
            Opponents = [
                TestableOpponentConverterTest.CreateOpponent("Name 1", "107"),
                TestableOpponentConverterTest.CreateOpponent("Name 2", "999"),
                TestableOpponentConverterTest.CreateOpponent("Name 3", "1")
            ]
        };
    }

    public static TestableStatusDataBase CreateExpectedResult(string sessionName = "SessionName")
    {
        return new TestableStatusDataBase
        {
            SessionName = sessionName,
            CurrentLap = 5,
            CurrentLapTime = TimeSpan.Parse("00:01:12.4010000"),
            Opponents = []
        };
    }
}
*/