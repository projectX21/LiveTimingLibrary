using Moq;

public class PropertyManagerTest
{
    [Fact]
    public void TestAdd()
    {
        var mockPropertyManager = new Mock<PropertyManager>();
        var type = mockPropertyManager.GetType();

        // There are two methods for adding a property. One is for driver based properties (have a pos as a parameter)
        // and one for general, non driver based, properties.

        // at first the the driver based properties
        mockPropertyManager.Object.Add(5, "Test", "ABC");
        mockPropertyManager.Verify(m => m.AddInPluginManager("CHB.Driver5.Test", "ABC"));

        mockPropertyManager.Object.Add(9, "TestNo", 1234);
        mockPropertyManager.Verify(m => m.AddInPluginManager("CHB.Driver9.TestNo", 1234));

        mockPropertyManager.Object.Add(7, "TestBool", true);
        mockPropertyManager.Verify(m => m.AddInPluginManager("CHB.Driver7.TestBool", true));

        // and now the general, non driver based, properties
        mockPropertyManager.Object.Add("General", "Test");
        mockPropertyManager.Verify(m => m.AddInPluginManager("CHB.General", "Test"));

        mockPropertyManager.Object.Add("GeneralNo", 561);
        mockPropertyManager.Verify(m => m.AddInPluginManager("CHB.GeneralNo", 561));

        mockPropertyManager.Object.Add("GeneralBool", false);
        mockPropertyManager.Verify(m => m.AddInPluginManager("CHB.GeneralBool", false));
    }

    [Fact]
    public void TestReset()
    {
        var mockPropertyManager = new Mock<PropertyManager>();
        var type = mockPropertyManager.GetType();

        // Reset only calls AddAll() again. Therefore the add method for every single property per entry should be called twice.
        mockPropertyManager.Object.ResetAll();

        for (var i = 1; i <= 50; i++)
        {
            TestProperty(mockPropertyManager, i, "CarNumber", type, "", Times.Exactly(2));
            TestProperty(mockPropertyManager, i, "Name", type, "", Times.Exactly(2));
            TestProperty(mockPropertyManager, i, "TeamName", type, "", Times.Exactly(2));
            TestProperty(mockPropertyManager, i, "CarName", type, "", Times.Exactly(2));
            TestProperty(mockPropertyManager, i, "CarClass", type, "", Times.Exactly(2));
            TestProperty(mockPropertyManager, i, "Position", type, "", Times.Exactly(2));
            TestProperty(mockPropertyManager, i, "CurrentLapNumber", type, 0, Times.Exactly(2));
            TestProperty(mockPropertyManager, i, "CurrentSector", type, 0, Times.Exactly(2));
            TestProperty(mockPropertyManager, i, "GapToFirst", type, "", Times.Exactly(2));
            TestProperty(mockPropertyManager, i, "GapToInFront", type, "", Times.Exactly(2));
            TestProperty(mockPropertyManager, i, "Sector1Time", type, "", Times.Exactly(2));
            TestProperty(mockPropertyManager, i, "Sector1Indicator", type, "", Times.Exactly(2));
            TestProperty(mockPropertyManager, i, "Sector2Time", type, "", Times.Exactly(2));
            TestProperty(mockPropertyManager, i, "Sector2Indicator", type, "", Times.Exactly(2));
            TestProperty(mockPropertyManager, i, "Sector3Time", type, "", Times.Exactly(2));
            TestProperty(mockPropertyManager, i, "Sector3Indicator", type, "", Times.Exactly(2));
            TestProperty(mockPropertyManager, i, "LapTime", type, "", Times.Exactly(2));
            TestProperty(mockPropertyManager, i, "LapTimeIndicator", type, "", Times.Exactly(2));
            TestProperty(mockPropertyManager, i, "BestLapTime", type, "", Times.Exactly(2));
            TestProperty(mockPropertyManager, i, "BestLapTimeIndicator", type, "", Times.Exactly(2));
            TestProperty(mockPropertyManager, i, "InPits", type, false, Times.Exactly(2));
            TestProperty(mockPropertyManager, i, "PitStopsTotal", type, 0, Times.Exactly(2));
            TestProperty(mockPropertyManager, i, "PitStopsTotalDuration", type, 0, Times.Exactly(2));
            TestProperty(mockPropertyManager, i, "PitStopLastDuration", type, 0, Times.Exactly(2));
            TestProperty(mockPropertyManager, i, "LapsInCurrentStint", type, 0, Times.Exactly(2));
            TestProperty(mockPropertyManager, i, "TyreCompound", type, "", Times.Exactly(2));
            TestProperty(mockPropertyManager, i, "FuelCapacity", type, "", Times.Exactly(2));
            TestProperty(mockPropertyManager, i, "FuelLoad", type, "", Times.Exactly(2));
        }
    }

    private static void TestProperty<T>(Mock<PropertyManager> mock, int index, string key, Type type, T value, Times times)
    {
        var property = "CHB.Driver" + index.ToString() + "." + key;
        mock.Verify(m => m.AddInPluginManager(property, value), times);
    }
}