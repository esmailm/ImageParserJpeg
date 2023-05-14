using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;


namespace Imagination.ImageHelper
{
    public interface IImageProcessor
    {
        Task Convert(Stream sourceArray, Stream responseStream, CancellationToken stoppingToken);
    }
}
