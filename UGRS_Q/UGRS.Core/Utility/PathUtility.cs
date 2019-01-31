// file:	Utility\PathUtility.cs
// summary:	Implements the path utility class

using System;
using System.IO;
using System.Reflection;

namespace UGRS.Core.Utility
{
    /// <summary> A path utilities. </summary>
    /// <remarks> Ranaya, 04/05/2017. </remarks>
    public class PathUtilities
    {
        /// <summary> Gets the current path. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>
        /// <returns> The current path. </returns>
        public static string GetCurrent()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase).Replace("file://", string.Empty).Replace("file:\\", string.Empty);
        }

        /// <summary> Gets the current path. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>
        /// <param name="pStrRelativePath"> Full path of the relative path. </param>
        /// <returns> The current path. </returns>
        public static string GetCurrent(string pStrRelativePath)
        {
            return Path.Combine(GetCurrent(), pStrRelativePath);
        }

        /// <summary> Gets the documents path. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>
        /// <returns> The documents path. </returns>
        public static string GetDocuments()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        /// <summary> Gets the documents path. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>
        /// <param name="pStrRelativePath"> Full path of the relative path. </param>
        /// <returns> The documents path. </returns>
        public static string GetDocuments(string pStrRelativePath)
        {
            return Path.Combine(GetDocuments(), pStrRelativePath);
        }

        /// <summary> Gets the programs path. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>
        /// <returns> The programs path. </returns>
        public static string GetPrograms()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        }

        /// <summary> Gets the programs path. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>
        /// <param name="pStrRelativePath"> Full path of the relative path. </param>
        /// <returns> The programs path. </returns>
        public static string GetPrograms(string pStrRelativePath)
        {
            return Path.Combine(GetPrograms(), pStrRelativePath);
        }

        /// <summary> Gets the programs x86 path. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>
        /// <returns> The programs x86 path. </returns>
        public static string GetProgramsX86()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        }

        /// <summary> Gets the programs x86 path. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>
        /// <param name="pStrRelativePath"> Full path of the relative path. </param>
        /// <returns> The programs x86 path. </returns>
        public static string GetProgramsX86(string pStrRelativePath)
        {
            return Path.Combine(GetProgramsX86(), pStrRelativePath);
        }

        /// <summary>
        ///     Gets the application data path
        /// </summary>
        /// <remarks>
        ///     Raul Anaya, 06/06/2018 
        /// </remarks>
        /// <returns> 
        ///     Application data path
        /// </returns>
        public static string GetApplicationData()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }

        /// <summary>
        ///     Get the application data path
        /// </summary>
        /// <param name="pStrRelativePath">
        ///     Relative path
        /// </param>
        /// <returns>
        ///     Application data path
        /// </returns>
        public static string GetApplicationData(string pStrRelativePath)
        {
            return Path.Combine(GetApplicationData(), pStrRelativePath);
        }
    }
}
