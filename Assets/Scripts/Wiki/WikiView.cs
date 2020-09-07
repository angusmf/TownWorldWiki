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
        private const string NO_DATA = "no data";

        private Stack<NavigationState> m_navHistory = new Stack<NavigationState>();

        private string m_appBarTitle = string.Empty;
        private string markdownData1 = NO_DATA;
        private string m_currentDirectory = string.Empty;
        private MarkdownStyleSheet m_markdownStyleSheet;
        private PanelConfig m_config;
        private HashSet<string> m_PathExists = new HashSet<string>();

        protected override async void Awake()
        {
            if (m_config == null) m_config = GetComponent<PanelConfig>();
            Title = m_config?.Title;
            RootName = m_config?.RootName;
            await LoadPageAsync(Path.Combine(Application.streamingAssetsPath, "markdown"), RootName + MARKDOWN__FILE_EXTENSION);
            m_markdownStyleSheet = MarkdownStyleSheet.fromTheme(new ThemeData(textTheme:
                new TextTheme(

                    display4:   new TextStyle(fontSize: 70.0f, fontWeight: Unity.UIWidgets.ui.FontWeight.bold), //h1
                    display3:   new TextStyle(fontSize: 60.0f, fontWeight: Unity.UIWidgets.ui.FontWeight.bold), //h2
                    display2:   new TextStyle(fontSize: 50.0f, fontWeight: Unity.UIWidgets.ui.FontWeight.bold), //h3
                    display1:   new TextStyle(fontSize: 40.0f, fontWeight: Unity.UIWidgets.ui.FontWeight.bold), //h4
                    headline:   new TextStyle(fontSize: 30.0f, fontWeight: Unity.UIWidgets.ui.FontWeight.bold), //h5
                    title:      new TextStyle(fontSize: 20.0f, fontWeight: Unity.UIWidgets.ui.FontWeight.bold), //h6

                    body1:      new TextStyle(fontSize: 16.0f)
                    //            caption: new TextStyle(color: Colors.pink, fontSize: 8.0f),
                    //            button: new TextStyle(color: Colors.purple, fontSize: 8.0f),
                    //            overline: new TextStyle(color: Colors.red, fontSize: 8.0f)

                    )));
                    
        
        }
        

        private async void HandleTap(string url)
        {
            if (!await HandleInternalLink(url))
                HandleExternalLink(url);  
        }

        private async UniTask<bool> HandleInternalLink(string url)
        {
            if (!url.StartsWith("./") && !url.StartsWith("..")) return false;

            if (!await NavigateTo(Path.Combine(m_currentDirectory, Path.GetDirectoryName(url)), Path.GetFileName(url)))
            { 
            Debug.LogWarningFormat("HandleInternalLink couldn't find file at path {0} (url = {1})",
                    Path.Combine(m_currentDirectory, Path.GetDirectoryName(url)),
                    url);
                //TODO user notifcation of missing file
                //TODO user add new file
            }

            return true;
        }

        async UniTask<bool> ReloadPage(string path, string file)
        {
            if (await LoadPageAsync(path, file))
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

        private async UniTask<bool> LoadPageAsync(string path, string file)
        {
            if (!Application.isPlaying) return false;

            loadPath = Path.Combine(path, file);

#if (!UNITY_EDITOR && UNITY_WEBGL) || (!UNITY_EDITOR && UNITY_ANDROID )

            markdownData1 = (await UnityWebRequest.Get(loadPath).SendWebRequest())
                .downloadHandler.text?.Replace("![](./", "![](");

            if (markdownData1 != null)
            {
#else
            if (File.Exists(loadPath))
            {
                markdownData1 = File.ReadAllText(loadPath).Replace("![](./", "![](");
#endif
                if (!m_PathExists.Contains(loadPath)) m_PathExists.Add(loadPath);
                m_currentDirectory = path;
                m_appBarTitle = file;
                return true;
            }
            else
            {
                Debug.LogErrorFormat("LoadPage couldn't find file at path {0}.", Path.Combine(path, file));
                return false;
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
            if (m_PathExists.Contains(Path.Combine(lastPath, lastFile)))
            {
                m_navHistory.Push(new NavigationState() { directory = lastPath, page = lastFile });
            }
            else
            {
                Debug.LogWarningFormat("Could not push non-existent page \"{0}\" to history.",
                    Path.Combine(m_currentDirectory, m_appBarTitle));
            }
        }

        private string lastPath;
        private string lastFile;

        private async UniTask<bool> NavigateTo(string path, string file)
        {
            lastPath = m_currentDirectory;
            lastFile = m_appBarTitle;

            if (await ReloadPage(path, file))
            {
                PushCurrentPageToHistory();
                return true;
            }
            return false;
        }

        async void Back()
        {
            if (m_navHistory.Count > 0)
            {
                NavigationState navigationState = m_navHistory.Pop();
                await ReloadPage(navigationState.directory, navigationState.page);
            }
        }

        async void Home()
        {
            await NavigateTo(Path.Combine(Application.streamingAssetsPath, "markdown"), RootName + MARKDOWN__FILE_EXTENSION);
        }

        protected override Widget createWidget()
        {
            if (!Application.isPlaying) return null;

            if (markdownData1 == NO_DATA)
            {
                NavigateTo(Path.Combine(Application.streamingAssetsPath, "markdown"), RootName + MARKDOWN__FILE_EXTENSION);
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
                    imageDirectory: m_currentDirectory,
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
