using Graball.Business.IO;
using System;

namespace Graball.Output.Console
{
    /// <summary>
    /// Output para janela de console.
    /// </summary>
    public class OutputConsole : OutputBaseWithFormater
    {
        /// <summary>
        /// Valor inicial (padrão) para ForegroundColor.
        /// </summary>
        public ConsoleColor DefaultForegroundColor { get; } = System.Console.ForegroundColor;

        /// <summary>
        /// Valor inicial (padrão) para BackgroundColor.
        /// </summary>
        public ConsoleColor DefaultBackgroundColor { get; } = System.Console.BackgroundColor;

        /// <summary>
        /// Solicita a escrita imediata.
        /// </summary>
        /// <param name="text">Texto.</param>
        /// <param name="mark">Marcador.</param>
        protected override void WriteNow(string text, char mark)
        {
            SetColors(mark);
            foreach (var ch in text)
            {
                if (ch == '\b')
                {
                    System.Console.Write("\b \b");
                }
                else
                {
                    System.Console.Write(ch);
                }
            }
            SetColors();
        }

        /// <summary>
        /// Define as cores de acordo com a marcação.
        /// </summary>
        /// <param name="mark"></param>
        private void SetColors(char mark = (char)0)
        {
            if (mark == FormaterHelper.CharTitle)
            {
                System.Console.ForegroundColor = ConsoleColor.White;
                System.Console.BackgroundColor = DefaultBackgroundColor;
            }
            else if (mark == FormaterHelper.CharHighlight)
            {
                System.Console.ForegroundColor = ConsoleColor.DarkCyan;
                System.Console.BackgroundColor = DefaultBackgroundColor;
            }
            else if (mark == FormaterHelper.CharDetail)
            {
                System.Console.ForegroundColor = ConsoleColor.DarkGray;
                System.Console.BackgroundColor = DefaultBackgroundColor;
            }
            else if (mark == FormaterHelper.CharHint)
            {
                System.Console.ForegroundColor = ConsoleColor.Magenta;
                System.Console.BackgroundColor = DefaultBackgroundColor;
            }
            else if (mark == FormaterHelper.CharError)
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.BackgroundColor = DefaultBackgroundColor;
            }
            else if (mark == FormaterHelper.CharQuestion)
            {
                System.Console.ForegroundColor = ConsoleColor.DarkYellow;
                System.Console.BackgroundColor = DefaultBackgroundColor;
            }
            else if (mark == FormaterHelper.CharAnswer)
            {
                System.Console.ForegroundColor = ConsoleColor.Yellow;
                System.Console.BackgroundColor = DefaultBackgroundColor;
            }
            else
            {
                System.Console.ForegroundColor = DefaultForegroundColor;
                System.Console.BackgroundColor = DefaultBackgroundColor;
            }
        }
    }
}
