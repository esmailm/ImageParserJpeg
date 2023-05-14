using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

using Microsoft.Extensions.Options;

using Imagination.ImageHelper;
using Imagination.Configurations;

namespace Imagination.ServicesHandler
{
    public class RequestHandler : IRequestHandler
    {
        private readonly IImageProcessor _imageProcessor;
        private readonly int maxInputImageSize;
        private readonly int minInputImageSize;
        public RequestHandler(IImageProcessor imageProcessor, IOptions<ImageConfiguration> iConfig)
        {
            _imageProcessor = imageProcessor;
            maxInputImageSize = iConfig.Value.MaxInputImageSize;
            minInputImageSize = iConfig.Value.MinInputImageSize;
        }

        public async Task ExtractAndConvert(Stream body, Stream responseStream, CancellationToken stoppingToken)
        {
            using var activity = Program.Telemetry.StartActivity(nameof(ExtractAndConvert));

            using (MemoryStream sourceStream = new MemoryStream())
            {
                await body.CopyToAsync(sourceStream, stoppingToken);

                if (sourceStream.Length > maxInputImageSize) {
                    var err = $"The input file size {responseStream.Length} bit exceed the maximum acceptable size {maxInputImageSize} bit.";
                    throw new ImageInputException(err); }

                else if (sourceStream.Length < minInputImageSize) {
                    var err = $"The input file size {responseStream.Length} bit is too small to be handeled.";
                    throw new ImageInputException(err); }

                else {
                    activity?.AddEvent(new ActivityEvent("Received Stream Extracted Successfully"));
                    await _imageProcessor.Convert(sourceStream, responseStream, stoppingToken); }
            }
        }
    }
}
