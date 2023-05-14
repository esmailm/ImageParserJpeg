using Microsoft.Extensions.Options;

using Imagination.Configurations;


namespace Imagination.ImageHelper.FormatWrappers
{
    public class FormatWrapperFactory : IFormatWrapperFactory
    {
        private readonly int _targetImageCompress;
        private readonly int _targetImageCompressTolerance;
        private readonly int _defaultImageQuality;
        public FormatWrapperFactory(IOptions<ImageConfiguration> iConfig)
        {
            _targetImageCompress = iConfig.Value.TargetImageCompress;
            _targetImageCompressTolerance = iConfig.Value.TargetImageCompressTolerance;
            _defaultImageQuality = iConfig.Value.DefaultImageQuality;
        }
        public IFormatWrapper GetTargetFormat(string tarTargetImageFormat)
        {
            switch (tarTargetImageFormat.Trim().ToLower())
            {
                case "jpeg":
                    return new JpegFormatWrapper(_targetImageCompress,
                        _targetImageCompressTolerance,
                        _defaultImageQuality);

                case "png":
                    return new PngFormatWrapper();

                default: throw new System.NotImplementedException();
            }

        }
    }
}
