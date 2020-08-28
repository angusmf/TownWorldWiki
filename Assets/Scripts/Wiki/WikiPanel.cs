using markdown;

using Unity.UIWidgets.engine;
using Unity.UIWidgets.material;
using Unity.UIWidgets.widgets;

namespace TownWorldWiki
{
    public class WikiPanel : UIWidgetsPanel
    {

        private Markdown markdown;
        private AppBar appBar;
        private string markdownData1;

        private string title = "Markdown Demo";

        private void buildMarkdown()
        {
            markdown = new Markdown(null, markdownData1,
                      syntaxHighlighter: new DartSyntaxHighlighter(SyntaxHighlighterStyle.lightThemeStyle()),
                      onTapLink: handleTap
                      );
        }

        private void handleTap(string url)
        {
            appBar = new AppBar(title: new Unity.UIWidgets.widgets.Text("Some NEW!!! title"));
            markdownData1 = new MarkdownData2().Data;
            recreateWidget();
        }

        protected override void OnEnable()
        {
            // if you want to use your own font or font icons.   
            // FontManager.instance.addFont(Resources.Load<Font>(path: "path to your font"), "font family name");

            // load custom font with weight & style. The font weight & style corresponds to fontWeight, fontStyle of 
            // a TextStyle object
            // FontManager.instance.addFont(Resources.Load<Font>(path: "path to your font"), "Roboto", FontWeight.w500, 
            //    FontStyle.italic);

            // add material icons, familyName must be "Material Icons"
            // FontManager.instance.addFont(Resources.Load<Font>(path: "path to material icons"), "Material Icons");

            base.OnEnable();
        }

        protected override Widget createWidget()
        {
            if (appBar == null) appBar = new AppBar(title: new Unity.UIWidgets.widgets.Text("Some title"));
            if (string.IsNullOrEmpty(markdownData1)) markdownData1 = new MarkdownData().Data;
            buildMarkdown();

            return new MaterialApp(
                title: title,
                home: new Scaffold(
                    appBar: appBar,
                    body: markdown
                )
            );
        }
    }
}
