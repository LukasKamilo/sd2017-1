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

namespace ModelProject2_Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btnIniciar_Click(object sender, RoutedEventArgs e)
        {
            string mensagemErro;

            Servidor servidor1 = new Servidor();
            Servidor servidor2 = new Servidor();
            Servidor servidor3 = new Servidor();

            if(txtPortaServidor1.Text != "")
            {
                servidor1.Identificador = Convert.ToInt32(txtIdServidor1.Text);
                servidor1.Nome = txtNomeServidor1.Text;
                servidor1.IP = txtIpServidor1.Text;
                servidor1.Porta = txtPortaServidor1.Text;
                VariaveisGlobais.servidor1 = servidor1;
            }

            if (txtPortaServidor2.Text != "")
            {
                servidor2.Identificador = Convert.ToInt32(txtIdServidor2.Text);
                servidor2.Nome = txtNomeServidor2.Text;
                servidor2.IP = txtIpServidor2.Text;
                servidor2.Porta = txtPortaServidor2.Text;
                VariaveisGlobais.servidor2 = servidor2;
            }

            if (txtPortaServidor3.Text != "")
            {
                servidor3.Identificador = Convert.ToInt32(txtIdServidor3.Text);
                servidor3.Nome = txtNomeServidor3.Text;
                servidor3.IP = txtIpServidor3.Text;
                servidor3.Porta = txtPortaServidor3.Text;
                VariaveisGlobais.servidor3 = servidor3;
            }

            //Iniciar servidores
            if(Uteis.IniciarServidores(out mensagemErro))
            {
                MessageBox.Show(mensagemErro);

                Principal p = new Principal();
                p.Show();

                this.Close();
            }
            else
            {
                MessageBox.Show(mensagemErro);
                this.Close();
            }
            
        }
    }
}
