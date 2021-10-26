using FiasGarImporter.Events;
using FiasGarImporter.Models;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("IntegrationTests")]
[assembly: InternalsVisibleTo("UnitTests")]
namespace FiasGarImporter
{
    public interface IImporter : IDisposable
    {
        event EventHandler<ProgressEventArgs>? Progress;

        ValueTask<IEnumerable<GarUrl>> GetUrlListAsync();
        IEnumerable<GarUrl> GetUrlList();

        IEnumerable<AddressObject> GetFull();
        ValueTask<IEnumerable<AddressObject>> GetFullAsync();

        IEnumerable<AddressObject> GetDiff(DateTime lastLoad);
        ValueTask<IEnumerable<AddressObject>> GetDiffAsync(DateTime lastLoad);
        IEnumerable<int> EnumerateRegionsInFile(string fileName);
    }
}
