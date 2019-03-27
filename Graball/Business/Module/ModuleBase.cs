using Graball.Business.IO;
using Graball.General.Reflection;
using Graball.General.Text;
using System;
using System.Collections;
using System.Collections.Generic;
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
                    translates = Regex.Match(this.GetType().FullName, @".*(?=\.[^\.]*$)").Value;
                    translates = ClassAssembly.GetResourceString(translates + ".Properties.Translates.json");
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
        /// <returns>Auto referência para a lista passada.</returns>
        public KeyValuePair<int, T> ChoseOption<T>(IList<T> options, string title)
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
        /// Execução do módulo.
        /// </summary>
        public abstract void Run();
    }
}
