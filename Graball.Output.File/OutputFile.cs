using Graball.Business;
using Graball.Business.IO;
using Graball.General.IO;
using Graball.General.Reflection;
using System;
using System.IO;

namespace Graball.Output.File
{
    /// <summary>
    /// Output para janela de console.
    /// </summary>
    public class OutputFile : OutputBaseWithFormater
    {
        /// <summary>
        /// Construtor.
        /// </summary>
        public OutputFile()
        {
            Filename = new FileInfo(Path.Combine(Definitions.DirectoryForUserData.FullName, this.GetType().GetNamespace() + string.Format(".{0:yyyy-MM-dd-HH-mm-ss}.log", DateTime.Now)));
            Filename.CreateEmpty();
        }

        /// <summary>
        /// Arquivo de saída.
        /// </summary>
        public FileInfo Filename { get; }

        /// <summary>
        /// Solicita a escrita imediata.
        /// </summary>
        /// <param name="text">Texto.</param>
        /// <param name="mark">Marcador.</param>
        protected override void WriteNow(string text, char mark)
        {
            using (var stream = new FileStream(Filename.FullName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            {
                stream.Position = stream.Length;
                foreach (var ch in text)
                {
                    if (ch == '\b')
                    {
                        if (stream.Position > 0)
                        {
                            stream.Position--;
                            var chr = (char)stream.ReadByte();
                            if (chr != '\n')
                            {
                                stream.Position--;
                            }
                        }
                    }
                    else
                    {
                        stream.WriteByte((byte)ch);
                    }
                }
                stream.SetLength(stream.Position);
            }
        }
    }
}
