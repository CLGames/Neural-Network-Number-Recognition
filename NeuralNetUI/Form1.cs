using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeuralNetUI
{
    public partial class DigitRecogniser : Form
    {
        private Bitmap canvasBitmap;
        private bool isDrawing = false;
        private int scale = 10;
        private NeuralNetwork NN;
        public DigitRecogniser()
        {
            InitializeComponent();
            canvasBitmap = new Bitmap(drawPanel.Width, drawPanel.Height);

            using (Graphics g = Graphics.FromImage(canvasBitmap))
            {
                g.Clear(Color.White);
            }

            drawPanel.MouseDown += DrawPanel_MouseDown;
            drawPanel.MouseMove += DrawPanel_MouseMove;
            drawPanel.MouseUp += DrawPanel_MouseUp;
            drawPanel.Paint += DrawPanel_Paint;

            label2.Text = "Prediction: ";

            NN = new NeuralNetwork(784, 128, 10, true);
        }

        private void DrawPanel_MouseDown(object sender, MouseEventArgs e)
        {
            isDrawing = true;
            DrawAt(e.X, e.Y);
        }

        private void DrawPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                DrawAt(e.X, e.Y);
            }
        }

        private void DrawPanel_MouseUp(object sender, MouseEventArgs e)
        {
            isDrawing = false;
        }

        private void DrawPanel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(canvasBitmap, 0, 0);
        }

        private void DrawAt(int x, int y)
        {
            int px = x / scale;
            int py = y / scale;

            using (Graphics g = Graphics.FromImage(canvasBitmap))
            {
                Brush brush = Brushes.Black;

                g.FillEllipse(
                Brushes.Black,
                x - 8,
                y - 8,
                16,
                16);
            }

            drawPanel.Invalidate();
            GetPrediction();
            
        }

        private double[] GetPixels()
        {
            double[] input = new double[784];
            Bitmap processedCanvas = ProcessImage(ResizeBitmap(canvasBitmap, 28, 28));

            for (int y = 0; y < 28; y++)
            {
                for (int x = 0; x < 28; x++)
                {
                    Color pixel = processedCanvas.GetPixel(x, y);

                    double value = 1.0 - (pixel.R / 255.0);

                    input[y * 28 + x] = value;
                }
            }

            return input;
        }

        private void GetPrediction()
        {
            double[] input = GetPixels();
            DisplayProcessedImage(input);

            int prediction = NN.FeedForward(input);
            progressBar1.Value = (int)(100 * NN.output[0].output);
            progressBar2.Value = (int)(100 * NN.output[1].output);
            progressBar3.Value = (int)(100 * NN.output[2].output);
            progressBar4.Value = (int)(100 * NN.output[3].output);
            progressBar5.Value = (int)(100 * NN.output[4].output);
            progressBar6.Value = (int)(100 * NN.output[5].output);
            progressBar7.Value = (int)(100 * NN.output[6].output);
            progressBar8.Value = (int)(100 * NN.output[7].output);
            progressBar9.Value = (int)(100 * NN.output[8].output);
            progressBar10.Value = (int)(100 * NN.output[9].output);

            label2.Text = "Prediction: " + prediction;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            using (Graphics g = Graphics.FromImage(canvasBitmap))
            {
                g.Clear(Color.White);
            }

            drawPanel.Invalidate();
        }

        private void DisplayProcessedImage(double[] pixels)
        {
            Bitmap bitmap = new Bitmap(28, 28);
            for (int y = 0; y < 28; y++)
            {
                for (int x = 0; x < 28; x++)
                {
                    double value = pixels[y * 28 + x];

                    int intensity = (int)(255 * value);

                    Color colour = Color.FromArgb(intensity, intensity, intensity);

                    bitmap.SetPixel(x, y, colour);
                }
            }

            pictureBox1.Image = bitmap;
        }

        private Bitmap ProcessImage(Bitmap image)
        {
            int minX = 27;
            int maxX = 0;
            int minY = 27;
            int maxY = 0;
            for (int y = 0; y < 28; y++)
            {
                for (int x = 0; x < 28; x++)
                {
                    Color c = image.GetPixel(x, y);
                    if (c.R < 250)
                    {
                        minX = Math.Min(minX, x);
                        maxX = Math.Max(maxX, x);
                        minY = Math.Min(minY, y);
                        maxY = Math.Max(maxY, y);
                    }
                }
            }

            int width = maxX - minX;
            int height = maxY - minY;
            int squareSize;

            if (width > height)
            {
                squareSize = width;
            }
            else
            {
                squareSize = height;
            }

            minX -= (squareSize - width) / 2;
            maxX = minX + squareSize;
            minY -= (squareSize - height) / 2;
            maxY = minY + squareSize;

            squareSize = Math.Max(squareSize, 1);

            Rectangle cropArea = new Rectangle(minX, minY, squareSize, squareSize);
            Bitmap crop = CropBitmap(image, cropArea);
            Bitmap resized = ResizeBitmap(crop, 20, 20);

            Bitmap canvas = new Bitmap(28, 28);

            using (Graphics g = Graphics.FromImage(canvas))
            {
                g.Clear(Color.White);

                int offsetX = (28 - 20) / 2;
                int offsetY = (28 - 20) / 2;

                g.DrawImage(resized, offsetX, offsetY);
            }

            return canvas;

        }

        private Bitmap CropBitmap(Bitmap source, Rectangle cropRect)
        {
            Bitmap cropped = new Bitmap(cropRect.Width, cropRect.Height);

            using (Graphics g = Graphics.FromImage(cropped))
            {
                g.DrawImage(
                    source,
                    new Rectangle(0, 0, cropped.Width, cropped.Height),
                    cropRect,
                    GraphicsUnit.Pixel
                );
            }

            return cropped;
        }

        Bitmap ResizeBitmap(Bitmap source, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(result))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                g.DrawImage(source, 0, 0, width, height);
            }

            return result;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void DigitRecogniser_Load(object sender, EventArgs e)
        {

        }
    }
}
