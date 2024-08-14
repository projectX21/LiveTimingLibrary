using System.ComponentModel;

public interface IPluginManager
{
    void AddProperty<T>(string name, System.Type pluginType, T value);

    void ResetAll();

    event PropertyChangedEventHandler PropertyChanged;
}