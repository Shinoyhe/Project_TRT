using FIMSpace.FTextureTools;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FIMSpace.FEditor
{
    public class FResizeWindow : EditorWindow
    {
        public Texture2D SourceTexture;
        public Texture2D OutputTexture;
        public Vector2 NewDimensions;

        public static void Init()
        {
            FResizeWindow window = (FResizeWindow)GetWindow( typeof( FResizeWindow ) );

            window.titleContent = new GUIContent( "Resize Texture", FTextureToolsGUIUtilities.FindIcon( "SPR_Scale" ) );
            window.minSize = new Vector2( 250f, 200f );
            window.maxSize = new Vector2( 250f, 242f );
            window.position = new Rect( 200, 100, 250, 236f );

            window.Show();
        }


        void OnGUI()
        {
            if( SourceTexture == null )
            {
                if( Selection.activeObject )
                {
                    string path = AssetDatabase.GetAssetPath( Selection.activeObject );
                    SourceTexture = (Texture2D)AssetDatabase.LoadAssetAtPath( path, typeof( Texture2D ) );

                    if( SourceTexture != null )
                    {
                        NewDimensions = new Vector2( SourceTexture.width, SourceTexture.height );
                    }
                }
            }

            if( SourceTexture == null )
            {
                EditorGUILayout.HelpBox( "You must select texture file in procets assets window!", MessageType.Warning );
                return;
            }

            SourceTexture = (Texture2D)EditorGUILayout.ObjectField( "To Rescale", SourceTexture, typeof( Texture2D ), false );

            if( OutputTexture == null ) OutputTexture = SourceTexture;

            if( OutputTexture == SourceTexture )
            {
                OutputTexture = (Texture2D)EditorGUILayout.ObjectField( "Output Texture (Replace?)", OutputTexture, typeof( Texture2D ), false, new GUILayoutOption[1] { GUILayout.MaxHeight( 28 ) } );

                if( GUILayout.Button( "Work on duplicate (backup)" ) )
                {
                    string path = AssetDatabase.GetAssetPath( SourceTexture );
                    string newPath = Path.GetDirectoryName( path ) + "/" + Path.GetFileNameWithoutExtension( path ) + "-Rescalled" + Path.GetExtension( path );

                    if( AssetDatabase.CopyAsset( path, newPath ) )
                        OutputTexture = (Texture2D)AssetDatabase.LoadAssetAtPath( newPath, typeof( Texture2D ) );
                }

                minSize = new Vector2( 250f, 236f ); maxSize = new Vector2(250f, 250f);
            }
            else
            {
                OutputTexture = (Texture2D)EditorGUILayout.ObjectField( "Output Texture", OutputTexture, typeof( Texture2D ), false );
                minSize = new Vector2( 250f, 298f ); maxSize = new Vector2(250f, 320f);
            }

            FTextureToolsGUIUtilities.DrawUILine( Color.white * 0.35f, 2, 5 );

            if( SourceTexture )
            {
                EditorGUILayout.LabelField( "Texture To Be Resized: " + SourceTexture.name );
                EditorGUILayout.LabelField( SourceTexture.width + "x" + SourceTexture.height );

                if( OutputTexture != SourceTexture )
                    if( OutputTexture != null )
                    {
                        GUILayout.Space( 4 );
                        EditorGUILayout.LabelField( "Output Texture: " + OutputTexture.name );
                        EditorGUILayout.LabelField( "Current Dimensions: " + OutputTexture.width + "x" + OutputTexture.height );
                    }

                FTextureToolsGUIUtilities.DrawUILine( Color.white * 0.35f, 2, 5 );
            }

            NewDimensions = EditorGUILayout.Vector2Field( "New Dimensions", NewDimensions );
            if( NewDimensions.x < 1 ) NewDimensions.x = 1;
            if( NewDimensions.y < 1 ) NewDimensions.y = 1;
            if( NewDimensions.x > 10000 ) NewDimensions.x = 10000;
            if( NewDimensions.y > 10000 ) NewDimensions.y = 10000;

            if( OutputTexture == SourceTexture )
            {
                if( GUILayout.Button( "Scale File (Replace File)" ) )
                {
                    FTextureEditorToolsMethods.ScaleTextureFile( SourceTexture, OutputTexture, NewDimensions );
                }
            }
            else
            {
                if( GUILayout.Button( "Scale File" ) )
                {
                    FTextureEditorToolsMethods.ScaleTextureFile( SourceTexture, OutputTexture, NewDimensions );
                }
            }
        }

    }
}