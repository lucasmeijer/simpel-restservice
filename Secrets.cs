static class Secrets
{
    static ConfigurationManager? _configurationManager;

    public static string Get(string key)
    {
        var cm = _configurationManager ?? throw new ApplicationException("ConfigurationManager not set");
        return cm[key] ?? throw new ApplicationException($"The configuration entry {key} has no value"); 
    }

    public static void Intialize(ConfigurationManager configurationManager)
    {
        _configurationManager = configurationManager;
    }
}