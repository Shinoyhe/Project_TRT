using FIMSpace.FEditor;
using UnityEditor;
using UnityEngine;

namespace FIMSpace.FTextureTools
{
    public class FTexEqualizeWindow : FTextureProcessWindow
    {
        private float Equalize = 0.5f;
        private float EqualizeWhites = 1f;
        private float EqualizeBlacks = 1f;
        private float EqualizeTexture = 0f;

        bool MaskingMode = false;
        Texture2D TexturizeWith;
        Color MaskingColor = Color.white;
        bool TexturizeReadable = false;
        float TexturizeTiling = 1f;

        Texture2D texturizeMemo = null;
        EMaskExport maskExport = EMaskExport.None;

        public static void Init()
        {
            FTexEqualizeWindow window = (FTexEqualizeWindow)GetWindow(typeof(FTexEqualizeWindow));
            window.titleContent = new GUIContent("Texure Equalize", FTextureToolsGUIUtilities.FindIcon("SPR_EqualizerGen"), "Tweak too bright or too dark parts of your texture");
            window.previewScale = FEPreview.m_1x1;
            window.drawPreviewScale = true;

            window.position = new Rect(340, 50, 585, 630);
            window.Show();
            called = true;
        }


        protected override void OnGUICustom()
        {
            EditorGUI.BeginChangeCheck();

            GUI.backgroundColor = new Color(0.6f, 1f, 0.7f);
            Equalize = EditorGUILayout.Slider(new GUIContent("Equalize Amount"), Equalize, 0.0f, 1f);
            GUI.backgroundColor = Color.white;
            GUILayout.Space(8);

            EditorGUI.indentLevel++;
            EqualizeWhites = EditorGUILayout.Slider(new GUIContent("Whites"), EqualizeWhites, 0f, 2f);
            EqualizeBlacks = EditorGUILayout.Slider(new GUIContent("Shadows"), EqualizeBlacks, 0f, 2f);

            GUILayout.Space(8);

            EditorGUILayout.BeginHorizontal();
            MaskingMode = EditorGUILayout.Toggle(new GUIContent("Masking Mode:", "Using single color to replace target texture areas, which can be useful when generating masks for shaders."), MaskingMode);

            if (MaskingMode)
            {
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

                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.EndHorizontal();

                GUI.backgroundColor = new Color(0.6f, 1f, 0.6f, 1f);
                EqualizeTexture = EditorGUILayout.Slider(new GUIContent("Texturize Blend:"), EqualizeTexture, 0f, 1f);
                GUI.backgroundColor = Color.white;

                TexturizeReadable = false;
                if (EqualizeTexture > 0f)
                {
                    EditorGUILayout.HelpBox("Texture should be tileable in order to paint mask without seams.", MessageType.None);

                    TexturizeWith = (Texture2D)EditorGUILayout.ObjectField(new GUIContent("Texturize (Optional)"), TexturizeWith, typeof(Texture2D), false, GUILayout.Height(40));

                    seed = EditorGUILayout.IntSlider(new GUIContent("  Seed", FTextureToolsGUIUtilities.FindIcon("FRandomIcon")), seed, -50, 50);

                    if (TexturizeWith != null)
                    {
                        string path = AssetDatabase.GetAssetPath(TexturizeWith);
                        TextureImporter tImp = (TextureImporter)AssetImporter.GetAtPath(path);
                        if (tImp is null == false)
                        {
                            TexturizeReadable = tImp.isReadable;

                            GUILayout.Space(4);
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Space(32);
                            if (tImp.isReadable == false) GUI.backgroundColor = Color.green;
                            if (GUILayout.Button("Switch readonly for '" + TexturizeWith.name + "' to " + (!tImp.isReadable).ToString()))
                            {
                                tImp.isReadable = !tImp.isReadable;
                                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                                TexturizeReadable = tImp.isReadable;
                            }
                            GUI.backgroundColor = Color.white;
                            GUILayout.Space(32);
                            EditorGUILayout.EndHorizontal();

                            GUILayout.Space(5);

                            EditorGUILayout.BeginHorizontal();
                            float highRange = TexturizeTiling > 4f ? 16f : 4.001f;
                            TexturizeTiling = EditorGUILayout.Slider("Texturize Tiling:", TexturizeTiling, 0.05f, highRange);
                            EditorGUILayout.EndHorizontal();

                            Texture2D srcTex = GetFirstTexture;
                            if (srcTex) if (TexturizeWith.width != srcTex.width)
                                {
                                    EditorGUILayout.HelpBox("Texture sizes differs, preview can look different than final file result!", MessageType.None);
                                }
                        }

                        if (TexturizeReadable == false)
                            EditorGUILayout.HelpBox("Texture must have 'Read/Write' enabled", MessageType.Warning);
                    }
                    else
                        TexturizeReadable = false;
                }


            }

            maskExport = EMaskExport.None;

            EditorGUI.indentLevel--;

            if (EditorGUI.EndChangeCheck())
                somethingChanged = true;
            else
                somethingChanged = false;
        }


        protected override void ProcessTexture(Texture2D source, Texture2D target, bool preview = true)
        {
            if (!preview) EditorUtility.DisplayProgressBar("Equalizing Texture...", "Preparing... ", 2f / 5f);

            #region Preparing variables to use down below

            Color32[] sourcePixels = source.GetPixels32();
            Color32[] newPixels = source.GetPixels32();

            Color32[] texturizePixels = sourcePixels;
            Texture2D texturizeSource = source;

            if (MaskingMode)
            {
                texturizePixels = texturizeSource.GetPixels32();
                for (int i = 0; i < texturizePixels.Length; i++) texturizePixels[i] = (Color32)MaskingColor;
            }
            else
            {
                if (TexturizeWith != null) if (TexturizeReadable)
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
                        texturizePixels = texturizeSource.GetPixels32();
                    }
            }


            if (source.width != target.width || source.height != target.height)
            {
                Debug.LogError("[SEAMLESS GENERATOR] Source texture is different scale or target texture! Can't create seamless texture!");
                return;
            }

            Vector2 dimensions = GetDimensions(source);

            #endregion


            #region Equalizing Texture

            if (!preview)
                EditorUtility.DisplayProgressBar("Equalizing...", "Equalizing... ", 3f / 5f);


            Vector3 rgbAverages = new Vector3(0.0f, 0.0f, 0.0f);
            Vector3 hsvAverages = new Vector3(0.0f, 0.0f, 0.0f);
            Vector2 dim = new Vector2(source.width, source.height);

            for (int x = 0; x < source.width; x++)
            {
                for (int y = 0; y < source.height; y++)
                {
                    int px = GetPX(x, y, dim);
                    Color c = sourcePixels[px];
                    rgbAverages.x += c.r;
                    rgbAverages.y += c.g;
                    rgbAverages.z += c.b;

                    float h, s, v;
                    Color.RGBToHSV(sourcePixels[px], out h, out s, out v);
                    hsvAverages.x += h;
                    hsvAverages.y += s;
                    hsvAverages.z += v;
                }
            }

            float norm = source.width * source.height;
            Vector3 rgbAveragesN = rgbAverages / norm;
            Vector3 hsvAveragesN = hsvAverages / norm;

            float highestVDiff = 0f;
            for (int x = 0; x < source.width; x++)
            {
                for (int y = 0; y < source.height; y++)
                {
                    int px = GetPX(x, y, dim);

                    float h, s, v;
                    Color.RGBToHSV(sourcePixels[px], out h, out s, out v);
                    hsvAverages.x += h;
                    hsvAverages.y += s;
                    hsvAverages.z += v;

                    float diff = Mathf.Abs(v - hsvAveragesN.z);
                    if (diff > highestVDiff) highestVDiff = diff;
                }
            }


            Color avgHsvCol = Color.HSVToRGB(hsvAveragesN.x, hsvAveragesN.y, hsvAveragesN.z);
            int randX = RandomRange(texturizeSource.width / 2, texturizeSource.width / 2 + (RandomRange(0f, 1f) < 0.5f ? texturizeSource.width / 3 : -source.width / 3));
            int randY = RandomRange(texturizeSource.height / 2, texturizeSource.height / 2 + (RandomRange(0f, 1f) < 0.5f ? texturizeSource.height / 3 : -source.height / 3));

            for (int x = 0; x < source.width; x++)
            {
                for (int y = 0; y < source.height; y++)
                {
                    int px = GetPX(x, y, dim);
                    Color srcPixel = sourcePixels[px];
                    float h, s, v;
                    Color.RGBToHSV(srcPixel, out h, out s, out v);
                    float vDiff = v - hsvAveragesN.z;
                    float diffTexBase = Mathf.Abs(v - hsvAveragesN.z);

                    if (vDiff > 0)
                    {
                        // Whites lower then less changed
                        vDiff *= EqualizeWhites;
                    }
                    else
                    if (vDiff < 0)
                    {
                        // Blacks lower then less changed
                        vDiff *= EqualizeBlacks;
                    }

                    float lerpV = Mathf.Abs(vDiff) / highestVDiff;
                    float tgtV = Mathf.LerpUnclamped(v, hsvAveragesN.z, lerpV);
                    Color tgtColor = Color.HSVToRGB(h, s, tgtV);

                    if (MaskingMode)
                    {
                        if (maskExport == EMaskExport.None)
                        {
                            Color offsetRefColor = texturizePixels[GetPXLoopSkipEdges(randX + x, randY + y, new Vector2(texturizeSource.width, texturizeSource.height))];
                            tgtColor = Color.Lerp(tgtColor, offsetRefColor, lerpV * 2f);
                        }
                        else if (maskExport == EMaskExport.Color)
                        {
                            tgtColor = Color.Lerp(Color.black, MaskingColor, lerpV * 2f);
                        }
                        else if (maskExport == EMaskExport.Grayscale)
                        {
                            tgtColor = Color.Lerp(Color.black, Color.white, lerpV * 2f);
                        }
                    }
                    else if (EqualizeTexture > 0f)
                    {
                        Color offsetRefColor = texturizePixels[GetPXLoopSkipEdges(randX + x, randY + y, new Vector2(texturizeSource.width, texturizeSource.height))];
                        tgtColor = Color.Lerp(tgtColor, offsetRefColor, lerpV * 2f * EqualizeTexture);
                    }

                    tgtColor.a = srcPixel.a;
                    newPixels[px] = Color32.LerpUnclamped(sourcePixels[px], tgtColor, Equalize);
                }
            }

            maskExport = EMaskExport.None;

            #endregion


            // Finalizing changes
            if (!preview) EditorUtility.DisplayProgressBar("Equalizing Texture...", "Applying Equalization to Texture... ", 3.85f / 5f);

            target.SetPixels32(newPixels);
            target.Apply(false, false);

            if (!preview)
                EditorUtility.ClearProgressBar();

        }


    }
}