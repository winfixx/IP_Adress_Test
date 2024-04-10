namespace IP_Adress_Test.Core.Models;
internal class Argument
{
    public string LogFilePath { get; set; }
    public string OutputFilePath { get; set; }
    public string StartAddress { get; set; }
    public int? Mask { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}

