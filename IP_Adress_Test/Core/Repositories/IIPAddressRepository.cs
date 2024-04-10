namespace IP_Adress_Test.Core.Repositories;
internal interface IIPAddressRepository
{
    IEnumerable<string> GetAdresses(string? path);
    void WriteAddress(IEnumerable<string> address, string? path);
}

