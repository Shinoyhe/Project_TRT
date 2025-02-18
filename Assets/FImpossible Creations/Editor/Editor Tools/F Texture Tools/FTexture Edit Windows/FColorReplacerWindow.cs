using FIMSpace.FEditor;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FIMSpace.FTextureTools
{
    public class FColorReplacerWindow : FTextureProcessWindow
    {
        private float EffectBlend = 1f;

        private float HueMatching = 0.75f;
        private float SaturationMatching = 0.0f;
        private float ValueMatching = 0.0f;
        private float ExtraMatching = 0.0f;

        private Color ToReplaceColor = Color.red;
        private Color ToReplaceHSV;

        private float HueOffset = 0.5f;
        private float ValueOffset = 0f;
        private float SaturationOffset = 0f;

        Color MaskingColor = Color.white;
        public Texture2D TexturizeWith = null;
        bool TexturizeIsReadable = false;
        float TexturizeTiling = 1f;

        enum EColorMode
        {
            HueShift, ColorMasking, Texturize
        }

        EColorMode ColorMode = EColorMode.HueShift;
        Texture2D texturizeMemo = null;
        EMaskExport maskExport = EMaskExport.None;

        public static void Init()
        {
            FColorReplacerWindow window = (FColorReplacerWindow)GetWindow(typeof(FColorReplacerWindow));
            window.titleContent = new GUIContent("Color Replacer", FTextureToolsGUIUtilities.FindIcon("SPR_ColorReplace"), "Replace certain color on your texture into other");
            window.previewScale = FEPreview.m_1x1;
            window.drawPreviewScale = true;

            window.previewSize = 128;
            window.position = new Rect(140, 50, 924, 662);
            window.Show();

            called = true;
        }

        protected override void OnGUICustom()
        {
            GUILayout.Space(4);
            GUI.backgroundColor = new Color(0.6f, 1f, 0.7f);
            EffectBlend = EditorGUILayout.Slider(new GUIContent("Effect Blend"), EffectBlend, 0.0f, 1f);
            GUI.backgroundColor = Color.white;
            GUILayout.Space(8);

            ToReplaceColor = EditorGUILayout.ColorField(new GUIContent("Color To Replace"), ToReplaceColor);

            GUILayout.BeginHorizontal();

            EditorGUIUtility.labelWidth = 110;
            float highRange = HueMatching > 1f ? 3f : 1.00001f;
            HueMatching = EditorGUILayout.Slider(new GUIContent("Hue Matching:"), HueMatching, 0f, highRange, GUILayout.MinWidth(360));
            EditorGUIUtility.labelWidth = 45;

            SaturationMatching = EditorGUILayout.Slider(new GUIContent("Sat:"), SaturationMatching, 0f, 1f, GUILayout.MinWidth(200));
            ValueMatching = EditorGUILayout.Slider(new GUIContent("Val:"), ValueMatching, 0f, 1f, GUILayout.MinWidth(200));
            EditorGUIUtility.labelWidth = 54;
            ExtraMatching = EditorGUILayout.Slider(new GUIContent("Extra:", "Matching factor will be reduced if pixel saturation or value is low."), ExtraMatching, 0f, 1f, GUILayout.MinWidth(140));
            GUILayout.EndHorizontal();
            GUILayout.Space(16);

            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            ColorMode = (EColorMode)EditorGUILayout.EnumPopup(ColorMode, GUILayout.Width(140));
            EditorGUILayout.LabelField("New Color Settings:", EditorStyles.centeredGreyMiniLabel);
            GUILayout.Space(160);
            EditorGUILayout.EndHorizontal();
            EditorGUIUtility.labelWidth = 0;

            GUILayout.Space(10);

            if (ColorMode == EColorMode.HueShift)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(34);
                EditorGUIUtility.labelWidth = 60;
                HueOffset = EditorGUILayout.Slider(new GUIContent("Hue:"), HueOffset, -1f, 1f, GUILayout.MinWidth(200));
                SaturationOffset = EditorGUILayout.Slider(new GUIContent("Sat:"), SaturationOffset, -1f, 1f, GUILayout.MinWidth(200));
                ValueOffset = EditorGUILayout.Slider(new GUIContent("Val:"), ValueOffset, -1f, 1f, GUILayout.MinWidth(200));
                EditorGUIUtility.labelWidth = 0;
                GUILayout.EndHorizontal();
            }
            else if (ColorMode == EColorMode.ColorMasking)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(34);
                MaskingColor = EditorGUILayout.ColorField("Masking Color", MaskingColor);

                if (GUILayout.Button(new GUIContent(" Export", FTextureToolsGUIUtilities.FindIcon("SPR_GrayscaleS"), "Export mask as grayscale .png file"), GUILayout.MaxWidth(70)))
                {
                    Texture2D newFile = GenerateExtraFile();
                    if (newFile != null)
                    {
                        maskExport = EMaskExport.Grayscale;
                        ApplyTexProcessToFile(newFile);
                    }
                }

                if (GUILayout.Button(new GUIContent(" Export", FTextureToolsGUIUtilities.FindIcon("SPR_rgbscale"), "Export mask as color channel .png file"), GUILayout.MaxWidth(70)))
                {
                    Texture2D newFile = GenerateExtraFile();
                    if (newFile != null)
                    {
                        maskExport = EMaskExport.Color;
                        ApplyTexProcessToFile(newFile);
                    }
                }

                GUILayout.EndHorizontal();
            }
            else if (ColorMode == EColorMode.Texturize)
            {
                #region Texturize options

                GUILayout.Space(-34);
                TexturizeWith = (Texture2D)EditorGUILayout.ObjectField(GUIContent.none, TexturizeWith, typeof(Texture2D), false/*, GUILayout.Height(18)*/ );

                GUILayout.Space(8);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(34);
                highRange = TexturizeTiling > 4f ? 16f : 4.001f;
                TexturizeTiling = EditorGUILayout.Slider("Texturize Tiling:", TexturizeTiling, 0.05f, highRange);
                EditorGUILayout.EndHorizontal();


                if (TexturizeWith != null)
                {
                    GUILayout.Space(8);

                    string path = AssetDatabase.GetAssetPath(TexturizeWith);
                    TextureImporter tImp = (TextureImporter)AssetImporter.GetAtPath(path);
                    if (tImp is null == false)
                    {
                        TexturizeIsReadable = tImp.isReadable;
                        if (tImp.isReadable == false) GUI.backgroundColor = Color.green;

                        if (GUILayout.Button("Switch readonly for '" + TexturizeWith.name + "' to " + (!tImp.isReadable).ToString()))
                        {
                            tImp.isReadable = !tImp.isReadable;
                            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                            TexturizeIsReadable = tImp.isReadable;
                        }

                        GUI.backgroundColor = Color.white;
                        Texture2D srcTex = GetFirstTexture;
                        if (srcTex) if (TexturizeWith.width != srcTex.width)
                            {
                                EditorGUILayout.HelpBox("Texture sizes differs, preview can look not as precize!", MessageType.None);
                            }
                    }

                    if (TexturizeIsReadable == false)
                        EditorGUILayout.HelpBox("Texture must have 'Read/Write' enabled", MessageType.Warning);
                }
                else
                    TexturizeIsReadable = false;

                #endregion
            }

            maskExport = EMaskExport.None;
            GUILayout.Space(8);
        }


        protected override void ProcessTexture(Texture2D source, Texture2D target, bool preview = true)
        {
            if (!preview) EditorUtility.DisplayProgressBar("Replacing Texture Color...", "Preparing... ", 2f / 5f);


            #region Preparing variables to use down below

            Color32[] sourcePixels = source.GetPixels32();
            Color32[] newPixels = source.GetPixels32();

            if (source.width != target.width || source.height != target.height)
            {
                Debug.LogError("[SEAMLESS GENERATOR] Source texture is different scale or target texture! Can't create seamless texture!");
                return;
            }

            int randX = 0;
            int randY = 0;

            Color[] texturizePixels = null;
            Texture2D texturizeSource = null;
            if (TexturizeWith != null) if (TexturizeIsReadable && ColorMode == EColorMode.Texturize)
                {
                    int targetWidth = Mathf.RoundToInt(source.width / TexturizeTiling);
                    int targetHeight = Mathf.RoundToInt(source.height / TexturizeTiling);
                    if (targetWidth < 1) targetWidth = 1;
                    if (targetHeight < 1) targetHeight = 1;

                    if (texturizeMemo == null || texturizeMemo.width != targetWidth || texturizeMemo.height != targetHeight)
                    {
                        texturizeMemo = FTextureEditorToolsMethods.GenerateScaledTexture2DReference(TexturizeWith, new Vector2(targetWidth, targetHeight), preview ? 2 : 4);
                    }

                    texturizeSource = texturizeMemo;
                    texturizePixels = texturizeSource.GetPixels();

                    //randX = RandomRange(texturizeSource.width / 2, texturizeSource.width / 2 + (RandomRange(0f, 1f) < 0.5f ? texturizeSource.width / 3 : -source.width / 3));
                    //randY = RandomRange(texturizeSource.height / 2, texturizeSource.height / 2 + (RandomRange(0f, 1f) < 0.5f ? texturizeSource.height / 3 : -source.height / 3));
                }


            #endregion


            #region Replacing Texture Color

            if (!preview)
                EditorUtility.DisplayProgressBar("Replacing...", "Replacing... ", 3f / 5f);

            Vector2 dim = new Vector2(source.width, source.height);
            Color.RGBToHSV(ToReplaceColor, out ToReplaceHSV.r, out ToReplaceHSV.g, out ToReplaceHSV.b);

            if (ColorMode == EColorMode.HueShift)
            {
                for (int x = 0; x < source.width; x++)
                {
                    for (int y = 0; y < source.height; y++)
                    {
                        int px = GetPX(x, y, dim);
                        Color sourcePx = sourcePixels[px];
                        float blend = CalculateSimilarity(sourcePx);

                        float h, s, v;
                        Color.RGBToHSV(sourcePx, out h, out s, out v);

                        h += HueOffset;
                        if (h < 0f) h += 1f;
                        if (h > 1f) h -= 1f;

                        Color tgtColor = Color.HSVToRGB(h, Mathf.Clamp01(s + SaturationOffset * blend), Mathf.Clamp01(v + ValueOffset * blend));

                        tgtColor.a = sourcePx.a;
                        tgtColor = Color.Lerp(sourcePx, tgtColor, blend);
                        newPixels[px] = Color32.LerpUnclamped(sourcePixels[px], tgtColor, EffectBlend);
                    }
                }
            }
            else if (ColorMode == EColorMode.ColorMasking)
            {
                for (int x = 0; x < source.width; x++)
                {
                    for (int y = 0; y < source.height; y++)
                    {
                        int px = GetPX(x, y, dim);
                        Color sourcePx = sourcePixels[px];
                        Color tgtColor = sourcePx;
                        float blend = CalculateSimilarity(sourcePx);

                        if (maskExport == EMaskExport.None)
                        {
                            tgtColor = Color.Lerp(sourcePx, MaskingColor, blend);
                        }
                        else if (maskExport == EMaskExport.Grayscale)
                        {
                            tgtColor = Color.Lerp(Color.black, Color.white, blend);
                        }
                        else if (maskExport == EMaskExport.Color)
                        {
                            tgtColor = Color.Lerp(Color.black, MaskingColor, blend);
                        }

                        newPixels[px] = Color32.LerpUnclamped(sourcePixels[px], tgtColor, EffectBlend);
                    }
                }
            }
            else if (ColorMode == EColorMode.Texturize)
            {
                for (int x = 0; x < source.width; x++)
                {
                    for (int y = 0; y < source.height; y++)
                    {
                        int px = GetPX(x, y, dim);
                        Color sourcePx = sourcePixels[px];
                        float blend = CalculateSimilarity(sourcePx);

                        Color tgtColor = sourcePx;
                        if (texturizeSource && TexturizeIsReadable) tgtColor = texturizePixels[GetPXLoopSkipEdges(randX + x, randY + y, new Vector2(texturizeSource.width, texturizeSource.height))];

                        tgtColor.a = sourcePx.a;
                        tgtColor = Color.Lerp(sourcePx, tgtColor, blend);
                        newPixels[px] = Color32.LerpUnclamped(sourcePixels[px], tgtColor, EffectBlend);
                    }
                }
            }

            maskExport = EMaskExport.None;

            #endregion


            // Finalizing changes
            if (!preview) EditorUtility.DisplayProgressBar("Replacing Texture Color...", "Applying Color Replacement to Texture... ", 4f / 5f);

            target.SetPixels32(newPixels);
            target.Apply(false, false);

            if (!preview)
                EditorUtility.ClearProgressBar();
        }

        float CalculateSimilarity(Color color)
        {
            float h, s, v;
            Color.RGBToHSV(color, out h, out s, out v);

            float hueDifference = Mathf.Min(Mathf.Abs(ToReplaceHSV.r - h), 1 - Mathf.Abs(ToReplaceHSV.r - h));
            float hueSimilarity = Mathf.Pow(1f - hueDifference, HueMatching * 32f);
            float finalSimilarity = hueSimilarity;

            float satDifference = Mathf.Min(Mathf.Abs(ToReplaceHSV.g - s), 1 - Mathf.Abs(ToReplaceHSV.g - s));
            float satSimilarity = Mathf.Pow(1f - satDifference, SaturationMatching * 16f);
            finalSimilarity *= Mathf.Lerp(1f, 1f - SaturationMatching, satSimilarity);
            //if (satDifference > 0.5f) finalSimilarity *= Mathf.Lerp(1f, 1f - SaturationMatching, Mathf.InverseLerp(0.5f, 1f, satSimilarity));


            float valDifference = Mathf.Min(Mathf.Abs(ToReplaceHSV.b - v), 1 - Mathf.Abs(ToReplaceHSV.b - v));
            float valSimilarity = Mathf.Pow(1f - valDifference, ValueMatching * 8f);
            finalSimilarity *= Mathf.Lerp(1f, 1f - ValueMatching, valSimilarity);
            //if (valDifference > 0.7f) finalSimilarity *= Mathf.Lerp(1f, 1f - ValueMatching, Mathf.InverseLerp(0.7f, 1f, valSimilarity));

            if (ExtraMatching > 0f)
            {
                float satThresh = Mathf.Lerp(0.05f, 0.4f, ExtraMatching);
                float valThresh = Mathf.Lerp(0.05f, 0.2f, ExtraMatching);
                float reduction = Mathf.InverseLerp(0f, satThresh, s);
                reduction *= Mathf.InverseLerp(0f, valThresh, v);
                finalSimilarity *= reduction;
            }

            return Mathf.Clamp01(finalSimilarity);
        }

    }
}