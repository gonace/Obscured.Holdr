using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Obscured.Holdr.Service
{
    public class ImageService : IImageService
    {
        private ImageService()
        {
        }

        public static ImageService Instance()
        {
            return new ImageService();
        }


        //Get random image
        public byte[] GetImageRandom(int width, int height, string filePath)
        {
            var imageAmount = Directory.GetFiles(filePath, "*.jpg", SearchOption.AllDirectories).Length;
            var random = new Random();
            var rndNumber = random.Next(imageAmount-1);

            var imgFile = filePath + rndNumber + ".jpg";

            if (File.Exists(imgFile))
            {
                var selectedImage = Image.FromFile(imgFile);
                var myImg = Instance().HardResize(selectedImage, width, height);

                return Instance().ImageToByteArray(myImg);
            }

            throw new Exception("Error: File not found or file damaged!");
        }

        //Get image by name(id)
        public byte[] GetImageByName(int width, int height, string imgPath)
        {
            if (File.Exists(imgPath))
            {
                var selectedImage = Image.FromFile(imgPath);
                var myImg = Instance().HardResize(selectedImage, width, height);

                return Instance().ImageToByteArray(myImg);
            }

            return null;
        }

        //Return image as byte[]
        public byte[] ImageToByteArray(Image image)
        {
            var ms = new MemoryStream();
            image.Save(ms, ImageFormat.Jpeg);
            return ms.ToArray();
        }

        //Overload for crop that default starts top left of the image.
        public Image Crop(Image image, int width, int height)
        {
            return Crop(image, width, height, 0, 0);
        }

        //The crop image sub
        public Image Crop(Image image, int width, int height, int startAtX, int startAtY)
        {
            try
            {
                if (image.Height < height)
                    height = image.Height;

                if (image.Width < width)
                    width = image.Width;

                var ms = new MemoryStream();
                using (var bmPhoto = new Bitmap(width, height))
                {
                    bmPhoto.SetResolution(170, 170);
                    bmPhoto.MakeTransparent();

                    var grPhoto = Graphics.FromImage(bmPhoto);
                    grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    grPhoto.SmoothingMode = SmoothingMode.HighQuality;
                    grPhoto.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    grPhoto.CompositingQuality = CompositingQuality.HighQuality;
                    grPhoto.DrawImage(image, new Rectangle(0, 0, width, height), startAtX, startAtY, width, height, GraphicsUnit.Pixel);

                    var encoderParameters = new EncoderParameters(2);
                    encoderParameters.Param[0] = new EncoderParameter(Encoder.Compression, 100);
                    encoderParameters.Param[1] = new EncoderParameter(Encoder.Quality, 100L);

                    bmPhoto.Save(ms, GetImageCodeInfo("image/jpeg"), encoderParameters);
                    image.Dispose();
                    bmPhoto.Dispose();
                    grPhoto.Dispose();

                    var outimage = Image.FromStream(ms);
                    return outimage;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error cropping image, the error was: " + ex.Message);
            }
        }

        //Hard resize attempts to resize as close as it can to the desired size and then crops the excess
        public Image HardResize(Image image, int newWidth, int newHeight)
        {
            var resized = Resize(image, newWidth, newHeight);

            //Calculate where to crop center image
            var startX = 0;
            if (newWidth < resized.Width)
                startX = (resized.Width / 2) - (newWidth / 2);

            var startY = 0;
            if (newHeight < resized.Height)
                startY = (resized.Height / 2) - (newHeight / 2);

            var output = Crop(resized, newWidth, newHeight, startX, startY);

            return output;
        }

        //Image resizing
        public Image Resize(Image image, int newWidth, int newHeight)
        {
            var width = image.Width;
            var height = image.Height;
            if (width > newWidth || height > newHeight)
            {
                //The flips are in here to prevent any embedded image thumbnails -- usually from cameras
                //from displaying as the thumbnail image later, in other words, we want a clean
                //resize, not a grainy one.
                image.RotateFlip(RotateFlipType.Rotate180FlipX);
                image.RotateFlip(RotateFlipType.Rotate180FlipX);

                float ratio;
                if(newWidth > newHeight)
                {
                    if(width > height)
                    {
                        ratio = (float)width / (float)height;
                        width = newWidth;
                        height = Convert.ToInt32(Math.Round((float)width / ratio));
                    }
                    else
                    {
                        ratio = (float)width / (float)height;
                        width = newWidth;
                        height = Convert.ToInt32(Math.Round((float)width / ratio));
                    }
                }
                else
                {
                    if (width > height)
                    {
                        ratio = (float) width/(float) height;
                        height = newHeight;
                        width = Convert.ToInt32(Math.Round((float) height*ratio));
                    }
                    else
                    {
                        ratio = (float)width / (float)height;
                        height = newHeight;
                        width = Convert.ToInt32(Math.Round((float)height * ratio));
                    }
                }

                var b = new Bitmap(width, height);
                b.MakeTransparent();

                using (var g = Graphics.FromImage(b))
                {
                    g.Clear(Color.Transparent);
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.DrawImage(image, 0, 0, width, height);
                    g.Dispose();
                }

                return b;
            }

            //return the original resized image
            return image;
        }

        //Get ImageCodecInfo
        public ImageCodecInfo GetImageCodeInfo(string mimeType)
        {
            var info = ImageCodecInfo.GetImageEncoders();
            return info.FirstOrDefault(ici => ici.MimeType.Equals(mimeType, StringComparison.OrdinalIgnoreCase));
        }
    }
}