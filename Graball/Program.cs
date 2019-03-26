
using Graball.Business;
using Graball.Business.IO;
using Graball.Business.Module;
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
            Output.Prevent = true;
            LoadTranslate(CultureInfo.CurrentUICulture.Name);
            ExtractAssemblies();
            LoadModules();
            Run();

#if DEBUG
            System.Console.ReadKey();
#endif
        }

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
                            Output.WriteLine($"!{Phrases.FILE_DELETE_ERROR.Translate()}", file.Name);
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
                            Output.WriteLine($"!{Phrases.FILE_WRITE_ERROR.Translate()}", file.Name);
                        }
                    }
                }
            }
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
                        Output.WriteLine($"!{Phrases.FILE_CONTENT_INVALID.Translate()}", file.Name);
                        Output.WriteLine($"!{ex.Message}");
                    }
                }
                catch (Exception ex)
                {
                    Output.WriteLine($"!{Phrases.FILE_LOAD_ERROR.Translate()}", file.Name);
                    Output.WriteLine($"!{ex.Message}");
                }
            }
        }

        /// <summary>
        /// Carrega um assembly de um arquivo e instancia a class.
        /// </summary>
        /// <typeparam name="T">Tipo para instanciar.</typeparam>
        /// <param name="fileMask">Filtro para localizar os arquivos.</param>
        /// <param name="verbose">Quando true, exibe mensagem sobre a criação da instância.</param>
        /// <param name="action">Método que recebe a instância.</param>
        private void LoadAndCreate<T>(string fileMask, bool verbose, Action<T> action)
        {
            foreach (var file in Definitions.DirectoryForExecutable.GetFiles(fileMask))
            {
                var assembly = AssemblyHelper.Load(file.FullName);
                try
                {
                    var instances = AssemblyHelper.Load(assembly, typeof(T));
                    if (instances.Length > 0)
                    {
                        foreach (var instance in instances)
                        {
                            action((T)instance);
                        }
                        if (verbose)
                        {
                            Output.WriteLine($"#{Phrases.FILE_LOADED_ASSEMBLY.Translate()}", assembly.Description());
                        }
                    }
                    else if (verbose)
                    {
                        throw new Exception();
                    }
                }
                catch
                {
                    Output.WriteLine($"!{Phrases.FILE_LOAD_ERROR.Translate()}", file.Name);
                }
            }
        }

        /// <summary>
        /// Carrega os módulos de entrada e saída de informações de/para o usuário.
        /// </summary>
        private void LoadModules()
        {
            LoadAndCreate<OutputInterface>(Definitions.FileMaskForOutput, true, (OutputInterface instance) => Output.Add(instance));
            LoadAndCreate<InputInterface>(Definitions.FileMaskForInput, true, (InputInterface instance) => Input.Add(instance));

            void loadModule(ModuleInterface instance)
            {
                Modules.Add(instance);
                if (!string.IsNullOrWhiteSpace(instance.Translates))
                {
                    Translate.LoadAll(instance.Translates);
                }
            }

            LoadAndCreate<ModuleInterface>(Definitions.FileMaskForOutput, false, loadModule);
            LoadAndCreate<ModuleInterface>(Definitions.FileMaskForInput, false, loadModule);
            LoadAndCreate<ModuleInterface>(Definitions.FileMaskForModule, true, loadModule);
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
        /// Lista de módulos para execução.
        /// </summary>
        private IList<ModuleInterface> Modules { get; } = new List<ModuleInterface>();

        /// <summary>
        /// Funcionamento do programa.
        /// </summary>
        private void Run()
        {
            do
            {
                Welcome();

                var d1 = Modules[1].Name;
                var d2 = Modules[1].Translates;

                Output.WriteLine("_*teste* de codigo_ Qual #é#?? Meu chapa.").WriteLine();
                Output.WriteLine("*Testando* essas implementações, ???hein??? - @Uau!!@").WriteLine();

            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }

        /// <summary>
        /// Mensagem de boas-vindas.
        /// </summary>
        private void Welcome()
        {
            Output.Prevent = false;

            var name = Assembly.GetExecutingAssembly().Description();
            Output.WriteLine("^+-{0}-+", new String('-', name.Length));
            Output.WriteLine("^| {0} |", name);
            Output.WriteLine("^+-{0}-+", new String('-', name.Length));
            Output.WriteLine();

            if (!Output.Flushed)
            {
                Output.QueueFlush();
                Output.WriteLine();
            }
        }
    }
}
