using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace Imagination.ImageHelper.FormatWrappers
{
    public class JpegFormatWrapper : IFormatWrapper
    {
        private Dictionary<int, long> _qualityVersusSize;
        private readonly int _targetImageCompress;
        private readonly int _targetImageCompressTolerance;
        private readonly int _defaultImageQuality;

        public JpegFormatWrapper(int targetImageCompress, int targetImageCompressTolerance, int defaultImageQuality)
        {
            _qualityVersusSize = new Dictionary<int, long>();
            _targetImageCompress = targetImageCompress;
            _targetImageCompressTolerance = targetImageCompressTolerance;
            _defaultImageQuality = defaultImageQuality;
        }

        public async Task SaveImageAsync(Image sourceImage, Stream responseStream, CancellationToken stoppingToken)
        {
            using var activity = Program.Telemetry.StartActivity(nameof(SaveImageAsync));

            var encoder = new JpegEncoder();

            encoder.Quality = _defaultImageQuality;

            await sourceImage.SaveAsJpegAsync(responseStream, encoder, stoppingToken);

            if (responseStream.Length > _targetImageCompress + _targetImageCompressTolerance)
            { await optimizeSizeAsync(sourceImage, responseStream, stoppingToken); }
            activity?.AddEvent(new ActivityEvent("Image Saved"));

        }

        private async Task optimizeSizeAsync(Image sourceImage, Stream responseStream, CancellationToken stoppingToken)
        {
            using var activity = Program.Telemetry.StartActivity(nameof(optimizeSizeAsync));

            var currentSize = responseStream.Length;
            int currentQuality = _defaultImageQuality;
            int maxQuality = _defaultImageQuality;
            var minQuality = 0;
            int maxSize = _targetImageCompress + _targetImageCompressTolerance;
            int minSize = _targetImageCompress < _targetImageCompressTolerance ? 0 : _targetImageCompress - _targetImageCompressTolerance;
            var encoder = new JpegEncoder();
            long finalSize = 0;
            int count = 0;
            using (var tempMS = new MemoryStream())
            {
                while ((currentSize > maxSize || currentSize < minSize) && minQuality < maxQuality)
                {
                    count++;

                    if (_qualityVersusSize.TryGetValue(currentQuality, out finalSize)) break;

                    if (currentSize < minSize) { minQuality = currentQuality; currentQuality = (currentQuality + maxQuality) / 2; }

                    else if (currentSize > maxSize) { maxQuality = currentQuality; currentQuality = (currentQuality + minQuality) / 2; }

                    else break;

                    encoder.Quality = currentQuality;
                    resetStream(tempMS);
                    await sourceImage.SaveAsJpegAsync(tempMS, encoder, stoppingToken);
                    currentSize = tempMS.Length != 0 ? tempMS.Length : currentSize;


                }
                resetStream(responseStream);
                await sourceImage.SaveAsJpegAsync(responseStream, encoder, stoppingToken);
                activity?.AddEvent(new ActivityEvent($"Image Size Optimized after {count} retries"));

            }
        }
        public void resetStream(Stream ms)
        {
            ms.Position = 0;
            ms.SetLength(0);
        }
    }
}