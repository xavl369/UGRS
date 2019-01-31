using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace UGRS.Core.Extension.String
{
    /// <summary>
    ///     Extensión para facilitar el uso de textos
    /// </summary>
    /// <remarks>
    ///     Raul Anaya, 11/01/2018
    /// </remarks>
    public static class StringExtension
    {
        /// <summary>
        ///     Método para quitar diacríticos
        /// </summary>
        /// <param name="pStrString">
        ///     Texto
        /// </param>
        /// <returns>
        ///     Texto normalizado
        /// </returns>
        public static string RemoveDiacritics(this string pStrString)
        {
            string lObjStrNormalizedString = pStrString.Normalize(NormalizationForm.FormD);
            StringBuilder lObjStringBuilder = new StringBuilder();

            for (int i = 0; i < lObjStrNormalizedString.Length; i++)
            {
                Char lObjChar = lObjStrNormalizedString[i];
                if (CharUnicodeInfo.GetUnicodeCategory(lObjChar) != UnicodeCategory.NonSpacingMark)
                {
                    lObjStringBuilder.Append(lObjChar);
                }
            }

            return lObjStringBuilder.ToString();
        }

        /// <summary>
        ///     Método para comparar dos strings de forma relativa
        /// </summary>
        /// <param name="pStrString">
        ///     Texto
        /// </param>
        /// <returns>
        ///     Retorna true si el string tiene mas de tres coincidencias
        /// </returns>
        public static bool RelativeEqual(this string pStrStringA, string pStrStringB)
        {
            List<string> lLstStrTempA = pStrStringA.RemoveDiacritics().ToLower().Split(' ').ToList();
            List<string> lLstStrTempB = pStrStringB.RemoveDiacritics().ToLower().Split(' ').ToList();
            int lIntPosibleCount = lLstStrTempA.Count;
            int lIntCurrentCount = 0;

            for (int i = 0; i < lLstStrTempA.Count; i++)
            {
                if(!string.IsNullOrEmpty(lLstStrTempA[i]) )
                {
                    for (int j = 0; j < lLstStrTempB.Count; j++)
                    {
                        if (!string.IsNullOrEmpty(lLstStrTempB[j]) && lLstStrTempA[i].Equals(lLstStrTempB[j]))
                        {
                            lIntCurrentCount++;
                            lLstStrTempB[j] = "";
                        }
                    }
                }
            }

            return lIntPosibleCount == lIntCurrentCount;
        }
    }
}
