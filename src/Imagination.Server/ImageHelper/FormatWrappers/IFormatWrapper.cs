using System.IO;
using System.Threading;
using System.Threading.Tasks;

using SixLabors.ImageSharp;


namespace Imagination.ImageHelper.FormatWrappers
{
    public interface IFormatWrapper
    {
        Task SaveImageAsync(Image image, Stream responseStream, CancellationToken stoppingToken);
    }
}
