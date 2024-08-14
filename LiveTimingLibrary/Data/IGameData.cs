using System;

public interface IGameData
{
    string GameName { get; set; }

    bool GameRunning { get; set; }

    Guid SessionId { get; set; }

    IStatusDataBase OldData { get; set; }

    IStatusDataBase NewData { get; set; }
}

public interface IGameData<T> : IGameData
{
    IStatusData<T> GameOldData { get; set; }

    IStatusData<T> GameNewData { get; set; }
}