public interface IRaceEntryProcessor
{
    void Process(SessionType sessionType, TestableOpponent newData, TestableOpponent oldData,
        TestableOpponent leaderData, TestableOpponent inFrontData,
        IFastestFragmentTimesStore timesStore);
}
