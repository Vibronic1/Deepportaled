
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

        public static Random rand = new Random();
        int gameTime = 0;
        Bitmap scene;
        Graphics g;
        Bitmap map;
        Graphics g1;
        Pen blackpen;
        Player player;
        Brush blackbrush = new SolidBrush(Color.Black);
        Portal portal;
        Font stand = new Font(FontFamily.GenericSansSerif, 20);
        public Form1()
        {
            InitializeComponent();
            GameTimer.Interval = 1000 / 60;
            GameTimer.Start();
            scene = new Bitmap(canvas.Width, canvas.Height);
            map = new Bitmap(canvas.Width, canvas.Height);
            g = Graphics.FromImage(scene);
            g1 = Graphics.FromImage(map);
            blackpen = new Pen(Color.Black);
            player = new Player(new PointF(scene.Width / 2, scene.Height / 2));



            //генерация карты
            //разбиение карты на комнаты
             room[] rooms = new room[16];
            rooms[0] = new room(10,10,scene.Height-10,scene.Width-10);
            
            
            for (int j = 4; j > 0; j--)
            {
                for (int i = 0; i < 16; i = i + (int)Math.Pow(2, j))
                {
                     rooms[i + (int)Math.Pow(2, j - 1)] = rooms[i].dilenie();
                        
                    
                    
                    

                }
               
            }
            //нахождения для каждой клетки её соседа
            for (int i = 0; i < 16; i++)//i- комната у которой проверяем
            {
                for (int j = 0; j < 16; j++)
                {
                    //левая сторона 
                    if ((rooms[i].verh.X == rooms[j].niz.X) &&(((rooms[i].verh.Y<= rooms[j].verh.Y) &&(rooms[i].niz.Y >= rooms[j].verh.Y))|| ((rooms[i].verh.Y <= rooms[j].niz.Y) && (rooms[i].niz.Y >= rooms[j].niz.Y))))
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
            //room vrem;
            //уменьшение комнат дабы создать пространство между ними и там в дальнейшем развернуть клеточный автомат...
            for (int i = 0; i < 16; i++)
            {
                room vrem;
                vrem = rooms[i];
                rooms[i].verh.X = vrem.verh.X + (vrem.niz.X - vrem.verh.X)/100*rand.Next(15,30);
                rooms[i].verh.Y = vrem.verh.Y + (vrem.niz.Y - vrem.verh.Y) / 100 * rand.Next(15, 30);
                rooms[i].niz.X= vrem.niz.X- (vrem.niz.X - vrem.verh.X) / 100 * rand.Next(15, 30);
                rooms[i].niz.Y= vrem.niz.Y- (vrem.niz.Y - vrem.verh.Y) / 100 * rand.Next(15, 30);
            }

            //присвоение формы каждой комнате исходя из кол-ва её соседей(отказ от этой идеи, закоменчено)
            //for (int i = 0; i < 16; i++)
            //{
            //    switch (rooms[i].conection)
            //    {
            //        case 1:
            //        case 2:
            //            rooms[i].vnyt[1].Y = rooms[i].niz.Y;
            //            rooms[i].vnyt[1].X = rooms[i].niz.X - (rooms[i].niz.X - rooms[i].verh.X) / 100 * rand.Next(15, 30);

            //            break;
            //        case 3:
            //        case 4:
            //            rooms[i].vnyt[1].Y = rooms[i].niz.Y;
            //            rooms[i].vnyt[1].X = rooms[i].niz.X - (rooms[i].niz.X - rooms[i].verh.X) / 100 * rand.Next(15, 30);
            //            rooms[i].vnyt[0].Y = rooms[i].verh.Y + (rooms[i].niz.Y - rooms[i].verh.Y) / 100 * rand.Next(30, 60);
            //            rooms[i].vnyt[0].X = rooms[i].verh.X + (rooms[i].niz.X - rooms[i].verh.X) / 100 * rand.Next(30, 60);

            //            break;
            //        case 5:
            //        case 6:
            //            rooms[i].vnyt[1].Y = rooms[i].niz.Y;
            //            rooms[i].vnyt[1].X = rooms[i].niz.X - (rooms[i].niz.X - rooms[i].verh.X) / 100 * rand.Next(15, 30);
            //            rooms[i].vnyt[0].Y = rooms[i].verh.Y + (rooms[i].niz.Y - rooms[i].verh.Y) / 100 * rand.Next(15, 30);
            //            rooms[i].vnyt[0].X = rooms[i].verh.X + (rooms[i].niz.X - rooms[i].verh.X) / 100 * rand.Next(15, 30);
            //            rooms[i].vnyt[2].Y = rooms[i].niz.Y - (rooms[i].niz.Y - rooms[i].verh.Y) / 100 * rand.Next(15, 30);
            //            rooms[i].vnyt[2].X = rooms[i].verh.X + (rooms[i].niz.X - rooms[i].verh.X) / 100 * rand.Next(15, 30);
            //            break;
            //        case 7:
            //        case 8:
            //            rooms[i].vnyt[1].Y = rooms[i].verh.Y + (rooms[i].niz.Y - rooms[i].verh.Y) / 100 * rand.Next(15, 30);
            //            rooms[i].vnyt[1].X = rooms[i].niz.X - (rooms[i].niz.X - rooms[i].verh.X) / 100 * rand.Next(15, 30);
            //            rooms[i].vnyt[0].Y = rooms[i].verh.Y + (rooms[i].niz.Y - rooms[i].verh.Y) / 100 * rand.Next(15, 30);
            //            rooms[i].vnyt[0].X = rooms[i].verh.X + (rooms[i].niz.X - rooms[i].verh.X) / 100 * rand.Next(15, 30);
            //            rooms[i].vnyt[2].Y = rooms[i].niz.Y - (rooms[i].niz.Y - rooms[i].verh.Y) / 100 * rand.Next(15, 30);
            //            rooms[i].vnyt[2].X = rooms[i].verh.X + (rooms[i].niz.X - rooms[i].verh.X) / 100 * rand.Next(15, 30);
            //            rooms[i].vnyt[3].Y = rooms[i].niz.Y - (rooms[i].niz.Y - rooms[i].verh.Y) / 100 * rand.Next(15, 30);
            //            rooms[i].vnyt[3].X = rooms[i].niz.X - (rooms[i].niz.X - rooms[i].verh.X) / 100 * rand.Next(15, 30);
            //            break;

            //    }
            //}
            
            //создаение проходов между комнатами
            int[] vrem1 = new int[16];
            int size = 0;
            int randomka;
            
            vrem1[size] = rand.Next(16);
            rooms[vrem1[size]].peseheno = true;
            size++;
            while (size != 0)
            {
                size--;
                int tekush = vrem1[size];
                




                randomka = rand.Next(rooms[tekush].conection);
                if (rooms[rooms[tekush].conect[randomka]].peseheno) {
                    for (int o = 0; o < rooms[tekush].conection; o++) {
                        if (rooms[rooms[tekush].conect[o]].peseheno) { } else { 

                            vrem1[size] = tekush;
                        size++;
                            break;
                    } } }
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
                    
                }
                    
                
            }
            //Переход от абстрактных комнат к карте boolmap, состоящей из пикселей
            bool[,] boolmap = new bool[scene.Height, scene.Width];
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
                    if ((second.X -first.X ) > 0)
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
                                otnoshenie_storon = (second.Y - first.Y)/(second.X - first.X);
                                int v = (int)first.X;
                                for (int w = (int)first.Y; w < second.Y; w++)
                                {
                                    while ((v < second.X) && ((w - first.Y) > otnoshenie_storon * (v - first.X )) ){ v++; }
                                    for (int e = -15; e < 16; e++)
                                    {
                                        boolmap[w, v + e] = false;
                                    }
                                }
                            }
                        }
                        else
                        {//ecли first.y>second.y
                            Double otnoshenie_storon = (second.X - first.X) / ((first.Y- second.Y));
                            
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
                                otnoshenie_storon = (first.Y-second.Y ) / (second.X - first.X);
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
                for (int h = (int)rooms[i].verh.Y-10; h < rooms[i].niz.Y+10; h++)
                {
                    
                    for (int w = (int)rooms[i].verh.X-10; w < rooms[i].niz.X+10; w++)
                    {
                        if (((h< (int)rooms[i].verh.Y) ||(h> rooms[i].niz.Y)) ||((w< (int)rooms[i].verh.X) ||(w> rooms[i].niz.X)))
                        {
                            if (rand.Next(100) > 35)
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
                                        if (rand.Next(100) > 35)
                                        {
                                            boolmap[v + e, w] = false;
                                        }
                                    }
                                    for (int e = 15; e < 30; e++)
                                    {
                                        if (rand.Next(100) > 35)
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
                                        if (rand.Next(100) > 35)
                                        {
                                            boolmap[w, v + e] = false;
                                        }
                                    }
                                    for (int e = 15; e < 30; e++)
                                    {
                                        if (rand.Next(100) > 35)
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
                                        if (rand.Next(100) > 35)
                                        {
                                            boolmap[v + e, w] = false;
                                        }
                                    }
                                    for (int e = 15; e < 30; e++)
                                    {
                                        if (rand.Next(100) > 35)
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
                                        if (rand.Next(100) > 35)
                                        {
                                            boolmap[w, v + e] = false;
                                        }
                                    }
                                    for (int e = 15; e < 30; e++)
                                    {
                                        if (rand.Next(100) > 35)
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
            //развертвование клеточного автомата

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
            //тесты
            for (int i = 0; i < 16; i = i + 1)
            {
                Point n = new Point((int)rooms[i].verh.X, (int)rooms[i].verh.Y);
                Point m = new Point((int)rooms[i].niz.X, (int)rooms[i].verh.Y);
                Point l = new Point((int)rooms[i].niz.X, (int)rooms[i].niz.Y);
                Point k = new Point((int)rooms[i].verh.X, (int)rooms[i].niz.Y);
                Point r = new Point((int)rooms[i].verh.X + ((int)rooms[i].niz.X - (int)rooms[i].verh.X) / 2, (int)rooms[i].verh.Y + ((int)rooms[i].niz.Y - (int)rooms[i].verh.Y) / 2);
                
                
                g.DrawString(rooms[i].conection.ToString(), stand, blackbrush, r);
                
                for (int y=0;y< rooms[i].soedinenie;y++)
                {
                    Point r1 = new Point((int)rooms[rooms[i].soedinen[y]].verh.X + ((int)rooms[rooms[i].soedinen[y]].niz.X - (int)rooms[rooms[i].soedinen[y]].verh.X) / 2, (int)rooms[rooms[i].soedinen[y]].verh.Y + ((int)rooms[rooms[i].soedinen[y]].niz.Y - (int)rooms[rooms[i].soedinen[y]].verh.Y) / 2);
                    g.DrawLine(blackpen, r, r1);
                }
                g.DrawLine(blackpen, n, m);
                g.DrawLine(blackpen, l, m);
                g.DrawLine(blackpen, l, k);
                g.DrawLine(blackpen, n, k);

            }
            

            portal = new Portal(rand.Next(scene.Height), rand.Next(scene.Width), rand.Next(scene.Height), rand.Next(scene.Width));
        }
        
        public class room
        {
            public int[] conect=new int[15];
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
                room ret=new room(verh.Y,verh.X,niz.Y,niz.X);
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
                { switch (napravlenie)
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
       
        private void GameTimer_Tick(object sender, EventArgs e)
        {


            //if (gameTime % 8 == 0)
            //{
            //    g.Clear(Color.White);

            //    g.DrawImage(portal.model[0], portal.pos);
            //    g.DrawImage(portal.model[2], portal.poss);
            //}
            //else if (gameTime % 4 == 0)
            //{
            //    g.Clear(Color.White);

            //    g.DrawImage(portal.model[1], portal.pos);
            //    g.DrawImage(portal.model[1], portal.poss);
            //}
            //else if (gameTime % 2 == 0)
            //{
            //    g.Clear(Color.White);

            //    g.DrawImage(portal.model[2], portal.pos);
            //    g.DrawImage(portal.model[0], portal.poss);
            //}


            if (gameTime % 200 == 0)
                {  canvas.Image = scene;}else if (gameTime % 100 == 0){
                canvas.Image = map;}
                gameTime++;
        }
    }
    class Player
    {
        int Size = 50;
        public PointF pos;
        public PointF acc;
        public Player(PointF pos)
        {
            this.pos = pos;
            this.acc = new PointF(0, 0);
        }


    }
    
    class Portal
    {
        int Size = 50;
        public PointF pos;
        public PointF poss;
        public void portal_enter(Player player)
        {
            if ((Math.Abs(player.pos.X- poss.X+25)<25)&& (Math.Abs(player.pos.Y - poss.Y+25) < 25)) { player.pos = pos; }else if ((Math.Abs(player.pos.X - pos.X) < 25) && (Math.Abs(player.pos.Y - pos.Y) < 25)) { player.pos = poss; }
        }
        public Portal(float posy, float posx, float possy, float possx)
        {
            this.pos.X = posx;
            this.poss.X = possx;
            this.pos.Y = posy;
            this.poss.Y = possy;
        }
        public Image[] model = new Image[] { Image.FromFile("../../resources/model1.png"), Image.FromFile("../../resources/model2.png"), Image.FromFile("../../resources/model3.png") } ;
    }
}
