using UnityEditor;
using UnityEngine;
using FIMSpace.FEditor;

namespace FIMSpace.FTextureTools
{
    public class FSeamlessWindow : FTextureProcessWindow
    {
        public enum FESeamlessAxis { XY, X, Y }
        public enum FEStampMode { Stamping, SplatMode, NoStamping }

        private float hardness = 0.6f;
        private float randomize = 0.25f;
        private float stamperRadius = 0.45f;
        private float stampDensity = 0.4f;
        private float stampNoiseMask = 1.0f;
        private int stampRotate = 1;

        private FEStampMode stampMode = FEStampMode.Stamping;
        private FESeamlessAxis toLoop = FESeamlessAxis.XY;

        public bool StampWithOtherTexture = false;
        public Texture2D StampWith = null;
        bool StampWithReadable = false;

        protected override string Title => "Seamless Texture Generator";

        public static void Init()
        {
            FSeamlessWindow window = (FSeamlessWindow)GetWindow(typeof(FSeamlessWindow));
            window.titleContent = new GUIContent("Seamless Generator", FTextureToolsGUIUtilities.FindIcon("SPR_SeamlessGenSmall"), "Stamp sides of the texture to make it tile");

            window.previewSize = 100;
            window.position = new Rect(340, 40, 577, 672);

            window.Show();
            called = true;
        }

        protected override void OnGUICustom()
        {
            EditorGUI.BeginChangeCheck();

            GUILayout.Space(4);
            seed = EditorGUILayout.IntSlider(new GUIContent("  Seed:", FTextureToolsGUIUtilities.FindIcon("FRandomIcon")), seed, -50, 50);
            //stampMode = (FEStampMode)EditorGUILayout.EnumPopup("Stmap Mode", stampMode);
            GUILayout.Space(6);

            StampWithReadable = false;
            if (stampMode != FEStampMode.NoStamping)
            {

                stamperRadius = EditorGUILayout.Slider(new GUIContent("Stamp Radius"), stamperRadius, 0f, 1f);
                stampDensity = EditorGUILayout.Slider(new GUIContent("Stamp Density"), stampDensity, 0f, 1f);
                hardness = EditorGUILayout.Slider(new GUIContent("Hardness"), hardness, 0.0f, 1f);
                GUILayout.Space(6);

                stampNoiseMask = EditorGUILayout.Slider(new GUIContent("Stamp Noise"), stampNoiseMask, 0.0f, 2f);
                if (stampNoiseMask > 1.35f) EditorGUILayout.HelpBox("Stamp Noise greater than 1 can reveal seams, beware!", MessageType.None);

                GUILayout.Space(6);
                randomize = EditorGUILayout.Slider(new GUIContent("Randomize"), randomize, 0.0f, .5f);

                stampRotate = EditorGUILayout.IntSlider(new GUIContent("Rotate"), stampRotate, 0, 360);
                if (stampRotate > 1f) EditorGUILayout.HelpBox("Rotation greater than 1 can create non precise pixels!", MessageType.None);
                toLoop = (FESeamlessAxis)EditorGUILayout.EnumPopup(new GUIContent("Dimensions to loop"), toLoop);

                GUILayout.Space(4);

                EditorGUILayout.BeginHorizontal();
                StampWithOtherTexture = EditorGUILayout.Toggle(new GUIContent( "Use Other Texture:", "Using other texture as stamps source. Can be useful for stamping textures with stripes."), StampWithOtherTexture);

                if (StampWithOtherTexture)
                {
                    StampWith = (Texture2D)EditorGUILayout.ObjectField(GUIContent.none, StampWith, typeof(Texture2D), false/*, GUILayout.Height(18)*/ );
                }

                EditorGUILayout.EndHorizontal();

                if (StampWithOtherTexture)
                {
                    if (StampWith != null)
                    {
                        string path = AssetDatabase.GetAssetPath(StampWith);
                        TextureImporter tImp = (TextureImporter)AssetImporter.GetAtPath(path);
                        if (tImp is null == false)
                        {
                            StampWithReadable = tImp.isReadable;

                            if (GUILayout.Button("Switch readonly for '" + StampWith.name + "' to " + (!tImp.isReadable).ToString()))
                            {
                                tImp.isReadable = !tImp.isReadable;
                                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                                StampWithReadable = tImp.isReadable;
                            }

                            Texture2D srcTex = GetFirstTexture;
                            if (srcTex) if (StampWith.width != srcTex.width)
                                {
                                    EditorGUILayout.HelpBox("Texture sizes differs, preview can look not as precize!", MessageType.None);
                                }
                        }

                        if (StampWithReadable == false)
                            EditorGUILayout.HelpBox("Texture must have 'Read/Write' enabled", MessageType.Warning);
                    }
                    else
                        StampWithReadable = false;
                }

                FTextureToolsGUIUtilities.DrawUILine(Color.white * 0.35f, 2, 4);
            }

            if (EditorGUI.EndChangeCheck())
                somethingChanged = true;
            else
                somethingChanged = false;
        }


        protected override void ProcessTexture(Texture2D source, Texture2D target, bool preview = true)
        {
            if (stampMode == FEStampMode.NoStamping)
            {
                base.ProcessTexture(source, target, preview);
                return;
            }

            if (!preview) EditorUtility.DisplayProgressBar("Generating Seamless Texture...", "Preparing... ", 2f / 5f);

            #region Preparing variables to use down below

            Color32[] sourcePixels = source.GetPixels32();
            Color32[] newPixels = source.GetPixels32();

            if (source.width != target.width || source.height != target.height)
            {
                Debug.LogError("[SEAMLESS GENERATOR] Source texture is different scale or target texture! Can't create seamless texture!");
                return;
            }

            bool doX = toLoop != FESeamlessAxis.X;
            bool doY = toLoop != FESeamlessAxis.Y;

            Vector2 dimensions = GetDimensions(source);

            #endregion

            #region Stamping Texture

            if (!preview)
                EditorUtility.DisplayProgressBar("Generating Seamless Texture...", "Creating stamps... ", 3f / 5f);

            float stampRadiusX = source.width * Mathf.LerpUnclamped(0.05f, .3f, stamperRadius);
            float stampRadiusY = source.height * Mathf.LerpUnclamped(0.05f, .3f, stamperRadius);

            float stampsOffsetX = stampRadiusX * Mathf.LerpUnclamped(1.45f, 0.45f, stampDensity);
            float stampsOffsetY = stampRadiusY * Mathf.LerpUnclamped(1.45f, 0.45f, stampDensity);

            Texture2D stampWith = source;
            Color32[] stampWithPixels = sourcePixels;

            if (StampWith != null) if (StampWithReadable)
                {
                    if (preview == false)
                    {
                        if (StampWith.width != target.width || StampWith.height != target.height)
                        {
                            stampWith = FTextureEditorToolsMethods.GenerateScaledTexture2DReference(StampWith, new Vector2(target.width, target.height), 4);
                            stampWithPixels = stampWith.GetPixels32();
                        }
                        else
                        {
                            stampWith = StampWith;
                            stampWithPixels = stampWith.GetPixels32();
                        }
                    }
                    else
                    {
                        stampWith = StampWith;
                        stampWithPixels = stampWith.GetPixels32();
                    }
                }

            if (doX)
            {
                int stampsCountX = (int)(source.width / (stampsOffsetX));
                for (int x = 0; x <= stampsCountX; x++)
                {
                    Color32[] stamp = GetStamp(stampWith, stampWithPixels, (int)stampRadiusX);
                    Vector2 pastePos = new Vector2(0, 0);

                    pastePos.x = (int)(x * stampsOffsetX);
                    // Randomize y
                    float boost = 1f;
                    if (stampMode == FEStampMode.SplatMode) boost = 1f / (0.01f + stamperRadius);
                    pastePos.y = (int)((stampRadiusY * 2) * (-1f + rand.NextDouble() * 2f) * 0.5f * randomize * boost);

                    PasteTo(stamp, newPixels, pastePos, GetSquareDimensions(stamp.Length), dimensions);
                }
            }

            if (!preview)
                EditorUtility.DisplayProgressBar("Generating Seamless Texture...", "Creating stamps... ", 3.5f / 5f);

            if (doY)
            {
                int stampsCountY = (int)(source.width / (stampsOffsetY));
                for (int y = 0; y <= stampsCountY; y++)
                {
                    Color32[] stamp = GetStamp(stampWith, stampWithPixels, (int)stampRadiusY);
                    Vector2 pastePos = new Vector2(0, 0);

                    // Randomize x
                    float boost = 1f;
                    if (stampMode == FEStampMode.SplatMode) boost = 1f / (0.01f + stamperRadius);
                    pastePos.x = (int)((stampRadiusX * 2) * (-1f + rand.NextDouble() * 2f) * 0.5f * randomize * boost);
                    pastePos.y = (int)(y * stampsOffsetY);

                    PasteTo(stamp, newPixels, pastePos, GetSquareDimensions(stamp.Length), dimensions);
                }
            }

            #endregion

            // Finalizing changes
            if (!preview) EditorUtility.DisplayProgressBar("Generating Seamless Texture...", "Applying Seamless Texture... ", 3.85f / 5f);

            target.SetPixels32(newPixels);
            target.Apply(false, false);

            if (!preview)
                EditorUtility.ClearProgressBar();
        }


        #region Texture Small Operations

        private Color32[] GetStamp(Texture2D source, Color32[] sourcePixels, int radius)
        {
            double randD = -0.2 + rand.NextDouble() * 1.2;
            if (randomize > 0)
            {
                int tRad = (int)((float)radius * (1f + (randD * (randomize * 1f))));
                if (radius < source.width && radius < source.height) radius = tRad;
            }

            int width = radius * 2;
            Color32[] stampPixels = new Color32[width * width];

            Vector2 origin = new Vector2
            {
                x = RandomRange(radius, source.width - radius),
                y = RandomRange(radius, source.height - radius)
            };

            Vector2 stampDim = new Vector2(width, width);
            Vector2 dimensions = GetDimensions(source);

            float randomOff = (float)rand.NextDouble() * radius * 512f;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    int xx = -radius + x;
                    int yy = -radius + y;
                    int i = GetPX(x, y, stampDim);
                    stampPixels[i] = sourcePixels[GetPX((int)origin.x + xx, (int)origin.y + yy, dimensions)];

                    float distance = Vector2.Distance(new Vector2(xx, yy), Vector2.zero);
                    float fadeMul = distance / ((float)radius * 0.95f);
                    stampPixels[i].a = System.Convert.ToByte(Mathf.Min(255, Mathf.Lerp(255 + hardness * 215, 0, fadeMul)));

                    // Applying perlin noise to stamp
                    if (stampNoiseMask > 0f)
                    {
                        if (stampPixels[i].a < 235 + hardness * 15 + stampNoiseMask * 10)
                        {
                            float noise = Mathf.PerlinNoise((float)x / radius * 3f + randomOff, (float)y / radius * 3f + randomOff);

                            float spreadAlpha = Mathf.Lerp(1f, 0f, Mathf.InverseLerp(255, 0, (int)stampPixels[i].a));

                            float noiseMask = stampNoiseMask;
                            if (stampNoiseMask > 1f)
                            {
                                float tA = Mathf.Lerp(255 + Mathf.LerpUnclamped(hardness * 215, 0, noiseMask - 1f), 0, fadeMul);
                                spreadAlpha = Mathf.Lerp(1f, 0f, Mathf.InverseLerp(255, 0, tA));
                                noiseMask -= 1f;
                            }

                            float noiseAlpha = Mathf.LerpUnclamped(1f, noise, noiseMask * 0.95f);
                            noiseAlpha = Mathf.Lerp(noiseAlpha, noiseAlpha * noiseAlpha, noiseMask - 0.5f);
                            noiseAlpha = Mathf.Lerp(1f, noiseAlpha, (1f - (spreadAlpha)));

                            stampPixels[i].a = (byte)(spreadAlpha * (noiseAlpha) * 255);
                        }
                    }
                }
            }

            // Rotating
            if (stampRotate >= 1f)
            {
                RotateImage(stampPixels, width, width, RandomRange(0, stampRotate));
            }


            return stampPixels;
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


    }
}