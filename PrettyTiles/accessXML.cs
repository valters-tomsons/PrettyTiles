﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PrettyTiles
{
    class accessXML
    {

        //Delete xml manifest
        private static void DeleteManifest(string _xml)
        {
            if (File.Exists(_xml))
            {
                Console.WriteLine("Manifest file broken, deleting");
                File.Delete(_xml);
            }
        }


        //Get tile image source from visual elements manifest xml
        public static string TileSourceFromXml(string _xml)
        {
            if (File.Exists(_xml))
            {
                try
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
                                        break;
                                    case "VisualElements":
                                        string attribute = reader["Square150x150Logo"];
                                        return attribute;
                                }
                            }
                        }
                    }
                }
                catch
                {
                    DeleteManifest(_xml);
                }
            }
            return null;
        }

        public static bool TileLabelFromXml(string _xml)
        {
            if (File.Exists(_xml))
            {
                try
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
                                        break;
                                    case "VisualElements":
                                        string attribute = reader["ShowNameOnSquare150x150Logo"];
                                        if (attribute == "on") { return true; } else { return false; }
                                }
                            }
                        }
                    }
                }
                catch
                {
                    DeleteManifest(_xml);
                }

            }
            return false;
        }

        public static bool TileDarkFromXml(string _xml)
        {
            if (File.Exists(_xml))
            {
                try
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
                                        break;
                                    case "VisualElements":
                                        string attribute = reader["ForegroundText"];
                                        //dark and light
                                        if (attribute == "dark") { return true; } else { return false; }
                                }
                            }
                        }
                    }
                }
                catch
                {
                    DeleteManifest(_xml);
                }

            }
            return false;
        }
    }
}
