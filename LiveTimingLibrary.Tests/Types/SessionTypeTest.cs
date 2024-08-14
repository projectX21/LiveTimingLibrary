public class SessionTypeTest
{
    [Fact]
    public void TestToEnum()
    {
        Assert.Equal(SessionType.Practice, SessionTypeConverter.ToEnum("PRACTICE"));
        Assert.Equal(SessionType.Practice, SessionTypeConverter.ToEnum("Practice"));
        Assert.Equal(SessionType.Practice, SessionTypeConverter.ToEnum("practice"));
        Assert.Equal(SessionType.Practice, SessionTypeConverter.ToEnum("PRACTICE 1"));
        Assert.Equal(SessionType.Practice, SessionTypeConverter.ToEnum("Practice 1"));
        Assert.Equal(SessionType.Practice, SessionTypeConverter.ToEnum("practice 1"));
        Assert.Equal(SessionType.Practice, SessionTypeConverter.ToEnum("PRACTICE 2"));
        Assert.Equal(SessionType.Practice, SessionTypeConverter.ToEnum("Practice 2"));
        Assert.Equal(SessionType.Practice, SessionTypeConverter.ToEnum("practice 2"));
        Assert.Equal(SessionType.Practice, SessionTypeConverter.ToEnum("PRACTICE 3"));
        Assert.Equal(SessionType.Practice, SessionTypeConverter.ToEnum("Practice 3"));
        Assert.Equal(SessionType.Practice, SessionTypeConverter.ToEnum("practice 3"));

        Assert.Equal(SessionType.Qualifying, SessionTypeConverter.ToEnum("QUALIFYING"));
        Assert.Equal(SessionType.Qualifying, SessionTypeConverter.ToEnum("Qualifying"));
        Assert.Equal(SessionType.Qualifying, SessionTypeConverter.ToEnum("qualifying"));
        Assert.Equal(SessionType.Qualifying, SessionTypeConverter.ToEnum("QUALIFY"));
        Assert.Equal(SessionType.Qualifying, SessionTypeConverter.ToEnum("Qualify"));
        Assert.Equal(SessionType.Qualifying, SessionTypeConverter.ToEnum("qualify"));
        Assert.Equal(SessionType.Qualifying, SessionTypeConverter.ToEnum("QUALIFYING 1"));
        Assert.Equal(SessionType.Qualifying, SessionTypeConverter.ToEnum("Qualifying 1"));
        Assert.Equal(SessionType.Qualifying, SessionTypeConverter.ToEnum("qualifying 1"));
        Assert.Equal(SessionType.Qualifying, SessionTypeConverter.ToEnum("QUALIFYING 2"));
        Assert.Equal(SessionType.Qualifying, SessionTypeConverter.ToEnum("Qualifying 2"));
        Assert.Equal(SessionType.Qualifying, SessionTypeConverter.ToEnum("qualifying 2"));
        Assert.Equal(SessionType.Qualifying, SessionTypeConverter.ToEnum("QUALIFYING 3"));
        Assert.Equal(SessionType.Qualifying, SessionTypeConverter.ToEnum("Qualifying 3"));
        Assert.Equal(SessionType.Qualifying, SessionTypeConverter.ToEnum("qualifying 3"));

        Assert.Equal(SessionType.Race, SessionTypeConverter.ToEnum("RACE"));
        Assert.Equal(SessionType.Race, SessionTypeConverter.ToEnum("Race"));
        Assert.Equal(SessionType.Race, SessionTypeConverter.ToEnum("race"));

        Assert.Throws<Exception>(() => SessionTypeConverter.ToEnum(""));
        Assert.Throws<Exception>(() => SessionTypeConverter.ToEnum("Warm Up"));
        Assert.Throws<Exception>(() => SessionTypeConverter.ToEnum("Invalid"));
        Assert.Throws<Exception>(() => SessionTypeConverter.ToEnum("Other"));
    }
}