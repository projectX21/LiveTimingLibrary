using System.Text;
using Moq;

public class TestableGameDataConverterTest
{
    [Fact]
    public void TestFromGameData()
    {
        var result = TestableGameDataConverter.FromGameData(CreateGameDataMock().Object);
        Assert.Equal(CreateExpectedResult(), result);
    }

    [Fact]
    public void TestAccSpecificConvertions()
    {
        TestSetOfCarClass("AssettoCorsaCompetizione", "GT3");
    }

    [Fact]
    public void TestF12023SpecificConvertions()
    {
        TestSetOfCarClass("F12023", "F1");
    }

    [Fact]
    public void TestRF2SpecificConvertions()
    {
        var mock = new Mock<IGameData<IWrapV2>>();
        mock.SetupGet(m => m.GameName).Returns("RFactor2");
        mock.SetupGet(m => m.GameRunning).Returns(true);
        mock.SetupGet(m => m.OldData).Returns(TestableStatusDataBaseConverterTest.CreateStatusDataBaseMock("Old").Object);
        mock.SetupGet(m => m.NewData).Returns(TestableStatusDataBaseConverterTest.CreateStatusDataBaseMock("New").Object);
        mock.SetupGet(m => m.GameOldData).Returns(CreateRF2StatusDataMock(10.8, 98.1).Object);
        mock.SetupGet(m => m.GameNewData).Returns(CreateRF2StatusDataMock(10.1, 97.5).Object);

        var result = TestableGameDataConverter.FromGameData(mock.Object);

        // adjust the expected result by setting the RF2 specific fuel values
        var expected = CreateExpectedResult();
        expected.GameName = "RFactor2";

        expected.OldData.Opponents[0].FuelLoad = 10.8;
        expected.OldData.Opponents[0].FuelCapacity = 99.9;
        expected.OldData.Opponents[2].FuelLoad = 98.1;
        expected.OldData.Opponents[2].FuelCapacity = 105.1;

        expected.NewData.Opponents[0].FuelLoad = 10.1;
        expected.NewData.Opponents[0].FuelCapacity = 99.9;
        expected.NewData.Opponents[2].FuelLoad = 97.5;
        expected.NewData.Opponents[2].FuelCapacity = 105.1;

        Assert.Equal(expected, result);

        // Same handling for LMU
        mock.SetupGet(m => m.GameName).Returns("LMU");
        result = TestableGameDataConverter.FromGameData(mock.Object);
        expected.GameName = "LMU";
        Assert.Equal(expected, result);
    }

    public static Mock<IGameData> CreateGameDataMock()
    {
        var mock = new Mock<IGameData>();
        mock.SetupGet(m => m.GameName).Returns("GameName");
        mock.SetupGet(m => m.GameRunning).Returns(true);
        mock.SetupGet(m => m.OldData).Returns(TestableStatusDataBaseConverterTest.CreateStatusDataBaseMock("Old").Object);
        mock.SetupGet(m => m.NewData).Returns(TestableStatusDataBaseConverterTest.CreateStatusDataBaseMock("New").Object);
        return mock;
    }

    public static TestableGameData CreateExpectedResult()
    {
        return new TestableGameData
        {
            GameName = "GameName",
            GameRunning = true,
            OldData = TestableStatusDataBaseConverterTest.CreateExpectedResult("Old"),
            NewData = TestableStatusDataBaseConverterTest.CreateExpectedResult("New")
        };
    }

    public static Mock<IStatusData<IWrapV2>> CreateRF2StatusDataMock(double fuel1, double fuel2)
    {
        var mockF2VehicleTelemetry1 = new Mock<IrF2VehicleTelemetry>();
        mockF2VehicleTelemetry1.SetupGet(m => m.mFuel).Returns(fuel1);
        mockF2VehicleTelemetry1.SetupGet(m => m.mFuelCapacity).Returns(99.9);
        mockF2VehicleTelemetry1.SetupGet(m => m.mVehicleName).Returns(Encoding.ASCII.GetBytes("#107 Entry1"));

        var mockF2VehicleTelemetry2 = new Mock<IrF2VehicleTelemetry>();
        mockF2VehicleTelemetry2.SetupGet(m => m.mFuel).Returns(fuel2);
        mockF2VehicleTelemetry2.SetupGet(m => m.mFuelCapacity).Returns(105.1);
        mockF2VehicleTelemetry2.SetupGet(m => m.mVehicleName).Returns(Encoding.ASCII.GetBytes("#1 Entry3"));

        var mockRF2Telemetry = new Mock<IrF2Telemetry>();
        mockRF2Telemetry.SetupGet(m => m.mVehicles).Returns([mockF2VehicleTelemetry1.Object, mockF2VehicleTelemetry2.Object]);

        var mockWrapV2 = new Mock<IWrapV2>();
        mockWrapV2.SetupGet(m => m.telemetry).Returns(mockRF2Telemetry.Object);

        var mock = new Mock<IStatusData<IWrapV2>>();
        mock.SetupGet(m => m.Raw).Returns(mockWrapV2.Object);

        return mock;
    }

    private static void TestSetOfCarClass(string gameName, string carClass)
    {
        var mock = CreateGameDataMock();
        mock.SetupGet(m => m.GameName).Returns(gameName);

        var expected = CreateExpectedResult();
        expected.GameName = gameName;

        expected.OldData.Opponents[0].CarClass = carClass;
        expected.OldData.Opponents[1].CarClass = carClass;
        expected.OldData.Opponents[2].CarClass = carClass;

        expected.NewData.Opponents[0].CarClass = carClass;
        expected.NewData.Opponents[1].CarClass = carClass;
        expected.NewData.Opponents[2].CarClass = carClass;

        var result = TestableGameDataConverter.FromGameData(mock.Object);
        Assert.Equal(expected, result);
    }
}