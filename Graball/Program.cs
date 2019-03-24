using Graball.Business;
using Graball.Business.IO;
using Graball.General.Reflection;
using Graball.General.Text;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Graball
{
    /// <summary>
    /// Classe principal.
    /// </summary>
    public sealed class Program
    {
        /// <summary>
        /// Método de entrada do sistema operacional.
        /// </summary>
        /// <param name="args">Argumentos de linha de comando.</param>
        static void Main(string[] args) => new Program(args);

        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="args">Argumentos de linha de comando.</param>
        public Program(string[] args)
        {
            ExtractAssemblies();
            LoadInputOutput();
            LoadTranslate(CultureInfo.CurrentUICulture.Name);
        }

        /// <summary>
        /// Recebedor de informações do usuário.
        /// </summary>
        private InputManager Input { get; } = new InputManager();

        /// <summary>
        /// Exibidor de informações para o usuário.
        /// </summary>
        private OutputManager Output { get; } = new OutputManager();

        /// <summary>
        /// Extrai assemblies obrigatórios para a mesma pasta do executável.
        /// </summary>
        private void ExtractAssemblies()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resources = assembly.GetManifestResourceNames();
            foreach (var resource in resources)
            {
                var match = Regex.Match(resource, @"(?<=Properties\.).*\.dll$", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    var file = new FileInfo(Path.Combine(Definitions.DirectoryForExecutable.FullName, match.Value));
                    if (file.Exists && AssemblyHelper.Load(file.FullName) == null)
                    {
                        try
                        {
                            file.Delete();
                            file.Refresh();
                        }
                        catch
                        {
                            Output.Write($"!{Phrases.FILE_DELETE_ERROR}\n", file.Name);
                        }
                    }
                    if (!file.Exists)
                    {
                        var bytes = assembly.ResourceBinary(resource);
                        try
                        {
                            File.WriteAllBytes(file.FullName, bytes);
                        }
                        catch
                        {
                            Output.Write($"!{Phrases.FILE_WRITE_ERROR}\n", file.Name);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Carrega os módulos de entrada e saída de informações de/para o usuário.
        /// </summary>
        private void LoadInputOutput()
        {
            void load<T>(ManagerInterface<T> manager, string mask)
            {
                foreach (var file in Definitions.DirectoryForExecutable.GetFiles(mask))
                {
                    var assembly = AssemblyHelper.Load(file.FullName);
                    try
                    {
                        var instances = AssemblyHelper.Load(assembly, typeof(T));
                        if (instances.Length > 0)
                        {
                            foreach (var instance in instances)
                            {
                                manager.Add((T)instance);
                            }                            
                            Output.Write($"#{Phrases.FILE_LOADED_ASSEMBLY}\n", assembly.Description());
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                    catch
                    {
                        Output.Write($"!{Phrases.FILE_LOAD_ERROR}\n", file.Name);
                    }
                }                
            }
            load<OutputInterface>(Output, Definitions.FileMaskForOutput);
            load<InputInterface>(Input, Definitions.FileMaskForInput);

            Output.QueueFlush();
        }

        /// <summary>
        /// Carrega as traduções de texto.
        /// </summary>
        /// <param name="language">Idioma.</param>
        private void LoadTranslate(string language)
        {
            var resource = Assembly.GetExecutingAssembly().ResourceString("Graball.Properties.Translates.json");
            Translate.LoadAll(resource);

            new Translate(language, true);

            var filename = Definitions.FileMaskForTranslates.Replace("*", Translate.Default.Language);
            var file = new FileInfo(Path.Combine(Definitions.DirectoryForUserData.FullName, filename));

            if (file.Exists)
            {
                try
                {
                    var content = File.ReadAllText(file.FullName);
                    try
                    {
                        Translate.Load(Translate.Default.Language, content);
                    }
                    catch (Exception ex)
                    {
                        Output.Write($"!{Phrases.FILE_CONTENT_INVALID}\n", file.Name);
                        Output.Write($"!{ex.Message}\n");
                    }
                }
                catch (Exception ex)
                {
                    Output.Write($"!{Phrases.FILE_LOAD_ERROR}\n", file.Name);
                    Output.Write($"!{ex.Message}\n");
                }
            }
        }
    }
}
