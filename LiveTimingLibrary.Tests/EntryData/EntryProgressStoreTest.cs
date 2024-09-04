public class EntryProgressStoreTest
{
    [Fact]
    public void TestUseCustomGapCalculation()
    {
        var store = new EntryProgressStore("AssettoCorsaCompetizione");
        Assert.True(store.UseCustomGapCalculation());

        store = new EntryProgressStore("RFactor2");
        Assert.False(store.UseCustomGapCalculation());

        store = new EntryProgressStore("LMU");
        Assert.False(store.UseCustomGapCalculation());

        store = new EntryProgressStore("F12023");
        Assert.False(store.UseCustomGapCalculation());
    }

    [Fact]
    public void TestGetLastEntryProgress()
    {
        var store = new EntryProgressStore("AssettoCorsaCompetizione");

        var lastOfTest1 = new EntryProgress("Test1", 1, 0, TimeSpan.Parse("00:00:01.4890000"), 1);
        store.AddIfNotAlreadyExists(lastOfTest1);

        var lastOfTest2 = new EntryProgress("Test2", 1, 2, TimeSpan.Parse("00:00:14.3200000"), 2);
        store.AddIfNotAlreadyExists(new EntryProgress("Test2", 1, 0, TimeSpan.Parse("00:00:00.0420000"), 2));
        store.AddIfNotAlreadyExists(new EntryProgress("Test2", 1, 1, TimeSpan.Parse("00:00:08.1590000"), 2));
        store.AddIfNotAlreadyExists(lastOfTest2);

        var lastOfTest3 = new EntryProgress("Test3", 1, 1, TimeSpan.Parse("00:00:13.6250000"), 3);
        store.AddIfNotAlreadyExists(new EntryProgress("Test3", 1, 0, TimeSpan.Parse("00:00:00.7480000"), 3));
        store.AddIfNotAlreadyExists(lastOfTest3);

        var lastOfTest4 = new EntryProgress("Test4", 1, 2, TimeSpan.Parse("00:00:14.3210000"), 4);
        store.AddIfNotAlreadyExists(new EntryProgress("Test4", 1, 0, TimeSpan.Parse("00:00:00.0130000"), 4));
        store.AddIfNotAlreadyExists(new EntryProgress("Test4", 1, 1, TimeSpan.Parse("00:00:08.1570000"), 4));
        store.AddIfNotAlreadyExists(lastOfTest4);

        Assert.Equal(lastOfTest1, store.GetLastEntryProgress("Test1"));
        Assert.Equal(lastOfTest2, store.GetLastEntryProgress("Test2"));
        Assert.Equal(lastOfTest3, store.GetLastEntryProgress("Test3"));
        Assert.Equal(lastOfTest4, store.GetLastEntryProgress("Test4"));
        Assert.Null(store.GetLastEntryProgress("Test5"));
    }

    [Fact]
    public void TestGetProgressForMiniSector()
    {
        var store = new EntryProgressStore("AssettoCorsaCompetizione");

        var p1Test1 = new EntryProgress("Test1", 1, 0, TimeSpan.Parse("00:00:01.4890000"), 1);
        store.AddIfNotAlreadyExists(p1Test1);

        var p1Test2 = new EntryProgress("Test2", 1, 0, TimeSpan.Parse("00:00:00.0420000"), 2);
        var p2Test2 = new EntryProgress("Test2", 1, 1, TimeSpan.Parse("00:00:08.1590000"), 2);
        var p3Test2 = new EntryProgress("Test2", 1, 2, TimeSpan.Parse("00:00:14.3200000"), 2);
        var p4Test2 = new EntryProgress("Test2", 2, 0, TimeSpan.Parse("00:00:17.8570000"), 2);
        store.AddIfNotAlreadyExists(p1Test2);
        store.AddIfNotAlreadyExists(p2Test2);
        store.AddIfNotAlreadyExists(p3Test2);
        store.AddIfNotAlreadyExists(p4Test2);

        var p1Test3 = new EntryProgress("Test3", 1, 0, TimeSpan.Parse("00:00:00.7480000"), 3);
        var p2Test3 = new EntryProgress("Test3", 1, 1, TimeSpan.Parse("00:00:13.6250000"), 3);
        store.AddIfNotAlreadyExists(p1Test3);
        store.AddIfNotAlreadyExists(p2Test3);

        var p1Test4 = new EntryProgress("Test4", 1, 0, TimeSpan.Parse("00:00:00.0130000"), 4);
        var p2Test4 = new EntryProgress("Test4", 1, 1, TimeSpan.Parse("00:00:08.1570000"), 4);
        var p3Test4 = new EntryProgress("Test4", 1, 2, TimeSpan.Parse("00:00:14.3210000"), 4);
        store.AddIfNotAlreadyExists(p1Test4);
        store.AddIfNotAlreadyExists(p2Test4);
        store.AddIfNotAlreadyExists(p3Test4);

        Assert.Equal(p1Test1, store.GetLatestProgress("Test1", 0));
        Assert.Equal(p4Test2, store.GetLatestProgress("Test2", 0)); // should return the latest progress with minisector 0 (lap 2 not lap 1)
        Assert.Equal(p2Test2, store.GetLatestProgress("Test2", 1));
        Assert.Equal(p3Test2, store.GetLatestProgress("Test2", 2));
        Assert.Equal(p1Test3, store.GetLatestProgress("Test3", 0));
        Assert.Equal(p2Test3, store.GetLatestProgress("Test3", 1));
        Assert.Equal(p1Test4, store.GetLatestProgress("Test4", 0));
        Assert.Equal(p2Test4, store.GetLatestProgress("Test4", 1));
        Assert.Equal(p3Test4, store.GetLatestProgress("Test4", 2));
        Assert.Null(store.GetLatestProgress("Test4", 3));
    }

    [Fact]
    public void TestAddIfNotAlreadyExists()
    {
        // shouldn't do anything if custom gap calculation shouldn't be used
        // Only ACC uses the custom calculation
        var store = new EntryProgressStore("RFactor2");
        store.AddIfNotAlreadyExists(new EntryProgress("Test1", 1, 0, TimeSpan.Parse("00:00:01.4890000"), 1));
        Assert.Null(store.GetAllProgressesForEntry("Test1"));

        store = new EntryProgressStore("AssettoCorsaCompetizione");
        store.AddIfNotAlreadyExists(new EntryProgress("Test1", 1, 0, TimeSpan.Parse("00:00:01.4890000"), 1));
        Assert.Single(store.GetAllProgressesForEntry("Test1"));

        // should not insert same lap/mini sector pair again
        store.AddIfNotAlreadyExists(new EntryProgress("Test1", 1, 0, TimeSpan.Parse("00:00:03.1870000"), 2));
        Assert.Single(store.GetAllProgressesForEntry("Test1"));

        // insert for another entry id
        store.AddIfNotAlreadyExists(new EntryProgress("Test2", 1, 0, TimeSpan.Parse("00:00:03.1870000"), 2));
        Assert.Single(store.GetAllProgressesForEntry("Test1"));
        Assert.Single(store.GetAllProgressesForEntry("Test2"));
    }

    [Fact]
    public void TestCalcGap()
    {
        var store = new EntryProgressStore("AssettoCorsaCompetizione");
        Assert.Null(store.CalcGap("Test1", "Test2"));
        Assert.Null(store.CalcGap("Test2", "Test1"));

        store.AddIfNotAlreadyExists(new EntryProgress("Test1", 1, 0, TimeSpan.Parse("00:00:01.4890000"), 2));
        Assert.Null(store.CalcGap("Test1", "Test2"));
        Assert.Null(store.CalcGap("Test2", "Test1"));

        store.AddIfNotAlreadyExists(new EntryProgress("Test2", 1, 0, TimeSpan.Parse("00:00:00.0420000"), 1));
        Assert.Null(store.CalcGap("Test1", "Test2"));
        Assert.Equal("+1.447", store.CalcGap("Test2", "Test1"));

        store.AddIfNotAlreadyExists(new EntryProgress("Test1", 1, 1, TimeSpan.Parse("00:00:18.1470000"), 1));
        store.AddIfNotAlreadyExists(new EntryProgress("Test1", 1, 2, TimeSpan.Parse("00:00:21.1980000"), 1));
        store.AddIfNotAlreadyExists(new EntryProgress("Test2", 1, 1, TimeSpan.Parse("00:00:28.1140000"), 2));
        Assert.Equal("+9.967", store.CalcGap("Test1", "Test2"));
        Assert.Null(store.CalcGap("Test2", "Test1"));

        store.AddIfNotAlreadyExists(new EntryProgress("Test1", 2, 0, TimeSpan.Parse("00:00:50.3320000"), 1));
        store.AddIfNotAlreadyExists(new EntryProgress("Test1", 3, 0, TimeSpan.Parse("00:01:29.1580000"), 1));
        store.AddIfNotAlreadyExists(new EntryProgress("Test2", 2, 0, TimeSpan.Parse("00:01:28.1140000"), 2));

        // Test2 isn't lapped yet
        Assert.Equal("+37.782", store.CalcGap("Test1", "Test2"));
        Assert.Null(store.CalcGap("Test2", "Test1"));

        // now Test 2 is lapped
        store.AddIfNotAlreadyExists(new EntryProgress("Test1", 4, 0, TimeSpan.Parse("00:01:45.3690000"), 1));
        store.AddIfNotAlreadyExists(new EntryProgress("Test2", 3, 0, TimeSpan.Parse("00:01:47.3580000"), 2));
        Assert.Equal("+1L", store.CalcGap("Test1", "Test2"));
        Assert.Null(store.CalcGap("Test2", "Test1"));

        // now Test 2 is lapped twice
        store.AddIfNotAlreadyExists(new EntryProgress("Test1", 7, 0, TimeSpan.Parse("00:07:15.1870000"), 1));
        store.AddIfNotAlreadyExists(new EntryProgress("Test2", 4, 0, TimeSpan.Parse("00:07:08.1470000"), 2));
        Assert.Equal("+2L", store.CalcGap("Test1", "Test2"));
        Assert.Null(store.CalcGap("Test2", "Test1"));

        // and now a third time
        store.AddIfNotAlreadyExists(new EntryProgress("Test1", 8, 0, TimeSpan.Parse("00:07:50.2280000"), 1));
        store.AddIfNotAlreadyExists(new EntryProgress("Test2", 5, 0, TimeSpan.Parse("00:07:52.3570000"), 2));
        Assert.Equal("+3L", store.CalcGap("Test1", "Test2"));
        Assert.Null(store.CalcGap("Test2", "Test1"));
    }

    [Fact]
    public void TestReorg()
    {
        var store = new EntryProgressStore("AssettoCorsaCompetizione");

        for (var i = 0; i < 200; i++)
        {
            store.AddIfNotAlreadyExists(new EntryProgress("Test1", 1, i, TimeSpan.Parse("00:00:01.4890000"), 1));
        }

        // Even though we've imported 200 entries, only the last 90 should exist
        Assert.Equal(90, store.GetAllProgressesForEntry("Test1").Count);
        Assert.Equal(199, store.GetAllProgressesForEntry("Test1").Last().GetMiniSector());
    }

    [Fact]
    public void TestGetEntryIdsSortedByProgress()
    {
        var store = new EntryProgressStore("AssettoCorsaCompetizione");

        store.AddIfNotAlreadyExists(new EntryProgress("Test1", 1, 0, TimeSpan.Parse("00:00:01.4890000"), 1));

        store.AddIfNotAlreadyExists(new EntryProgress("Test2", 1, 0, TimeSpan.Parse("00:00:00.0420000"), 2));
        store.AddIfNotAlreadyExists(new EntryProgress("Test2", 1, 1, TimeSpan.Parse("00:00:08.1590000"), 2));
        store.AddIfNotAlreadyExists(new EntryProgress("Test2", 1, 2, TimeSpan.Parse("00:00:14.3200000"), 2));

        store.AddIfNotAlreadyExists(new EntryProgress("Test3", 1, 0, TimeSpan.Parse("00:00:00.7480000"), 3));
        store.AddIfNotAlreadyExists(new EntryProgress("Test3", 1, 1, TimeSpan.Parse("00:00:13.6250000"), 3));

        store.AddIfNotAlreadyExists(new EntryProgress("Test4", 1, 0, TimeSpan.Parse("00:00:00.0130000"), 4));
        store.AddIfNotAlreadyExists(new EntryProgress("Test4", 1, 1, TimeSpan.Parse("00:00:08.1570000"), 4));
        store.AddIfNotAlreadyExists(new EntryProgress("Test4", 1, 2, TimeSpan.Parse("00:00:14.3210000"), 4));

        Assert.Equal(["Test2", "Test4", "Test3", "Test1"], store.GetEntryIdsSortedByProgress());
    }
}