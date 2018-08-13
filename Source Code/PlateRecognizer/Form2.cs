using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Linq;

namespace PlateRecognizer {
    public partial class Form2 : Form {
        public Form2() {
            InitializeComponent();

            Thread th = new Thread(imageThread);
            th.IsBackground = true;
            th.Start();
        }

        private Capture _capture;
        private int learningCount = 2; // Testar 2 vezes cada tipo de placa então enviar para a proxima etapa
        private const int sleepTimeSmall = 40;
        private const int sleepTimeBig = 3000;
        List<Plate> listLearning = new List<Plate>();

        private void imageThread() {
            _capture = new Emgu.CV.Capture();
            Image<Bgr, byte> imageFrame, imageFrameLearning;
            PlateType learningStage = PlateType.Moto;

            try {
                while (true) {
                    using (imageFrame = _capture.QueryFrame().ToImage<Bgr, Byte>()) {
                        if (imageFrame != null) {
                            imageFrame.Resize(640, 480, Emgu.CV.CvEnum.Inter.Linear, true);

                            if (learningStage.In(PlateType.Moto, PlateType.Carro)) {
                                imageFrameLearning = imageFrame.GetLearningArea();
                                imgCamUser2.Image = imageFrameLearning;

                                if (learning(imageFrameLearning, learningStage, imageFrame)) {
                                    if (listLearning.Count == learningCount) {
                                        if (learningStage == PlateType.Moto) {
                                            learningStage = PlateType.Carro;
                                            learningCount *= 2;
                                        } else {
                                            learningStage = PlateType.None;
                                        }
                                    }
                                } else {
                                    imageFrame.WriteMessage("Aprendizagem - " + learningStage.ToString(), "Posicione a placa");
                                    imageFrame.DrawLearningSquare();
                                }
                            } else {
                                PlateType pT = identifyPlate(imageFrame);

                                if (pT == PlateType.None) {
                                    imageFrame.WriteMessage("Nenhuma placa identificada");
                                } else {
                                    imageFrame.WriteMessage("Placa identificada - " + pT.ToString());
                                }
                            }

                            imgCamUser.Image = imageFrame;
                        }
                    }

                    Thread.Sleep(sleepTimeSmall);
                }
            } catch (Exception ex) {
                MessageBox.Show("Um erro ocorreu: " + ex.Message);
            }
        }

        private bool learning(Image<Bgr, byte> imageFrame, PlateType plateType, Image<Bgr, byte> mainFrame) {
            var plate = scanImage(imageFrame);

            if (plate != null) { //Identificou
                CountdownMessage("Centralize a imagem...", true);

                mainFrame = _capture.QueryFrame().ToImage<Bgr, Byte>();
                imageFrame = mainFrame.GetLearningArea();
                plate = scanImage(imageFrame);

                if (plate != null) {
                    CountdownMessage("Remova a placa");
                    plate.plateType = plateType;
                    listLearning.Add(plate);

                    return true;
                } else {
                    CountdownMessage("Nao foi possivel reconhecer");

                    return false;
                }
            }

            return false;
        }

        #region Image Recognition
        private PlateType identifyPlate(Image<Bgr, byte> imageFrame) {
            var plate = scanImage(imageFrame);

            if (plate != null) {
                for (int i = 0; i < listLearning.Count; i++) {
                    int diffW, diffH;
                    double diffA;

                    diffH = plate.Height - listLearning[i].Height;
                    diffW = plate.Width - listLearning[i].Width;
                    diffA = plate.Aspect - listLearning[i].Aspect;

                    diffH = diffH < 0 ? diffH * -1 : diffH;
                    diffW = diffW < 0 ? diffW * -1 : diffW;
                    diffA = diffA < 0 ? diffA * -1 : diffA;

                    listLearning[i].Diff = diffH + diffW + diffA;
                }

                return listLearning.OrderBy(p => p.Diff).FirstOrDefault().plateType;
            }

            return PlateType.None;
        }

        private Plate scanImage(Image<Bgr, byte> imageFrame) {
            Image<Bgr, byte> workingImage = PlateRecognition(imageFrame);
            imgCamUser3.Image = workingImage;

            int lowerWidth = int.MaxValue, higherWidth = 0,
                lowerHeight = int.MaxValue, higherHeight = 0;

            //Bitmap img = imageRed.ToBitmap();
            //img = img.Clone(new Rectangle(new Point(0, 0), img.Size), System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            Color pixel;
            int x = 0, y = 0;
            byte red, green, blue;

            for (x = 0; x < workingImage.Height; x++) {
                for (y = 0; y < workingImage.Width; y++) {
                    blue = workingImage.Data[x, y, 0];
                    green = workingImage.Data[x, y, 1];
                    red = workingImage.Data[x, y, 2];


                    if (red == 255 && blue == 0 && green == 0) {
                        if (x < lowerWidth)
                            lowerWidth = x;

                        if (y < lowerHeight)
                            lowerHeight = y;

                        if (x > higherWidth)
                            higherWidth = x;

                        if (y > higherHeight)
                            higherHeight = y;
                    }
                }
            }

            int totalWidth = higherWidth - lowerWidth;
            int totalHeight = higherHeight - lowerHeight;
            int totalArea = totalWidth * totalHeight;

            if (totalHeight > 0 && totalWidth > 0) {
                if (totalArea.Between(17000, 50000)) {
                    return new Plate() {
                        Width = totalWidth,
                        Height = totalHeight
                    };
                }
            }

            return null;
        }

        //private Image<Bgr, byte> PlateRecognition(Image<Bgr, byte> imageFrame) {
        //    Bitmap img = imageFrame.ToBitmap();
        //    img = img.Clone(new Rectangle(new Point(0, 0), img.Size), System.Drawing.Imaging.PixelFormat.Format32bppArgb);

        //    Color pixel;
        //    int ct = 80, x = 0, y = 0;

        //    for (x = 0; x < img.Width; x++) {
        //        for (y = 0; y < img.Height; y++) {
        //            pixel = img.GetPixel(x, y);

        //            if (pixel.R < ct && pixel.G < ct && pixel.B < ct) {
        //                img.SetPixel(x, y, Color.Red);
        //            }
        //        }
        //    }

        //    int maxColorSize = img.Width / 10;

        //    int tempCount = 0, tempX = 0;
        //    int i;

        //    for (y = 0; y < img.Height; y++) {
        //        tempCount = 0;
        //        tempX = 0;

        //        for (x = 0; x < img.Width; x++) {
        //            pixel = img.GetPixel(x, y);

        //            if (pixel.R > 250 && pixel.B < 10 && pixel.G < 10) {
        //                tempCount++;
        //            } else {
        //                if (tempCount > maxColorSize) {
        //                    for (i = tempX; i < x; i++) {
        //                        img.SetPixel(i, y, Color.Black);
        //                    }
        //                }

        //                tempCount = 0;
        //                tempX = x;
        //            }
        //        }

        //        if (tempCount > 0) {
        //            for (i = tempX; i < img.Width; i++) {
        //                img.SetPixel(i, y, Color.Black);
        //            }
        //        }
        //    }

        //    return new Image<Bgr, byte>(img);

        //}

        private Image<Bgr, byte> PlateRecognition(Image<Bgr, byte> imageFrame) {
            Image<Bgr, byte> workingImage = imageFrame.Clone();
            int ct = 80, x = 0, y = 0;
            byte red, green, blue;


            for (x = 0; x < workingImage.Height; x++) {
                for (y = 0; y < workingImage.Width; y++) {
                    blue = workingImage.Data[x, y, 0];
                    green = workingImage.Data[x, y, 1];
                    red = workingImage.Data[x, y, 2];

                    if (red < ct && green < ct && blue < ct) {
                        workingImage.Data[x, y, 0] = 0; // Blue
                        workingImage.Data[x, y, 1] = 0; //Green
                        workingImage.Data[x, y, 2] = 255; //Red
                    }
                }
            }

            int maxColorSize = workingImage.Width / 8;

            int tempCount = 0, tempX = 0;
            int i;

            for (x = 0; x < workingImage.Height; x++) {
                tempCount = 0;
                tempX = 0;

                for (y = 0; y < workingImage.Width; y++) {
                    red = workingImage.Data[x, y, 0];
                    green = workingImage.Data[x, y, 1];
                    blue = workingImage.Data[x, y, 2];

                    if (red == 255 && green == 0 && blue == 0) {
                        tempCount++;
                    } else {
                        if (tempCount > maxColorSize) {
                            for (i = tempX; i < x; i++) {
                                workingImage.Data[x, i, 0] = 0;
                                workingImage.Data[x, i, 1] = 0;
                                workingImage.Data[x, i, 2] = 0;
                            }
                        }
                    }
                }

                if (tempCount > 0) {
                    for (i = tempX; i < workingImage.Width; i++) {
                        workingImage.Data[x, i, 0] = 0;
                        workingImage.Data[x, i, 1] = 0;
                        workingImage.Data[x, i, 2] = 0;
                    }
                }
            }

            return workingImage;
        }


        private void CountdownMessage(string message, bool hasLearningSquare = false) {
            int tempCount = 0;
            string tempMessage = message;
            Image<Bgr, byte> mainFrame;

            while (tempCount < sleepTimeBig) {
                using (mainFrame = _capture.QueryFrame().ToImage<Bgr, Byte>()) {
                    if (hasLearningSquare) {
                        mainFrame.DrawLearningSquare();

                        message = tempMessage + "... " + ((100 / (double)sleepTimeBig) * (double)tempCount).ToString("0.00") + "%";
                    }

                    mainFrame.WriteMessage(message);



                    imgCamUser.Image = mainFrame;

                    Thread.Sleep(sleepTimeSmall);
                    tempCount += sleepTimeSmall;
                }
            }

        }
        #endregion

        #region Class
        public class Plate {
            public PlateType plateType { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public double Aspect { get { return ((double)Width / (double)Height); } }

            public double Diff { get; set; }
        }

        public enum PlateType {
            None = 0,
            Moto = 1,
            Carro = 2
        }
        #endregion
    }

    #region Extensions
    static class Ext {
        private static Bgr redColor = new Bgr(0, 0, 255);

        public static void WriteMessage(this Image<Bgr, byte> imageFrame, params string[] listMessage) {
            int height = 35;

            foreach (var message in listMessage) {
                int wid = (imageFrame.Width / 2) - (int)(message.Length * 10);

                Point pos = new Point(wid, height);

                imageFrame.Draw(message, pos, Emgu.CV.CvEnum.FontFace.HersheyComplex, 1, redColor);

                height += 35;
            }
        }

        public static void DrawLearningSquare(this Image<Bgr, byte> imageFrame) {
            imageFrame.Draw(GetLearningRectangle(imageFrame), redColor, 1);
        }

        public static Image<Bgr, byte> GetLearningArea(this Image<Bgr, byte> imageFrame) {
            return imageFrame.Copy(GetLearningRectangle(imageFrame));
        }

        public static Image<Bgr, byte> GetReadableArea(this Image<Bgr, byte> imageFrame) {
            return imageFrame.Copy(GetLearningRectangle(imageFrame, 500, 375));
        }

        private static Rectangle GetLearningRectangle(this Image<Bgr, byte> imageFrame, int Width = 320, int Height = 240) {
            return new Rectangle(new Point((imageFrame.Width - Width) / 2, (imageFrame.Height - Height) / 2), new Size(Width, Height));
        }
    }

    static class Ext2 {
        public static bool In<T>(this T Obj, params T[] listObj) {
            foreach (T o in listObj) {
                if (EqualityComparer<T>.Default.Equals(Obj, o)) {
                    return true;
                }
            }

            return false;
        }

        public static bool Between(this int value, int minValue, int maxValue) {
            return value >= minValue && value <= maxValue;
        }
    }
    #endregion
}
