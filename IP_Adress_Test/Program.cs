using IP_Adress_Test.Core.Logger;
using IP_Adress_Test.Core.Repositories;
using IP_Adress_Test.Core.Services;
using IP_Adress_Test.Infrastructure.Controllers.ConsoleController;
using IP_Adress_Test.Infrastructure.Logger;
using IP_Adress_Test.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace IP_Adress_Test;
internal class Program
{
    static void Main()
    {
        var services = new ServiceCollection()
            .AddTransient<IIPAddressRepository, FileIPAddressRepository>()
            .AddTransient<ILogger, ConsoleLogger>()
            .AddTransient<IPAddressService>();

        using var serviceProvider = services.BuildServiceProvider();

        ILogger consoleLogger = serviceProvider.GetService<ILogger>()!;
        IPAddressService ipAddressService = serviceProvider.GetService<IPAddressService>()!;

        new IPAdressesController(ipAddressService, consoleLogger).Run();
    }
}
