using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Yılan_oyunu
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        List<Kareinfo> listKare = new List<Kareinfo>();
        List<Kareinfo> listuzuv = new List<Kareinfo>();

        yilaninfo yilaninf = null;

        int yon = 2; // varsayılan başlangıç yönü. 1 yukarı, 2 sağ, 3 aşağı, 4 sola
        int toplamkaresayisi = 625;
        bool gameover = false;
        bool yemvar = false;

        private void Form1_Load(object sender, EventArgs e)
        {
            int karekenaruzunluk = 18; // panel 500*500. 25'e 25 karelere bölersek 500/25=20 piksel. dış çerçevede bölen piksel 1'er piksel
            int kareX = 1;
            int kareY = 1;
            int margin = 2;

            for(int i=0 ; i<toplamkaresayisi ; i++) //ekranı karelere bölüyoruz
            {
                Kareinfo kareinfo = new Kareinfo(this.panel1, new Point(kareX, kareY), new Size(karekenaruzunluk, karekenaruzunluk),i);

                listKare.Add(kareinfo);

                kareX += karekenaruzunluk + margin; 

                if ((i+1)%25==0)
                {
                    kareX = 1;
                    kareY += karekenaruzunluk + margin;
                }
            }
            sinirekle();
            yilaninf = new yilaninfo(listKare,listuzuv);
        }

        void newGame()
        {
            timer1.Stop();

            foreach(Kareinfo item in listKare) //oyunu sıfırlıyoruz
            {
                if(!item.sinir)
                {
                    item.uzuvyapma();
                    item.yemyapma();
                }
            }

            gameover = false;
            yon = 2;
            yemvar = false;
            lblSkor.Text = "0";
            listuzuv.Clear();
            timer1.Interval = 180;
            yilaninf = new yilaninfo(listKare, listuzuv);
        }

        void yemEkle()
        {
            if(yemvar)//yem varsa görev iptal
            {
                return;
            }

            Random rnd = new Random(); //rastgele yem 
            int indis = 0;
            bool durum = false;

            while(durum==false) //yem yoksa
            {
                indis = rnd.Next(0, toplamkaresayisi); //ratgele yem at
                durum = true;

                if(this.listKare[indis].uzuv||this.listKare[indis].sinir) //sınırda ya da yılan üstündeyse tekrar yem oluştur
                {
                    durum = false;
                }
            }

            if (durum) //sıkıntı yoksa kareye yem bilgisini ekle
            {
                this.listKare[indis].yemyap();
                this.yemvar = true;
            }
        }

        void sinirekle() //sınırları çiziyoruz
        {
            for (int i = 0; i <= 24; i+=1) //her satır ve sütun 25 kare. ilk kare 0
            {
                listKare[i].siniryap();
            }
            for (int i = 0; i <= 599; i += 25) //sol duvar
            {
                listKare[i].siniryap();
            }
            for (int i = 599; i <= 624; i += 1) //taban duvar
            {
                listKare[i].siniryap();
            }
            for (int i = 24; i <= 624; i += 25) //sağ duvar
            {
                listKare[i].siniryap();
            }
        }

        class Kareinfo //her bir kare kendi bilgisini oluşturacak
        {
            public Point location { get; set; }
            public Size size { get; set; }
            public Color backcolor { get; set; }
            public PictureBox pictureBox { get; set; }
            public int indis { get; set; }
            public bool uzuv { get; set; }
            public bool yem { get; set; }
            public bool sinir { get; set; }
            public Panel panel { get; set; }

            public Kareinfo(Panel panel,Point location, Size size, int indis)
            {
                this.panel = panel;
                this.location = location;
                this.size = size;
                this.indis = indis;
                this.uzuv = false;
                this.yem = false;
                this.sinir = false;
                this.backcolor = Color.Black;
                picturBoxAdd(); //her kareye bir picturebox. yukarıda karelere bölme işi yapılırken her döngüde çağrılıyor
            }

            void picturBoxAdd()
            {
                pictureBox = new PictureBox();
                pictureBox.Location = this.location;
                pictureBox.Size = this.size;
                pictureBox.BackColor = this.backcolor;
                this.panel.Controls.Add(pictureBox);
            }

            public void uzuvyap() //tanım
            {
                this.pictureBox.BackColor = Color.GreenYellow;
                this.uzuv = true;
            }
            public void uzuvyapma() //tanım
            {
                this.pictureBox.BackColor = this.backcolor;
                this.uzuv = false;
            }
            public void yemyap() //tanım
            {
                this.pictureBox.BackColor = Color.Red;
                this.yem = true;
            }
            public void yemyapma() //tanım
            {
                this.pictureBox.BackColor = this.backcolor;
                this.yem = false;
            }
            public void siniryap() //tanım
            {
                this.pictureBox.BackColor = Color.Purple;
                this.sinir = true;
            }
        }

        class yilaninfo
        {
            public int startposition = 28; //kafanın bulunduğu karenin numarası
            public int yon { get; set; }
            public List<Kareinfo> listKare { get; set; }
            public List<Kareinfo> listuzuv { get; set; }

            public yilaninfo(List<Kareinfo> listKare, List<Kareinfo> listuzuv)
            {
                this.listKare = listKare;
                this.listuzuv = listuzuv;

                this.listKare[26].uzuvyap(); //karelere uzuv ekle
                this.listKare[27].uzuvyap();
                this.listKare[28].uzuvyap();

                this.listuzuv.Add(this.listKare[26]); //uzuv listesine kareleri ekle
                this.listuzuv.Add(this.listKare[27]);
                this.listuzuv.Add(this.listKare[28]);
            }

            public int yurut(int yon)
            {
                this.yon = yon;

                switch(yon)
                {
                    case 1:
                        startposition -= 25; //her satır 25 kare old. için düşeyde kare numaraları 25'er
                        break;
                    case 2:
                        startposition += 1;
                        break;
                    case 3:
                        startposition += 25;
                        break;
                    case 4:
                        startposition -= 1;
                        break;
                    default:
                        break;
                }

                if (this.listKare[startposition].uzuv||this.listKare[startposition].sinir) //kafa, yani startposition sınır ya da uzuzla çakışırsa
                {
                    return 0; //hata varsa oyun bitiyor
                }
                else
                {
                    this.listKare[startposition].uzuvyap(); //çakışma yoksa kafa artık uzuv
                    this.listuzuv.Add(this.listKare[startposition]);

                    if (this.listKare[startposition].yem) //yem kafa ile çakışırsa
                    {
                        this.listKare[startposition].yem = false;  //yemi sil
                        
                        return 2; //yem yenmiş ise
                    }
                    else
                    {
                        this.listKare[this.listuzuv[0].indis].uzuvyapma(); //yem yenmemiş ise uzuv özelliklerini sil
                        this.listuzuv.RemoveAt(0); //uzvu sil

                        return 1; //kuyruk silinmiş ise
                    }
                }
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            yemEkle();
            int sonuc = yilaninf.yurut(yon);

            switch(sonuc)
            {
                case 0:
                    timer1.Stop();
                    gameover = true;
                    MessageBox.Show("öldün çıq :p");
                    break;

                case 1:
                    break;

                case 2:
                    yemvar = false; //yem yendi
                    lblSkor.Text=Convert.ToString(Convert.ToInt32(lblSkor.Text)+1);
                    if(timer1.Interval>=100)
                    {
                        timer1.Interval -= 2;
                    }
                    break;

                default:
                    break;
            }
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            switch(keyData) //tuş ayarlamaları
            {
                case Keys.Up:
                    if (yilaninf.yon != 3)
                    {
                        yon = 1;
                    }
                    break;
                case Keys.Right :
                    if (yilaninf.yon != 4)
                    {
                        yon = 2;
                    }
                    break;
                case Keys.Down:
                    if (yilaninf.yon != 1)
                    {
                        yon = 3;
                    }
                        break;
                case Keys.Left:
                    if (yilaninf.yon != 2)
                    {
                        yon = 4;
                    }
                    break;
                case Keys.B:
                    if (gameover == false)
                    {
                        timer1.Start();
                    }
                    break;
                case Keys.D:
                    if (gameover == false)
                    {
                        timer1.Stop();
                    }
                    break;
                case Keys.N:
                    newGame();
                    break;
                case Keys.S:
                    MessageBox.Show("SERKAN APAYDIN tarafından üretilmiştir");
                    break;
            }
            return base.ProcessDialogKey(keyData);
        }
    }
}