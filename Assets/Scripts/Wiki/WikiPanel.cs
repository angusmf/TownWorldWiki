using markdown;
using System.Collections.Generic;



using Unity.UIWidgets.engine;
using Unity.UIWidgets.material;
using Unity.UIWidgets.widgets;
using System;
using Unity.UIWidgets.rendering;
using Unity.UIWidgets.painting;

namespace TownWorldWiki
{
    public class WikiPanel : UIWidgetsPanel
    {

        private Stack<string> m_navHistory = new Stack<string>();

        private string m_appBarTitle;
        private string markdownData1;

        private string title = "Town World Wiki";
        private string m_wikiRoot = "./Town_World";
        private string m_currURL;

        protected override void Awake()
        {
            if (string.IsNullOrWhiteSpace(m_appBarTitle)) m_appBarTitle = "Some title";
            if (string.IsNullOrEmpty(markdownData1)) markdownData1 = new MarkdownData().Data;
        }

        private void HandleTap(string url)
        {
            if (!HandleInternalLink(url))
                HandleExternalLink(url);  
        }

        private bool HandleInternalLink(string url)
        {
            if (!url.StartsWith(m_wikiRoot)) return false;


            m_navHistory.Push(url);
            m_appBarTitle = url == m_wikiRoot ? "HOME" : "Some NEW!!! title";
            markdownData1 = (url == m_currURL || url == m_wikiRoot) ? new MarkdownData().Data : new MarkdownData2().Data;

            recreateWidget();
            m_currURL = url;

            return true;
        }

        private void HandleExternalLink(string url)
        {
            throw new NotImplementedException();
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
            return new MaterialApp(
                title: title,
                home: new Scaffold(
                    appBar: new AppBar(title: new Text(m_appBarTitle)),

                    body: new Markdown(data: markdownData1,
                      syntaxHighlighter: new DartSyntaxHighlighter(SyntaxHighlighterStyle.lightThemeStyle()),
                      onTapLink: HandleTap),

                    persistentFooterButtons: new List<Widget>()
                    { 
                        RaisedButton.icon(
                            icon: new Icon(Icons.home, size: 18.0f, color: new Unity.UIWidgets.ui.Color(0xFFFFFFFF)),
                            label: new Text("HOME BUTTON", style: new TextStyle(true, new Unity.UIWidgets.ui.Color(0xFF000000))),
                            onPressed: () => {
                                // Perform some action
                                HandleInternalLink(m_wikiRoot);
                            }),
                        RaisedButton.icon(
                            icon: new Icon(Icons.arrow_back, size: 18.0f, color: new Unity.UIWidgets.ui.Color(0xFFFFFFFF)),
                            label: new Text("BACK BUTTON", style: new TextStyle(true, new Unity.UIWidgets.ui.Color(0xFF000000))),
                            onPressed: () => {
                                // Perform some action
                                HandleInternalLink(m_navHistory.Pop());
                            }),
                    }));
        }
    }
}
