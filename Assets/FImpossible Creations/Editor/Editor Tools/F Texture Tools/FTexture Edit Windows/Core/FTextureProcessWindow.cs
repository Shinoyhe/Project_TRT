using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using FIMSpace.FTex;
using FIMSpace.FEditor;

namespace FIMSpace.FTextureTools
{
    public abstract class FTextureProcessWindow : EditorWindow
    {
        protected enum FEPreview { m_1x1, m_2x2, m_4x4, m_8x8 }
        protected enum EMaskExport { None, Grayscale, Color }

        private Object mainSelection;
        private List<Texture2D> textures;
        public Texture2D GetFirstTexture
        {
            get
            {
                if (textures == null) return null;
                if (textures.Count == 0) return null;
                return textures[0];
            }
        }

        private Texture2D previewWorkTex;
        private Texture2D previewTex;

        protected int previewSize = 128;
        protected int seed;

        protected FEPreview previewScale = FEPreview.m_2x2;
        protected bool drawPreviewScale = true;
        protected bool drawApplyButton = true;
        protected bool drawPreviewButton = true;
        protected bool drawGenCopyButton = true;
        protected string previewTitle = "Preview";
        protected System.Random rand;

        //private Texture2D headerTitle;
        protected bool somethingChanged = true;
        protected static bool called = false;
        GUIStyle prev;

        protected virtual string Title => titleContent.text;
        protected virtual Texture Logo => titleContent.image;
        protected virtual string SubTitle => titleContent.tooltip;


        //public static void Init()
        //{
        //    FTextureProcessWindow window = (FTextureProcessWindow)GetWindow(typeof(FTextureProcessWindow));
        //    window.InitializeWindow(window);
        //    called = true;
        //}

        void OnSelectionChange()
        {
            if (Selection.objects.Length > 0)
                for (int i = 0; i < Selection.objects.Length; i++)
                {
                    Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GetAssetPath(Selection.objects[i]));
                    if (texture != null) return;
                }

            ClearTextures();
        }


        void Reset()
        {
            called = false;
            position = new Rect(position.x, position.y, Mathf.Clamp(position.width, 400, 1200), position.height);
        }

        Vector2 scrollPos = Vector2.zero;

        void OnGUI()
        {
            #region Target Texture Detection

            //if (headerTitle == null) headerTitle = Resources.Load("S_SeamlessTextureGenerator_Header", typeof(Texture2D)) as Texture2D;
            if (textures == null) textures = new List<Texture2D>();
            var ev = Event.current;

            textures.Clear();

            if (Selection.objects.Length > 0)
            {
                for (int i = 0; i < Selection.objects.Length; i++)
                {
                    Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GetAssetPath(Selection.objects[i]));
                    if (texture != null) textures.Add(texture);
                }
            }

            if (textures.Count == 0)
            {
                EditorGUILayout.HelpBox("You must select at least one texture file!", MessageType.Warning);
                ClearTextures();
                return;
            }

            rand = new System.Random(seed);
            if (Selection.objects[0] != mainSelection) ClearTextures();

            #endregion


            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);

            prev = new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleCenter };
            EditorGUI.indentLevel++;
            if (called) Reset();
            int refHeight = 341;


            #region Styles

            GUIStyle middleStyle = new GUIStyle();
            middleStyle.alignment = TextAnchor.MiddleCenter;

            Color preC = GUI.color;

            prev.normal.textColor = Color.black * 0.5f; prev.fontSize = 10;

            #endregion


            #region Header GUI


            GUIContent title = new GUIContent(Title);
            GUI.Label(new Rect(0, 0, position.width, 36), title, FTextureToolsGUIUtilities.HeaderStyleBig);

            float wdth = FTextureToolsGUIUtilities.HeaderStyleBig.CalcSize(title).x;
            GUI.DrawTexture(new Rect(position.width / 2f + wdth / 2f + 20, 6, 24, 24), Logo);

            GUI.Label(new Rect(0, 19, position.width, 50), SubTitle, FTextureToolsGUIUtilities.HeaderStyle);
            int currY = 60;

            GUILayout.Space(currY + 4);
            EditorGUIUtility.labelWidth = 160;

            previewSize = EditorGUILayout.IntSlider(new GUIContent(" Preview Resolution:", EditorGUIUtility.IconContent("ScaleTool").image), previewSize, 64, 256);
            GUILayout.Space(2);
            int targetprevSize = previewSize;

            switch (previewScale)
            {
                case FEPreview.m_1x1: targetprevSize = previewSize * 2; break;
                case FEPreview.m_2x2: break;
                case FEPreview.m_4x4: targetprevSize = previewSize / 2; break;
                case FEPreview.m_8x8: targetprevSize = previewSize / 4; break;
            }

            if (drawPreviewScale)
                previewScale = (FEPreview)EditorGUILayout.EnumPopup(new GUIContent(" Preview Tiling:", EditorGUIUtility.IconContent("GridLayoutGroup Icon").image), previewScale);
            else
                currY -= 18;

            EditorGUIUtility.labelWidth = 0;

            #endregion


            #region Preview Generation

            if (previewTex != null)
                if (previewTex.width != targetprevSize)
                {
                    GUI.backgroundColor = new Color(0.8f, 1f, 0.8f, 1f);
                    if (GUILayout.Button("Refresh Preview Texture")) ClearTextures();
                    GUI.backgroundColor = Color.white;
                    currY += 20;
                }

            if (!previewTex || !previewWorkTex)
            {
                mainSelection = Selection.objects[0];
                GeneratePreviewTexture(targetprevSize);
            }

            if (somethingChanged) RefreshWindowPreview();

            #endregion


            #region Preview GUI


            GUILayout.Space(6);
            FTextureToolsGUIUtilities.DrawUILine(Color.white * 0.35f, 2, 4);
            currY += 50;

            minSize = new Vector2(312 + previewSize * 2f, refHeight + previewSize * 2f);
            maxSize = new Vector2(4096, 4096);

            GUILayout.Space(previewSize * 2 + 18);

            GUI.Label(new Rect(0, currY - 8, position.width / 2, 48), "Source", FTextureToolsGUIUtilities.HeaderStyle);
            GUI.Label(new Rect(position.width / 2, currY - 8, position.width / 2, 48), previewTitle, FTextureToolsGUIUtilities.HeaderStyle);
            currY += 48;


            // Drawing source and preview texture ----------------------------

            int cnt = 4;
            switch (previewScale)
            {
                case FEPreview.m_1x1: break;
                case FEPreview.m_2x2:
                    cnt = 2;
                    break;
                case FEPreview.m_4x4:
                    cnt = 4;
                    break;
                case FEPreview.m_8x8:
                    cnt = 8;
                    break;
            }

            int fr = -cnt + 1;
            currY -= 18;

            if (previewScale == FEPreview.m_1x1)
            {
                Rect pos = new Rect(0, 0, previewSize * 2f + 4, previewSize * 2f + 4);
                pos.x = position.width / 4 - previewSize;
                pos.y = currY;
                GUI.Label(pos, previewWorkTex);

                pos = new Rect(0, 0, previewSize * 2f + 4, previewSize * 2f + 4);
                pos.x = position.width / 2 + position.width / 4 - previewSize;
                pos.y = currY;
                GUI.Label(pos, previewTex);
            }
            else
            {
                if (previewWorkTex)
                    for (int x = fr; x <= -fr; x += 2)
                    {
                        int yi = 0;
                        for (int y = fr; y <= -fr; y += 2)
                        {
                            Rect pos = new Rect(0, 0, targetprevSize + 4, targetprevSize + 4);
                            pos.x = position.width / 4 - targetprevSize / 2 + (targetprevSize / 2) * x;
                            pos.y = currY + targetprevSize * yi;
                            GUI.Label(pos, previewWorkTex);
                            yi++;
                        }
                    }

                for (int x = fr; x <= -fr; x += 2)
                {
                    int yi = 0;
                    for (int y = fr; y <= -fr; y += 2)
                    {
                        Rect pos = new Rect(0, 0, targetprevSize + 4, targetprevSize + 4);
                        pos.x = position.width / 2 + position.width / 4 - targetprevSize / 2 + (targetprevSize / 2) * x;
                        pos.y = currY + targetprevSize * yi;
                        GUI.Label(pos, previewTex);
                        yi++;
                    }
                }
            }

            currY += 18;
            GUILayout.Space(15);

            FTextureToolsGUIUtilities.DrawUILine(Color.white * 0.35f, 2, 4);

            #endregion


            // Parameters GUI ----------------------------
            EditorGUI.BeginChangeCheck();
            OnGUICustom();
            if (EditorGUI.EndChangeCheck()) somethingChanged = true; else somethingChanged = false;

            OnGUIFooter();

            EditorGUILayout.EndScrollView();
        }

        protected virtual void OnGUICustom()
        {

        }

        protected void OnGUIFooter()
        {
            GUILayout.Space(10f);

            GUILayout.BeginHorizontal();

            if (drawApplyButton)
            {
                GUI.backgroundColor = new Color(0.75f, 1f, 0.75f, 1f);

                if (GUILayout.Button(new GUIContent("  Apply To File (No Undo)", FTextureToolsGUIUtilities.FindIcon("FReplaceFileIcon")), GUILayout.Height(30)))
                {
                    for (int i = 0; i < textures.Count; i++)
                    {
                        ApplyTexProcessToFile(textures[i]);
                    }
                }

                GUI.backgroundColor = Color.white;
            }

            if (drawPreviewButton)
                if (GUILayout.Button(new GUIContent("  Preview (Reimport File For Undo)", FTextureToolsGUIUtilities.FindIcon("FPreviewIcon"), "Preview on scene view. Undo after hitting 'reimport' on texture file.\nTexture can look too sharp (mip maps disabled for preview)"), GUILayout.Height(30)))
                {
                    for (int i = 0; i < textures.Count; i++)
                    {
                        ApplyTexProcessToFile(textures[i], true);
                    }
                }

            GUILayout.EndHorizontal();

            if (drawGenCopyButton)
            {
                GUILayout.BeginHorizontal();

                GUI.backgroundColor = new Color(0.8f, 1f, 0.8f, 1f);

                if (GUILayout.Button(new GUIContent("  Result As New File", FTextureToolsGUIUtilities.FindIcon("FSaveIcon")), GUILayout.Height(30)))
                {
                    for (int i = 0; i < textures.Count; i++)
                    {
                        string path = AssetDatabase.GetAssetPath(textures[i]);
                        string newPath = Path.GetDirectoryName(path) + "/" + Path.GetFileNameWithoutExtension(path) + "-" + FTextureEditorToolsMethods.GetFilenameSuffix() + Path.GetExtension(path);

                        Texture2D copied = null;
                        if (AssetDatabase.CopyAsset(path, newPath))
                            copied = (Texture2D)AssetDatabase.LoadAssetAtPath(newPath, typeof(Texture2D));

                        if (copied != null)
                            ApplyTexProcessToFile(copied);
                    }
                }

                GUI.backgroundColor = Color.white;


                if (GUILayout.Button(new GUIContent("  Generate Backup And Apply", FTextureToolsGUIUtilities.FindIcon("FMoreIcon")), GUILayout.Height(30)))
                {
                    for (int i = 0; i < textures.Count; i++)
                    {
                        string path = AssetDatabase.GetAssetPath(textures[i]);
                        string newPath = Path.GetDirectoryName(path) + "/" + Path.GetFileNameWithoutExtension(path) + "-Backup" + Path.GetExtension(path);

                        Texture2D copied = null;
                        if (AssetDatabase.CopyAsset(path, newPath))
                            copied = (Texture2D)AssetDatabase.LoadAssetAtPath(newPath, typeof(Texture2D));

                        if (copied != null)
                            ApplyTexProcessToFile(textures[i]);
                    }
                }

                GUILayout.EndHorizontal();
            }

            prev.alignment = TextAnchor.MiddleRight;
            EditorGUILayout.LabelField("FImpossible Creations " + System.DateTime.Now.Year, prev);
            EditorGUI.indentLevel--;
        }


        protected virtual void ProcessTexture(Texture2D source, Texture2D target, bool preview = true)
        {
            ExampleTemplate(source, target, preview);
        }


        private void ExampleTemplate(Texture2D source, Texture2D target, bool preview = true)
        {
            if (!preview) EditorUtility.DisplayProgressBar("Changing Texture...", "Preparing... ", 2f / 5f);

            #region Preparing variables to use down below

            Color32[] sourcePixels = source.GetPixels32();
            Color32[] newPixels = source.GetPixels32();

            if (source.width != target.width || source.height != target.height)
            {
                Debug.LogError("[SEAMLESS GENERATOR] Source texture is different scale or target texture! Can't create seamless texture!");
                return;
            }

            Vector2 dimensions = GetDimensions(source);

            #endregion


            // HERE YOUR OPERATIONS ON PIXELS ARRAY


            // Finalizing changes
            if (!preview) EditorUtility.DisplayProgressBar("Changing Texture...", "Applying changes to Texture... ", 3.85f / 5f);
            target.SetPixels32(newPixels);
            target.Apply(false, false);
            if (!preview) EditorUtility.ClearProgressBar();
        }


        #region Texture Small Operations


        private void GeneratePreviewTexture(int previewSize)
        {
            if (previewWorkTex != null) return;

            EditorUtility.DisplayProgressBar("Generating Preview Texture", "Generating scaled preview texture", 0.5f);

            //seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);

            previewWorkTex = new Texture2D(previewSize, previewSize, TextureFormat.ARGB32, false, false);

            if (textures.Count > 0)
                if (textures[0] != null)
                {
                    Color32[] newPixels = FTex_ScaleLanczos.ScaleTexture(FTex_Methods.GetPixelsFrom(textures[0]), textures[0].width, textures[0].height, previewSize, previewSize, 0, false);
                    previewWorkTex.SetPixels32(newPixels);
                    previewWorkTex.Apply(false, false);
                }

            EditorUtility.ClearProgressBar();
        }


        /// <summary>
        /// Pasting texture on another in certain place
        /// </summary>
        private void PasteTo(Color32[] toPaste, Color32[] target, Vector2 origin, Vector2 toPasteDim, Vector2 targetDim)
        {
            for (int x = 0; x < toPasteDim.x; x++)
            {
                for (int y = 0; y < toPasteDim.y; y++)
                {
                    int index = GetPXLoop((int)(origin.x - toPasteDim.x / 2 + x), (int)(origin.y - toPasteDim.y / 2 + y), targetDim);
                    int toP = GetPX(x, y, toPasteDim);
                    target[index] = BlendPixel(target[index], toPaste[toP]);
                }
            }
        }


        #endregion


        #region Texture Utilities


        /// <summary>
        /// Getting index for position on texture
        /// </summary>
        protected int GetPX(int x, int y, Vector2 dimensions)
        {
            if (y < 0) y = 0;
            if (y >= dimensions.y) y = (int)dimensions.y - 1;

            if (x < 0) x = 0;
            if (x >= dimensions.x) x = (int)dimensions.x - 1;

            return (int)Mathf.Min(dimensions.x * dimensions.y - 1, y * dimensions.x + x);
        }


        /// <summary>
        /// Getting index for position on texture looping if greater than dimensions
        /// </summary>
        protected int GetPXLoop(int x, int y, Vector2 dimensions)
        {
            if (x < 0)
            {
                x += (int)dimensions.x;
                x %= (int)dimensions.x;
                if (x < 0) x = -x;
            }
            else if (x >= dimensions.x)
            {
                x -= (int)dimensions.x;
                x %= (int)dimensions.x;
            }

            if (y < 0)
            {
                y += (int)dimensions.y;
                y %= (int)dimensions.y;
                if (y < 0) y = -y;
            }
            else if (y >= dimensions.y)
            {
                y -= (int)dimensions.y;
                y %= (int)dimensions.y;
            }

            return (int)Mathf.Min(dimensions.x * dimensions.y - 1, y * dimensions.x + x);
        }

        /// <summary>
        /// Getting index for position on texture looping if greater than dimensions
        /// </summary>
        protected int GetPXLoopSkipEdges(int x, int y, Vector2 dimensions)
        {
            if (x % dimensions.x == 0) x -= 1;
            if (y % dimensions.y == 0) y -= 1;

            return GetPXLoop(x, y, dimensions);
        }


        /// <summary>
        /// Blending pixels using alpha channel
        /// </summary>
        protected Color32 BlendPixel(Color32 a, Color32 b)
        {
            Color32 blended = Color32.Lerp(a, b, (float)b.a / 255);
            blended.a = a.a;

            return blended;
        }


        /// <summary>
        /// Getting Vector2Int of width and height from texture
        /// </summary>
        protected Vector2 GetDimensions(Texture2D source)
        {
            return new Vector2(source.width, source.height);
        }


        /// <summary>
        /// Just Random.FromTo with custom seed
        /// </summary>
        protected int RandomRange(int from, int to)
        {
            return from + rand.Next(Mathf.Abs(to - from));
        }

        protected float RandomRange(float from, float to)
        {
            return from + (float)rand.NextDouble() * (to - from);
        }

        /// <summary>
        /// Getting dimensions of image by it's pixels count
        /// </summary>
        public Vector2 GetSquareDimensions(int arrayLength)
        {
            int dim = (int)Mathf.Sqrt(arrayLength);
            return new Vector2(dim, dim);
        }


        protected static void RotateImage(Color32[] tex, int width, int height, int angle)
        {
            int x = 0, y = 0;

            Color32[] sourceCopy = new Color32[tex.Length];
            RotateSquare(tex, width, height, (System.Math.PI / 180 * (double)angle));
            for (int j = 0; j < height; j++)
            {
                for (var i = 0; i < width; i++)
                {
                    sourceCopy[width / 2 - width / 2 + x + i + width * (height / 2 - height / 2 + j + y)] = tex[i + j * width];
                }
            }

            sourceCopy.CopyTo(tex, 0);
        }


        private static void RotateSquare(Color32[] tex, int width, int height, double phi)
        {
            int x, y, i, j;
            double sn = System.Math.Sin(phi);
            double cs = System.Math.Cos(phi);
            Color32[] sourceCopy = new Color32[tex.Length];
            tex.CopyTo(sourceCopy, 0);

            int xc = width / 2;
            int yc = height / 2;

            for (j = 0; j < height; j++)
            {
                for (i = 0; i < width; i++)
                {
                    tex[j * width + i] = new Color32(0, 0, 0, 0);
                    x = (int)(cs * (i - xc) + sn * (j - yc) + xc);
                    y = (int)(-sn * (i - xc) + cs * (j - yc) + yc);
                    if ((x > -1) && (x < width) && (y > -1) && (y < height))
                    {
                        tex[j * width + i] = sourceCopy[y * width + x];
                    }
                }
            }
        }


        #endregion


        #region Window Utilities


        private void ClearTextures()
        {
            previewTex = null;
            previewWorkTex = null;
            somethingChanged = true;
        }


        private void RefreshWindowPreview()
        {
            rand = new System.Random(seed);

            previewTex = new Texture2D(previewWorkTex.width, previewWorkTex.height, TextureFormat.ARGB32, false, false);
            ProcessTexture(previewWorkTex, previewTex);
        }

        #endregion


        public void ApplyTexProcessToFile(Texture2D source, bool justUnity = false)
        {
            // Getting textures
            string sPath = AssetDatabase.GetAssetPath(source);

            TextureImporter sourceTex = (TextureImporter)AssetImporter.GetAtPath(sPath);

            if (sourceTex != null)
            {
                // Remember some important texture asset parameters to restore them after changes
                bool swasReadable = sourceTex.isReadable;
                bool doMips = source.mipmapCount > 1;
                TextureFormat outFormat = source.format;
                TextureImporterCompression comp = sourceTex.textureCompression;
                TextureImporterNPOTScale preNPot = sourceTex.npotScale;
                TextureImporterType typ = sourceTex.textureType;
                TextureImporterPlatformSettings sets = sourceTex.GetPlatformTextureSettings("Standalone");

                try
                {
                    EditorUtility.DisplayProgressBar("Processing Texture File...", "Preparing source texture file... ", 1f / 5f);

                    // Making source texture be open for GetPixels method
                    sourceTex.isReadable = true;
                    sourceTex.textureType = TextureImporterType.Default;
                    sourceTex.npotScale = TextureImporterNPOTScale.None;
                    bool premips = sourceTex.mipmapEnabled;
                    sourceTex.mipmapEnabled = false;
                    sourceTex.textureCompression = TextureImporterCompression.Uncompressed;
                    sets.format = TextureImporterFormat.RGBA32;
                    sourceTex.SetPlatformTextureSettings(sets);

                    // Refreshing assets for our changes
                    sourceTex.SaveAndReimport();

                    source.GetPixels32();
                    rand = new System.Random(seed);
                    ProcessTexture(source, source, false);

                    if (justUnity == false)
                    {
                        EditorUtility.DisplayProgressBar("Processing Texture File...", "Saving file... ", 4f / 5f);

                        int startBytes = File.ReadAllBytes(sPath).Length;

                        byte[] fileBytes = null;

                        string extension = Path.GetExtension(sPath);
                        if (extension.ToLower().Contains("png")) fileBytes = source.EncodeToPNG();
                        else if (extension.ToLower().Contains("jpg") || extension.ToLower().Contains("jpeg")) fileBytes = source.EncodeToJPG(95);
                        else if (extension.ToLower().Contains("exr")) fileBytes = source.EncodeToEXR();
                        else if (extension.ToLower().Contains("tga")) fileBytes = FTex_AdditionalEncoders.EncodeToTGA(source);
                        else if (extension.ToLower().Contains("tif")) fileBytes = FTex_AdditionalEncoders.EncodeToTIFF(source);
                        else
                        {
                            Debug.LogError("[SEAMLESS GENERATOR] Not supported format to scale texture, icons scaler supports only .JPG .PNG .TGA .EXR files!");
                        }

                        // Applying changes to file
                        if (fileBytes != null)
                        {
                            File.WriteAllBytes(sPath, fileBytes);
                        }

                        source.Apply(doMips, !swasReadable);

                        EditorUtility.DisplayProgressBar("Processing Texture File...", "Refreshing file... ", 5f / 5f);
                    }
                    else
                    {
                    }

                    // Restoring parameters
                    sourceTex.isReadable = swasReadable;
                    sourceTex.textureCompression = comp;
                    sourceTex.SetPlatformTextureSettings(sets);
                    sourceTex.textureType = typ;
                    sourceTex.mipmapEnabled = premips;
                    sourceTex.npotScale = preNPot;

                    OnAfterProcessingImage(sourceTex);

                    if (justUnity == false)
                        sourceTex.SaveAndReimport();

                    EditorUtility.ClearProgressBar();

                }
                catch (System.Exception exc)
                {
                    sourceTex.isReadable = swasReadable;
                    sourceTex.textureCompression = comp;
                    sourceTex.SetPlatformTextureSettings(sets);
                    sourceTex.textureType = typ;

                    AssetDatabase.ImportAsset(sPath);
                    AssetDatabase.Refresh();
                    Debug.LogError("[SEAMLESS GENERATOR] Something went wrong when rescalling image file! " + exc);
                    EditorUtility.ClearProgressBar();
                }
            }
            else
            {
                Debug.LogError("[SEAMLESS GENERATOR] No Texture to Rescale!");
            }
        }

        protected virtual void OnAfterProcessingImage(TextureImporter importer)
        {

        }

        protected Texture2D GenerateExtraFile()
        {
            string path = AssetDatabase.GetAssetPath(GetFirstTexture);
            string newPath = Path.GetDirectoryName(path) + "/" + Path.GetFileNameWithoutExtension(path) + "-" + FTextureEditorToolsMethods.GetFilenameSuffix() + Path.GetExtension(path);

            Texture2D copied = null;
            if (AssetDatabase.CopyAsset(path, newPath)) copied = (Texture2D)AssetDatabase.LoadAssetAtPath(newPath, typeof(Texture2D));
            return copied;
        }

    }
}