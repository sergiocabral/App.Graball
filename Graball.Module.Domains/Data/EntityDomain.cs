using Graball.Module.Domains.Util;
using System;
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
            return Fullname;
        }
    }
}
