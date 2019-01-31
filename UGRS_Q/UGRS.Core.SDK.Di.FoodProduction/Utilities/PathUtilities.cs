using System;
using System.IO;
using System.Reflection;

namespace UGRS.Core.SDK.DI.FoodProduction.Utilities
{
    public class PathUtilities
    {
        public static string GetCurrent()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase).Replace("file://", string.Empty).Replace("file:\\", string.Empty);
        }

        public static string GetCurrent(string relativePath)
        {
            return Path.Combine(GetCurrent(), relativePath);
        }

        public static string GetDocuments()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        public static string GetDocuments(string relativePath)
        {
            return Path.Combine(GetDocuments(), relativePath);
        }

        public static string GetPrograms()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        }

        public static string GetPrograms(string relativePath)
        {
            return Path.Combine(GetPrograms(), relativePath);
        }

        public static string GetProgramsX86()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        }

        public static string GetProgramsX86(string relativePath)
        {
            return Path.Combine(GetProgramsX86(), relativePath);
        }
    }
}
