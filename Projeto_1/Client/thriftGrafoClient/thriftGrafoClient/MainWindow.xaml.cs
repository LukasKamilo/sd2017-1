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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Thrift.Protocol;
using Thrift.Transport;
using thriftGrafo;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace thriftGrafoClient
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        private thriftGrafo.GrafoService.Client client;

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                TTransport transport = new TSocket("localhost", 9091);
                transport.Open();
                TProtocol protocol = new TBinaryProtocol(transport);
                client = new thriftGrafo.GrafoService.Client(protocol);
            }catch(Exception ex)
            {
                MessageBox.Show("Erro ao conectar ao servidor " + ex.Message);
                Application.Current.Shutdown();
            }

            cbBidirecionalAresta.SelectedIndex = 1;
            cbBidirecionalAresta_Update.SelectedIndex = 1;
            cbBidirecionalAresta_Delete.SelectedIndex = 1;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btnInserirVertice_Click(object sender, RoutedEventArgs e)
        {

            thriftGrafo.Vertice v1 = new thriftGrafo.Vertice();

            v1.Nome = Convert.ToInt32(txtNomeVertice.Text);
            v1.Cor = Convert.ToInt32(txtCorVertice.Text);
            v1.Descricao = txtDescricaoVertice.Text;
            v1.Peso = Convert.ToDouble(txtPesoVertice.Text);

            Retorno r = client.insertVertice(v1);
            if (r.Sucesso)
            {
                txtNomeVertice.Text = "";
                txtCorVertice.Text = "";
                txtDescricaoVertice.Text = "";
                txtPesoVertice.Text = "";
                MessageBox.Show(r.Mensagem);
            }
            else
            {
                MessageBox.Show(r.Mensagem);
            }
        }

        private void btnInserirAresta_Click(object sender, RoutedEventArgs e)
        {
            thriftGrafo.Aresta a = new thriftGrafo.Aresta();

            a.VerticeInicio = Convert.ToInt32(txtVerticeOrigemAresta.Text);
            a.VerticeFim = Convert.ToInt32(txtVerticeDestinoAresta.Text);
            a.Descricao = txtDescricaoAresta.Text;
            a.Peso = Convert.ToDouble(txtPesoAresta.Text);
            a.FlagBidirecional = (cbBidirecionalAresta.SelectedIndex == 0 ? true : false);

            Retorno r = client.insertAresta(a);
            if (r.Sucesso)
            {
                txtVerticeOrigemAresta.Text = "";
                txtVerticeDestinoAresta.Text = "";
                txtDescricaoAresta.Text = "";
                txtPesoAresta.Text = "";
                cbBidirecionalAresta.SelectedIndex = 1;
                MessageBox.Show(r.Mensagem);
            }
            else
            {
                MessageBox.Show(r.Mensagem);
            }
        }

        private void txtNomeVertice_Update_LostFocus(object sender, RoutedEventArgs e)
        {

            if(txtNomeVertice_Update.Text == "")
            {
                MessageBox.Show("Por favor informe o nome do vértice!");
                return;
            }
            
            Retorno r = client.getGrafo();

            GrafoAtributo grafo = JsonConvert.DeserializeObject<GrafoAtributo>(r.Retorno_);

            Vertice v = null;

            v = grafo.Vertices.Where(p => p.Nome == Convert.ToInt32(txtNomeVertice_Update.Text)).FirstOrDefault();

            if(v == null)
            {
                txtNomeVertice_Update.Text = "";
                MessageBox.Show("O vértice informado não existe!!!");
            }
            else
            {
                txtNomeVertice_Update.IsEnabled = false;

                txtCorVertice_Update.Text = v.Cor.ToString();
                txtDescricaoVertice_Update.Text = v.Descricao;
                txtPesoVertice_Update.Text = v.Peso.ToString();
            }
        }

        private void btnLimparVerticeUpdate_Click(object sender, RoutedEventArgs e)
        {
            txtNomeVertice_Update.IsEnabled = true;
            txtNomeVertice_Update.Text = "";

            txtCorVertice_Update.Text = "";
            txtDescricaoVertice_Update.Text = "";
            txtPesoVertice_Update.Text = "";
        }

        private void btnAtualizarVertice_Click(object sender, RoutedEventArgs e)
        {
            thriftGrafo.Vertice v = new thriftGrafo.Vertice();

            v.Nome = Convert.ToInt32(txtNomeVertice_Update.Text);
            v.Cor = Convert.ToInt32(txtCorVertice_Update.Text);
            v.Descricao = txtDescricaoVertice_Update.Text;
            v.Peso = Convert.ToDouble(txtPesoVertice_Update.Text);

            Retorno r = client.updateVertice(v);
            if (r.Sucesso)
            {
                txtNomeVertice_Update.Text = "";
                txtCorVertice_Update.Text = "";
                txtDescricaoVertice_Update.Text = "";
                txtPesoVertice_Update.Text = "";
                txtNomeVertice_Update.IsEnabled = true;
                MessageBox.Show(r.Mensagem);
            }
            else
            {
                txtNomeVertice_Update.IsEnabled = true;
                MessageBox.Show(r.Mensagem);
            }
        }

        private void txtDescricaoAresta_Update_LostFocus(object sender, RoutedEventArgs e)
        {

            if (txtDescricaoAresta_Update.Text == "")
            {
                MessageBox.Show("Por favor informe a descrição da aresta!");
            }

            Retorno r = client.getGrafo();

            GrafoAtributo grafo = JsonConvert.DeserializeObject<GrafoAtributo>(r.Retorno_);

            Aresta a = null;

            a = grafo.Arestas.Where(p => p.Descricao == txtDescricaoAresta_Update.Text).FirstOrDefault();

            if (a == null)
            {
                txtDescricaoAresta_Update.Text = "";
                MessageBox.Show("A aresta informado não existe!!!");
            }
            else
            {
                txtDescricaoAresta_Update.IsEnabled = false;

                txtVerticeOrigemAresta_Update.Text = a.VerticeInicio.ToString();
                txtVerticeDestinoAresta_Update.Text = a.VerticeFim.ToString();
                txtPesoAresta_Update.Text = a.Peso.ToString();
                cbBidirecionalAresta_Update.SelectedIndex = (a.FlagBidirecional == true ? 0 : 1);
            }
        }

        private void btnLimparArestaUpdate_Click(object sender, RoutedEventArgs e)
        {
            txtDescricaoAresta_Update.IsEnabled = true;

            txtVerticeOrigemAresta_Update.Text = "";
            txtVerticeDestinoAresta_Update.Text = "";
            txtDescricaoAresta_Update.Text = "";
            txtPesoAresta_Update.Text = "";
            cbBidirecionalAresta_Update.SelectedIndex = 1;
        }

        private void btnAtualizarAresta_Click(object sender, RoutedEventArgs e)
        {
            thriftGrafo.Aresta a = new thriftGrafo.Aresta();

            a.VerticeInicio = Convert.ToInt32(txtVerticeOrigemAresta_Update.Text);
            a.VerticeFim = Convert.ToInt32(txtVerticeDestinoAresta_Update.Text);
            a.Descricao = txtDescricaoAresta_Update.Text;
            a.Peso = Convert.ToDouble(txtPesoAresta_Update.Text);
            a.FlagBidirecional = (cbBidirecionalAresta_Update.SelectedIndex == 0 ? true : false);

            Retorno r = client.updateAresta(a);
            if (r.Sucesso)
            {
                txtVerticeOrigemAresta_Update.Text = "";
                txtVerticeDestinoAresta_Update.Text = "";
                txtDescricaoAresta_Update.Text = "";
                txtPesoAresta_Update.Text = "";
                cbBidirecionalAresta_Update.SelectedIndex = 1;

                txtDescricaoAresta_Update.IsEnabled = true;
                MessageBox.Show(r.Mensagem);
            }
            else
            {
                txtDescricaoAresta_Update.IsEnabled = true;
                MessageBox.Show(r.Mensagem);
            }
        }

        private void txtNomeVertice_Delete_LostFocus(object sender, RoutedEventArgs e)
        {

            if(txtNomeVertice_Delete.Text == "")
            {
                MessageBox.Show("Por favor informe o nome do vértice!");
                return;
            }

            Retorno r = client.getGrafo();

            GrafoAtributo grafo = JsonConvert.DeserializeObject<GrafoAtributo>(r.Retorno_);

            Vertice v = null;

            v = grafo.Vertices.Where(p => p.Nome == Convert.ToInt32(txtNomeVertice_Delete.Text)).FirstOrDefault();

            if (v == null)
            {
                txtNomeVertice_Delete.Text = "";
                MessageBox.Show("O vértice informado não existe!!!");
            }
            else
            {
                txtNomeVertice_Delete.IsEnabled = false;

                txtCorVertice_Delete.Text = v.Cor.ToString();
                txtDescricaoVertice_Delete.Text = v.Descricao;
                txtPesoVertice_Delete.Text = v.Peso.ToString();
            }
        }

        private void txtDescricaoAresta_Delete_LostFocus(object sender, RoutedEventArgs e)
        {

            if (txtDescricaoAresta_Delete.Text == "")
            {
                MessageBox.Show("Por favor informe a descrição da aresta!");
                return;
            }

            Retorno r = client.getGrafo();

            GrafoAtributo grafo = JsonConvert.DeserializeObject<GrafoAtributo>(r.Retorno_);

            Aresta a = null;

            a = grafo.Arestas.Where(p => p.Descricao == txtDescricaoAresta_Delete.Text).FirstOrDefault();

            if (a == null)
            {
                txtDescricaoAresta_Delete.Text = "";
                MessageBox.Show("A aresta informado não existe!!!");
            }
            else
            {
                txtDescricaoAresta_Delete.IsEnabled = false;

                txtVerticeOrigemAresta_Delete.Text = a.VerticeInicio.ToString();
                txtVerticeDestinoAresta_Delete.Text = a.VerticeFim.ToString();
                txtPesoAresta_Delete.Text = a.Peso.ToString();
                cbBidirecionalAresta_Delete.SelectedIndex = (a.FlagBidirecional == true ? 0 : 1);
            }
        }

        private void btnLimparVertice_Delete_Click(object sender, RoutedEventArgs e)
        {
            txtNomeVertice_Delete.IsEnabled = true;
            txtNomeVertice_Delete.Text = "";

            txtCorVertice_Delete.Text = "";
            txtDescricaoVertice_Delete.Text = "";
            txtPesoVertice_Delete.Text = "";
        }

        private void btnExcluirVertice_Delete_Click(object sender, RoutedEventArgs e)
        {
            thriftGrafo.Vertice v = new thriftGrafo.Vertice();

            v.Nome = Convert.ToInt32(txtNomeVertice_Delete.Text);
            v.Cor = Convert.ToInt32(txtCorVertice_Delete.Text);
            v.Descricao = txtDescricaoVertice_Delete.Text;
            v.Peso = Convert.ToDouble(txtPesoVertice_Delete.Text);

            Retorno r = client.deleteVertice(v);
            if (r.Sucesso)
            {
                txtNomeVertice_Delete.Text = "";
                txtCorVertice_Delete.Text = "";
                txtDescricaoVertice_Delete.Text = "";
                txtPesoVertice_Delete.Text = "";
                txtNomeVertice_Delete.IsEnabled = true;
                MessageBox.Show(r.Mensagem);
            }
            else
            {
                txtNomeVertice_Delete.IsEnabled = true;
                MessageBox.Show(r.Mensagem);
            }
        }

        private void btnLimparAresta_Delete_Click(object sender, RoutedEventArgs e)
        {
            txtDescricaoAresta_Delete.IsEnabled = true;

            txtVerticeOrigemAresta_Delete.Text = "";
            txtVerticeDestinoAresta_Delete.Text = "";
            txtDescricaoAresta_Delete.Text = "";
            txtPesoAresta_Delete.Text = "";
            cbBidirecionalAresta_Delete.SelectedIndex = 1;
        }

        private void btnExcluirAresta_Delete_Click(object sender, RoutedEventArgs e)
        {
            thriftGrafo.Aresta a = new thriftGrafo.Aresta();

            a.VerticeInicio = Convert.ToInt32(txtVerticeOrigemAresta_Delete.Text);
            a.VerticeFim = Convert.ToInt32(txtVerticeDestinoAresta_Delete.Text);
            a.Descricao = txtDescricaoAresta_Delete.Text;
            a.Peso = Convert.ToDouble(txtPesoAresta_Delete.Text);
            a.FlagBidirecional = (cbBidirecionalAresta_Delete.SelectedIndex == 0 ? true : false);

            Retorno r = client.deleteAresta(a);
            if (r.Sucesso)
            {
                txtVerticeOrigemAresta_Delete.Text = "";
                txtVerticeDestinoAresta_Delete.Text = "";
                txtDescricaoAresta_Delete.Text = "";
                txtPesoAresta_Delete.Text = "";
                cbBidirecionalAresta_Delete.SelectedIndex = 1;

                txtDescricaoAresta_Delete.IsEnabled = true;
                MessageBox.Show(r.Mensagem);
            }
            else
            {
                txtDescricaoAresta_Delete.IsEnabled = true;
                MessageBox.Show(r.Mensagem);
            }
        }

        private void btnBuscarVerticesAresta_Click(object sender, RoutedEventArgs e)
        {

            if(txtDescricaoArestaListar.Text == "")
            {
                MessageBox.Show("Por favor informe a descrição da aresta a ser utilizada!");
                return;
            }

            Retorno r = client.getGrafo();

            GrafoAtributo grafo = JsonConvert.DeserializeObject<GrafoAtributo>(r.Retorno_);

            Aresta a = null;

            a = grafo.Arestas.Where(p => p.Descricao == txtDescricaoArestaListar.Text).FirstOrDefault();

            if(a == null)
            {
                txtDescricaoArestaListar.Text = "";
                MessageBox.Show("A aresta informada não existe!");
                return;
            }

            Retorno r1 = client.listarVerticesAresta(a);

            List<Vertice> vertices = JsonConvert.DeserializeObject<List<Vertice>>(r1.Retorno_);

            txtNomeVertice_V1.Text = vertices[0].Nome.ToString();
            txtCorVertice_V1.Text = vertices[0].Cor.ToString();
            txtDescricaoVertice_V1.Text = vertices[0].Descricao;
            txtPesoVertice_V1.Text = vertices[0].Peso.ToString();

            txtNomeVertice_V2.Text = vertices[1].Nome.ToString();
            txtCorVertice_V2.Text = vertices[1].Cor.ToString();
            txtDescricaoVertice_V2.Text = vertices[1].Descricao;
            txtPesoVertice_V2.Text = vertices[1].Peso.ToString();
        }

        private void btnBuscarListaArestas_Click(object sender, RoutedEventArgs e)
        {

            if(txtNomeVerticeListar.Text == "")
            {
                MessageBox.Show("Por favor informe o vértice a ser utilizado!");
                return;
            }

            Retorno r = client.getGrafo();

            GrafoAtributo grafo = JsonConvert.DeserializeObject<GrafoAtributo>(r.Retorno_);

            Vertice v = grafo.Vertices.Where(p => p.Nome == Convert.ToInt32(txtNomeVerticeListar.Text)).FirstOrDefault();


            if(v == null)
            {
                MessageBox.Show("O vértice informado não existe!");
                return;
            }

            Retorno r1 = client.listarArestasVertice(v);

            List<Aresta> arestas = JsonConvert.DeserializeObject<List<Aresta>>(r1.Retorno_);

            dgArestas.ItemsSource = arestas;
        }

        private void btnBuscarVizinhos_Click(object sender, RoutedEventArgs e)
        {
            if (txtNomeVerticeVizinhos.Text == "")
            {
                MessageBox.Show("Por favor informe o vértice a ser utilizado!");
                return;
            }

            Retorno r = client.getGrafo();

            GrafoAtributo grafo = JsonConvert.DeserializeObject<GrafoAtributo>(r.Retorno_);

            Vertice v = grafo.Vertices.Where(p => p.Nome == Convert.ToInt32(txtNomeVerticeVizinhos.Text)).FirstOrDefault();


            if (v == null)
            {
                MessageBox.Show("O vértice informado não existe!");
                return;
            }

            Retorno r1 = client.listarVizinhoVertice(v);

            List<Vertice> vertices = JsonConvert.DeserializeObject<List<Vertice>>(r1.Retorno_);

            dgVizinhos.ItemsSource = vertices;
        }

        private void btnBuscarMenorCaminho_Click(object sender, RoutedEventArgs e)
        {

            if (txtNomeVerticeCaminhoOrigem.Text == "")
            {
                MessageBox.Show("Por favor informe o vértice a ser utilizado como origem!");
                return;
            }

            if (txtNomeVerticeCaminhoDestino.Text == "")
            {
                MessageBox.Show("Por favor informe o vértice a ser utilizado como destino!");
                return;
            }

            Retorno r = client.getGrafo();

            GrafoAtributo grafo = JsonConvert.DeserializeObject<GrafoAtributo>(r.Retorno_);

            Vertice v1 = grafo.Vertices.Where(p => p.Nome == Convert.ToInt32(txtNomeVerticeCaminhoOrigem.Text)).FirstOrDefault();

            if (v1 == null)
            {
                MessageBox.Show("O vértice de origem informado não existe!");
                return;
            }

            Vertice v2 = grafo.Vertices.Where(p => p.Nome == Convert.ToInt32(txtNomeVerticeCaminhoDestino.Text)).FirstOrDefault();

            if (v2 == null)
            {
                MessageBox.Show("O vértice de destino informado não existe!");
                return;
            }

            Retorno r1 = client.menorCaminho(v1, v2);

            List<Vertice> caminho = JsonConvert.DeserializeObject<List<Vertice>>(r1.Retorno_);

            string caminhoString = "Menor Caminho: ";

            foreach(Vertice item in caminho)
            {
                caminhoString += item.Nome + " - ";
            }

            txtCaminho.Content = caminhoString.Substring(0, caminhoString.Length - 3);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Retorno r = client.excluirGrafo();

            MessageBox.Show(r.Mensagem);
        }
    }
}
