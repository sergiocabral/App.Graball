using Graball.General.Reflection;
using Graball.General.Text;
using Graball.General.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            Undefined = 0,
            Registered = 1,
            Available = 2,
            WaitingRelease = 4,
            Reserved = 8
        }

        /// <summary>
        /// Formas de obter Whois.
        /// </summary>
        public enum WhoisService
        {
            Normal,
            WebsiteWhoisCom,
            WebsiteInstantDomainSearchCom,
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

        /// <summary>
        /// Calcula o próximo domínio.
        /// </summary>
        /// <param name="domain">Domínio.</param>
        /// <returns>Nome de domínio.</returns>
        public static string Next(string domain)
        {
            var result = new StringBuilder();

            var chars = domain.ToLower().ToCharArray();
            Array.Reverse(chars);

            var overflow = true;
            foreach (var c in chars)
            {
                var letter = !overflow ? c : (char)((byte)c + 1);

                if (letter == (byte)'z' + 1)
                {
                    letter = '0';
                }

                overflow = letter == (byte)'9' + 1;
                if (overflow)
                {
                    letter = 'a';
                }

                result.Insert(0, letter);
            }
            if (overflow)
            {
                result.Insert(0, 'a');
            }

            return result.ToString();
        }

        /// <summary>
        /// Verifica o status de um domínio.
        /// </summary>
        /// <param name="whois">Informações do WHOIS</param>
        /// <returns>Status</returns>
        public static Status GetStatus(string whois)
        {
            var keys = ExtractWhoisKeys(whois);
            if (keys.ContainsKey("domain name") || keys.ContainsKey("owner") || whois.IndexOf("\"availability\":\"registered\"") >= 0)
            {
                return Status.Registered;
            }
            else if (whois.IndexOf("release process:") >= 0)
            {
                return Status.WaitingRelease;
            }
            else if (whois.IndexOf("reserved:") >= 0 || whois.IndexOf("\"availability\":\"reserved\"") >= 0)
            {
                return Status.Reserved;
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
        /// <param name="mode">Modo de consulta.</param>
        /// <returns>Whois</returns>
        public static string Whois(string domain, WhoisService mode = WhoisService.Normal)
        {
            switch (mode)
            {
                case WhoisService.Normal:
                    return WhoisByServer(domain, true);
                case WhoisService.WebsiteWhoisCom:
                    return WhoisByWebsiteWhoisCom(domain);
                case WhoisService.WebsiteInstantDomainSearchCom:
                    return WhoisByWebsiteInstantDomainSearchCom(domain);
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Consulta Whois. Usa conexão em servidor WHOIS
        /// </summary>
        /// <param name="domain">Domínio.</param>
        /// <param name="server">Servidor que responde pelo domínio.</param>
        /// <param name="port">Porta de conexão.</param>
        /// <returns>Retorn do Whois.</returns>
        public static string WhoisByServer(string domain, string server = null, int port = 43)
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
        /// Consulta Whois. Usa conexão em servidor WHOIS
        /// </summary>
        /// <param name="domain">Domínio.</param>
        /// <param name="acceptRedirect">Quando true aceita redirecionamento para outros WHOIS com mais informações.</param>
        /// <returns>Retorn do Whois.</returns>
        public static string WhoisByServer(string domain, bool acceptRedirect)
        {
            var suffix1 = domain.Substring(domain.IndexOf(".") + 1);
            var suffix2 = suffix1.Substring(suffix1.IndexOf(".") + 1);

            var server = WhoisServers.ContainsKey(suffix1) ? WhoisServers[suffix1] : WhoisServers.ContainsKey(suffix2) ? WhoisServers[suffix2] : null;
            if (server == null) { return null; }
            server = server.Split(',')[0];

            var whois = WhoisByServer(domain, server);

            if (whois == null)
            {
                return null;
            }

            var keys = ExtractWhoisKeys(whois);

            if (keys.ContainsKey("registrar whois server"))
            {
                whois += Environment.NewLine + WhoisByServer(domain, keys["registrar whois server"][0]);
            }

            return whois;
        }

        /// <summary>
        /// Consulta Whois. Usa o website whois.com
        /// Usa Captcha
        /// </summary>
        /// <param name="domain">Domínio.</param>
        /// <returns>Retorn do Whois.</returns>
        public static string WhoisByWebsiteWhoisCom(string domain)
        {
            var client = new WebClientWithCookie();
            var result = client.Load($"https://www.whois.com/whois/{domain}");
            var whois = Regex.Match(result.Html, @"(?<=\<pre[^\>]*\>).*(?=\</pre>)", RegexOptions.Singleline).Value;
            return string.IsNullOrWhiteSpace(whois) ? null : whois;
        }

        /// <summary>
        /// Consulta Whois. Usa o website instantdomainsearch.com
        /// Usa Captcha
        /// </summary>
        /// <param name="domains">Domínio.</param>
        /// <returns>Retorn do Whois.</returns>
        public static string WhoisByWebsiteInstantDomainSearchCom(params string[] domains)
        {
            string hash(string e, int t) {
                int n, r, o;
                for (n = t, r = e.Length, o = 0; o < r; o += 1)
                {
                    n = (n << 5) - n + (byte)e[o];
                    n &= n;
                }
                return n.ToString();
            }

            var name = domains[0].Substring(0, domains[0].IndexOf("."));
            var suffix = domains[0].Substring(domains[0].IndexOf(".") + 1);
            var names = string.Join(',', domains.Select(a => a.Substring(0, (a + ".").IndexOf("."))).ToArray());

            var client = new WebClientWithCookie();
            var result = client.Load($"https://check.instantdomainsearch.com/bulk/?names={names}&tlds={suffix}&hash={hash(domains[0], 27)}");
            return string.IsNullOrWhiteSpace(result.Html) ? null : result.Html;
        }
        
    }
}
