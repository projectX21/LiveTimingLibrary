using System.Linq;

public class TestableStatusDataBaseConverter
{
    public static TestableStatusDataBase FromStatusDataBase(IStatusDataBase data)
    {
        return new TestableStatusDataBase
        {
            SessionName = data.SessionName,
            CurrentLap = data.CurrentLap,
            CurrentLapTime = data.CurrentLapTime,
            Opponents = data.Opponents.Select(TestableOpponentConverter.FromOpponent).ToArray()
        };
    }
}