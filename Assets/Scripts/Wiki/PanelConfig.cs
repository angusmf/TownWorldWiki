using Unity.UIWidgets.ui;
using UnityEngine;

namespace UIWidgetsWiki
{
    public class PanelConfig : MonoBehaviour
    {
        public string Title = "Town World Wiki";
        public string RootName = "Town_World";

        public string MaterialIcons = "fonts/MaterialIcons-Regular";
        public string GalleryIcons = "fonts/GalleryIcons";
        public string CupertinoIcons = "fonts/CupertinoIcons";

        public string FontFamily = ".SF Pro Text";

        public string RegularFont = "fonts/SF-Pro-Text-Regular";
        public string SemiboldFont = "fonts/SF-Pro-Text-Semibold";
        public string BoldFont = "fonts/SF-Pro-Text-Bold";

        private string format = "{0}/{1}";
  

        internal void LoadFonts()
        {

            FontManager.instance.addFont(Resources.Load<Font>(MaterialIcons), "Material Icons");
            FontManager.instance.addFont(Resources.Load<Font>(GalleryIcons), "GalleryIcons");
            FontManager.instance.addFont(Resources.Load<Font>(CupertinoIcons), "CupertinoIcons");

            FontManager.instance.addFont(Resources.Load<Font>(path: RegularFont), FontFamily, FontWeight.w400);
            FontManager.instance.addFont(Resources.Load<Font>(path: SemiboldFont), FontFamily, FontWeight.w600);
            FontManager.instance.addFont(Resources.Load<Font>(path: BoldFont), FontFamily, FontWeight.w700);

        }
    }
}
