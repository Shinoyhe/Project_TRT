using FIMSpace.FTex;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FIMSpace.FEditor
{
    public static class FTextureEditorToolsMethods
    {
        #region Acquirung texture for editing


        public static TextureImporter GetTextureAsset( Texture2D source )
        {
            string sPath = AssetDatabase.GetAssetPath( source );

            TextureImporter sourceTex = (TextureImporter)AssetImporter.GetAtPath( sPath );
            TextureImporter outTex = sourceTex;

            if( sourceTex != null && outTex != null )
            {
                return outTex;
            }
            else
            {
                Debug.LogError( "[Fimpo Image Tools] No Texture!" );
                return null;
            }
        }

        public static TextureInfo GetTextureInfo( TextureImporter sourceTex, Texture2D source )
        {
            return new TextureInfo( sourceTex, source );
        }

        public static TextureInfo StartEditingTextureAsset( TextureImporter sourceTex, Texture2D source, TextureInfo src, bool saveAndReimport = true )
        {
            try
            {
                string sPath = AssetDatabase.GetAssetPath( source );
                FETextureExtension extension = FTex_Methods.GetFileExtension( sPath );

                if( extension == FETextureExtension.UNSUPPORTED )
                {
                    Debug.LogError( "[Fimpo Image Tools] Not supported format to scale texture, Fimpo Image Tools supports only .JPG .PNG .TGA .EXR files!" );
                    return src;
                }

                // Making source texture be open for GetPixels method
                // Setting output texture params be able for pixels replacement
                sourceTex.isReadable = true;
                sourceTex.textureType = TextureImporterType.Default;
                sourceTex.textureCompression = TextureImporterCompression.Uncompressed;

                TextureImporterPlatformSettings sourceSets = sourceTex.GetPlatformTextureSettings( "Standalone" );
                sourceSets.format = TextureImporterFormat.RGBA32;
                sourceSets.overridden = true;
                sourceTex.SetPlatformTextureSettings( sourceSets );

                // Refreshing assets for our changes
                if( saveAndReimport ) sourceTex.SaveAndReimport();
            }
            catch( System.Exception exc )
            {
                src.RestoreOn( sourceTex, source, false );
                Debug.LogError( "[Fimpo Image Tools] Something went wrong when modifying image file! " + exc );
            }

            return src;
        }


        public static TextureInfo StartEditingTextureAsset( Texture2D source, bool saveAndReimport = true )
        {
            TextureImporter sourceTex;
            TextureInfo src;

            try
            {
                sourceTex = GetTextureAsset( source );
                string path = AssetDatabase.GetAssetPath( source );
                string directory = System.IO.Path.GetDirectoryName( path );
                src = GetTextureInfo( sourceTex, source );
            }
            catch( System.Exception )
            {
                throw;
            }

            try
            {
                string sPath = AssetDatabase.GetAssetPath( source );
                FETextureExtension extension = FTex_Methods.GetFileExtension( sPath );

                if( extension == FETextureExtension.UNSUPPORTED )
                {
                    Debug.LogError( "[Fimpo Image Tools] Not supported format to scale texture, Fimpo Image Tools supports only .JPG .PNG .TGA .EXR files!" );
                    return new TextureInfo();
                }

                // Making source texture be open for GetPixels method
                // Setting output texture params be able for pixels replacement
                sourceTex.isReadable = true;
                sourceTex.textureType = TextureImporterType.Default;
                sourceTex.textureCompression = TextureImporterCompression.Uncompressed;

                TextureImporterPlatformSettings sourceSets = sourceTex.GetPlatformTextureSettings( "Standalone" );
                sourceSets.format = TextureImporterFormat.RGBA32;
                sourceSets.overridden = true;
                sourceTex.SetPlatformTextureSettings( sourceSets );

                // Refreshing assets for our changes
                if( saveAndReimport ) sourceTex.SaveAndReimport();
            }
            catch( System.Exception exc )
            {
                src.RestoreOn( sourceTex, source, false );
                Debug.LogError( "[Fimpo Image Tools] Something went wrong when modifying image file! " + exc );
            }

            return src;
        }


        public static void EndEditingTextureAsset( Color32[] newPixels, TextureInfo info, TextureImporter sourceTex, Texture2D output, bool saveAndReimport = true )
        {
            string oPath = AssetDatabase.GetAssetPath( output );

            if( newPixels != null )
            {
                output.SetPixels32( newPixels );

                FETextureExtension extension = FTex_Methods.GetFileExtension( oPath );
                byte[] fileBytes = null;
                switch( extension )
                {
                    case FETextureExtension.JPG: fileBytes = output.EncodeToJPG( 95 ); break;
                    case FETextureExtension.PNG: fileBytes = output.EncodeToPNG(); break;
                    case FETextureExtension.TGA: fileBytes = FTex_AdditionalEncoders.EncodeToTGA( output ); break;
                    case FETextureExtension.TIFF: fileBytes = FTex_AdditionalEncoders.EncodeToTIFF( output ); break;
                    case FETextureExtension.EXR: fileBytes = output.EncodeToEXR(); break;
                }

                // Applying changes to file
                if( fileBytes != null ) File.WriteAllBytes( oPath, fileBytes );
            }

            info.RestoreOn( sourceTex, output );

            if( saveAndReimport )
            {
                // Refreshing assets in editor window
                sourceTex.SaveAndReimport();

                AssetDatabase.ImportAsset( oPath );
                AssetDatabase.Refresh();
            }
        }

        public static void EndEditingTextureAsset( Texture2D source, TextureInfo backup, bool saveAndReimport = true )
        {
            TextureImporter sourceTex;
            TextureInfo src;

            try
            {
                sourceTex = GetTextureAsset( source );
                string path = AssetDatabase.GetAssetPath( source );
                string directory = System.IO.Path.GetDirectoryName( path );
                src = GetTextureInfo( sourceTex, source );
            }
            catch( System.Exception )
            {
                throw;
            }

            backup.RestoreOn( sourceTex, source );

            if( saveAndReimport )
            {
                string oPath = AssetDatabase.GetAssetPath( source );

                // Refreshing assets in editor window
                sourceTex.SaveAndReimport();

                AssetDatabase.ImportAsset( oPath );
                AssetDatabase.Refresh();
            }
        }


        public static Texture2D DuplicateAsset( Texture2D source )
        {
            string path = AssetDatabase.GetAssetPath( source );
            string newPath = Path.GetDirectoryName( path ) + "/" + Path.GetFileNameWithoutExtension( path ) + "-Backup" + Path.GetExtension( path );

            Texture2D copied = null;
            if( AssetDatabase.CopyAsset( path, newPath ) )
                copied = (Texture2D)AssetDatabase.LoadAssetAtPath( newPath, typeof( Texture2D ) );

            return copied;
        }


        public struct TextureInfo
        {
            public bool wasReadable;
            public TextureImporterType sType;
            public TextureFormat outFormat;
            public TextureImporterCompression comp;
            public TextureImporterPlatformSettings sourceSets;
            public bool doMips;
            public int anisoLevel;
            public FilterMode filter;
            public TextureImporterShape shape;

            public TextureInfo( TextureImporter sourceTex, Texture2D source )
            {
                wasReadable = sourceTex.isReadable;
                sType = sourceTex.textureType;
                outFormat = source.format;
                comp = sourceTex.textureCompression;
                sourceSets = sourceTex.GetPlatformTextureSettings( "Standalone" );
                doMips = source.mipmapCount > 1;
                anisoLevel = source.anisoLevel;
                filter = source.filterMode;
                shape = sourceTex.textureShape;
            }

            public void RestoreOn( TextureImporter sourceTex, Texture2D source, bool apply = true )
            {
                if( apply )
                {
                    source.Apply( doMips, !wasReadable );
                }

                sourceTex.mipmapEnabled = doMips;
                sourceTex.isReadable = wasReadable;
                sourceTex.textureType = sType;
                sourceTex.textureCompression = comp;
                sourceTex.SetPlatformTextureSettings( sourceSets );
                sourceTex.anisoLevel = anisoLevel;
                sourceTex.filterMode = filter;
                sourceTex.textureShape = shape;
            }
        }


        public static Texture2D DuplicateAsPNG( Texture2D source, string postFix = "-ToPNG", bool saveAndReimport = true, bool addNewAlphaChannelIfNeeded = false )
        {
            TextureImporter imp = GetTextureAsset( source );
            string path = AssetDatabase.GetAssetPath( source );
            string directory = System.IO.Path.GetDirectoryName( path );

            TextureInfo info = GetTextureInfo( imp, source );
            TextureInfo ainfo = info;
            StartEditingTextureAsset( imp, source, ainfo );

            // Generate png out of source texture pixels and data
            Texture2D newPng = new Texture2D( source.width, source.height, TextureFormat.RGBA32, source.mipmapCount > 1 );
            Color32[] px = source.GetPixels32();
            if( addNewAlphaChannelIfNeeded ) for( int i = 0; i < px.Length; i++ ) px[i].a = byte.MaxValue;
            newPng.SetPixels32( px );


            newPng.Apply( source.mipmapCount > 1, false );

            // Save new png texture asset in directory
            string nPath = directory + "/" + source.name + postFix + ".png";
            File.WriteAllBytes( nPath, newPng.EncodeToPNG() );
            AssetDatabase.Refresh( ImportAssetOptions.Default );
            AssetDatabase.ImportAsset( nPath, ImportAssetOptions.Default );

            // Set texture asset same settings like source texture asset
            TextureImporter pimp = (TextureImporter)AssetImporter.GetAtPath( nPath );
            if( pimp != null )
            {
                TextureInfo pinfo = info;
                if( addNewAlphaChannelIfNeeded )
                {
                    pinfo.outFormat = TextureFormat.RGBA32;
                }

                pinfo.RestoreOn( pimp, newPng, false );
                AssetDatabase.Refresh( ImportAssetOptions.Default );
                AssetDatabase.ImportAsset( nPath, ImportAssetOptions.Default );
                if( saveAndReimport ) pimp.SaveAndReimport();

                // Finalize editings
                //EndEditingTextureAsset(null, info, pimp, newPng);
            }

            EndEditingTextureAsset( null, info, imp, source );

            newPng = AssetDatabase.LoadAssetAtPath<Texture2D>( nPath );

            return newPng;
        }

        public static Texture2D GenerateCopyWithNewPixels( Texture2D source, Color[] newPixels, string suffix = "-ToPNG", bool saveAndReimport = true )
        {
            TextureImporter imp = GetTextureAsset( source );
            string path = AssetDatabase.GetAssetPath( source );
            string directory = System.IO.Path.GetDirectoryName( path );

            TextureInfo info = GetTextureInfo( imp, source );
            TextureInfo ainfo = info;
            StartEditingTextureAsset( imp, source, ainfo );

            // Generate png out of source texture pixels and data
            Texture2D newPng = new Texture2D( source.width, source.height, TextureFormat.RGBA32, source.mipmapCount > 1 );
            newPng.SetPixels( newPixels );

            newPng.Apply( source.mipmapCount > 1, false );

            // Save new png texture asset in directory
            string nPath = directory + "/" + source.name + suffix + ".png";
            File.WriteAllBytes( nPath, newPng.EncodeToPNG() );
            AssetDatabase.Refresh( ImportAssetOptions.Default );
            AssetDatabase.ImportAsset( nPath, ImportAssetOptions.Default );

            // Set texture asset same settings like source texture asset
            TextureImporter pimp = (TextureImporter)AssetImporter.GetAtPath( nPath );
            if( pimp != null )
            {
                TextureInfo pinfo = info;
                pinfo.outFormat = TextureFormat.RGBA32;

                pinfo.RestoreOn( pimp, newPng, false );
                AssetDatabase.Refresh( ImportAssetOptions.Default );
                AssetDatabase.ImportAsset( nPath, ImportAssetOptions.Default );
                if( saveAndReimport ) pimp.SaveAndReimport();
            }

            EndEditingTextureAsset( null, info, imp, source );

            newPng = AssetDatabase.LoadAssetAtPath<Texture2D>( nPath );

            return newPng;
        }


        #endregion



        public static void ScaleTextureFile( Texture2D source, Texture2D output, Vector2 dimensions, int quality = 4 )
        {
            if( output )
                if( output.width == (int)dimensions.x && output.height == (int)dimensions.y )
                {
                    Debug.Log( "[Fimpo Image Tools] " + source.name + " have already dimensions " + dimensions.x + " x " + dimensions.y );
                    return;
                }

            // Getting textures
            string sPath = AssetDatabase.GetAssetPath( source );
            string oPath = AssetDatabase.GetAssetPath( output );

            TextureImporter sourceTex = (TextureImporter)AssetImporter.GetAtPath( sPath );
            TextureImporter outTex = sourceTex;

            if( source != output ) outTex = (TextureImporter)AssetImporter.GetAtPath( oPath );

            if( sourceTex != null && outTex != null )
            {
                // Remember some important texture asset parameters to restore them after changes
                bool swasReadable = sourceTex.isReadable;
                bool owasReadable = outTex.isReadable;
                TextureImporterType oType = outTex.textureType;
                TextureImporterType sType = sourceTex.textureType;
                TextureFormat outFormat = output.format;
                TextureImporterCompression comp = outTex.textureCompression;
                TextureImporterPlatformSettings preSets = outTex.GetPlatformTextureSettings( "Standalone" );
                TextureImporterPlatformSettings sourceSets = outTex.GetPlatformTextureSettings( "Standalone" );

                try
                {
                    FETextureExtension extension = FTex_Methods.GetFileExtension( oPath );

                    if( extension == FETextureExtension.UNSUPPORTED )
                    {
                        Debug.LogError( "[Fimpo Image Tools] Not supported format to scale texture, Fimpo Image Tools supports only .JPG .PNG .TGA .EXR files!" );
                        return;
                    }

                    // Remember some important texture asset parameters to restore them after changes
                    bool doMips = output.mipmapCount > 1;

                    // Making source texture be open for GetPixels method
                    sourceTex.isReadable = true;
                    sourceTex.textureType = TextureImporterType.Default;

                    // Setting output texture params be able for pixels replacement
                    outTex.isReadable = true;
                    outTex.textureType = TextureImporterType.Default;
                    outTex.textureCompression = TextureImporterCompression.Uncompressed;
                    sourceSets.format = TextureImporterFormat.RGBA32;
                    outTex.SetPlatformTextureSettings( sourceSets );

                    // Refreshing assets for our changes
                    sourceTex.SaveAndReimport();
                    outTex.SaveAndReimport();

                    // Rescaling image
                    Color32[] newPixels = FTex_ScaleLanczos.ScaleTexture( source.GetPixels32(), source.width, source.height, (int)dimensions.x, (int)dimensions.y, quality );

                    //int startBytes = File.ReadAllBytes(oPath).Length;

                    // Applying to texture asset
                    output.Reinitialize( (int)dimensions.x, (int)dimensions.y );
                    output.SetPixels32( newPixels );

                    byte[] fileBytes = null;
                    switch( extension )
                    {
                        case FETextureExtension.JPG: fileBytes = output.EncodeToJPG( 95 ); break;
                        case FETextureExtension.PNG: fileBytes = output.EncodeToPNG(); break;
                        case FETextureExtension.TGA: fileBytes = FTex_AdditionalEncoders.EncodeToTGA( output ); break;
                        case FETextureExtension.TIFF: fileBytes = FTex_AdditionalEncoders.EncodeToTIFF( output ); break;
                        case FETextureExtension.EXR: fileBytes = output.EncodeToEXR(); break;
                    }

                    // Applying changes to file
                    if( fileBytes != null )
                    {
                        File.WriteAllBytes( oPath, fileBytes );
                    }

                    output.Apply( doMips, !owasReadable );

                    // Restoring parameters
                    if( fileBytes != null )
                        if( !Mathf.IsPowerOfTwo( output.width ) || !Mathf.IsPowerOfTwo( output.height ) )
                        {
                            Debug.Log( "<b>[Fimpo Image Tools]</b> " + output.name + " resized to " + output.width + "x" + output.height + " So there is no power of 2, that means texture can't be compressed to take less memory in build. If it was intended ignore this message. (changing texture settings 'Power of two' under '/advanced/' to 'None')" );
                            outTex.npotScale = TextureImporterNPOTScale.None;
                        }


                    sourceTex.isReadable = swasReadable;
                    sourceTex.textureType = sType;
                    sourceTex.SetPlatformTextureSettings( sourceSets );

                    outTex.isReadable = owasReadable;
                    outTex.textureType = oType;
                    outTex.textureCompression = comp;
                    outTex.SetPlatformTextureSettings( preSets );

                    // Refreshing assets in editor window
                    sourceTex.SaveAndReimport();
                    outTex.SaveAndReimport();
                }
                catch( System.Exception exc )
                {
                    sourceTex.isReadable = swasReadable;
                    sourceTex.textureType = sType;
                    sourceTex.SetPlatformTextureSettings( sourceSets );

                    outTex.isReadable = owasReadable;
                    outTex.textureType = oType;
                    outTex.textureCompression = comp;
                    outTex.SetPlatformTextureSettings( preSets );

                    AssetDatabase.ImportAsset( sPath );
                    if( source != output ) AssetDatabase.ImportAsset( oPath );
                    AssetDatabase.Refresh();
                    Debug.LogError( "[Fimpo Image Tools] Something went wrong when rescalling image file! " + exc );
                }
            }
            else
            {
                Debug.LogError( "[Fimpo Image Tools] No Texture to Rescale!" );
            }
        }


        public static Texture2D GenerateScaledTexture2DReference( Texture2D source, Vector2 dimensions, int quality = 4, bool doMipMaps = false, bool makeNoLongerReadable = false )
        {
            // Getting texture
            string sPath = AssetDatabase.GetAssetPath( source );
            TextureImporter sourceTex = (TextureImporter)AssetImporter.GetAtPath( sPath );
            Texture2D generatedTex = null;

            if( sourceTex != null )
            {
                // Remember some important texture asset parameters to restore them after changes
                bool swasReadable = sourceTex.isReadable;
                TextureImporterType sType = sourceTex.textureType;

                try
                {
                    // Making source texture be open for GetPixels method
                    sourceTex.isReadable = true;
                    sourceTex.textureType = TextureImporterType.Default;

                    // Refreshing assets for our changes
                    sourceTex.SaveAndReimport();

                    // Rescaling image
                    Color32[] newPixels = FTex_ScaleLanczos.ScaleTexture( source.GetPixels32(), source.width, source.height, (int)dimensions.x, (int)dimensions.y, quality );

                    generatedTex = new Texture2D( (int)dimensions.x, (int)dimensions.y );
                    // Applying to texture asset
                    generatedTex.SetPixels32( newPixels );
                    generatedTex.Apply( doMipMaps, makeNoLongerReadable );

                    sourceTex.textureType = sType;
                    sourceTex.isReadable = swasReadable;

                    // Refreshing assets in editor window
                    sourceTex.SaveAndReimport();
                }
                catch( System.Exception exc )
                {
                    sourceTex.isReadable = swasReadable;
                    AssetDatabase.ImportAsset( sPath );
                    AssetDatabase.Refresh();
                    Debug.LogError( "[Fimpo Image Tools] Something went wrong when rescalling image file! " + exc );
                }
            }
            else
            {
                Debug.LogError( "[Fimpo Image Tools] No Texture to Rescale!" );
            }

            return generatedTex;
        }


        /// <summary> Get suffix text for filename to prevent overwriting file </summary>
        public static string GetFilenameSuffix()
        {
            var t = System.DateTime.Now;
            string totalSeconds = Mathf.RoundToInt( (float)t.TimeOfDay.TotalSeconds ).ToString();
            return t.Date.DayOfYear.ToString() + "-" + totalSeconds;
        }

        public static Color32[] ApplyGaussianBlur( Color32[] imagePixels, int width, int height, float smoothing, bool loop = true )
        {
            float sigma = Mathf.Lerp( 1, ( width + height ) / 70f, smoothing );
            int kernel = Mathf.RoundToInt( Mathf.Lerp( 3, ( width + height ) / 12f, smoothing ) );
            return ApplyGaussianBlur( imagePixels, width, height, sigma, kernel, loop );
        }


        public static Color32[] ApplyGaussianBlur( Color32[] imagePixels, int width, int height, float sigma = 1.0f, int kernelSize = 3, bool loop = true )
        {
            Color32[] tempPixels = new Color32[imagePixels.Length];
            Color32[] blurredPixels = new Color32[imagePixels.Length];

            // Generate 1D Gaussian kernel
            float[] kernel = GenerateGaussianKernel1D( kernelSize, sigma );
            int halfKernel = kernelSize / 2;

            // Horizontal pass
            for( int y = 0; y < height; y++ )
            {
                for( int x = 0; x < width; x++ )
                {
                    Vector4 colorSum = Vector4.zero;
                    float weightSum = 0f;

                    for( int k = -halfKernel; k <= halfKernel; k++ )
                    {
                        int sampleX = x + k;

                        if( loop ) { if( sampleX < 0 ) sampleX += width; else if( sampleX >= height ) sampleX -= width; }
                        else
                        { sampleX = Mathf.Clamp( sampleX, 0, height - 1 ); }

                        int idx = k + halfKernel;

                        if( idx < 0 ) continue; else if( idx >= kernel.Length ) continue;

                        float weight = kernel[idx];

                        Color32 sample = imagePixels[y * width + sampleX];
                        colorSum += new Vector4( sample.r, sample.g, sample.b, sample.a ) * weight;
                        weightSum += weight;
                    }

                    tempPixels[y * width + x] = new Color32(
                        (byte)( colorSum.x / weightSum ),
                        (byte)( colorSum.y / weightSum ),
                        (byte)( colorSum.z / weightSum ),
                        (byte)( colorSum.w / weightSum )
                    );
                }
            }

            // Vertical pass
            for( int y = 0; y < height; y++ )
            {
                for( int x = 0; x < width; x++ )
                {
                    Vector4 colorSum = Vector4.zero;
                    float weightSum = 0f;

                    for( int k = -halfKernel; k <= halfKernel; k++ )
                    {
                        int sampleY = y + k;

                        if( loop ) { if( sampleY < 0 ) sampleY += width; else if( sampleY >= width ) sampleY -= width; }
                        else
                        { sampleY = Mathf.Clamp( sampleY, 0, width - 1 ); }

                        int idx = k + halfKernel;

                        if( idx < 0 ) continue; else if( idx >= kernel.Length ) continue;

                        float weight = kernel[idx];

                        Color32 sample = tempPixels[sampleY * width + x];
                        colorSum += new Vector4( sample.r, sample.g, sample.b, sample.a ) * weight;
                        weightSum += weight;
                    }

                    blurredPixels[y * width + x] = new Color32(
                        (byte)( colorSum.x / weightSum ),
                        (byte)( colorSum.y / weightSum ),
                        (byte)( colorSum.z / weightSum ),
                        (byte)( colorSum.w / weightSum )
                    );
                }
            }

            return blurredPixels;
        }

        private static float[] GenerateGaussianKernel1D( int size, float sigma )
        {
            float[] kernel = new float[size];
            int halfSize = size / 2;

            float twoSigmaSquare = 2 * sigma * sigma;
            float sigmaRoot = Mathf.Sqrt( twoSigmaSquare * Mathf.PI );
            float total = 0f;

            for( int i = -halfSize; i <= halfSize; i++ )
            {
                float distance = i * i;
                float value = Mathf.Exp( -distance / twoSigmaSquare ) / sigmaRoot;

                int idx = i + halfSize;
                if( idx < 0 ) continue; else if( idx >= kernel.Length ) continue;
                kernel[idx] = value;
                total += value;
            }

            // Normalize the kernel
            for( int i = 0; i < size; i++ )
            {
                kernel[i] /= total;
            }

            return kernel;
        }


        public static Color32 BlendNormals( Color32 normal1, Color32 normal2, float blend = 1f )
        {
            Vector3 n1 = DecodeNormal( normal1 );
            Vector3 n2 = DecodeNormal( normal2 );

            Vector3 blendedNormal = Vector3.Normalize( Vector3.Lerp( n1, n1 + n2, blend ) );

            return EncodeNormal( blendedNormal );
        }

        private static Vector3 DecodeNormal( Color32 normal )
        {
            Vector3 decodedNormal = new Vector3(
                normal.r / 255.0f * 2.0f - 1.0f,
                normal.g / 255.0f * 2.0f - 1.0f,
                normal.b / 255.0f * 2.0f - 1.0f
            );
            return decodedNormal.normalized;
        }

        private static Color32 EncodeNormal( Vector3 normal )
        {
            return new Color32(
                (byte)( ( normal.x * 0.5f + 0.5f ) * 255.0f ),
                (byte)( ( normal.y * 0.5f + 0.5f ) * 255.0f ),
                (byte)( ( normal.z * 0.5f + 0.5f ) * 255.0f ),
                255
            );
        }


    }
}