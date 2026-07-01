namespace TopUpSimulation.Framework.Common.Settings;

public class MCITopUpSetting
{
    public string BaseUrl { get; set; } = default!;
    public string SecretToken { get; set; } = default!;
    public string InstantCharge { get; set; } = default!;
    public bool MockService { get; set; } = default!;
}
