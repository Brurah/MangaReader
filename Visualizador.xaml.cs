using MR_metro.Class;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MR_metro
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Visualizador : Page
    {
        ImgPage pg = new ImgPage();
        int idImg = 0;
        BitmapImage crImg = new BitmapImage();
        string[] HtmlResp;
        List<ResultadoHtml> RespList = new List<ResultadoHtml>();
        int nmPaginas;
        string urlDownload, nUrlPagina;
        int tpPage = 0;

        public Visualizador()
        {
            this.InitializeComponent();
            //flpImgs.IsTabStop = true;

        }

        private async void loadPages()
        {
            string st = await getPage(pg.endCap);
            RespList = new List<ResultadoHtml>();
            List<ResultadoHtml> RespListC = new List<ResultadoHtml>();
            HtmlResp = st.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

            foreach (string s in HtmlResp)
            {

                RespList.Add(new ResultadoHtml() { resultado = s });
                RespListC.Add(new ResultadoHtml() { resultado = s });
            }

            Regex filtro = new Regex("(((.*?)(href=\")(.*?)(id=\"img\")(.*?)(src=\")(.*?))|(^</select>))");


            RespList = RespList.Where(a => filtro.IsMatch(a.resultado)).ToList();

            filtro = new Regex("(<option )(.*?)(</option>)");

            RespListC = RespListC.Where(a => filtro.IsMatch(a.resultado)).ToList();

            nmPaginas = Convert.ToInt32(Regex.Replace(Regex.Replace(RespList[0].resultado, "</select> of ", ""), "</div>", ""));
            urlDownload = Regex.Replace(RespList[1].resultado, "(.*?)(src=\")|(\")(.*?)(</div>)", "");
            nUrlPagina = Regex.Replace(RespList[1].resultado, "(.*?)(href=\")|(\")(.*?)(</div>)", "");

            pg.Npaginas = nmPaginas;

            for (int i = 0; i < nmPaginas; i++)
            {
                Imagens im = new Imagens();

                im.endPag = Regex.Replace(RespListC[i].resultado, "((.*?)(value=\"))|(((\" )|(\">))(.*?)(</option>))", "");
                im.endImg = urlDownload;
                im.Imagem = new BitmapImage(new Uri(this.BaseUri, "/Imagens/carregando.png"));
                im.nmPag = i + 1;

                flpImgs.Items.Add(im.Imagem);

                //if (i == 0)
                //{
                //    Window.Current.CoreWindow.KeyDown += evDown;
                //}

                pg.lstImg.Add(im);


            }

            PreencherImg();
        }

        private async void PreencherImg()
        {

            foreach (Imagens item in pg.lstImg)
            {
                if (item.endImg != string.Empty && item.endImg != null)
                {
                    string st = await getPage(item.endPag);                    
                    RespList = new List<ResultadoHtml>();
                    HtmlResp = st.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

                    foreach (string s in HtmlResp)
                    {
                        RespList.Add(new ResultadoHtml() { resultado = s });
                    }

                    Regex filtro = new Regex("((.*?)(href=\")(.*?)(id=\"img\")(.*?)(src=\")(.*?))");

                    RespList = RespList.Where(a => filtro.IsMatch(a.resultado)).ToList();

                    urlDownload = Regex.Replace(RespList[0].resultado, "(.*?)(src=\")|(\")(.*?)(</div>)", "");

                    item.endImg = urlDownload;               
                }    

                BitmapImage b = await getImg(item.endImg);
                item.Imagem = b;
                flpImgs.Items[item.nmPag -1] = b;
            }

        }

        private async Task<BitmapImage> getImg(string url)
        {
            HttpClient client = new HttpClient();

            HttpResponseMessage response = await client.GetAsync(url);
            byte[] img = await response.Content.ReadAsByteArrayAsync();
            InMemoryRandomAccessStream randomAccessStream = new InMemoryRandomAccessStream();
            DataWriter writer = new DataWriter(randomAccessStream.GetOutputStreamAt(0));
            writer.WriteBytes(img);
            await writer.StoreAsync();
            BitmapImage b = new BitmapImage();
            b.SetSource(randomAccessStream);
            tpPage++;
            percent.Text = Convert.ToInt32(((double)tpPage / (double)pg.lstImg.Count) * 100).ToString() + "% " + tpPage + "pg\'s";

            return b;

            
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



        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            pg = (ImgPage)e.Parameter;
            pg.Npaginas = 0;
            pg.lstImg = new List<Imagens>();
            //crImg = new BitmapImage(new Uri(this.BaseUri, "Imagens/Carregando.png"));
            //imgcontainer.Source = crImg;
            loadPages();
        }


        private void evDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            //if (args.VirtualKey == Windows.System.VirtualKey.Right)
            //{
            //    try
            //    {
            //        flpImgs.SelectedIndex++;
            //    }
            //    catch { }            
            //}
            //else if (args.VirtualKey == Windows.System.VirtualKey.Left)
            //{
            //    try
            //    {
            //        flpImgs.SelectedIndex--;
            //    }
            //    catch { }
            //}
            NmT.Text = flpImgs.SelectedIndex.ToString();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void flpImgs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NmT.Text = (1 + flpImgs.SelectedIndex).ToString() + " de " + pg.lstImg.Count;
        }
    }
}
