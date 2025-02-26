using FIMSpace.FEditor;
using UnityEditor;

namespace FIMSpace.FTextureTools
{
    public static class FChannelTools_MenuItems
    {
        [MenuItem("Assets/FImpossible Creations/Texture Tools/Channelled Generator", priority = -99)]
        public static void ChannelledGenerator()
        {
            FChannelledGenerator.Init();
        }


        [MenuItem("Assets/FImpossible Creations/Texture Tools/Channel Insert", priority = 3)]
        public static void ChannelInserter()
        {
            FChannelInserter.Init();
        }


        [MenuItem("Assets/FImpossible Creations/Texture Tools/Extract RGBA Channels", priority = 4)]
        public static void ChannelsExtract()
        {
            FChannelsExtractor.Init();
        }
    }
}