using IP_Adress_Test.Core.Repositories;

namespace IP_Adress_Test.Infrastructure.Repositories;
internal class FileIPAddressRepository : IIPAddressRepository
{
    public IEnumerable<string> GetAdresses(string? path)
    {
        if (path != null)
            return File.ReadLines(path);
        else
            throw new ArgumentNullException(nameof(path));
    }

    public void WriteAddress(IEnumerable<string> address, string? path)
    {
        if (path != null)
            File.AppendAllLines(path, address);
        else
            throw new ArgumentNullException(nameof(path));
    }
}