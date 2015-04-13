/*
   Copyright 2015 Esa Leppänen

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Karttailu2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {      
        // Karttaluokka ja loggeri

        MainMap mainMap = null;
        private ILog logger = null;

        
        public MainWindow()
        {
            InitializeComponent();

            // Alustetaan loggeri 
            logger = new Logger(tbLog);
        }
    
     
        /// <summary>
        /// Tallentaa Canvaksen png-tiedostona levylle.
        /// </summary>
        /// <param name="target">UILementti, joka tallennetan</param>
        /// <param name="filename">Tallennettava tiedostonimi</param>
        void CreateBitmapFromVisual(UIElement target, string filename)
        {
            if (target == null)
            {
                return;
            }

            // Otetaan leveys ja korkeus
            int width = (int)target.RenderSize.Width;
            int height = (int)target.RenderSize.Height;

            // Tehdään siitä bitmap

            RenderTargetBitmap rtb = new RenderTargetBitmap(width, height, Graphics.Settings.GlobalDPI, Graphics.Settings.GlobalDPI, System.Windows.Media.PixelFormats.Default);
            rtb.Render(target);

            // Kropataan se: saadaan oikea tyyppi BitmapEncoderia varten
            //var crop = new CroppedBitmap(rtb, new Int32Rect(0, 0, width,height));

            // Tehdään kuva PNG:ksi
            BitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(rtb));

            // Tallennetaan kuva levylle
            using (var fs = System.IO.File.OpenWrite(filename))
            {
                pngEncoder.Save(fs);
            }
        }

        /// <summary>
        /// Tallennetaan ruudun kartta kuvaksi. Pyydetään dialogilla tiedoston nimi.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SavePicture_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Kartta"; 
            dlg.DefaultExt = ".png"; 
            dlg.Filter = "PNG kuvat (.png)|*.png";
                        
            Nullable<bool> result = dlg.ShowDialog();

            // Jos saatiin nimi, yritetään tallennusta
            if (result == true)
            {
                // Save picture.
                CreateBitmapFromVisual(cMap,  dlg.FileName );
                logger.WriteLog(dlg.FileName + " saved.");
            }
        }

        /// <summary>
        /// Ladataan kartta ruutuun. Pyydetään dialogin kautta tiedostonnimi ja sen hakemiston perusteella ladataan
        /// kaikki shapefilet siitä hakemistosta.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Load_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Jos ensimmäinen latauskerta
                if( mainMap == null )
                {
                    Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                    dlg.DefaultExt = ".shp";
                    dlg.Filter = "Shapefiles (*.shp)|*.shp";
                    dlg.Title = "Valitse ensimmäinen shapefile hakemistosta";

                    Nullable<bool> result = dlg.ShowDialog();

                    if (result == true)
                    {
                        btnLoad.IsEnabled = false;
                        string filename = dlg.FileName;
                        string filePath = System.IO.Path.GetDirectoryName(filename);

                        var cWidth = cMap.Width;
                        var cHeight = cMap.Height;

                        // Ajetaan tämä main threadissa, koska toteutus käyttää WPF komponentteja
                        //var task = Task.Factory.StartNew(() => CreateMapTask(filePath, cMap, cWidth, cHeight));
                        CreateMapTask(filePath, cMap, cWidth, cHeight);
                        
                    }

                }
                else 
                {
                    logger.WriteLog("Map already loaded.");
                }
                
            } catch(Exception ex)
            {
                logger.WriteLog("Virhe: " + ex.Message);
            }
        }
   
        /// <summary>
        /// Kartan lataamistaski. Optimitilanteessa tämän voi ajaa taskina, mutta vain jos
        /// ei käytetä piirrossa WPF UI-komponentteja.
        /// </summary>
        /// <param name="filePath">Karttatiedostojen hakemisto</param>
        /// <param name="canv">Canvas</param>
        /// <param name="width">Canvaksen leveys</param>
        /// <param name="height">Canvaksen korkeus</param>
        private void CreateMapTask(string filePath, Canvas canv, double width, double height)
        {
            Stopwatch sw = Stopwatch.StartNew();
            logger.WriteLog("Initializing object...");
            mainMap = new MainMap();

            logger.WriteLog("Loading map...");

            // Ladataan shepfeile
            mainMap.LoadMap(Data.MapfileFactory.MapfileType.ShapeFile, filePath);

            logger.WriteLog("Map loaded: " + sw.Elapsed);

            // Katsotaan kumpaa piirtorajapintaa käytetään, nopeaa vai geneeristä
            var visualizer = Graphics.MapVisualiserFactory.MapVisualiserType.GenericWPF;
            if( rbFast.IsChecked == true)
            {
                visualizer = Graphics.MapVisualiserFactory.MapVisualiserType.FastWPF;
            }

            // Piirretään kartta
            mainMap.DrawMap(canv, visualizer, logger, width, height);

            sw.Stop();
            logger.WriteLog("Map drew." + sw.Elapsed);
        }
    }
}
