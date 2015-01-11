using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using PDM.Common;

namespace PDM.Client.Settings
{
    internal static class ApplicationSettings
    {
        private static readonly string _SettingsFilePath =
            Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location),
                @"ApplicationSettings.xml");

        static ApplicationSettings()
        {
            if (File.Exists(_SettingsFilePath))
            {
                SerializedSetings = new DataSerializer().DeserializeFromFile<SeriazableSettings>(_SettingsFilePath);
            }
            else
            {
                SerializedSetings = new SeriazableSettings();
            }            
        }

        #region User settings

        /// <summary>
        /// Gets the settings.
        /// </summary>
        private static
            UserSettings Settings
        {
            get { return UserSettings.Default; }
        }

        public static StringCollection Folders
        {
            get { return Settings.Folders ?? new StringCollection(); }
        }

        public static StringCollection FileTypes
        {
            get { return Settings.FileTypes ?? new StringCollection(); }
        }

        public static StringCollection Templates
        {
            get { return Settings.Templates ?? new StringCollection(); }
        }

        public static void Save()
        {
            Settings.Save();
        }

        #endregion

        #region Serialized settings

        public static SeriazableSettings SerializedSetings { get; set; }

        public static void SaveSerializedSettings()
        {
            new DataSerializer().SerializeToFile(SerializedSetings, _SettingsFilePath);
        }

        public static List<string> FilterNames
        {
            get
            {
                return new List<string>
                {
                    SerializedSetings.Filter01Name,
                    SerializedSetings.Filter02Name,
                    SerializedSetings.Filter03Name,
                    SerializedSetings.Filter04Name,
                    SerializedSetings.Filter05Name,
                    SerializedSetings.Filter06Name,
                    SerializedSetings.Filter07Name,
                    SerializedSetings.Filter08Name
                };
            }
        }

        public static List<string> FilterTexts
        {
            get
            {
                return new List<string>
                {
                    SerializedSetings.Filter01Text,
                    SerializedSetings.Filter02Text,
                    SerializedSetings.Filter03Text,
                    SerializedSetings.Filter04Text,
                    SerializedSetings.Filter05Text,
                    SerializedSetings.Filter06Text,
                    SerializedSetings.Filter07Text,
                    SerializedSetings.Filter08Text
                };
            }
        }

        public static List<string> FilterIcons
        {
            get
            {
                return new List<string>
                {
                    SerializedSetings.Filter01Icon,
                    SerializedSetings.Filter02Icon,
                    SerializedSetings.Filter03Icon,
                    SerializedSetings.Filter04Icon,
                    SerializedSetings.Filter05Icon,
                    SerializedSetings.Filter06Icon,
                    SerializedSetings.Filter07Icon,
                    SerializedSetings.Filter08Icon
                };
            }
        }

        #endregion
    }
}
