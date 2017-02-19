using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GravityBallCSharp
{
    public partial class Form1 : Form
    {
        float x, y,t,v,bouncingCount; //  абсциса/ордината/време/моментна скорост/брояч отскоци
        static  float gravity= 9.8F;
        static int velocity = 4;
        static int bounceLimit = 10;
        bool boing = false;
        SolidBrush white = new SolidBrush(Color.LightGray);
        SolidBrush red = new SolidBrush(Color.Red);
        public Form1()
        {
            InitializeComponent();
           // this.DoubleBuffered = true;
            this.BackColor = Color.LightGray;
            button3.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button3.Hide();
            Invalidate();
            timer1.Start();
            Random rnd = new Random();
            //позиция на топката
            x = rnd.Next(0, ClientSize.Width - 50);
            y = rnd.Next(0, ClientSize.Height - 50);
            drawBall(x,y,red);
            //зануляване на моментното време и брояча
            t = 0;
            bouncingCount = 0;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void drawBall(float x, float y , SolidBrush color)
        {
            Graphics g = CreateGraphics();
            g.FillEllipse(color, x, y, 50, 50);
        }
        
        private void MoveBall()
        {
            
            v = gravity * (t/10); //  при таймер от 0.1 сек
            if ( y + 49 > this.ClientSize.Height )
            {
                t -= velocity; // При отскачане намаляваме скоростта чрез  t(времето за достигане на точка)
                boing = true;
                bouncingCount++;
            }
            if (boing) v = -v; // обръщаме посоката
            if (t <= 0) { v = -v; boing = false; } // обръщаме посоката когато скоростта удари 0
            
            // прикриването на анимация е по ефективно от Invalidate()  на цялата форма
            drawBall(x, y,white);
            y = y + v;
            if (bouncingCount == bounceLimit)// спиране на анимацията
            {
                y = this.ClientSize.Height - 49;
                timer1.Stop();
            }
            drawBall(x, y,red);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            button3.Show();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            MoveBall();
            if (boing) t--;
            if (!boing) t++;
        }
        
       
    }
}
