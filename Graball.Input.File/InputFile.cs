using Graball.Business;
using Graball.Business.IO;
using Graball.General.IO;
using Graball.General.Reflection;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Graball.Input.File
{
    /// <summary>
    /// Input pela janela de console.
    /// </summary>
    public class InputFile : InputBase
    {
        /// <summary>
        /// Construtor.
        /// </summary>
        public InputFile()
        {
            Filename = new FileInfo(Path.Combine(Definitions.DirectoryForUserData.FullName, this.GetType().GetNamespace() + ".txt"));
            Filename.CreateEmpty();
            Filename.OpenWrite().Close();
        }

        public FileInfo Filename { get; }

        /// <summary>
        /// Intervalo entre verificações de HasRead(). (para evitar acesso a disco muito frequente).
        /// </summary>
        public int Interval { get; set; } = 1000;

        /// <summary>
        /// Temporizador para evitar tentativas muito próximas.
        /// </summary>
        private Stopwatch Stopwatch { get; } = new Stopwatch();

        /// <summary>
        /// Recebe uma entrada do usuário.
        /// </summary>
        /// <returns>Entrada do usuário</returns>
        public override string Read()
        {
            Stopwatch.Stop();
            Filename.Refresh();
            if (Filename.Exists && Filename.Length > 0)
            {
                var lines = System.IO.File.ReadAllLines(Filename.FullName).Where(a => !string.IsNullOrWhiteSpace(a)).ToList();
                if (lines.Count > 0)
                {
                    var answer = lines[0];
                    lines.RemoveAt(0);
                    System.IO.File.WriteAllLines(Filename.FullName, lines.ToArray());
                    return answer;
                }
            }
            return null;
        }

        /// <summary>
        /// Resultado da última veriicação de HasRead()
        /// </summary>
        private bool lastHasRead = false;

        /// <summary>
        /// Verifica se possui resposta prévia.
        /// </summary>
        public override bool HasRead()
        {
            if (!Stopwatch.IsRunning || Stopwatch.ElapsedMilliseconds > Interval)
            {
                Stopwatch.Restart();
                Filename.Refresh();
                if (Filename.Exists && Filename.Length > 0)
                {
                    lastHasRead = 0 < System.IO.File.ReadAllLines(Filename.FullName).Where(a => !string.IsNullOrWhiteSpace(a)).Count();
                }
                else
                {
                    lastHasRead = false;
                }
            }
            return lastHasRead;
        }

        /// <summary>
        /// Solicita uma tecla do usuário para continuar.
        /// </summary>
        /// <returns>Caracter recebido.</returns>
        public override char ReadKey()
        {
            var read = Read();
            return read.Length > 0 ? read[0] : (char)0;
        }
    }
}
