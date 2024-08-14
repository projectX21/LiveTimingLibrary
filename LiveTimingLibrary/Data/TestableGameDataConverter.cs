using System;
using System.Linq;

public class TestableGameDataConverter
{
    public static TestableGameData FromGameData(IGameData data)
    {
        var result = new TestableGameData
        {
            GameName = data.GameName,
            GameRunning = data.GameRunning,
            OldData = TestableStatusDataBaseConverter.FromStatusDataBase(data.OldData),
            NewData = TestableStatusDataBaseConverter.FromStatusDataBase(data.NewData),
        };

        return GameSpecificConvertions(result, data);
    }

    private static TestableGameData GameSpecificConvertions(TestableGameData result, IGameData originData)
    {
        if (result.GameName == "RFactor2" || result.GameName == "LMU")
        {
            RF2SpecificConvertions(result.OldData.Opponents, (originData as IGameData<IWrapV2>).GameOldData.Raw.telemetry.mVehicles);
            RF2SpecificConvertions(result.NewData.Opponents, (originData as IGameData<IWrapV2>).GameNewData.Raw.telemetry.mVehicles);
        }
        else if (result.GameName == "AssettoCorsaCompetizione")
        {
            SetCarClass(result.OldData.Opponents, "GT3");
            SetCarClass(result.NewData.Opponents, "GT3");
        }
        else if (result.GameName == "F12023")
        {
            SetCarClass(result.OldData.Opponents, "F1");
            SetCarClass(result.NewData.Opponents, "F1");
        }

        return result;
    }

    private static TestableOpponent[] SetCarClass(TestableOpponent[] entries, string carClass)
    {
        foreach (var entry in entries)
        {
            entry.CarClass = carClass;
        }

        return entries;
    }

    private static TestableOpponent[] RF2SpecificConvertions(TestableOpponent[] entries, IrF2VehicleTelemetry[] rF2Entries)
    {
        foreach (var entry in entries)
        {
            IrF2VehicleTelemetry rF2Entry = Array.Find(rF2Entries, e =>
             {
                 var rF2Name = new string(System.Text.Encoding.Default.GetString(e.mVehicleName).Where(c => !char.IsControl(c)).ToArray());
                 return rF2Name.Contains("#" + entry.CarNumber + " ");
             });

            if (rF2Entry != null)
            {
                entry.FuelLoad = rF2Entry.mFuel;
                entry.FuelCapacity = rF2Entry.mFuelCapacity;
            }
        }

        return entries;
    }
}