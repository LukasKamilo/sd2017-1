using ModelProject2_Server.CodeBehind;
using Newtonsoft.Json;
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
using System.Windows.Shapes;
using thriftGrafo;

namespace ModelProject2_Client
{
    /// <summary>
    /// Interaction logic for Principal.xaml
    /// </summary>
    public partial class Principal : Window
    {

        private Aresta arestaCorrente = null;

        public Principal()
        {
            InitializeComponent();

            cbBidirecionalArestaInsert.SelectedIndex = 1;
            cbBidirecionalArestaUpdate.SelectedIndex = 1;
            cbBidirecionalArestaDelete.SelectedIndex = 1;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        #region Métodos Vértices
        private void btnInsertVertice_Click(object sender, RoutedEventArgs e)
        {
            Vertice v1 = new Vertice();

            v1.Nome = Convert.ToInt32(txtNomeVerticeInsert.Text);
            v1.Cor = Convert.ToInt32(txtCorVerticeInsert.Text);
            v1.Descricao = txtDescricaoVerticeInsert.Text;
            v1.Peso = Convert.ToDouble(txtPesoVerticeInsert.Text);

            //Vamos identificar qual client será utilizado
            int nClient = Uteis.GetServidor(v1.Nome.ToString());

            Retorno r = new Retorno();

            switch (nClient)
            {
                case 0:
                    r = VariaveisGlobais.client_servidor1.insertVertice(v1);
                    break;
                case 1:
                    r = VariaveisGlobais.client_servidor2.insertVertice(v1);
                    break;
                case 2:
                    r = VariaveisGlobais.client_servidor3.insertVertice(v1);
                    break;
            }

            if (r != null && r.Sucesso)
            {
                txtNomeVerticeInsert.Text = "";
                txtCorVerticeInsert.Text = "";
                txtDescricaoVerticeInsert.Text = "";
                txtPesoVerticeInsert.Text = "";
                MessageBox.Show(r.Mensagem + " Servidor Utilizado - ID " + (nClient + 1) + "!");
            }
            else
            {
                MessageBox.Show("Não foi possível inserir o vértice!" + r.Mensagem);
            }
        }

        private void txtNomeVertice_Update_LostFocus(object sender, RoutedEventArgs e)
        {

            if (txtNomeVerticeUpdate.Text == "")
            {
                MessageBox.Show("Por favor informe o nome do vértice!");
                return;
            }

            int nClient = Uteis.GetServidor(txtNomeVerticeUpdate.Text);

            Retorno r = null;

            switch (nClient)
            {
                case 0:
                    r = VariaveisGlobais.client_servidor1.getGrafo();
                    break;
                case 1:
                    r = VariaveisGlobais.client_servidor2.getGrafo();
                    break;
                case 2:
                    r = VariaveisGlobais.client_servidor3.getGrafo();
                    break;
            }

            GrafoAtributo grafo = JsonConvert.DeserializeObject<GrafoAtributo>(r.Retorno_);

            Vertice v = null;

            v = grafo.Vertices.Where(p => p.Nome == Convert.ToInt32(txtNomeVerticeUpdate.Text)).FirstOrDefault();

            if (v == null)
            {
                txtNomeVerticeUpdate.Text = "";
                MessageBox.Show("O vértice informado não existe!!!");
            }
            else
            {
                txtNomeVerticeUpdate.IsEnabled = false;

                txtCorVerticeUpdate.Text = v.Cor.ToString();
                txtDescricaoVerticeUpdate.Text = v.Descricao;
                txtPesoVerticeUpdate.Text = v.Peso.ToString();
            }
        }

        private void btnUpdateVertice_Click(object sender, RoutedEventArgs e)
        {
            Vertice v1 = new Vertice();

            v1.Nome = Convert.ToInt32(txtNomeVerticeUpdate.Text);
            v1.Cor = Convert.ToInt32(txtCorVerticeUpdate.Text);
            v1.Descricao = txtDescricaoVerticeUpdate.Text;
            v1.Peso = Convert.ToDouble(txtPesoVerticeUpdate.Text);

            //Vamos identificar qual client será utilizado
            int nClient = Uteis.GetServidor(v1.Nome.ToString());

            Retorno r = new Retorno();

            switch (nClient)
            {
                case 0:
                    r = VariaveisGlobais.client_servidor1.updateVertice(v1);
                    break;
                case 1:
                    r = VariaveisGlobais.client_servidor2.updateVertice(v1);
                    break;
                case 2:
                    r = VariaveisGlobais.client_servidor3.updateVertice(v1);
                    break;
            }

            if (r != null && r.Sucesso)
            {
                txtNomeVerticeUpdate.Text = "";
                txtCorVerticeUpdate.Text = "";
                txtDescricaoVerticeUpdate.Text = "";
                txtPesoVerticeUpdate.Text = "";
                txtNomeVerticeUpdate.IsEnabled = true;
                MessageBox.Show(r.Mensagem + " Servidor Utilizado - ID " + (nClient + 1) + "!");
            }
            else
            {
                txtNomeVerticeUpdate.IsEnabled = true;
                MessageBox.Show("Não foi possível atualizar o vértice!" + r.Mensagem);
            }
        }

        private void btnLimparCamposUpdateVertice_Click(object sender, RoutedEventArgs e)
        {
            txtNomeVerticeUpdate.IsEnabled = true;
            txtNomeVerticeUpdate.Text = "";
            txtCorVerticeUpdate.Text = "";
            txtDescricaoVerticeUpdate.Text = "";
            txtPesoVerticeUpdate.Text = "";
        }

        private void txtNomeVertice_Delete_LostFocus(object sender, RoutedEventArgs e)
        {

            if (txtNomeVerticeDelete.Text == "")
            {
                MessageBox.Show("Por favor informe o nome do vértice!");
                return;
            }

            int nClient = Uteis.GetServidor(txtNomeVerticeDelete.Text);

            Retorno r = null;

            switch (nClient)
            {
                case 0:
                    r = VariaveisGlobais.client_servidor1.getGrafo();
                    break;
                case 1:
                    r = VariaveisGlobais.client_servidor2.getGrafo();
                    break;
                case 2:
                    r = VariaveisGlobais.client_servidor3.getGrafo();
                    break;
            }

            GrafoAtributo grafo = JsonConvert.DeserializeObject<GrafoAtributo>(r.Retorno_);

            Vertice v = null;

            v = grafo.Vertices.Where(p => p.Nome == Convert.ToInt32(txtNomeVerticeDelete.Text)).FirstOrDefault();

            if (v == null)
            {
                txtNomeVerticeDelete.Text = "";
                MessageBox.Show("O vértice informado não existe!!!");
            }
            else
            {
                txtNomeVerticeDelete.IsEnabled = false;

                txtCorVerticeDelete.Text = v.Cor.ToString();
                txtDescricaoVerticeDelete.Text = v.Descricao;
                txtPesoVerticeDelete.Text = v.Peso.ToString();
            }
        }

        private void btnDeleteVertice_Click(object sender, RoutedEventArgs e)
        {
            Vertice v1 = new Vertice();

            v1.Nome = Convert.ToInt32(txtNomeVerticeDelete.Text);
            v1.Cor = Convert.ToInt32(txtCorVerticeDelete.Text);
            v1.Descricao = txtDescricaoVerticeDelete.Text;
            v1.Peso = Convert.ToDouble(txtPesoVerticeDelete.Text);

            //Vamos identificar qual client será utilizado
            int nClient = Uteis.GetServidor(v1.Nome.ToString());

            Retorno r = new Retorno();

            switch (nClient)
            {
                case 0:
                    r = VariaveisGlobais.client_servidor1.deleteVertice(v1);
                    break;
                case 1:
                    r = VariaveisGlobais.client_servidor2.deleteVertice(v1);
                    break;
                case 2:
                    r = VariaveisGlobais.client_servidor3.deleteVertice(v1);
                    break;
            }

            if (r != null && r.Sucesso)
            {
                txtNomeVerticeDelete.Text = "";
                txtCorVerticeDelete.Text = "";
                txtDescricaoVerticeDelete.Text = "";
                txtPesoVerticeDelete.Text = "";
                txtNomeVerticeDelete.IsEnabled = true;
                MessageBox.Show(r.Mensagem + " Servidor Utilizado - ID " + (nClient + 1) + "!");
            }
            else
            {
                txtNomeVerticeDelete.IsEnabled = true;
                MessageBox.Show("Não foi possível excluir o vértice!" + r.Mensagem);
            }
        }

        private void btnLimparCamposDeleteVertice_Click(object sender, RoutedEventArgs e)
        {
            txtNomeVerticeDelete.IsEnabled = true;
            txtNomeVerticeDelete.Text = "";
            txtCorVerticeDelete.Text = "";
            txtDescricaoVerticeDelete.Text = "";
            txtPesoVerticeDelete.Text = "";
        }

        #endregion

        #region Métodos Arestas

        private void btnInsertAresta_Click(object sender, RoutedEventArgs e)
        {
            string mensagemRetorno = "";

            Aresta a = new Aresta();

            a.VerticeInicio = Convert.ToInt32(txtVerticeOrigemInsertAresta.Text);
            a.VerticeFim = Convert.ToInt32(txtVerticeDestinoInsertAresta.Text);
            a.Descricao = txtDescricaoArestaInsert.Text;
            a.Peso = Convert.ToDouble(txtPesoArestaInsert.Text);
            a.FlagBidirecional = (cbBidirecionalArestaInsert.SelectedIndex == 0 ? true : false);

            //Vamos identificar qual client será utilizado
            int nClient = Uteis.GetServidor(a.VerticeInicio.ToString());

            Retorno r = new Retorno();

            switch (nClient)
            {
                case 0:
                    r = VariaveisGlobais.client_servidor1.insertAresta(a);
                    break;
                case 1:
                    r = VariaveisGlobais.client_servidor2.insertAresta(a);
                    break;
                case 2:
                    r = VariaveisGlobais.client_servidor3.insertAresta(a);
                    break;
            }

            mensagemRetorno += r.Mensagem + " Servidor Utilizado - ID " + (nClient + 1) + "!";

            //Se a aresta for bidirecional devemos inserir no outro servidor também
            if (a.FlagBidirecional)
            {
                int aux = a.VerticeInicio;
                a.VerticeInicio = a.VerticeFim;
                a.VerticeFim = aux;

                nClient = Uteis.GetServidor(a.VerticeInicio.ToString());

                r = new Retorno();

                switch (nClient)
                {
                    case 0:
                        r = VariaveisGlobais.client_servidor1.insertAresta(a);
                        break;
                    case 1:
                        r = VariaveisGlobais.client_servidor2.insertAresta(a);
                        break;
                    case 2:
                        r = VariaveisGlobais.client_servidor3.insertAresta(a);
                        break;
                }
            }

            mensagemRetorno += "\n" + r.Mensagem + " Servidor Utilizado - ID " + (nClient + 1) + "!";

            if (r != null && r.Sucesso)
            {
                txtVerticeOrigemInsertAresta.Text = "";
                txtVerticeDestinoInsertAresta.Text = "";
                txtDescricaoArestaInsert.Text = "";
                txtPesoArestaInsert.Text = "";
                cbBidirecionalArestaInsert.SelectedIndex = 1;
                MessageBox.Show(mensagemRetorno);
            }
            else
            {
                MessageBox.Show("Não foi possível inserir a aresta! " + r.Mensagem);
            }
        }

        private void Aresta_Update_LostFocus(object sender, RoutedEventArgs e)
        {

            if (txtVerticeOrigemUpdateAresta.Text == "" || txtVerticeDestinoUpdateAresta.Text == "")
            {
                return;
            }

            //Vamos identificar qual client será utilizado
            int nClient = Uteis.GetServidor(txtVerticeOrigemUpdateAresta.Text);

            Retorno r = new Retorno();

            switch (nClient)
            {
                case 0:
                    r = VariaveisGlobais.client_servidor1.getGrafo();
                    break;
                case 1:
                    r = VariaveisGlobais.client_servidor2.getGrafo();
                    break;
                case 2:
                    r = VariaveisGlobais.client_servidor3.getGrafo();
                    break;
            }

            GrafoAtributo grafo = JsonConvert.DeserializeObject<GrafoAtributo>(r.Retorno_);

            Aresta a = null;

            a = grafo.Arestas.Where(p => p.VerticeInicio == Convert.ToInt32(txtVerticeOrigemUpdateAresta.Text) && p.VerticeFim == Convert.ToInt32(txtVerticeDestinoUpdateAresta.Text)).FirstOrDefault();

            if (a == null)
            {
                txtDescricaoArestaUpdate.Text = "";
                MessageBox.Show("A aresta informado não existe!!!");
            }
            else
            {
                txtVerticeOrigemUpdateAresta.IsEnabled = false;
                txtVerticeDestinoUpdateAresta.IsEnabled = false;

                txtVerticeOrigemUpdateAresta.Text = a.VerticeInicio.ToString();
                txtVerticeDestinoUpdateAresta.Text = a.VerticeFim.ToString();
                txtDescricaoArestaUpdate.Text = a.Descricao;
                txtPesoArestaUpdate.Text = a.Peso.ToString();
                cbBidirecionalArestaUpdate.SelectedIndex = (a.FlagBidirecional == true ? 0 : 1);

                arestaCorrente = a;
            }
        }

        private void btnUpdateAresta_Click(object sender, RoutedEventArgs e)
        {
            Aresta a = new Aresta();

            a.VerticeInicio = Convert.ToInt32(txtVerticeOrigemUpdateAresta.Text);
            a.VerticeFim = Convert.ToInt32(txtVerticeDestinoUpdateAresta.Text);
            a.Descricao = txtDescricaoArestaUpdate.Text;
            a.Peso = Convert.ToDouble(txtPesoArestaUpdate.Text);
            a.FlagBidirecional = (cbBidirecionalArestaUpdate.SelectedIndex == 0 ? true : false);

            //Vamos identificar qual client será utilizado
            int nClient = Uteis.GetServidor(a.VerticeInicio.ToString());

            Retorno r = new Retorno();

            switch (nClient)
            {
                case 0:
                    r = VariaveisGlobais.client_servidor1.updateAresta(a);
                    break;
                case 1:
                    r = VariaveisGlobais.client_servidor2.updateAresta(a);
                    break;
                case 2:
                    r = VariaveisGlobais.client_servidor3.updateAresta(a);
                    break;
            }

            //Devemos atualizar a volta da aresta, no outro servidor
            if(arestaCorrente.FlagBidirecional == a.FlagBidirecional)
            {

                if (arestaCorrente.FlagBidirecional == true)
                {
                    int aux = a.VerticeInicio;
                    a.VerticeInicio = a.VerticeFim;
                    a.VerticeFim = aux;

                    nClient = Uteis.GetServidor(a.VerticeInicio.ToString());

                    r = new Retorno();

                    switch (nClient)
                    {
                        case 0:
                            r = VariaveisGlobais.client_servidor1.updateAresta(a);
                            break;
                        case 1:
                            r = VariaveisGlobais.client_servidor2.updateAresta(a);
                            break;
                        case 2:
                            r = VariaveisGlobais.client_servidor3.updateAresta(a);
                            break;
                    }
                }

            }
            else if(arestaCorrente.FlagBidirecional == true)
            {
                //Devemos remover a volta da aresta, visto que a aresta não é mais bidirecional
                int aux = a.VerticeInicio;
                a.VerticeInicio = a.VerticeFim;
                a.VerticeFim = aux;

                nClient = Uteis.GetServidor(a.VerticeInicio.ToString());

                r = new Retorno();

                switch (nClient)
                {
                    case 0:
                        r = VariaveisGlobais.client_servidor1.deleteAresta(a);
                        break;
                    case 1:
                        r = VariaveisGlobais.client_servidor2.deleteAresta(a);
                        break;
                    case 2:
                        r = VariaveisGlobais.client_servidor3.deleteAresta(a);
                        break;
                }
            }
            else if(arestaCorrente.FlagBidirecional == false)
            {
                //Devemos inserir a volta da aresta, visto que a aresta passou a ser bidirecional
                int aux = a.VerticeInicio;
                a.VerticeInicio = a.VerticeFim;
                a.VerticeFim = aux;

                nClient = Uteis.GetServidor(a.VerticeInicio.ToString());

                r = new Retorno();

                switch (nClient)
                {
                    case 0:
                        r = VariaveisGlobais.client_servidor1.insertAresta(a);
                        break;
                    case 1:
                        r = VariaveisGlobais.client_servidor2.insertAresta(a);
                        break;
                    case 2:
                        r = VariaveisGlobais.client_servidor3.insertAresta(a);
                        break;
                }
            }

            if (r.Sucesso)
            {
                txtVerticeOrigemUpdateAresta.Text = "";
                txtVerticeDestinoUpdateAresta.Text = "";
                txtDescricaoArestaUpdate.Text = "";
                txtPesoArestaUpdate.Text = "";
                cbBidirecionalArestaUpdate.SelectedIndex = 1;

                txtVerticeOrigemUpdateAresta.IsEnabled = true;
                txtVerticeDestinoUpdateAresta.IsEnabled = true;
                MessageBox.Show("A aresta foi atualizada com sucesso!");
            }
            else
            {
                txtVerticeOrigemUpdateAresta.IsEnabled = true;
                txtVerticeDestinoUpdateAresta.IsEnabled = true;
                MessageBox.Show("Não foi possível atualizar a aresta! " + r.Mensagem);
            }
        }

        private void btnLimparCamposUpdateAresta_Click(object sender, RoutedEventArgs e)
        {
            txtVerticeOrigemUpdateAresta.IsEnabled = true;
            txtVerticeDestinoUpdateAresta.IsEnabled = true;
            txtVerticeOrigemUpdateAresta.Text = "";
            txtVerticeDestinoUpdateAresta.Text = "";
            txtDescricaoArestaUpdate.Text = "";
            txtPesoArestaUpdate.Text = "";
            cbBidirecionalArestaUpdate.SelectedIndex = 1;

            arestaCorrente = null;
        }

        private void Aresta_Delete_LostFocus(object sender, RoutedEventArgs e)
        {

            if (txtVerticeOrigemDeleteAresta.Text == "" || txtVerticeDestinoDeleteAresta.Text == "")
            {
                return;
            }

            //Vamos identificar qual client será utilizado
            int nClient = Uteis.GetServidor(txtVerticeOrigemDeleteAresta.Text);

            Retorno r = new Retorno();

            switch (nClient)
            {
                case 0:
                    r = VariaveisGlobais.client_servidor1.getGrafo();
                    break;
                case 1:
                    r = VariaveisGlobais.client_servidor2.getGrafo();
                    break;
                case 2:
                    r = VariaveisGlobais.client_servidor3.getGrafo();
                    break;
            }

            GrafoAtributo grafo = JsonConvert.DeserializeObject<GrafoAtributo>(r.Retorno_);

            Aresta a = null;

            a = grafo.Arestas.Where(p => p.VerticeInicio == Convert.ToInt32(txtVerticeOrigemDeleteAresta.Text) && p.VerticeFim == Convert.ToInt32(txtVerticeDestinoDeleteAresta.Text)).FirstOrDefault();

            if (a == null)
            {
                txtDescricaoArestaDelete.Text = "";
                MessageBox.Show("A aresta informado não existe!!!");
            }
            else
            {
                txtVerticeOrigemDeleteAresta.IsEnabled = false;
                txtVerticeDestinoDeleteAresta.IsEnabled = false;

                txtVerticeOrigemDeleteAresta.Text = a.VerticeInicio.ToString();
                txtVerticeDestinoDeleteAresta.Text = a.VerticeFim.ToString();
                txtDescricaoArestaDelete.Text = a.Descricao;
                txtPesoArestaDelete.Text = a.Peso.ToString();
                cbBidirecionalArestaDelete.SelectedIndex = (a.FlagBidirecional == true ? 0 : 1);
            }
        }

        private void btnDeleteAresta_Click(object sender, RoutedEventArgs e)
        {
            Aresta a = new Aresta();

            a.VerticeInicio = Convert.ToInt32(txtVerticeOrigemDeleteAresta.Text);
            a.VerticeFim = Convert.ToInt32(txtVerticeDestinoDeleteAresta.Text);
            a.Descricao = txtDescricaoArestaDelete.Text;
            a.Peso = Convert.ToDouble(txtPesoArestaDelete.Text);
            a.FlagBidirecional = (cbBidirecionalArestaDelete.SelectedIndex == 0 ? true : false);

            //Vamos identificar qual client será utilizado
            int nClient = Uteis.GetServidor(txtVerticeOrigemDeleteAresta.Text);

            Retorno r = new Retorno();

            switch (nClient)
            {
                case 0:
                    r = VariaveisGlobais.client_servidor1.deleteAresta(a);
                    break;
                case 1:
                    r = VariaveisGlobais.client_servidor2.deleteAresta(a);
                    break;
                case 2:
                    r = VariaveisGlobais.client_servidor3.deleteAresta(a);
                    break;
            }

            //Se Bidirecional, excluir a volta
            if (a.FlagBidirecional)
            {
                int aux = a.VerticeInicio;
                a.VerticeInicio = a.VerticeFim;
                a.VerticeFim = aux;

                //Vamos identificar qual client será utilizado
                nClient = Uteis.GetServidor(a.VerticeInicio.ToString());

                r = new Retorno();

                switch (nClient)
                {
                    case 0:
                        r = VariaveisGlobais.client_servidor1.deleteAresta(a);
                        break;
                    case 1:
                        r = VariaveisGlobais.client_servidor2.deleteAresta(a);
                        break;
                    case 2:
                        r = VariaveisGlobais.client_servidor3.deleteAresta(a);
                        break;
                }
            }

            if (r.Sucesso)
            {
                txtVerticeOrigemDeleteAresta.Text = "";
                txtVerticeDestinoDeleteAresta.Text = "";
                txtDescricaoArestaDelete.Text = "";
                txtPesoArestaDelete.Text = "";
                cbBidirecionalArestaDelete.SelectedIndex = 1;

                txtVerticeOrigemDeleteAresta.IsEnabled = true;
                txtVerticeDestinoDeleteAresta.IsEnabled = true;
                MessageBox.Show(r.Mensagem);
            }
            else
            {
                txtVerticeOrigemDeleteAresta.IsEnabled = true;
                txtVerticeDestinoDeleteAresta.IsEnabled = true;
                MessageBox.Show("Não foi possível excuir a aresta! " + r.Mensagem);
            }
        }

        private void btnLimparCamposDeleteAresta_Click(object sender, RoutedEventArgs e)
        {
            txtVerticeOrigemDeleteAresta.IsEnabled = true;
            txtVerticeDestinoDeleteAresta.IsEnabled = true;
            txtVerticeOrigemDeleteAresta.Text = "";
            txtVerticeDestinoDeleteAresta.Text = "";
            txtDescricaoArestaDelete.Text = "";
            txtPesoArestaDelete.Text = "";
            cbBidirecionalArestaDelete.SelectedIndex = 1;
        }

        #endregion

        #region Outros Métodos

        private void btnBuscarArestasVertice_Click(object sender, RoutedEventArgs e)
        {

            if (txtNomeVertice_BuscarArestas.Text == "")
            {
                MessageBox.Show("Por favor informe o vértice a ser utilizado!");
                return;
            }

            try
            {
                Uteis.startAguarde();

                Vertice v = new Vertice();

                v.Nome = Convert.ToInt32(txtNomeVertice_BuscarArestas.Text);

                //Vamos identificar qual client será utilizado
                int nClient = Uteis.GetServidor(v.Nome.ToString());

                Retorno r = new Retorno();

                switch (nClient)
                {
                    case 0:
                        r = VariaveisGlobais.client_servidor1.listarArestasVertice(v);
                        break;
                    case 1:
                        r = VariaveisGlobais.client_servidor2.listarArestasVertice(v);
                        break;
                    case 2:
                        r = VariaveisGlobais.client_servidor3.listarArestasVertice(v);
                        break;
                }

                List<Aresta> arestas = JsonConvert.DeserializeObject<List<Aresta>>(r.Retorno_);

                dgArestas.ItemsSource = arestas;

            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }finally
            {
                Uteis.stopAguarde();
            }

        }

        private void btnBuscarVizinhos_Click(object sender, RoutedEventArgs e)
        {
            if (txtNomeVerticeVizinhos.Text == "")
            {
                MessageBox.Show("Por favor informe o vértice a ser utilizado!");
                return;
            }

            Vertice v = new Vertice();

            v.Nome = Convert.ToInt32(txtNomeVerticeVizinhos.Text);

            try
            {
                Uteis.startAguarde();

                //Vamos identificar qual client será utilizado
                int nClient = Uteis.GetServidor(v.Nome.ToString());

                Retorno r = new Retorno();

                switch (nClient)
                {
                    case 0:
                        r = VariaveisGlobais.client_servidor1.listarVizinhoVertice(v);
                        break;
                    case 1:
                        r = VariaveisGlobais.client_servidor2.listarVizinhoVertice(v);
                        break;
                    case 2:
                        r = VariaveisGlobais.client_servidor3.listarVizinhoVertice(v);
                        break;
                }

                List<Vertice> vertices = JsonConvert.DeserializeObject<List<Vertice>>(r.Retorno_);

                dgVizinhos.ItemsSource = vertices;

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }finally
            {
                Uteis.stopAguarde();
            }
            
        }

        private void btnBuscarVerticesAresta_Click(object sender, RoutedEventArgs e)
        {
            if (txtDescricaoArestaBuscar.Text == "")
            {
                MessageBox.Show("Por favor informe a descrição da aresta a ser utilizada!");
                return;
            }

            Aresta a = new Aresta();
            a.Descricao = txtDescricaoArestaBuscar.Text;

            try
            {

                Uteis.startAguarde();

                //Verificar de forma ordenada em qual servidor está a aresta
                Retorno r = VariaveisGlobais.client_servidor1.listarVerticesAresta(a);

                if (!r.Sucesso)
                {
                    r = VariaveisGlobais.client_servidor2.listarVerticesAresta(a);

                    if (!r.Sucesso)
                    {
                        r = VariaveisGlobais.client_servidor3.listarVerticesAresta(a);

                        if (!r.Sucesso)
                        {
                            MessageBox.Show("A aresta informa não existe");
                            return;
                        }
                    }
                }

                List<Vertice> vertices = JsonConvert.DeserializeObject<List<Vertice>>(r.Retorno_);

                txtNomeVertice1.Text = vertices[0].Nome.ToString();
                txtCorVertice1.Text = vertices[0].Cor.ToString();
                txtDescricaoVertice1.Text = vertices[0].Descricao;
                txtPesoVertice1.Text = vertices[0].Peso.ToString();

                txtNomeVertice2.Text = vertices[1].Nome.ToString();
                txtCorVertice2.Text = vertices[1].Cor.ToString();
                txtDescricaoVertice2.Text = vertices[1].Descricao;
                txtPesoVertice2.Text = vertices[1].Peso.ToString();

            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }finally
            {
                Uteis.stopAguarde();
            }
        }

        private void btnBuscarMenorCaminho_Click(object sender, RoutedEventArgs e)
        {

            Vertice v1 = new Vertice();
            Vertice v2 = new Vertice();

            v1.Nome = Convert.ToInt32(txtVerticeOrigem.Text);
            v2.Nome = Convert.ToInt32(txtVerticeDestino.Text);

            try
            {
                Uteis.startAguarde();

                //Vamos identificar qual client será utilizado
                int nClient = Uteis.GetServidor(v1.Nome.ToString());

                Retorno retornoMenorCaminho = new Retorno();

                switch (nClient)
                {
                    case 0:
                        retornoMenorCaminho = VariaveisGlobais.client_servidor1.menorCaminho(v1, v2);
                        break;
                    case 1:
                        retornoMenorCaminho = VariaveisGlobais.client_servidor2.menorCaminho(v1, v2);
                        break;
                    case 2:
                        retornoMenorCaminho = VariaveisGlobais.client_servidor3.menorCaminho(v1, v2);
                        break;
                }

                if (retornoMenorCaminho.Sucesso)
                {
                    List<Vertice> caminho = JsonConvert.DeserializeObject<List<Vertice>>(retornoMenorCaminho.Retorno_);

                    string caminhoString = "Menor Caminho: ";

                    foreach (Vertice item in caminho)
                    {
                        caminhoString += item.Nome + " - ";
                    }

                    txtCaminho.Content = caminhoString.Substring(0, caminhoString.Length - 3);
                }
                else
                {
                    MessageBox.Show(retornoMenorCaminho.Mensagem);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Uteis.stopAguarde();
            }
        }

        private void btnExcluirGrafo_Click(object sender, RoutedEventArgs e)
        {
            Retorno r = new Retorno();

            r = VariaveisGlobais.client_servidor1.excluirGrafo();
            if (!r.Sucesso)
            {
                MessageBox.Show(r.Mensagem);
            }

            r = VariaveisGlobais.client_servidor2.excluirGrafo();
            if (!r.Sucesso)
            {
                MessageBox.Show(r.Mensagem);
            }

            r = VariaveisGlobais.client_servidor3.excluirGrafo();
            if (!r.Sucesso)
            {
                MessageBox.Show(r.Mensagem);
            }

        }

        #endregion


    }
}
