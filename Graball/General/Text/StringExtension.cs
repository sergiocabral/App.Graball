using System;
using System.Collections.Generic;
using System.Text;

namespace Graball.General.Text
{
    /// <summary>
    /// Extensões para string.
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// Traduz um texto.
        /// </summary>
        /// <param name="text">Texto.</param>
        /// <param name="language">Opcional. Especificar idioma.</param>
        /// <returns>Texto traduzido.</returns>
        public static string Translate(this string text, string language = null)
        {
            return Text.Translate.GetText(text, language ?? Text.Translate.Default.Language);
        }
    }
}
