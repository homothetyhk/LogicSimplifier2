using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace LogicSimplifier2
{
    public static class XmlLoader
    {
        public static string BaseDirectory => AppDomain.CurrentDomain.BaseDirectory;
        public static string InputDirectory => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Input");

        public static bool HasMacroXml()
        {
            return File.Exists(Path.Combine(InputDirectory, "macros.xml"));
        }

        public static bool HasSettingsXml()
        {
            return File.Exists(Path.Combine(InputDirectory, "settings.xml"));
        }

        public static bool HasLocationXml()
        {
            return File.Exists(Path.Combine(InputDirectory, "locations.xml"));
        }

        public static bool HasWaypointsXml()
        {
            return File.Exists(Path.Combine(InputDirectory, "waypoints.xml"));
        }

        public static string[] LoadSettings()
        {
            XmlDocument settingsDoc = new XmlDocument();
            settingsDoc.Load(Path.Combine(InputDirectory, "settings.xml"));
            return settingsDoc.SelectNodes("randomizer/setting").Cast<XmlNode>()
                .Select(x => x.InnerText).ToArray();
        }

        public static Dictionary<string, string> LoadWaypoints()
        {
            XmlDocument waypointDoc = new XmlDocument();
            waypointDoc.Load(Path.Combine(InputDirectory, "waypoints.xml"));
            return waypointDoc.SelectNodes("randomizer/item").Cast<XmlNode>()
                .ToDictionary(x => x.Attributes["name"].Value,
                x => x.ChildNodes.Cast<XmlNode>().First(c => c.LocalName.Contains("ogic")).InnerText);
        }

        public static Dictionary<string, string> LoadLocations()
        {
            XmlDocument locationDoc = new XmlDocument();
            locationDoc.Load(Path.Combine(InputDirectory, "locations.xml"));
            return locationDoc.SelectNodes("randomizer/item").Cast<XmlNode>()
                .ToDictionary(x => x.Attributes["name"].Value,
                x => x.ChildNodes.Cast<XmlNode>().First(c => c.LocalName.Contains("ogic")).InnerText);
        }

        public static Dictionary<string, string> LoadMacros()
        {
            XmlDocument macroDoc = new XmlDocument();
            macroDoc.Load(Path.Combine(InputDirectory, "macros.xml"));
            return macroDoc.SelectNodes("randomizer/macro").Cast<XmlNode>()
                .ToDictionary(x => x.Attributes["name"].Value, x => x.InnerText);
        }

        public static void Load(out string[] settings, out Dictionary<string, string> macros, 
            out Dictionary<string, string> waypoints, out Dictionary<string, string> locations)
        {
            settings = LoadSettings();
            macros = LoadMacros();
            waypoints = LoadWaypoints();
            locations = LoadLocations();
        }
    }
}
