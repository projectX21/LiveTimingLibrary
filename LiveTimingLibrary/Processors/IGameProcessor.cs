public interface IGameProcessor
{
    string CurrentGameName { get; }

    TestableOpponent[] GetEntries();

    void Run(TestableGameData gameData);
}