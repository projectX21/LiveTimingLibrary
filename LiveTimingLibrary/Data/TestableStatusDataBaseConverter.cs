using System.Collections.Generic;
using System.Linq;
using GameReaderCommon;

public class TestableStatusDataBaseConverter
{
    public static TestableStatusDataBase FromStatusDataBase(StatusDataBase data, string GameName)
    {
        return new TestableStatusDataBase
        {
            GameName = GameName,
            TrackName = data.TrackName,
            SessionName = data.SessionTypeName,
            CurrentLap = data.CurrentLap,
            CurrentLapTime = data.CurrentLapTime,
            Opponents = (data.Opponents?.Count > 0 ? data.Opponents.Select(TestableOpponentConverter.FromOpponent) : new List<TestableOpponent>()).ToArray()
        };
    }
}