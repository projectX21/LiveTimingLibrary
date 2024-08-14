using System;

public interface IStatusDataBase
{
    string SessionName { get; set; }

    int CurrentLap { get; set; }

    TimeSpan CurrentLapTime { get; set; }

    IOpponent[] Opponents { get; set; }
}