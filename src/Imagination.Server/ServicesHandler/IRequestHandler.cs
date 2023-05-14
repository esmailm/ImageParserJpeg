using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Imagination.ServicesHandler
{
    public interface IRequestHandler
    {
        Task ExtractAndConvert(Stream Body, Stream responseStream, CancellationToken stoppingToken);
    }
}
