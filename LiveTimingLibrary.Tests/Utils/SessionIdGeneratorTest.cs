public class SessionIdGeneratorTest
{
    [Fact]
    public void TestGenerate()
    {
        var data = new TestableGameData
        {
            GameName = "Testgame",
            TrackName = "Testtrack New",
        };

        Assert.Equal("testgame_testtrack_new", SessionIdGenerator.Generate(data));
    }
}
