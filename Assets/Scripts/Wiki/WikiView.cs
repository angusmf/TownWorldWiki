using markdown;

using UnityEngine;

using Unity.UIWidgets.engine;
using Unity.UIWidgets.material;
using Unity.UIWidgets.widgets;
using Unity.UIWidgets.painting;

using System;
using System.IO;
using System.Collections.Generic;


namespace UIWidgetsWiki
{
    public class WikiView : UIWidgetsPanel
    {

        private const string MARKDOWN__FILE_EXTENSION = ".markdown";
        private const string NO_DATA = "no data";

        private Stack<string> m_navHistory = new Stack<string>();

        private string m_appBarTitle = string.Empty;
        private string markdownData1 = NO_DATA;
        private string m_currentPath = string.Empty;
        private MarkdownStyleSheet m_markdownStyleSheet;
        private PanelConfig m_config;
        private HashSet<string> m_PathExists = new HashSet<string>();

        protected override void Awake()
        {
            if (m_config == null) m_config = GetComponent<PanelConfig>();
            m_currentPath = Path.Combine(Application.streamingAssetsPath, "markdown/Town_World" + MARKDOWN__FILE_EXTENSION);

            m_markdownStyleSheet = m_config.GetStyleSheet();

            //m_appTheme = new ThemeData

            LoadPage(m_currentPath);
        }

        

        private void HandleTap(string url)
        {
            if (!HandleInternalLink(url))
                HandleExternalLink(url);  
        }

        private bool HandleInternalLink(string url)
        {
            if (!url.StartsWith("./") && !url.StartsWith("..")) return false;

            if (!NavigateTo(url))
            { 
            Debug.LogWarningFormat("HandleInternalLink couldn't find file at path {0} (url = {1})",
                    Path.Combine(Path.GetDirectoryName(m_currentPath), Path.GetDirectoryName(url)),
                    url);
                //TODO user notifcation of missing file
                //TODO user add new file
            }

            return true;
        }

        bool ReloadPage(string path)
        {
            if (LoadPage(path))
            {
                recreateWidget();
                return true;
            }
            else
            {
                return false;
            }
        }

        private string loadPath;

        private bool LoadPage(string path)
        {
            if (!Application.isPlaying) return false;

            loadPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(m_currentPath), path));
            Debug.LogFormat("loadPath after GetFullPath = {0}", loadPath);


            if (File.Exists(loadPath))
            {
                markdownData1 = File.ReadAllText(loadPath).Replace("![](./", "![](");

                if (!m_PathExists.Contains(loadPath)) m_PathExists.Add(loadPath);
                m_currentPath = loadPath;
                SetAppBarTitle(loadPath.Replace(Path.Combine(Application.streamingAssetsPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar), "markdown"), string.Empty));
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
            if (m_PathExists.Contains(Path.GetFullPath(lastPath)))
            {
                m_navHistory.Push(lastPath);
                return true;
            }
            else
            {
                Debug.LogWarningFormat("Could not push non-existent page \"{0}\" to history.",
                    lastPath);
                return false;
            }
        }

        private string lastPath;
        private int count;

        private bool NavigateTo(string path)
        {
            Debug.LogFormat("current path = {0}, new path = {1}", m_currentPath, path);

            lastPath = m_currentPath;

            if (ReloadPage(path))
            {
                return PushCurrentPageToHistory();
            }
            return false;
        }

        private void Back()
        {
            if (m_navHistory.Count > 0)
            {
                ReloadPage(m_navHistory.Pop());
            }
        }

        void Home()
        {
            NavigateTo(Path.Combine(Application.streamingAssetsPath, "markdown/Town_World" + MARKDOWN__FILE_EXTENSION));
        }

        protected override Widget createWidget()
        {
            if (!Application.isPlaying) return null;

            return new MaterialApp(
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
            return Path.GetDirectoryName(m_currentPath);
        }
    }
}
