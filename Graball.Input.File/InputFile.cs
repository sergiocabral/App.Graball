using Graball.Business;
using Graball.Business.IO;
using Graball.General.IO;
using Graball.General.Reflection;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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
        /// Recebe uma entrada do usuário.
        /// </summary>
        /// <returns>Entrada do usuário</returns>
        public override string Read()
        {
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
        /// Verifica se possui resposta prévia.
        /// </summary>
        public override bool HasRead()
        {
            Filename.Refresh();
            if (Filename.Exists && Filename.Length > 0)
            {
                return 0 < System.IO.File.ReadAllLines(Filename.FullName).Where(a => !string.IsNullOrWhiteSpace(a)).Count();
            }
            return false;
        }
    }
}
