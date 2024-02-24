namespace PainSaber
{
    /// <summary>
    /// Lists all states that the plugin my be in.
    /// </summary>
    public enum PainSaberPluginStatus
    {
        ValidatingApiKey,
        ValidatingDevice,
        ConnectingToDevice,
        ConnectedToDevice,


        // failstates
        InvalidApiKey,
        InvalidDevice,
        DeviceIsOffline,
        NoDevicesAvailable,
        DeviceConnectionFailed
    }
}