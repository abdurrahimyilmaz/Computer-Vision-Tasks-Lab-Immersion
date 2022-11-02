
//Abdurrahim Yılmaz 1606A041 a.rahim.yilmaz@hotmail.com

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge;
using AForge.Imaging.Filters;
using AForge.Imaging;
using AForge.Vision;
using AForge.Vision.Motion;
using AForge.Math.Geometry;
//Kullanılmayan kütüphaneleri kaldırdım.

namespace WindowsFormsApplication4
{
    public partial class Form1 : Form
    {

        private FilterInfoCollection cihazlar;   //Bilgisayarda ki kameraların verilerini cihazlar adı altında topladım.
        private VideoCaptureDevice kamera;      //Kullacağım kameraya "kamera" adını verdim.
        Bitmap resim;

        public Form1()
        {
            
            InitializeComponent();
           
        }
                
        private void Form1_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;            //"kutuicineal" fonksiyonunda ki thread çakışmasını önledim.
            cihazlar = new
            FilterInfoCollection(FilterCategory.VideoInputDevice);

            kamera = new
            VideoCaptureDevice(cihazlar[0].MonikerString);          //Kullanacağım kameraya varolan kamerayı atadım.(Webcam)

            kamera.NewFrame += new NewFrameEventHandler(kamera_klonlama);           //Kamerayı klonladım bunu pictureboxda gösterebilmek için.
            kamera.Start();         //Kamerayı başlattım.                                        

        }
        
        void kamera_klonlama(object sender, NewFrameEventArgs eventArgs)            //Kamera klonlama fonksiyonu.
        {
            Bitmap klon = (Bitmap)eventArgs.Frame.Clone();
            Bitmap cisim_kirmizi = (Bitmap)eventArgs.Frame.Clone();         //Birden fazla renkte nesneyi işleyebilmek için
            Bitmap cisim_mavi = (Bitmap)eventArgs.Frame.Clone();            //Her renk için ayrı ayrı Bitmap sınıfı oluşturdum.
            Bitmap cisim_sari = (Bitmap)eventArgs.Frame.Clone();            

            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;         //Pictureboxın boyutunu otomatik çekim çözünürlüğüne ayarladım.
            pictureBox1.Image = klon;


            kirmizi_filtre(cisim_kirmizi);          //Her bir renk için ayrı ayrı fonksiyon yazdım.
            mavi_filtre(cisim_mavi);            
            sari_filtre(cisim_sari);                          
        }

        public void kirmizi_filtre(Bitmap image)            //Kırmızı rengin filtre aralığı
        {
            ColorFiltering filtre = new ColorFiltering();
            filtre.Red = new IntRange(100, 255);           //Bu renkte yüzü algılamadığı için aralığı daha geniş tuttum.
            filtre.Blue = new IntRange(0, 75);           //Böylece blob algılamama gibi bir sorun olmuyacak kırmızı renkte ki gibi.
            filtre.Green = new IntRange(0, 75);
            filtre.ApplyInPlace(image);
            
            kutuicineal_kirmizi(image);
        }

        public void mavi_filtre(Bitmap image)           //Mavi rengin filtre aralığı
        {
            ColorFiltering filtre = new ColorFiltering();
            filtre.Red = new IntRange(0, 75);           //Bu renkte yüzü algılamadığı için aralığı daha geniş tuttum.
            filtre.Blue = new IntRange(100, 255);           //Böylece blob algılamama gibi bir sorun olmuyacak kırmızı renkte ki gibi.
            filtre.Green = new IntRange(0, 75);
            filtre.ApplyInPlace(image);
            kutuicineal_mavi(image);
        }

        public void sari_filtre(Bitmap image)           //Sarı rengin filtre aralığı
        {
            ColorFiltering filtre = new ColorFiltering();
            filtre.Red = new IntRange(150, 255);            //Bu renkte yüzü algılama yüzü algılama sorunu olduğu için blob piksel aralıklarını
            filtre.Blue = new IntRange(0, 100);         //kırmızı renkte ki gibi geniş tuttum ve filtre aralığını deneme yanılma ile en iyi
            filtre.Green = new IntRange(150, 255);          //aralığı bulmaya çalıştım.
            filtre.ApplyInPlace(image);
            kutuicineal_sari(image);
        }


        public void kutuicineal_kirmizi(Bitmap image)           //Her biri için ayrı ayrı kutu fonksiyonu yazdım. Kırmızı rengin kutu fonksiyonu
        {
            BlobCounter blobCounter = new BlobCounter();            //Blob yöntemi ile blobları tespit ederek onları kutu içine aldım.
            blobCounter.FilterBlobs = true;         //Blobların filtresini açtım.
            blobCounter.MinHeight = 7;          //Alınacak minimum ve maksimum blob genişliği.
            blobCounter.MinWidth = 7;           //Bu genişliği biraz arttırarak kamera da ufak tefek yerleri göstermesini önledim.
            blobCounter.ObjectsOrder = ObjectsOrder.Size;

            Grayscale grayFilter = new Grayscale(0.2125, 0.7154, 0.0721);
            Bitmap grayImage = grayFilter.Apply(image);

            blobCounter.ProcessImage(grayImage);            //Resmi gri filtre uyguladım.
            Rectangle[] rects = blobCounter.GetObjectsRectangles();         //Bu filtre üzerinden blobların rectangle verisini elde ettim.

            AForge.Point agirlik_merkezi = new AForge.Point();          //Resmin ortasında yazması için ağırlık merkezi oluşturdum.

            if (rects.Length > 0)           //Eğer rectangle 0'dan büyükse yani blob varsa aşağıda ki komutları çalıştıracak.
            {
                Rectangle objectRect = rects[0];               //İlk rectangle verisi üzerinden işlem yapacağız bu da kırmızı cisim oluyor.
                Graphics g = pictureBox1.CreateGraphics();
                using (Pen pen = new Pen(Color.Red, 3))         //Kutunun rengini mavi olarak belirledim belirledim.
                {
                    g.DrawRectangle(pen, objectRect);           //Kutu içine alma fonksiyonu.
                }

                Blob[] blobs = blobCounter.GetObjectsInformation();         //Blobların verisini aldım.
                int alan = blobs[0].Area;                //İlk blobun alanını piksel türünde "alan" değişkenine atadım.
                agirlik_merkezi = blobs[0].CenterOfGravity;         //Ağırlık merkezinin koordinatlarını aldım.

                using (Font myFont = new Font("Arial", 15))
                {
                    g.DrawString(alan.ToString(), myFont, Brushes.Red, agirlik_merkezi.X, agirlik_merkezi.Y);           //Kutunun alanını ağırlık merkezine yazdırdım.
                    g.DrawString("Kırmızı", myFont, Brushes.Red, agirlik_merkezi.X, agirlik_merkezi.Y + 15);            //Rengin adını alan ile çakışmaması için y düzleminde ağırlık merkezinden 15 piksel yukarı yazdırdım.
                }
            }
        }

        public void kutuicineal_mavi(Bitmap image)          //Mavi rengin kutu fonksiyonu.
        {
            BlobCounter blobCounter = new BlobCounter();            //Blob yöntemi ile blobları tespit ederek onları kutu içine aldım.
            blobCounter.FilterBlobs = true;         //Blobların filtresini açtım.
            blobCounter.MinHeight = 7;          //Alınacak minimum ve maksimum blob genişliği.
            blobCounter.MinWidth = 7;           //Bu genişliği biraz arttırarak kamera da ufak tefek yerleri göstermesini önledim.
            blobCounter.ObjectsOrder = ObjectsOrder.Size;

            Grayscale grayFilter = new Grayscale(0.2125, 0.7154, 0.0721);
            Bitmap grayImage = grayFilter.Apply(image);

            blobCounter.ProcessImage(grayImage);            //Resmi gri filtre uyguladım.
            Rectangle[] rects = blobCounter.GetObjectsRectangles();         //Bu filtre üzerinden blobların rectangle verisini elde ettim.

            AForge.Point agirlik_merkezi = new AForge.Point();          //Resmin ortasında yazması için ağırlık merkezi oluşturdum.

            if (rects.Length > 0)           //Eğer rectangle 0'dan büyükse yani blob varsa aşağıda ki komutları çalıştıracak.
            {
                Rectangle objectRect = rects[0];               //İlk rectangle verisi üzerinden işlem yapacağız bu da kırmızı cisim oluyor.
                Graphics g = pictureBox1.CreateGraphics();
                using (Pen pen = new Pen(Color.Blue, 3))         //Kutunun rengini mavi olarak belirledim belirledim.
                {
                    g.DrawRectangle(pen, objectRect);           //Kutu içine alma fonksiyonu.
                }

                Blob[] blobs = blobCounter.GetObjectsInformation();         //Blobların verisini aldım.
                int alan = blobs[0].Area;                //İlk blobun alanını piksel türünde "alan" değişkenine atadım.
                agirlik_merkezi = blobs[0].CenterOfGravity;         //Ağırlık merkezinin koordinatlarını aldım.

                using (Font myFont = new Font("Arial", 15))
                {
                    g.DrawString(alan.ToString(), myFont, Brushes.Blue, agirlik_merkezi.X, agirlik_merkezi.Y);           //Kutunun alanını ağırlık merkezine yazdırdım.
                    g.DrawString("Mavi", myFont, Brushes.Blue, agirlik_merkezi.X, agirlik_merkezi.Y+15);            //Rengin adını alan ile çakışmaması için y düzleminde ağırlık merkezinden 15 piksel yukarı yazdırdım.
                }
            }
        }

        public void kutuicineal_sari(Bitmap image)          //Sarı rengin kutu fonksiyonu.
        {
            BlobCounter blobCounter = new BlobCounter();            //Blob yöntemi ile blobları tespit ederek onları kutu içine aldım.
            blobCounter.FilterBlobs = true;         //Blobların filtresini açtım.
            blobCounter.MinHeight = 11;          //Alınacak minimum ve maksimum blob genişliği.
            blobCounter.MinWidth = 11;           //Bu genişliği biraz arttırarak kamera da ufak tefek yerleri göstermesini önledim.
            blobCounter.ObjectsOrder = ObjectsOrder.Size;
            
            Grayscale grayFilter = new Grayscale(0.2125, 0.7154, 0.0721);
            Bitmap grayImage = grayFilter.Apply(image);

            blobCounter.ProcessImage(grayImage);            //Resmi gri filtre uyguladım.
            Rectangle[] rects = blobCounter.GetObjectsRectangles();         //Bu filtre üzerinden blobların rectangle verisini elde ettim.

            AForge.Point agirlik_merkezi = new AForge.Point();          //Resmin ortasında yazması için ağırlık merkezi oluşturdum.

            if (rects.Length > 0)           //Eğer rectangle 0'dan büyükse yani blob varsa aşağıda ki komutları çalıştıracak.
            {
                Rectangle objectRect = rects[0];               //İlk rectangle verisi üzerinden işlem yapacağız bu da kırmızı cisim oluyor.
                Graphics g = pictureBox1.CreateGraphics();
                using (Pen pen = new Pen(Color.Yellow, 3))         //Kutunun rengini sarı olarak belirledim.
                {
                    g.DrawRectangle(pen, objectRect);           //Kutu içine alma fonksiyonu.
                }

                Blob[] blobs = blobCounter.GetObjectsInformation();         //Blobların verisini aldım.
                int alan = blobs[0].Area;                //İlk blobun alanını piksel türünde "alan" değişkenine atadım.
                agirlik_merkezi = blobs[0].CenterOfGravity;         //Ağırlık merkezinin koordinatlarını aldım.

                using (Font myFont = new Font("Arial", 15))
                {
                    g.DrawString(alan.ToString(), myFont, Brushes.Yellow, agirlik_merkezi.X, agirlik_merkezi.Y);           //Kutunun alanını ağırlık merkezine yazdırdım.
                    g.DrawString("Sarı", myFont, Brushes.Yellow, agirlik_merkezi.X, agirlik_merkezi.Y + 15);            //Rengin adını alan ile çakışmaması için y düzleminde ağırlık merkezinden 15 piksel yukarı yazdırdım.
                }
            }
        }
    }
}
