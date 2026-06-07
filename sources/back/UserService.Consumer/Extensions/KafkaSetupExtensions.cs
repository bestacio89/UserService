using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Franz.Common.Messaging.Kafka;
using UserService.Consumer.Services;
using Franz.Common.Messaging.Kafka.Extensions;

namespace UserService.Consumer.Extensions
{
  public static class KafkaSetupExtensions
  {
    public static IServiceCollection AddKafkaMessagingServices(this IServiceCollection services, IConfiguration configuration)
    {
      // Register Kafka-related services
      services.AddKafkaMessaging(configuration);
     // services.AddHostedService<KafkaConsumerService>();
      return services;
    }
  }
}


