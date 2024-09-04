public interface IRaceEntryProcessor
{
    void Process(string sessionId, SessionType sessionType, int entryPos, TestableOpponent newData, TestableOpponent oldData,
        TestableOpponent leaderData, TestableOpponent inFrontData,
        IFastestFragmentTimesStore timesStore,
        IEntryProgressStore entryProgressStore);
}
