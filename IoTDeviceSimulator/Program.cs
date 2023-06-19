using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IoTDeviceSimulator
{
    class Program
    {
        private static DeviceClient deviceClient;
        private static string connectionString;
        private static int deviceId = 1;
        private static Random random = new Random();

        static async Task Main(string[] args)
        {
            Console.WriteLine("IoT Device Simulator");

            // Configuración
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            connectionString = config.GetConnectionString("IoTHubConnectionString");
            deviceClient = DeviceClient.CreateFromConnectionString(connectionString, TransportType.Mqtt);

            // Simulación
            while (true)
            {
                int state = random.Next(0, 2);
                await SendDeviceToCloudMessageAsync(deviceId, state);
                Console.WriteLine($"Sent message - DeviceId: {deviceId}, State: {state}");

                await Task.Delay(300000); // Espera 5 minutos antes de enviar el siguiente mensaje
            }
        }

        private static async Task SendDeviceToCloudMessageAsync(int deviceId, int state)
        {
            var telemetryDataPoint = new
            {
                DeviceId = deviceId,
                State = state
            };

            var messageString = Newtonsoft.Json.JsonConvert.SerializeObject(telemetryDataPoint);
            var message = new Message(Encoding.UTF8.GetBytes(messageString));

            await deviceClient.SendEventAsync(message);
        }
    }
}