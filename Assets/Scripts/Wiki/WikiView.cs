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

    public class NavigationState
    {
        public string page;
        public string directory;
    }

    public class WikiView : UIWidgetsPanel
    {

        string Title = "Town World Wiki";
        string RootName = "Town_World";

        private const string MARKDOWN__FILE_EXTENSION = ".markdown";

        private Stack<NavigationState> m_navHistory = new Stack<NavigationState>();

        private string m_appBarTitle;
        private string markdownData1;
        private string m_currentDirectory;

        private PanelConfig m_config;

        protected override void Awake()
        {
            if (m_config == null) m_config = GetComponent<PanelConfig>();
            Title = m_config?.Title;
            RootName = m_config?.RootName;
            LoadPage(Path.Combine(Application.streamingAssetsPath, "markdown"), RootName); 
        }

        private void HandleTap(string url)
        {
            if (!HandleInternalLink(url))
                HandleExternalLink(url);  
        }

        private bool HandleInternalLink(string url)
        {
            if (!url.StartsWith("./") && !url.StartsWith("..")) return false;

            string currentDirectory = Path.Combine(m_currentDirectory, Path.GetDirectoryName(url));

            if (File.Exists(Path.Combine(currentDirectory, Path.GetFileName(url))))
            {
                PushCurrentPageToHistory();
                ReloadPage(currentDirectory, Path.GetFileNameWithoutExtension(url));
            }
            else
            {
                Debug.LogWarningFormat("couldn't find file at path {0} (url = {1})",
                    Path.Combine(currentDirectory, Path.GetFileName(url)),
                    url);
                //TODO user notifcation of missing file
                //TODO user add new file
            }

            return true;
        }

        void ReloadPage(string path, string file)
        {
            LoadPage(path, file);
            recreateWidget();
        }

        private void LoadPage(string path, string file)
        {
            if (File.Exists(Path.Combine(path, file + MARKDOWN__FILE_EXTENSION)))
            {
                markdownData1 = File.ReadAllText(Path.Combine(path, file + MARKDOWN__FILE_EXTENSION)).Replace("![](./", "![](");

                m_currentDirectory = path;
                m_appBarTitle = file;
            }
            else
            {
                Debug.LogErrorFormat("couldn't find file at path {0}. Real Error. This shouldn't happen", Path.Combine(path, file + MARKDOWN__FILE_EXTENSION));
            }
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

        private void PushCurrentPageToHistory()
        {
            if (File.Exists(Path.Combine(m_currentDirectory, m_appBarTitle + MARKDOWN__FILE_EXTENSION)))
            {
                m_navHistory.Push(new NavigationState() { directory = m_currentDirectory, page = m_appBarTitle });
            }
            else
            {
                Debug.LogWarningFormat("Could not push non-existent page \"{0}\" to history.",
                    Path.Combine(m_currentDirectory, m_appBarTitle + MARKDOWN__FILE_EXTENSION));
            }
        }

        protected override Widget createWidget()
        {
            return new MaterialApp(
                title: Title,
                home: new Scaffold(
                    
                    //app bar
                    appBar: new AppBar(title: new Text(m_appBarTitle.Replace('_', ' '))),

                    //body
                    body: new Markdown(data: markdownData1,
                      syntaxHighlighter: new DartSyntaxHighlighter(SyntaxHighlighterStyle.lightThemeStyle()),
                      onTapLink: HandleTap,
                      imageDirectory: m_currentDirectory),

                    //footer
                    persistentFooterButtons: new List<Widget>()
                    {
                        RaisedButton.icon(
                            icon: new Icon(Icons.home, size: 18.0f, color: Colors.white),
                            label: new Text("HOME", style: new TextStyle(true, color: Colors.white)),
                            onPressed: () => {
                                PushCurrentPageToHistory();
                                ReloadPage(Path.Combine(Application.streamingAssetsPath, "markdown"),  RootName);
                            }),
                   
                        RaisedButton.icon(
                            icon: new Icon(Icons.arrow_back, size: 18.0f, color: Colors.white),
                            label: new Text("BACK", style: new TextStyle(true, color: Colors.white)),
                            onPressed: () => {
                                if (m_navHistory.Count > 0)
                                {
                                    NavigationState navigationState = m_navHistory.Pop();
                                    ReloadPage(navigationState.directory, navigationState.page);
                                }
                            }),


                        RaisedButton.icon(
                            icon: new Icon(Icons.power, size: 18.0f, color: Colors.white),
                            label: new Text("QUIT", style: new TextStyle(true, color: Colors.white)),
                            onPressed: () => Application.Quit()),

                    }));
        }
    }
}
