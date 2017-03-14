using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.Drawing.Drawing2D;

namespace MazeWinform
{
    public partial class Form1 : Form

    {
        #region Constants And Fields
        static int s0 = 95,s1=09,trapP = 01;
        static int breakLearningAfter = 100;
        const double Alpha = 0.2, Sigma = 0.99, instantReward = -0.02;
        Agent agent = new Agent(s1/10,s1%10);
        static int[,] mazePoints = new int[10, 10] {{0, 0, 1, 1, 1, 0, 1, 0, 0, 1},

                                                    {0, 0, 1, 0, 0, 0, 0, 0, 1, 1},

                                                    {0, 0, 0, 0, 0, 1, 0, 0, 0, 0},

                                                    {0, 1, 0, 0, 0, 1, 0, 1, 1, 0},

                                                    {0, 0, 0, 1, 0, 1, 0, 0, 0, 0},

                                                    {1, 0, 0, 0, 0, 0, 0, 1, 0, 1},

                                                    {1, 1, 0, 1, 0, 0, 0, 0, 0, 0},

                                                    {0, 0, 0, 0, 0, 1, 0, 1, 0, 0},

                                                    {1, 0, 1, 0, 0, 1, 0, 0, 0, 1},

                                                    {0, 0, 1, 1, 0, 0, 0, 0, 1, 1}};
        #endregion
        #region Initialize Form And Panel
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitAgentMap();
        }

        private void panel1_Paint(object sender, PaintEventArgs e )
        {
            this.DrawField(e);
            this.DrawAgent(agent.get_X(),agent.get_Y());
            this.KeyUp += new KeyEventHandler(MoveAgent);
        }
        #endregion
        #region Draw World and Agent
        public void DrawField(PaintEventArgs e)
        {
            // Create pen.
            Pen pen = new Pen(Color.Green, 1);
            // Create Brush
            SolidBrush blueBrush = new SolidBrush(Color.Blue);
            // Draw horizontal lines
            for (int x=0; x <= 500; x += 50) {
                Point i = new Point(x, 0);
                Point l = new Point(x, 500);
                e.Graphics.DrawLine(pen, i,l);
            }
            
            //Draw vertical lines
            for (int y = 0; y <= 500; y += 50)
            {
                Point i = new Point(0, y);
                Point l = new Point(500, y);
                e.Graphics.DrawLine(pen, i, l);
            }
            //Draw Rectangles to screen.
            for(int i = 0; i < 10; i++)
            {
                for(int j=0; j < 10; j++)
                {
                    if (mazePoints[j,i] == 1)
                    {
                        // Create rectangle.
                        Rectangle rect = new Rectangle(i*50, j*50, 50, 50);

                        // Draw rectangle to screen.
                        e.Graphics.FillRectangle(blueBrush, rect);
                    }
                    if(i==s0/10 && j == s0%10)
                    {
                        SolidBrush greenBrush = new SolidBrush(Color.Green);
                        Rectangle rect = new Rectangle(i * 50, j * 50, 50, 50);
                        // Draw rectangle to screen.
                        e.Graphics.FillRectangle(greenBrush, rect);
                    }
                    if(i==trapP % 10 && j == trapP/10)
                    {
                        SolidBrush redBrush = new SolidBrush(Color.Red);
                        Rectangle rect = new Rectangle(i * 50, j * 50, 50, 50);
                        // Draw rectangle to screen.
                        e.Graphics.FillRectangle(redBrush, rect);
                    }
                }
            }//end Draw Rectangles to screen.
        }//end DrawLinesPoint();
        public void DrawArrows(int x, int y,int direction)
        {
            Graphics g = panel1.CreateGraphics();
            Pen pen2 = new Pen(Color.Green, 8);
            pen2.StartCap = LineCap.ArrowAnchor;
            pen2.EndCap = LineCap.RoundAnchor;
            switch (direction)
            {
                case 0: g.DrawLine(pen2, x * 50 , y * 50 + 25, x * 50+ 40, y * 50 + 25); break; // left
                case 1: g.DrawLine(pen2, x * 50 + 50, y * 50 + 25,x* 50+10,y * 50 + 25); break; // right
                case 3: g.DrawLine(pen2, x * 50 + 25, y * 50+50 , x * 50 + 25, y * 50+10); break; // down
                case 2: g.DrawLine(pen2, x * 50 + 25, y * 50, x * 50 + 25, y * 50 + 40); break; // up
            }
            
        }
        public void DrawArrowsPriority(int x, int y, double[] values)
        {
            Graphics g = panel1.CreateGraphics();
            Pen pen2 = new Pen(Color.Green, 8);
            pen2.StartCap = LineCap.ArrowAnchor;
            pen2.EndCap = LineCap.RoundAnchor;
            int indexOf=5;
            double max = values.Max();
            for(int i=0; i < values.Length; i++)
            {
                if (values[i] == max && max >0) indexOf = i;
            }
            switch (indexOf)
            {
                case 0: g.DrawLine(pen2, x * 50, y * 50 + 25, x * 50 + 40, y * 50 + 25); break; // left
                case 1: g.DrawLine(pen2, x * 50 + 50, y * 50 + 25, x * 50 + 10, y * 50 + 25); break; // right
                case 3: g.DrawLine(pen2, x * 50 + 25, y * 50 + 50, x * 50 + 25, y * 50 + 10); break; // down
                case 2: g.DrawLine(pen2, x * 50 + 25, y * 50, x * 50 + 25, y * 50 + 40); break; // up
            }
        }
        public void DrawAgent(int x,int y)
        {
            SolidBrush tBrush = new SolidBrush(Color.Red);
            Graphics g = panel1.CreateGraphics();
            RectangleF rect = new RectangleF(x * 50, y * 50, 50,50);
            g.FillEllipse(tBrush, rect);
        }
        public void ClearDrawAgent(int x, int y)
        {
            SolidBrush tBrush = new SolidBrush(Control.DefaultBackColor);
            Graphics g = panel1.CreateGraphics();
            RectangleF rect = new RectangleF(x * 50, y * 50, 50, 50);
            g.FillEllipse(tBrush, rect);
        }
        #endregion

        #region Agent Class
        public class Agent
        {
            private int x,y,pos;
            public Agent(int x,int y)
            {
                this.x = x;
                this.y = y;
            }
            public int get_X()
            {
                return x;
            }
            public int get_Y()
            {
                return y;
            }
            public void set_X(int x)
            {
                this.x = x;
            }
            public void set_Y(int y)
            {
                this.y = y;
            }
            public bool Win(int x,int y)
            {
                pos = (x * 10) + (y % 10);
                if (pos == s0 || x==trapP%10 && y==trapP/10)
                {
                    if (x == trapP % 10 && y == trapP / 10) return true;
                   // MessageBox.Show("Win!");
                    return true;
                }
                return false;
            }
            public void moveUp()
            {
                if(Win(x,y-1) || y>0 && mazePoints[y-1,x] != 1) { 
                this.y-=1;
                }
            }
            public void moveDown()
            {
                if (Win(x, y + 1) || y < 9 && mazePoints[y+1,x] != 1)
                {
                    if (y + 1 == 10) return;
                    this.y+=1;
                }
            }
            public void moveLeft()
            {
                if (Win(x- 1, y ) || x > 0 && mazePoints[y,x-1] != 1)
                {
                    this.x -= 1;
                }
            }
            public void moveRight()
            {
                if (Win(x + 1, y) || x < 9 && mazePoints[y,x+1] != 1)
                {
                    this.x += 1;
                }
            }
        }
        #endregion

        #region Agent PressKey Moving Function
        public void MoveAgent(object sender, KeyEventArgs e)
        {
            if (e.Handled)
                return;
            {
                switch (e.KeyCode)
                {
                    case Keys.Up: { ClearDrawAgent(agent.get_X(), agent.get_Y()); DrawArrows(agent.get_X(),agent.get_Y(),2); agent.moveUp(); DrawAgent(agent.get_X(), agent.get_Y()); label1.Text = agent.get_X() + " : " + agent.get_Y(); break; }
                    case Keys.Down: { ClearDrawAgent(agent.get_X(), agent.get_Y()); DrawArrows(agent.get_X(), agent.get_Y(), 3); agent.moveDown(); DrawAgent(agent.get_X(), agent.get_Y()); label1.Text = agent.get_X() + " : " + agent.get_Y(); break; }
                    case Keys.Left: { ClearDrawAgent(agent.get_X(), agent.get_Y()); DrawArrows(agent.get_X(), agent.get_Y(), 0); agent.moveLeft(); DrawAgent(agent.get_X(), agent.get_Y()); label1.Text = agent.get_X() + " : " + agent.get_Y(); break; }
                    case Keys.Right: { ClearDrawAgent(agent.get_X(), agent.get_Y()); DrawArrows(agent.get_X(), agent.get_Y(), 1); agent.moveRight(); DrawAgent(agent.get_X(), agent.get_Y()); label1.Text = agent.get_X() + " : " + agent.get_Y(); break; }
                }//end switch()
            }
            e.Handled = true;
        }//end MoveAgent()
        #endregion

        //Agent Reinforcement Learning Q Learning
        #region Initialize Agent Known stuff
        
        public Dictionary<int, double[]> States;
        public void InitAgentMap()
        {
            States = new Dictionary<int, double[]>();
            for(int i = 0; i < 10; i++)
            {//i=y red j=x kolona
                for (int j = 0; j < 10; j++)
                {
                    if (i==s0%10 && j==s0/10)
                    {
                        States.Add((i * 10) + (j % 10), new double[1] { 1.00 }); continue;
                    }
                    if (i==trapP/10 && j==trapP%10) // trapP = 10 (x=1,y=0) > 
                    {
                        States.Add((i * 10) + (j % 10), new double[1] { -1.00 }); continue;
                    }
                    if (mazePoints[i, j] == 1) continue;
                    States.Add((i * 10) + (j % 10), new double[4] { 0.00, 0.00, 0.00, 0.00, });
                }
            }
            //ShowAgentMap(States);
        }
        public static String PrintValues(IEnumerable List)
        {
            string str= "";
            foreach (double actionRates in List)
            {
                str = str  + actionRates.ToString()+ ", ";
            }
            return str;
        }
        public void ShowAgentMap(Dictionary<int, double[]> States)
        {
            int count = 0;
            foreach (KeyValuePair<int,double[]> state in States)
            {
                Console.WriteLine("State:"+state.Key+" Values(Left,Right,Up,Down):"+ PrintValues(state.Value));
                count++;
            }
        }
        public void ShowAgentWay(Dictionary<int, double[]> States)
        {
            foreach (KeyValuePair<int, double[]> state in States)
            {
                if(state.Value.Max() > 0)
                {
                    Console.WriteLine("State:" + state.Key + " Values(Left,Right,Up,Down):" + PrintValues(state.Value));
                    DrawArrowsPriority((state.Key)%10,(state.Key)/10,state.Value); // y- first
                }
            }
        }
        #endregion

        #region Agent Self Movement 
        // Thinking :    имаме  Dictionary  с състояние сочещо към списък с коефициенти за всяка възможна посока
        //(Вкл. в списъка са финала и стените със стойност -1.00)
        int episode = 0;

        private void timer1_Tick(object sender, EventArgs e)
        {
            QLearning(States);
            if(agent.Win(agent.get_X(),agent.get_Y()))
            {
                episode++;
                agent.set_X(s1/10);
                agent.set_Y(s1%10);
                //panel1.Invalidate();
                label1.Text = "Episode: "+episode;
                //ShowAgentMap(States);
            }
            if (episode == breakLearningAfter) { Console.WriteLine("Way:"); timer1.Stop(); }
        }

        
        public void QLearning(Dictionary<int, double[]> States)
        {
            
                int agentState = (agent.get_Y() * 10) + agent.get_X(); // състоянието/позицията в която се намира агента
                double[] actionsForState = States[agentState]; // state actions
                Random rnd = new Random();
                int[] indexesOfMaxActions = GetIndexesOfmaxActionsThisState(actionsForState); // indeksite na max
                int direction = rnd.Next(0, indexesOfMaxActions.Length);// izbor na index
                int chosenDirection = indexesOfMaxActions[direction]; // izbran index
                ClearDrawAgent(agent.get_X(), agent.get_Y());
                DrawArrowsPriority(agent.get_X(), agent.get_Y(), actionsForState);
                switch (chosenDirection) // случаен избор на посоката на движение
                {
                    case 0: agent.moveLeft(); break;Console.WriteLine("LEft"); 
                    case 1: agent.moveRight(); break;Console.WriteLine("Right"); 
                    case 2: agent.moveUp();  break;Console.WriteLine("Up");
                    case 3: agent.moveDown();  break;Console.WriteLine("Down");
                }
                DrawAgent(agent.get_X(), agent.get_Y());
            
               // Console.WriteLine("In(" + agentState + "):" + PrintValues(States[agentState]));
                
                int  agentState2 = (agent.get_Y() * 10) + agent.get_X();

            if (agentState == agentState2)
            {
                actionsForState[chosenDirection] = -1; States[agentState] = actionsForState;
            }
            else
            {
                double[] nextStateActions = States[agentState2];
                // Console.WriteLine("NExt("+ agentState2 + "):"+PrintValues(nextStateActions));
                actionsForState[chosenDirection] += Alpha * (instantReward + (Sigma * nextStateActions.Max()) - actionsForState[chosenDirection]); // реизчисляваме Q коефициента  за този ход в това състояние
                actionsForState[chosenDirection] = Math.Round(actionsForState[chosenDirection], 3);
                States[agentState] = actionsForState;
            }
               //Console.WriteLine("LEfted(" + agentState + "):" + PrintValues(States[agentState]));
            
        }
        int[] GetIndexesOfmaxActionsThisState(double[] actions)
        {
            double max = actions.Max();
            List<int> indexOfMaxElements = new List<int>();
            for (int i = 0; i < actions.Length; i++)
            {
                if (max == actions[i]) indexOfMaxElements.Add(i);
            }
            int[] mass = indexOfMaxElements.ToArray();
            return mass;
        }
        #endregion
    }//end From()
}//end main()

