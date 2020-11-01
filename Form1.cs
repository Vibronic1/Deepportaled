
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Deepportaled
{

    public partial class Form1 : Form
    {

        
        // Ресурсы игры
        int gameTime = 0;
        Bitmap scene;
        Graphics g;
        Pen blackpen;
        Player player;
        int[,] map;
        Portal portal;
        public Form1()
        {
            InitializeComponent();
            GameTimer.Interval = 1000 / 30;
            GameTimer.Start();
            scene = new Bitmap(canvas.Width, canvas.Height);
            g = Graphics.FromImage(scene);
            blackpen = new Pen(Color.Black);
            player = new Player(new PointF(scene.Width / 2, scene.Height / 2));



            //генерация карты
            map = new int[canvas.Width, canvas.Height];
            Random rand = new Random();
            portal = new Portal(rand.Next(0, scene.Width), rand.Next(0, scene.Height), rand.Next(0, scene.Width), rand.Next(0, scene.Height));
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            
            int i=0;
            int j = 0;
            if (gameTime % 8 == 0)
            {
                g.Clear(Color.White);
                g.DrawImage(portal.model[0], portal.pos);
                g.DrawImage(portal.model[2], portal.poss);
            }
            else if(gameTime % 4 == 0)
            {
                g.Clear(Color.White);
                g.DrawImage(portal.model[1], portal.pos);
                g.DrawImage(portal.model[1], portal.poss);
            }
            else if(gameTime % 2 == 0)
            {
                g.Clear(Color.White);
                g.DrawImage(portal.model[2], portal.pos);
                g.DrawImage(portal.model[0], portal.poss);
            }
          
            canvas.Image = scene;
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
            if ((Math.Abs(player.pos.X- poss.X)<25)&& (Math.Abs(player.pos.Y - poss.Y) < 25)) { player.pos = pos; }else if ((Math.Abs(player.pos.X - pos.X) < 25) && (Math.Abs(player.pos.Y - pos.Y) < 25)) { player.pos = poss; }
        }
        public Portal(float posx,float possx, float posy, float possy)
        {
            this.pos.X = posx;
            this.poss.X = possx;
            this.pos.Y = posy;
            this.poss.Y = possy;
        }
        public Image[] model = new Image[] { Image.FromFile("../../resources/model1.png"), Image.FromFile("../../resources/model2.png"), Image.FromFile("../../resources/model3.png") } ;
    }
}
