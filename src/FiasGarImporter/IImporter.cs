using FiasGarImporter.Events;
using FiasGarImporter.Models;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("IntegrationTests")]
namespace FiasGarImporter
{
    public interface IImporter : IDisposable
    {
        event EventHandler<ProgressEventArgs>? Progress;

        IEnumerable<AddressObject> GetFull();
        ValueTask<IEnumerable<AddressObject>> GetFullAsync();
    }
}
