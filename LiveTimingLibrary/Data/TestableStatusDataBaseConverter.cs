using System.Collections.Generic;
using System.Linq;
using GameReaderCommon;

public class TestableStatusDataBaseConverter
{
    public static TestableStatusDataBase FromStatusDataBase(StatusDataBase data)
    {
        return new TestableStatusDataBase
        {
            SessionName = data.SessionTypeName,
            CurrentLap = data.CurrentLap,
            CurrentLapTime = data.CurrentLapTime,
            Opponents = data.Opponents.Select(TestableOpponentConverter.FromOpponent).ToArray()
        };
    }
}