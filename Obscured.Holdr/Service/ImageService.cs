using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web.Mvc;

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
                Image myImg = Instance().HardResizeImage(selectedImage, width, height);

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
                Image myImg = Instance().HardResizeImage(selectedImage, width, height);

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
        public Image CropImage(Image image, int width, int height)
        {
            return CropImage(image, width, height, 0, 0);
        }

        //The crop image sub
        public Image CropImage(Image image, int width, int height, int startAtX, int startAtY)
        {
            try
            {
                //check the image height against our desired image height
                if (image.Height < height)
                {
                    height = image.Height;
                }

                if (image.Width < width)
                {
                    width = image.Width;
                }

                //create a bitmap window for cropping
                var bmPhoto = new Bitmap(width, height); //, PixelFormat.Format64bppArgb
                bmPhoto.SetResolution(720, 720);

                //create a new graphics object from our image and set properties
                Graphics grPhoto = Graphics.FromImage(bmPhoto);
                grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
                grPhoto.SmoothingMode = SmoothingMode.HighQuality;
                grPhoto.PixelOffsetMode = PixelOffsetMode.HighQuality;
                grPhoto.CompositingQuality = CompositingQuality.HighQuality;

                //now do the crop
                grPhoto.DrawImage(image, new Rectangle(0, 0, width, height), startAtX, startAtY, width, height, GraphicsUnit.Pixel);

                // Save out to memory and get an image from it to send back out the method.
                var encoderParameters = new EncoderParameters(2);
                encoderParameters.Param[0] = new EncoderParameter(Encoder.Compression, 100);
                encoderParameters.Param[1] = new EncoderParameter(Encoder.Quality, 100L);
                var ms = new MemoryStream();
                //bmPhoto.Save(mm, Image.RawFormat);

                bmPhoto.Save(ms, GetImageCodeInfo("image/jpeg"), encoderParameters);
                image.Dispose();
                bmPhoto.Dispose();
                grPhoto.Dispose();

                Image outimage = System.Drawing.Image.FromStream(ms);
                return outimage;
            }
            catch (Exception ex)
            {
                throw new Exception("Error cropping image, the error was: " + ex.Message);
            }
        }

        //Hard resize attempts to resize as close as it can to the desired size and then crops the excess
        public Image HardResizeImage(Image image, int width, int height)
        {
            Image resized = null;
            resized = ResizeImage(image, width, height);
            Image output = CropImage(resized, width, height);
            //return the original resized image
            return output;
        }

        //Image resizing
        public Image ResizeImage(Image image, int newWidth, int newHeight)
        {
            int width = image.Width;
            int height = image.Height;
            if (width > newWidth || height > newHeight)
            {
                //The flips are in here to prevent any embedded image thumbnails -- usually from cameras
                //from displaying as the thumbnail image later, in other words, we want a clean
                //resize, not a grainy one.
                image.RotateFlip(RotateFlipType.Rotate180FlipX);
                image.RotateFlip(RotateFlipType.Rotate180FlipX);

                float ratio = 0;
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
                    if(width > height)
                    {
                        ratio = (float)width / (float)height;
                        height = newHeight;
                        width = Convert.ToInt32(Math.Round((float)height * ratio));
                    }
                    else
                    {
                        ratio = (float)width / (float)height;
                        height = newHeight;
                        width = Convert.ToInt32(Math.Round((float)height * ratio));
                    }
                }

                //return the resized image
                var b = new Bitmap(width, height);
                var g = Graphics.FromImage(b);

                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.DrawImage(image, 0, 0, width, height);
                g.Dispose();
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