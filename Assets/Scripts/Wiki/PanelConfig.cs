using markdown;

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

        internal void LoadFonts()
        {

            FontManager.instance.addFont(Resources.Load<Font>(MaterialIcons), "Material Icons");
            FontManager.instance.addFont(Resources.Load<Font>(GalleryIcons), "GalleryIcons");
            FontManager.instance.addFont(Resources.Load<Font>(CupertinoIcons), "CupertinoIcons");

            FontManager.instance.addFont(Resources.Load<Font>(path: RegularFont), FontFamily, FontWeight.w400);
            FontManager.instance.addFont(Resources.Load<Font>(path: SemiboldFont), FontFamily, FontWeight.w600);
            FontManager.instance.addFont(Resources.Load<Font>(path: BoldFont), FontFamily, FontWeight.w700);
        }



        public long ToHex(Color32 c)
        {
            return (c.a << 24) | (c.r << 16) | (c.g << 8) | (c.b);
        }

        internal ThemeData GetAppTheme()
        {
            return new ThemeData(primaryColor: new Unity.UIWidgets.ui.Color(ToHex(primaryColor)), buttonTheme: GetButtonTheme());
        }


        private ButtonThemeData GetButtonTheme()
        {
            return new ButtonThemeData(colorScheme: ColorScheme.light()
                .copyWith(secondary: Colors.white));
        }

        internal MarkdownStyleSheet GetStyleSheet()
        {
            var t = MarkdownStyleSheet.fromTheme(
                new ThemeData(textTheme: new TextTheme(
                    display4: new TextStyle(fontSize: h1_Size, fontWeight: FontWeight.bold), //h1
                    display3: new TextStyle(fontSize: h2_Size, fontWeight: FontWeight.bold), //h2
                    display2: new TextStyle(fontSize: h3_Size, fontWeight: FontWeight.bold), //h3
                    display1: new TextStyle(fontSize: h4_Size, fontWeight: FontWeight.bold), //h4
                    headline: new TextStyle(fontSize: h5_Size, fontWeight: FontWeight.bold), //h5
                    title: new TextStyle(fontSize: h6_Size, fontWeight: FontWeight.bold), //h6
                    body1: new TextStyle(fontSize: 16.0f)
                //caption: new TextStyle(color: Colors.pink, fontSize: 8.0f),
                //button: new TextStyle(color: Colors.purple, fontSize: 8.0f),
                //overline: new TextStyle(color: Colors.red, fontSize: 8.0f)
                )));


            var body = new TextStyle(fontSize: body_Size);

            var tt = new MarkdownStyleSheet(
                new TextStyle(true, Colors.blue),
                body,
                new TextStyle(true, Colors.grey.shade700, fontSize: body.fontSize * .85f, fontFamily: "monospace"),
                new TextStyle(fontSize: h1_Size, fontWeight: FontWeight.bold), //h1
                new TextStyle(fontSize: h2_Size, fontWeight: FontWeight.bold), //h2
                new TextStyle(fontSize: h3_Size, fontWeight: FontWeight.bold), //h3
                new TextStyle(fontSize: h4_Size, fontWeight: FontWeight.bold), //h4
                new TextStyle(fontSize: h5_Size, fontWeight: FontWeight.bold), //h5
                new TextStyle(fontSize: h6_Size, fontWeight: FontWeight.bold), //h6
                new TextStyle(true, fontStyle: Unity.UIWidgets.ui.FontStyle.italic),
                new TextStyle(true, fontWeight: FontWeight.bold),
                body,
                body,
                blockSpacing,
                listIndent,
                blockQuotePadding,
                new BoxDecoration(null, null, null, BorderRadius.circular(borderRadius), null, null, null),
                8,
                new BoxDecoration(Colors.grey.shade100, null, null, BorderRadius.circular(borderRadius), null, null, null),
                new BoxDecoration(null, null, new Border(
                    new BorderSide(Colors.grey.shade300, 5)))
            );


            return tt;
        }
    }
}
