
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

            //тестовая отрисовка
            for (int i = 0; i < 16; i = i + 1)
            {
                Point n = new Point((int)rooms[i].verh.X, (int)rooms[i].verh.Y);
                Point m = new Point((int)rooms[i].niz.X, (int)rooms[i].verh.Y);
                Point l = new Point((int)rooms[i].niz.X, (int)rooms[i].niz.Y);
                Point k = new Point((int)rooms[i].verh.X, (int)rooms[i].niz.Y);
                Point r = new Point((int)rooms[i].verh.X+((int)rooms[i].niz.X-(int)rooms[i].verh.X)/2, (int)rooms[i].verh.Y+((int)rooms[i].niz.Y- (int)rooms[i].verh.Y) / 2);
                g.DrawLine(blackpen, m, n);
                g.DrawLine(blackpen, n, k);
                g.DrawLine(blackpen, k, l);
                g.DrawLine(blackpen, l, m);
                g.DrawString(rooms[i].conection.ToString(),stand,blackbrush,r);
            }

            portal = new Portal(rand.Next(scene.Height), rand.Next(scene.Width), rand.Next(scene.Height), rand.Next(scene.Width));
        }
        
        public class room
        {
            public int[] conect=new int[15];
            public int conection;
            public PointF verh, niz;
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
            

                canvas.Image = scene;
            //gameTime++;
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
