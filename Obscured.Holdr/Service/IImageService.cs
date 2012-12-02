using System.Drawing;
using System.Drawing.Imaging;

namespace Obscured.Holdr.Service
{
    public interface IImageService
    {
        byte[] ImageToByteArray(Image image);
        Image Crop(Image image, int width, int height);
        Image Crop(Image image, int width, int height, int startAtX, int startAtY);
        Image HardResize(Image image, int width, int height);
        Image Resize(Image image, int newWidth, int newHeight);
        ImageCodecInfo GetImageCodeInfo(string mimeType);
    }
}
