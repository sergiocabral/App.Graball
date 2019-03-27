using System;
using System.Collections.Generic;
using System.Linq;
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
	///     Nova linha: \n  (não é marcador, mas é usado para padronizar NewLine como Environment.NewLine)
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
                            var ch = (char)property.GetValue(null);
                            if (ch != CharNewLine)
                            {
                                list.Add(ch);
                            }
                        }
                    }
                    chars = list.ToArray();
                }
                return chars;
            }
        }

        /// <summary>
        /// Escapa o texto para exibir todos os caracteres de marcadores.
        /// </summary>
        /// <param name="text">Texto.</param>
        /// <returns>Texto.</returns>
        public static string Escape(string text)
        {
            StringBuilder result = new StringBuilder(text);
            foreach (var item in Chars)
            {
                result.Replace(item.ToString(), item.ToString() + item.ToString());
            }
            return result.ToString();
        }

        /// <summary>
        /// Solicita a escrita parte por parte formatada.
        /// </summary>
        /// <param name="text">Texto</param>
        /// <param name="write">Função de escrita</param>
        /// <param name="raw">Ignora o formatador. Aplica apenas NewLine.</param>
        /// <param name="rawMark">Marcador padrão usado para o texto raw.</param>
        public static void Output(string text, Action<string, char> write, bool raw = false, char rawMark = (char)0)
        { 
            if (text == null) { return; }

            text = text.Replace("\r", string.Empty);

            const char NO_MARK = (char)0;
            List<char> marks = new List<char>();
            StringBuilder currentText = new StringBuilder();

            if (raw)
            {
                currentText.Append(Escape(text));
                text = string.Empty;
                marks.Add(rawMark);
            }

            char chr(string str, int index) => string.IsNullOrEmpty(str) || index >= str.Length ? NO_MARK : str[index];
            char lastMask() => marks.Count == 0 ? NO_MARK : marks[marks.Count - 1];
            bool isMark(char mark) => Chars.Contains(mark);

            var i = 0;
            while (i < text.Length)
            {
                var currentChar = text[i];

                if (currentChar == CharNewLine)                     //É marca de nova linha
                {
                    currentText.Append(Environment.NewLine);
                }
                else if (
                    isMark(currentChar) &&                          //É uma marca
                    currentChar == chr(text, i + 1)                 //Marca duplicada
                    )
                {
                    currentText.Append(currentChar);
                    i++;
                }
                else if (
                    isMark(currentChar)                             //É uma marca
                    )
                {
                     if (currentText.Length > 0)                    //Tem texto pendente
                     {
                         write(currentText.ToString(), lastMask());
                         currentText.Clear();
                     }
                     if (currentChar != lastMask())                 //Abre marcação
                     {
                         marks.Add(currentChar);
                     }
                     else                                           //Fecha marcação em aberto
                     {
                         marks.RemoveAt(marks.Count - 1);
                     }
                }
                else
                {
                    currentText.Append(currentChar);
                }

                i++;
            }
            if (currentText.Length > 0)
            {
                write(currentText.ToString(), lastMask());
                currentText.Clear();
            }
        }
    }
}
