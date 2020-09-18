using markdown;

using UnityEngine;

using Unity.UIWidgets.engine;
using Unity.UIWidgets.material;
using Unity.UIWidgets.widgets;


using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine.Networking;


namespace UIWidgetsWiki
{
    public class WikiView2 : UIWidgetsPanel
    {

        private const string MARKDOWN__FILE_EXTENSION = ".markdown";
        private const string NO_DATA = "no data";

        private Stack<string> m_navHistory = new Stack<string>();

        private string m_appBarTitle = string.Empty;
        private string markdownData1 = NO_DATA;
        private Uri m_currentPath;
        private MarkdownStyleSheet m_markdownStyleSheet;
        private ThemeData m_appTheme;
        private PanelConfig m_config;
        private Dictionary<string, string> m_pageCache = new Dictionary<string, string>();

        protected override void Awake()
        {
            if (m_config == null) m_config = GetComponent<PanelConfig>();

            m_currentPath = new Uri(new Uri(m_config.WebRoot), "/Town_World" + MARKDOWN__FILE_EXTENSION);

            m_markdownStyleSheet = m_config.GetStyleSheet();
            m_appTheme = m_config.GetAppTheme();
        }

        protected override void Start()
        {
            GetPage(m_currentPath.ToString());
        }

        private void HandleTap(string url)
        {
            if (!HandleInternalLink(url))
                HandleExternalLink(url);
        }

        private bool HandleInternalLink(string url)
        {
            if (!url.StartsWith("./") && !url.StartsWith("..")) return false;
            GetPage(url);

            return true;
        }


        private Uri loadPath;
        UnityWebRequest request;
        AsyncOperation operation;

        private void GetPage(string path, bool pushToHistory = true)
        {
            if (!Application.isPlaying) return;

            lastPath = m_currentPath;

            loadPath = new Uri(m_currentPath, path);

            if (m_pageCache.ContainsKey(loadPath.AbsolutePath))
            {
                Debug.Log("used cache");
                markdownData1 = m_pageCache[loadPath.AbsolutePath];
                LoadPage(pushToHistory);
            }
            else
            {
                request = UnityWebRequest.Get(loadPath);
                operation = request.SendWebRequest();
                operation.completed += completed =>
                {
                    markdownData1 = request.downloadHandler.text;
                    if (markdownData1 != null)
                    {
                        if (!m_pageCache.ContainsKey(loadPath.AbsolutePath)) m_pageCache.Add(loadPath.AbsolutePath, markdownData1);
                        LoadPage(pushToHistory);
                    }
                    else
                    {
                        Debug.LogWarningFormat("HandleInternalLink couldn't find markdown at path {0}", path);
                    //TODO user notifcation of missing file
                    //TODO user add new file
                }
                };
            }
        }

        private void LoadPage(bool pushToHistory)
        {
            if (pushToHistory) PushCurrentPageToHistory();
            m_currentPath = loadPath;
            SetAppBarTitle(loadPath.AbsolutePath);
            recreateWidget();
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
            if (m_pageCache.ContainsKey(lastPath?.AbsolutePath))
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

        private void Back()
        {
            if (m_navHistory.Count > 0)
            {
                GetPage(m_navHistory.Pop(), pushToHistory: false);
            }
        }

        private void Home()
        {
            GetPage(new Uri(m_currentPath, "/Town_World" + MARKDOWN__FILE_EXTENSION).ToString());
        }


        protected override Widget createWidget()
        {

            return new MaterialApp(
                theme: m_appTheme,
                title: m_config.Title,
                home: new Scaffold(

                    //app bar
                    appBar: new AppBar(title: new Text(m_appBarTitle.Replace('_', ' '))),

                    //body
                    body: new Markdown(data: markdownData1,
                    syntaxHighlighter: new DartSyntaxHighlighter(SyntaxHighlighterStyle.lightThemeStyle()),
                    onTapLink: HandleTap,
                    imageDirectory: GetImageDir(),
                    styleSheet: m_markdownStyleSheet),

                    //footer
                    persistentFooterButtons: new List<Widget>()
                    {
                        RaisedButton.icon(
                            icon: new Icon(Icons.home),
                            label: new Text("HOME"),
                            onPressed: Home),

                        RaisedButton.icon(
                            icon: new Icon(Icons.arrow_back),
                            label: new Text("BACK"),
                            onPressed: Back),


                        RaisedButton.icon(
                            icon: new Icon(Icons.power),
                            label: new Text("QUIT"),
                            onPressed: () => Application.Quit()),

                    }));
        }

        private string GetImageDir()
        {
            return m_config.WebRoot;
        }

    }
}
