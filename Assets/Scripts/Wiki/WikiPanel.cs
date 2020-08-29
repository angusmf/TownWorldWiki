﻿using markdown;

using UnityEngine;

using Unity.UIWidgets.engine;
using Unity.UIWidgets.material;
using Unity.UIWidgets.widgets;
using Unity.UIWidgets.painting;

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace UIWidgetsWiki
{

    public class NavigationState
    {
        public string page;
        public string directory;
    }

    public class WikiPanel : UIWidgetsPanel
    {

        string Title = "Town World Wiki";
        string RootName = "Town_World";

        private const string MARKDOWN__FILE_EXTENSION = ".markdown";

        private Stack<NavigationState> m_navHistory = new Stack<NavigationState>();

        private string m_appBarTitle;
        private string markdownData1;
        private string m_currentDirectory;

        protected override void Awake()
        {
            LoadPage(Path.Combine(Application.streamingAssetsPath, "markdown"), RootName);
            PanelConfig config = GetComponent<PanelConfig>();
            if (config != null)
            {
                Title = config.Title;
                RootName = config.RootName;
            }
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

            string path = Path.Combine(currentDirectory, Path.GetFileName(url));
            Debug.LogFormat("url ={0} - path = {1} - Path.GetDirectoryName(url) = {2} - Path.GetFileName(url) = {3} - m_appBarTitle = {4} - currentDirectory = {5}",
                url, path, Path.GetDirectoryName(url), Path.GetFileName(url), m_appBarTitle, currentDirectory);

            if (File.Exists(path))
            {
                PushCurrentPageToHistory();
                LoadPage(currentDirectory, Path.GetFileNameWithoutExtension(url));
                recreateWidget();
            }
            else
            {
                Debug.LogWarningFormat("couldn't find file at path {0}", path);
                //TODO user notifcation of missing file
                //TODO user add new file
            }

            return true;
        }

        private void LoadPage(string path, string file)
        {
            if (File.Exists(Path.Combine(path, file + MARKDOWN__FILE_EXTENSION)))
            {
                //markdownData1 = File.ReadAllText(Path.Combine(path, file));
                markdownData1 = File.ReadLines(Path.Combine(path, file + MARKDOWN__FILE_EXTENSION)).Where(line => !line.StartsWith("![]"))
                    .Aggregate(string.Empty, (data, line) => data += line + '\n', data => data);

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
                      onTapLink: HandleTap),

                    //footer
                    persistentFooterButtons: new List<Widget>()
                    {
                        RaisedButton.icon(
                            icon: new Icon(Icons.home, size: 18.0f, color: new Unity.UIWidgets.ui.Color(0xFFFFFFFF)),
                            label: new Text("^ HOME", style: new TextStyle(true, new Unity.UIWidgets.ui.Color(0xFF000000))),
                            onPressed: () => {
                                PushCurrentPageToHistory();
                                LoadPage(Path.Combine(Application.streamingAssetsPath, "markdown"),  RootName);
                                recreateWidget();
                            }),
                        RaisedButton.icon(
                            icon: new Icon(Icons.arrow_back, size: 18.0f, color: new Unity.UIWidgets.ui.Color(0xFFFFFFFF)),
                            label: new Text("< BACK", style: new TextStyle(true, new Unity.UIWidgets.ui.Color(0xFF000000))),
                            onPressed: () => {
                                if (m_navHistory.Count > 0)
                                {
                                    NavigationState navigationState = m_navHistory.Pop();
                                    LoadPage(navigationState.directory, navigationState.page);
                                    recreateWidget();
                                }
                            }),
                    }));
        }
    }
}
