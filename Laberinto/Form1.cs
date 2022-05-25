using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

namespace Laberinto
{
    public partial class Form1 : Form
    {
        Color currentColor = Color.White;
        private int XTILES = 25; //Numero de pixeles en x
        private int YTILES = 25; //Numero de pixeles en y
        private int TILESIZE = 10; //Tamaño de pixeles
        private PictureBox[,] mazeTiles;
        private bool hilos = false;

        public Form1()
        {
            InitializeComponent();
            createNewMaze();
        }

        private void createNewMaze()
        {
            mazeTiles = new PictureBox[XTILES, YTILES];

            for (int i = 0; i < XTILES; i++)
            {
                for (int j = 0; j < YTILES; j++)
                {
                    //Inizializar la ubicacion del laberinto
                    mazeTiles[i, j] = new PictureBox();

                    //Posicion del laberinto
                    int xPosition = (i * TILESIZE) + 13;
                    int yPosition = (j * TILESIZE) + 45;
                    mazeTiles[i, j].SetBounds(xPosition, yPosition, TILESIZE, TILESIZE);

                    //Inicio y Final del laberinto
                    if ((i == 0 && j == 0) || (i == XTILES - 1 && j == YTILES - 1))
                    {
                        mazeTiles[i, j].BackColor = Color.Green;
                    }
                    else
                    {
                        //Lugares en blanco
                        mazeTiles[i, j].BackColor = Color.White;

                        //Modificar el laberinto
                        EventHandler clickEvent = new EventHandler(PictureBox_Click);
                        mazeTiles[i, j].Click += clickEvent;
                    }

                    //Añadir controles de modificacion
                    this.Controls.Add(mazeTiles[i, j]);
                }
            }
            
            //Generar muros
            for (int i = 0; i < XTILES; i++)
            {
                for (int j = 0; j < YTILES; j++)
                {
                    if(i % 2 != 0)
                    {
                        mazeTiles[i, j].BackColor = Color.Black;
                    }
                }
            }
            //Generar muros en codigo feo
            mazeTiles[3, 0].BackColor = Color.White;
            mazeTiles[7, 0].BackColor = Color.White;
            mazeTiles[11, 0].BackColor = Color.White;
            mazeTiles[15, 0].BackColor = Color.White;
            mazeTiles[19, 0].BackColor = Color.White;
            mazeTiles[23, 0].BackColor = Color.White;
            mazeTiles[1, 24].BackColor = Color.White;
            mazeTiles[5, 24].BackColor = Color.White;
            mazeTiles[9, 24].BackColor = Color.White;
            mazeTiles[13, 24].BackColor = Color.White;
            mazeTiles[17, 24].BackColor = Color.White;
            mazeTiles[21, 24].BackColor = Color.White;
        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {
            //Cambiar color a Blanco
            currentColor = Color.White;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            //Cambiar color a Negro
            currentColor = Color.Black;
        }

        private void PictureBox_Click(object sender, EventArgs e)
        {
            //Cambiar el color actual del laberinto
            ((PictureBox)sender).BackColor = currentColor;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            //Crear lugares por los que ya pasamos
            bool[,] alreadySearched = new bool[XTILES, YTILES];
            DateTime inicio = DateTime.Now;
            //Empieza a resolver de forma recursiva en el inicio
            if (!solveMaze(0, 0, alreadySearched))
                MessageBox.Show("Maze can not be solved");
            else
            {
                DateTime final = DateTime.Now;
                MessageBox.Show("Maze solved in " + (final - inicio));
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Limpia todo el recorrido
            for (int i = 0; i < XTILES; i++)
            {
                for (int j = 0; j < YTILES; j++)
                {
                    if (mazeTiles[i, j].BackColor == Color.Blue)
                        mazeTiles[i, j].BackColor = Color.White;
                }
            }

            //Vuelve a colocar el inicio y el final
            mazeTiles[0, 0].BackColor = Color.Green;
            mazeTiles[XTILES - 1, YTILES - 1].BackColor = Color.Green;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            bool[,] alreadySearched = new bool[XTILES, YTILES];
            DateTime inicio = DateTime.Now;
            //Creacion de hilos
            Thread Hilo1,Hilo2;
            if (!hilos)
            {
                Hilo1 = new Thread(() => IniciaResuelveHilos1(0, 0, alreadySearched));
                Hilo2 = new Thread(() => IniciaResuelveHilos2(0, 0, alreadySearched));

                Hilo1.Start();
                Hilo2.Start();

                DateTime final = DateTime.Now;
                MessageBox.Show("Maze solved in " + (final - inicio));
            }
        }

        private bool solveMaze(int xPos, int yPos, bool[,] alreadySearched)
        {
            bool correctPath = false;
            bool shouldCheck = true;

            //Comprobamos los limites del laberinto
            if (xPos >= XTILES || xPos < 0 || yPos >= YTILES || yPos < 0)
                shouldCheck = false;
            else
            {
                //Comprobamos si llego al final
                if (mazeTiles[xPos, yPos].BackColor == Color.Green && (xPos != 0 && yPos != 0))
                {
                    correctPath = true;
                    shouldCheck = false;
                }

                //Revisamos los muros
                if (mazeTiles[xPos, yPos].BackColor == Color.Black)
                    shouldCheck = false;

                //Revisamos si ya pasamos por ahi
                if (alreadySearched[xPos, yPos])
                    shouldCheck = false;
            }

            //Buscar en el laberinto
            if (shouldCheck)
            {
                //Mostrar camino
                alreadySearched[xPos, yPos] = true;

                //Movimiento
                correctPath = correctPath || solveMaze(xPos + 1, yPos, alreadySearched);
                correctPath = correctPath || solveMaze(xPos, yPos + 1, alreadySearched);
                correctPath = correctPath || solveMaze(xPos - 1, yPos, alreadySearched);
                correctPath = correctPath || solveMaze(xPos, yPos - 1, alreadySearched);
            }

            //Mostrar la ruta
            if (correctPath)
                mazeTiles[xPos, yPos].BackColor = Color.Blue;

            return correctPath;

        }

        private bool IniciaResuelveHilos1(int xPos, int yPos, bool[,] alreadySearched)
        {
            bool correctPath = false;
            bool shouldCheck = true;

            if (xPos >= XTILES || xPos < 0 || yPos >= YTILES || yPos < 0)
            {
                shouldCheck = false;
            }
            else
            {
                if (mazeTiles[xPos, yPos].BackColor == Color.Green && (xPos != 0 && yPos != 0))
                {
                    correctPath = true;
                    shouldCheck = false;
                }

                if (mazeTiles[xPos, yPos].BackColor == Color.Black)
                {
                    shouldCheck = false;
                }
                if (alreadySearched[xPos, yPos])
                {
                    shouldCheck = false;
                }
            }

            if (shouldCheck)
            {
                alreadySearched[xPos, yPos] = true;

                correctPath = correctPath || IniciaResuelveHilos1(xPos + 1, yPos, alreadySearched);
                correctPath = correctPath || IniciaResuelveHilos1(xPos, yPos + 1, alreadySearched);
                correctPath = correctPath || IniciaResuelveHilos1(xPos - 1, yPos, alreadySearched);
                correctPath = correctPath || IniciaResuelveHilos1(xPos, yPos - 1, alreadySearched);
            }

            if (correctPath)
            { 
                mazeTiles[xPos, yPos].BackColor = Color.Blue;
            }
            return correctPath;
        }

        private bool IniciaResuelveHilos2(int xPos, int yPos, bool[,] alreadySearched)
        {
            bool correctPath = false;
            bool shouldCheck = true;

            if (xPos >= XTILES || xPos < 0 || yPos >= YTILES || yPos < 0)
            {
                shouldCheck = false;
            }
            else
            {
                if (mazeTiles[xPos, yPos].BackColor == Color.Green && (xPos != 0 && yPos != 0))
                {
                    correctPath = true;
                    shouldCheck = false;
                }

                if (mazeTiles[xPos, yPos].BackColor == Color.Black)
                {
                    shouldCheck = false;
                }

                if (alreadySearched[xPos, yPos])
                {
                    shouldCheck = false;
                }
            }

            if (shouldCheck)
            {
                alreadySearched[xPos, yPos] = true;

                correctPath = correctPath || IniciaResuelveHilos2(xPos, yPos + 1, alreadySearched);
                correctPath = correctPath || IniciaResuelveHilos2(xPos - 1, yPos, alreadySearched);
                correctPath = correctPath || IniciaResuelveHilos2(xPos, yPos - 1, alreadySearched);
                correctPath = correctPath || IniciaResuelveHilos2(xPos + 1, yPos, alreadySearched);
            }

            if (correctPath)
            {
                mazeTiles[xPos, yPos].BackColor = Color.Blue;
            }
            return correctPath;
        }
    }
}
