using FIMSpace.FTex;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace FIMSpace.FTex
{
    public enum FETextureExtension
    {
        UNSUPPORTED, JPG, PNG, TGA, TIFF, EXR
    }

    public static class FTex_Methods
    {
        public static FETextureExtension GetFileExtension(string path)
        {
            string extension = Path.GetExtension(path);

            if (extension.ToLower().Contains("png")) return FETextureExtension.PNG;
            else if (extension.ToLower().Contains("jpg") || extension.ToLower().Contains("jpeg")) return FETextureExtension.JPG;
            else if (extension.ToLower().Contains("tga")) return FETextureExtension.TGA;
            else if (extension.ToLower().Contains("tif")) return FETextureExtension.TIFF;
            else if (extension.ToLower().Contains("exr")) return FETextureExtension.EXR;

            return FETextureExtension.UNSUPPORTED;
        }

        public static int FindNearestPowOf2(int val)
        {
            return Mathf.ClosestPowerOfTwo(val);
        }

        public static int FindHigherPowOf2(int val)
        {
            return Mathf.NextPowerOfTwo(val+1);
        }

        public static int FindLowerPowOf2(int val)
        {
            return Mathf.NextPowerOfTwo((val-1) / 2);
        }


        public static Color32[] GetPixelsFrom(Texture2D source)
        {
            Color32[] newPixels = null;

#if UNITY_EDITOR
            string sPath = AssetDatabase.GetAssetPath(source);
            TextureImporter sourceTex = (TextureImporter)AssetImporter.GetAtPath(sPath);

            if (sourceTex != null)
            {
                bool swasReadable = sourceTex.isReadable;
                sourceTex.isReadable = true;
                sourceTex.SaveAndReimport();

                newPixels = source.GetPixels32();

                sourceTex.isReadable = swasReadable;
                sourceTex.SaveAndReimport();
            }
#endif

            return newPixels;
        }
    }
}