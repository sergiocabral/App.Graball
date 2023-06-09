﻿using Graball.Business;
using Graball.Business.IO;
using Graball.Business.Module;
using Graball.General.Reflection;
using Graball.General.Text;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Graball
{
    /// <summary>
    /// Classe principal.
    /// </summary>
    public sealed class Program: ModuleBase
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
            SetInput(new InputManager());
            SetOutput(new OutputManager() { Prevent = true });

            LoadTranslate(CultureInfo.CurrentUICulture.Name);
            ExtractAssemblies();
            LoadModules();
            Run();
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
                        var bytes = assembly.GetResourceBinary(resource);
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
            var resource = Assembly.GetExecutingAssembly().GetResourceString("Translates.json");
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
                            Output.WriteLine($"#{Phrases.FILE_LOADED_ASSEMBLY.Translate()}", assembly.GetDescription());
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
                instance.SetOutput(Output);
                instance.SetInput(Input);
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
        /// Referência para o assembly da instância.
        /// </summary>
        protected override Assembly ClassAssembly { get => Assembly.GetExecutingAssembly(); }

        /// <summary>
        /// Recebedor de informações do usuário.
        /// </summary>
        private new InputManager Input { get => base.Input as InputManager; }

        /// <summary>
        /// Exibidor de informações para o usuário.
        /// </summary>
        private new OutputManager Output { get => base.Output as OutputManager; }

        /// <summary>
        /// Contexto do módulo. Apenas com Context vazio aparecem na listagem inicial do programa.
        /// </summary>
        public override string Context { get => "{2FE98BB0-1485-4146-9DC3-D67DDF309E3C}"; }

        /// <summary>
        /// Execução do módulo.
        /// </summary>
        public override void Run()
        {
            Welcome();
            ChooseModule(string.Empty, "Available modules:");
            Output.WriteLine("_{0}", "Finished.".Translate());
#if DEBUG
            System.Console.ReadKey();
#endif
        }

        /// <summary>
        /// Mensagem de boas-vindas.
        /// </summary>
        private void Welcome()
        {

            Output.Prevent = false;

            var space = '#';
            var name = Assembly.GetExecutingAssembly().GetDescription();
            name = new String(space, 3) + " " + name + " " + new String(space, 3);
            var line = new String(space, Console.BufferWidth - name.Length - 2);

            Output.WriteLine("^" + (new String(space, name.Length) + " " + line).Replace(space.ToString(), space.ToString() + space.ToString()));
            Output.WriteLine("^" + (name + " " + line).Replace(space.ToString(), space.ToString() + space.ToString()));
            Output.WriteLine("^" + (new String(space, name.Length) + " " + line).Replace(space.ToString(), space.ToString() + space.ToString()));
            Output.WriteLine();

            if (!Output.Flushed)
            {
                Output.QueueFlush();
                Output.WriteLine();
            }
        }
    }
}
