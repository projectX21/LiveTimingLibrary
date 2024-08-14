using System;

public class RaceEntryProcessor : IRaceEntryProcessor
{
    private readonly IPropertyManager _propertyManager;

    private readonly IRaceEventHandler _raceEventHandler;

    private SessionType _sessionType;

    private IFastestFragmentTimesStore _fastestFragmentTimesStore;

    private TestableOpponent _newEntryData;

    private TestableOpponent _oldEntryData;

    private TestableOpponent _leaderData;

    private TestableOpponent _inFrontData;

    public RaceEntryProcessor(IPropertyManager propertyManager, IRaceEventHandler raceEventHandler)
    {
        _propertyManager = propertyManager;
        _raceEventHandler = raceEventHandler;
    }


    public void Process(
        SessionType sessionType,
        TestableOpponent newData, TestableOpponent oldData,
        TestableOpponent leaderData, TestableOpponent inFrontData,
        IFastestFragmentTimesStore timesStore
    )
    {
        _sessionType = sessionType;
        _newEntryData = newData;
        _oldEntryData = oldData;
        _leaderData = leaderData;
        _inFrontData = inFrontData;
        _fastestFragmentTimesStore = timesStore;

        if (_newEntryData.IsPlayer)
        {
            CreateEventWhenPlayerFinishedLap();
        }

        if (sessionType == SessionType.Race)
        {
            ProcessPitStopEvents();
        }

        UpdateProperties();
    }

    private void CreateEventWhenPlayerFinishedLap()
    {
        if (_oldEntryData == null)
        {
            return;
        }

        if (_oldEntryData.CurrentLap < _newEntryData.CurrentLap && _newEntryData.CurrentLap > 0)
        {
            _raceEventHandler.AddEvent(
                new PlayerFinishedLapEvent(
                    _newEntryData.CurrentLap - 1 ?? 1,
                    _newEntryData.LastTimes.GetByLapFragmentType(LapFragmentType.FULL_LAP) ?? TimeSpan.Zero
                )
            );
        }
    }

    private void ProcessPitStopEvents()
    {
        if (_oldEntryData == null)
        {
            return;
        }

        if (_oldEntryData.IsInPit == false && _newEntryData.IsInPit == true)
        {
            _raceEventHandler.AddEvent(
                new PitEvent(RaceEventType.PitIn, _newEntryData.Id, _newEntryData.CurrentLap ?? 1)
            );
        }
        else if (_oldEntryData.IsInPit == true && _newEntryData.IsInPit == false)
        {
            _raceEventHandler.AddEvent(
                new PitEvent(RaceEventType.PitOut, _newEntryData.Id, _newEntryData.CurrentLap ?? 1)
            );
        }
    }

    private void UpdateProperties()
    {
        UpdateGeneralProperties();
        UpdateLapFragmentProperties();
        UpdatePitProperties();
    }

    private void UpdateGeneralProperties()
    {
        UpdateProperty(PropertyManagerConstants.CAR_NUMBER, _newEntryData.CarNumber);
        UpdateProperty(PropertyManagerConstants.NAME, _newEntryData.Name);
        UpdateProperty(PropertyManagerConstants.TEAM_NAME, _newEntryData.TeamName);
        UpdateProperty(PropertyManagerConstants.CAR_NAME, _newEntryData.CarName);
        UpdateProperty(PropertyManagerConstants.CAR_CLASS, _newEntryData.CarClass);
        UpdateProperty(PropertyManagerConstants.MANUFACTURER, _newEntryData.Manufacturer);
        UpdateProperty(PropertyManagerConstants.POSITION, _newEntryData.Position);
        UpdateProperty(PropertyManagerConstants.CURRENT_LAP_NUMBER, _newEntryData.CurrentLap);
        UpdateProperty(PropertyManagerConstants.CURRENT_SECTOR, _newEntryData.CurrentSector);
        UpdateProperty(PropertyManagerConstants.GAP_TO_FIRST, GapCalculator.Calc(_sessionType, _newEntryData, _leaderData));
        UpdateProperty(PropertyManagerConstants.GAP_TO_IN_FRONT, GapCalculator.Calc(_sessionType, _newEntryData, _inFrontData));
        UpdateProperty(PropertyManagerConstants.TYRE_COMPOUND, _newEntryData.FrontTyreCompound);
        UpdateProperty(PropertyManagerConstants.FUEL_CAPACITY, _newEntryData.FuelCapacity);
        UpdateProperty(PropertyManagerConstants.FUEL_LOAD, _newEntryData.FuelLoad);
    }

    private void UpdateLapFragmentProperties()
    {
        // init the sector times from the last lap
        var s1Time = _newEntryData.GetLastLapFragmentTime(LapFragmentType.SECTOR_1);
        var s1Indicator = _fastestFragmentTimesStore.GetLastLapFragmentTimeIndicator(_newEntryData, LapFragmentType.SECTOR_1);

        var s2Time = _newEntryData.GetLastLapFragmentTime(LapFragmentType.SECTOR_2);
        var s2Indicator = _fastestFragmentTimesStore.GetLastLapFragmentTimeIndicator(_newEntryData, LapFragmentType.SECTOR_2);

        var s3Time = _newEntryData.GetLastLapFragmentTime(LapFragmentType.SECTOR_3);
        var s3Indicator = _fastestFragmentTimesStore.GetLastLapFragmentTimeIndicator(_newEntryData, LapFragmentType.SECTOR_3);

        var lapTime = _newEntryData.GetLastLapFragmentTime(LapFragmentType.FULL_LAP);
        var lapIndicator = _fastestFragmentTimesStore.GetLastLapFragmentTimeIndicator(_newEntryData, LapFragmentType.FULL_LAP);

        // Not every sim fills the current lap sector times. For example ACC doesn't do it on every track.
        // Therefore we only use the current lap sector times when they are available. Otherwise we only show the sector times of the last lap
        var currentLapS1Time = _newEntryData.GetCurrentLapFragmentTime(LapFragmentType.SECTOR_1);

        if (_newEntryData.CurrentSector > 1 && currentLapS1Time != null)
        {
            s1Time = currentLapS1Time;
            s1Indicator = _fastestFragmentTimesStore.GetCurrentLapFragmentTimeIndicator(_newEntryData, LapFragmentType.SECTOR_1);

            s2Time = null;
            s2Indicator = "";

            s3Time = null;
            s3Indicator = "";

            var currentLapS2Time = _newEntryData.GetCurrentLapFragmentTime(LapFragmentType.SECTOR_2);

            if (_newEntryData.CurrentSector > 2 && currentLapS2Time != null)
            {
                s2Time = currentLapS2Time;
                s2Indicator = _fastestFragmentTimesStore.GetCurrentLapFragmentTimeIndicator(_newEntryData, LapFragmentType.SECTOR_2);
            }
        }

        var bestLapTime = _newEntryData.BestTimes.GetByLapFragmentType(LapFragmentType.FULL_LAP);
        var bestLapIndicator = _fastestFragmentTimesStore.GetFragmentTimeIndicator(_newEntryData, bestLapTime, LapFragmentType.FULL_LAP);

        if (bestLapTime == null)
        {
            bestLapTime = lapTime;
            bestLapIndicator = lapIndicator;
        }

        // We don't want to highlight the entry best lap time, because otherwise every best lap of any entry will be highlighted...
        if (bestLapIndicator == "ENTRY_BEST") bestLapIndicator = "";

        UpdateProperty(PropertyManagerConstants.SECTOR_1_TIME, TimeSpanFormatter.Format(s1Time));
        UpdateProperty(PropertyManagerConstants.SECTOR_1_INDICATOR, s1Indicator);
        UpdateProperty(PropertyManagerConstants.SECTOR_2_TIME, TimeSpanFormatter.Format(s2Time));
        UpdateProperty(PropertyManagerConstants.SECTOR_2_INDICATOR, s2Indicator);
        UpdateProperty(PropertyManagerConstants.SECTOR_3_TIME, TimeSpanFormatter.Format(s3Time));
        UpdateProperty(PropertyManagerConstants.SECTOR_3_INDICATOR, s3Indicator);
        UpdateProperty(PropertyManagerConstants.LAP_TIME, TimeSpanFormatter.Format(lapTime));
        UpdateProperty(PropertyManagerConstants.LAP_TIME_INDICATOR, lapIndicator);
        UpdateProperty(PropertyManagerConstants.BEST_LAP_TIME, TimeSpanFormatter.Format(bestLapTime));
        UpdateProperty(PropertyManagerConstants.BEST_LAP_TIME_INDICATOR, bestLapIndicator);
    }

    private void UpdatePitProperties()
    {
        var pitData = _raceEventHandler.GetPitDataByEntryId(_newEntryData.Id);

        var total = 0;
        var totalDuration = 0;
        int? lastDuration = null;
        var lapsInCurrentStint = _newEntryData.CurrentLap;

        if (pitData != null)
        {
            total = pitData.TotalPitStops;
            totalDuration = pitData.TotalPitDuration;
            lastDuration = pitData.LastPitDuration;

            var currentLap = _newEntryData.CurrentLap ?? 1;
            var lapNumberLastPitIn = pitData.LapNumberLastPitOut != null ? pitData.LapNumberLastPitOut - 1 : 0;
            lapsInCurrentStint = currentLap - lapNumberLastPitIn;
        }

        UpdateProperty(PropertyManagerConstants.IN_PITS, _newEntryData.IsInPit);
        UpdateProperty(PropertyManagerConstants.PIT_STOPS_TOTAL, total);
        UpdateProperty(PropertyManagerConstants.PIT_STOPS_TOTAL_DURATION, totalDuration);
        UpdateProperty(PropertyManagerConstants.PIT_STOPS_LAST_DURATION, lastDuration);
        UpdateProperty(PropertyManagerConstants.LAPS_IN_CURRENT_STINT, lapsInCurrentStint);
    }

    private void UpdateProperty<U>(string key, U value)
    {
        //SimHub.Logging.Current.Info("Update key: " + key + ": " + value);
        _propertyManager.Add(_newEntryData.Position, key, value);
    }
}