using System;
using System.Configuration;
using System.Text;
using System.Threading;
using Messaging.Contracts;
using Newtonsoft.Json;
using Prescriptions.Domain.Entities;
using Prescriptions.Infrastructure.Persistence;
using Prescriptions.Infrastructure.Repositories;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Prescriptions.Api.Messaging
{
    public class AppointmentFinishedConsumer : IDisposable
    {
        private IConnection _connection;
        private IChannel _channel;
        private bool _started;

        public void Start()
        {
            if (_started)
            {
                return;
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

            _connection = factory.CreateConnectionAsync(CancellationToken.None).GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync(new CreateChannelOptions(false, false, null, null), CancellationToken.None).GetAwaiter().GetResult();

            _channel.ExchangeDeclareAsync(
                exchange: exchange,
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false,
                arguments: null).GetAwaiter().GetResult();

            _channel.QueueDeclareAsync(
                queue: queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null).GetAwaiter().GetResult();

            _channel.QueueBindAsync(
                queue: queue,
                exchange: exchange,
                routingKey: routingKey,
                arguments: null).GetAwaiter().GetResult();

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += HandleMessageAsync;

            _channel.BasicConsumeAsync(
                queue: queue,
                autoAck: false,
                consumer: consumer).GetAwaiter().GetResult();

            _started = true;
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }

        private async System.Threading.Tasks.Task HandleMessageAsync(object sender, BasicDeliverEventArgs args)
        {
            try
            {
                var json = Encoding.UTF8.GetString(args.Body.ToArray());
                var evt = JsonConvert.DeserializeObject<AppointmentFinishedEvent>(json);

                if (evt == null || evt.PatientId == Guid.Empty || evt.AppointmentId == Guid.Empty)
                {
                    await _channel.BasicAckAsync(args.DeliveryTag, false);
                    return;
                }

                var code = BuildCode(evt.AppointmentId);

                using (var context = new PrescriptionsDbContext())
                {
                    var repository = new PrescriptionRepository(context);
                    var existingByCode = repository.GetByCode(code);
                    if (existingByCode != null)
                    {
                        await _channel.BasicAckAsync(args.DeliveryTag, false);
                        return;
                    }

                    var prescription = new Prescription(
                        Guid.NewGuid(),
                        code,
                        evt.PatientId,
                        evt.FinishedAt == default(DateTime) ? DateTime.UtcNow : evt.FinishedAt);

                    repository.Add(prescription);
                    repository.SaveChanges();
                }

                await _channel.BasicAckAsync(args.DeliveryTag, false);
            }
            catch
            {
                await _channel.BasicNackAsync(args.DeliveryTag, false, true);
            }
        }

        private static string BuildCode(Guid appointmentId)
        {
            return $"RX-{appointmentId:N}".ToUpperInvariant();
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