using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Graball.General.Text.Formatter
{
    /// <summary>
    /// <para>Implementa a interface <see cref="IFormatProvider"/>.</para>
    /// <para>Esta classe converte caracteres especiais do html para texto comum.</para>
    /// </summary>
    public class FormatHtmlEspecialChars : FormatNulled
    {
        #region Public

        /// <summary>
        /// <para>Construtor.</para>
        /// </summary>
        public FormatHtmlEspecialChars() : base() { }

        /// <summary>
        /// <para>Construtor.</para>
        /// </summary>
        /// <param name="formatProvider"><para>Decora um FormatProvider já existente.</para></param>
        public FormatHtmlEspecialChars(IFormatProvider formatProvider) : base(formatProvider) { }

        /// <summary>
        /// <para>Contrutor.</para>
        /// </summary>
        /// <param name="tags">
        /// <para>Lista das tags HTML ou XML que devem ser removidas.</para>
        /// </param>
        public FormatHtmlEspecialChars(params KeyValuePair<string, string>[] chars) : this(null, chars) { }

        /// <summary>
        /// <para>Contrutor.</para>
        /// </summary>
        /// <param name="tags">
        /// <para>Lista das tags HTML ou XML que devem ser removidas.</para>
        /// </param>
        /// <param name="formatProvider"><para>Decora um FormatProvider já existente.</para></param>
        public FormatHtmlEspecialChars(IFormatProvider formatProvider, params KeyValuePair<string, string>[] chars)
            : this(formatProvider)
        {
            if (chars != null)
            {
                foreach (var item in chars)
                {
                    Chars[item.Key] = item.Value;
                }
            }
        }

        /// <summary>
        /// <para>(Leitura)</para>
        /// <para>Lista das tags HTML ou XML que devem ser removidas.</para>
        /// </summary>
        public IDictionary<string, string> Chars { get; } = new Dictionary<string, string>()
        {
            { "&nbsp;", " " },
            { "&amp;", "&" }
        };

        #endregion

        #region ICustomFormatter Members

        /// <summary>
        /// <para>Converte o valor de um objeto especificado para um representação 
        /// de seqüência equivalente usando o formato especificado e 
        /// informações de formatação da região (culture-specific).</para>
        /// </summary>
        /// <param name="format"><para>A seqüência de formato que contém 
        /// especificações de formatação.</para></param>
        /// <param name="arg"><para>O objeto a ser formatado.</para></param>
        /// <param name="formatProvider"><para>Um objeto que fornece informações sobre 
        /// o formato da instância atual.</para></param>
        /// <returns><para>A sequência de texto <paramref name="arg"/> formatada
        /// conforme especificado pelos parâmetros <paramref name="format"/> e 
        /// <paramref name="formatProvider"/>.</para></returns>
        public override string Format(string format, object arg, IFormatProvider formatProvider)
        {
            var result = base.Format(format, arg, formatProvider);
            foreach (var item in Chars)
            {
                result = Regex.Replace(result, item.Key, item.Value);
            }
            return result;
        }

        #endregion
    }
}
