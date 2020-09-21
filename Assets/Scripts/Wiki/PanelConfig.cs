using markdown;
using System;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
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

        public string WebRoot = "https://twzwiki.angusmf.com";

        public int body_Size = 16;
        public int h1_Size = 40;
        public int h2_Size = 36;
        public int h3_Size = 32;
        public int h4_Size = 28;
        public int h5_Size = 24;
        public int h6_Size = 20;

        public int blockSpacing = 8;
        public int listIndent = 32;
        public int blockQuotePadding = 8;
        public int borderRadius = 2;

        public Color32 primaryColor;
        public Color32 linkColor;
        public Color32 bodyColor;
        public Color32 h1Color;
        public Color32 h2Color;
        public Color32 h3Color;
        public Color32 h4Color;
        public Color32 h5Color;
        public Color32 h6Color;
        public bool darkButtonColorSchem;
        public Color32 mainButtonColor;
        public Color32 buttonTextColor = new Color32(255, 255, 255, 255);
        public Color32 codeColor;
        public Color32 blockQuoteColor;
        public Color32 codeBlockColor;
        public Color32 hRuleColor;


        internal void LoadFonts()
        {

            FontManager.instance.addFont(Resources.Load<Font>(MaterialIcons), "Material Icons");
            FontManager.instance.addFont(Resources.Load<Font>(GalleryIcons), "GalleryIcons");
            FontManager.instance.addFont(Resources.Load<Font>(CupertinoIcons), "CupertinoIcons");

            FontManager.instance.addFont(Resources.Load<Font>(path: RegularFont), FontFamily, FontWeight.w400);
            FontManager.instance.addFont(Resources.Load<Font>(path: SemiboldFont), FontFamily, FontWeight.w600);
            FontManager.instance.addFont(Resources.Load<Font>(path: BoldFont), FontFamily, FontWeight.w700);
        }

        public Unity.UIWidgets.ui.Color Color32ToUIWidgetsColor(Color32 c)
        {
            return new Unity.UIWidgets.ui.Color((c.a < 1 ? c.a : 255 << 24) | (c.r << 16) | (c.g << 8) | (c.b));
        }

        private Unity.UIWidgets.ui.Color GetColor(Color32 c, Unity.UIWidgets.ui.Color defaultColor = null)
        {
            return (c.r > 0 || c.b > 0 || c.g > 0) && c.a > 0 ?
                new Unity.UIWidgets.ui.Color((c.a < 1 ? c.a : 255 << 24) | (c.r << 16) | (c.g << 8) | (c.b)) : defaultColor;
        }

        internal ThemeData GetAppTheme()
        {
            return new ThemeData(primaryColor: GetColor(primaryColor), buttonTheme: GetButtonTheme());
        }


        private ButtonThemeData GetButtonTheme()
        {
            return new ButtonThemeData(colorScheme: (darkButtonColorSchem ? ColorScheme.dark() : ColorScheme.light())
                .copyWith(primary: GetColor(mainButtonColor), secondary: Colors.white));
        }

        internal MarkdownStyleSheet GetStyleSheet()
        {
        
            var body = new TextStyle(true, GetColor(bodyColor), fontSize: body_Size);

            var tt = new MarkdownStyleSheet(
                new TextStyle(true, GetColor(linkColor, Colors.blue)),
                body,
                new TextStyle(true, GetColor(codeColor, Colors.grey.shade700), fontSize: body.fontSize * .85f, fontFamily: "monospace"),
                new TextStyle(true, GetColor(h1Color), fontSize: h1_Size, fontWeight: FontWeight.bold), //h1
                new TextStyle(true, GetColor(h2Color), fontSize: h2_Size, fontWeight: FontWeight.bold), //h2
                new TextStyle(true, GetColor(h3Color), fontSize: h3_Size, fontWeight: FontWeight.bold), //h3
                new TextStyle(true, GetColor(h4Color), fontSize: h4_Size, fontWeight: FontWeight.bold), //h4
                new TextStyle(true, GetColor(h5Color), fontSize: h5_Size, fontWeight: FontWeight.bold), //h5
                new TextStyle(true, GetColor(h6Color), fontSize: h6_Size, fontWeight: FontWeight.bold), //h6
                new TextStyle(true, fontStyle: Unity.UIWidgets.ui.FontStyle.italic),
                new TextStyle(true, fontWeight: FontWeight.bold),
                body,
                body,
                blockSpacing,
                listIndent,
                blockQuotePadding,
                new BoxDecoration(GetColor(blockQuoteColor), null, null, BorderRadius.circular(borderRadius), null, null, null),
                8,
                new BoxDecoration(GetColor(codeBlockColor, Colors.grey.shade100), null, null, BorderRadius.circular(borderRadius), null, null, null),
                new BoxDecoration(null, null, new Border(
                    new BorderSide(GetColor(hRuleColor, Colors.grey.shade300), 5)))
            );


            return tt;
        }


    }
}
