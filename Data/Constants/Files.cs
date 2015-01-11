using System.IO;

namespace PDM.BusinessData.Constants
{
    static class Files
    {
        static Files()
        {
            DataFile = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), @"Data\PDM.xml");
        }

        /// <summary>
        /// Data\PDM.xml"
        /// </summary>
        public static string DataFile { get; private set; }
    }
}
