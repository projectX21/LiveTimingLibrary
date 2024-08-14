using SimHub.Plugins;

public class PropertyManager : IPropertyManager
{
    public PropertyManager()
    {
        AddAll();
    }

    public void Add<T>(string key, T value)
    {
        AddInPluginManager(PropertyManagerConstants.PREFIX + key, value);
    }

    public void Add<T>(int pos, string key, T value)
    {
        Add("Driver" + pos + "." + key, value);
    }

    public virtual void AddInPluginManager<T>(string key, T value)
    {
        PluginManager.GetInstance().AddProperty(key, GetType(), value);
    }

    public void ResetAll()
    {
        AddAll();
    }

    private void AddAll()
    {
        for (int i = 1; i <= 50; i++)
        {
            Add(i, PropertyManagerConstants.CAR_NUMBER, "");
            Add(i, PropertyManagerConstants.NAME, "");
            Add(i, PropertyManagerConstants.TEAM_NAME, "");
            Add(i, PropertyManagerConstants.CAR_NAME, "");
            Add(i, PropertyManagerConstants.CAR_CLASS, "");
            Add(i, PropertyManagerConstants.MANUFACTURER, "");
            Add(i, PropertyManagerConstants.POSITION, "");
            Add(i, PropertyManagerConstants.CURRENT_LAP_NUMBER, 0);
            Add(i, PropertyManagerConstants.CURRENT_SECTOR, 0);
            Add(i, PropertyManagerConstants.GAP_TO_FIRST, "");
            Add(i, PropertyManagerConstants.GAP_TO_IN_FRONT, "");
            Add(i, PropertyManagerConstants.SECTOR_1_TIME, "");
            Add(i, PropertyManagerConstants.SECTOR_1_INDICATOR, "");
            Add(i, PropertyManagerConstants.SECTOR_2_TIME, "");
            Add(i, PropertyManagerConstants.SECTOR_2_INDICATOR, "");
            Add(i, PropertyManagerConstants.SECTOR_3_TIME, "");
            Add(i, PropertyManagerConstants.SECTOR_3_INDICATOR, "");
            Add(i, PropertyManagerConstants.LAP_TIME, "");
            Add(i, PropertyManagerConstants.LAP_TIME_INDICATOR, "");
            Add(i, PropertyManagerConstants.BEST_LAP_TIME, "");
            Add(i, PropertyManagerConstants.BEST_LAP_TIME_INDICATOR, "");
            Add(i, PropertyManagerConstants.IN_PITS, false);
            Add(i, PropertyManagerConstants.PIT_STOPS_TOTAL, 0);
            Add(i, PropertyManagerConstants.PIT_STOPS_TOTAL_DURATION, 0);
            Add(i, PropertyManagerConstants.PIT_STOPS_LAST_DURATION, 0);
            Add(i, PropertyManagerConstants.LAPS_IN_CURRENT_STINT, 0);
            Add(i, PropertyManagerConstants.TYRE_COMPOUND, "");
            Add(i, PropertyManagerConstants.FUEL_CAPACITY, "");
            Add(i, PropertyManagerConstants.FUEL_LOAD, "");
        }
    }
}