using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Diagnostics;
using System.IO;
using MahApps.Metro.Controls;
using Shell32;
using System.Xml;
using System.Windows.Media.Imaging;
using System.Drawing;

namespace PrettyTiles
{
    public partial class MainWindow : MetroWindow
    {
        private static string _appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        private static List<string> ShortcutList = new List<string>();
        private static string _programs = $"{_appdata}\\Microsoft\\Windows\\Start Menu\\Programs\\";

        private static string CurrentFile = null;

        public MainWindow()
        {
            InitializeComponent();
            ExploreShortcuts();
            LoadShortcuts();
        }

        //Discover the shortcuts in start menu
        static void ExploreShortcuts()
        {
            foreach (string file in Directory.EnumerateFiles(_programs))
            {
                string lnk = file;
                if(lnk.Contains(".lnk"))
                {
                    lnk = lnk.Replace(_programs, String.Empty);
                    lnk = lnk.Replace(".lnk", String.Empty);
                    Console.WriteLine($"{lnk} added to internal list");
                    ShortcutList.Add(lnk);
                }
            }
        }

        //Load shortcut list into the ItemList control
        void LoadShortcuts()
        {
            foreach(string foo in ShortcutList)
            {
                IconList.Items.Add(foo);
            }
        }

        private void GithubIcon_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://github.com/FaithLV");
        }

        //Display tile in preview
        private void UpdateImage()
        {
            string fileDirectory = Path.GetDirectoryName(CurrentFile);
            string currentFile = Path.GetFileNameWithoutExtension(CurrentFile);
            string visualXml = Path.Combine(fileDirectory, $"{currentFile}.visualelementsmanifest.xml");
            string tileImage = TileSourceFromXml(visualXml);
            if(tileImage != null)
            {
                //Load tile from image
                TilePreview.Source = new BitmapImage(new Uri($"{fileDirectory}\\{tileImage}"));
            }
            else
            {
                //Set placeholder tile
                var placeholderUri = new Uri("resources/placeholder.jpg", UriKind.RelativeOrAbsolute);
                TilePreview.Source = new BitmapImage(placeholderUri);
            }
            
        }

        //Get tile image source from visual elements manifest xml
        static string TileSourceFromXml(string _xml)
        {
            if (File.Exists(_xml))
            {
                using (XmlReader reader = XmlReader.Create(_xml))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case "Application":
                                    Console.WriteLine("Application found!");
                                    break;
                                case "VisualElements":
                                    string attribute = reader["Square150x150Logo"];
                                    return attribute;
                            }
                        }
                    }
                }
            }
            return null;
        }

        //Get target path from shortcut (lnk)
        //Should work for all windows versions, because 2006
        //http://www.saunalahti.fi/janij/blog/2006-12.html#d6d9c7ee-82f9-4781-8594-152efecddae2
        private string GetTargetPath(string _lnk)
        {
            Shell shell = new ShellClass();
            Folder folder = shell.NameSpace(_programs);
            FolderItem item = folder.ParseName($"{_lnk}.lnk");
            ShellLinkObject link = (ShellLinkObject)item.GetLink;
            return link.Path;
        }

        private void IconList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            //Full path to the shortcut
            string _linkpath = $"{_programs}{IconList.SelectedItem.ToString()}.lnk";

            //Full path to the target file
            CurrentFile = GetTargetPath(IconList.SelectedItem.ToString());

            //Update the "Target Path" textbox
            TargetTextBox.Text = CurrentFile;

            //Update tile image file
            UpdateImage();
        }
    }
}
