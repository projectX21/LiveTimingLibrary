public class SessionIdGeneratorTest
{
    [Fact]
    public void TestGenerate()
    {
        var data = new TestableStatusDataBase
        {
            GameName = "Testgame",
            TrackName = "Testtrack New",
            SessionName = "Race"
        };

        Assert.Equal("testgame_testtrack_new_race", SessionIdGenerator.Generate(data));
    }
}
