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



namespace WindowsFormsApplication4
{
    public partial class Form1 : Form
    {       
        
        public Form1()
        {
            InitializeComponent();
            
            MessageBox.Show("Filtre Seçiniz ve Değer Giriniz.", "Bilgi");           //Başlangıçta mesaj kutusu gösterir.

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dosya = new OpenFileDialog();                               

            if (dosya.ShowDialog() == DialogResult.OK)          //Listboxta kaçıncı öğe seçiliyse o fonksiyonu çalıştırır.
            {
               
                if (listBox1.SelectedIndex == 0)
                {
                    
                    pictureBox1.Image = DonusturKarartma(new Bitmap(dosya.FileName));
                }
                else if (listBox1.SelectedIndex == 1)
                {
                    pictureBox1.Image = DonusturBulaniklik(new Bitmap(dosya.FileName));
                }
                else if (listBox1.SelectedIndex == 2)
                {
                    pictureBox1.Image = DonusturParlatma(new Bitmap(dosya.FileName));
                }
                else if (listBox1.SelectedIndex == 3)
                {
                    pictureBox1.Image = DonusturTers(new Bitmap(dosya.FileName));
                }
                else if (listBox1.SelectedIndex == 4)
                {
                    pictureBox1.Image = DonusturKabart(new Bitmap(dosya.FileName));
                }
                else if (listBox1.SelectedIndex == 5)
                {
                    pictureBox1.Image = DonusturKontrast(new Bitmap(dosya.FileName));
                }
                else if (listBox1.SelectedIndex == 6)
                {
                    pictureBox1.Image = DonusturGama(new Bitmap(dosya.FileName));
                }                
            }
        }

        Bitmap DonusturKarartma(Bitmap resim)
        {
            Bitmap yeniResim = new Bitmap(resim.Width, resim.Height);
            
           


            for (int i = 0; i < resim.Width; i++)           //Resimde ki bütün pikselleri dolaşıp rgb kodlarını toplayıp girilen değer böler.
            {
                for (int j = 0; j < resim.Height; j++)
                {
                    Color renk = resim.GetPixel(i, j);          //(i,j) de ki piksel değerlerini alır.
                    int yeniRenk = (renk.R + renk.G + renk.B) / int.Parse(textBox1.Text);
                    yeniResim.SetPixel(i,j,Color.FromArgb(yeniRenk,yeniRenk,yeniRenk));         //(i,j) deye yeni piksel değerlerini atar rgb olarak.
                   
                }
            }
            return yeniResim;
        }

        Bitmap DonusturBulaniklik(Bitmap resim)
        {
            Bitmap yeniResim = new Bitmap(resim.Width, resim.Height);
            int deger;

            deger = int.Parse(textBox1.Text);

            for (int i = deger; i <= resim.Width - deger; i++)
            {
                for (int j = deger; j < resim.Height - deger; j++)
                {

                    Color onX = resim.GetPixel(i - deger, j);
                    Color sonX = resim.GetPixel(i+deger, j);
                    Color onY = resim.GetPixel(i, j - deger);
                    Color sonY = resim.GetPixel(i, j + deger);

                    int ortR = (onX.R + sonX.R + onY.R + sonY.R) / 4;
                    int ortG = (onX.G + sonX.G + onY.G + sonY.G) / 4;
                    int ortB = (onX.B + sonX.B + onY.B + sonY.B) / 4;

                    yeniResim.SetPixel(i, j, Color.FromArgb(ortR, ortG, ortB));

                }
            }
            return yeniResim;
        }

        Bitmap DonusturParlatma(Bitmap resim)
        {
            Bitmap yeniResim = new Bitmap(resim.Width, resim.Height);

            for (int i = 0; i < resim.Width; i++)
            {
                for (int j = 0; j < resim.Height; j++)
                {
                    Color renk = resim.GetPixel(i, j);
                    int yeniRenk = (renk.R + renk.G + renk.B) * (int.Parse(textBox1.Text) / 2);
                    if (yeniRenk > 255)
                        yeniRenk = 255;
                    yeniResim.SetPixel(i, j, Color.FromArgb(yeniRenk, yeniRenk, yeniRenk));
                }
            }
            return yeniResim;
        }

        Bitmap DonusturTers(Bitmap resim)
        {
            Bitmap yeniResim = new Bitmap(resim.Width, resim.Height);

            for (int i = 0; i <= resim.Width - 1; i++)
            {
                for (int j = 0; j <= resim.Height - 1; j++)
                {
                    Color renk = resim.GetPixel(i, j);
                    renk = Color.FromArgb(renk.A, (byte)~renk.R, (byte)~renk.G, (byte)~renk.B);
                    yeniResim.SetPixel(i, j, renk);
                }
            }
            return yeniResim;
        }

        Bitmap DonusturKabart(Bitmap resim)
        {
            Bitmap yeniResim = new Bitmap(resim.Width, resim.Height);
            int sondeg = 0;

            for (int i = 1; i <= resim.Width - 1; i++)
            {
                for (int j = 1; j <= resim.Height - 1; j++)
                {
                    yeniResim.SetPixel(i, j, Color.DarkGray);
                }
            }

            for (int i = 1; i <= resim.Width - 1; i++)
            {
                for (int j = 1; j <= resim.Height - 1; j++)
                {                  
                    Color pixel = resim.GetPixel(i, j);
                    int topdeg = (pixel.R + pixel.G + pixel.B);
                    if (sondeg == 0) sondeg = (pixel.R + pixel.G + pixel.B);
                    int fark;
                    if (topdeg > sondeg)
                    {
                        fark = topdeg - sondeg;
                    }
                    else
                    {
                        fark = sondeg - topdeg;
                    }
                    if (fark > 100)
                    {
                        yeniResim.SetPixel(i, j, Color.Gray);
                        sondeg = topdeg;
                    }                                 
                }
            }

            for (int j = 1; j <= resim.Width - 1; j++)
            {
                for (int i = 1; i <= resim.Height - 1; i++)
                {
                    Color pixel = resim.GetPixel(j, i);
                    int topdeg = (pixel.R + pixel.G + pixel.B);
                    if (sondeg == 0) sondeg = (pixel.R + pixel.G + pixel.B);
                    int fark;
                    if (topdeg > sondeg)
                    {
                        fark = topdeg - sondeg;
                    }
                    else
                    {
                        fark = sondeg - topdeg;
                    }

                    if (fark > 100)
                    {
                        yeniResim.SetPixel(j, i, Color.Gray);
                        sondeg = topdeg;
                    }
                }
            }
            return yeniResim;
        }

        Bitmap DonusturKontrast(Bitmap resim)
        {
            Bitmap yeniResim = new Bitmap(resim.Width, resim.Height);
            float kontrast = 0;
            kontrast = 0.04f * int.Parse(textBox1.Text);

            Graphics g = Graphics.FromImage(yeniResim);
            ImageAttributes ia = new ImageAttributes();

            ColorMatrix cm = new ColorMatrix(new float[][] {
                                new float[] {kontrast,0f,0f,0f,0f},
                                new float[] {0f,kontrast,0f,0f,0f },
                                new float[] {0f,0f,kontrast,0f,0f },
                                new float[] {0f,0f,0f,1f,0f },
                                new float[] {0.001f,0.001f,0.001f,0f,1f } });
            ia.SetColorMatrix(cm);
            g.DrawImage(resim, new Rectangle(0, 0, resim.Width, resim.Height), 0, 0, resim.Width, resim.Height, GraphicsUnit.Pixel, ia);
            g.Dispose();
            ia.Dispose();

            return yeniResim;
        }

        Bitmap DonusturGama(Bitmap resim)
        {
            Bitmap yeniResim = new Bitmap(resim.Width, resim.Height);
            float gama = 1;

            gama = 0.04f * int.Parse(textBox1.Text);

            Graphics g = Graphics.FromImage(yeniResim);
            ImageAttributes ia = new ImageAttributes();

            ia.SetGamma(gama);

            g.DrawImage(resim, new Rectangle(0, 0, resim.Width, resim.Height), 0, 0, resim.Width, resim.Height, GraphicsUnit.Pixel, ia);
            g.Dispose();
            ia.Dispose();

            return yeniResim;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}

