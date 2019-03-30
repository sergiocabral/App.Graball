using Graball.General.Reflection;
using Graball.General.Text;
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
            Undefined,
            Registered,
            Available,
            WaitingRelease
        }

        /// <summary>
        /// Verifica o status de um domínio.
        /// </summary>
        /// <param name="whois">Informações do WHOIS</param>
        /// <returns>Status</returns>
        public static Status GetStatus(string whois)
        {
            var keys = ExtractWhoisKeys(whois);
            if (keys.ContainsKey("domain name") || keys.ContainsKey("owner"))
            {
                return Status.Registered;
            }
            else if (whois.IndexOf("waiting") >= 0)
            {
                return Status.WaitingRelease;
            }
            else
            {
                return Status.Available;
            }
        }

        /// <summary>
        /// Consulta o Whois para um domínio.
        /// </summary>
        /// <param name="domain">Domínio.</param>
        /// <param name="server">Responsável pelo domínio.</param>
        /// <param name="port">Porta de conexão.</param>
        /// <returns>Status</returns>
        public static string WhoisRaw(string domain, string server = null, int port = 43)
        {
            StringBuilder result = new StringBuilder();

            result.AppendLine();
            result.AppendLine(new String('#', 60));
            result.AppendLine(string.Format("{0}:{1} - {2}", server, port, domain));
            result.AppendLine(new String('#', 60));
            result.AppendLine();

            try
            {
                using (var stream = new TcpClient(server, port).GetStream())
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
            }
            catch (Exception ex)
            {
                result.AppendLine($"{"Fail".Translate().ToUpper()}: {ex.Message}");
            }

            return result.ToString();
        }

        /// <summary>
        /// Consulta o Whois para um domínio.
        /// </summary>
        /// <param name="domain">Domínio.</param>
        /// <param name="acceptRedirect">Opcional. Quando true aceita redirecionamento para outros WHOIS com mais informações.</param>
        /// <returns>Status</returns>
        public static string Whois(string domain, bool acceptRedirect = true)
        {
            var suffix1 = domain.Substring(domain.IndexOf(".") + 1);
            var suffix2 = suffix1.Substring(suffix1.IndexOf(".") + 1);
            var server = WhoisServers.ContainsKey(suffix1) ? WhoisServers[suffix1] : WhoisServers.ContainsKey(suffix2) ? WhoisServers[suffix2] : null;

            if (server == null) { return null; }

            var whois = WhoisRaw(domain, server);

            if (whois == null)
            {
                return null;
            }

            var keys = ExtractWhoisKeys(whois);

            if (keys.ContainsKey("registrar whois server"))
            {
                whois += Environment.NewLine + WhoisRaw(domain, keys["registrar whois server"][0]);
            }

            return whois;
        }

        /// <summary>
        /// Extrai as chaves de Whois
        /// </summary>
        /// <param name="whois">Texto whois.</param>
        /// <returns>Chaves Whois</returns>
        public static IDictionary<string, IList<string>> ExtractWhoisKeys(string whois)
        {
            if (whois == null) { return null; }

            var result = new Dictionary<string, IList<string>>();
            foreach (var lineRaw in whois.Split('\n'))
            {
                var line = lineRaw.Trim();
                if (!Regex.IsMatch(line, @"^\w+.*:.*$")) { continue; }
                var key = line.Substring(0, line.IndexOf(":")).Trim().ToLower();
                var value = line.Substring(line.IndexOf(":") + 1).Trim();
                if (!result.ContainsKey(key))
                {
                    result[key] = new List<string>();
                }
                result[key].Add(value);
            }
            return result;
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
