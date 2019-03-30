using Graball.General.Text;
using Graball.Module.Domains.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Graball.Module.Domains.Data
{
    /// <summary>
    /// Class para campos de tabela: Domain
    /// </summary>
    public class EntityDomain : EntityDomainInterface
    {
        private string fullname;
        /// <summary>
        /// Nome completo do domínio.
        /// </summary>
        public virtual string Fullname {
            get => fullname;
            set => fullname = value == null ? value : value.Trim().ToLower();
        }

        /// <summary>
        /// Nome antes do primeiro ponto.
        /// </summary>
        public virtual string Name { get => Regex.Match(Fullname, @"^[^\.]*", RegexOptions.IgnoreCase).Value; }

        /// <summary>
        /// Sufixo apos o primeiro ponto.
        /// </summary>
        public virtual string Suffix { get => Regex.Match(Fullname, @"\..*", RegexOptions.IgnoreCase).Value; }

        /// <summary>
        /// Sufixo depois do último ponto.
        /// </summary>
        public virtual string TLD { get => Regex.Match(Fullname, @"\.[^\.]*$", RegexOptions.IgnoreCase).Value; }

        /// <summary>
        /// Comprimento do nome.
        /// </summary>
        public virtual int Length { get => Name.Length; }

        /// <summary>
        /// Status de disponibilidade.
        /// </summary>
        public virtual Domain.Status Status { get; set; }

        /// <summary>
        /// Última atualização na tabela.
        /// </summary>
        public virtual DateTime Updated { get; set; } = DateTime.Now;

        /// <summary>
        /// Representação da instância como texto.
        /// </summary>
        /// <returns>Texto.</returns>
        public override string ToString()
        {
            var padding = 13;
            padding = this.Fullname.Length > padding ? this.Fullname.Length + 2 : padding;
            StringBuilder result = new StringBuilder();
            result.AppendLine(string.Format("{0}", this.Fullname.ToUpper().PadLeft(padding)));
            result.AppendLine(string.Format("{0}", new String('^', this.Fullname.Length).PadLeft(padding)));
            result.AppendLine(string.Format("{0}: {1}", "Name".Translate().PadLeft(padding), this.Name));
            result.AppendLine(string.Format("{0}: {1}", "Suffix".Translate().PadLeft(padding), this.Suffix));
            result.AppendLine(string.Format("{0}: {1}", "TLD".Translate().PadLeft(padding), this.TLD));
            result.AppendLine(string.Format("{0}: {1}", "Length".Translate().PadLeft(padding), this.Length));
            result.AppendLine(string.Format("{0}: {1}", "Status".Translate().PadLeft(padding), this.Status));
            result.AppendLine(string.Format("{0}: {1:yyyy-MM-dd HH:mm:ss}", "Updated".Translate().PadLeft(padding), this.Updated));

            return result.ToString();
        }
    }
}
