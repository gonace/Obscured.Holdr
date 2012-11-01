using System.Drawing;
using System.Drawing.Imaging;

namespace Obscured.Holdr.Service
{
    public interface IImageService
    {
        byte[] ImageToByteArray(System.Drawing.Image image);
        Image CropImage(Image image, int width, int height);
        Image CropImage(Image image, int width, int height, int startAtX, int startAtY);
        Image HardResizeImage(Image image, int width, int height);
        Image ResizeImage(Image image, int newWidth, int newHeight);
        ImageCodecInfo GetImageCodeInfo(string mimeType);
    }
}
