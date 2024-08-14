using System;
using System.Linq;
using CrewChiefV4.rFactor2_V2.rFactor2Data;
using GameReaderCommon;
using RfactorReader.RF2;

public class TestableGameDataConverter
{
    public static TestableGameData FromGameData(GameData data)
    {
        var result = new TestableGameData
        {
            GameName = data.GameName,
            GameRunning = data.GameRunning,
            OldData = data.OldData != null ? TestableStatusDataBaseConverter.FromStatusDataBase(data.OldData) : null,
            NewData = data.NewData != null ? TestableStatusDataBaseConverter.FromStatusDataBase(data.NewData) : null
        };

        return GameSpecificConvertions(result, data);
    }

    private static TestableGameData GameSpecificConvertions(TestableGameData result, GameData originData)
    {
        if (result.GameName == "RFactor2" || result.GameName == "LMU")
        {
            if (result.OldData != null)
            {
                RF2SpecificConvertions(result.OldData.Opponents, (originData as GameData<WrapV2>).GameOldData.Raw.telemetry.mVehicles);
            }

            if (result.NewData != null)
            {
                RF2SpecificConvertions(result.NewData.Opponents, (originData as GameData<WrapV2>).GameNewData.Raw.telemetry.mVehicles);
            }
        }
        else if (result.GameName == "AssettoCorsaCompetizione")
        {
            if (result.OldData != null)
            {
                SetCarClass(result.OldData.Opponents, "GT3");
            }

            if (result.NewData != null)
            {
                SetCarClass(result.NewData.Opponents, "GT3");
            }
        }
        else if (result.GameName == "F12023")
        {
            if (result.OldData != null)
            {
                SetCarClass(result.OldData.Opponents, "F1");
            }

            if (result.NewData != null)
            {
                SetCarClass(result.NewData.Opponents, "F1");
            }
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

    private static TestableOpponent[] RF2SpecificConvertions(TestableOpponent[] entries, rF2VehicleTelemetry[] rF2Entries)
    {
        foreach (var entry in entries)
        {
            foreach (var rF2Entry in rF2Entries)
            {
                var rF2Name = new string(System.Text.Encoding.Default.GetString(rF2Entry.mVehicleName).Where(c => !char.IsControl(c)).ToArray());

                if (rF2Name.Contains("#" + entry.CarNumber + " "))
                {
                    entry.FuelLoad = rF2Entry.mFuel;
                    entry.FuelCapacity = rF2Entry.mFuelCapacity;
                }
            };
        }

        return entries;
    }
}