using Emgu.CV;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using MarioMaker2OCR.Objects;
using System;
using System.Drawing;

namespace MarioMaker2OCR
{
    public static class OCRLibrary
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string tesseractLibrary = "tessdata";

        private static Rectangle levelCodeBoundry720 = new Rectangle(78, 175, 191, 33); // based on 1280x720
        private static Rectangle creatorNameBoundry720 = new Rectangle(641, 173, 422, 39); // based on 1280x720
        private static Rectangle levelTitleBoundry720 = new Rectangle(100, 92, 1080, 43); // based on 1280x720
        private static Rectangle levelCodeBoundry;
        private static Rectangle creatorNameBoundry;
        private static Rectangle levelTitleBoundry;

        private static Size resolution720 = new Size(1280, 720);

        /// <summary>
        /// <para>Get Level object from currentFrame. </para>
        /// <para>currentFrame will also have boundries for OCR read drawn in red.</para>
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public static Level GetLevelFromFrame(Image<Bgr, byte> frame)
        {
            updateOCRBoundryResolutions(frame.Size);

            try
            {
                Level ocrLevel = new Level();

                // Level Code
                frame.ROI = levelCodeBoundry;
                ocrLevel.code = getStringFromLevelCodeImage(frame);

                // Level Title
                frame.ROI = levelTitleBoundry;
                ocrLevel.name = getStringFromImage(frame);

                // Creator Name
                frame.ROI = creatorNameBoundry;
                ocrLevel.author = getStringFromImage(frame);

                // hate out params, but this doesn't feel like enough to warrant an event.
                drawCapturedScreenAreas(frame);

                return ocrLevel;
            }
            catch (Exception ex)
            {
                log.Error("Error Performing OCR: " + ex.Message);
                log.Debug(ex.StackTrace);
            }

            return null;
        }

        private static void updateOCRBoundryResolutions(Size newResolution)
        {
            // only update if a new resolution is detected
            if (newResolution.Height != levelCodeBoundry.Height)
            {
                levelCodeBoundry = ImageLibrary.ChangeSize(levelCodeBoundry720, resolution720, newResolution);
                creatorNameBoundry = ImageLibrary.ChangeSize(creatorNameBoundry720, resolution720, newResolution);
                levelTitleBoundry = ImageLibrary.ChangeSize(levelTitleBoundry720, resolution720, newResolution);
            }
        }

        private static string getStringFromImage(Image<Bgr, byte> image)
        {
            using (Image<Gray, byte> ocrReadyImage = ImageLibrary.PrepareImageForOCR(image))
            {
                return doOCROnImage(ocrReadyImage);
            }
        }

        private static string getStringFromLevelCodeImage(Image<Bgr, byte> image)
        {
            Image<Gray, byte> ocrReadyImage = ImageLibrary.PrepareImageForOCR(image);
            return doOCROnLevelCodeImage(ocrReadyImage);
        }

        private static void drawCapturedScreenAreas(Image<Bgr, byte> image)
        {
            // clear any ROI
            image.ROI = Rectangle.Empty;

            MCvScalar borderColor = new Bgr(Color.Red).MCvScalar;
            CvInvoke.Rectangle(image, levelCodeBoundry, borderColor, 2);
            CvInvoke.Rectangle(image, creatorNameBoundry, borderColor, 2);
            CvInvoke.Rectangle(image, levelTitleBoundry, borderColor, 2);
        }

        /// <summary>
        /// Get string from image using Tesserect Library for OCR
        /// Only use eng+jpn language
        /// </summary>
        private static string doOCROnImage(Image<Gray, byte> image)
        {
            using (Tesseract r = new Tesseract(tesseractLibrary, "eng+jpn", OcrEngineMode.TesseractLstmCombined))
            {
                r.PageSegMode = PageSegMode.SingleLine;
                r.SetImage(image);
                r.Recognize();

                string ocrString = r.GetUTF8Text().Trim();
                ocrString = ocrString.Replace('①', '1').Replace('②', '2').Replace('③', '3').Replace('④', '4').Replace('⑤', '5')
                    .Replace('⑥', '6').Replace('⑦', '7').Replace('⑧', '8').Replace('⑨', '9').Replace('⓪', '0');
                return ocrString;
            }
        }

        /// <summary>
        /// Get string from image using Tesserect Library for OCR
        /// Only use eng language and a subset of characters representing MM2 Level Codes
        /// </summary>
        private static string doOCROnLevelCodeImage(Image<Gray, byte> image)
        {
            using (Tesseract r = new Tesseract(tesseractLibrary, "eng", OcrEngineMode.TesseractLstmCombined))
            {
                r.SetVariable("tessedit_char_whitelist", "-0123456789ABCDEFGHJKLMNPQRSTUVWXY"); // only works for OcrEngineMode.TesseractOnly
                r.PageSegMode = PageSegMode.SingleWord;
                r.SetImage(image);
                r.Recognize();
                string originalLevelCode = r.GetUTF8Text().Trim();

                // manual replace for invalid chars that showed up in testing
                // possible solution is to filter out anything not in the whitelist above
                // but just want to make sure there aren't any we could easily replace first
                string modifiedLevelCode = originalLevelCode.Replace(" ", "").Replace("$", "S").Replace("O", "0")
                    .Replace("I", "1").Replace("'", "").Replace("‘", "").Replace("Z","S");

                log.Debug($"level: {originalLevelCode}");

                if (originalLevelCode != modifiedLevelCode)
                    log.Info($"GetStringFromLevelCodeImage - Attempting to correct OCR on level code - original: {originalLevelCode} | modified: {modifiedLevelCode}");

                return modifiedLevelCode;
            }
        }
    }
}
