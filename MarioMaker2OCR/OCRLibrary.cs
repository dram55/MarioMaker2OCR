using Emgu.CV;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.CvEnum;
using MarioMaker2OCR.Objects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace MarioMaker2OCR
{
    public static class OCRLibrary
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string tesseractLibrary = "tessdata";

        // Level Code Boundries
        private static Rectangle levelCodeBoundry720 = new Rectangle(78, 175, 191, 33); // based on 1280x720
        private static Rectangle creatorNameBoundry720 = new Rectangle(641, 173, 422, 39); // based on 1280x720
        private static Rectangle levelTitleBoundry720 = new Rectangle(100, 92, 1080, 43); // based on 1280x720
        private static Rectangle levelCodeBoundry;
        private static Rectangle creatorNameBoundry;
        private static Rectangle levelTitleBoundry;

        // Clear Time Boundry
        private static Rectangle clearTimeBoundry;
        private static Rectangle clearTimeBoundry720 = new Rectangle(602, 357, 176, 37); // based on 1280x720

        private static Size resolution720 = new Size(1280, 720);

        /// <summary>
        /// <para>Get Level object from currentFrame. </para>
        /// <para>currentFrame will also have boundries for OCR read drawn in red.</para>
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public static Level GetLevelFromFrame(Image<Bgr, byte> frame)
        {
            updateLevelBoundryResolutions(frame.Size);

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

        internal static string GetClearTimeFromFrame(Image<Bgr, byte> frame)
        {
            // Update boundry sizes relative to the current resolution
            if (frame.Size.Height != clearTimeBoundry.Height)
                clearTimeBoundry = ImageLibrary.ChangeSize(clearTimeBoundry720, resolution720, frame.Size);

            // Set ROI
            frame.ROI = clearTimeBoundry;

            // Prepare Image for OCR
            Image<Gray, byte> ocrReadyImage = ImageLibrary.PrepareImageForOCR(frame);

            // Segment characters
            List<Mat> characters = segmentCharacters(ocrReadyImage);

            // expect time to be 9 characters, quote reads as 2 chars (ex: 01'34''789)
            if (characters.Count == 10)
            {
                // Different Time Formats
                // 12:34,567 | 12:34.567 | 12'34"567

                // Remove punctuation - can identify by height
                // Make relative to max character size incase we ever resize images.
                int maxCharHeight = characters.Max(p => p.Height);
                int minimumHeight = (int)Math.Floor(maxCharHeight * .60);
                characters.RemoveAll(p => p.Height < minimumHeight);

                // Do OCR
                string clearTime = doOCROnCharacterImages(characters, "0123456789");

                // format clear time
                try
                {
                    if (clearTime.Length >= 7)
                        clearTime = $"{clearTime.Substring(0, 2)}'{clearTime.Substring(2, 2)}\"{clearTime.Substring(4, 3)}";
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    log.Error($"ArgumentOutOfRangeException formatting clear time: {clearTime}");
                    clearTime = "";
                }
                return clearTime;
            }
            else
            {
                log.Debug($"segmentChacters for time - {characters.Count} characters detected, expected 10, falling back to original OCR method");
                return doOCROnImage(ocrReadyImage);
            }
        }

        private static void updateLevelBoundryResolutions(Size newResolution)
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
            ocrReadyImage = cropLineOfText(ocrReadyImage, new Size(33, 3));
            List<Mat> characters = segmentCharacters(ocrReadyImage);

            // 11 characters expected in a level code (XXX-XXX-XXX)
            if (characters.Count == 11)
            {
                // Remove the dashes
                characters.RemoveAt(7);
                characters.RemoveAt(3);

                string levelCode = doOCROnCharacterImages(characters, "0123456789ABCDEFGHJKLMNPQRSTUVWXY");

                // format code
                levelCode = $"{levelCode.Substring(0, 3)}-{levelCode.Substring(3, 3)}-{levelCode.Substring(6, 3)}";

                return levelCode;
            }
            else
            {
                // fallback to original OCR if segmentation has unexpected number of characters
                log.Debug($"segmentChacters - level code - detected {characters.Count} characters, falling back to original OCR method");
                return doOCROnLevelCodeImage(ocrReadyImage);
            }
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
        /// 
        /// Only use as a fallback if doOCROnCharacterImages() cannot be called
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
                    .Replace("I", "1").Replace("'", "").Replace("‘", "").Replace("Z", "S");

                log.Debug($"level: {originalLevelCode}");

                if (originalLevelCode != modifiedLevelCode)
                    log.Info($"GetStringFromLevelCodeImage - Attempting to correct OCR on level code - original: {originalLevelCode} | modified: {modifiedLevelCode}");

                return modifiedLevelCode;
            }
        }

        /// <summary>
        /// Get string from list of character images using Tesserect Library for OCR
        /// </summary>
        /// <param name="characters">List of characters</param>
        /// <param name="whiteListCharacters">White listed characters - ignored if empty</param>
        /// <returns></returns>
        private static string doOCROnCharacterImages(List<Mat> characters, string whiteListCharacters)
        {
            string returnString = "";

            using (Tesseract r = new Tesseract(tesseractLibrary, "eng", OcrEngineMode.TesseractOnly))
            {
                if (!string.IsNullOrEmpty(whiteListCharacters))
                    r.SetVariable("tessedit_char_whitelist", whiteListCharacters);

                r.PageSegMode = PageSegMode.SingleChar;
                foreach (var letter in characters)
                {
                    r.SetImage(letter);
                    r.Recognize();
                    returnString += r.GetUTF8Text().Trim();
                }
            }

            return returnString;
        }

        /// <summary>
        /// Crop a line of text to the given dimensions
        /// </summary>
        private static Image<Gray, byte> cropLineOfText(Image<Gray, byte> image, Size dimension)
        {
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat hier = new Mat();

            List<Mat> letters = new List<Mat>();

            // Better contouring with inverted image (black background on white text)
            image._Not();

            // Get rid of extra space around the text
            Mat structuringElement = CvInvoke.GetStructuringElement(ElementShape.Rectangle, dimension, new Point(-1, -1));
            Mat dilation = new Mat();
            CvInvoke.Dilate(image, dilation, structuringElement, new Point(-1, -1), 1, BorderType.Default, new MCvScalar());
            CvInvoke.FindContours(dilation, contours, hier, RetrType.External, ChainApproxMethod.ChainApproxNone);

            // Invert image back to original
            image._Not();

            // Create new image cropped by the new lineBoundry
            Image<Gray, byte> croppedLine = null;
            Rectangle lineBoundry = CvInvoke.BoundingRectangle(contours[0]);
            croppedLine = image.Copy(lineBoundry);

            return croppedLine;
        }

        /// <summary>
        /// Takes an image of a line of text and returns a list of images containing each character.
        /// </summary>
        private static List<Mat> segmentCharacters(Image<Gray, byte> image)
        {
            // Better contouring with inverted image (black background on white text)
            image._Not();

            // Get contours for each character
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat hier = new Mat();
            CvInvoke.FindContours(image, contours, hier, RetrType.External, ChainApproxMethod.ChainApproxTc89Kcos, new Point(0, 0));

            // Invert image back to original
            image._Not();

            // Gather all bounding rectangles (one for each char)
            List<Rectangle> letterBoundries = new List<Rectangle>();
            for (int i = 0; i < contours.Size; i++)
            {
                letterBoundries.Add(CvInvoke.BoundingRectangle(contours[i]));
            }

            List<Mat> characters = new List<Mat>();

            // Sort bounding rectangles from left to right
            letterBoundries = letterBoundries.OrderBy(p => p.Left).ToList();
            for (int i = 0; i < letterBoundries.Count; i++)
            {
                // Grab current letter into new Mat
                image.ROI = letterBoundries[i];
                Mat letter = image.Mat.Clone();

                // 5 pixel white border around the letter
                CvInvoke.CopyMakeBorder(image.Mat, letter, 5, 5, 5, 5, BorderType.Constant, new MCvScalar(255, 255, 255));

                characters.Add(letter);
                image.ROI = Rectangle.Empty;
            }

            return characters;
        }
    }
}