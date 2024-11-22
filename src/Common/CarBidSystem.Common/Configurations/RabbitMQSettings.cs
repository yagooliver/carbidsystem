namespace CarBidSystem.Common.Configurations
{
    public class RabbitMqSettings
    {
        public string Host { get; set; } = "rabbitmq-service";
        public string VirtualHost { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string QueueName { get; set; } = string.Empty;
    }
}
