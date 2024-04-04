namespace BitMapMaker
{

    public struct Pixel
    {
        public byte R, G, B;

        public readonly byte[] GetValues()
        {
            return new byte[] { B, G, R };
        }
        public void SetValues(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }
        public void SetValuesAlong(byte Val)
        {
            R = G = B = Math.Clamp(Val, (byte)0, (byte)255);
        }
        public void Noise(byte intensity)
        {
            Random rnd = new();
            if (rnd.Next(0, 2) == 0)
                R = (byte)Math.Clamp(R + BackUtility.RandomRange(0, (byte)(intensity / 2)), 0, 255);
            else
                R = (byte)Math.Clamp(R - BackUtility.RandomRange((byte)(intensity / 2), 0), 0, 255);
            if (rnd.Next(0, 2) == 0)
                G = (byte)Math.Clamp(G + BackUtility.RandomRange(0, (byte)(intensity / 2)), 0, 255);
            else
                G = (byte)Math.Clamp(G - BackUtility.RandomRange((byte)(intensity / 2), 0), 0, 255);
            if (rnd.Next(0, 2) == 0)
                B = (byte)Math.Clamp(B + BackUtility.RandomRange(0, (byte)(intensity / 2)), 0, 255);
            else
                B = (byte)Math.Clamp(B - BackUtility.RandomRange((byte)(intensity / 2), 0), 0, 255);
        }
        public void NoiseAlong(byte intensity)
        {
            Random rnd = new();
            if (rnd.Next(0, 2) == 0)
                this.SetValuesAlong((byte)(B + BackUtility.RandomRange(0, (byte)(intensity / 2))));
            else
                this.SetValuesAlong((byte)(B - BackUtility.RandomRange((byte)(intensity / 2), 0)));
        }
        public void PixelBlur(Pixel[,] image, int startX, int endX, int startY, int endY)
        {
            int totalR = 0, totalG = 0, totalB = 0;
            int count = 0;

            for (int x = startX; x <= endX; x++)
                for (int y = startY; y <= endY; y++)
                    if (x >= 0 && x < image.GetLength(0) && y >= 0 && y < image.GetLength(1))
                    {
                        totalR += image[x, y].R;
                        totalG += image[x, y].G;
                        totalB += image[x, y].B;
                        count++;
                    }

            if (count > 0)
            {
                R = (byte)(totalR / count);
                G = (byte)(totalG / count);
                B = (byte)(totalB / count);
            }
        }
        public void InvertAlong()
        {
            R = (byte)(255 - R);
            G = (byte)(255 - G);
            B = (byte)(255 - B);
        }
        public void ColorShift(short Shift)
        {
            R = (byte)((R + Shift) % 256);
            G = (byte)((G + Shift) % 256);
            B = (byte)((B + Shift) % 256);
        }
        public void MulAlong(float Mul)
        {
            R = (byte)(R * Mul);
            G = (byte)(G * Mul);
            B = (byte)(B * Mul);
        }
    }

    public class BitmapImage
    {
        public Pixel[,] Image;
        public byte[] Data;
        public string Path;

        public int lengthX;
        public int lengthY;

        public BitmapImage(int sizex, int sizey, bool LoadFrom, string path)
        {
            Path = path;
            lengthX = sizex;
            lengthY = sizey;
            Image = new Pixel[lengthX, lengthY];

            // Initialize Image with white pixels (255, 255, 255)
            if (!LoadFrom)
                for (int x = 0; x < sizex; x++)
                    for (int y = 0; y < sizey; y++)
                    {
                        Image[x, y].B = 255;
                        Image[x, y].R = 255;
                        Image[x, y].G = 255;
                    }

            if (LoadFrom)
            {
                (lengthX, lengthY) = GetImageSize(ExtractData(path + ".bmp"));

                Image = new Pixel[lengthX, lengthY];

                FileStream fs = new(Path + ".bmp", FileMode.Open);

                fs.Seek(54, SeekOrigin.Begin);

                for (int x = 0; x < lengthX; x++)
                    for (int y = 0; y < lengthY; y++)
                    {
                        byte[] pixelData = new byte[3];
                        fs.Read(pixelData, 0, 3);

                        Image[x, y].SetValues(pixelData[2], pixelData[1], pixelData[0]);
                    }

                fs.Close();
            }

            Data = GetData(lengthX, lengthY);
        }

        // utilities

        public void SaveImage(string? path = null)
        {
            path ??= Path;
            FileStream fs = new(path + ".bmp", FileMode.Create);
            fs.Write(Data);

            Console.WriteLine("SavingStuff");

            for (int x = 0; x < lengthX; x++)
                for (int y = 0; y < lengthY; y++)
                {
                    byte[] pixelValues = Image[x, y].GetValues();
                    fs.Write(pixelValues);
                }

            fs.Close();
        }

        public void SaveHistogram()
        {
            FileStream fs = new(Path + "_Hist" + ".bmp", FileMode.Create);
            fs.Write(GetData(256, 256));

            Console.WriteLine("SavingStuff");

            byte[] BSingular = new byte[lengthX * lengthY];

            Pixel[] Fimage = BackUtility.FlattenArray((Pixel[,])Image.Clone());

            for (int i = 0; i < BSingular.Length; i++)
                BSingular[i] = (byte)((Fimage[i].GetValues()[0] + Fimage[i].GetValues()[1] + Fimage[i].GetValues()[2]) / 3);

            Array.Sort(BSingular);
            double max = BSingular.Max();

            for (int i = 0; i < BSingular.Length; i++)
                BSingular[i] = (byte)((BSingular[i] / max) * 256);

            int[] AMNT = new int[256];
            Array.Clear(AMNT, 0, AMNT.Length);

            byte LastDone = 0;
            for (int i = 0; i < BSingular.Length; i++)
                if (LastDone != BSingular[i])
                    LastDone = BSingular[i];
                else
                    AMNT[BSingular[i]]++;

            for (int x = 0; x < 256; x++)
                for (int y = 0; y < 256; y++)
                    fs.Write(new byte[] { (byte)(AMNT[x] * AMNT.Max() / 255), (byte)(AMNT[x] * AMNT.Max() / 255), (byte)(AMNT[x] * AMNT.Max() / 255) });

            fs.Close();
        }

        public static byte[] GetData(int Sizex, int Sizey)
        {
            int headerSize = 54; // Size of BMP header
            int imageSize = Sizex * Sizey * 3; // Size of image data (assuming 24 bits per pixel)
            int fileSize = headerSize + imageSize;

            byte[] NewData = new byte[headerSize];

            // BMP header structure (set the correct values accordingly)
            NewData[0] = 0x42; NewData[1] = 0x4D; // Signature "BM"
            NewData[2] = (byte)fileSize; NewData[3] = (byte)(fileSize >> 8);
            NewData[4] = (byte)(fileSize >> 16); NewData[5] = (byte)(fileSize >> 24);
            NewData[10] = (byte)headerSize; // Offset to start of pixel data
            NewData[14] = 40; // DIB Header size
            NewData[18] = (byte)Sizex; NewData[19] = (byte)(Sizex >> 8); // Image width
            NewData[22] = (byte)Sizey; NewData[23] = (byte)(Sizey >> 8); // Image height
            NewData[26] = 1; // Color planes
            NewData[28] = 24; // Bits per pixel (24-bit color depth)
            NewData[34] = (byte)imageSize; NewData[35] = (byte)(imageSize >> 8);
            NewData[36] = (byte)(imageSize >> 16); NewData[37] = (byte)(imageSize >> 24);

            return NewData;
        }

        public static (int Width, int Height) GetImageSize(byte[] headerData)
        {
            if (headerData.Length < 54)
            {
                throw new ArgumentException("Invalid header data format.");
            }

            int width = BitConverter.ToInt32(headerData, 18);
            int height = BitConverter.ToInt32(headerData, 22);

            return (width, height);
        }

        public static byte[] ExtractData(string path)
        {
            FileStream fs = new(path, FileMode.Open);

            byte[] headerData = new byte[54];

            fs.Read(headerData, 0, headerData.Length);

            fs.Close();

            return headerData;
        }

        public void LoadImageFromByteAlong(byte[,] image)
        {
            for (int x = 0; x < image.GetLength(0); x++)
                for (int y = 0; y < image.GetLength(1); y++)
                    Image[x, y].SetValuesAlong(image[x, y]);
        }

        public void SetPixelPerColor(Color[,] color)
        {
            for (int x = 0; x < lengthX; x++)
                for (int y = 0; y < lengthY; y++)
                {
                    Image[x, y].SetValues(color[x, y].R, color[x, y].G, color[x, y].B);
                }
        }

        // filters
        public void Noise(int Xstart, int Xend, int Ystart, int Yend, byte intensity)
        {
            Console.WriteLine($"Adding noise ({intensity})");
            for (int x = Xstart; x < Xend; x++)
                for (int y = Ystart; y < Yend; y++)
                    Image[x, y].Noise(intensity);
            Console.WriteLine("Done");
        }
        public void NoiseAlong(int Xstart, int Xend, int Ystart, int Yend, byte intensity)
        {
            Console.WriteLine($"Adding noise ({intensity})");
            for (int x = Xstart; x < Xend; x++)
                for (int y = Ystart; y < Yend; y++)
                    Image[x, y].NoiseAlong(intensity);
            Console.WriteLine("Done");
        }
        public void Blur(int Xstart, int Xend, int Ystart, int Yend, int intensity)
        {
            int Space = Xend - Xstart;
            Console.WriteLine("Bluring:");
            Parallel.For(Xstart, Xend + 1, x =>
            {
                for (int y = Ystart; y <= Yend; y++)
                    Image[x, y].PixelBlur(Image, x - intensity, x + intensity, y - intensity, y + intensity);

                if (x % Space / 100 == 0)
                    Console.Write($"x");
            });
            Console.WriteLine();
        }
        public void LuminosityInvertion()
        {
            for (int x = 0; x < lengthX; x++)
                for (int y = 0; y < lengthY; y++)
                {
                    Image[x, y].InvertAlong();
                }
        }
        public void ColorShift(short Shift)
        {
            for (int x = 0; x < lengthX; x++)
                for (int y = 0; y < lengthY; y++)
                {
                    Image[x, y].ColorShift(Shift);
                }
        }
        public void ColorInvert()
        {
            ColorShift(128);
        }

        public void Sharpen(float Mul)
        {
            float[,] sharpenMatrix = {
            { -1 * Mul, -1 * Mul, -1 * Mul },
            { -1 * Mul,  9 * Mul, -1 * Mul },
            { -1 * Mul, -1 * Mul, -1 * Mul }
            };

            Pixel[,] NewImage = new Pixel[lengthX, lengthY];

            for (int x = 1; x < lengthX - 1; x++)
            {
                for (int y = 1; y < lengthY - 1; y++)
                {
                    float newR = 0, newG = 0, newB = 0;

                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            newR += Image[x + i, y + j].R * sharpenMatrix[i + 1, j + 1];
                            newG += Image[x + i, y + j].G * sharpenMatrix[i + 1, j + 1];
                            newB += Image[x + i, y + j].B * sharpenMatrix[i + 1, j + 1];
                        }
                    }

                    // Apply the multiplication factor
                    NewImage[x, y].R = Clamp((byte)(Image[x, y].R + newR));
                    NewImage[x, y].G = Clamp((byte)(Image[x, y].G + newG));
                    NewImage[x, y].B = Clamp((byte)(Image[x, y].B + newB));
                }
            }
            Image = NewImage;
        } // broken

        // tools

        public void SharpScale(float scale)
        {
            int Newx = (int)(lengthX * scale);
            int Newy = (int)(lengthY * scale);

            Pixel[,] NewImage = new Pixel[Newx, Newy];

            for (int x = 0; x < Newx; x++)
                for (int y = 0; y < Newy; y++)
                {
                    NewImage[x, y].SetValues(Image[(int)(x / scale), (int)(y / scale)].R, Image[(int)(x / scale), (int)(y / scale)].G, Image[(int)(x / scale), (int)(y / scale)].B);
                }

            lengthX = Newx;
            lengthY = Newy;

            Image = NewImage;

            Data = GetData(Newx, Newy);
        }

        private static byte Clamp(byte value)
        {
            return Math.Clamp(value, (byte)0, (byte)255);
        }

    }

    public static class BackUtility
    {
        static public byte RandomRange(byte min, byte max)
        {
            Random rnd = new();
            if (min > max)
                (min, max) = (max, min);
            return (byte)rnd.Next(min, max);
        }

        static public T[] FlattenArray<T>(T[,] twoDArray)
        {
            int rows = twoDArray.GetLength(0);
            int cols = twoDArray.GetLength(1);

            T[] oneDArray = new T[rows * cols];
            int index = 0;

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                {
                    oneDArray[index] = twoDArray[i, j];
                    index++;
                }

            return oneDArray;
        }

        public static float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
        }
    }
}
