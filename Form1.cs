
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace Deepportaled
{
    public partial class Form1 : Form
    {
        // Ресурсы игры
        public int portal_kolvo = 0;
        public static Random rand = new Random();
        public int gameTime = 0;
        Bitmap scene;
        Graphics g;
        Bitmap map;
        Graphics g1;
        Pen blackpen;
        Player player;
        Enemy enemy;
        Brush blackbrush = new SolidBrush(Color.Black);
        Portal[] portal = new Portal[10];
        Font stand = new Font(FontFamily.GenericSansSerif, 50);
        List<Enemy> enemies = new List<Enemy>();
        bool keyW = false;
        bool keyA = false;
        bool keyS = false;
        bool keyD = false;
        bool keySpace = false;
        bool[,] boolmap;
        bool[,] vidimmap;
        int enemyes = 0;
        public Form1()
        {
            InitializeComponent();
            GameTimer.Interval = 1000 / 60;
            GameTimer.Start();
            scene = new Bitmap(canvas.Width, canvas.Height);
            map = new Bitmap(scene);
            g = Graphics.FromImage(scene);
            g1 = Graphics.FromImage(map);
            blackpen = new Pen(Color.Black);
            generationMap();
        }
        private void generationMap()
        {
            portal_kolvo = 0;
            room[] rooms = new room[16];
            rooms[0] = new room(10, 10, scene.Height - 10, scene.Width - 10);
            vidimmap = new bool[canvas.Height, canvas.Width];
            for (int i = 0; i < canvas.Height; i++)
            {
                for (int j = 0; j < canvas.Width; j++)
                {
                    vidimmap[i, j] = false;
                }
            }
            {
                for (int j = 4; j > 0; j--)
                {
                    for (int i = 0; i < 16; i = i + (int)Math.Pow(2, j))
                    {
                        rooms[i + (int)Math.Pow(2, j - 1)] = rooms[i].dilenie();
                    }
                }
                //нахождения для каждой комнаты её соседа
                for (int i = 0; i < 16; i++)//i- комната у которой проверяем
                {
                    for (int j = 0; j < 16; j++)
                    {
                        //левая сторона 
                        if ((rooms[i].verh.X == rooms[j].niz.X) && (((rooms[i].verh.Y <= rooms[j].verh.Y) && (rooms[i].niz.Y >= rooms[j].verh.Y)) || ((rooms[i].verh.Y <= rooms[j].niz.Y) && (rooms[i].niz.Y >= rooms[j].niz.Y))))
                        {
                            rooms[i].conected(j);
                            rooms[i].conection++;
                        }
                        //нижняя
                        if ((rooms[i].niz.Y == rooms[j].verh.Y) && (((rooms[i].niz.X >= rooms[j].niz.X) && (rooms[i].verh.X <= rooms[j].niz.X)) || ((rooms[i].niz.X >= rooms[j].verh.X) && (rooms[i].verh.X <= rooms[j].verh.X))))
                        {
                            rooms[i].conected(j);
                            rooms[i].conection++;
                        }
                        //правая
                        if ((rooms[i].niz.X == rooms[j].verh.X) && (((rooms[i].niz.Y >= rooms[j].niz.Y) && (rooms[i].verh.Y <= rooms[j].niz.Y)) || ((rooms[i].niz.Y >= rooms[j].verh.Y) && (rooms[i].verh.Y <= rooms[j].verh.Y))))
                        {
                            rooms[i].conected(j);
                            rooms[i].conection++;
                        }
                        //верхняя
                        if ((rooms[i].verh.Y == rooms[j].niz.Y) && (((rooms[i].verh.X <= rooms[j].verh.X) && (rooms[i].niz.X >= rooms[j].verh.X)) || ((rooms[i].verh.X <= rooms[j].niz.X) && (rooms[i].niz.X >= rooms[j].niz.X))))
                        {
                            rooms[i].conected(j);
                            rooms[i].conection++;
                        }
                    }
                }
                //уменьшение комнат дабы создать пространство между ними и там в дальнейшем развернуть клеточный автомат...
                for (int i = 0; i < 16; i++)
                {
                    room vrem;
                    vrem = rooms[i];
                    rooms[i].verh.X = vrem.verh.X + (vrem.niz.X - vrem.verh.X) / 100 * rand.Next(15, 30);
                    rooms[i].verh.Y = vrem.verh.Y + (vrem.niz.Y - vrem.verh.Y) / 100 * rand.Next(15, 30);
                    rooms[i].niz.X = vrem.niz.X - (vrem.niz.X - vrem.verh.X) / 100 * rand.Next(15, 30);
                    rooms[i].niz.Y = vrem.niz.Y - (vrem.niz.Y - vrem.verh.Y) / 100 * rand.Next(15, 30);
                }
                //создаение проходов между комнатами
                int[] vrem1 = new int[16];
                int size = 0;
                int randomka;
                vrem1[size] = rand.Next(16);
                rooms[vrem1[size]].peseheno = true;
                size++;
                PointF nach_portal = new PointF();
                bool nach = false;
                bool vilet = false;
                int prosh = vrem1[size - 1];
                while (size != 0)
                {
                    size--;
                    int tekush = vrem1[size];
                    vilet = false;
                    randomka = rand.Next(rooms[tekush].conection);
                    if (rooms[rooms[tekush].conect[randomka]].peseheno)
                    {
                        for (int o = 0; o < rooms[tekush].conection; o++)
                        {
                            if (rooms[rooms[tekush].conect[o]].peseheno) { }
                            else
                            {
                                vrem1[size] = tekush;
                                size++;
                                prosh = tekush;
                                vilet = true;
                                break;
                            }
                        }
                        if (prosh == vrem1[size])
                        {
                            if (vilet) { }
                            else
                            {
                                if (nach)
                                {
                                    portal[portal_kolvo] = new Portal(nach_portal.Y, nach_portal.X, (rooms[tekush].verh.Y + (rooms[tekush].niz.Y - rooms[tekush].verh.Y) / 2), (rooms[tekush].verh.X + (rooms[tekush].niz.X - rooms[tekush].verh.X) / 2));
                                    nach = false;
                                    portal_kolvo++;
                                }
                                else
                                {
                                    nach = true;
                                    nach_portal.X = (rooms[tekush].verh.X + (rooms[tekush].niz.X - rooms[tekush].verh.X) / 2);
                                    nach_portal.Y = (rooms[tekush].verh.Y + (rooms[tekush].niz.Y - rooms[tekush].verh.Y) / 2);
                                }
                            }
                        }
                    }
                    else
                    {
                        vrem1[size] = tekush;
                        size++;
                        rooms[tekush].soedinen[rooms[tekush].soedinenie] = rooms[tekush].conect[randomka];
                        rooms[tekush].soedinenie++;
                        rooms[rooms[tekush].conect[randomka]].soedinen[rooms[rooms[tekush].conect[randomka]].soedinenie] = tekush;
                        rooms[rooms[tekush].conect[randomka]].soedinenie++;
                        tekush = rooms[tekush].conect[randomka];
                        rooms[tekush].peseheno = true;
                        vrem1[size] = tekush;
                        size++;
                        prosh = tekush;
                    }
                }
                //Переход от абстрактных комнат к карте boolmap, состоящей из пикселей
                boolmap = new bool[scene.Height, scene.Width];
                for (int i = 0; i < scene.Height; i++)
                {
                    for (int j = 0; j < scene.Width; j++)
                    {
                        boolmap[i, j] = true;
                    }
                }
                //
                for (int i = 0; i < 16; i++)
                {
                    for (int h = (int)rooms[i].verh.Y; h < rooms[i].niz.Y; h++)
                    {
                        for (int w = (int)rooms[i].verh.X; w < rooms[i].niz.X; w++)
                        {
                            boolmap[h, w] = false;
                        }
                    }
                    //
                    //
                    for (int q = 0; q < rooms[i].soedinenie; q++)
                    {
                        PointF first = new PointF((rooms[i].verh.X + (rooms[i].niz.X - rooms[i].verh.X) / 2), (rooms[i].verh.Y + (rooms[i].niz.Y - rooms[i].verh.Y) / 2));
                        PointF second = new PointF((rooms[rooms[i].soedinen[q]].verh.X + (rooms[rooms[i].soedinen[q]].niz.X - rooms[rooms[i].soedinen[q]].verh.X) / 2), (rooms[rooms[i].soedinen[q]].verh.Y + (rooms[rooms[i].soedinen[q]].niz.Y - rooms[rooms[i].soedinen[q]].verh.Y) / 2));
                        if ((second.X - first.X) > 0)
                        {
                            if ((second.Y - first.Y) > 0)
                            {
                                Double otnoshenie_storon = (second.X - first.X) / ((second.Y - first.Y));
                                //если х>y
                                if (1 < (otnoshenie_storon))
                                {
                                    int v = (int)first.Y;
                                    for (int w = (int)first.X; w < second.X; w++)
                                    {
                                        while ((v < second.Y) && ((w - first.X) > otnoshenie_storon * (v - first.Y))) { v++; }
                                        for (int e = -15; e < 16; e++)
                                        {
                                            boolmap[v + e, w] = false;
                                        }
                                    }
                                }
                                else
                                {
                                    otnoshenie_storon = (second.Y - first.Y) / (second.X - first.X);
                                    int v = (int)first.X;
                                    for (int w = (int)first.Y; w < second.Y; w++)
                                    {
                                        while ((v < second.X) && ((w - first.Y) > otnoshenie_storon * (v - first.X))) { v++; }
                                        for (int e = -15; e < 16; e++)
                                        {
                                            boolmap[w, v + e] = false;
                                        }
                                    }
                                }
                            }
                            else
                            {//ecли first.y>second.y
                                Double otnoshenie_storon = (second.X - first.X) / ((first.Y - second.Y));
                                if (1 < (otnoshenie_storon))
                                {
                                    int v = (int)first.Y;
                                    for (int w = (int)first.X; w < second.X; w++)
                                    {
                                        while ((v > second.Y) && ((w - first.X) > otnoshenie_storon * Math.Abs(v - first.Y))) { v--; }
                                        for (int e = -15; e < 16; e++)
                                        {
                                            boolmap[v + e, w] = false;
                                        }
                                    }
                                }
                                else
                                {
                                    otnoshenie_storon = (first.Y - second.Y) / (second.X - first.X);
                                    int v = (int)first.X;
                                    for (int w = (int)first.Y; w > second.Y; w--)
                                    {
                                        while ((v < second.X) && (Math.Abs(w - first.Y) > otnoshenie_storon * (v - first.X))) { v++; }
                                        for (int e = -15; e < 16; e++)
                                        {
                                            boolmap[w, v + e] = false;
                                        }
                                    }
                                }
                            }
                        }
                        //добавть исход, если комнаты находятся на одной линии
                    }
                }
                //разбрасывание рандомных "клеток" для клеточного автомата
                for (int i = 0; i < 16; i++)
                {
                    int veroyatnost = 59;
                    for (int h = (int)rooms[i].verh.Y - 30; h < rooms[i].niz.Y + 30; h++)
                    {
                        for (int w = (int)rooms[i].verh.X - 30; w < rooms[i].niz.X + 30; w++)
                        {
                            double a = (rooms[i].niz.Y - rooms[i].verh.Y) / 2;
                            double b = (rooms[i].niz.X - rooms[i].verh.X) / 2;
                            double a1 = (rooms[i].niz.Y+30 - (rooms[i].verh.Y-30)) / 2;
                            double b1 = (rooms[i].niz.X+30 - (rooms[i].verh.X-30)) / 2;
                           double w1 =( w - (rooms[i].verh.X + b));
                            double h1 = (h - (rooms[i].verh.Y + a));
                            if (((h > 0) && (w > 0) && (h < scene.Height) && (w < scene.Width)) && ((((w1*w1)/(b*b))+(((h1*h1)/(a*a))))>1)&& ((((w1 * w1) /(b1*b1))+((h1*h1)/(a1*a1)))< 1))
                            {
                                if (rand.Next(100) > veroyatnost)
                                {
                                    boolmap[h, w] = false;
                                }
                            }
                        }
                    }
                    //
                    
                    //
                    for (int q = 0; q < rooms[i].soedinenie; q++)
                    {
                        PointF first = new PointF((rooms[i].verh.X + (rooms[i].niz.X - rooms[i].verh.X) / 2), (rooms[i].verh.Y + (rooms[i].niz.Y - rooms[i].verh.Y) / 2));
                        PointF second = new PointF((rooms[rooms[i].soedinen[q]].verh.X + (rooms[rooms[i].soedinen[q]].niz.X - rooms[rooms[i].soedinen[q]].verh.X) / 2), (rooms[rooms[i].soedinen[q]].verh.Y + (rooms[rooms[i].soedinen[q]].niz.Y - rooms[rooms[i].soedinen[q]].verh.Y) / 2));
                        if ((second.X - first.X) > 0)
                        {
                            if ((second.Y - first.Y) > 0)
                            {
                                Double otnoshenie_storon = (second.X - first.X) / ((second.Y - first.Y));
                                //если х>y
                                if (1 < (otnoshenie_storon))
                                {
                                    int v = (int)first.Y;
                                    for (int w = (int)first.X; w < second.X; w++)
                                    {
                                        while ((v < second.Y) && ((w - first.X) > otnoshenie_storon * (v - first.Y))) { v++; }
                                        for (int e = -30; e < -15; e++)
                                        {
                                            if (rand.Next(100) > veroyatnost)
                                            {
                                                boolmap[v + e, w] = false;
                                            }
                                        }
                                        for (int e = 15; e < 30; e++)
                                        {
                                            if (rand.Next(100) > veroyatnost)
                                            {
                                                boolmap[v + e, w] = false;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    otnoshenie_storon = (second.Y - first.Y) / (second.X - first.X);
                                    int v = (int)first.X;
                                    for (int w = (int)first.Y; w < second.Y; w++)
                                    {
                                        while ((v < second.X) && ((w - first.Y) > otnoshenie_storon * (v - first.X))) { v++; }
                                        for (int e = -30; e < -15; e++)
                                        {
                                            if (rand.Next(100) > veroyatnost)
                                            {
                                                boolmap[w, v + e] = false;
                                            }
                                        }
                                        for (int e = 15; e < 30; e++)
                                        {
                                            if (rand.Next(100) > veroyatnost)
                                            {
                                                boolmap[w, v + e] = false;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {//ecли first.y>second.y
                                Double otnoshenie_storon = (second.X - first.X) / ((first.Y - second.Y));
                                if (1 < (otnoshenie_storon))
                                {
                                    int v = (int)first.Y;
                                    for (int w = (int)first.X; w < second.X; w++)
                                    {
                                        while ((v > second.Y) && ((w - first.X) > otnoshenie_storon * Math.Abs(v - first.Y))) { v--; }
                                        for (int e = -30; e < -15; e++)
                                        {
                                            if (rand.Next(100) > veroyatnost)
                                            {
                                                boolmap[v + e, w] = false;
                                            }
                                        }
                                        for (int e = 15; e < 30; e++)
                                        {
                                            if (rand.Next(100) > veroyatnost)
                                            {
                                                boolmap[v + e, w] = false;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    otnoshenie_storon = (first.Y - second.Y) / (second.X - first.X);
                                    int v = (int)first.X;
                                    for (int w = (int)first.Y; w > second.Y; w--)
                                    {
                                        while ((v < second.X) && (Math.Abs(w - first.Y) > otnoshenie_storon * (v - first.X))) { v++; }
                                        for (int e = -30; e < -15; e++)
                                        {
                                            if (rand.Next(100) > veroyatnost)
                                            {
                                                boolmap[w, v + e] = false;
                                            }
                                        }
                                        for (int e = 15; e < 30; e++)
                                        {
                                            if (rand.Next(100) > veroyatnost)
                                            {
                                                boolmap[w, v + e] = false;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        //добавть исход, если комнаты находятся на одной линии
                    }
                }
                //
                //
               
                //развертвование клеточного автомата
                //
                //                

                //доббавление врагов в комнаты
                /*for (int i = 0; i < 16; i++)
                {
                    for (int e = 0; e < rand.Next(2, 7); e++)
                    {
                        enemies.Add(new Enemy(rand.Next((int)rooms[i].verh.Y + 15, (int)rooms[i].niz.Y - 15), rand.Next((int)rooms[i].verh.X + 15, (int)rooms[i].niz.X - 15)));
                    }
                }*/
                for (int i = 0; i < 10; i++)
                {
                    bool[,] boolmap1 = new bool[scene.Height, scene.Width];
                    for (int h = 0; h < scene.Height; h++)
                    {
                        for (int w = 0; w < scene.Width; w++)
                        {
                            boolmap1[h, w] = boolmap[h, w];
                        }
                    }
                    for (int h = 3; h < scene.Height - 3; h++)
                    {
                        for (int w = 3; w < scene.Width - 3; w++)
                        {
                            int sosedi = 0;
                            for (int a = h - 2; a < h + 3; a++)
                            {
                                for (int s = w - 2; s < w + 3; s++)
                                {
                                    if ((a != h) || (s != w))
                                    {
                                        if (boolmap[a, s])
                                        {
                                            sosedi++;
                                        }
                                    }
                                }
                            }
                            if (boolmap[h, w])
                            {
                                if (sosedi >= 14)
                                {
                                    boolmap1[h, w] = true;
                                }
                                else
                                {
                                    boolmap1[h, w] = false;
                                }
                            }
                            else
                            {
                                if (sosedi >= 15)
                                {
                                    boolmap1[h, w] = true;
                                }
                                else
                                {
                                    boolmap1[h, w] = false;
                                }
                            }
                        }
                    }
                    for (int h = 0; h < scene.Height; h++)
                    {
                        for (int w = 0; w < scene.Width; w++)
                        {
                            boolmap[h, w] = boolmap1[h, w];
                        }
                    }
                }
                int start_room = rand.Next(16);
                player = new Player(rooms[start_room].verh.X + (rooms[start_room].niz.X - rooms[start_room].verh.X) / (rand.Next(20, 31) / 10), rooms[start_room].verh.Y + (rooms[start_room].niz.Y - rooms[start_room].verh.Y) / (rand.Next(20, 31) / 10));
                 enemy = new Enemy(rooms[start_room].verh.X + (rooms[start_room].niz.X - rooms[start_room].verh.X) / (rand.Next(20, 31) / 10), rooms[start_room].verh.Y + (rooms[start_room].niz.Y - rooms[start_room].verh.Y) / (rand.Next(20, 31) / 10));
                //тестовая отрисовка в map
                for (int i = 10; i < scene.Height; i = i + 1)
                {
                    for (int j = 10; j < scene.Width; j++)
                    {
                        Point n = new Point(j, i);
                        while ((j < scene.Width) && (boolmap[i, j]))
                        {
                            j++;
                        }
                        Point m = new Point(j, i);
                        g1.DrawLine(blackpen, n, m);
                        j++;
                    }
                }
            }
        }
        private void onKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W) this.keyW = true;
            if (e.KeyCode == Keys.A) this.keyA = true;
            if (e.KeyCode == Keys.S) this.keyS = true;
            if (e.KeyCode == Keys.D) this.keyD = true;
            if (e.KeyCode == Keys.Space) this.keySpace = true;
        }
        private void onKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W) { this.keyW = false; }
            if (e.KeyCode == Keys.A) this.keyA = false;
            if (e.KeyCode == Keys.S) this.keyS = false;
            if (e.KeyCode == Keys.D) this.keyD = false;
            if (e.KeyCode == Keys.Space) this.keySpace = false;
        }
        public class room
        {
            public int[] conect = new int[15];
            public int conection;
            public PointF verh, niz;
            public bool peseheno = false;
            public int[] soedinen = new int[15];
            public int soedinenie = 0;
            public void conected(int conect_)
            {
                this.conect[conection] = conect_;
            }
            public room(float verhy, float verhx, float nizy, float nizx)
            {
                this.verh.X = verhx;
                this.verh.Y = verhy;
                this.niz.X = nizx;
                this.niz.Y = nizy;
            }
            public room dilenie()
            {
                int delitel;
                room ret = new room(verh.Y, verh.X, niz.Y, niz.X);
                int napravlenie = rand.Next(0, 2);
                if ((((Math.Abs(verh.X - ((int)verh.X + (int)((niz.X - verh.X) / 100 * 30))) / Math.Abs(verh.Y - niz.Y)) > 3) || ((Math.Abs(verh.X - ((int)verh.X + (int)((niz.X - verh.X) / 100 * 30))) / Math.Abs(verh.Y - niz.Y)) < 0.333)))
                {
                    do
                    {
                        delitel = rand.Next((int)verh.Y + (int)((niz.Y - verh.Y) / 100 * 30), (int)verh.Y + (int)((niz.Y - verh.Y) / 100 * 70));
                        ret = new room(delitel, verh.X, niz.Y, niz.X);
                    }
                    while ((((Math.Abs(ret.verh.X - ret.niz.X) / Math.Abs(ret.verh.Y - ret.niz.Y)) > 3) || ((Math.Abs(ret.verh.X - ret.niz.X) / Math.Abs(ret.verh.Y - ret.niz.Y)) < 0.333)) || (((Math.Abs(verh.X - niz.X) / Math.Abs(verh.Y - delitel)) > 3) || ((Math.Abs(verh.X - niz.X) / Math.Abs(verh.Y - delitel)) < 0.333)));
                    niz.Y = delitel;
                    return (ret);
                }
                else if ((((Math.Abs(verh.X - niz.X) / Math.Abs(verh.Y - ((int)verh.Y + (int)((niz.Y - verh.Y) / 100 * 50)))) > 3) || ((Math.Abs(verh.X - niz.X) / Math.Abs(verh.Y - ((int)verh.Y + (int)((niz.Y - verh.Y) / 100 * 50)))) < 0.333)))
                {
                    do
                    {
                        delitel = rand.Next((int)verh.X + (int)((niz.X - verh.X) / 100 * 30), (int)verh.X + (int)((niz.X - verh.X) / 100 * 70));
                        ret = new room(verh.Y, delitel, niz.Y, niz.X);
                    }
                    while ((((Math.Abs(ret.verh.X - ret.niz.X) / Math.Abs(ret.verh.Y - ret.niz.Y)) > 3) || ((Math.Abs(ret.verh.X - ret.niz.X) / Math.Abs(ret.verh.Y - ret.niz.Y)) < 0.333)) || (((Math.Abs(verh.X - delitel) / Math.Abs(verh.Y - niz.Y)) > 3) || ((Math.Abs(verh.X - delitel) / Math.Abs(verh.Y - niz.Y)) < 0.333)));
                    niz.X = delitel;
                    return (ret);
                }
                else
                {
                    switch (napravlenie)
                    {
                        case 0:
                            do
                            {
                                delitel = rand.Next((int)verh.X + (int)((niz.X - verh.X) / 100 * 30), (int)verh.X + (int)((niz.X - verh.X) / 100 * 70));
                                ret = new room(verh.Y, delitel, niz.Y, niz.X);
                            }
                            while ((((Math.Abs(ret.verh.X - ret.niz.X) / Math.Abs(ret.verh.Y - ret.niz.Y)) > 3) || ((Math.Abs(ret.verh.X - ret.niz.X) / Math.Abs(ret.verh.Y - ret.niz.Y)) < 0.333)) || (((Math.Abs(verh.X - delitel) / Math.Abs(verh.Y - niz.Y)) > 3) || ((Math.Abs(verh.X - delitel) / Math.Abs(verh.Y - niz.Y)) < 0.333)));
                            niz.X = delitel;
                            return (ret);
                        case 1:
                            do
                            {
                                delitel = rand.Next((int)verh.Y + (int)((niz.Y - verh.Y) / 100 * 30), (int)verh.Y + (int)((niz.Y - verh.Y) / 100 * 70));
                                ret = new room(delitel, verh.X, niz.Y, niz.X);
                            }
                            while ((((Math.Abs(ret.verh.X - ret.niz.X) / Math.Abs(ret.verh.Y - ret.niz.Y)) > 3) || ((Math.Abs(ret.verh.X - ret.niz.X) / Math.Abs(ret.verh.Y - ret.niz.Y)) < 0.333)) || (((Math.Abs(verh.X - niz.X) / Math.Abs(verh.Y - delitel)) > 3) || ((Math.Abs(verh.X - niz.X) / Math.Abs(verh.Y - delitel)) < 0.333)));
                            niz.Y = delitel;
                            return (ret);
                    }
                }
                return (ret);
            }
        }
        private void GameTimer_Tick(object sender, EventArgs owe)
        {
            PointF old = new PointF(player.pos.X, player.pos.Y);
            if (keyW)
            {
                player.acc.Y -= player.speed;
                player.tekush = Player.model[0];
                player.Size.X = 20;
                player.Size.Y = 25;
            }
            if (keyA)
            {
                player.acc.X -= player.speed;
                player.tekush = Player.model[2];
                player.Size.X = 25;
                player.Size.Y = 20;
            }
            if (keyS)
            {
                player.acc.Y += player.speed;
                player.tekush = Player.model[1];
                player.Size.X = 20;
                player.Size.Y = 25;
            }
            if (keyD)
            {
                player.acc.X += player.speed;
                player.tekush = Player.model[3];
                player.Size.X = 25;
                player.Size.Y = 20;
            }
            // Логика игрового перемещения
            player.pos.X += player.acc.X;
            player.pos.Y += player.acc.Y;
            player.acc.X *= player.fade;
            player.acc.Y *= player.fade;
            // Инверсия ускорения игрока дла предотвращения
            // выхода за границы игровой карты
            if (gameTime % 2 == 0)
            {
                g.Clear(Color.White);
                g.DrawImage(map, 0, 0);
            }
            if (boolmap[(int)player.pos.Y, (int)player.pos.X])
            {
                player.pos.X = old.X;
                player.pos.Y = old.Y;
            }
            if (boolmap[(int)player.pos.Y, (int)player.pos.X])
            {
                player.pos.X = old.X;
                player.pos.Y = old.Y;
            }
            if (boolmap[(int)player.pos.Y, (int)player.pos.X])
            {
                player.pos.X = old.X;
                player.pos.Y = old.Y;
            }
            if (boolmap[(int)player.pos.Y, (int)player.pos.X])
            {
                player.pos.X = old.X;
                player.pos.Y = old.Y;
            }
            //логика врагов
            
                Color color = new Color();
            color = Color.Red;
            if (enemy.vidimost(player, boolmap,color))
            {
                    g.FillEllipse(
                    new SolidBrush(color),
                     enemy.pos.X - 50 / 2f,
                    enemy.pos.Y - 50 / 2f,
                    50,
                    50
                );
                    enemy.shooting(player);
                }
               
            
            for (var i = 0; i < enemy.shoots.Count; i++)
            {
                var shot = enemy.shoots[i];

                // Отрисовка одного снадяра
                g.FillEllipse(
                    new SolidBrush(Color.FromArgb(255 - (int)(200), Color.Yellow)),
                    shot.pos.Y - shot.size / 2f,
                    shot.pos.X - shot.size / 2f,
                    shot.size,
                    shot.size
                );

                // Изменение позиции снаряда
                shot.pos.X += shot.acc.X;
                shot.pos.Y += shot.acc.Y;
            }
                //
                for (int i = 0; i < portal_kolvo; i++)
            {
                if (gameTime % 240 == 0) { portal[i].portal_enter(player); }
                g.DrawImage(player.tekush, player.pos.X - (player.Size.X / 2), player.pos.Y - (player.Size.Y / 2));
                if (gameTime % 8 == 0)
                {
                    portal[i].portal_enter(player);
                    g.DrawImage(portal[i].model[0], portal[i].pos.X - (portal[i].Size.X / 2), portal[i].pos.Y - (portal[i].Size.Y / 2));
                    g.DrawImage(portal[i].model[2], portal[i].poss.X - (portal[i].Size.X / 2), portal[i].poss.Y - (portal[i].Size.Y / 2));
                }
                else if (gameTime % 4 == 0)
                {
                    g.DrawImage(portal[i].model[1], portal[i].pos.X - (portal[i].Size.X / 2), portal[i].pos.Y - (portal[i].Size.Y / 2));
                    g.DrawImage(portal[i].model[1], portal[i].poss.X - (portal[i].Size.X / 2), portal[i].poss.Y - (portal[i].Size.Y / 2));
                }
                else if (gameTime % 2 == 0)
                {
                    g.DrawImage(portal[i].model[2], portal[i].pos.X - (portal[i].Size.X / 2), portal[i].pos.Y - (portal[i].Size.Y / 2));
                    g.DrawImage(portal[i].model[0], portal[i].poss.X - (portal[i].Size.X / 2), portal[i].poss.Y - (portal[i].Size.Y / 2));
                }
            }
            if (gameTime % 8 == 0)
            {
                canvas.Image = scene;
            }
            else if (gameTime % 4 == 0)
            {
                canvas.Image = scene;
            }
            else if (gameTime % 2 == 0)
            {
                canvas.Image = scene;
            }
            
            gameTime++;
        }
    }
    class Player
    {
        public PointF Size = new PointF(25, 20);
        public PointF pos;
        public PointF acc;
        public float speed = 0.3f;
        public float fade = 0.95f;
        public int health = 100;
        public Image tekush = Image.FromFile("../../resources/player_w.png");
        public static Image[] model = new Image[] { Image.FromFile("../../resources/player_w.png"), Image.FromFile("../../resources/player_s.png"), Image.FromFile("../../resources/player_a.png"), Image.FromFile("../../resources/player_d.png") };
        public Player(PointF pos)
        {
            this.pos = pos;
            this.acc = new PointF(0, 0);
        }
        public Player(float x, float y)
        {
            this.pos.X = x - 13;
            this.pos.Y = y - 13;
            this.acc = new PointF(0, 0);
        }
    }
    class Enemy
    {
        public PointF pos;
        public PointF acc;
        public int speed;
        public int attacspeed;
        public int attac = 10;
        public int health = 50;
        public List<shoot> shoots = new List<shoot>();
        public Enemy(PointF pos_)
        {
            this.pos = pos_;
        }
        public Enemy(float x, float y)
        {
            this.pos.X = x;
            this.pos.Y = y;
        }
        public Enemy(int x, int y)
        {
            this.pos.X = x;
            this.pos.Y = y;
        }
        public bool vidimost(Player player, bool[,] boolmap, Color color)
        {
           

            double rastx;//растояние между игроком и врагом по x
            double rasty;// растояние между игроком и врагом по y
            if (player.pos.X - pos.X > 0)
            {
                rastx = player.pos.X - pos.X;
                if (player.pos.Y - pos.Y > 0)
                {
                    rasty = player.pos.Y - pos.Y;
                    for (int i = (int)player.pos.X; i > pos.X; i--)
                    {
                        if (boolmap[(int)(player.pos.Y - (rasty / rastx * Math.Abs(i - pos.X))), i])
                        {
                            color = Color.Yellow;
                            return false;
                        }
                    }
                }
                else if (pos.Y - player.pos.Y > 0)
                {
                    rasty = pos.Y - player.pos.Y;
                    for (int i = (int)player.pos.X; i > pos.X; i--)
                    {
                        if (boolmap[(int)(player.pos.Y + (rasty/rastx* Math.Abs(i- pos.X))), i])
                        {
                            color = Color.Green;
                            return false;
                        }
                    }

                }

            }
            else if ((pos.X - player.pos.X > 0))
            {
                rastx = pos.X - player.pos.X;
                if (player.pos.Y - pos.Y > 0)
                {
                    rasty = player.pos.Y - pos.Y;
                    for (int i = (int)player.pos.X; i < (int)pos.X; i++)
                    {
                        if (boolmap[(int)(player.pos.Y - (rasty / rastx * Math.Abs(i- player.pos.X))), i])
                        {
                            color = Color.Red;
                            return false;
                        }
                    }
                }
                else if ((int)(pos.Y - player.pos.Y) > 0)
                {
                    rasty = (int)(pos.Y - player.pos.Y);
                    for (int i = (int)player.pos.X; i < (int)pos.X; i++)
                    {
                        if (boolmap[(int)(player.pos.Y + (rasty / rastx * Math.Abs(i-player.pos.X))), i])
                        {
                            color = Color.Blue;
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        public void shooting(Player player)
        {
           
            
                //попытка определения куда стреляет бот
                bool popadanie = false;
                int tick = 0;
                PointF accc = new PointF(0, 0);

                while (popadanie)
                {
                    accc.X = ((player.pos.X + (player.acc.X * tick)) - pos.X) / ((player.pos.Y + (player.acc.Y * tick)) - pos.Y) * 2;
                    accc.Y = ((player.pos.Y + (player.acc.Y * tick)) - pos.Y) / ((player.pos.X + (player.acc.X * tick)) - pos.X) * 2;
                    if ((player.pos.X + (player.acc.X * tick)) == (pos.X + accc.X * tick) && ((player.pos.Y + (player.acc.Y * tick)) == (pos.Y + accc.Y * tick))) { popadanie = true; }
                }
                shoots.Add(new shoot(pos, accc));
            
        }
    }
    public class shoot
    {
        public PointF pos;
        public PointF acc;
            public float size=5f;
            public shoot(PointF pos, PointF acc)
        {
            this.acc = acc;
            this.pos = pos;
        }
    }
    class Portal
    {
        public Point Size = new Point(60, 60);
        public PointF pos;
        public PointF poss;
        public void portal_enter(Player player)
        {//если зашел в pos
            if (((player.pos.X - player.Size.X / 2) > (pos.X - Size.X / 2)) && ((player.pos.X + player.Size.X / 2) < (pos.X + Size.X / 2)) && ((player.pos.Y - player.Size.Y / 2) > (pos.Y - Size.Y / 2)) && ((player.pos.Y + player.Size.Y / 2) < (pos.Y + Size.Y / 2)))
            {
                player.pos.X = poss.X + Size.X / 2;
                player.pos.Y = poss.Y;
            }
            //если зашел в poss
            if (((player.pos.X - player.Size.X / 2) > (poss.X - Size.X / 2)) && ((player.pos.X + player.Size.X / 2) < (poss.X + Size.X / 2)) && ((player.pos.Y - player.Size.Y / 2) > (poss.Y - Size.Y / 2)) && ((player.pos.Y + player.Size.Y / 2) < (poss.Y + Size.Y / 2)))
            {
                player.pos.X = pos.X + Size.X / 2;
                player.pos.Y = pos.Y;
            }
        }
        public Portal(float posy, float posx, float possy, float possx)
        {
            this.pos.X = posx;
            this.poss.X = possx;
            this.pos.Y = posy;
            this.poss.Y = possy;
        }
        public Image[] model = new Image[] { Image.FromFile("../../resources/model1.png"), Image.FromFile("../../resources/model2.png"), Image.FromFile("../../resources/model3.png") };
    }
}
// сделать ещё одну boolmap для отслеживания иследованного поля игроком