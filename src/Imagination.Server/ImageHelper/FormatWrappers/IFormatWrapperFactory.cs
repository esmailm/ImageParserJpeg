namespace Imagination.ImageHelper.FormatWrappers
{
    public interface IFormatWrapperFactory
    {
        IFormatWrapper GetTargetFormat(string tarTargetImageFormat);
    }
}
