

//Abdurrahim Yılmaz 1606A041 a.rahim.yilmaz@hotmail.com


using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Tesseract;
using AForge.Imaging.Filters;

//Bu proje de Gabor filtresi ve bağlantılı bileşen etiketleme olmak üzere 2 dll kullandım.
//Gabor Filtresi:Görüntüde ki dik ayrıntıları tespit edip süzmek için kullandım.
//Bağlantılı bileşen etiketleme:Geçiş piksellerinin tespitine yarayan bir dll.Geçiş piksellerinin
// tespit edilip etiketlenmesi ve bu verilen kullanarak koordinatların elde edilmesi gibi konularda yarıyor.
//Dlller ile gelen kısımları düzenleyerek kodu olabildiğince okunabilir kılmaya çalıştım.

namespace plaka
{


    public partial class Form1 : Form
    {
       
        const int genisletmeMiktari = 8; //Genişletme ve daha sonra aşındırma miktarı(pixel) (dll'lerin içinde gelen değer)
        const int esik = 70; //Gabor filtresinden geçmiş görüntünün ikili seviyeye indirgenirken eşikleneceği değer (dll'lerin içinde gelen değer)

        [DllImport(@"..\..\Kutuphaneler\GaborFiltresi.dll")]            //Dll'leri koda entegre edilmesi.
        public static extern void GaborFiltresi(
                int YonelimAcisi,
                int FazAcisi,
                float EnBoyOrani,
                float BandGenisligi,
                float StandartSapma,
                int DalgaBoyu,
                ref byte ResimVerisi,
                int ResimGenisligi,
                int ResimYuksekligi
            );

        [DllImport(@"..\..\Kutuphaneler\BaglantiliBilesenEtiketleme.dll")]
        public static extern void Etiketle(ref byte girisDizisi, int genislik, int yukseklik, bool renklendir, ref int bilesenSayisi, ref int renkliDizi);

        [DllImport(@"..\..\Kutuphaneler\BaglantiliBilesenEtiketleme.dll")]
        public static extern void KoordinatTespit(ref byte etiketDizisi, int genislik, int yukseklik, int bilesenSayisi, ref int x1, ref int y1, ref int x2, ref int y2);

        public Form1()
        {
            InitializeComponent();        
        
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dosya = new OpenFileDialog();
            if (dosya.ShowDialog() == DialogResult.OK)
            {
                resimfonk(dosya.FileName);      //Açılan dosyanın boyutlandırma fonksiyonu.
            }
        }

        private void resimfonk(String resimYolu)            //Plakayı tespit ederken resim belli aralıkların dışındaysa resmi 
        {                                                   //tekrardan boyutlandırıp belli oranlarda küçültmek, tespit oranını 
            Bitmap resim;                                   //daha arttırıyor ama çok küçültünce yine tespit etmiyor, resmin boyutları için ideal aralıklar.
            resim = new Bitmap(resimYolu);            
                
            if (resim.Width > 480 || resim.Height > 360)
            {
                float katsayi;
                int genislik;
                int yukseklik;
                
                if (resim.Width - 480 > resim.Height - 360)
                {
                    katsayi = (float)480 / resim.Width;
                    genislik = 360;                //Burada varolan değer 480 değilde 360 yapıldığında 2 kat daha iyi sonuç veriyor.
                    yukseklik = (int)(katsayi * resim.Height);
                }
                else
                {
                    katsayi = (float)360 / resim.Height;
                    yukseklik = 360;
                    genislik = (int)(katsayi * resim.Width);
                }
                Bitmap boyutlandirilmis = new Bitmap(genislik, yukseklik);
                Graphics grafik = Graphics.FromImage(boyutlandirilmis);
                grafik.DrawImage(resim, 0, 0, genislik, yukseklik);
                pictureBox1.Image = boyutlandirilmis;
            }
            else pictureBox1.Image = resim;

            Application.DoEvents();         //Plaka konumu fonksiyonu çok uzun olduğundan dolayı, fonksiyon içerisinde ki komutların
                                            // doğru çalışması için kullandım.            
            plakakonumu();          //Resim boyutlandırıldıktan sonra plaka tespit aşaması.
        }

        private void plakakonumu()          //Plaka konum tespit fonksiyonu.
        {
            int x, y;
            int genislik = (int)pictureBox1.Image.Width;        //Genişlik ve yükseklik verilerinin resimden alınması.
            int yukseklik = (int)pictureBox1.Image.Height;
            byte[] gPikseller = new byte[(int)genislik * yukseklik];     //Gri piksel verisi.
            int[] rPikseller = new int[(int)genislik * yukseklik];       //Renkli piksel verisi.
            
            Bitmap resim = (Bitmap)pictureBox1.Image;    //Görüntüyü işleyebilmek için bir Bitmap'e atandı.
            Color renk;
            int sayi;
            for (y = 0; y < yukseklik; y++)
                for (x = 0; x < genislik; x++)
                {
                    renk = resim.GetPixel(x, y);
                    sayi = renk.R;
                    sayi += renk.G;
                    sayi += renk.B;
                    gPikseller[y * genislik + x] = (byte)(sayi / 3);  
                    //Görüntüyü normal yol ile GRAY formatına dönüştürüldü.
                }          
            
            GaborFiltresi(90, 0, (float)0.9, (float)1, (float)0, 4, ref gPikseller[0], genislik, yukseklik); //Gabor filtresi uygulandı.

            pictureBox3.Image = resim;
           
            resim = new Bitmap(genislik, yukseklik);
            Graphics grafik = Graphics.FromImage(resim); 
            Pen kalem = new Pen(Brushes.White);
            grafik.Clear(Color.Black);  
            for (y = 0; y < yukseklik; y++)             //Bütün piksellerin dolaşılması.
                for (x = 0; x < genislik; x++)          
                    if (gPikseller[y * genislik + x] > esik)         //Normal olarak Thresholding(eşikleme) yapılması.
                        grafik.DrawLine(kalem, x - genisletmeMiktari, y, x + genisletmeMiktari, y);         //Sebebi ise bağlantılı bileşen etiketleme 
                                                                                                            //işleminin ikili görüntülerde yapılabilmesi.
            for (y = 0; y < yukseklik; y++)
                for (x = 0; x < genislik; x++)
                    gPikseller[y * genislik + x] = resim.GetPixel(x, y).B;   //Yukarıda düzenlenen sınıfın gPikseller dizisine atanması.

            int bilesenSayisi = 0;
            
            Etiketle(ref gPikseller[0], genislik, yukseklik, false, ref bilesenSayisi, ref rPikseller[0]);       //Etiketleme işlemi.
           
            int[] x1 = new int[bilesenSayisi], y1 = new int[bilesenSayisi], x2 = new int[bilesenSayisi], y2 = new int[bilesenSayisi];
                        
            //Plakanın koordinatlarının tespit fonksiyonu.
            KoordinatTespit(ref gPikseller[0], genislik, yukseklik, bilesenSayisi, ref x1[0], ref y1[0], ref x2[0], ref y2[0]); 
           
            grafik = pictureBox1.CreateGraphics();      //Plaka bilgilerinin bulunup resim üzerine yazılması.
            kalem = new Pen(Brushes.Red, 2);

            int en, boy, tespitEdilen = 0;
            for (x = 0; x < bilesenSayisi; x++)
            {
                x1[x] += genisletmeMiktari;         //Plaka, yatayda genişletmeden dolayı konumu değiştiği için norma konumuna getirdim.
                x2[x] -= genisletmeMiktari;
                en = x2[x] - x1[x] - genisletmeMiktari;
                boy = y2[x] - y1[x];
                if (plakamisin(en, boy))    //Tespit edilen alanın Türkiye plakaları ile uyup uymadığının yapıldığı kısım.
                {
                    tespitEdilen++;
                    x1[x] += 4;     //Plakanın işlenmesi için olabildiğince plaka başında ki küçük mavi bölgeyi almamaya 
                    x2[x] -= 5;     //ve çerçeveyi geniş tutup plakanın tam olarak tespit edilmesi için çerçeve genişletme işlemlerini yaptım.
                    y1[x] -= 2;
                    y2[x] += 2;
                    
                    //Tespit edilen alanın etrafının çizilmesi.
                    grafik.DrawLines(kalem, new Point[] { new System.Drawing.Point(x1[x], y1[x]), new System.Drawing.Point(x2[x], y1[x]), new System.Drawing.Point(x2[x], y2[x]), new System.Drawing.Point(x1[x], y2[x]), new System.Drawing.Point(x1[x], y1[x]) });

                    //Plaka ile ilgili bilgiler resim üzerine yazma işlemi.
                    grafik.DrawString("Plaka " + tespitEdilen.ToString() + "\n(x,y): (" + x1[x].ToString() + "," + y1[x].ToString() + ")\nen, boy: " + (x2[x] - x1[x]).ToString() + ", " + (y2[x] - y1[x]).ToString(), new Font("Arial", 8), Brushes.Red, new System.Drawing.Point(x1[x], y2[x] + 2));
                                   

                    Rectangle kes = new Rectangle(x1[x], y1[x], x2[x] - x1[x], y2[x] - y1[x]);     //Tespit edilen plakanın kesilme işlemi.
                    var bmp = new Bitmap(kes.Width, kes.Height);
                    using (var gr = Graphics.FromImage(bmp))
                    {
                        //PictureBox'a atanma işlemi.
                        gr.DrawImage(pictureBox1.Image, new Rectangle(0, 0, bmp.Width, bmp.Height), kes, GraphicsUnit.Pixel);      
                    }

                    pictureBox8.Image = bmp;        //Kesilen yerin normal görüntüsünü picturebox'a atadım.
                    
                    Grayscale filter = new Grayscale(0.2125, 0.7154, 0.0721);       //OCR işleminin daha iyi sonuçlar vermesi için
                    Bitmap gray = filter.Apply(bmp);                                //kesilen alanı GRAY formatına dönüştürdüm.
                    pictureBox2.Image = gray;              
                                                                                               
                    //Burada çok farklı filtreler denedim hem tek tek hem karıştırarak en iyi sonucu GRAY formatına dönüştürülmüş
                    //resimler veriyor.Çok küçük ihtimalde Thresholding uygulanmış resimler veriyor ama Thresholding ve GRAY formatını
                    //karıştırınca sonuçlar eskisi kadar iyi olmuyor.Ayrıca kontrast arttırma gibi işlemler de denedim
                    //sonuçlar GRAY formatında ki kadar iyi olmadı.

                    if (pictureBox2.Image != null)
                     {
                         var ocr = new TesseractEngine("./tessdata", "eng", EngineMode.TesseractOnly);      //Tesseract kütüphanesi ile 
                         var page = ocr.Process(gray);                                                      //resimde ki metni okuma işlemi.
                         textBox1.Text = page.GetText(); 
                     }                     
                }
            }              
        }

        private bool plakamisin(int en, int boy)            //Tespit edilen bir alanın Türkiyede ki plaka boyutları ile kıyaslama fonksiyonu.
        {
            if (boy < 5) return false;            
            if (Math.Abs((float)en / boy - 4.5) < 2 && en > 50 && boy>20) return true;
            return false;
            
            //Kod bazen amblemleri ve arabanın ön kasasında ki şekillerin belli düzende ilerlemesinden dolayı oraları da algılayabiliyor,
            //bunu önlemek için olabildiğince aralıkları plakanın en boy oranı olabilecek değerler ve minimum plaka boyutları
            //arasında tutmaya çalıştım.
        }
    }
}
