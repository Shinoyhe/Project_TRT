using FIMSpace.FEditor;
using UnityEditor;
using UnityEngine;

namespace FIMSpace.FTextureTools
{
    public class FNormalToolWindow : FTextureProcessWindow
    {

        enum ENormalMode
        {
            BasicPictureToNormal,
            OpenGLDirectXSwapConversion,
            InvertHeight,
            BlurNormalMap,
        }


        ENormalMode Mode = ENormalMode.BasicPictureToNormal;


        private float Blend = 1f;
        private float Boost = 0f;
        private float Range = 0f;
        private float RangeSens = 1.2f;
        private float Smoothing = 0f;
        private bool BlurAfterNormalizing = false;

        public static void Init()
        {
            FNormalToolWindow window = (FNormalToolWindow)GetWindow( typeof( FNormalToolWindow ) );
            window.titleContent = new GUIContent( "Normal Tool", FTextureToolsGUIUtilities.FindIcon( "SPR_NormalGen" ), "Normal Maps related operations" );
            window.previewScale = FEPreview.m_1x1;
            window.drawPreviewScale = true;

            window.previewSize = 82;
            window.position = new Rect( 340, 50, 520, 564 );
            window.Show();
            called = true;
        }


        protected override void OnGUICustom()
        {
            EditorGUI.BeginChangeCheck();

            Mode = (ENormalMode)EditorGUILayout.EnumPopup( "Mode:", Mode );
            GUILayout.Space( 8 );

            if( Mode == ENormalMode.BasicPictureToNormal )
            {
                GUI.backgroundColor = new Color( 0.6f, 1f, 0.7f );

                Blend = EditorGUILayout.Slider( new GUIContent( "Blend" ), Blend, -1f, 1f );
                Boost = EditorGUILayout.Slider( new GUIContent( "Boost" ), Boost, 0f, 20f );
                GUI.backgroundColor = Color.white;
                GUILayout.Space( 8 );

                EditorGUILayout.BeginHorizontal();
                Range = EditorGUILayout.Slider( new GUIContent( "Range" ), Range, 0f, 1f );
                if( Range <= 0f ) GUI.enabled = false;
                EditorGUILayout.LabelField( "Boost:", GUILayout.Width( 58 ) );
                RangeSens = GUILayout.HorizontalSlider( RangeSens, 1f, 2f, GUILayout.MaxWidth( 120 ) );
                GUI.enabled = true;
                GUILayout.Space( 8 );
                EditorGUILayout.EndHorizontal();
                GUILayout.Space( 2 );
                Smoothing = EditorGUILayout.Slider( new GUIContent( "Smoothing" ), Smoothing, 0f, 1f );
                BlurAfterNormalizing = EditorGUILayout.Toggle( new GUIContent( "Post Blur:", "Blur after or before normalizing, giving different results" ), BlurAfterNormalizing );

            }
            else if( Mode == ENormalMode.OpenGLDirectXSwapConversion )
            {
                EditorGUILayout.HelpBox( "If you use this on OpenGL normal map, it will be converted to DirectX coordinates (left-handed)\nIf you use this on DirectX normal map, it will be converted to OpenGL coordinates (right-handed)", MessageType.Info );
            }
            else if( Mode == ENormalMode.InvertHeight )
            {
                EditorGUILayout.HelpBox( "Just inverting normal map height direction", MessageType.Info );
            }
            else if( Mode == ENormalMode.BlurNormalMap )
            {
                EditorGUILayout.HelpBox( "Blurring base image and then applying normalization", MessageType.Info );
                Blend = EditorGUILayout.Slider( new GUIContent( "Blend" ), Blend, 0f, 1f );
                Smoothing = EditorGUILayout.Slider( new GUIContent( "Smoothing" ), Smoothing, 0f, 1f );
            }

            if( EditorGUI.EndChangeCheck() )
                somethingChanged = true;
            else
                somethingChanged = false;
        }


        protected override void ProcessTexture( Texture2D source, Texture2D target, bool preview = true )
        {
            if( !preview ) EditorUtility.DisplayProgressBar( "Normalizing Texture...", "Preparing... ", 2f / 5f );

            if( Mode == ENormalMode.BasicPictureToNormal )
            {
                Color[] sourcePixels = source.GetPixels();
                Color32[] newPix32 = new Color32[sourcePixels.Length];
                Color[] newPixels;

                if( BlurAfterNormalizing == false )
                {
                    if( Smoothing > 0 )
                    {
                        newPix32 = FTextureEditorToolsMethods.ApplyGaussianBlur( source.GetPixels32(), source.width, source.height, Smoothing, true );
                        for( int i = 0; i < newPix32.Length; i++ ) sourcePixels[i] = (Color)newPix32[i];
                    }
                }

                int smoothingPixels = Mathf.RoundToInt( Mathf.Lerp( 1, ( source.width + source.height ) / 128f, Range ) );

                if( Range == 0 )
                {
                    newPixels = GenerateNormalMap( sourcePixels, source.width, source.height );
                }
                else
                {
                    newPixels = GenerateNormalMap( sourcePixels, source.width, source.height, smoothingPixels, RangeSens );
                }

                // Finalizing changes
                if( !preview ) EditorUtility.DisplayProgressBar( "Normalizing Texture...", "Applying Normalizing to Texture... ", 3.85f / 5f );

                for( int i = 0; i < newPixels.Length; i++ ) newPix32[i] = (Color32)newPixels[i];

                if( BlurAfterNormalizing )
                {
                    if( Smoothing > 0 ) newPix32 = FTextureEditorToolsMethods.ApplyGaussianBlur( newPix32, source.width, source.height, Smoothing, true );
                }

                target.SetPixels32( newPix32 );
            }
            else if( Mode == ENormalMode.OpenGLDirectXSwapConversion )
            {
                Color[] sourcePixels = source.GetPixels();
                Color32[] newPix32 = ConvertDirectXToOpenGL( sourcePixels, source.width, source.height );
                target.SetPixels32( newPix32 );
            }
            else if( Mode == ENormalMode.InvertHeight )
            {
                Color[] sourcePixels = source.GetPixels();
                Color32[] newPix32 = InvertNormal( sourcePixels, source.width, source.height );
                target.SetPixels32( newPix32 );
            }
            else if( Mode == ENormalMode.BlurNormalMap )
            {
                Color32[] sourcePixels = source.GetPixels32();
                Color32[] newPix32 = Smoothing == 0 ? sourcePixels : FTextureEditorToolsMethods.ApplyGaussianBlur( sourcePixels, source.width, source.height, Smoothing, true );
                newPix32 = ReGenerateNormalMap( sourcePixels, newPix32, source.width, source.height, Blend );
                target.SetPixels32( newPix32 );
            }

            target.Apply( false, false );

            if( !preview ) EditorUtility.ClearProgressBar();
        }


        protected override void OnAfterProcessingImage( TextureImporter importer )
        {
            importer.textureType = TextureImporterType.NormalMap;
        }

        public Color[] GenerateNormalMap( Color[] imagePixels, int width, int height )
        {
            Color[] normalMapPixels = new Color[imagePixels.Length];
            float blend = Blend * ( 1f + Boost );

            for( int y = 0; y < height; y++ )
            {
                for( int x = 0; x < width; x++ )
                {
                    // Get surrounding pixel intensities
                    float left = GetIntensity( imagePixels, width, height, x - 1, y ) * blend;
                    float right = GetIntensity( imagePixels, width, height, x + 1, y ) * blend;
                    float top = GetIntensity( imagePixels, width, height, x, y - 1 ) * blend;
                    float bottom = GetIntensity( imagePixels, width, height, x, y + 1 ) * blend;

                    // Calculate direction gradients
                    float dx = right - left;
                    float dy = bottom - top;

                    Vector3 normal = new Vector3( -dx, -dy, 1.0f ).normalized;

                    Color normalColor = new Color(
                        normal.x * 0.5f + 0.5f,
                        normal.y * 0.5f + 0.5f,
                        normal.z * 0.5f + 0.5f
                    );

                    normalMapPixels[y * width + x] = normalColor;
                }
            }

            return normalMapPixels;
        }

        public Color32[] ReGenerateNormalMap( Color32[] prePixels, Color32[] imagePixels, int width, int height, float blend = 1f )
        {
            Color32[] normalMapPixels = new Color32[imagePixels.Length];

            for( int y = 0; y < height; y++ )
            {
                for( int x = 0; x < width; x++ )
                {
                    int pix = GetPX( x, y, new Vector2( width, height ) );

                    float preX = NormalToIntensity32( prePixels[pix].r ); 
                    float preY = NormalToIntensity32( prePixels[pix].g );

                    float dx = Mathf.Lerp( preX, NormalToIntensity32( imagePixels[pix].r ), blend );
                    float dy = Mathf.Lerp( preY, NormalToIntensity32( imagePixels[pix].g ), blend );

                    Vector3 normal = new Vector3( dx, dy, 1.0f ).normalized;

                    normalMapPixels[y * width + x] = new Color32(
                        (byte)( ( normal.x * 0.5f + 0.5f ) * 255.0f ),
                        (byte)( ( normal.y * 0.5f + 0.5f ) * 255.0f ),
                        (byte)( ( normal.z * 0.5f + 0.5f ) * 255.0f ),
                        255
                    );
                }
            }

            return normalMapPixels;
        }

        private float ReGetIntensity( Color32[] pixels, int width, int height, int x, int y )
        {
            Color32 pixel = pixels[GetPXLoop( x, y, new Vector2( width, height ) )];
            float r = NormalToIntensity32( pixel.r );
            float g = NormalToIntensity32( pixel.g );
            return ( r + g ) / 2f;
        }

        float NormalToIntensity32( byte c ) => ( ( c / 255.0f ) - 0.5f ) * 2f;

        public Color32[] GenerateNormalMap( Color32[] imagePixels, int width, int height, float blend, float boost )
        {
            Color32[] normalMapPixels = new Color32[imagePixels.Length];
            float adjustedBlend = blend * ( 1f + boost );

            for( int y = 0; y < height; y++ )
            {
                for( int x = 0; x < width; x++ )
                {
                    float left = GetIntensity( imagePixels, width, height, x - 1, y ) * adjustedBlend;
                    float right = GetIntensity( imagePixels, width, height, x + 1, y ) * adjustedBlend;
                    float top = GetIntensity( imagePixels, width, height, x, y - 1 ) * adjustedBlend;
                    float bottom = GetIntensity( imagePixels, width, height, x, y + 1 ) * adjustedBlend;

                    float dx = right - left;
                    float dy = bottom - top;

                    Vector3 normal = new Vector3( -dx, -dy, 1.0f ).normalized;

                    normalMapPixels[y * width + x] = new Color32(
                        (byte)( ( normal.x * 0.5f + 0.5f ) * 255.0f ),
                        (byte)( ( normal.y * 0.5f + 0.5f ) * 255.0f ),
                        (byte)( ( normal.z * 0.5f + 0.5f ) * 255.0f ),
                        255
                    );
                }
            }

            return normalMapPixels;
        }

        private float GetIntensity( Color32[] pixels, int width, int height, int x, int y )
        {
            Color32 pixel = pixels[GetPXLoop( x, y, new Vector2( width, height ) )];
            return ( pixel.r + pixel.g + pixel.b ) / 3.0f / 255.0f;
        }

        public Color[] GenerateNormalMap( Color[] imagePixels, int width, int height, int range, float rangeSensitivity )
        {
            Color[] normalMapPixels = new Color[imagePixels.Length];
            float blend = Blend * ( 1f + Boost );
            float maxDistance = range * rangeSensitivity + 0.1f;

            for( int y = 0; y < height; y++ )
            {
                for( int x = 0; x < width; x++ )
                {
                    float dx = 0f, dy = 0f;

                    for( int offsetY = -range; offsetY <= range; offsetY++ )
                    {
                        for( int offsetX = -range; offsetX <= range; offsetX++ )
                        {
                            float distance = Vector2.Distance( Vector2.zero, new Vector2( offsetX, offsetY ) );
                            float distanceBlend = Mathf.Min( 1f, distance / maxDistance );
                            distanceBlend *= distanceBlend;
                            distanceBlend = 1f - distanceBlend;

                            // Get intensities of neighboring pixels
                            float neighborIntensity = GetIntensity( imagePixels, width, height, x + offsetX, y + offsetY ) * blend * distanceBlend;
                            float centerIntensity = GetIntensity( imagePixels, width, height, x, y ) * blend * distanceBlend;

                            // Compute gradient contributions
                            dx += ( neighborIntensity - centerIntensity ) * offsetX;
                            dy += ( neighborIntensity - centerIntensity ) * offsetY;
                        }
                    }

                    // Normalize direction gradients
                    float magnitude = Mathf.Sqrt( dx * dx + dy * dy + 1.0f );
                    Vector3 normal = new Vector3( -dx / magnitude, -dy / magnitude, 1.0f / magnitude );

                    Color normalColor = new Color(
                        normal.x * 0.5f + 0.5f,
                        normal.y * 0.5f + 0.5f,
                        normal.z * 0.5f + 0.5f
                    );

                    normalMapPixels[y * width + x] = normalColor;
                }
            }

            return normalMapPixels;
        }


        private float GetIntensity( Color[] pixels, int width, int height, int x, int y )
        {
            Color pixel = pixels[GetPXLoop( x, y, new Vector2( width, height ) )];
            return ( pixel.r + pixel.g + pixel.b ) / 3.0f; // Average intensity
        }


        public Color32[] ConvertDirectXToOpenGL( Color[] baseNormalMap, int width, int height )
        {
            Color[] coverterNormalMap = new Color[baseNormalMap.Length];

            for( int y = 0; y < height; y++ )
            {
                for( int x = 0; x < width; x++ )
                {
                    Color baseNormalColor = baseNormalMap[y * width + x];

                    Color convertedCol = new Color(
                        baseNormalColor.r,
                        1f - baseNormalColor.g,       // Invert
                        baseNormalColor.b,
                        baseNormalColor.a
                    );

                    coverterNormalMap[y * width + x] = convertedCol;
                }
            }

            Color32[] pix = new Color32[coverterNormalMap.Length];

            for( int i = 0; i < pix.Length; i++ ) pix[i] = (Color32)coverterNormalMap[i];

            return pix;
        }


        public Color32[] InvertNormal( Color[] baseNormalMap, int width, int height )
        {
            Color[] coverterNormalMap = new Color[baseNormalMap.Length];

            for( int y = 0; y < height; y++ )
            {
                for( int x = 0; x < width; x++ )
                {
                    Color baseNormalColor = baseNormalMap[y * width + x];

                    Color convertedCol = new Color(
                        1f - baseNormalColor.r,
                        baseNormalColor.g,
                        baseNormalColor.b,
                        baseNormalColor.a
                    );

                    coverterNormalMap[y * width + x] = convertedCol;
                }
            }

            Color32[] pix = new Color32[coverterNormalMap.Length];

            for( int i = 0; i < pix.Length; i++ ) pix[i] = (Color32)coverterNormalMap[i];

            return pix;
        }

    }

}