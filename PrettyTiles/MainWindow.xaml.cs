using System;
using System.Collections.Generic;
using System.Windows.Input;
using MahApps.Metro.Controls;
using System.Diagnostics;
using System.IO;
using Shell32;

namespace PrettyTiles
{
    public partial class MainWindow : MetroWindow
    {
        private static string _appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        private static List<string> ShortcutList = new List<string>();
        private static string _programs = $"{_appdata}\\Microsoft\\Windows\\Start Menu\\Programs\\";

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
            string _filepath = GetTargetPath(IconList.SelectedItem.ToString());

            //Update the "Target Path" textbox
            TargetTextBox.Text = _filepath;
        }
    }
}
