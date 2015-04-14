# Karttailu2

Tämä on yksinkertainen Maanmittauslaitoksen avoimen maastotietokannan vektoriaineiston piirtosovellus. [Tiedostopalvelu](http://www.maanmittauslaitos.fi/aineistot-palvelut/latauspalvelut/avoimien-aineistojen-tiedostopalvelu) Ohjelman tarkoitus on lukea Shapefile-pohjainen aineisto ja rasterisoida se noin 1:8000 mittakaavaan resoluutiolle kokoon 5734 X 5734 pikseliä.

Ohjelma on WPF sovellus .NET 4.5 alustalle ja kääntyy ainakin Visual Studio Community 2013 käyttäen. 


Käyttö:
* Käännä ohjelma ja aja se. Vaatii joko Microsoft.Jet.OLEDB.4.0 tai Microsoft.ACE.OLEDB.12.0 -ajurit DBase IV -tiedoston käsittelyyn, riippuen bittisyydestä.
* Lataa shapefile .zip -paketti halutusta alueesta ja pura se johonkin kansioon. Esim. V5221L.shp.zip
* Avaa ja valitse ensimmäinen shapefile. Ohjelma ladaa kaikki .shp ja .dbf tiedostot
* Odota että kartta piirtyy 
* Piirtomoodina kannattaa olla Fast. Generic on todella hidas zIndeksin vuoksi, vaikka siinä on hieman parempi antialiasiointi.
* Kuvan tallennuksen pitäisi toimia. Jos ei toimi, kokeile uudestaan.
* Piirto on flattina, projisointi/koordinaatisto/muu on jätetty pois.

#### Tulevaisuuden suunnitelmat

Seuraavat jutut pitäisi lisätä:

* skaalaus / mittakaavan valinta
* koordinaatisto mukaan
* resoluution valinta
* useamman karttapalasen piirto
* parempi fontti
* tekstien näkyvyys mittakaavan mukaan
* grafiikka selvemmäksi
* lisädatan haalinta: pitkospuut, kolmiopisteet (nämä ilmeisesti xml-datassa?), muu lisäinformaatio

#### Käyttöliittymä

![](https://github.com/lepesa/Karttailu2/blob/master/screen.png)
