namespace Imagination.Configurations
{
    public class ImageConfiguration
    {
        public int TargetPixelsWidth { get; set; }
        public int TargetPixelsHeight { get; set; }
        public string TargetImageFormat { get; set; }
        public int TargetImageCompress { get; set; }
        public int TargetImageCompressTolerance { get; set; }
        public int MaxInputImageSize { get; set; }
        public int MinInputImageSize { get; set; }
        public int DefaultImageQuality { get; set; }
    }
}
