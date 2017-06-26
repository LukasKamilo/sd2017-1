using ModelProject2_Server.CodeBehind;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Thrift.Server;
using Thrift.Transport;
using thriftGrafo;

namespace UIProject2_Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        List<ServidorVizinho> servidoresVizinhos;

        public MainWindow()
        {
            InitializeComponent();

            servidoresVizinhos = new List<ServidorVizinho>();
        }

        #region Eventos

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btnAdicionar_Click(object sender, RoutedEventArgs e)
        {

            ServidorVizinho servidor = new ServidorVizinho();

            servidor.Identificador = Convert.ToInt32(txtIdentificadorServidorVizinho.Text);
            servidor.IP = txtIpServidorVizinho.Text;
            servidor.Porta = txtPortaServidorVizinho.Text;

            servidoresVizinhos.Add(servidor);

            try
            {
                dgVizinhos.ItemsSource = null;
                dgVizinhos.ItemsSource = servidoresVizinhos;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            this.LimparCampos();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            VariaveisGlobais.servidorLocal.Identificador = Convert.ToInt32(txtIdentificadorServidorLocal.Text);
            VariaveisGlobais.servidorLocal.Nome = txtNomeServidorLocal.Text;
            VariaveisGlobais.servidorLocal.IP = txtIpServidorLocal.Text;
            VariaveisGlobais.servidorLocal.Porta = txtPortaServidorLocal.Text;

            VariaveisGlobais.caminhoArquivos = txtCaminhoArquivoServidorLocal.Text;

            VariaveisGlobais.servidoresVizinhos = servidoresVizinhos;

            if (StartServidor.IniciarServidor())
            {
                MessageBox.Show("Servidor iniciado com sucesso!");

                VariaveisGlobais.telaPrincipal = new Principal();
                VariaveisGlobais.telaPrincipal.Show();

                this.Close();
            }
            else
            {
                MessageBox.Show("Não foi possível iniciar o servidor. Verifique os arquivos de LOG.");
            }
        }

        #endregion

        #region Métodos

        private void LimparCampos()
        {
            txtIdentificadorServidorVizinho.Text = "";
            txtIpServidorVizinho.Text = "";
            txtPortaServidorVizinho.Text = "";
        }

        #endregion

    }
}
