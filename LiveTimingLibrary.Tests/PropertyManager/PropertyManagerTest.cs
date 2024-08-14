using Moq;

public class PropertyManagerTest
{
    [Fact]
    public void TestAddAllPropertiesForTheFirst50Entries()
    {
        var mockPluginManager = new Mock<IPluginManager>();
        var manager = new PropertyManager(mockPluginManager.Object);
        var type = manager.GetType();

        for (var i = 1; i <= 50; i++)
        {
            TestProperty(mockPluginManager, i, "CarNumber", type, "", Times.Once());
            TestProperty(mockPluginManager, i, "Name", type, "", Times.Once());
            TestProperty(mockPluginManager, i, "TeamName", type, "", Times.Once());
            TestProperty(mockPluginManager, i, "CarName", type, "", Times.Once());
            TestProperty(mockPluginManager, i, "CarClass", type, "", Times.Once());
            TestProperty(mockPluginManager, i, "Manufacturer", type, "", Times.Once());
            TestProperty(mockPluginManager, i, "Position", type, "", Times.Once());
            TestProperty(mockPluginManager, i, "CurrentLapNumber", type, 0, Times.Once());
            TestProperty(mockPluginManager, i, "CurrentSector", type, 0, Times.Once());
            TestProperty(mockPluginManager, i, "GapToFirst", type, "", Times.Once());
            TestProperty(mockPluginManager, i, "GapToInFront", type, "", Times.Once());
            TestProperty(mockPluginManager, i, "Sector1Time", type, "", Times.Once());
            TestProperty(mockPluginManager, i, "Sector1Indicator", type, "", Times.Once());
            TestProperty(mockPluginManager, i, "Sector2Time", type, "", Times.Once());
            TestProperty(mockPluginManager, i, "Sector2Indicator", type, "", Times.Once());
            TestProperty(mockPluginManager, i, "Sector3Time", type, "", Times.Once());
            TestProperty(mockPluginManager, i, "Sector3Indicator", type, "", Times.Once());
            TestProperty(mockPluginManager, i, "LapTime", type, "", Times.Once());
            TestProperty(mockPluginManager, i, "LapTimeIndicator", type, "", Times.Once());
            TestProperty(mockPluginManager, i, "BestLapTime", type, "", Times.Once());
            TestProperty(mockPluginManager, i, "BestLapTimeIndicator", type, "", Times.Once());
            TestProperty(mockPluginManager, i, "InPits", type, false, Times.Once());
            TestProperty(mockPluginManager, i, "PitStopsTotal", type, 0, Times.Once());
            TestProperty(mockPluginManager, i, "PitStopsTotalDuration", type, 0, Times.Once());
            TestProperty(mockPluginManager, i, "PitStopLastDuration", type, 0, Times.Once());
            TestProperty(mockPluginManager, i, "LapsInCurrentStint", type, 0, Times.Once());
            TestProperty(mockPluginManager, i, "TyreCompound", type, "", Times.Once());
            TestProperty(mockPluginManager, i, "FuelCapacity", type, "", Times.Once());
            TestProperty(mockPluginManager, i, "FuelLoad", type, "", Times.Once());
        }
    }

    [Fact]
    public void TestAdd()
    {
        var mockPluginManager = new Mock<IPluginManager>();
        var manager = new PropertyManager(mockPluginManager.Object);
        var type = manager.GetType();

        // There are two methods for adding a property. One is for driver based properties (have a pos as a parameter)
        // and one for general, non driver based, properties.

        // at first the the driver based properties
        manager.Add(5, "Test", "ABC");
        mockPluginManager.Verify(m => m.AddProperty("CHB.Driver5.Test", type, "ABC"));

        manager.Add(9, "TestNo", 1234);
        mockPluginManager.Verify(m => m.AddProperty("CHB.Driver9.TestNo", type, 1234));

        manager.Add(7, "TestBool", true);
        mockPluginManager.Verify(m => m.AddProperty("CHB.Driver7.TestBool", type, true));

        // and now the general, non driver based, properties
        manager.Add("General", "Test");
        mockPluginManager.Verify(m => m.AddProperty("CHB.General", type, "Test"));

        manager.Add("GeneralNo", 561);
        mockPluginManager.Verify(m => m.AddProperty("CHB.GeneralNo", type, 561));

        manager.Add("GeneralBool", false);
        mockPluginManager.Verify(m => m.AddProperty("CHB.GeneralBool", type, false));
    }


    [Fact]
    public void TestReset()
    {
        var mockPluginManager = new Mock<IPluginManager>();
        var manager = new PropertyManager(mockPluginManager.Object);
        var type = manager.GetType();

        // Reset only calls AddAll() again. Therefore the add method for every single property per entry should be called twice.
        manager.ResetAll();

        for (var i = 1; i <= 50; i++)
        {
            TestProperty(mockPluginManager, i, "CarNumber", type, "", Times.Exactly(2));
            TestProperty(mockPluginManager, i, "Name", type, "", Times.Exactly(2));
            TestProperty(mockPluginManager, i, "TeamName", type, "", Times.Exactly(2));
            TestProperty(mockPluginManager, i, "CarName", type, "", Times.Exactly(2));
            TestProperty(mockPluginManager, i, "CarClass", type, "", Times.Exactly(2));
            TestProperty(mockPluginManager, i, "Manufacturer", type, "", Times.Exactly(2));
            TestProperty(mockPluginManager, i, "Position", type, "", Times.Exactly(2));
            TestProperty(mockPluginManager, i, "CurrentLapNumber", type, 0, Times.Exactly(2));
            TestProperty(mockPluginManager, i, "CurrentSector", type, 0, Times.Exactly(2));
            TestProperty(mockPluginManager, i, "GapToFirst", type, "", Times.Exactly(2));
            TestProperty(mockPluginManager, i, "GapToInFront", type, "", Times.Exactly(2));
            TestProperty(mockPluginManager, i, "Sector1Time", type, "", Times.Exactly(2));
            TestProperty(mockPluginManager, i, "Sector1Indicator", type, "", Times.Exactly(2));
            TestProperty(mockPluginManager, i, "Sector2Time", type, "", Times.Exactly(2));
            TestProperty(mockPluginManager, i, "Sector2Indicator", type, "", Times.Exactly(2));
            TestProperty(mockPluginManager, i, "Sector3Time", type, "", Times.Exactly(2));
            TestProperty(mockPluginManager, i, "Sector3Indicator", type, "", Times.Exactly(2));
            TestProperty(mockPluginManager, i, "LapTime", type, "", Times.Exactly(2));
            TestProperty(mockPluginManager, i, "LapTimeIndicator", type, "", Times.Exactly(2));
            TestProperty(mockPluginManager, i, "BestLapTime", type, "", Times.Exactly(2));
            TestProperty(mockPluginManager, i, "BestLapTimeIndicator", type, "", Times.Exactly(2));
            TestProperty(mockPluginManager, i, "InPits", type, false, Times.Exactly(2));
            TestProperty(mockPluginManager, i, "PitStopsTotal", type, 0, Times.Exactly(2));
            TestProperty(mockPluginManager, i, "PitStopsTotalDuration", type, 0, Times.Exactly(2));
            TestProperty(mockPluginManager, i, "PitStopLastDuration", type, 0, Times.Exactly(2));
            TestProperty(mockPluginManager, i, "LapsInCurrentStint", type, 0, Times.Exactly(2));
            TestProperty(mockPluginManager, i, "TyreCompound", type, "", Times.Exactly(2));
            TestProperty(mockPluginManager, i, "FuelCapacity", type, "", Times.Exactly(2));
            TestProperty(mockPluginManager, i, "FuelLoad", type, "", Times.Exactly(2));
        }
    }

    private static void TestProperty<T>(Mock<IPluginManager> mock, int index, string key, Type type, T value, Times times)
    {
        var property = "CHB.Driver" + index.ToString() + "." + key;
        mock.Verify(m => m.AddProperty(property, type, value), times);
    }
}