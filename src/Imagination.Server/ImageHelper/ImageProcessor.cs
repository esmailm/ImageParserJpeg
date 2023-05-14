using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

using Microsoft.Extensions.Options;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

using Imagination.Configurations;
using Imagination.ImageHelper.FormatWrappers;

namespace Imagination.ImageHelper
{
    public class ImageProcessor : IImageProcessor
    {
        private readonly int _targetPixelsWidth;
        private readonly int _targetPixelsHeight;
        private readonly string _targetFormat;
        private readonly IFormatWrapperFactory _formatWrapper;
        public ImageProcessor(IOptions<ImageConfiguration> iConfig, IFormatWrapperFactory formatWrapper)
        {
            _targetPixelsWidth = iConfig.Value.TargetPixelsWidth;
            _targetPixelsHeight = iConfig.Value.TargetPixelsHeight;
            _targetFormat = iConfig.Value.TargetImageFormat;
            _formatWrapper = formatWrapper;
        }

        public async Task Convert(Stream sourceStream, Stream responseStream, CancellationToken stoppingToken)
        {
            try
            {
                using var activity = Program.Telemetry.StartActivity(nameof(Convert));

                sourceStream.Position = 0;
                using (var sourceImage = await Image.LoadAsync(sourceStream, stoppingToken))
                {
                    activity?.AddEvent(new ActivityEvent("Image Loaded"));

                    transformImage(sourceImage);
                    activity?.AddEvent(new ActivityEvent("Image size updated"));

                    saveImage(sourceImage, responseStream, stoppingToken);

                }
            }
            catch (UnknownImageFormatException e)
            {
                throw new ImageInputException(e.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        private void transformImage(Image sourceImage)
        {
            if ((_targetPixelsWidth * _targetPixelsHeight) > (sourceImage.Width * sourceImage.Height)) return;
            var ratio = Math
                .Sqrt((_targetPixelsWidth * _targetPixelsHeight) / (double)(sourceImage.Width * sourceImage.Height));

            var newWidth = (int)Math.Round(sourceImage.Width * ratio, 0);

            var newHeight = (int)Math.Round(sourceImage.Height * ratio, 0);

            sourceImage.Mutate(i => i.Resize(newWidth, newHeight));
        }


        private void saveImage(Image image, Stream responseStream, CancellationToken stoppingToken)
        {
            _formatWrapper.GetTargetFormat(_targetFormat).SaveImageAsync(image, responseStream, stoppingToken);
        }


    }
}
