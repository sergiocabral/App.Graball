using Graball.General.Reflection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace Graball.Module.Domains.Util
{
    /// <summary>
    /// Ferramentas para verificação de domínios.
    /// </summary>
    public static class Domain
    {
        /// <summary>
        /// Status possíveis para um domínio.
        /// </summary>
        public enum Status
        {
            Registered,
            Available
        }

        /// <summary>
        /// Verifica o status de um domínio.
        /// </summary>
        /// <param name="domain">Domínio.</param>
        /// <param name="tld">Responsável pelo domínio.</param>
        /// <returns>Status</returns>
        public static Status GetStatus(string domain, string tld = null)
        {
            var whois = Whois(domain, tld);
            return Regex.IsMatch(whois, @"^owner:", RegexOptions.Multiline) ? Status.Registered : Status.Available;
        }

        /// <summary>
        /// Consulta o Whois para um domínio.
        /// </summary>
        /// <param name="domain">Domínio.</param>
        /// <param name="tld">Responsável pelo domínio.</param>
        /// <returns>Status</returns>
        public static string Whois(string domain, string tld = null)
        {
            StringBuilder result = new StringBuilder();

            if (tld == null)
            {
                tld = Regex.Match(domain, @"\.[^\.]*$").Value;
            }

            using (var stream = new TcpClient(WhoisServers[tld], 43).GetStream())
            using (var buffered = new BufferedStream(stream))
            {
                var writer = new StreamWriter(buffered);
                writer.WriteLine(domain);
                writer.Flush();

                var reader = new StreamReader(buffered);
                string response;
                while ((response = reader.ReadLine()) != null)
                {
                    result.AppendLine(response);
                }
            }

            return result.ToString();
        }

        public static IDictionary<string, string> whoisServers = null;
        /// <summary>
        /// Lista de servidores para Whois.
        /// </summary>
        public static IDictionary<string, string> WhoisServers
        {
            get
            {
                if (whoisServers == null)
                {
                    var json = Assembly.GetExecutingAssembly().GetResourceString("WhoisServers.json");
                    using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                    {
                        var serializer = new DataContractJsonSerializer(typeof(IDictionary<string, string>));
                        whoisServers = (IDictionary<string, string>)serializer.ReadObject(stream);
                    }
                }
                return whoisServers;
            }
        }
    }
}
