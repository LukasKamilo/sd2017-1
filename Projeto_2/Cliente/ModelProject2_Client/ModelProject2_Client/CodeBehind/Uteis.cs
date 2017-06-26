using ModelProject2_Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Thrift.Protocol;
using Thrift.Transport;
using thriftGrafo;

namespace ModelProject2_Server.CodeBehind
{
    public class Uteis
    {

        /// <summary>
        /// Calcula o Hash MD5 de uma string
        /// </summary>
        /// <param name="strEntrada">String para calcular o hash</param>
        /// <returns>O hash MD5 da string</returns>
        public static string GeraHashMD5(string strEntrada)
        {
            if (String.IsNullOrEmpty(strEntrada))
            {
                return "";
            }

            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(strEntrada));

                StringBuilder sBuilder = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                return sBuilder.ToString();
            }
        }

        /// <summary>
        /// Calcula o servidor que será destinado dado a string codificada
        /// </summary>
        /// <param name="dado">String para efeutar o calculo</param>
        /// <returns>O identificador do servidor</returns>
        public static int CalcularModHash(string dado)
        {
            int soma = 0;

            for (var i = 0; i < 4; i++)
            {
                soma += Convert.ToInt16(dado[i]);
            }

            return soma % VariaveisGlobais.N_Servidores;
        }

        /// <summary>
        /// Calcula o servidor que será destinado
        /// </summary>
        /// <param name="valor">String para efeutar o calculo</param>
        /// <returns>O identificador do servidor</returns>
        public static int GetServidor(string valor)
        {
            string dado = GeraHashMD5(valor);

            return CalcularModHash(dado);
        }

        public static bool IniciarServidores(out string mensagemErro)
        {
            mensagemErro = "";

            if(VariaveisGlobais.servidor1.Identificador != 0)
            {
                try
                {
                    TTransport transport = new TSocket(VariaveisGlobais.servidor1.IP, Convert.ToInt32(VariaveisGlobais.servidor1.Porta));
                    transport.Open();
                    TProtocol protocol = new TBinaryProtocol(transport);
                    VariaveisGlobais.client_servidor1 = new thriftGrafo.GrafoService.Client(protocol);
                }
                catch (Exception ex)
                {
                    mensagemErro = "Não foi possível iniciar o servidor 1. " + ex.Message;
                    return false;
                }
            }

            if (VariaveisGlobais.servidor2.Identificador != 0)
            {
                try
                {
                    TTransport transport = new TSocket(VariaveisGlobais.servidor2.IP, Convert.ToInt32(VariaveisGlobais.servidor2.Porta));
                    transport.Open();
                    TProtocol protocol = new TBinaryProtocol(transport);
                    VariaveisGlobais.client_servidor2 = new thriftGrafo.GrafoService.Client(protocol);
                }
                catch (Exception ex)
                {
                    mensagemErro = "Não foi possível iniciar o servidor 2. " + ex.Message;
                    return false;
                }
            }

            if (VariaveisGlobais.servidor3.Identificador != 0)
            {
                try
                {
                    TTransport transport = new TSocket(VariaveisGlobais.servidor3.IP, Convert.ToInt32(VariaveisGlobais.servidor3.Porta));
                    transport.Open();
                    TProtocol protocol = new TBinaryProtocol(transport);
                    VariaveisGlobais.client_servidor3 = new thriftGrafo.GrafoService.Client(protocol);
                }
                catch (Exception ex)
                {
                    mensagemErro = "Não foi possível iniciar o servidor 3. " + ex.Message;
                    return false;
                }
            }

            mensagemErro = "Servidores conectados com sucesso!";

            return true;
        }

        public static void startAguarde()
        {
            Thread viewThread = new Thread(delegate ()
            {
                VariaveisGlobais.wait = new Aguarde();
                VariaveisGlobais.wait.Show();
                System.Windows.Threading.Dispatcher.Run();
            });

            viewThread.SetApartmentState(ApartmentState.STA);
            viewThread.Start();
        }

        public static void stopAguarde()
        {
            VariaveisGlobais.wait.Dispatcher.InvokeShutdown();
        }
    }
}
