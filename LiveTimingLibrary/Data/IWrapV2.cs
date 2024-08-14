public interface IrF2VehicleTelemetry
{
    byte[] mVehicleName { get; set; }

    double mFuel { get; set; }

    double mFuelCapacity { get; set; }
}

public interface IrF2Telemetry
{
    IrF2VehicleTelemetry[] mVehicles { get; set; }
}

public interface IWrapV2
{
    IrF2Telemetry telemetry { get; set; }
}
