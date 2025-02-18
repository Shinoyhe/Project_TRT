using FIMSpace.FTextureTools;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

namespace FIMSpace.FEditor
{
    public class FChannelInserter : EditorWindow
    {
        public Texture2D From;
        public enum EChannelSelect { R, G, B, A, RGB }
        public EChannelSelect ChannelFrom = EChannelSelect.R;
        public EChannelSelect ApplyTo = EChannelSelect.A;

        public static void Init()
        {
            FChannelInserter window = (FChannelInserter)GetWindow(typeof(FChannelInserter));

            window.minSize = new Vector2(270f, 225f);

            window.titleContent = new GUIContent("Channel Insert", FTextureToolsGUIUtilities.FindIcon("SPR_Channels"));

            window.position = new Rect(200, 100, 270, 225);

            window.Show();
        }


        void OnGUI()
        {
            Texture2D texture = null;

            if (Selection.objects.Length > 0)
            {
                for (int i = 0; i < Selection.objects.Length; i++)
                {
                    texture = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GetAssetPath(Selection.objects[i]));
                    if (texture != null) break;
                }
            }

            if (texture == null)
            {
                EditorGUILayout.HelpBox("You must select at least one texture file!", MessageType.Warning);
                return;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Texture to edit: " + texture.name);

            if (texture)
            {
                GUILayout.FlexibleSpace();
                var texRect = GUILayoutUtility.GetRect(40, 40);
                GUI.DrawTexture(texRect, texture, ScaleMode.ScaleToFit);
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(6);

            EditorGUIUtility.labelWidth = 190;
            From = (Texture2D)EditorGUILayout.ObjectField(From == null ? "Get Channel From:" : "Get Channel From: " + From.name, From, typeof(Texture2D), false);
            EditorGUIUtility.labelWidth = 0;

            GUILayout.Space(6);
            ChannelFrom = (EChannelSelect)EditorGUILayout.EnumPopup("Get channel:", ChannelFrom);
            ApplyTo = (EChannelSelect)EditorGUILayout.EnumPopup("Apply it to:", ApplyTo);


            if (From != null && texture != null)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUIUtility.labelWidth = 50;
                EditorGUILayout.LabelField("Will Paste this: ");
                var rect = GUILayoutUtility.GetRect(40, 40);
                EditorGUI.DrawPreviewTexture(rect, From, null, ScaleMode.ScaleToFit, 1f, 0, EChannelToWriteChannel(ChannelFrom));
                EditorGUILayout.LabelField("Replacing this: ");
                rect = GUILayoutUtility.GetRect(40, 40);
                EditorGUI.DrawPreviewTexture(rect, texture, null, ScaleMode.ScaleToFit, 1f, 0, EChannelToWriteChannel(ApplyTo));
                EditorGUILayout.EndHorizontal();
                EditorGUIUtility.labelWidth = 0;
            }


            FTextureToolsGUIUtilities.DrawUILine(Color.white * 0.35f, 2, 5);

            if (From != null)
            {
                bool dimensionsDiffer = false;
                Texture2D tfrom = From;

                if (From.width != texture.width || From.height != texture.height)
                {
                    dimensionsDiffer = true;
                    EditorGUILayout.HelpBox("Dimensions of the textures are not equal. Algorithm will generate copy of texture and scale it to fit target texture.", MessageType.Info);
                }

                if (GUILayout.Button((ApplyTo == EChannelSelect.A ? "(Requires A channel) " : "") + "Insert '" + ChannelFrom + "' to '" + ApplyTo + "' channel of " + texture.name))
                {
                    if (dimensionsDiffer) FTextureEditorToolsMethods.ScaleTextureFile(From, tfrom, new Vector2(texture.width, texture.height));

                    ProcessChanneling(tfrom, texture);
                }

                if (GUILayout.Button("Duplicate (png) and Insert Channel (" + texture.name + ")"))
                {
                    if (dimensionsDiffer)
                        tfrom = FTextureEditorToolsMethods.GenerateScaledTexture2DReference(From, new Vector2(texture.width, texture.height), 4, true);

                    Texture2D duplicated = FTextureEditorToolsMethods.DuplicateAsPNG(texture, "-PNG", true, ApplyTo == EChannelSelect.A);
                    if (duplicated != null) ProcessChanneling(tfrom, duplicated);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("You must select 'From' texture", MessageType.Info);
            }

        }


        UnityEngine.Rendering.ColorWriteMask EChannelToWriteChannel(EChannelSelect channel)
        {
            switch (channel)
            {
                case EChannelSelect.R: return UnityEngine.Rendering.ColorWriteMask.Red;
                case EChannelSelect.G: return UnityEngine.Rendering.ColorWriteMask.Green;
                case EChannelSelect.B: return UnityEngine.Rendering.ColorWriteMask.Blue;
                case EChannelSelect.A: return UnityEngine.Rendering.ColorWriteMask.Alpha;
            }

            return UnityEngine.Rendering.ColorWriteMask.All;
        }

        public void ProcessChanneling(Texture2D source, Texture2D target)
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

                for (int i = 0; i < newPixels.Length; i++)
                    newPixels[i] = SwapChannel(srcPixels[i], newPixels[i], ChannelFrom, ApplyTo);

                FTextureEditorToolsMethods.EndEditingTextureAsset(srcPixels, srcInfo, srcImporter, source);
                FTextureEditorToolsMethods.EndEditingTextureAsset(newPixels, tgtInfo, tgtImporter, target);

                EditorUtility.ClearProgressBar();
            }
            catch (System.Exception exc)
            {
                srcInfo.RestoreOn(srcImporter, source, false);
                tgtInfo.RestoreOn(tgtImporter, source, false);

                EditorUtility.ClearProgressBar();
                UnityEngine.Debug.LogError("[Fimpo Image Tools Something went wrong when channeling textures! " + exc);
            }
        }

        public Color32 SwapChannel(Color32 source, Color32 target, EChannelSelect from, EChannelSelect to)
        {
            Color32 newC = target;
            byte tgt = target.r;

            switch (from)
            {
                case EChannelSelect.R: tgt = source.r; break;
                case EChannelSelect.G: tgt = source.g; break;
                case EChannelSelect.B: tgt = source.b; break;
                case EChannelSelect.A: tgt = source.a; break;
                case EChannelSelect.RGB: tgt = (byte)(Mathf.Min((source.r + source.g + source.b) / 3, byte.MaxValue)); break;
            }

            switch (to)
            {
                case EChannelSelect.R: newC.r = tgt; break;
                case EChannelSelect.G: newC.g = tgt; break;
                case EChannelSelect.B: newC.b = tgt; break;
                case EChannelSelect.A: newC.a = tgt; break;
                case EChannelSelect.RGB: newC.r = tgt; newC.g = tgt; newC.b = tgt; break;
            }

            return newC;
        }

    }
}