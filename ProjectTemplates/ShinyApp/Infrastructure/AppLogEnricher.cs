using Microsoft.Extensions.Diagnostics.Enrichment;

namespace ShinyApp.Infrastructure;


public class AppLogEnricher(
    // IAuthenticationService auth,
    IAppInfo appInfo,
    IDeviceDisplay display,
    IDeviceInfo device
) : ILogEnricher
{
    public void Enrich(IEnrichmentTagCollector collector)
    {
        // if (auth.Identity != null)
        //     collector.Add("UserId", auth.Identity.UserId);
        
        collector.Add("AppVersion", appInfo.VersionString);
        collector.Add("AppBuild", appInfo.BuildString);
        collector.Add("DeviceManufacturer", device.Manufacturer);
        collector.Add("DeviceModel", device.Model);
        collector.Add("DeviceName", device.Name);
        collector.Add("DeviceType", device.DeviceType);
        collector.Add("DeviceVersion", device.Version);
        collector.Add("DisplayOrientation", display.MainDisplayInfo.Orientation);
        collector.Add("DisplayScale", display.MainDisplayInfo.Density);
        collector.Add("DisplayWidth", display.MainDisplayInfo.Width);
        collector.Add("DisplayHeight", display.MainDisplayInfo.Height);
    }
}