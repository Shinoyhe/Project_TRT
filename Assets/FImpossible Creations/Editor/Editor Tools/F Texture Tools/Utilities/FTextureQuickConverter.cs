using FIMSpace.FTextureTools;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FIMSpace.FEditor
{
    public class FTextureQuickConverter : EditorWindow
    {
        private List<Texture2D> textures;
        private bool dimSetted = false;
        public string postFix = "-ToPNG";

        public enum EChannelSelect { R, G, B, A, SetBlack, SetWhite, SetGray }
        public EChannelSelect SwapRedTo = EChannelSelect.R;
        public EChannelSelect SwapGreenTo = EChannelSelect.G;
        public EChannelSelect SwapBlueTo = EChannelSelect.B;
        public EChannelSelect SwapAlphaTo = EChannelSelect.A;

        public static void Init()
        {
            FTextureQuickConverter window = (FTextureQuickConverter)GetWindow(typeof(FTextureQuickConverter));

            window.minSize = new Vector2(250f, 180f);
            window.maxSize = new Vector2(250f, 250f);

            window.titleContent = new GUIContent("PNG Conversion", FTextureToolsGUIUtilities.FindIcon("SPR_TexTool"));
            window.position = new Rect(200, 100, 250, 188f);
            window.Show();
        }


        void OnGUI()
        {
            if (textures == null) textures = new List<Texture2D>();
            textures.Clear();

            if (Selection.objects.Length > 0)
            {
                for (int i = 0; i < Selection.objects.Length; i++)
                {
                    Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GetAssetPath(Selection.objects[i]));
                    if (texture != null) textures.Add(texture);
                }

                if (!dimSetted)
                    if (textures.Count > 0)
                        dimSetted = true;
            }

            if (textures.Count == 0)
            {
                EditorGUILayout.HelpBox("You must select at least one texture file!", MessageType.Warning);
                return;
            }

            EditorGUILayout.LabelField("Textures to convert: " + textures.Count);

            SwapRedTo = (EChannelSelect)EditorGUILayout.EnumPopup("Red to:", SwapRedTo);
            SwapGreenTo = (EChannelSelect)EditorGUILayout.EnumPopup("Green to:", SwapGreenTo);
            SwapBlueTo = (EChannelSelect)EditorGUILayout.EnumPopup("Blue to:", SwapBlueTo);
            SwapAlphaTo = (EChannelSelect)EditorGUILayout.EnumPopup("Alpha to:", SwapAlphaTo);

            GUILayout.Space(4);
            if (GUILayout.Button("Reset"))
            {
                SwapRedTo = EChannelSelect.R;
                SwapGreenTo = EChannelSelect.G;
                SwapBlueTo = EChannelSelect.B;
                SwapAlphaTo = EChannelSelect.A;
            }

            FTextureToolsGUIUtilities.DrawUILine(Color.white * 0.35f, 2, 5);

            postFix = EditorGUILayout.TextField("Suffix:",  postFix);

            if (GUILayout.Button("Duplicate and convert Files (" + textures.Count + ")"))
            {
                for (int i = 0; i < textures.Count; i++)
                {
                    Texture2D pngDuplicate = FTextureEditorToolsMethods.DuplicateAsPNG(textures[i]);

                    if ( SwapRedTo != EChannelSelect.R || SwapGreenTo != EChannelSelect.G || SwapBlueTo != EChannelSelect.B || SwapAlphaTo != EChannelSelect.A)
                    {
                        if (pngDuplicate != null) ProcessConversion(textures[i], pngDuplicate);
                    }
                }
            }

        }

        public void ProcessConversion(Texture2D source, Texture2D target)
        {
            TextureImporter srcImporter = FTextureEditorToolsMethods.GetTextureAsset(source);
            var srcInfo = FTextureEditorToolsMethods.GetTextureInfo(srcImporter, source);

            TextureImporter tgtImporter = FTextureEditorToolsMethods.GetTextureAsset(target);
            var tgtInfo = FTextureEditorToolsMethods.GetTextureInfo(tgtImporter, target);

            try
            {
                EditorUtility.DisplayProgressBar("Channeling textures...", "Scaling texture " + target.name, 0.2f);

                FTextureEditorToolsMethods.StartEditingTextureAsset(srcImporter, source, srcInfo);
                FTextureEditorToolsMethods.StartEditingTextureAsset(tgtImporter, target, tgtInfo);

                Color32[] srcPixels = source.GetPixels32();
                Color32[] newPixels = target.GetPixels32();

                for (int i = 0; i < newPixels.Length; i++) newPixels[i] = SwapChannels(srcPixels[i], newPixels[i]);

                FTextureEditorToolsMethods.EndEditingTextureAsset(srcPixels, srcInfo, srcImporter, source);
                FTextureEditorToolsMethods.EndEditingTextureAsset(newPixels, tgtInfo, tgtImporter, target);

                EditorUtility.ClearProgressBar();
            }
            catch (System.Exception exc)
            {
                srcInfo.RestoreOn(srcImporter, source, false);
                tgtInfo.RestoreOn(tgtImporter, source, false);

                EditorUtility.ClearProgressBar();
                UnityEngine.Debug.LogError("[Fimpo Image Tools] Something went wrong when channeling textures! " + exc);
            }
        }

        public Color32 SwapChannels(Color32 source, Color32 target)
        {
            Color32 newC = target;

            switch (SwapRedTo)
            {
                case EChannelSelect.R: newC.r = target.r; break;
                case EChannelSelect.G: newC.r = target.g; break;
                case EChannelSelect.B: newC.r = target.b; break;
                case EChannelSelect.A: newC.r = target.a; break;
                case EChannelSelect.SetBlack: newC.r = 0; break;
                case EChannelSelect.SetWhite: newC.r = byte.MaxValue; break;
                case EChannelSelect.SetGray: newC.r = byte.MaxValue / 2; break;
            }

            switch (SwapGreenTo)
            {
                case EChannelSelect.R: newC.g = target.r; break;
                case EChannelSelect.G: newC.g = target.g; break;
                case EChannelSelect.B: newC.g = target.b; break;
                case EChannelSelect.A: newC.g = target.a; break;
                case EChannelSelect.SetBlack: newC.g = 0; break;
                case EChannelSelect.SetWhite: newC.g = byte.MaxValue; break;
                case EChannelSelect.SetGray: newC.g = byte.MaxValue / 2; break;
            }

            switch (SwapBlueTo)
            {
                case EChannelSelect.R: newC.b = target.r; break;
                case EChannelSelect.G: newC.b = target.g; break;
                case EChannelSelect.B: newC.b = target.b; break;
                case EChannelSelect.A: newC.b = target.a; break;
                case EChannelSelect.SetBlack: newC.b = 0; break;
                case EChannelSelect.SetWhite: newC.b = byte.MaxValue; break;
                case EChannelSelect.SetGray: newC.b = byte.MaxValue / 2; break;
            }

            switch (SwapAlphaTo)
            {
                case EChannelSelect.R: newC.a = target.r; break;
                case EChannelSelect.G: newC.a = target.g; break;
                case EChannelSelect.B: newC.a = target.b; break;
                case EChannelSelect.A: newC.a = target.a; break;
                case EChannelSelect.SetBlack: newC.a = 0; break;
                case EChannelSelect.SetWhite: newC.a = byte.MaxValue; break;
                case EChannelSelect.SetGray: newC.a = byte.MaxValue / 2; break;
            }

            return newC;
        }


        [MenuItem("CONTEXT/Material/Convert Material Textures To PNGs")]
        private static void ConvertMaterialTexturesToPNGs(MenuCommand menuCommand)
        {
            Material targetMaterial = (Material)menuCommand.context;

            if (targetMaterial)
            {
                List<TextureInfo> textures = GetAllTexturesFromMaterial(targetMaterial);
                for (int i = 0; i < textures.Count; i++)
                {
                    if (textures[i].tex == null) continue;

                    string texPath = AssetDatabase.GetAssetPath(textures[i].tex);
                    FTex.FETextureExtension extension = FTex.FTex_Methods.GetFileExtension(texPath);

                    if (extension != FTex.FETextureExtension.PNG)
                    {
                        Texture2D duplicated = FTextureEditorToolsMethods.DuplicateAsPNG(textures[i].tex);
                        if ( duplicated != null)
                            targetMaterial.SetTexture(textures[i].nameInShader, duplicated);
                    }
                }

            }

            AssetDatabase.Refresh();
        }

        [MenuItem("CONTEXT/Material/Convert Material Textures To PNGs and Remove Sources")]
        private static void ConvertMaterialTexturesToPNGsAndRemove(MenuCommand menuCommand)
        {
            Material targetMaterial = (Material)menuCommand.context;

            if (targetMaterial)
            {

                if (EditorUtility.DisplayDialog("Warning", "This action will remove source texture files into system trash after converting them into pngs.", "Ok", "Cancel"))
                {
                    List<TextureInfo> textures = GetAllTexturesFromMaterial(targetMaterial);
                    for (int i = 0; i < textures.Count; i++)
                    {
                        if (textures[i].tex == null) continue;

                        string texPath = AssetDatabase.GetAssetPath(textures[i].tex);
                        FTex.FETextureExtension extension = FTex.FTex_Methods.GetFileExtension(texPath);

                        if (extension != FTex.FETextureExtension.PNG)
                        {
                            Texture2D duplicated = FTextureEditorToolsMethods.DuplicateAsPNG(textures[i].tex);
                            if (duplicated != null)
                            {
                                targetMaterial.SetTexture(textures[i].nameInShader, duplicated);
                                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(textures[i].tex));
                            }
                        }
                    }
                }

            }

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }


        public static List<TextureInfo> GetAllTexturesFromMaterial(Material mat)
        {
            List<TextureInfo> allTexture = new List<TextureInfo>();
            Shader shader = mat.shader;
            for (int i = 0; i < ShaderUtil.GetPropertyCount(shader); i++)
            {
                if (ShaderUtil.GetPropertyType(shader, i) == ShaderUtil.ShaderPropertyType.TexEnv)
                {
                    string name = ShaderUtil.GetPropertyName(shader, i);
                    Texture2D texture = (Texture2D)mat.GetTexture(name);
                    allTexture.Add(new TextureInfo(texture, name, i));
                }
            }

            return allTexture;
        }

        public struct TextureInfo
        {
            public Texture2D tex;
            public string nameInShader;
            public int idInShader;

            public TextureInfo(Texture2D tex, string nameInShader, int idInShader)
            {
                this.tex = tex;
                this.nameInShader = nameInShader;
                this.idInShader = idInShader;
            }
        }

    }


}