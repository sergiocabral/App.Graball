using System;
using System.Collections.Generic;
using System.Text;

namespace Graball.Business.IO
{
    /// <summary>
    /// Classe base para gerenciadores de Input ou Output.
    /// </summary>
    public abstract class ManagerBase<T>: ManagerInterface<T>
    {
        protected IList<T> Items { get; } = new List<T>();

        /// <summary>
        /// Adiciona um item gerenciado.
        /// </summary>
        /// <param name="item">Item gerenciado.</param>
        public void Add(T item)
        {
            Items.Add(item);
        }
    }
}
