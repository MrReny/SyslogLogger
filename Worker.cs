using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rebex.Net;

namespace SyslogLogger
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            StartSyslogServer();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

        }

        private static SyslogServer _server;

        private static void StartSyslogServer()
        {
            _server = new SyslogServer(514);
            _server.MessageReceived += (sender, eventArgs) =>
            {
                var message = eventArgs.Message.Text;
                Console.WriteLine(message);
                Task.Factory.StartNew( async (s) => await WriteMessageToLogFile(s.ToString()),message);
            };
            _server.Start();

        }

        private static async Task WriteMessageToLogFile(string message)
        {
            await using var sr = new StreamWriter("g:\\SyslogLogs\\log-"+DateTime.Now.ToString("dd-MM-yy")+".txt", true, Encoding.Default);
            await sr.WriteLineAsync(message.Trim('\n'));
        }
    }
}
