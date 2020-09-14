using markdown;

using UnityEngine;

using Unity.UIWidgets.engine;
using Unity.UIWidgets.material;
using Unity.UIWidgets.widgets;
using Unity.UIWidgets.painting;

using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine.Networking;

using Cysharp.Threading.Tasks;


namespace UIWidgetsWiki
{
    public class WikiView2 : UIWidgetsPanel
    {
        string Title = "Town World Wiki";

        private const string MARKDOWN__FILE_EXTENSION = ".markdown";
        private const string NO_DATA = "no data";

        private Stack<string> m_navHistory = new Stack<string>();

        private string m_appBarTitle = string.Empty;
        private string markdownData1 = NO_DATA;
        private Uri m_currentPath;
        private MarkdownStyleSheet m_markdownStyleSheet;
        private ThemeData m_appTheme;
        private PanelConfig m_config;
        private HashSet<string> m_PathExists = new HashSet<string>();

        protected override void Awake()
        {

            if (m_config == null) m_config = GetComponent<PanelConfig>();
            Title = m_config?.Title;
            m_currentPath = new Uri(new Uri("https://twzwiki.angusmf.com"), "/Town_World" + MARKDOWN__FILE_EXTENSION);

            m_markdownStyleSheet = MarkdownStyleSheet.fromTheme(
                new ThemeData(textTheme: new TextTheme(
                    display4: new TextStyle(fontSize: 40.0f, fontWeight: Unity.UIWidgets.ui.FontWeight.bold), //h1
                    display3: new TextStyle(fontSize: 36.0f, fontWeight: Unity.UIWidgets.ui.FontWeight.bold), //h2
                    display2: new TextStyle(fontSize: 32.0f, fontWeight: Unity.UIWidgets.ui.FontWeight.bold), //h3
                    display1: new TextStyle(fontSize: 28.0f, fontWeight: Unity.UIWidgets.ui.FontWeight.bold), //h4
                    headline: new TextStyle(fontSize: 24.0f, fontWeight: Unity.UIWidgets.ui.FontWeight.bold), //h5
                    title: new TextStyle(fontSize: 20.0f, fontWeight: Unity.UIWidgets.ui.FontWeight.bold), //h6
                    body1: new TextStyle(fontSize: 16.0f)
        //            caption: new TextStyle(color: Colors.pink, fontSize: 8.0f),
        //            button: new TextStyle(color: Colors.purple, fontSize: 8.0f),
        //            overline: new TextStyle(color: Colors.red, fontSize: 8.0f)
            )));

            //m_appTheme = new ThemeData
        }

        private async void HandleTap(string url)
        {
            if (!await HandleInternalLink(url))
                HandleExternalLink(url);
        }

        private async UniTask<bool> HandleInternalLink(string url)
        {
            if (!url.StartsWith("./") && !url.StartsWith("..")) return false;

            if (!await NavigateTo(url))
            {
                Debug.LogWarningFormat("HandleInternalLink couldn't find markdown at path {0}", url);
                //TODO user notifcation of missing file
                //TODO user add new file
            }

            return true;
        }

        async UniTask<bool> ReloadPage(string path)
        {
            if (await LoadPageAsync(path))
            {
                recreateWidget();
                return true;
            }
            else
            {
                return false;
            }
        }

        private Uri loadPath;

        private async UniTask<bool> LoadPageAsync(string path)
        {
            if (!Application.isPlaying) return false;

            loadPath = new Uri(m_currentPath, path);
            markdownData1 = (await UnityWebRequest.Get(loadPath).SendWebRequest()).downloadHandler.text;

            if (markdownData1 != null)
            {
                if (!m_PathExists.Contains(loadPath.AbsolutePath)) m_PathExists.Add(loadPath.AbsolutePath);
                m_currentPath = loadPath;
                SetAppBarTitle(loadPath.AbsolutePath);
                return true;
            }
            else
            {
                Debug.LogErrorFormat("LoadPage couldn't find file at path {0}.", path);
                return false;
            }
        }

        private void SetAppBarTitle(string loadPath)
        {
            m_appBarTitle = loadPath;
            if (m_appBarTitle.StartsWith(Path.DirectorySeparatorChar.ToString()) ||
                m_appBarTitle.StartsWith(Path.AltDirectorySeparatorChar.ToString()))
            {
                m_appBarTitle = m_appBarTitle.Remove(0, 1);
            }
            m_appBarTitle = m_appBarTitle.Replace(MARKDOWN__FILE_EXTENSION, string.Empty)
                .Replace(Path.DirectorySeparatorChar.ToString(), " : ")
                .Replace(Path.AltDirectorySeparatorChar.ToString(), " : ");
        }

        private void HandleExternalLink(string url)
        {
            Debug.LogErrorFormat("url = {0}", url);
            throw new NotImplementedException();
        }

        protected override void OnEnable()
        {
            if (m_config == null)
                m_config = GetComponent<PanelConfig>();
            m_config?.LoadFonts();

            base.OnEnable();
        }

        private bool PushCurrentPageToHistory()
        {
            if (m_PathExists.Contains(lastPath.AbsolutePath))
            {
                m_navHistory.Push(lastPath.AbsolutePath);
                return true;
            }
            else
            {
                Debug.LogWarningFormat("Could not push non-existent page \"{0}\" to history.",
                    lastPath);
                return false;
            }
        }

        private Uri lastPath;

        private async UniTask<bool> NavigateTo(string path)
        {
            lastPath = m_currentPath;

            if (await ReloadPage(path))
            {
                return PushCurrentPageToHistory();
            }
            return false;
        }

        async void Back()
        {
            if (m_navHistory.Count > 0)
            {
                await ReloadPage(m_navHistory.Pop());
            }
        }

        async void Home()
        {
            await NavigateTo(new Uri(m_currentPath, "/Town_World" + MARKDOWN__FILE_EXTENSION).ToString());
        }

        protected override Widget createWidget()
        {
            if (!Application.isPlaying) return null;

            if (markdownData1 == NO_DATA)
            {
                NavigateTo(m_currentPath.ToString());
                return null;
            }

            return new MaterialApp(
                title: Title,
                home: new Scaffold(

                    //app bar
                    appBar: new AppBar(title: new Text(m_appBarTitle.Replace('_', ' '))),

                    //body
                    body: new Markdown(data: markdownData1,
                    syntaxHighlighter: new DartSyntaxHighlighter(SyntaxHighlighterStyle.lightThemeStyle()),
                    onTapLink: HandleTap,
                    imageDirectory: "https://twzwiki.angusmf.com",
                    styleSheet: m_markdownStyleSheet),

                    //footer
                    persistentFooterButtons: new List<Widget>()
                    {
                        RaisedButton.icon(
                            icon: new Icon(Icons.home, size: 18.0f, color: Colors.white),
                            label: new Text("HOME", style: new TextStyle(true, color: Colors.white)),
                            onPressed: Home),

                        RaisedButton.icon(
                            icon: new Icon(Icons.arrow_back, size: 18.0f, color: Colors.white),
                            label: new Text("BACK", style: new TextStyle(true, color: Colors.white)),
                            onPressed: Back),


                        RaisedButton.icon(
                            icon: new Icon(Icons.power, size: 18.0f, color: Colors.white),
                            label: new Text("QUIT", style: new TextStyle(true, color: Colors.white)),
                            onPressed: () => Application.Quit()),

                    }));
        }
    }
}
