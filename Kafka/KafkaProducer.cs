using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace Kafka
{
	public class KafkaProducer
	{
		public KafkaProducer(IConfiguration configuration) => Configuration = configuration;

		public IConfiguration Configuration { get; }

		public async Task ProduceMessage(string topic, string message)
		{
			var bootstrapServers = Configuration.GetSection("Kafka")["BootstrapServers"];

			var config = new ProducerConfig { BootstrapServers = bootstrapServers };

			// If serializers are not specified, default serializers from
			// `Confluent.Kafka.Serializers` will be automatically used where
			// available. Note: by default strings are encoded as UTF8.
			using var p = new ProducerBuilder<Null, string>(config).Build();
			try
			{
				var dr = await p.ProduceAsync(topic, new Message<Null, string> { Value = message });
				Console.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
			}
			catch (ProduceException<Null, string> e)
			{
				Console.WriteLine($"Delivery failed: {e.Error.Reason}");
			}
		}
	}
}