using System;
using UnityEngine;

namespace FIMSpace.FTex
{
    /// <summary>
    /// FM: Lanczos scalling algorithm logics
    /// </summary>
    public static class FTex_ScaleLanczos
    {
        /// <summary>
        /// Lanczos scalling image with given parameters resulting with scaled array of pixels
        /// </summary>
        /// <param name="textureBytes"> Array of pixels from original texture </param>
        /// <param name="sourceWidth"> Width of original texture </param>
        /// <param name="sourceHeight"> Height of original texture </param>
        /// <param name="targetWidth"> Width for new scaled image </param>
        /// <param name="targetHeight"> Height for new scaled image</param>
        /// <param name="quality"> Sample count for scaling algorithm, it's recommended to use low values like 1-4 (max 8 - much slower), if you want scale image up to bigger size than original, you can try use higher quality value - then lower value will give a bit pixelate effect </param>
        /// <returns> Pixels array to create texture with scaled version of original texture </returns>
        public static Color32[] ScaleTexture(Color32[] textureBytes, int sourceWidth, int sourceHeight, int targetWidth, int targetHeight, int quality = 4, bool alpha = true)
        {
            // Calculating mid reference point of image
            double xfactor = targetWidth / (double)sourceWidth;
            double yfactor = targetHeight / (double)sourceHeight;
            int samples = quality;

            if (samples < 1) samples = 1;
            if (samples > 8) samples = 8;

            // Resetting kernels
            LanczosKernel[] kernelsCacheX = new LanczosKernel[100];
            LanczosKernel[] kernelsCacheY = new LanczosKernel[100];

            float[] convolutionBufferR = new float[0];
            float[] convolutionBufferG = new float[0];
            float[] convolutionBufferB = new float[0];
            float[] convolutionBufferA = new float[0];

            targetWidth = (int)(0.5 + sourceWidth * xfactor);
            targetHeight = (int)(0.5 + sourceHeight * yfactor);

            // Creating empty image to be filled
            Color32[] scaled = new Color32[targetWidth * targetHeight];

            // Going through each pixel in x and y
            for (int x = 0; x < targetWidth; x++)
            {
                for (int y = 0; y < targetHeight; y++)
                {
                    double xToMid = x / xfactor;
                    double yToMid = y / yfactor;

                    int xFull = (int)xToMid;
                    double xMid = xToMid - xFull;

                    int yFull = (int)yToMid;
                    double yMid = yToMid - yFull;

                    // Getting kernels for this pixel
                    LanczosKernel xKernel = GetKernel(kernelsCacheX, xfactor, xMid, ref samples, ref convolutionBufferR, ref convolutionBufferG, ref convolutionBufferB, ref convolutionBufferA);
                    LanczosKernel yKernel = GetKernel(kernelsCacheY, yfactor, yMid, ref samples, ref convolutionBufferR, ref convolutionBufferG, ref convolutionBufferB, ref convolutionBufferA);

                    // Calculating pixel
                    Color32 rgb = FastConvolve(textureBytes, sourceWidth, sourceHeight, xFull, yFull, xKernel, yKernel, ref convolutionBufferR, ref convolutionBufferG, ref convolutionBufferB, ref convolutionBufferA, alpha);

                    // Filling image with new pixel
                    scaled[x + y * targetWidth] = rgb;
                }
            }

            return scaled;
        }


        /// <summary>
        /// Convolving pixels with provided kernels
        /// </summary>
        private static Color32 FastConvolve(Color32[] textureBytes, int sourceWidth, int sourceHeight, int x, int y, LanczosKernel xKernel, LanczosKernel yKernel, ref float[] cbr, ref float[] cbg, ref float[] cbb, ref float[] cba, bool alpha = true)
        {
            int midY = yKernel.Size / 2;
            int midX = xKernel.Size / 2;

            CleanArray(cbr);
            CleanArray(cbg);
            CleanArray(cbb);
            CleanArray(cba);

            if (!alpha)
            {
                int targetIndex;
                // Horizontal Convolution
                for (int cY = -midY; cY <= midY; cY++)
                {
                    for (int cX = -midX; cX <= midX; cX++)
                    {
                        int yy = y + cY;
                        if (yy < 0)
                        {
                            yy = 0;
                        }
                        else if (yy >= sourceHeight)
                        {
                            yy = sourceHeight - 1;
                        }

                        int xx = x + cX;
                        if (xx < 0)
                        {
                            xx = 0;
                        }
                        else if (xx >= sourceWidth)
                        {
                            xx = sourceWidth - 1;
                        }

                        targetIndex = xx + yy * sourceWidth;

                        Color32 rgb = textureBytes[targetIndex];

                        cbr[midY + cY] += xKernel.SampleWeights[midX - cX] * rgb.r;
                        cbg[midY + cY] += xKernel.SampleWeights[midX - cX] * rgb.g;
                        cbb[midY + cY] += xKernel.SampleWeights[midX - cX] * rgb.b;
                        cba[midY + cY] += xKernel.SampleWeights[midX - cX] * rgb.a;
                    }
                }
            }
            else
            {
                int targetIndex;
                // Horizontal Convolution
                for (int cY = -midY; cY <= midY; cY++)
                {
                    int yy = y + cY;
                    if (yy < 0 || yy >= sourceHeight) continue;

                    for (int cX = -midX; cX <= midX; cX++)
                    {
                        int xx = x + cX;
                        if (xx < 0 || xx >= sourceWidth) continue;

                        targetIndex = xx + yy * sourceWidth;

                        Color32 rgb = textureBytes[targetIndex];

                        if (rgb.a > 2)
                        {
                            cbr[midY + cY] += xKernel.SampleWeights[midX - cX] * rgb.r;
                            cbg[midY + cY] += xKernel.SampleWeights[midX - cX] * rgb.g;
                            cbb[midY + cY] += xKernel.SampleWeights[midX - cX] * rgb.b;
                        }

                        cba[midY + cY] += xKernel.SampleWeights[midX - cX] * rgb.a;
                    }
                }
            }

            // Vertical Convolution
            double rc = 0, gc = 0, bc = 0, ac = 0;
            for (int cY = -midY; cY <= midY; cY++)
            {
                rc += yKernel.SampleWeights[midY - cY] * cbr[midY + cY];
                gc += yKernel.SampleWeights[midY - cY] * cbg[midY + cY];
                bc += yKernel.SampleWeights[midY - cY] * cbb[midY + cY];
                ac += yKernel.SampleWeights[midY - cY] * cba[midY + cY];
            }

            double normalization = xKernel.Normalizer * yKernel.Normalizer;
            rc /= normalization; gc /= normalization; bc /= normalization; ac /= normalization;

            // Limiting range to avoid color artifacts
            byte r = (byte)Math.Min(255, Math.Max(0, rc));
            byte g = (byte)Math.Min(255, Math.Max(0, gc));
            byte b = (byte)Math.Min(255, Math.Max(0, bc));
            byte a = (byte)Math.Min(255, Math.Max(0, ac));

            return new Color32(r, g, b, a);
        }


        #region Kernel Operations


        /// <summary>
        /// FM: Helper Lanczos Kernel class to help resampling pixels
        /// </summary>
        private class LanczosKernel
        {
            public int Size;
            public float[] SampleWeights;
            public float Normalizer;

            public LanczosKernel(int size, float[] weights, float normalization)
            {
                Size = size;
                SampleWeights = weights;
                Normalizer = normalization;
            }
        }


        private static LanczosKernel GetKernel(LanczosKernel[] kernels, double scale, double mid, ref int samples, ref float[] cbr, ref float[] cbg, ref float[] cbb, ref float[] cba)
        {
            int kernelIndex = (int)(mid * 100);
            LanczosKernel kernel = kernels[kernelIndex];

            if (kernel == null)
            {
                kernel = ComputeKernel(scale, mid, ref samples);
                kernels[kernelIndex] = kernel;
                if (kernel.Size > cbr.Length) cbr = new float[kernel.Size];
                if (kernel.Size > cbg.Length) cbg = new float[kernel.Size];
                if (kernel.Size > cbb.Length) cbb = new float[kernel.Size];
                if (kernel.Size > cba.Length) cba = new float[kernel.Size];
            }

            return kernel;
        }


        /// <summary>
        /// Computing lanczos kernel for given scale and mid value
        /// </summary>
        private static LanczosKernel ComputeKernel(double scale, double mid, ref int samples)
        {
            // How many pixels for one new pixel
            int sampling = (int)(1 + 1.0 / scale);
            if (sampling < samples) sampling = samples;
            if (sampling % 2 == 0) sampling++;

            scale = Math.Min(scale, 1.0);

            LanczosKernel kernel = new LanczosKernel(sampling, new float[sampling], 0);

            int i = 0;
            int halfwindow = sampling / 2;
            for (int dx = -halfwindow; dx <= halfwindow; dx++)
            {
                // Mid point
                double x = scale * (dx + mid);

                double sampleWeight = GetContribution(halfwindow, x);

                // Storing kernel
                float w = (float)(1000 * sampleWeight + 0.5);
                kernel.SampleWeights[i++] = w;
                kernel.Normalizer += w;
            }

            return kernel;
        }


        #endregion


        #region Others

        /// <summary>
        /// Calculating contribution factor for pixel with Lanczos formula
        /// </summary>
        private static double GetContribution(double sampling, double s)
        {
            if (s == 0) return 1.0;
            if (s >= sampling) return 0.0;
            double t = s * Math.PI;
            return sampling * Math.Sin(t) * Math.Sin(t / sampling) / (t * t);
        }

        /// <summary>
        /// Filling array with zeros
        /// </summary>
        private static void CleanArray(float[] array)
        {
            for (int i = 0; i < array.Length; i++) array[i] = 0f;
        }

        #endregion
    }
}