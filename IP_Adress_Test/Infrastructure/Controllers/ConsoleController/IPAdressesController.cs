using IP_Adress_Test.Core.Logger;
using IP_Adress_Test.Core.Repositories;
using IP_Adress_Test.Core.Services;

namespace IP_Adress_Test.Infrastructure.Controllers.ConsoleController
{
    internal class IPAdressesController(IPAddressService ipAddressService, ILogger logger)
    {
        private readonly IPAddressService ipAddressService = ipAddressService;
        private readonly ILogger logger = logger;

        public void Run()
        {
            while (true)
            {
                logger.Info("жду приказа..");

                string command = Console.ReadLine()!;

                if (string.IsNullOrEmpty(command)) continue;

                string[] commandArgs = command.Split(' ');

                if (commandArgs[0] == "exit")
                {
                    logger.Info("ухожу..");
                    break;
                }

                if (commandArgs.Length != 12)
                {
                    logger.Warn("команду не знаю, повтори. \nЮзай: --file-log <log_file_path> --file-output <output_file_path> --address-start <start_ip> --address-mask <mask> --time-start <start_time> --time-end <end_time>");
                    continue;
                }

                try
                {
                    ipAddressService?.ProcessArguments(commandArgs);
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                    continue;
                }

                logger.Info("все вери гуд");
            }
        }
    }
}
