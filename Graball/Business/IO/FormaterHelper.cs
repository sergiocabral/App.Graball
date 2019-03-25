using Graball.General.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Graball.Business.IO
{

    /// <summary>
    /// Auxilia na formatação de textos para output
    /// Implementar esses marcadores ao escrever texto:
    /// Pode iniciar a frase ("#Por exemplo") ou conter palavras ("Outro *exemplo* aqui.")
    ///     Título:     ^
	///     Destacado:  *
	///     Detalhe:    #
	///     Dica:       _
	///     Erro:       !
	///     Pergunta:   ?
	///     Resposta:   @
	///     Nova linha: \n
    /// </summary>
    public static class FormaterHelper
    {
        /// <summary>
        /// Caracter marcado: Nova linha
        /// </summary>
        public static char CharNewLine { get; } = '\n';

        /// <summary>
        /// Caracter marcado: Título
        /// </summary>
        public static char CharTitle { get; } = '^';

        /// <summary>
        /// Caracter marcado: Destacado
        /// </summary>
        public static char CharHighlight { get; } = '*';

        /// <summary>
        /// Caracter marcado: Detalhe
        /// </summary>
        public static char CharDetail { get; } = '#';

        /// <summary>
        /// Caracter marcado: Dica
        /// </summary>
        public static char CharHint { get; } = '_';

        /// <summary>
        /// Caracter marcado: Erro
        /// </summary>
        public static char CharError { get; } = '!';

        /// <summary>
        /// Caracter marcado: Pergunta
        /// </summary>
        public static char CharQuestion { get; } = '?';

        /// <summary>
        /// Caracter marcado: Resposta
        /// </summary>
        public static char CharAnswer { get; } = '@';

        private static char[] chars = null;
        /// <summary>
        /// Lista de caracteres especiais.
        /// </summary>
        public static char[] Chars
        {
            get
            {
                if (chars == null) {
                    var list = new List<char>();
                    foreach (var property in typeof(FormaterHelper).GetProperties())
                    {
                        if (typeof(char) == property.PropertyType)
                        {
                            list.Add((char)property.GetValue(null));
                        }
                    }
                    chars = list.ToArray();
                }
                return chars;
            }
        }

        /// <summary>
        /// Solicita a escrita parte por parte formatada.
        /// </summary>
        /// <param name="text">Texto</param>
        /// <param name="write">Função de escrita</param>
        /// <param name="ignoreFormatter">Ignora o formatador. Aplica apenas NewLine.</param>
        public static void Output(string text, Action<string, char> write)
        {
            text = (text + string.Empty).Replace(CharNewLine.ToString(), Environment.NewLine);

            var firstCharIsMark =
                !string.IsNullOrWhiteSpace(text) &&
                Chars.Contains(text[0]) &&
                (text.Length > 1 && text[0] != text[1]);

            if (firstCharIsMark)
            {
                write(text.Substring(1), text[0]);
            }
            else
            {
                write(text, (char)0);
            }
        }
    }
}
