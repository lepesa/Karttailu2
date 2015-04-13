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

using Karttailu2.Data.Generic;
using System;
using System.Collections.Generic;
using System.Windows.Media;
namespace Karttailu2.Graphics
{
    /// <summary>
    /// Shape-luokka. Kertoo mahdollisen tyypin ja onko teksti keskitetty vai ei. (tekstityypit)
    /// </summary>
    public class Shape
    {
        // Tyyppi. Tyypit on seuraavat:
        // ei annettu = oletus, normi viiva/alue/piste
        // 1 == pelkkä kuva/png, ei viivoja
        // 5 == talvitie shape, katkoviia + rinnakkaiset viivat
        // 6 == polku, pieni, katkoviiva
        // 7 == polku, iso, katkoviiva
        // 8 == autotie, viiva ja reunaviivat.
        // 9 == aita, viiva ja pisteitä
        // 19 == luonnoonsuojeluaue shape
        // 20 == kunnanraja, viiva, piste, viiva
        // 21 == jyrkänne, viiva ja siitä viivoja normaalin suuntaisesti
        // 22 == luiska, ohut viiva ja siitä viivoja normaaliin suuntaisesti
        // 25 == keskitä teksti
        // 26 == keskitä teksti, bold
        // 27 == bold teksti
        // 28 == keskitä teksti, kursiivi eteen
        // 29 == kursiivi eteen
        // 30 == viettoviiva
        // 31 == Versuh=-1 -> katkoviiva. Eli puro/joki juonne
        // 32 == suojanne. pallokatkoviiva
        // 33 == koski. valkoinen katkopalloviiva
        // 34 = puurivi. vihreä katkopalloviiva, harvempi kuin koski
        // 35 = venereitti. katkoviiva
        // 37 = rautatie, kkaksivärinen katkoviiva
        // 38 = pensasaita
        // 39 = suoja-alue, pitkä katkoviiva

        public int Type;

        // Tehdäänkö rotaatio keskipisteestä vai alakulmasta. True/False. Käytössä teksteille vain
        public bool TextCentered;

        public Shape(int type = 0, bool textCentered = false)
        {
            Type = type;
            TextCentered = textCentered;
        }

        public Shape Clone()
        {
            return new Shape(this.Type, this.TextCentered);
        }

    }

    /// <summary>
    /// Asetusluokka. Tähän tuodaan kaikkien olemassa olevien (tunnistettujen) luokkien tiedot.
    /// </summary>
    public class ClassSettings
    {

        public ClassSettings(ShapeType shapeType, Decimal id, int group, int zIndex, Brush stroke, double strokeThickness, double width, double height, Brush fill = null, Shape shape = null, VerSuh verSuh = VerSuh.Normaali)
        {
            ShapeType = shapeType;
            Id = id;
            Group = group;
            ZIndex = zIndex;
            Stroke = stroke;
            StrokeThickness = strokeThickness;
            Width = width;
            Height = height;
            Fill = fill;
            if( shape == null )
            {
                Shape = new Shape();
            }
            else
            {
                Shape = shape.Clone();
            }
            VerSuh = verSuh;
        }

        public ShapeType ShapeType;
        public Decimal Id;
        public int ZIndex;
        public Brush Stroke;
        public double StrokeThickness;
            
        public double Width;
        public double Height;

        public Brush Fill;
        public Shape Shape;

        public VerSuh VerSuh;
        public int Group;
    }

    public enum VerSuh
    {
        Normaali,
        EiMitaan,
        NousutOk,
        LaskutOk,
        MolemmatOk,
        Erikois
    }
   

    public class DrawingSettings
    {

        // Ladattavat luokkien kuvat/tekstuurit
       
        public string[] imagesNames = { "26200.png", "22392.png", "22395.png", "22394.png", 
                                 "12200.png", "34300.png", "33000.png", "32719.png",
                                 "35421.png", "35422.png", "32111.png", "34700.png",
                                 "34200.png", "36400.png", "36100.png", "32710.png",
                                 "32715.png", "32714.png", "32713.png", "36392.png",
                                 "36393.png", "33091.png", "32891.png", "34600.png",
                                 "39120.png", "39110.png", "38991.png", "38100.png",
                                 "45710.png", "45200.png", "44800.png", "44900.png",
                                 "45300.png", "72320.png", "72000.png", "72700.png",
                                "22100.png", "30300.png", "39500.png", "36391.png", "72330.png", "72500.png", "38600.png", "38513.png",
                                "38511.png", "38512.png", "32291.png", "44700.png", "32300.png", "32612.png",
                                "45000.png", "35100.png", "45800.png", "45400.png", "38400.png",
                                "44600.png", "16121.png", "32500.png", "44591.png",
                                "16141.png", "16142.png", "16145.png", "16505.png", "32191.png",
                                "16144.png","16800.png", "45500.png", "16712.png","14191.png",
                                "62100.png", "72340.png", "72310.png", "32112.png","16722.png", "16600.png",
                                "16721.png", "34900.png","26191.png",
                               };

        // Erikoisvärit eri luokkia varten.

        private static SolidColorBrush scbVesi = new SolidColorBrush(Color.FromRgb(135, 254, 254));
        private static SolidColorBrush scbVesiRaja = new SolidColorBrush(Color.FromRgb(44, 171, 251));
        private static SolidColorBrush scbVesiMaatuva = new SolidColorBrush(Color.FromRgb(184, 254, 254));

        private static SolidColorBrush scbSuoHelppoMetsaa = new SolidColorBrush(Color.FromRgb(198, 234, 234));
        private static SolidColorBrush scbSuoHelppoPuuton = new SolidColorBrush(Color.FromRgb(212, 206, 69));

        private static SolidColorBrush scbSoistuma = new SolidColorBrush(Color.FromRgb(231, 247, 247));
        private static SolidColorBrush scbKallio = new SolidColorBrush(Color.FromRgb(210, 199, 208));
        private static SolidColorBrush scbUrheilu = new SolidColorBrush(Color.FromRgb(224, 254, 134));
        private static SolidColorBrush scbAutoliikenne = new SolidColorBrush(Color.FromRgb(254, 177, 127));
        private static SolidColorBrush scbTie = new SolidColorBrush(Color.FromRgb(202, 0, 0));
        private static SolidColorBrush scbNiitty = new SolidColorBrush(Color.FromRgb(254, 254,103));

        private static SolidColorBrush scbVarasto = new SolidColorBrush(Color.FromRgb(246,239,231));

        private static SolidColorBrush scbLuonnonSuojelualue = new SolidColorBrush(Color.FromRgb(10, 178, 6));
        private static SolidColorBrush scbLuonnonSuojametsa = new SolidColorBrush(Color.FromRgb(10, 178, 6));

        private static SolidColorBrush scbLuiska = new SolidColorBrush(Color.FromRgb(101, 11, 0));

        private static SolidColorBrush scbTienumero = new SolidColorBrush(Color.FromRgb(205, 26, 231));

        private static SolidColorBrush scbPelto = new SolidColorBrush(Color.FromRgb(254, 218, 134));

        private static SolidColorBrush scbPuisto = new SolidColorBrush(Color.FromRgb(179, 254, 103));

        private static SolidColorBrush scbKeinottekoinenRantaviiva = new SolidColorBrush(Color.FromRgb(221, 236, 131));

        private static SolidColorBrush scbHautausmaa = new SolidColorBrush(Color.FromRgb(112, 233, 110));

        private static SolidColorBrush scbLentokenttaPaallystetty = new SolidColorBrush(Color.FromRgb(241, 90, 90));
        private static SolidColorBrush scbLentokenttaPaallystettyMuualue = new SolidColorBrush(Color.FromRgb(254, 230, 134));
        private static SolidColorBrush scbLentokenttaPaallystamatonMuualue = new SolidColorBrush(Color.FromRgb(254, 254, 142));
        
        private static SolidColorBrush scbRautatieSahko = new SolidColorBrush(Color.FromRgb(170, 254, 254));

        private static SolidColorBrush scbTurve = new SolidColorBrush(Color.FromRgb(205, 179, 101));

        private static SolidColorBrush scbAmpumaAlue = new SolidColorBrush(Color.FromRgb(205, 9, 254));

        private static SolidColorBrush scbVesijatto = new SolidColorBrush(Color.FromRgb(254,254, 67));

        private static SolidColorBrush scbPensasaita = new SolidColorBrush(Color.FromRgb(8, 255, 7));

        private static SolidColorBrush scbSuojaAlue = new SolidColorBrush(Color.FromRgb(204, 5, 254));

        public void LoadImages()
        {
            var settings = this.GetSettings();
            
            foreach (var fileName in imagesNames)
            {
                int typeId = Int32.Parse(fileName.Replace(".png",""));
        
                if(settings.ContainsKey(typeId))
                {
                    var path = System.IO.Path.Combine(Environment.CurrentDirectory, "Images", fileName);
                    var uri = new Uri(path);
                    var imgBrush = new ImageBrush();
                    var img = new System.Windows.Media.Imaging.BitmapImage(uri);
                    imgBrush.TileMode = TileMode.Tile;
                    imgBrush.Viewport = new System.Windows.Rect(0, 0, img.Width, img.Height);
                    imgBrush.ViewportUnits = BrushMappingMode.Absolute;
                    
                    img.Freeze();
                    imgBrush.ImageSource = img;
                    imgBrush.Freeze();

                    settings[typeId].Fill = imgBrush;
                    settings[typeId].Width = img.Width;
                    settings[typeId].Height = img.Height;
                  }

            }
        }

        Dictionary<int, ClassSettings> settings = null;
        Dictionary<int, SolidColorBrush> reunaviivaSettings = null;
        Dictionary<int, SolidColorBrush> epamaarainenReunaviivaSettings = null;

        /// <summary>
        /// Reunaviiva on viiva, joka rajoittaa alueita. Vesialueet pääasiassa, mutta myös muita alueita.
        /// Reunaviivan värit identifioidaan luokan numerolla
        /// </summary>
        /// <returns>Reunaviivojen väriasetukset.</returns>

        public Dictionary<int, SolidColorBrush> GetReunaviivaSettings()
        {
            if (reunaviivaSettings == null)
            {
                reunaviivaSettings = new Dictionary<int, SolidColorBrush>()
                {
                    {0, null},
                    {32111, Brushes.Black},         // Karkea kivennäisenotto
                    {32112, Brushes.Black},         // Hieno kivennäisenotto
                    {32113, Brushes.Black},         // Turvesuo
                    {32200, Brushes.Black},         // Hautausmaa
                    {32300, Brushes.Black},         // Kaatopaikka
                    
                    {32411, Brushes.Black},         // Lentokenttä
                    {32413, Brushes.Black},         // Lentokenttä
                    {32415, Brushes.Black},         // Lentokenttä
                    {32416, Brushes.Black},         // Lentokenttä
                    {32417, Brushes.Black},         // Lentokenttä
                    {32418, Brushes.Black},         // Lentokenttä
                    {32421, Brushes.Black},         // Autoliikennealue
                    {32611, Brushes.Black},         // Pelto
                    {32612, Brushes.Black},         // Puutarha
                    {32500, Brushes.Black},         // Louhos
                    {32800, Brushes.Black},         // Niitty
                    {32900, Brushes.Black},         // Puisto
                    {33000, Brushes.Black},         // Täytemaa
                    {33100, Brushes.Black},         // Urheilualue
                    {34100, null},                  // Kallio
                    {34300, null},                  // Hietikko
                    {35300, null},                  // Soistuma
                    {34700, null},                  // Kivikko
                    {35411, null},                  // Suo
                    {35412, null},                  // Suo
                    {35421, null},                  // Suo
                    {35422, null},                  // Suo
                    {36200, scbVesiRaja},           // Järvi
                    {36211, scbVesiRaja},           // Meri                
                    {36313, scbVesiRaja},           // Virtavesialue
                    {38300, null},                  // Maatuva vesialue
                    {38400, null},                  // Tulva-alue
                    {38600, null},                  // Vesikivikko
                    {38700, null},                  // Matalikko
                    {38900, Brushes.Black},         // Varatoalue
                    {39110, null},                  // Avoin metsämaa
                    {39120, null},                  // Varvikko
                    {39130, null},                  // Avoin vesijättö
                };
            }
            return reunaviivaSettings;
        }

        /// <summary>
        /// Epämääräinen reunaviiva on pisteviiva. Tässä tapauksessa piirretään vain vesialueille, joilla on epämääräinen ranta.
        /// Reunaviivan värit identifioidaan luokan numerolla
        /// </summary>
        /// <returns>Epämääräisten reunaviivojen asetukset</returns>

        public Dictionary<int,SolidColorBrush> GetEpamaarainenReunaviivaSettings()
        {
            if (epamaarainenReunaviivaSettings == null)
            {
                epamaarainenReunaviivaSettings = new Dictionary<int, SolidColorBrush>()
                {
                    {0, null},
                    
                    {32421, Brushes.Black},         // Autoliikennealue
                 
                    {32800, Brushes.Black},         // Niitty
                    {33100, Brushes.Black},         // Urheilualue
                
                    {34100, null},                  // Kallio, symboli (soistumaa vasten)
                    {34300, null},                  // Hietikko
                    {34700, null},                  // Kivikko     ( soistumaa vasten
                    {35300, null},                  // Soistuma
                    {35411, null},                  // Suo
                    {35412, null},                  // Suo, helppokulkuinen

                    {36200, scbVesiRaja},           // Järvivesi
                    {36211, scbVesiRaja},           // Meri     
                    {36313, scbVesiRaja},           // Virtavesialue

                    {38300, null},                  // Maatuva vesialue
                    {38400, null},                  // Tulva-alue
                    {38600, null},                  // Vesikivikko
                    {38700, null},                  // Matalikko
                    {38900, null},                  // Varastoalue
                    {39110, null},                  // Avoin metsämaa
                    {39120, null},                  // Varvikko
                    {39130, null}                   // Avoin vesijättö
                };
            }
            return epamaarainenReunaviivaSettings;
        }

        public Dictionary<int, ClassSettings> GetSettings()
        {
           

            if( settings == null )
            {
                settings = new Dictionary<int, ClassSettings>()
                    { 

                        { 0, new ClassSettings(ShapeType.All, 0,  0, 55, Brushes.Red, 99, 116, 116, Brushes.Blue)},                                          // Oletus, jos luokkaa ei ole alla.

                        { 12105, new ClassSettings(ShapeType.Text, 12105, 55, 30, Brushes.Black, -1, 16, 16, Brushes.Red)},                                  // Autotien siltanumero. Ei näytetä
                        
                        { 12111, new ClassSettings(ShapeType.Polyline, 12111, 25, 20, Brushes.Black, 6, 8, 1, scbTie, new Shape( type: 8 ), VerSuh.MolemmatOk)},              // Autotie Ia motari
                        { 12112, new ClassSettings(ShapeType.Polyline, 12112, 25, 20, Brushes.Black, 6, 7, 1, scbTie, new Shape( type: 8 ), VerSuh.MolemmatOk)},              // Autotie Ib, "iso tie"

                      
                        { 12121, new ClassSettings(ShapeType.Polyline, 12121, 25, 20, Brushes.Black, 6, 6, 1, scbTie, new Shape( type: 8 ), VerSuh.MolemmatOk)},              // Autotie IIa == kaksikaistainne, esim. nelostie
                        { 12122, new ClassSettings(ShapeType.Polyline, 12122, 25, 20, Brushes.Black, 5, 5, 1, scbTie, new Shape( type: 8 ), VerSuh.MolemmatOk)},              // Autotie IIb == kylän pääväylä
                        { 12131, new ClassSettings(ShapeType.Polyline, 12131, 25, 20, Brushes.Black, 4, 4, 1, scbTie, new Shape( type: 8 ), VerSuh.MolemmatOk)},              // Autotie IIIa  == kylän sivuväylä
                        { 12132, new ClassSettings(ShapeType.Polyline, 12132, 25, 20, Brushes.Black, 3, 3, 1, scbTie, new Shape( type: 8 ), VerSuh.MolemmatOk)},              // Autotie IIIb == poikkikadut rakennuksiin

                        { 12141, new ClassSettings(ShapeType.Polyline, 12141, 25, 19, Brushes.Black, 2, 2, 1, Brushes.Black)},                               // Ajotie = asfailtti, hiekkatie. kapea
                        
                        { 12181, new ClassSettings(ShapeType.Text, 12181, 55, 32, scbTienumero, 1, 16, 16, Brushes.Black)},                                  // Paikallistien numero, teksti
                       
                        { 12182, new ClassSettings(ShapeType.Text, 12182, 55, 32, scbTienumero, 1, 16, 16, Brushes.Black )},                                 // Maantien numero, teksti
                        { 12183, new ClassSettings(ShapeType.Text, 12183, 55, 32, scbTienumero, 1, 24, 24, Brushes.Black )},                                 // E- valta- tai kantatien numero, teksti
                        
                        { 12200, new ClassSettings(ShapeType.Point, 12200, 45, 30, Brushes.Black, 1, 16, 12, Brushes.White, new Shape( type: 1 ))},          // Tie/este. (puomi)

                        { 12301, new ClassSettings(ShapeType.Text, 12301, 55, 32, Brushes.Black, 1, 16, 16, Brushes.Black)},                                 // Kulkuväylän nimi, teksti:  "Ruijanpolku"
                        { 12302, new ClassSettings(ShapeType.Text, 12302, 55, 32, Brushes.Black, 1, 16, 16, Brushes.Black)},                                 // Kulkuväylän selite, teksti:  "Val. kuntorata"

                        { 12312, new ClassSettings(ShapeType.Polyline, 12312, 25, 18, Brushes.Black, 1, 4, 1, Brushes.Black, new Shape( type: 5 ), VerSuh.MolemmatOk)},          // talvitie
                        { 12313, new ClassSettings(ShapeType.Polyline, 12313, 25, 18, Brushes.Black, 1.3, 16, 16, Brushes.Black, new Shape( type: 6 ), VerSuh.MolemmatOk )},     // polku. ihan pieni, ei hiekkatie
                        { 12314, new ClassSettings(ShapeType.Polyline, 12314, 25, 17, Brushes.Black, 2, 2, 1, Brushes.Black, new Shape( type: 7 ), VerSuh.MolemmatOk )},         // pyörätie
                        { 12316, new ClassSettings(ShapeType.Polyline, 12316, 25, 17, Brushes.Black, 2, 2, 1,Brushes.Black, new Shape( type: 6 ), VerSuh.MolemmatOk)},           // ajopolku. sellainen hiekkatie "valtatie" keskellä erämaata. voi ajaa mönkkärillä
                                     
                        { 14200, new ClassSettings(ShapeType.Point, 14200, 45, 11, Brushes.Red, -1, 16, 12, Brushes.White )},                                // Rautatieliikennepaikka, piste
                        { 14201, new ClassSettings(ShapeType.Text, 14201, 55, 32, Brushes.Black, 1, 24, 24, Brushes.Black )},                                // Rautatieliikennepaikan nimi
                       
                        // rautatiellä koko kertoo ekana viivan leveyden, ja sitten tunnelin leveyden ja viimeiseksi tunnelin viivan leveyden.
                        { 14111, new ClassSettings(ShapeType.Polyline, 14111, 25, 21, Brushes.Black, 4, 4, 1, scbRautatieSahko, new Shape( type: 36 ) )},    // Rautatie, sähkö
                        { 14112, new ClassSettings(ShapeType.Polyline, 14112, 25, 21, Brushes.Black, 4, 4, 1, Brushes.White, new Shape( type: 36 ) )},       // Rautatie, sähköistämätön
                        
                        { 14131, new ClassSettings(ShapeType.Polyline, 14131, 25, 21, Brushes.Black, 4, 4, 1, scbRautatieSahko, new Shape( type: 36 ) )},    // Metro

                        { 14191, new ClassSettings(ShapeType.Point, 14191, 45, 30, Brushes.Black, 1, 16, 12, Brushes.White, new Shape( type: 1 ))},          // Rautatie, sähköistyssyboli
                        
                        { 16102, new ClassSettings(ShapeType.Text, 16102, 55, 32, Brushes.Black, 1, 24, 24, Brushes.Black)},                                 // Turvalaitteen selite
                       
                        { 16121, new ClassSettings(ShapeType.Point, 16121, 45, 30, Brushes.Black, 0, 16, 12, Brushes.White, new Shape( type: 1 ))},          // Kummeli

                        { 16141, new ClassSettings(ShapeType.Point, 16141, 45, 30, Brushes.Black, 0, 16, 12, Brushes.White, new Shape( type: 1 ))},          // Merimerkki, vasen (poistunut kohde)
                        { 16142, new ClassSettings(ShapeType.Point, 16142, 45, 30, Brushes.Black, 0, 16, 12, Brushes.White, new Shape( type: 1 ))},          // Merimerkki, oikea (poistunut kohde)
                        { 16144, new ClassSettings(ShapeType.Point, 16144, 45, 30, Brushes.Black, 0, 16, 12, Brushes.White, new Shape( type: 1 ))},          // Merimerkki, etelä (poistunut kohde)
                                                  
                        { 16145, new ClassSettings(ShapeType.Point, 16145, 45, 30, Brushes.Black, 0, 16, 12, Brushes.White, new Shape( type: 1 ))},          // Merimerkki, itä (poistunut kohde)
                        
                        { 16503, new ClassSettings(ShapeType.Text, 16503, 55, 32, Brushes.Black, 1, 24, 24, Brushes.Black )},                                // Kulkusyvyys (2.2mm teksti)
                        { 16504, new ClassSettings(ShapeType.Text, 16504, 55, 32, Brushes.Black, 1, 16, 16, Brushes.Black )},                                // Kulkusyvyys (1.8mm teksti)
                        { 16505, new ClassSettings(ShapeType.Point, 16505, 45, 30, Brushes.Black, 0, 16, 12, Brushes.White, new Shape( type: 1 ))},          // Nimelliskulkusuunta, vesi (poistunut kohde)

                        { 16508, new ClassSettings(ShapeType.Text, 16508, 55, 32, Brushes.Black, 1, 16, 16, Brushes.Black )},                                //Alikulkukorkeus
                        
                        { 16511, new ClassSettings(ShapeType.Polyline, 16511, 25, 19, Brushes.Black, 1.25, 1, 1)},                                           // Laivaväylä, viiva 
                        { 16512, new ClassSettings(ShapeType.Polyline, 16512, 25, 19, Brushes.Black, 1.25, 1, 1, null, new Shape(type: 35))},                // Veneilyreitti, katkoviiva 

                        { 16600, new ClassSettings(ShapeType.Point, 16600, 45, 25, Brushes.Black, 0, 16, 12, Brushes.White, new Shape( type: 1 ))},          // Ankkuripaikka
                    
                        { 16703, new ClassSettings(ShapeType.Text, 16703, 55, 32, Brushes.Black, -1, 16, 16, Brushes.Black )},                               // Hylyn syvyys. ei näytetä
                        
                        { 16712, new ClassSettings(ShapeType.Point, 16712, 45, 25, Brushes.Black, 0, 16, 12, Brushes.White, new Shape( type: 1 ))},          // Hylky, pinnalla
                        { 16721, new ClassSettings(ShapeType.Point, 16721, 45, 25, Brushes.Black, 0, 16, 12, Brushes.White, new Shape( type: 1 ))},          // Hylky, syvyys tuntematon.

                        { 16722, new ClassSettings(ShapeType.Point, 16722, 45, 25, Brushes.Black, 0, 16, 12, Brushes.White, new Shape( type: 1 ))},          // Hylky, syvyys tunnettu.
                        
                        { 16800, new ClassSettings(ShapeType.Point, 16800, 45, 25, Brushes.Black, 0, 16, 12, Brushes.White, new Shape( type: 1 ))},          // tunnelin aukko
                        
                        { 22100, new ClassSettings(ShapeType.Point, 22100, 48, 40, Brushes.Black, 0, 09, +9, Brushes.White, new Shape( type: 1 ))},          // Muuntaja, piste
                        { 22200, new ClassSettings(ShapeType.Polyline, 22395, 28, 18, Brushes.Black, 1, 12, 12, Brushes.White, new Shape( type: 1 ))},       // Muuntoasema, viiva
                                           
                        { 22311, new ClassSettings(ShapeType.Polyline, 22311, 28, 38, Brushes.Black, 2, 1, 1, null, null,  VerSuh.EiMitaan)},                // Sähkölinja, suurjännite. viiva
                        { 22312, new ClassSettings(ShapeType.Polyline, 22311, 28, 38, Brushes.Black, 1, 12, 12, null, null, VerSuh.EiMitaan)},               // Sähkölinja, jakelujännite, viiva
                        
                        { 22392, new ClassSettings(ShapeType.Point, 22392, 48, 18, Brushes.Blue, 1, 12, 12, Brushes.White, new Shape( type: 1 ))},           // Suurjännitelinjan pylväs, poikkiviiva
                        { 22394, new ClassSettings(ShapeType.Point, 22394, 48, 18, Brushes.Green, 1, 12, 12, Brushes.White, new Shape( type: 1 ))},          // Suurjännitelinjan symboli, poikki zeta
                        { 22395, new ClassSettings(ShapeType.Point, 22395, 48, 18, Brushes.Orange, 1, 12, 12, Brushes.White, new Shape( type: 1 ))},         // Jakelujännitelinjan piste, poikki zeta
                   
                        { 26111, new ClassSettings(ShapeType.Polyline, 26111, 28, 6, Brushes.Black, 1, 12, 12, Brushes.LightBlue, null, VerSuh.MolemmatOk)}, // putkijohot, kaasu

                        { 26112, new ClassSettings(ShapeType.Polyline, 26112, 28, 6, Brushes.Black, 1, 12, 12, Brushes.LightBlue, null )},                   // putkijohot, kiinteä aine
                        { 26113, new ClassSettings(ShapeType.Polyline, 26113, 28, 6, Brushes.Black, 1, 12, 12, Brushes.LightBlue, null, VerSuh.EiMitaan)},   // putkijohto, lämpö
                    
                        { 26114, new ClassSettings(ShapeType.Polyline, 26114, 28, 6, Brushes.Black, 1, 12, 12, Brushes.LightBlue, null, VerSuh.EiMitaan)},   // putkijohot, vesi
                        { 26115, new ClassSettings(ShapeType.Polyline, 26115, 28, 6, Brushes.Black, 1, 12, 12, Brushes.LightBlue, null, VerSuh.EiMitaan)},       
                        { 26116, new ClassSettings(ShapeType.Polyline, 26116, 28, 6, Brushes.Black, 1, 12, 12, Brushes.LightBlue, null, VerSuh.EiMitaan)},   // putkijohot, viemäri
                        
                        { 26191, new ClassSettings(ShapeType.Point, 26191, 48, 21, Brushes.Red, 1, 12, 12, Brushes.LightBlue, new Shape(type: 1) )},         // Kaasuohdon symboli, kaasu
                        { 26193, new ClassSettings(ShapeType.Point, 26193, 48, 19, Brushes.Red, -1, 12, 12, Brushes.LightBlue)},                             // Putkijohdon symboli, lämpö, ei näytetä?
                        { 26194, new ClassSettings(ShapeType.Point, 26194, 48, 21, Brushes.Red, -1, 12, 12, Brushes.LightBlue)},                             // Putkijohdon symboli, vesi, ei näytetä
                        { 26196, new ClassSettings(ShapeType.Point, 26196, 48, 19, Brushes.Red, -1, 12, 12, Brushes.LightBlue)},                             // Putkijohdon symboli, viemäri, ei näytetä
                        
                        { 26200, new ClassSettings(ShapeType.Point, 26200, 48, 19, Brushes.Black, 1, 12, 12, Brushes.LightBlue)},                            // vedenottamo. sininen ympyrä, poikkiviiva

                        { 26202, new ClassSettings(ShapeType.Text, 26202, 58, 32, Brushes.Black, 1, 16, 16) },                                               // Putkijohto, kiinteä aine

                        { 30100, new ClassSettings(ShapeType.Polyline, 30100, 19, 17,  Brushes.Black, 1, 16, 16)},                                           // Keinotekoinen rantaviiva, alua 

                        { 30211, new ClassSettings(ShapeType.Polyline, 30211, 19, 19, Brushes.Red, 1.25, 16, 16, Brushes.Red)},                              // Yksikäsitteinen reunaviiva. Ei haluta näkyvän?
                        { 30211*10+1, new ClassSettings(ShapeType.Polyline, 30211, 22, 116, Brushes.Yellow, 1.25, 16, 16, Brushes.Red)},                     // Maasto/2 yksikäsitteinen reunaviiva. Ei haluta näkyvän?

                        { 30212, new ClassSettings(ShapeType.Polyline, 30212, 19, 20, Brushes.Red, 1.5, 16, 16, Brushes.Red)},                               // Epämääräinen reunaviiva. Ei haluta näkyvän?
                        { 30212*10+1, new ClassSettings(ShapeType.Polyline, 30212, 22, 20, Brushes.Red, 1.5, 16, 16, Brushes.Red)},                          // Maasto/2 epämääräinen reunaviiva     
               
                        { 30300, new ClassSettings(ShapeType.Polyline, 30300, 19, 19, Brushes.Black, 3, 16, 16, Brushes.Black, new Shape( type: 1 ))},       // Pato. musta viiva
                        { 30400, new ClassSettings(ShapeType.Polyline, 30400, 19, 19, Brushes.Black, 1, 16, 16, Brushes.Black)},                             // Sulkuportti, pitääkö näkyä?
                      
                        { 30900, new ClassSettings(ShapeType.Polyline, 30900, 19, 16, Brushes.LimeGreen, -1, 16, 16, Brushes.Green)},                        // vesialueiden välinen reuna. esim. joki ja joen sivuhaara. ei näytetä

                        { 30999, new ClassSettings(ShapeType.Polyline, 30999, 19, 12,  Brushes.Black, -1, 0, 0, Brushes.Red  )},                             // Maasto/1 tekninen viiva 
                        { 30999*10+1, new ClassSettings(ShapeType.Polyline, 30999, 22, 12,  Brushes.Black, -1, 0, 0, Brushes.Red  )},                        // Maasto/2 tekninen viiva 
                        { 30999*10+2, new ClassSettings(ShapeType.Polyline, 30999, 26, -9,  Brushes.Red, -1, 0, 0)},                                         // Suojelukohteet tekninen viiva

                        { 32102, new ClassSettings(ShapeType.Text, 32102, 16, 22, Brushes.Black, 1, 16, 16, Brushes.White)},                                 // Maa-aineksenottoalueen selite

                        { 32111, new ClassSettings(ShapeType.Polygon, 32111, 64, 3,  Brushes.Black, 0, 16, 16, Brushes.Pink)},                               // Maa-aineksenottoalue, karkea kivennäisaines
                        { 32112, new ClassSettings(ShapeType.Polygon, 32112, 64, 3,  Brushes.Black, 0, 16, 16, Brushes.Pink)},                               // Maa-aineksenottoalue, hieno kivennäisaines
                        { 32113, new ClassSettings(ShapeType.Polygon, 32113, 64, 3,  Brushes.Black, 0, 16, 16, scbTurve)},                                   // Maa-aineksenottoalue, eloperäinen aines

                        { 32191, new ClassSettings(ShapeType.Point, 32191, 13, 19, Brushes.Black, 0, 20, 18, Brushes.White, new Shape( type: 1 ))},          // Eloperäinen ainessymboli
                        
                        { 32200, new ClassSettings(ShapeType.Polygon, 32200, 64, 1,  Brushes.Black, 0, 16, 16, scbHautausmaa)},                              // Hautausmaa, alue
                        
                        { 32300, new ClassSettings(ShapeType.Polygon, 32300, 64, 0,  Brushes.Black, 0, 16, 16, Brushes.Yellow)},                             // Kaatopaikka
                        { 32301, new ClassSettings(ShapeType.Text, 32301, 16, 22, Brushes.Black, 1, 16, 16, Brushes.White)},                                 // Kaatopaikan nimi, teksti
                        { 32302, new ClassSettings(ShapeType.Text, 32302, 16, 22, Brushes.Black, 1, 16, 16, Brushes.White)},                                 // Kaatopaikan selite
                   
                        { 32202, new ClassSettings(ShapeType.Text, 32202, 16, 19,  Brushes.Black, 1, 16, 16, Brushes.Black)},                                // Hautausmaan selite
                        
                        { 32500, new ClassSettings(ShapeType.Polygon, 32500, 64, 0,  Brushes.Red, 0, 16, 16, Brushes.Yellow)},                               // Louhos
                    
                        { 32502, new ClassSettings(ShapeType.Text, 32502, 16, 19,  Brushes.Black, 1, 16, 16, Brushes.Black)},                                // Louhoksen selite   
                        
                        { 32291, new ClassSettings(ShapeType.Point, 32291, 13, 19, Brushes.Black, 0, 20, 18, Brushes.White, new Shape( type: 1 ))},          // Hautausmaan symboli. 

                        { 32401, new ClassSettings(ShapeType.Text, 32401, 16, 19,  Brushes.Black, 1, 16, 16, Brushes.Black)},                                // Liikennealueen nimi
                        { 32402, new ClassSettings(ShapeType.Text, 32402, 16, 19,  Brushes.Black, 1, 16, 16, Brushes.Black)},                                // Liikennealueen selite
                        { 32411, new ClassSettings(ShapeType.Polygon, 32411, 64, 4, Brushes.Black,  0, 1, 1, scbLentokenttaPaallystetty)},                   // Lentokentän kiitotie, päällystetty 
                        { 32413, new ClassSettings(ShapeType.Polygon, 32413, 64, 3, Brushes.Black, 0, 1, 1, scbLentokenttaPaallystamatonMuualue)},           // Muu lentokenttäalue
                        { 32415, new ClassSettings(ShapeType.Polygon, 32416, 64, 3, Brushes.Black, 0, 1, 1, scbLentokenttaPaallystamatonMuualue)},           // Muu lentokenttäalue, päällystämätön 
                        { 32416, new ClassSettings(ShapeType.Polygon, 32416, 64, 3, Brushes.Black, 0, 1, 1, scbLentokenttaPaallystamatonMuualue)},           // Muu lentokenttäalue, päällystämätön 
                        { 32417, new ClassSettings(ShapeType.Polygon, 32417, 64, 3, Brushes.Black,  0, 1, 1, scbLentokenttaPaallystettyMuualue)},            // Muu lentoliikennealue, päällystetty 

                        { 32418, new ClassSettings(ShapeType.Polygon, 32418, 64, 3, Brushes.Black, 0, 1, 1, scbLentokenttaPaallystettyMuualue)},             // Muu lentoliikennealue, päällystetty 
                   
                        { 32421, new ClassSettings(ShapeType.Polygon, 32421, 64, 1,  scbAutoliikenne, 0, 16, 16, scbAutoliikenne)},                          // Autoliikennealue

                        { 32591, new ClassSettings(ShapeType.Point, 32591, 13, 19, Brushes.Black, -1, 20, 18, Brushes.White, new Shape( type: 1 ))},        // louhoksen symboli, ei näytetä?

                        { 32611, new ClassSettings(ShapeType.Polygon, 32611, 64, 1,  Brushes.Black, 0, 16, 16, scbPelto)},                                   // Pelto, alue 
                        { 32612, new ClassSettings(ShapeType.Polygon, 32312, 64, 1,  Brushes.Black, 0, 16, 16, Brushes.Yellow)},                             // Puutarha
                     
                        { 32710, new ClassSettings(ShapeType.Point, 32710, 13, 19, Brushes.Red, 0, 12, 42, Brushes.Green, new Shape( type: 1 ))},            // havumetsä, symboli
                        { 32713, new ClassSettings(ShapeType.Point, 32713, 13, 19, Brushes.Blue, 0, 18, 16, Brushes.Green,new Shape( type: 1 ))},            // lehtimetsä, symboli
                        { 32714, new ClassSettings(ShapeType.Point, 32714, 13, 19, Brushes.Orange,0, 14, 21, Brushes.Green, new Shape( type: 1 ))},          // sekametsä, symboli
                        { 32715, new ClassSettings(ShapeType.Point, 32715, 13, 19, Brushes.Orange, 0, 14, 10, Brushes.White, new Shape( type: 1 ))},         // varvikko, symboli

                        { 32719, new ClassSettings(ShapeType.Point, 32719, 13, 19, Brushes.Orange, 0, 20, 18, Brushes.White, new Shape( type: 1 ))},         // varvikko, symboli
                        
                        { 32800, new ClassSettings(ShapeType.Polygon, 32800, 64, 1,  scbNiitty, 0, 16, 16, scbNiitty)},                                      // Niitty
                        { 32891, new ClassSettings(ShapeType.Point, 32891, 13, 3,  Brushes.Black, 0, 8, 12, scbNiitty, new Shape( type: 1 ))},               // Niitty, symboli
                        
                        { 32900, new ClassSettings(ShapeType.Polygon, 32900, 64, 1,  Brushes.Black, 0, 16, 16, scbPuisto)},                                  // Puisto, alue        
                        { 32902, new ClassSettings(ShapeType.Text, 32902, 16, 32, Brushes.Black, 1, 16, 16, Brushes.White)},                                 // Puiston selite
                 
                        { 33000, new ClassSettings(ShapeType.Polygon, 33000, 64, 0,  Brushes.Yellow, 0, 16, 16, Brushes.Yellow)},                            // Täytemaa        
                        { 33001, new ClassSettings(ShapeType.Text, 33001, 16, 32,  Brushes.Black, 1, 16, 16, Brushes.Yellow)},                               // Täytemaa, teksti
                        { 33002, new ClassSettings(ShapeType.Text, 33002, 16, 32,  Brushes.Black, 1, 16, 16, Brushes.White)},                                // Täytemaa, selite
                     
                        { 33091, new ClassSettings(ShapeType.Point, 33091, 13, 0,  Brushes.Black, -1, 8, 12, scbNiitty, new Shape( type: 1 ))},              // täytemaa, symboli (tyhjää)
                      
                        { 33100, new ClassSettings(ShapeType.Polygon, 33100, 64, 0,  Brushes.Black, 0, 16, 16, scbUrheilu)},                                 // Urheilu ja virkistysalue, alue
                        { 33101, new ClassSettings(ShapeType.Text, 33101, 16, 32,  Brushes.Black, 1, 16, 16, Brushes.White)},                                // Urheilu- ja virkistysalueen nimi
                        { 33102, new ClassSettings(ShapeType.Text, 33102, 16, 32,  Brushes.Black, 1, 16, 16, Brushes.White)},                                // Urheilu ja virkistysalueen selite, teksti
                     
                        { 34100, new ClassSettings(ShapeType.Polygon, 34100, 64, 0,  scbKallio, 1, 9, 9, scbKallio)},                                        // Kalllio, alue
                        { 34100*10+1, new ClassSettings(ShapeType.Polygon, 34100, 13, 0,  scbKallio, -1, 9, 9, scbKallio)},                                  // Kallio. symboli.
                        
                        { 34200, new ClassSettings(ShapeType.Point, 34200, 13, 16, Brushes.Green, 0, 12, 12, Brushes.White, new Shape( type: 1 ))},          // Harva louhikko

                        { 34300, new ClassSettings(ShapeType.Polygon, 34300, 64, 0,  scbKallio, 0, 16, 16, scbKallio)},                                      // Hietikko
                        { 34400, new ClassSettings(ShapeType.Polyline, 34400, 19, 16, Brushes.Black, 2, 16, 16, Brushes.Green, new Shape( type: 21 ) )},     // jyrkänne. viiva, jossa piikkejä

                        { 34600, new ClassSettings(ShapeType.Point, 34600, 13, 19, Brushes.Black, 0, 16, 16, scbKallio, new Shape( type: 1 ))},              // Kivi, symboli
                        { 34601, new ClassSettings(ShapeType.Text, 34601, 16, 32, Brushes.Black, 1, 16, 16, Brushes.White)},                                 // Kiven nimi, teksti
                   
                        { 34700, new ClassSettings(ShapeType.Polygon, 34700, 64, 0,  Brushes.Red, 0, 16, 16, Brushes.Goldenrod)},                            // Kivikko
                        
                        { 34800, new ClassSettings(ShapeType.Polyline, 34800, 19, 16, scbLuiska, 1, 16, 16, scbLuiska, new Shape( type: 22 ) )},             // luiska, viiva jossa piikkejä sisäänpäin

                        { 34900, new ClassSettings(ShapeType.Text, 34900, 13, 13, Brushes.Black, 1, 16, 16, Brushes.White, new Shape( type: 1))},            // Merkittävän luontokohde
                        
                        { 34901, new ClassSettings(ShapeType.Text, 34901, 16, 32, Brushes.Black, 1, 16, 16, Brushes.White)},                                 // Merkittävän luontokohteen nimi
                        { 34902, new ClassSettings(ShapeType.Text, 34902, 16, 32, Brushes.Black, 1, 16, 16, Brushes.White)},                                 // Merkittävän luontokohteen selite
                        
                        { 35010, new ClassSettings(ShapeType.Text, 35010, 16, 32,  Brushes.Black, 1, 28, 28, scbSoistuma)},                                  // Niityn tai pellon nimi
                     
                        { 35020, new ClassSettings(ShapeType.Text, 35020, 16, 32,  Brushes.Black, 1, 28, 28, scbSoistuma)},                                  // Metsäalueen nimi
                        { 35030, new ClassSettings(ShapeType.Text, 35030, 16, 32,  Brushes.Black, 1, 16, 16, scbSoistuma)},                                  // Suon nimi, teksti
                        { 35040, new ClassSettings(ShapeType.Text, 35040, 16, 32,  Brushes.Black, 1, 28, 28, scbSoistuma)},                                  // Kohouman nimi, teksti
                        { 35050, new ClassSettings(ShapeType.Text, 35050, 16, 32,  Brushes.Black, 1, 16, 16, scbSoistuma)},                                  // Painanteen nimi
                        { 35060, new ClassSettings(ShapeType.Text, 35060, 16, 32,  Brushes.Black, 1, 16, 16, scbSoistuma)},                                  // Niemen nimi
                        { 35070, new ClassSettings(ShapeType.Text, 35070, 16, 32,  Brushes.Black, 1, 16, 16, Brushes.Black)},                                // Saaren nimi
                        { 35080, new ClassSettings(ShapeType.Text, 35080, 16, 32,  scbVesiRaja, 1, 16, 16, Brushes.Black)},                                  // Matalikon nimi
                     
                        { 35090, new ClassSettings(ShapeType.Text, 35090, 16, 32,  Brushes.Black, 1, 16, 16, scbSoistuma)},                                  // Muu maastonimi, teksti. "Laanilantulli"
                        { 35100, new ClassSettings(ShapeType.Point, 35100, 13, 19, Brushes.Black, 0, 16, 16, scbKallio, new Shape( type: 1 ))},              // Puu (ei rauhoitettu)
                     
                        { 35200, new ClassSettings(ShapeType.Polyline, 35200, 19, 25,  scbPensasaita, 4, 16, 16, Brushes.Green, new Shape(type: 34))},       // Puurivi. Palloja harvaan
                        
                        { 35300, new ClassSettings(ShapeType.Polygon, 35300, 64, 0,  scbSoistuma, 0, 16, 16,scbSoistuma)},                                   // Soistuma
                        { 35411, new ClassSettings(ShapeType.Polygon, 35411, 64, 0,  scbSuoHelppoPuuton, 0, 16, 16, scbSuoHelppoPuuton)},                    // Suo, helppokulkuinen puuton 
                        { 35412, new ClassSettings(ShapeType.Polygon, 35412, 64, 0,  scbSuoHelppoMetsaa, 0, 16, 16, scbSuoHelppoMetsaa)},                    // Suo, helppokulkuinen metsää kasvava 
                        { 35421, new ClassSettings(ShapeType.Polygon, 35421, 64, 0,  Brushes.Blue, 0, 16, 16, Brushes.Blue)},                                // Suo, vaikeakulkuinen puuton 
                        { 35422, new ClassSettings(ShapeType.Polygon, 35422, 64, 0,  Brushes.Blue, 0, 16, 16, Brushes.Blue)},                                // Suo, vaikeakulkuinen metsää kasvava 

                        { 35500, new ClassSettings(ShapeType.Polyline, 35500, 19, 32,  Brushes.Black, 2.5, 16, 16, Brushes.Black, new Shape(type: 32))},     // Suojänne, viiva. Vaikeakulkuisella SUOLLA merkittävän ylityskohdan muodostava, yleensä veden virtaussuuntaa vastaan kohtisuora mätäsmuodostuma.
                        
                        { 36100, new ClassSettings(ShapeType.Point, 36100, 13, 16, Brushes.Orange, 0, 16, 16, Brushes.White, new Shape( type: 1 ))},         // lähde
                        { 36101, new ClassSettings(ShapeType.Text, 36101, 16, 32,  scbVesiRaja, 1, 16, 16, scbVesi)},                                        // lähteen nimi
                     
                        { 36200, new ClassSettings(ShapeType.Polygon, 36200, 64, 10,scbVesi, 0, 16, 16, scbVesi)},                                           // Järvivesi, alue
                        { 36201, new ClassSettings(ShapeType.Text, 36201, 16, 32, scbVesiRaja, 1, 16, 16, Brushes.White )},                                  // Vakaveden nimi, teksti

                        { 36211, new ClassSettings(ShapeType.Polygon, 36211, 64, 10,scbVesi, 0, 16, 16, scbVesi)},                                           // Merivesi, alue

                        { 36291, new ClassSettings(ShapeType.Text, 36291, 16, 32, scbVesiRaja, 1, 16, 16, Brushes.White )},                                  // Vedenpinnan korkeusluku
                        { 36301, new ClassSettings(ShapeType.Text, 36301, 16, 32, scbVesiRaja, 1, 16, 16, Brushes.White)},                                   // Virtaveden nimi, teksti
                        { 36311, new ClassSettings(ShapeType.Polyline, 36311, 19, 14, scbVesiRaja, 1.5, 16, 16, scbVesiRaja, new Shape( type: 31), VerSuh.Erikois)},      // virtavesi, alle 2m, viiva
                        { 36312, new ClassSettings(ShapeType.Polyline, 36312, 19, 14, scbVesiRaja, 2, 16, 16, scbVesiRaja, new Shape( type: 31), VerSuh.Erikois)},        // virtavesi, 2m-5m, viiva
                        { 36313, new ClassSettings(ShapeType.Polyline, 36313, 64, 12,  scbVesi, 0, 16, 16, scbVesi)},                                        // Virtavesi, alue
                        { 36391, new ClassSettings(ShapeType.Point, 36391, 13, 16, Brushes.Red, 5, 16, 23, Brushes.Green, new Shape( type: 1 ))},            // Virtaveden juoksusuunta (tallennettu alaluokkiin)
                        { 36392, new ClassSettings(ShapeType.Point, 36392, 13, 16, Brushes.Red, 5, 16, 23, Brushes.Green, new Shape( type: 1 ))},            // virtuassuunta, kapea vesi
                        { 36393, new ClassSettings(ShapeType.Point, 36393, 13, 16, Brushes.Red, 5, 16, 23, Brushes.Green, new Shape( type: 1 ))},            // virtuassuunta, leveä vesi
                        { 36400, new ClassSettings(ShapeType.Point, 36400, 13, 16, Brushes.Orange, 0, 16, 18, Brushes.White, new Shape( type: 1 ))},         // vesikuoppa
                        
                        { 36410, new ClassSettings(ShapeType.Text, 36420, 52, 32, scbVesiRaja, 1, 16, 16, Brushes.Red)},    // Vakaveden osan nimi
                        { 36420, new ClassSettings(ShapeType.Text, 36420, 52, 32, scbVesiRaja, 1, 20, 20, Brushes.Green)},  // Virtaveden osan nimi
                      
                        { 36490, new ClassSettings(ShapeType.Text, 36420, 52, 32, scbVesiRaja, 1, 20, 20, Brushes.Green)},  // Muu vesistökohteen nimi
                      
                        { 36500, new ClassSettings(ShapeType.Text, 36500, 16, 32, Brushes.Black, 1, 16, 16, Brushes.White)},                                 // Muun maastokohteen selite, teksti. "Ajoharj. rata"
                        { 38100, new ClassSettings(ShapeType.Point, 38100, 42, 20, Brushes.Red, 0, 22, 21, Brushes.Green, new Shape( type: 1 ))},            // kaislikko symboli
                        { 38200, new ClassSettings(ShapeType.Point, 38200, 22, 20, Brushes.White, 4, 16, 16, Brushes.White, new Shape( type: 33 ))},         // koski, valkoinen palloviiva
                        { 38201, new ClassSettings(ShapeType.Text, 38201, 52, 32, scbVesiRaja, 1, 16, 16, Brushes.White)},  // Kosken nimi, teksti
                        
                        { 38300, new ClassSettings(ShapeType.Polygon, 38300, 70, 12, Brushes.Red, 0, 16, 16, scbVesiMaatuva)},                               // Maatuva vesialue
                        { 38400, new ClassSettings(ShapeType.Polygon, 38400, 70, 12, scbVesiRaja, 0, 16, 16, scbVesi)},                                      // Tulvalue
                        { 38700, new ClassSettings(ShapeType.Polygon, 38700, 70, 12, scbVesiRaja, -1, 16, 16, scbVesi)},                                     // Matalikko, ei näytetä
                     
                        { 38501, new ClassSettings(ShapeType.Text, 38501, 52, 32, Brushes.Black, 1, 16, 16, Brushes.White)},                                 // Vesikiven nimi, teksti
                       
                        { 38511, new ClassSettings(ShapeType.Point, 38511, 42, 19, Brushes.Black, 0, 16, 16, scbKallio, new Shape( type: 1 ))},              // Vesikivi, vedenalainen
                        { 38512, new ClassSettings(ShapeType.Point, 38512, 42, 19, Brushes.Black, 0, 16, 16, scbKallio, new Shape( type: 1 ))},              // Vesikivi, pinnassa
                       
                        { 38513, new ClassSettings(ShapeType.Point, 38513, 42, 19, Brushes.White, 0, 16, 16, scbKallio, new Shape( type: 1 ))},              // Kivi, symboli
                       
                        { 38600, new ClassSettings(ShapeType.Polygon, 38600, 70, 20,  Brushes.Red, 0, 16, 16, Brushes.Goldenrod)},                           // Vesikivikko

                        { 38900, new ClassSettings(ShapeType.Polygon, 38900, 70, 0,  Brushes.Black, 0, 16, 16, scbVarasto, new Shape( type: 1 ))},           // Varastoalue, alue
                        { 38991, new ClassSettings(ShapeType.Point, 38991, 42, 16, Brushes.Black, 4, 22, 21, Brushes.Green, new Shape( type: 1 ))},          // Varastoalueen symboli, ei piirretä
                        { 39110, new ClassSettings(ShapeType.Polygon, 39110, 70, -1,  Brushes.Black, 0, 16, 16, scbKallio, new Shape( type: 1 ))},           // avoin metsä (hakkuu?), alue
                        { 39120, new ClassSettings(ShapeType.Polygon, 39120, 70, -1,  Brushes.Black, 0, 16, 16, scbKallio, new Shape( type: 1 ))},           // Varvikko, alue
                        { 39130, new ClassSettings(ShapeType.Polygon, 39130, 70, 0,  Brushes.Black, 0, 16, 16, scbVesijatto)},                               // Avóin vesijättö
                   
                        { 39500, new ClassSettings(ShapeType.Polyline, 39500, 22, 32,  scbLuonnonSuojelualue, 2, 0, 0, Brushes.Red, new Shape( type: 19 ) )},   // Metsän raja: samanlainen kuin kansallispuistoviiva
                        { 39502, new ClassSettings(ShapeType.Text, 39502, 52, 32,  Brushes.Black, 1, 16, 16, Brushes.Black )},                               // Metsän rajan selite
                         
                        { 40100, new ClassSettings(ShapeType.Polyline, 40100, 34, 16, Brushes.Red, -1, 16, 16, Brushes.Green)},                              //Taajaan rakennetun alueen reunaviiva
                        { 40200, new ClassSettings(ShapeType.Polyline, 40200, 82, 16, Brushes.Red, -1, 16, 16, Brushes.Green)},                              // Taajaan rakennettu alue

                        { 42101, new ClassSettings(ShapeType.Text, 42101, 57, 32, Brushes.Black, 1, 16, 16, Brushes.Green)},                                 // Rakennuksen nimi, teksti
                        { 42102, new ClassSettings(ShapeType.Text, 42101, 57, 32, Brushes.Black, 1, 16, 16, Brushes.Green)},                                 // Rakennuksen selite, teksti
                        { 42201, new ClassSettings(ShapeType.Text, 42201, 57, 16, Brushes.Black, 1, 16, 16, Brushes.Green)},                                 // Rakennusryhmän nimi
                        { 42110, new ClassSettings(ShapeType.Polyline, 42110, 27, 24, Brushes.Red, -1, 16, 16, Brushes.Green)},                              // Asuinrakennus, ? krs, viiva. Ei tarvita, tulee jo 42111
                        { 42111, new ClassSettings(ShapeType.Polyline, 42111, 27, 24, Brushes.Red, -1, 16, 16, Brushes.Green)},                              // Asuinrakennus, 1-2 krs, viiva. Ei tarvita, tulee jo 42111
                        { 42112, new ClassSettings(ShapeType.Polyline, 42112, 27, 24, Brushes.Red, -1, 16, 16, Brushes.Green)},                              // Asuinrakennus, 3-n krs, viiva. Ei tarvita, tulee jo 42212
                        { 42120, new ClassSettings(ShapeType.Polyline, 42120, 27, 24, Brushes.LimeGreen, -1, 16, 16, Brushes.Green)},                        // Liike- tai julkinen rakennus, ? krs, viiva. Ei tarvita, tulee jo 42220   
                        { 42121, new ClassSettings(ShapeType.Polyline, 42121, 27, 24, Brushes.LimeGreen, -1, 16, 16, Brushes.Green)},                        // Liike- tai julkinen rakennus, 1-2 krs, viiva. Ei tarvita, tulee jo 42221
                        { 42122, new ClassSettings(ShapeType.Polyline, 42122, 27, 24, Brushes.LimeGreen, -1, 16, 16, Brushes.Green)},                        // Liike- tai julkinen rakennus, 3-n krs, viiva. Ei tarvita, tulee jo 42222
                        { 42131, new ClassSettings(ShapeType.Polyline, 42131, 27, 24, Brushes.LimeGreen, -1, 16, 16, Brushes.Green)},                        // Lomarakennus, 1-2 krs, viiva. Ei tarvita, tulee jo 42231
                        { 42141, new ClassSettings(ShapeType.Polyline, 42141, 27, 24, Brushes.LimeGreen, -1, 16, 16, Brushes.Green)},                        // Teollinen rakennus, 1-2 krs, viiva. Ei tarvita, tulee jo 42241
                        { 42142, new ClassSettings(ShapeType.Polyline, 42142, 27, 24, Brushes.LimeGreen, -1, 16, 16, Brushes.Green)},                        // Teollinen rakennus, 3-n krs, viiva. Ei tarvita, tulee jo 42242
                        { 42150, new ClassSettings(ShapeType.Polyline, 42150, 27, 24, Brushes.LimeGreen, -1, 16, 16, Brushes.Green)},                        // Kirkollinen rakennus, ? krs, viiva. Ei tarvita, tulee jo 42250
                        { 42151, new ClassSettings(ShapeType.Polyline, 42151, 27, 24, Brushes.LimeGreen, -1, 16, 16, Brushes.Green)},                        // Kirkollinen rakennus, 1-2 krs, viiva. Ei tarvita, tulee jo 42251
                        { 42152, new ClassSettings(ShapeType.Polyline, 42152, 27, 24, Brushes.LimeGreen, -1, 16, 16, Brushes.Green)},                        // Kirkollinen rakennus, 1-2 krs, viiva. Ei tarvita, tulee jo 42252
                       
                        { 42160, new ClassSettings(ShapeType.Polyline, 42160, 27, 24, Brushes.LimeGreen, -1, 16, 16, Brushes.Green)},                        // Muu rakennus, ? krs, viiva. Ei tarvita, tulee jo 42260
                        { 42161, new ClassSettings(ShapeType.Polyline, 42161, 27, 24, Brushes.LimeGreen, -1, 16, 16, Brushes.Green)},                        // Muu rakennus, 1-2 krs, viiva. Ei tarvita, tulee jo 42261
                        { 42162, new ClassSettings(ShapeType.Polyline, 42161, 27, 24, Brushes.LimeGreen, -1, 16, 16, Brushes.Green)},                        // Muu rakennus, 3-n krs, viiva. Ei tarvita, tulee jo 42262
                        
                        { 42170, new ClassSettings(ShapeType.Polyline, 42170, 27, 24, Brushes.Black, -1, 16, 16, Brushes.Purple)},                           // Kirkko. Ei tarvita, tulee jo 42270
                        
                        { 42200, new ClassSettings(ShapeType.Polyline, 42200, 27, 24, Brushes.Red, -1, 16, 16, Brushes.Green)},                              // Rakennusalueen reunaviiva, viiva. Ei tarvita. (esimerkkinä iso hotelli)
                        { 42202, new ClassSettings(ShapeType.Text, 42202, 57, 24, Brushes.Black, 1, 16, 16, Brushes.Green)},                                 // Rakennusryhmän selite   
                        { 42210, new ClassSettings(ShapeType.Polygon, 42210, 75, 24, Brushes.Black, 1, 0, 0, Brushes.DarkSlateGray)},                        // Asuinrakennus, ? krs
                        { 42211, new ClassSettings(ShapeType.Polygon, 42211, 75, 24, Brushes.Black, 1, 0, 0, Brushes.DarkSlateGray)},                        // Asuinrakennus, 1-2 krs
                        { 42212, new ClassSettings(ShapeType.Polygon, 42212, 75, 24, Brushes.Black, 1, 0, 0, Brushes.DarkSlateGray)},                        // Asuinrakennus, 3-n krs
                        { 42220, new ClassSettings(ShapeType.Polygon, 42220, 75, 24, Brushes.Black, 1, 0, 0, Brushes.Pink)},                                 // Liike- tai julkinen rakennus, ? krs
                        { 42221, new ClassSettings(ShapeType.Polygon, 42221, 75, 24, Brushes.Black, 1, 0, 0, Brushes.Pink)},                                 // Liike- tai julkinen rakennus, 1-2 krs
                        { 42222, new ClassSettings(ShapeType.Polygon, 42222, 75, 24, Brushes.Black, 1, 0, 0, Brushes.Pink)},                                 // Liike- tai julkinen rakennus, 3-n krs
                        { 42231, new ClassSettings(ShapeType.Polygon, 42251, 75, 24, Brushes.Black, 1, 0, 0, Brushes.LightGreen)},                           // Lomarakennus, 1-2 krs
                        { 42241, new ClassSettings(ShapeType.Polygon, 42241, 75, 24, Brushes.Black, 1, 0, 0, Brushes.DarkGray)},                             // Teollinen rakennus, 1-2 krs
                        { 42242, new ClassSettings(ShapeType.Polygon, 42242, 75, 24, Brushes.Black, 1, 0, 0, Brushes.DarkGray)},                             // Teollinen rakennus, 3-n krs
                        { 42250, new ClassSettings(ShapeType.Polygon, 42250, 75, 24, Brushes.Black, 1, 0, 0, Brushes.Purple)},                               // Kirkollinen rakennus, ? krs
                        { 42251, new ClassSettings(ShapeType.Polygon, 42251, 75, 24, Brushes.Black, 1, 0, 0, Brushes.Purple)},                               // Kirkollinen rakennus, 1-2 krs
                        { 42252, new ClassSettings(ShapeType.Polygon, 42252, 75, 24, Brushes.Black, 1, 0, 0, Brushes.Purple)},                               // Kirkollinen rakennus, 3-n krs
                        { 42260, new ClassSettings(ShapeType.Polygon, 42260, 75, 24, Brushes.Black, 1, 0, 0 , Brushes.LightGray)},                           // Muu rakennus,? krs, alue. 1:50000, pinta-ala >100m2
                        { 42261, new ClassSettings(ShapeType.Polygon, 42261, 75, 24, Brushes.Black, 1, 0, 0 , Brushes.LightGray)},                           // Muu rakennus, 1-2 krs, alue. 1:50000, pinta-ala >100m2
                        { 42262, new ClassSettings(ShapeType.Polygon, 42261, 75, 24, Brushes.Black, 1, 0, 0 , Brushes.LightGray)},                           // Muu rakennus, 3-n krs, alue. 1:50000, pinta-ala >100m2
                       
                        { 42270, new ClassSettings(ShapeType.Polygon, 42270, 75, 24, Brushes.Black, 1, 0, 0, Brushes.Purple)},                               // Kirkko
                        
                        { 44202, new ClassSettings(ShapeType.Text, 44202, 57, 2, Brushes.Black, 1, 16, 12, Brushes.Black)},                                  // Aidan selite  
                      
                        { 44100, new ClassSettings(ShapeType.Polyline, 44100, 27, 24, Brushes.Black, 2, 16, 12, Brushes.Black )},                            // aallonmurtaja
                        
                        { 44211, new ClassSettings(ShapeType.Polyline, 44211, 27, 20, Brushes.Black, 1, 16, 12, Brushes.Black, new Shape( type: 9 ) )},      // aita, tekoaines

                        { 44213, new ClassSettings(ShapeType.Polyline, 44213, 27, 25, scbPensasaita, 3, 5, 5, Brushes.Black, new Shape( type: 38 ) )},       // aita, istutettu
                        
                        { 44300, new ClassSettings(ShapeType.Polygon, 44300, 75, 10,  Brushes.Black, 1, 0, 0 , scbVesi)},                                    // Allas - alue
                        { 44300*10+1, new ClassSettings(ShapeType.Polyline, 44300, 27, 2,  Brushes.Red, -1, 0, 0 , Brushes.LightBlue)},                      // Allas - viiva. Ei piirretä, tulee jo altaasta
                        { 44301, new ClassSettings(ShapeType.Text, 44301, 57, 32,  Brushes.Black, 1, 16, 16 , Brushes.LightBlue)},                           // Altaan nimi, teksti

                        { 44302, new ClassSettings(ShapeType.Text, 44302, 57, 32,  Brushes.Black, 1, 16, 16 , Brushes.LightBlue)},                           // Altaan selite, teksti
                        
                        { 44500, new ClassSettings(ShapeType.Polyline, 44500, 27, 20, Brushes.Black, 1, 16, 12, Brushes.Black )},                            // Ilmarata
                     
                        { 44591, new ClassSettings(ShapeType.Point, 44591, 47, 19, Brushes.Red, 2, 8, 8, null, new Shape( type: 1 ))},                       // Ilmaradan kannatinpylväs
                        
                        { 44600, new ClassSettings(ShapeType.Point, 44600, 47, 19, Brushes.Black, 0, 16, 16, scbKallio, new Shape( type: 1 ))},              // Kellotapuli

                        { 44700, new ClassSettings(ShapeType.Point, 44700, 47, 19, Brushes.Black, 0, 16, 16, scbKallio, new Shape( type: 1 ))},              // Lähestymisvalo (lentokenttä)
                        
                        { 44800, new ClassSettings(ShapeType.Point, 44800, 47, 24, Brushes.Red, 0, 23, 39, Brushes.Green, new Shape( type: 1 ))},            // masto, symboli   
                        { 44803, new ClassSettings(ShapeType.Text, 44803, 57, 32, Brushes.Black, 0, 16, 16, Brushes.Green)},                                 // maston korkeus, teksti   
                        { 44900, new ClassSettings(ShapeType.Point, 44900, 47, 42, Brushes.Red, 0, 17, 22, Brushes.Green, new Shape( type: 1 ))},            // muistomerkki, symboli  
                        { 44901, new ClassSettings(ShapeType.Text, 44901, 57, 32, Brushes.Black, 1, 16, 16, Brushes.Green)},                                 // muistomerkin nimi, teksti 
                        { 44902, new ClassSettings(ShapeType.Text, 44902, 57, 32, Brushes.Black, 1, 16, 16, Brushes.Green)},                                 // muistomerkin selite, teksti
                        
                        { 45000, new ClassSettings(ShapeType.Point, 45000, 47, 24, Brushes.Red, 0, 23, 39, Brushes.Green, new Shape( type: 1 ))},            // Näkötorni
                        { 45002, new ClassSettings(ShapeType.Point, 45002, 57, 24, Brushes.Black, 0, 23, 39, Brushes.Green)},                                // Näkötornin selite
                      
                        { 45111, new ClassSettings(ShapeType.Polyline, 45111, 27, 20, Brushes.Black, 1.5,0,0)},                                              // Pistolaituri alle 5m, viiva
                        { 45112, new ClassSettings(ShapeType.Polyline, 45112, 27, 20, Brushes.Black, 2,0,0)},                                                // Pistolaituri vähintään 5m, viiva
                  
                        { 45200, new ClassSettings(ShapeType.Point, 45200, 47, 16, Brushes.Red, 0, 37, 39, Brushes.Green, new Shape( type: 1 ))},            // portti, symboli 
                        { 45300, new ClassSettings(ShapeType.Point, 45300, 47, 24, Brushes.Red, 0, 15, 15, Brushes.Green, new Shape( type: 1 ))},            // savupiippu, symboli
                        { 45303, new ClassSettings(ShapeType.Text, 45303, 57, 25, Brushes.Black, 1, 16, 16, Brushes.Green)},                                 // savupiipun korkeus, teksti
                       
                        { 45400, new ClassSettings(ShapeType.Point, 45400, 47, 24, Brushes.Red, 0, 15, 15, Brushes.Green, new Shape( type: 1 ))},            // TErvahauta, symboli
                        { 45402, new ClassSettings(ShapeType.Text, 45402, 57, 25, Brushes.Black, 1, 16, 16, Brushes.Green)},                                 //  Tervahauta, selite
                     
                        { 45500, new ClassSettings(ShapeType.Point, 45500, 47, 19, Brushes.Red, 2, 8, 8, null, new Shape( type: 1 ))},                       // tuulivoimala
                        
                        { 45700, new ClassSettings(ShapeType.Polyline, 45700, 27, 1,  Brushes.Black, 1, 1, 1 , Brushes.LightBlue)},                          // Rakennelma, viiva
                        { 45702, new ClassSettings(ShapeType.Text, 45702, 57, 19, Brushes.Black, 1, 16, 16, Brushes.Green)},                                 // Rakennelman selite
                                            
                        { 45710, new ClassSettings(ShapeType.Point, 45710, 47, 20, Brushes.Red, 0, 22, 21, Brushes.Green, new Shape( type: 1 ))},            // tulentekopaikka, symboli

                        { 45800, new ClassSettings(ShapeType.Point, 45800, 47, 24, Brushes.Red, 0, 23, 39, Brushes.Green, new Shape( type: 1 ))},            // Vesitorni
                        { 45802, new ClassSettings(ShapeType.Text, 45802, 57, 19, Brushes.Black, 1, 16, 16, Brushes.Green)},                                 // Vesitornin selite
                                               
                        { 48111, new ClassSettings(ShapeType.Text, 48120, 57, 32, Brushes.Black, 0, 24, 24, Brushes.Green)},                                 // Kaupungin nimi
                        { 48120, new ClassSettings(ShapeType.Text, 48120, 57, 32, Brushes.Black, 0, 24, 24, Brushes.Green)},                                 // Kylän, kaupunginosan tai kulmakunnan nimi, teksti
                        { 48130, new ClassSettings(ShapeType.Text, 48130, 57, 32, Brushes.Black, 1, 16, 16, Brushes.Green)},                                 // Talon nimi. "Tuiskutunturi"                        
                        { 48190, new ClassSettings(ShapeType.Text, 48190, 57, 32, Brushes.Black, 0, 24, 24, Brushes.Green)},                                 // Muu asutusnimi
                        { 52100, new ClassSettings(ShapeType.Polyline, 52100, 20, 10, Brushes.Brown, 1, 0, 0)},                                              // korkeuskäyrä
                        { 52191, new ClassSettings(ShapeType.Text, 52191, 17, 10, Brushes.Brown, 1, 16,16, Brushes.White, new Shape( textCentered: true ))}, // Korkeuskäyrän korkeusarvo, teksti
                        { 52192, new ClassSettings(ShapeType.Point, 52192, 14, 10, Brushes.Brown, 1.25, 1.25, 6, Brushes.Brown,new Shape( type: 30 ))},      // Korkeyskäyrän viettoviiva
                        { 52193, new ClassSettings(ShapeType.Point, 52193, 14, 10, Brushes.Brown, 1.25, 1.25, 6, Brushes.Brown,new Shape( type: 30 ))},      // Apukäyrän viettoviiva
                      
                        { 52210, new ClassSettings(ShapeType.Text, 52210, 17, 32, Brushes.Black, -1, 12, 12)},                                               // korkeuspiste, teksti. Ei näytetä

                        { 54100, new ClassSettings(ShapeType.Polyline, 54100, 20, 20, scbVesiRaja, 1.5, 1.5, 1.5)},                                          // Syvyyskäyrä
                        { 54191, new ClassSettings(ShapeType.Text, 54191, 17, 32, scbVesiRaja, 1, 12, 12)},                                                  // Syvyyskäyrän syvyysarvo
                        { 54192, new ClassSettings(ShapeType.Point, 54192, 14, 17, scbVesiRaja, 1.25, 1.25, 6, scbVesiRaja,new Shape( type: 30 ))},          // Syvyyskäyrän viettoviiva
                        { 54210, new ClassSettings(ShapeType.Text, 54210, 17, 32, scbVesiRaja, 1, 12, 12)},                                                  // Syvyyspiste

                        { 62100, new ClassSettings(ShapeType.Polyline, 62100, 29, 12,  scbAmpumaAlue, 2, 0, 0, Brushes.Red, new Shape( type: 19 ) )},        // sotilasalueen reunaviiva, viiva
                        { 62102, new ClassSettings(ShapeType.Text, 62102, 59, 19,  scbAmpumaAlue, 2, 10, 10, Brushes.Red )},                                 // sotilasalueen selite
                      
                        { 62200, new ClassSettings(ShapeType.Polyline, 62200, 29, 16,  scbSuojaAlue, 2, 3, 3, scbSuojaAlue, new Shape(type:39))},            //Suoja-alue, viiva
                        { 62200*10+1, new ClassSettings(ShapeType.Polygon, 62200, 77, 2,  scbSuojaAlue, -1, 3, 3, scbSuojaAlue)},                            //Suoja-alue, alue
                        { 62202, new ClassSettings(ShapeType.Text, 62202, 59, 19,  scbSuojaAlue, 2, 10, 10, scbSuojaAlue )},                                 // suoja-alueen selite
                      
                        { 72000, new ClassSettings(ShapeType.Polyline, 72000, 26, 32,  scbLuonnonSuojelualue, 2, 0, 0, Brushes.Red, new Shape( type: 19 ) )}, // suojelualueen reunaviiva, viiva
                      
                        { 72200, new ClassSettings(ShapeType.Polygon, 72200, 74, -9,  Brushes.LightBlue, -1, 3, 3, Brushes.Green)},                          // luonnonsuojelualue, alue. ei näytetä
                        { 72201, new ClassSettings(ShapeType.Polygon, 72201, 74, -9,  Brushes.LightBlue, -1, 3, 3, Brushes.Green)},                          // luonnonpuisto, alue
                        { 72201*10+1, new ClassSettings(ShapeType.Polygon, 72201, 56, 32,  scbLuonnonSuojelualue, 1, 3, 3, Brushes.Green)},                  // Luonnonsuojelualueen nimi, teksti
                        
                        { 72202, new ClassSettings(ShapeType.Polygon, 72202, 74, -9,  Brushes.LightBlue, -1, 3, 3, Brushes.Green)},                          //Kansallispuisto, alue
                        { 72202*10+1, new ClassSettings(ShapeType.Text, 72202, 56, 31,  scbLuonnonSuojelualue, 1, 3, 3, Brushes.Green)},                     //Luonnonsuojelualueen selite, teksti
                                             
                        { 72304, new ClassSettings(ShapeType.Text, 72202, 56, 31,  Brushes.Green, 1, 3, 3, Brushes.Green)},                                  //Luonnonmuistomerkin selite

                        { 72310, new ClassSettings(ShapeType.Point, 72310, 46, 32,  Brushes.Red, 4, 23, 23, Brushes.Aqua, new Shape( type: 1 ))},            //Rauhoitettu kivi
                      
                        { 72320, new ClassSettings(ShapeType.Point, 72320, 46, 32,  Brushes.Green, 4, 23, 23, Brushes.Aqua, new Shape( type: 1 ))},          // rauhoitetu puu, piste
                        { 72330, new ClassSettings(ShapeType.Point, 72330, 46, 32,  Brushes.Green, 4, 10, 16, Brushes.Aqua, new Shape( type: 1 ))},          // Muinaisjäännös, piste
                        { 72340, new ClassSettings(ShapeType.Point, 72320, 46, 32,  Brushes.Green, 4, 23, 23, Brushes.Aqua, new Shape( type: 1 ))},          // Muu rauhoitettu kohde
                                                
                        { 72404, new ClassSettings(ShapeType.Text, 72404, 56, 32,  scbLuonnonSuojelualue, 1, 16, 16, Brushes.Green)},                        // Muinaisjäännöksen selite

                        { 72501, new ClassSettings(ShapeType.Polygon, 72501, 56, 32,  scbLuonnonSuojelualue, 1, 16, 16, Brushes.Green)},                     // Suojametsän selite

                        { 72601, new ClassSettings(ShapeType.Text, 72601, 56, 32,  scbLuonnonSuojelualue, 2, 26, 26, Brushes.Green)},                        // Kansallispuiston nimi, teksti
                        { 72701, new ClassSettings(ShapeType.Text, 72701, 56, 32,  scbLuonnonSuojelualue, 2, 26, 26, Brushes.Green)},                        // Erämääaalueen nimi, teksti

                        { 72500, new ClassSettings(ShapeType.Polyline, 72700, 26, 25,  scbLuonnonSuojametsa, 2, 0, 0, Brushes.Green, new Shape( type:19 ))},     // Suojametsä, viiva
                        { 72700, new ClassSettings(ShapeType.Polyline, 72700, 26, 25,  scbLuonnonSuojelualue, 2, 0, 0, Brushes.Green, new Shape( type:19 ))},  // erämaa-alue, viiva


                        { 82400, new ClassSettings(ShapeType.Polyline, 84113, 23, 32,  Brushes.Blue, -1, 0, 0, Brushes.Blue )},                              // Ulko- ja sisäsaariston raja. ei näytetä
                        { 82402, new ClassSettings(ShapeType.Text, 82402, 53, 32,  Brushes.Black, -1, 26, 26, Brushes.Green)},                               // Ulko- ja sisäsaariston raja, selite


                        { 84113, new ClassSettings(ShapeType.Polyline, 84113, 23, 32,  Brushes.Blue, 2, 0, 0, Brushes.Blue, new Shape( type: 20 ) )},        // Kunnan raja, viiva.
                        { 84200, new ClassSettings(ShapeType.Polygon, 84200, 71, 0, Brushes.Green, -1, 0, 0)},                                               // kunta, alue. Ei piirretä.

                        { 84302, new ClassSettings(ShapeType.Text, 84302, 53, 32,  Brushes.Black, 2, 26, 26, Brushes.Green)},                                // Muu kaupunki, teksti
                        { 84303, new ClassSettings(ShapeType.Text, 84303, 53, 32,  Brushes.Black, 2, 26, 26, Brushes.Green)},                                // Muu kunta, teksti

                    };
            }
            return settings;
        }

      
    }
}
