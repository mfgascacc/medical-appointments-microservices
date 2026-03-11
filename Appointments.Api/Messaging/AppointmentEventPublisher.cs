using System;
using System.Configuration;
using System.Text;
using System.Threading;
using Appointments.Domain.Entities;
using Messaging.Contracts;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Appointments.Api.Messaging
{
    public class AppointmentEventPublisher : IAppointmentEventPublisher
    {
        public void PublishAppointmentFinished(Appointment appointment)
        {
            if (appointment == null)
            {
                throw new ArgumentNullException(nameof(appointment));
            }

            var factory = new ConnectionFactory
            {
                HostName = ReadSetting("RabbitMqHost", "localhost"),
                Port = ReadIntSetting("RabbitMqPort", 5672),
                UserName = ReadSetting("RabbitMqUser", "guest"),
                Password = ReadSetting("RabbitMqPass", "guest")
            };

            var exchange = ReadSetting("RabbitMqExchange", "appointments.events");
            var queue = ReadSetting("RabbitMqQueue", "prescriptions.create.queue");
            var routingKey = ReadSetting("RabbitMqRoutingKey", "appointment.finished");

            var message = new AppointmentFinishedEvent
            {
                AppointmentId = appointment.Id,
                DoctorId = appointment.DoctorId,
                PatientId = appointment.PatientId,
                FinishedAt = DateTime.UtcNow
            };

            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            using (var connection = factory.CreateConnectionAsync(CancellationToken.None).GetAwaiter().GetResult())
            using (var channel = connection.CreateChannelAsync(new CreateChannelOptions(false, false, null, null), CancellationToken.None).GetAwaiter().GetResult())
            {
                channel.ExchangeDeclareAsync(
                    exchange: exchange,
                    type: ExchangeType.Direct,
                    durable: true,
                    autoDelete: false,
                    arguments: null).GetAwaiter().GetResult();

                channel.QueueDeclareAsync(
                    queue: queue,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null).GetAwaiter().GetResult();

                channel.QueueBindAsync(
                    queue: queue,
                    exchange: exchange,
                    routingKey: routingKey,
                    arguments: null).GetAwaiter().GetResult();

                channel.BasicPublishAsync(
                    exchange: exchange,
                    routingKey: routingKey,
                    body: body).GetAwaiter().GetResult();
            }
        }

        private static string ReadSetting(string key, string defaultValue)
        {
            var value = ConfigurationManager.AppSettings[key];
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
        }

        private static int ReadIntSetting(string key, int defaultValue)
        {
            var raw = ConfigurationManager.AppSettings[key];
            return int.TryParse(raw, out var parsed) ? parsed : defaultValue;
        }
    }
}