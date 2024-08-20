public class SessionIdGeneratorTest
{
    [Fact]
    public void TestGenerate()
    {
        var data = new TestableStatusDataBase
        {
            GameName = "Testgame",
            TrackName = "Testtrack",
            SessionName = "Race"
        };

        Assert.Equal("d8248d7cce41618d2caea0ac66ae8870", SessionIdGenerator.Generate(data));
    }
}