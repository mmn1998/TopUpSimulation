namespace TopUpSimulation.Framework.Common.Settings;

public class KafkaSetting
{
    public ProducerConfig ProducerConfigs { get; set; } = default!;
    public ConsumerConfig ConsumerConfig { get; set; } = default!;
}
public class ProducerConfig
{
    public string BootstrapServers { get; set; } = default!;
}
public class ConsumerConfig
{
    public string BootstrapServers { get; set; } = default!;
    public string GroupId { get; set; } = default!;
}
