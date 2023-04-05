using System;
using System.Net.NetworkInformation;
using System.Text;

namespace PingUtility
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Введите адрес для пинга: ");
            string address = Console.ReadLine();

            Console.WriteLine($"Начат пинг {address}...");

            Ping ping = new Ping();
            PingOptions options = new PingOptions();
            StringBuilder data = new StringBuilder();
            int bytes = 32;
            for (int i = 0; i < bytes; i++)
            {
                data.Append("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
            }

            options.DontFragment = true;

            int timeout = 120;
            int ttl = 128;
            int count = 4;

            int successCount = 0;
            long totalTime = 0;
            long minTime = long.MaxValue;
            long maxTime = long.MinValue;

            for (int i = 0; i < count; i++)
            {
                options.Ttl = ttl;
                byte[] buffer = Encoding.ASCII.GetBytes(data.ToString());
                PingReply reply = null;

                Console.Write($"Пинг {address} с TTL = {ttl}... ");

                reply = ping.Send(address, timeout, buffer, options);
                if (reply.Status == IPStatus.Success)
                {
                    successCount++;
                    Console.WriteLine($"Успешно: время = {reply.RoundtripTime} мс");
                    totalTime += reply.RoundtripTime;
                    if (reply.RoundtripTime < minTime)
                    {
                        minTime = reply.RoundtripTime;
                    }
                    if (reply.RoundtripTime > maxTime)
                    {
                        maxTime = reply.RoundtripTime;
                    }
                }
                else if (reply.Status == IPStatus.TtlExpired || reply.Status == IPStatus.TimedOut)
                {
                    Console.WriteLine("Не получен ответ: " + reply.Status.ToString());
                }
                else
                {
                    Console.WriteLine("Ошибка: " + reply.Status.ToString());
                }

                // Отправляем пинг каждую секунду
                System.Threading.Thread.Sleep(1000);
            }

            Console.WriteLine();
            Console.WriteLine($"Статистика: отправлено = {count}, получено = {successCount}, потеряно = {count - successCount} ({((count - successCount) * 100) / count}% потерь)");
            Console.WriteLine($"Минимальное время = {minTime} мс, Максимальное время = {maxTime} мс, Среднее время = {(successCount > 0 ? totalTime / successCount : 0)} мс");

            Console.WriteLine("Нажмите любую клавишу, чтобы закрыть окно...");
            Console.ReadKey();
        }
    }
}
