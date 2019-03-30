using Graball.General.Data;
using Graball.General.Text;
using Graball.Module.Domains.Util;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;
using System.Text.RegularExpressions;

namespace Graball.Module.Domains.Data
{
    /// <summary>
    /// Class para campos de tabela: Domain
    /// </summary>
    public class EntityDomain : EntityDomainInterface
    {
        /// <summary>
        /// Construtor.
        /// </summary>
        public EntityDomain() { }

        /// <summary>
        /// Construtor.
        /// </summary>
        public EntityDomain(string domain) : this()
        {
            Fullname = domain;
        }

        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="reader">DataReader</param>
        public EntityDomain(SQLiteDataReader reader) : this()
        {
            Fullname = reader.GetString(reader.GetFieldIndex("Fullname"));
            Status = (Domain.Status)Enum.Parse(typeof(Domain.Status), reader.GetString(reader.GetFieldIndex("Status")));
            Updated = reader.GetDateTime(reader.GetFieldIndex("Updated"));
        }

        private string fullname;
        /// <summary>
        /// Nome completo do domínio.
        /// </summary>
        public virtual string Fullname {
            get => fullname;
            set
            {
                fullname = value == null ? value : value.Trim().ToLower();
                Name = Regex.Match(Fullname, @"^[^\.]*", RegexOptions.IgnoreCase).Value;
                Suffix = Regex.Match(Fullname, @"\..*", RegexOptions.IgnoreCase).Value;
                TLD = Regex.Match(Fullname, @"\.[^\.]*$", RegexOptions.IgnoreCase).Value;
                Length = Name.Length;
            }
        }

        /// <summary>
        /// Nome antes do primeiro ponto.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Sufixo apos o primeiro ponto.
        /// </summary>
        public virtual string Suffix { get; set; }

        /// <summary>
        /// Sufixo depois do último ponto.
        /// </summary>
        public virtual string TLD { get; set; }

        /// <summary>
        /// Comprimento do nome.
        /// </summary>
        public virtual int Length { get; set; }

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

        /// <summary>
        /// Representação da instância como texto.
        /// Formato para tabela.
        /// </summary>
        /// <param name="header">Indica se deve exibir cabeçalho.</param>
        /// <param name="highlightName">Destaque para o nome</param>
        /// <param name="highlightStatus">Destaque para o status.</param>
        /// <returns>Texto.</returns>
        public string ToString(bool header, Domain.Status highlightStatus = Domain.Status.Undefined, string highlightName = null)
        {
            StringBuilder result = new StringBuilder();

            if (header)
            {
                result.AppendLine(string.Format("{0} {1} | {2} | {3}",
                    "#",
                    "Updated".Translate().PadRight(15),
                    "Status".Translate().PadRight(14),
                    "Domain Name".Translate()));
                result.AppendLine(string.Format("{0}-{1}-|-{2}-|-{3}",
                    "#",
                    "".PadRight(15, '-'),
                    "".PadRight(14, '-'),
                    "".PadRight(40, '-')));
            }

            result.AppendLine(string.Format("{0} {1} | {2} | {3}",
                (this.Status & highlightStatus) == highlightStatus ? "" : "#",
                this.Updated.ToString("yyyy-MM-dd").PadRight(15),
                this.Status.ToString().PadRight(14),
                string.IsNullOrWhiteSpace(highlightName) ? this.Fullname.ToLower() : this.Fullname.ToLower().Replace(highlightName.ToLower(), $"*{highlightName.ToLower()}*")));

            return result.ToString();
        }
    }
}
