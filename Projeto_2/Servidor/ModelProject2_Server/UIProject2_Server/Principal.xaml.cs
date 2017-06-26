using ModelProject2_Server.CodeBehind;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using thriftGrafo;

namespace UIProject2_Server
{
    /// <summary>
    /// Interaction logic for Principal.xaml
    /// </summary>
    public partial class Principal : Window
    {
        public Principal()
        {
            InitializeComponent();

            lblHeader.Content = lblHeader.Content + " " + VariaveisGlobais.servidorLocal.Identificador + "   " + VariaveisGlobais.servidorLocal.IP + ":" + VariaveisGlobais.servidorLocal.Porta;
        }

        public void AtualizarLog(string mensagem)
        {
            string conteudo = (string)txbLog.Dispatcher.Invoke(new Func<string>(() => txbLog.Text));

            txbLog.Dispatcher.Invoke(new Action<string>(r => txbLog.Text = r.ToString()), conteudo + mensagem + "\n");
        }

        private void btnExecutarCommand_Click(object sender, RoutedEventArgs e)
        {

            string comando = txtCommandLine.Text;

            if(comando != "")
            {
                if(comando.ToLower() == "listar vertices")
                {
                    //listar vértices
                    GrafoCB grafo = new GrafoCB();
                    string mensagem = "";

                    if (Uteis.lerGrafoArquivo(grafo))
                    {
                        mensagem = "Vértices: ";
                        foreach(Vertice item in grafo.Vertices)
                        {
                            mensagem += item.Nome + " - ";
                        }

                        mensagem = mensagem.Substring(0, mensagem.Length - 3);

                        AtualizarLog(mensagem);
                    }

                }
                else if(comando.ToLower() == "listar arestas")
                {
                    //Listas arestas
                    GrafoCB grafo = new GrafoCB();
                    string mensagem = "";

                    if (Uteis.lerGrafoArquivo(grafo))
                    {
                        mensagem = "Arestas: ";
                        foreach (Aresta item in grafo.Arestas)
                        {
                            mensagem += item.Descricao + " - ";
                        }

                        mensagem = mensagem.Substring(0, mensagem.Length - 3);

                        AtualizarLog(mensagem);
                    }
                }
                else
                {
                    AtualizarLog("Comando não identificado!");
                }
            }

            txtCommandLine.Text = "";

        }
    }
}
