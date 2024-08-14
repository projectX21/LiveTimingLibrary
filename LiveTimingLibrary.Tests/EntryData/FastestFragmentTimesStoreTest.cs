public class FastestFragmentTimesStoreTest
{
    [Fact]
    public void TestGetCurrentLapFragmentTimeIndicator()
    {
        var entries = CreateTestEntries();
        var store = new FastestFragmentTimesStore(entries);

        // Entry 1
        Assert.Equal("ENTRY_BEST", store.GetCurrentLapFragmentTimeIndicator(entries[0], LapFragmentType.SECTOR_1));
        Assert.Equal("", store.GetCurrentLapFragmentTimeIndicator(entries[0], LapFragmentType.SECTOR_2));
        Assert.Equal("", store.GetCurrentLapFragmentTimeIndicator(entries[0], LapFragmentType.SECTOR_3)); // never filled
        Assert.Equal("", store.GetCurrentLapFragmentTimeIndicator(entries[0], LapFragmentType.FULL_LAP)); // never filled

        // Entry 2
        Assert.Equal("", store.GetCurrentLapFragmentTimeIndicator(entries[1], LapFragmentType.SECTOR_1));
        Assert.Equal("ENTRY_BEST", store.GetCurrentLapFragmentTimeIndicator(entries[1], LapFragmentType.SECTOR_2));
        Assert.Equal("", store.GetCurrentLapFragmentTimeIndicator(entries[1], LapFragmentType.SECTOR_3)); // never filled
        Assert.Equal("", store.GetCurrentLapFragmentTimeIndicator(entries[1], LapFragmentType.FULL_LAP)); // never filled

        // Entry 3
        Assert.Equal("CLASS_BEST", store.GetCurrentLapFragmentTimeIndicator(entries[2], LapFragmentType.SECTOR_1));
        Assert.Equal("", store.GetCurrentLapFragmentTimeIndicator(entries[2], LapFragmentType.SECTOR_2)); // not filled
        Assert.Equal("", store.GetCurrentLapFragmentTimeIndicator(entries[2], LapFragmentType.SECTOR_3)); // never filled
        Assert.Equal("", store.GetCurrentLapFragmentTimeIndicator(entries[2], LapFragmentType.FULL_LAP)); // never filled

        // Entry 4
        Assert.Equal("", store.GetCurrentLapFragmentTimeIndicator(entries[3], LapFragmentType.SECTOR_1)); // not filled
        Assert.Equal("", store.GetCurrentLapFragmentTimeIndicator(entries[3], LapFragmentType.SECTOR_2)); // not filled
        Assert.Equal("", store.GetCurrentLapFragmentTimeIndicator(entries[3], LapFragmentType.SECTOR_3)); // never filled
        Assert.Equal("", store.GetCurrentLapFragmentTimeIndicator(entries[3], LapFragmentType.FULL_LAP)); // never filled
    }

    [Fact]
    public void TestGetLastLapFragmentTimeIndicator()
    {
        var entries = CreateTestEntries();
        var store = new FastestFragmentTimesStore(entries);

        // Entry 1
        Assert.Equal("", store.GetLastLapFragmentTimeIndicator(entries[0], LapFragmentType.SECTOR_1));
        Assert.Equal("CLASS_BEST", store.GetLastLapFragmentTimeIndicator(entries[0], LapFragmentType.SECTOR_2));
        Assert.Equal("CLASS_BEST", store.GetLastLapFragmentTimeIndicator(entries[0], LapFragmentType.SECTOR_3));
        Assert.Equal("", store.GetLastLapFragmentTimeIndicator(entries[0], LapFragmentType.FULL_LAP));

        // Entry 2
        Assert.Equal("CLASS_BEST", store.GetLastLapFragmentTimeIndicator(entries[1], LapFragmentType.SECTOR_1));
        Assert.Equal("", store.GetLastLapFragmentTimeIndicator(entries[1], LapFragmentType.SECTOR_2));
        Assert.Equal("ENTRY_BEST", store.GetLastLapFragmentTimeIndicator(entries[1], LapFragmentType.SECTOR_3));
        Assert.Equal("", store.GetLastLapFragmentTimeIndicator(entries[1], LapFragmentType.FULL_LAP));

        // Entry 3
        Assert.Equal("", store.GetLastLapFragmentTimeIndicator(entries[2], LapFragmentType.SECTOR_1));
        Assert.Equal("", store.GetLastLapFragmentTimeIndicator(entries[2], LapFragmentType.SECTOR_2));
        Assert.Equal("", store.GetLastLapFragmentTimeIndicator(entries[2], LapFragmentType.SECTOR_3));
        Assert.Equal("CLASS_BEST", store.GetLastLapFragmentTimeIndicator(entries[2], LapFragmentType.FULL_LAP));

        // Entry 4
        Assert.Equal("ENTRY_BEST", store.GetLastLapFragmentTimeIndicator(entries[3], LapFragmentType.SECTOR_1));
        Assert.Equal("CLASS_BEST", store.GetLastLapFragmentTimeIndicator(entries[3], LapFragmentType.SECTOR_2));
        Assert.Equal("", store.GetLastLapFragmentTimeIndicator(entries[3], LapFragmentType.SECTOR_3));
        Assert.Equal("ENTRY_BEST", store.GetLastLapFragmentTimeIndicator(entries[3], LapFragmentType.FULL_LAP));
    }

    [Fact]
    public void TestGetFragmentTimeIndicator()
    {
        TestGetFragmentTimeIndicatorForEntries(CreateTestEntries());
    }

    /*
        [Fact]
        public void TestGetFragmentTimeIndicatorWhenCarClassIsNotFilled()
        {
            var entries = CreateTestEntries();
            entries[2].CarClass = null;
            entries[3].CarClass = "";

            // should work exactly the same as the test before. In this case the third and fourth entry have no car class set (normally Class2).
            // null and empty CarClass should be transformed into "unknown"
            TestGetFragmentTimeIndicatorForEntries(entries);
        }
    */

    [Fact]
    public void TestIsFastestInClass()
    {
        var entries = CreateTestEntries();
        var store = new FastestFragmentTimesStore(entries);

        // CarClass "invalid" doesn't exist
        Assert.Throws<Exception>(() => store.IsFastestInClass("invalid", LapFragmentType.SECTOR_1, TimeSpan.Parse("00:00:20.103")));
        Assert.False(store.IsFastestInClass("Class1", LapFragmentType.SECTOR_1, TimeSpan.Parse("0:00:10.410")));
        Assert.True(store.IsFastestInClass("Class1", LapFragmentType.SECTOR_1, TimeSpan.Parse("0:00:10.409")));

        // An empty or null CarClass should be converted into "unknown"
        entries[2].CarClass = null;
        entries[3].CarClass = "";
        store = new FastestFragmentTimesStore(entries);
        Assert.True(store.IsFastestInClass(null, LapFragmentType.SECTOR_1, TimeSpan.Parse("0:00:12.158")));
        Assert.True(store.IsFastestInClass("unknown", LapFragmentType.SECTOR_1, TimeSpan.Parse("0:00:12.158")));

        // some tests for the other lap fragments
        Assert.False(store.IsFastestInClass("Class1", LapFragmentType.SECTOR_2, TimeSpan.Parse("0:01:02.110")));
        Assert.True(store.IsFastestInClass("Class1", LapFragmentType.SECTOR_2, TimeSpan.Parse("0:01:02.109")));

        Assert.False(store.IsFastestInClass("unknown", LapFragmentType.SECTOR_3, TimeSpan.Parse("0:00:41.971")));
        Assert.True(store.IsFastestInClass("unknown", LapFragmentType.SECTOR_3, TimeSpan.Parse("0:00:41.970")));

        Assert.False(store.IsFastestInClass(null, LapFragmentType.FULL_LAP, TimeSpan.Parse("0:01:58.401")));
        Assert.True(store.IsFastestInClass(null, LapFragmentType.FULL_LAP, TimeSpan.Parse("0:01:58.400")));
    }

    [Fact]
    public void TestIsFastestForEntry()
    {
        var entries = CreateTestEntries();
        var store = new FastestFragmentTimesStore(entries);

        // EntryId 585 doesn't exist
        Assert.Throws<Exception>(() => store.IsFastestForEntry("585", LapFragmentType.SECTOR_1, TimeSpan.Parse("00:00:10.411")));
        Assert.False(store.IsFastestForEntry("107", LapFragmentType.SECTOR_1, TimeSpan.Parse("00:00:10.411")));
        Assert.True(store.IsFastestForEntry("107", LapFragmentType.SECTOR_1, TimeSpan.Parse("00:00:10.410")));

        // This is also the class best, but should be of course the entry best as well
        Assert.True(store.IsFastestForEntry("999", LapFragmentType.SECTOR_1, TimeSpan.Parse("00:00:10.409")));
        Assert.False(store.IsFastestForEntry("999", LapFragmentType.SECTOR_1, TimeSpan.Parse("00:00:10.410")));

        // some tests for the other lap fragments
        Assert.False(store.IsFastestForEntry("1", LapFragmentType.SECTOR_2, TimeSpan.Parse("00:01:05.201")));
        Assert.True(store.IsFastestForEntry("1", LapFragmentType.SECTOR_2, TimeSpan.Parse("00:01:05.200")));

        Assert.False(store.IsFastestForEntry("30", LapFragmentType.SECTOR_3, TimeSpan.Parse("00:00:41.999")));
        Assert.True(store.IsFastestForEntry("30", LapFragmentType.SECTOR_3, TimeSpan.Parse("00:00:41.998")));

        Assert.False(store.IsFastestForEntry("107", LapFragmentType.FULL_LAP, TimeSpan.Parse("00:01:52.482")));
        Assert.True(store.IsFastestForEntry("107", LapFragmentType.FULL_LAP, TimeSpan.Parse("00:01:52.481")));
    }

    private static void TestGetFragmentTimeIndicatorForEntries(TestableOpponent[] entries)
    {
        var store = new FastestFragmentTimesStore(entries);

        // Entry 1 / Class 1
        // Sector 1 - class best: 10.409, entry best: 10.410
        Assert.Equal("", store.GetFragmentTimeIndicator(entries[0], null, LapFragmentType.SECTOR_1));
        Assert.Equal("", store.GetFragmentTimeIndicator(entries[0], TimeSpan.Parse("0:00:00.000"), LapFragmentType.SECTOR_1));
        Assert.Equal("", store.GetFragmentTimeIndicator(entries[0], TimeSpan.Parse("0:00:10.411"), LapFragmentType.SECTOR_1));
        Assert.Equal("ENTRY_BEST", store.GetFragmentTimeIndicator(entries[0], TimeSpan.Parse("0:00:10.410"), LapFragmentType.SECTOR_1));
        Assert.Equal("CLASS_BEST", store.GetFragmentTimeIndicator(entries[0], TimeSpan.Parse("0:00:10.409"), LapFragmentType.SECTOR_1));

        // Sector 2 - class best: 1:02.109, entry best: 1:02.109
        Assert.Equal("", store.GetFragmentTimeIndicator(entries[0], TimeSpan.Parse("0:01:02.110"), LapFragmentType.SECTOR_2));
        Assert.Equal("CLASS_BEST", store.GetFragmentTimeIndicator(entries[0], TimeSpan.Parse("0:01:02.109"), LapFragmentType.SECTOR_2));

        // Sector 3 - class best: 39.980, entry best: 39.980
        Assert.Equal("", store.GetFragmentTimeIndicator(entries[0], TimeSpan.Parse("0:00:39.981"), LapFragmentType.SECTOR_3));
        Assert.Equal("CLASS_BEST", store.GetFragmentTimeIndicator(entries[0], TimeSpan.Parse("0:00:39.980"), LapFragmentType.SECTOR_3));

        // Full lap - class best: 1:52.305, entry best: 1:52.481
        Assert.Equal("", store.GetFragmentTimeIndicator(entries[0], TimeSpan.Parse("0:01:52.482"), LapFragmentType.FULL_LAP));
        Assert.Equal("ENTRY_BEST", store.GetFragmentTimeIndicator(entries[0], TimeSpan.Parse("0:01:52.481"), LapFragmentType.FULL_LAP));
        Assert.Equal("CLASS_BEST", store.GetFragmentTimeIndicator(entries[0], TimeSpan.Parse("0:01:52.305"), LapFragmentType.FULL_LAP));


        // Entry 2 / Class 1
        // Sector 1 - class best: 10.409, entry best: 10.409
        Assert.Equal("", store.GetFragmentTimeIndicator(entries[1], TimeSpan.Parse("0:00:10.410"), LapFragmentType.SECTOR_1));
        Assert.Equal("CLASS_BEST", store.GetFragmentTimeIndicator(entries[1], TimeSpan.Parse("0:00:10.409"), LapFragmentType.SECTOR_1));

        // Sector 2 - class best: 1:02.109, entry best: 1:02.110
        Assert.Equal("", store.GetFragmentTimeIndicator(entries[1], TimeSpan.Parse("0:01:02.111"), LapFragmentType.SECTOR_2));
        Assert.Equal("ENTRY_BEST", store.GetFragmentTimeIndicator(entries[1], TimeSpan.Parse("0:01:02.110"), LapFragmentType.SECTOR_2));
        Assert.Equal("CLASS_BEST", store.GetFragmentTimeIndicator(entries[1], TimeSpan.Parse("0:01:02.109"), LapFragmentType.SECTOR_2));

        // Sector 3 - class best: 39.980, entry best: 39.999
        Assert.Equal("", store.GetFragmentTimeIndicator(entries[1], TimeSpan.Parse("0:00:40.000"), LapFragmentType.SECTOR_3));
        Assert.Equal("ENTRY_BEST", store.GetFragmentTimeIndicator(entries[1], TimeSpan.Parse("0:00:39.999"), LapFragmentType.SECTOR_3));
        Assert.Equal("CLASS_BEST", store.GetFragmentTimeIndicator(entries[1], TimeSpan.Parse("0:00:39.980"), LapFragmentType.SECTOR_3));

        // Full lap - class best: 1:52.305, entry best: 1:52.305
        Assert.Equal("", store.GetFragmentTimeIndicator(entries[1], TimeSpan.Parse("0:01:52.306"), LapFragmentType.FULL_LAP));
        Assert.Equal("CLASS_BEST", store.GetFragmentTimeIndicator(entries[1], TimeSpan.Parse("0:01:52.305"), LapFragmentType.FULL_LAP));


        // Entry 3 / Class 2
        // Sector 1 - class best: 12.158, entry best: 12.158
        Assert.Equal("", store.GetFragmentTimeIndicator(entries[2], TimeSpan.Parse("0:00:12.159"), LapFragmentType.SECTOR_1));
        Assert.Equal("CLASS_BEST", store.GetFragmentTimeIndicator(entries[2], TimeSpan.Parse("0:00:12.158"), LapFragmentType.SECTOR_1));

        // Sector 2 - class best: 1:05.190, entry best: 1:05.200
        Assert.Equal("", store.GetFragmentTimeIndicator(entries[2], TimeSpan.Parse("0:01:05.201"), LapFragmentType.SECTOR_2));
        Assert.Equal("ENTRY_BEST", store.GetFragmentTimeIndicator(entries[2], TimeSpan.Parse("0:01:05.200"), LapFragmentType.SECTOR_2));
        Assert.Equal("CLASS_BEST", store.GetFragmentTimeIndicator(entries[2], TimeSpan.Parse("0:01:05.190"), LapFragmentType.SECTOR_2));

        // Sector 3 - class best: 41.970, entry best: 41.970
        Assert.Equal("", store.GetFragmentTimeIndicator(entries[2], TimeSpan.Parse("0:00:41.971"), LapFragmentType.SECTOR_3));
        Assert.Equal("CLASS_BEST", store.GetFragmentTimeIndicator(entries[2], TimeSpan.Parse("0:00:41.970"), LapFragmentType.SECTOR_3));

        // Full lap - class best: 1:58.400, entry best: 1:58.400
        Assert.Equal("", store.GetFragmentTimeIndicator(entries[2], TimeSpan.Parse("0:01:58.401"), LapFragmentType.FULL_LAP));
        Assert.Equal("CLASS_BEST", store.GetFragmentTimeIndicator(entries[2], TimeSpan.Parse("0:01:58.400"), LapFragmentType.FULL_LAP));


        // Entry 4 / Class 2
        // Sector 1 - class best: 12.158, entry best: 12.160
        Assert.Equal("", store.GetFragmentTimeIndicator(entries[3], TimeSpan.Parse("0:00:12.161"), LapFragmentType.SECTOR_1));
        Assert.Equal("ENTRY_BEST", store.GetFragmentTimeIndicator(entries[3], TimeSpan.Parse("0:00:12.160"), LapFragmentType.SECTOR_1));
        Assert.Equal("CLASS_BEST", store.GetFragmentTimeIndicator(entries[3], TimeSpan.Parse("0:00:12.158"), LapFragmentType.SECTOR_1));

        // Sector 2 - class best: 1:05.190, entry best: 1:05.190
        Assert.Equal("", store.GetFragmentTimeIndicator(entries[3], TimeSpan.Parse("0:01:05.191"), LapFragmentType.SECTOR_2));
        Assert.Equal("CLASS_BEST", store.GetFragmentTimeIndicator(entries[3], TimeSpan.Parse("0:01:05.190"), LapFragmentType.SECTOR_2));

        // Sector 3 - class best: 41.970, entry best: 41.998
        Assert.Equal("", store.GetFragmentTimeIndicator(entries[3], TimeSpan.Parse("0:00:41.999"), LapFragmentType.SECTOR_3));
        Assert.Equal("ENTRY_BEST", store.GetFragmentTimeIndicator(entries[3], TimeSpan.Parse("0:00:41.998"), LapFragmentType.SECTOR_3));
        Assert.Equal("CLASS_BEST", store.GetFragmentTimeIndicator(entries[3], TimeSpan.Parse("0:00:41.970"), LapFragmentType.SECTOR_3));

        // Full lap - class best: 1:58.400, entry best: 1:58.602
        Assert.Equal("", store.GetFragmentTimeIndicator(entries[3], TimeSpan.Parse("0:01:58.603"), LapFragmentType.FULL_LAP));
        Assert.Equal("ENTRY_BEST", store.GetFragmentTimeIndicator(entries[3], TimeSpan.Parse("0:01:58.602"), LapFragmentType.FULL_LAP));
        Assert.Equal("CLASS_BEST", store.GetFragmentTimeIndicator(entries[3], TimeSpan.Parse("0:01:58.400"), LapFragmentType.FULL_LAP));
    }

    private static TestableOpponent[] CreateTestEntries()
    {
        return
        [
            new TestableOpponent
            {
                Name = "Entry1",
                CarNumber = "107",
                TeamName = "Team1",
                CarClass = "Class1",
                CurrentLapTime = TimeSpan.Parse("0:00:10.408"),
                CurrentTimes = new TestableSectorTimes
                {
                    Sector1 = TimeSpan.Parse("0:00:10.410"), // Entry bist
                    Sector2 = TimeSpan.Parse("0:01:02.110"), // nothing
                },
                LastTimes = new TestableSectorTimes
                {
                    Sector1 = TimeSpan.Parse("0:00:10.419"), // nothing
                    Sector2 = TimeSpan.Parse("0:01:02.109"), // Class best
                    Sector3 = TimeSpan.Parse("0:00:39.980"), // Class best
                    FullLap = TimeSpan.Parse("0:01:52.508")  // nothing
                },
                BestTimes = new TestableSectorTimes
                {
                    Sector1 = TimeSpan.Parse("0:00:10.410"),
                    Sector2 = TimeSpan.Parse("0:01:02.109"),
                    Sector3 = TimeSpan.Parse("0:00:39.980"),
                    FullLap = TimeSpan.Parse("0:01:52.481"),
                }
            },
            new TestableOpponent
            {
                Name = "Entry2",
                CarNumber = "999",
                TeamName = "Team2",
                CarClass = "Class1",
                CurrentLapTime = TimeSpan.Parse("0:00:10.408"),
                CurrentTimes = new TestableSectorTimes
                {
                    Sector1 = TimeSpan.Parse("0:00:10.410"), // nothing
                    Sector2 = TimeSpan.Parse("0:01:02.110"), // Entry best
                },
                LastTimes = new TestableSectorTimes
                {
                    Sector1 = TimeSpan.Parse("0:00:10.409"), // Class best
                    Sector2 = TimeSpan.Parse("0:01:02.294"), // nothing
                    Sector3 = TimeSpan.Parse("0:00:39.999"), // Entry best
                    FullLap = TimeSpan.Parse("0:01:52.602")  // nothing
                },
                BestTimes = new TestableSectorTimes
                {
                    Sector1 = TimeSpan.Parse("0:00:10.409"),
                    Sector2 = TimeSpan.Parse("0:01:02.111"),
                    Sector3 = TimeSpan.Parse("0:00:39.999"),
                    FullLap = TimeSpan.Parse("0:01:52.305"),
                }
            },
            new TestableOpponent
            {
                Name = "Entry3",
                CarNumber = "1",
                TeamName = "Team3",
                CarClass = "Class2",
                CurrentLapTime = TimeSpan.Parse("0:00:10.408"),
                CurrentTimes = new TestableSectorTimes
                {
                    Sector1 = TimeSpan.Parse("0:00:12.158"), // Class best
                },
                LastTimes = new TestableSectorTimes
                {
                    Sector1 = TimeSpan.Parse("0:00:12.159"), // nothing
                    Sector2 = TimeSpan.Parse("0:01:05.209"), // nothing
                    Sector3 = TimeSpan.Parse("0:00:41.980"), // nothing
                    FullLap = TimeSpan.Parse("0:01:58.400")  // Class best
                },
                BestTimes = new TestableSectorTimes
                {
                    Sector1 = TimeSpan.Parse("0:00:12.159"),
                    Sector2 = TimeSpan.Parse("0:01:05.200"),
                    Sector3 = TimeSpan.Parse("0:00:41.970"),
                    FullLap = TimeSpan.Parse("0:01:58.400"),
                }
            },
            new TestableOpponent
            {
                Name = "Entry4",
                CarNumber = "30",
                TeamName = "Team4",
                CarClass = "Class2",
                CurrentLapTime = TimeSpan.Parse("0:00:10.408"),
                CurrentTimes = null,
                LastTimes = new TestableSectorTimes
                {
                    Sector1 = TimeSpan.Parse("0:00:12.160"), // Entry best
                    Sector2 = TimeSpan.Parse("0:01:05.190"), // Class best
                    Sector3 = TimeSpan.Parse("0:00:41.999"), // nothing
                    FullLap = TimeSpan.Parse("0:01:58.602")  // Entry best
                },
                BestTimes = new TestableSectorTimes
                {
                    Sector1 = TimeSpan.Parse("0:00:12.161"),
                    Sector2 = TimeSpan.Parse("0:01:05.191"),
                    Sector3 = TimeSpan.Parse("0:00:41.998"),
                    FullLap = TimeSpan.Parse("0:01:58.602"),
                }
            },
        ];
    }
}