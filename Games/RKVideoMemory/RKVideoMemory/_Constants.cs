using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RKVideoMemory
{
    internal static class Constants
    {
        public const string GAME_SCENE_NAME = "VideoMemoryScene";

        public const string GFX_LAYER_BACKGROUND = "Background";
        public const int GFX_LAYER_BACKGROUND_ORDERID = -100;

        public static readonly string[] SUPPORTED_IMAGE_FORMATS = new string[] { ".jpg", ".jpeg", ".png", ".bmp" };
        public const string TITLE_IMAGE_NAME = "_Title";
    }
}
