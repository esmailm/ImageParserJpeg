using System.IO;
using System.Threading;
using System.Threading.Tasks;

using SixLabors.ImageSharp;


namespace Imagination.ImageHelper.FormatWrappers
{
    public class PngFormatWrapper : IFormatWrapper
    {
        public Task SaveImageAsync(Image image, Stream responseStream, CancellationToken stopToken)
        {
            throw new System.NotImplementedException();
        }
    }
}