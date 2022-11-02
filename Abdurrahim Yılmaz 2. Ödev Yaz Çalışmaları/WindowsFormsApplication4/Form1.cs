
//Abdurrahim Yılmaz 1606A041 a.rahim.yilmaz@hotmail.com

using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;
//using System.Drawing.Imaging;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge;
using AForge.Imaging.Filters;
using AForge.Imaging;
//using AForge.Vision;
//using AForge.Vision.Motion;
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
            VideoCaptureDevice(cihazlar[0].MonikerString);          //Kullancağım kameraya varolan kamerayı atadım.(Webcam)

            kamera.NewFrame += new NewFrameEventHandler(kamera_klonlama);           //Kamerayı klonladım bunu pictureboxda gösterebilmek için.
            kamera.Start();         //Kamerayı başlattım.                                        

        }
        
        void kamera_klonlama(object sender, NewFrameEventArgs eventArgs)            //Kamera klonlama fonksiyonu.
        {
            Bitmap klon = (Bitmap)eventArgs.Frame.Clone();
            Bitmap cisim = (Bitmap)eventArgs.Frame.Clone();

            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;         //Pictureboxın boyutunu otomatik çekim çözünürlüğüne ayarladım.
            pictureBox1.Image = klon;

            /*ColorFiltering filtre = new ColorFiltering();           //Kutu içine alınacak renklerin aralığını belirledim.
            filtre.Red = new IntRange(100, 255);            //Eğer aralıkları daraltırsam blobları tam olarak görmüyor,
            filtre.Blue = new IntRange(0, 25);              //eğer aralıkları genişletirsem webcam kalitesi kötü olmasından dolayı
            filtre.Green = new IntRange(0, 25);             //insan tenini de blob olarak görebiliyor bazen.İnsan tenini görme ihtimalini azaltmak için
            filtre.ApplyInPlace(cisim);                     //Blue ve Green değerlerini daraltıp Red değerini arttırsak belli oranlarda ihtimal düşüyor.
                                                            //Bu aralıkta iyi çalışıyor.*/
            
        }

        /*public void kutuicineal(Bitmap image)
        {
            BlobCounter blobCounter = new BlobCounter();            //Blob yöntemi ile blobları tespit ederek onları kutu içine aldım.
            blobCounter.FilterBlobs = true;         //Blobların filtresini açtım.
            blobCounter.MinHeight = 3;          //Yükseklik olarak minimum 3
            blobCounter.MinWidth = 3;           //Genişlik olarak minimum 3 piksel üzerinde ki blobları kutu içine aldım.
            blobCounter.ObjectsOrder = ObjectsOrder.Size;                       

            Grayscale grayFilter = new Grayscale(0.2125, 0.7154, 0.0721);
            Bitmap grayImage = grayFilter.Apply(image);

            blobCounter.ProcessImage(grayImage);            //Resmi gri filtre uyguladım.
            Rectangle[] rects = blobCounter.GetObjectsRectangles();         //Bu filtre üzerinden blobların rectangle verisini elde ettim.

            if (rects.Length > 0)           //Eğer rectangle 0'dan büyükse yani blob varsa aşağıda ki komutları çalıştıracak.
            {
                Rectangle objectRect = rects[0];               //İlk rectangle verisi üzerinden işlem yapacağız bu da kırmızı cisim oluyor.
                Graphics g = pictureBox1.CreateGraphics();           
                using (Pen pen = new Pen(Color.Red, 3))         //Kutunun rengini belirledim.
                {
                    g.DrawRectangle(pen, objectRect);           //Kutu içine alma fonksiyonu.
                }                

                Blob[] blobs = blobCounter.GetObjectsInformation();         //Blobların verisini aldım.
                int alan = blobs[0].Area;                //İlk blobun alanını piksel türünde "alan" değişkenine atadım.

                using(Font myFont = new Font("Arial",14))           
                {
                    g.DrawString(alan.ToString(), myFont, Brushes.Red, 1, 1);           //Kutunun alanını ekranın sol üstüne yazdırdım.
                }               
            }           
        }*/
    }
}
