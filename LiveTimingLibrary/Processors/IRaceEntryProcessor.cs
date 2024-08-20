public interface IRaceEntryProcessor
{
    void Process(string sessionId, SessionType sessionType, TestableOpponent newData, TestableOpponent oldData,
        TestableOpponent leaderData, TestableOpponent inFrontData,
        IFastestFragmentTimesStore timesStore);
}
