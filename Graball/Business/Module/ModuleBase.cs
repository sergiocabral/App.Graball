using Graball.Business.IO;
using Graball.General.Data;
using Graball.General.IO;
using Graball.General.Reflection;
using Graball.General.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Graball.Business.Module
{
    /// <summary>
    /// Class base para módulos do sistema.
    /// </summary>
    public abstract class ModuleBase: ModuleInterface
    {
        /// <summary>
        /// Construtor.
        /// </summary>
        public ModuleBase()
        {
            AllModules.Add(this);
        }

        /// <summary>
        /// Lista de todos os módulos instanciados.
        /// </summary>
        public static IList<ModuleInterface> AllModules { get; } = new List<ModuleInterface>();

        /// <summary>
        /// Contexto do módulo. Apenas com Context vazio aparecem na listagem inicial do programa.
        /// </summary>
        public virtual string Context { get => string.Empty; }

        /// <summary>
        /// Referência para o assembly da instância.
        /// </summary>
        protected abstract Assembly ClassAssembly { get; }

        private string name = null;
        /// <summary>
        /// Nome de apresentação.
        /// </summary>
        public virtual string Name
        {
            get
            {
                if (name == null)
                {
                    name = Regex.Match(this.GetType().FullName, @"[^\.]*(?=\.[^\.]*$)").Value;
                }
                return name;
            }
        }

        public string translates = null;
        /// <summary>
        /// Traduções em formato JSON.
        /// </summary>
        public virtual string Translates
        {
            get
            {
                if (translates == null)
                {
                    translates = ClassAssembly.GetResourceString("Translates.json");
                }
                return translates;
            }
        }

        /// <summary>
        /// Output padrão.
        /// </summary>
        protected OutputInterface Output { get; private set; }

        /// <summary>
        /// Define o output padrão.
        /// </summary>
        /// <param name="output">Instância.</param>
        public void SetOutput(OutputInterface output) => Output = output;

        /// <summary>
        /// Input padrão.
        /// </summary>
        protected InputInterface Input { get; private set; }

        /// <summary>
        /// Define o input padrão.
        /// </summary>
        /// <param name="input">Instância.</param>
        public void SetInput(InputInterface input) => Input = input;

        private IniFile iniFile = null;
        /// <summary>
        /// Banco de dados.
        /// </summary>
        protected IniFile IniFile
        {
            get
            {
                if (iniFile == null)
                {
                    var file = new FileInfo(Path.Combine(Definitions.DirectoryForUserData.FullName, Assembly.GetExecutingAssembly().GetName().Name + ".ini"));
                    iniFile = new IniFile(file.FullName, this.GetType().FullName);
                }
                return iniFile;
            }
            set => iniFile = value;
        }

        private SQLite sqlite = null;
        /// <summary>
        /// Banco de dados.
        /// </summary>
        protected SQLite SQLite
        {
            get
            {
                if (sqlite == null)
                {
                    var file = new FileInfo(Path.Combine(Definitions.DirectoryForUserData.FullName, this.GetType().GetNamespace() + ".sqlite"));
                    sqlite = new SQLite(file.FullName);
                    if (SQLiteStructures != null)
                    {
                        foreach (var structure in SQLiteStructures)
                        {
                            sqlite.AddStructure(structure);
                        }
                    }
                }
                return sqlite;
            }
            set => sqlite = value;
        }

        /// <summary>
        /// Lista de ações para ajustes na estrutura e invremento da versão do banco.
        /// </summary>
        protected virtual IList<Action<SQLiteConnection>> SQLiteStructures { get; } = null;

        /// <summary>
        /// Mensagem pronta para "Em desenvolvimento"
        /// </summary>
        protected void NotImplemented()
        {
            Output.WriteLine($"#{Phrases.NOT_IMPLEMENTED.Translate()}").WriteLine();
            Output.Write($"?{Phrases.PRESS_ANY_KEY.Translate()}");
            Input.ReadKey();
            Output.WriteLine().WriteLine();
        }

        /// <summary>
        /// Formata uma lista para exibição.
        /// </summary>
        /// <param name="options">Lista de opções.</param>
        /// <param name="format">opcional. Formato de exibição.</param>
        /// <returns>Lista como texto.</returns>
        public string Options<T>(IList<T> options, string format = " {1}) {0}")
        {
            StringBuilder result = new StringBuilder();
            var padding = options.Count.ToString().Length;
            var i = 0;
            foreach (var option in options)
            {
                string text = null;
                if (typeof(T) != typeof(string) && typeof(IEnumerable).IsInstanceOfType(option))
                {
                    foreach (var first in (IEnumerable)option)
                    {
                        text = Convert.ToString(first);
                        break;
                    }
                }
                else
                {
                    text = Convert.ToString(option);
                }
                result.Append(string.Format(" {1}) {0}\n", text.Translate(), (++i).ToString().PadLeft(padding)));
            }
            return result.ToString();
        }
        
        /// <summary>
        /// Exibe um lista para seleção.
        /// </summary>
        /// <typeparam name="T">Tipo do conteúdo da lista.</typeparam>
        /// <param name="options">Lista de opções.</param>
        /// <param name="title">Título.</param>
        /// <returns>Resposta com índice e opção selecionada. Índice -1 para nenhuma seleção.</returns>
        public KeyValuePair<int, T> ChooseOption<T>(IList<T> options, string title)
        {
            IList<string> getNames(T option)
            {
                var names = new List<string>();
                if (typeof(T) != typeof(string) && typeof(IEnumerable).IsInstanceOfType(option))
                {
                    foreach (var name in (IEnumerable)option)
                    {
                        names.Add(Convert.ToString(name).Translate());
                    }
                }
                else
                {
                    names.Add(Convert.ToString(option).Translate());
                }
                return names;
            }

            var optionsAsText = Options<T>(options);
            var result = new KeyValuePair<int, T>(-1, default(T));
            string answer;
            do
            {
                Output.WriteLine(title.Translate());
                Output.WriteLine(optionsAsText);
                Output.Write($"?{Phrases.CHOOSE_ONE.Translate()}");

                answer = Input.Read();

                if (!string.IsNullOrWhiteSpace(answer))
                {
                    if (Regex.IsMatch(answer, @"^[0-9]*$") && int.Parse(answer) > 0 && int.Parse(answer) <= options.Count)
                    {
                        result = new KeyValuePair<int, T>(int.Parse(answer) - 1, options[int.Parse(answer) - 1]);
                    }
                    else
                    {
                        var index = new List<int>();
                        var answerSlug = answer.Slug();
                        for (int i = 0; i < options.Count; i++)
                        {
                            if (getNames(options[i]).Count(a => a.Slug().IndexOf(answerSlug) == 0) > 0)
                            {
                                index.Add(i);
                            }
                        }
                        if (index.Count == 1)
                        {
                            result = new KeyValuePair<int, T>(index[0], options[index[0]]);
                        }
                    }
                    if (result.Key >= 0)
                    {
                        Output.WriteLine("@{1}) {0}", getNames(options[result.Key])[0], result.Key + 1).WriteLine();
                    }
                    else
                    {
                        Output.WriteLine($"@{answer}").WriteLine();
                        Output.WriteLine($"!{Phrases.CHOOSE_WRONG}").WriteLine();
                    }
                }
                else
                {
                    Output.WriteLine($"_{Phrases.CHOOSE_BLANK.Translate()}").WriteLine();
                }
            } while (!string.IsNullOrWhiteSpace(answer) && result.Key < 0);
            return result;
        }

        /// <summary>
        /// Exibe uma lista para seleção.
        /// </summary>
        /// <param name="options">Opções.</param>
        /// <param name="title">Título.</param>
        public void ChooseOption(IDictionary<string, Action> options, string title = Phrases.OPERATIONS)
        {
            do
            {
                var option = ChooseOption<string>(options.Select(a => a.Key).ToArray(), title);
                if (option.Key == -1)
                {
                    return;
                }
                else
                {
                    options[option.Value]();
                }
            } while (true);
        }

        /// <summary>
        /// Exibe uma lista de módulos para seleção.
        /// </summary>
        /// <param name="context">Contexto dos módulos para exibição.</param>
        /// <param name="title">Título.</param>
        public void ChooseModule(string context, string title = Phrases.RESOURCES)
        {
            do
            {
                var option = ChooseOption(ModuleBase.AllModules.Where(a => a.Context == context).Select(a => a.Name).ToList(), title);
                if (option.Key == -1)
                {
                    return;
                }
                else
                {
                    ModuleBase.AllModules.Single(a => a.Name == option.Value).Run();
                }
            } while (true);
        }

        /// <summary>
        /// Execução do módulo.
        /// </summary>
        public abstract void Run();
    }
}
