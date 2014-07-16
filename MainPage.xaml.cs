using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using MR_metro.Class;
using SQLite;
using System.Net;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using System.Net.Http;
using System.Text.RegularExpressions;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MR_metro
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        List<lMangaAll> lstTodos = new List<lMangaAll>();
        InfoManga im = new InfoManga();

        public MainPage()
        {
            this.InitializeComponent();    

           if (lstTodos.Count == 0)
           {
               getAll();
           }

            //createF();

        }

        private async void createF()
        {
            StorageFolder fr = await DownloadsFolder.CreateFolderAsync("Mangas");

            await fr.CreateFolderAsync("Temp");
        }

        private async void getAll()
        {
            string st = await getPage("/alphabetical");

            string[] HtmlResp;
            List<ResultadoHtml> RespList = new List<ResultadoHtml>();

            HtmlResp = st.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

            foreach (string s in HtmlResp)
            {
                RespList.Add(new ResultadoHtml() { resultado = s });
            }

            Regex filtro = new Regex("(<li><a href=\"/)(?!\")(?!privacy)(.*?)(\">)(?!<span>)(.*?)(</a>)");
            RespList = RespList.Where(a => filtro.IsMatch(a.resultado)).ToList();

            int i = 0;
            
            foreach (ResultadoHtml item in RespList.ToList())
            {

                lstTodos.Add(new lMangaAll() {Imagem = "/Imagens/mangaReaderIco.png", EndHtml = Regex.Replace(Regex.Replace(item.resultado, "^(<)(.*?)(href=\")", ""), "(\")(.*?)*(</li>)", ""), Titulo = Regex.Replace(Regex.Replace(item.resultado, "^(<)(.*?)(href=\")(.*?)*(>)", ""), "(<)(.*?)*(</li>)", "") });
            }

            lstAll.ItemsSource = lstTodos;
        }

        private async void getManga(string url)
        {
            string st = await getPage(url);
            string[] HtmlResp;

            List<ResultadoHtml> RespList = new List<ResultadoHtml>();
            List<ResultadoHtml> RespListInfo = new List<ResultadoHtml>();
            
            HtmlResp = st.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

            foreach (string s in HtmlResp)
            {
                RespList.Add(new ResultadoHtml() { resultado = s });
                RespListInfo.Add(new ResultadoHtml() { resultado = s });
            }

            
            Regex filtro = new Regex(@"^(<div id=""mangaimg"">|<div id=""mangaproperties"">|<h1>(.*?)</h1>|<td class=""propertytitle"">|<h2 class=""aname"">|<td class=""propertytitle"">|<td>|<div id=""readmangasum"">|<h2>(.*?)</h2>|<p>(.*?)</p)");

            RespListInfo = RespListInfo.Where(a => filtro.IsMatch(a.resultado)).ToList();

            im = nInfoM();
            string cr = "";
            im.Info.EndImagem = Regex.Replace(RespListInfo[0].resultado, "(^(<d)(.*?)(c=\"))|((\" alt)(.*?)(v>))", "");
            im.Info.Titulo += Regex.Replace(RespListInfo[2].resultado, "(<h1>)|(</h1>)", "");
            im.Info.Nome += Regex.Replace(RespListInfo[5].resultado, "(<h2)(.*?)(e\">)|(</h2>)", "");
            im.Info.ANome += Regex.Replace(RespListInfo[7].resultado, "(<td>)|(</td>)", "");
            im.Info.Ano += Regex.Replace(RespListInfo[9].resultado, "(<td>)|(</td>)", "");
            im.Info.Status += Regex.Replace(RespListInfo[11].resultado, "(<td>)|(</td>)", "");
            im.Info.Autor += Regex.Replace(RespListInfo[13].resultado, "(<td>)|(</td>)", "");
            im.Info.Artista += Regex.Replace(RespListInfo[15].resultado, "(<td>)|(</td>)", "");
            im.Info.DirecaoLeitura += Regex.Replace(RespListInfo[17].resultado, "(<td>)|(</td>)", "");

            im.Info.Generos += Regex.Replace(RespListInfo[19].resultado, "(</a>)|(</span>)|(<td>)|(</td>)|(<a href=)(.*?)(\">)|(<span )(.*?)(\">)", "");

            im.Info.Descricao1 = Regex.Replace(RespListInfo[27].resultado, "(<h2>)|(</h2>)", "");
            im.Info.Descricao2 = Regex.Replace(RespListInfo[28].resultado, "(<p>)|(</p>)", "");

            List<Capitulos> ctl = new List<Capitulos>();
            
            filtro = new Regex("^(<div class=\"chico_manga\"></div>|(<a href=\"/)|((<td>)([0-9]{2}/[0-9]{2}/[0-9]{4})(</td>)))");


            RespList = RespList.Where(a => filtro.IsMatch(a.resultado)).ToList();

            grdInfoManga.DataContext = im;

            Regex filtroDiv, filtroHref, filtroTd;
            filtroDiv = new Regex("^(<div )");
            filtroHref = new Regex("^(<a href=)");
            filtroTd = new Regex("^(<td>)");

            int i = 0;
            for (int x = 0; x < RespList.Count(); x++)
            {
                if (filtroDiv.IsMatch(RespList[x].resultado) && filtroHref.IsMatch(RespList[x + 1].resultado) && filtroTd.IsMatch(RespList[x + 2].resultado))
                {
                    i++;
                    ctl.Add(new Capitulos()
                    {                        
                        Endereco = Regex.Replace(RespList[x + 1].resultado, "(<a href=\")|(\">)(.*?)*(</td>)", ""),
                        Numero = Convert.ToInt32(Regex.Replace(RespList[x + 1].resultado, "(^(<a )(.*?)(\">)|(</a>)(.*?)*(</td>))|(\\D+)", "")),
                        nome = Regex.Replace(RespList[x + 1].resultado, "^(<a )(.*?)( : )|(</td>)", ""),
                        Data = Regex.Replace(RespList[x + 2].resultado, "(<td>)|(</td>)", ""),
                    });

                }
            }

            im.Caps = ctl.OrderByDescending(t => t.Numero).ToList();
            lstCap.ItemsSource = im.Caps;
            grdInfoManga.DataContext = im;
            lstAll.IsEnabled = true;
        }


        private async Task<string> getPage(string comp)
        {
            HttpClient httpClient = new HttpClient();

            HttpResponseMessage response = await httpClient.GetAsync("http://www.mangareader.net" + comp);

            using (var streamReader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                return streamReader.ReadToEnd();
            }
        }

        private InfoManga nInfoM()
        {
            Mangas mg = new Mangas()
            {
                Titulo = "",
                Nome = "Nome: ",
                ANome = "Nome Alternativo: ",
                Ano = "Ano de Lançamento: ",
                Status = "Status: ",
                Autor = "Autor: ",
                Artista = "Artista: ",
                DirecaoLeitura = "Direção de Leitura: ",
                Generos = "Generos: "
            };
            InfoManga im = new InfoManga();
            im.Info = mg;
            return im;
        }




        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }     

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection("Mangabd");


            var query = conn.Table<Fontes>();
            List<Fontes> result = await query.ToListAsync();



            //ItemListView.DataContext = result;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection("Mangabd");
            await conn.CreateTableAsync<Fontes>();

            Fontes fnt = new Fontes
            {
                Endereco = "teste",
                Nome = "Teste"
            };
            try
            {
                await conn.InsertAsync(fnt);
            }
            catch
            {

            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            lstAll.Height = brdAll.ActualHeight - 40 - txtPesquisa.ActualHeight;
        }

        private void txtPesquisa_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox txb = (TextBox)sender;

            if (txb.Text == "Pesquisar")
            {
                txb.Text = string.Empty;
            }
        }

        private void txtPesquisa_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox txb = (TextBox)sender;

            if (txb.Text == string.Empty)
            {
                txb.Text = "Pesquisar";
            }
        }

        private void txtPesquisa_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (lstTodos.Count > 0)
            {
                TextBox txb = (TextBox)sender;
                if (!(txb.Text == string.Empty || txb.Text == "Pesquisar"))
                {
                    lstAll.ItemsSource = lstTodos.Where(t => Regex.IsMatch(t.Titulo, "(?i).*" + txb.Text + ".*"));
                }
                else
                {
                    lstAll.ItemsSource = lstTodos;
                }
            }
        }

        private void lstAll_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView ls = (ListView)sender;
            if (ls.SelectedIndex > -1)
            {
                lMangaAll lm = (lMangaAll)ls.SelectedItem;
                lstAll.IsEnabled = false;
                getManga(lm.EndHtml);
            }
        }

        private void lstCap_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView ls = (ListView)sender;
            Capitulos cs = (Capitulos)ls.SelectedItem;

            ImgPage ip = new ImgPage();

            ip.endCap = cs.Endereco;
            ip.NomeM = im.Info.Nome;
            //ip.NumeroCap = cs.Numero;

            //Visualizador vs = new Visualizador(ip);

            Frame.Navigate(typeof(Visualizador), ip);
            //ContainerImg.Children.Add(vs);

        }       
    }
}
