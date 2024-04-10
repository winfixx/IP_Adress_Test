using IP_Adress_Test.Core.Logger;
using IP_Adress_Test.Core.Models;
using IP_Adress_Test.Core.Repositories;
using System.Globalization;
using System.Net;

namespace IP_Adress_Test.Core.Services;
internal class IPAddressService(IIPAddressRepository ipAddressRepository, ILogger logger)
{
    private readonly IIPAddressRepository ipAddressRepository = ipAddressRepository;
    private readonly ILogger logger = logger;

    public void ProcessArguments(string[] args)
    {
        Argument arguments = ParseArguments(args);

        IEnumerable<string> filteredAddresses = FilterAddresses(
            arguments.LogFilePath,
            arguments.StartTime,
            arguments.EndTime,
            arguments.StartAddress,
            arguments.Mask);

        Dictionary<string, int> ipCounts = CountIPAddresses(filteredAddresses);

        WriteResultsToFile(arguments.OutputFilePath, ipCounts);
    }

    private Argument ParseArguments(string[] args)
    {
        var arguments = new Argument();

        for (int i = 0; i < args.Length; i += 2)
        {
            switch (args[i])
            {
                case "--file-log":
                    arguments.LogFilePath = args[i + 1];
                    break;
                case "--file-output":
                    arguments.OutputFilePath = args[i + 1];
                    break;
                case "--address-start":
                    arguments.StartAddress = args[i + 1];
                    break;
                case "--address-mask":
                    arguments.Mask = int.Parse(args[i + 1]);
                    break;
                case "--time-start":
                    arguments.StartTime = DateTime.ParseExact(args[i + 1], "dd.MM.yyyy", null);
                    break;
                case "--time-end":
                    arguments.EndTime = DateTime.ParseExact(args[i + 1], "dd.MM.yyyy", null);
                    break;
                default:
                    logger.Warn($"неизвестный аргумент: {args[i]}");
                    break;
            }
        }

        return arguments;
    }

    private IEnumerable<string> FilterAddresses(
        string logFilePath,
        DateTime startTime,
        DateTime endTime,
        string startAddress,
        int? mask)
    {
        if (string.IsNullOrEmpty(logFilePath))
            throw new ArgumentNullException(nameof(logFilePath), "путь к логам отсутствует");

        var filteredAddresses = new List<string>();
        string line;

        try
        {
            IEnumerable<string> addresses = ipAddressRepository.GetAdresses(logFilePath);

            for (int i = 0; i < addresses.Count(); i++)
            {
                line = addresses.ElementAt(i);
                var parts = line.Split(':');
                var address = parts[0];
                var timeString = string.Join(":", parts.Skip(1));
                var timeParts = timeString.Split(' ');
                var datePart = timeParts[0];
                var timePart = timeParts[1];
                var fullTimeString = $"{datePart} {timePart}";
                var time = DateTime.ParseExact(fullTimeString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

                if (time >= startTime && time <= endTime && IPInRange(address, startAddress, mask))
                    filteredAddresses.Add(address);
            }
        }
        catch (Exception ex)
        {
            logger.Error($"ошибка в чтении файла: {ex.Message}");
        }

        return filteredAddresses;
    }


    private bool IPInRange(string ip, string startAddress, int? mask)
    {
        if (mask == null) return true;

        var ipAddress = IPAddress.Parse(ip).GetAddressBytes();
        var startIpAddress = IPAddress.Parse(startAddress).GetAddressBytes();

        byte[] maskBytes = new byte[4];
        for (int i = 0; i < 4; i++)
        {
            if (mask > 8)
            {
                maskBytes[i] = 255;
                mask -= 8;
            }
            else if (mask == 8)
            {
                maskBytes[i] = 255;
                mask = 0;
            }
            else
            {
                maskBytes[i] = (byte)(255 - (Math.Pow(2, 8 - (int)mask) - 1));
                mask = 0;
            }
        }

        for (int i = 0; i < startIpAddress.Length; i++)
            startIpAddress[i] = (byte)(startIpAddress[i] & maskBytes[i]);

        for (int i = 0; i < ipAddress.Length; i++)
            if (ipAddress[i] != startIpAddress[i]) return false;

        return true;
    }

    private Dictionary<string, int> CountIPAddresses(IEnumerable<string> addresses)
    {
        var ipCounts = new Dictionary<string, int>();
        foreach (var address in addresses)
        {
            if (!ipCounts.ContainsKey(address)) ipCounts.Add(address, 0);

            ipCounts[address]++;
        }

        return ipCounts;
    }

    private void WriteResultsToFile(string filePath, Dictionary<string, int> ipCounts)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentNullException(nameof(filePath), "путь к выходному файлу отсутствует");

        try
        {
            var adresses = new List<string>();
            foreach (var pair in ipCounts)
                adresses.Add($"{pair.Key}: {pair.Value}");

            ipAddressRepository.WriteAddress(adresses, filePath);
        }
        catch (Exception ex)
        {
            logger.Error($"ошибка записи в файл: {ex.Message}");
        }
    }
}

