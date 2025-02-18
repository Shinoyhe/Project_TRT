using UnityEditor;
namespace FIMSpace.FEditor
{
    public static class FTextureUtils_MenuItems
    {
        [MenuItem("Assets/FImpossible Creations/Texture Tools/Convert any to PNG", priority = 2)]
        public static void ToPNGConversion()
        {
            FTextureQuickConverter.Init();
        }
    }
}
