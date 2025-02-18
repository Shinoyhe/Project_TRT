using FIMSpace.FTex;
using UnityEditor;
using UnityEngine;

namespace FIMSpace.FEditor
{
    public static class FResizeTools_MenuItems
    {
        [MenuItem("Assets/FImpossible Creations/Texture Tools/Change Texture Resolution", priority = 0)]
        public static void ResizeTexture()
        {
            FResizeWindow.Init();
        }

        [MenuItem("Assets/FImpossible Creations/Texture Tools/Quick Resize", priority = 1)]
        public static void QuickResizeTexture()
        {
            FQuickResizeWindow.Init();
        }


        [MenuItem("Assets/FImpossible Creations/Texture Tools/Resize to nearest power of 2", priority = 14)]
        public static void ResizeToPowerOf2()
        {
            try
            {
                for (int i = 0; i < Selection.objects.Length; i++)
                {
                    Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GetAssetPath(Selection.objects[i]));

                    EditorUtility.DisplayProgressBar("Scaling textures...", "Scaling texture " + texture.name, (float)i / (float)Selection.objects.Length);

                    if (texture != null)
                        FTextureEditorToolsMethods.ScaleTextureFile(texture, texture, new Vector2(FTex_Methods.FindNearestPowOf2(texture.width), FTex_Methods.FindNearestPowOf2(texture.height)));
                }

                EditorUtility.ClearProgressBar();
            }
            catch (System.Exception exc)
            {
                EditorUtility.ClearProgressBar();
                Debug.LogError("[Fimpo Image Tools Something went wrong when scaling textures! " + exc);
            }
        }

        [MenuItem("Assets/FImpossible Creations/Texture Tools/Resize to power of 2 Lower", priority = 27)]
        public static void ResizeToPowerOf2Lower()
        {
            try
            {
                for (int i = 0; i < Selection.objects.Length; i++)
                {
                    Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GetAssetPath(Selection.objects[i]));

                    EditorUtility.DisplayProgressBar("Scaling textures...", "Scaling texture " + texture.name, (float)i / (float)Selection.objects.Length);

                    if (texture != null)
                        FTextureEditorToolsMethods.ScaleTextureFile(texture, texture, new Vector2(FTex_Methods.FindLowerPowOf2(texture.width), FTex_Methods.FindLowerPowOf2(texture.height)));
                }

                EditorUtility.ClearProgressBar();
            }
            catch (System.Exception exc)
            {
                EditorUtility.ClearProgressBar();
                Debug.LogError("[Fimpo Image Tools] Something went wrong when scaling textures! " + exc);
            }
        }

        [MenuItem("Assets/FImpossible Creations/Texture Tools/Resize to power of 2 Higher", priority = 26)]
        public static void ResizeToPowerOf2Higher()
        {
            try
            {
                for (int i = 0; i < Selection.objects.Length; i++)
                {
                    Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GetAssetPath(Selection.objects[i]));

                    EditorUtility.DisplayProgressBar("Scaling textures...", "Scaling texture " + texture.name, (float)i / (float)Selection.objects.Length);

                    if (texture != null)
                        FTextureEditorToolsMethods.ScaleTextureFile(texture, texture, new Vector2(FTex_Methods.FindHigherPowOf2(texture.width), FTex_Methods.FindHigherPowOf2(texture.height)));
                }

                EditorUtility.ClearProgressBar();
            }
            catch (System.Exception exc)
            {
                EditorUtility.ClearProgressBar();
                Debug.LogError("[Fimpo Image Tools] Something went wrong when scaling textures! " + exc);
            }

        }


        [MenuItem("Assets/FImpossible Creations/Texture Tools/Change Texture Resolution", true)]
        [MenuItem("Assets/FImpossible Creations/Texture Tools/Quick Resize", true)]
        public static bool CheckResizeTextureAllSelected()
        {
            if (!Selection.activeObject) return false;

            for (int i = 0; i < Selection.objects.Length; i++) // We need just one file to be texture to return true
            {
                AssetImporter tex = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(Selection.objects[i]));
                if (tex as TextureImporter) return true;
            }

            return false;
        }
    }
}