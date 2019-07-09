using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.Linq;

namespace MarioMaker2OCR
{
    public static class ImageLibrary
    {
        /// <summary>
        /// Grayscale, increase gamma and size of the image.
        /// 
        /// Read on a google forum it was good practice to increase OCR accuracy
        /// </summary>
        public static Image<Gray, byte> PrepareImageForOCR(Image<Bgr, byte> image)
        {
            Image<Gray, byte> grayScaleImage = image.Convert<Gray, byte>();
            grayScaleImage = grayScaleImage.Resize(2.2d, Inter.Cubic);
            grayScaleImage._GammaCorrect(3.5d);
            return grayScaleImage;
        }

        /// <summary>
        /// Compare two images - return the percentage the images match
        /// </summary>
        public static double CompareImages(Mat firstImage, Mat secondImage)
        {
            using (Image<Bgr, byte> diffImage = new Image<Bgr, byte>(firstImage.Size))
            {
                // OpenCV method to produce an image which is the difference of the 2.
                CvInvoke.AbsDiff(firstImage, secondImage, diffImage);

                // Threshold to filter out pixels that are basically a match.
                // Count remaining black pixels. 
                var nonZeroPixels = diffImage.ThresholdBinary(new Bgr(20, 20, 20), new Bgr(255d, 255d, 255d)).CountNonzero().Average();

                // Divide by total pixels in resolution for total percentage images match
                return 1 - (nonZeroPixels / (firstImage.Height * firstImage.Width));
            }
        }

        public static Rectangle ChangeSize(Rectangle rectangle, Size oldResolution, Size newResolution)
        {
            if (oldResolution.Equals(newResolution)) return rectangle;

            float widthDifference = (newResolution.Width) / (float)oldResolution.Width;
            float heightDifference = (newResolution.Height) / (float)oldResolution.Height;


            Rectangle newRectangle = new Rectangle(rectangle.Location, rectangle.Size);
            newRectangle.Height = (int)(newRectangle.Height * heightDifference);
            newRectangle.Y = (int)(newRectangle.Y * heightDifference);

            newRectangle.Width = (int)(newRectangle.Width * widthDifference);
            newRectangle.X = (int)(newRectangle.X*widthDifference);

            return newRectangle;

        }

        public static Mat ChangeSize(Mat image, Size oldResolution, Size newResolution)
        {
            if (oldResolution.Equals(newResolution)) return image;

            double widthDifference = (newResolution.Width) / (double)oldResolution.Width;
            double heightDifference = (newResolution.Height) / (double)oldResolution.Height;

            Mat newImage = new Mat();
            CvInvoke.Resize(image, newImage, new Size(), widthDifference, heightDifference);

            return newImage;

        }
    }
}
