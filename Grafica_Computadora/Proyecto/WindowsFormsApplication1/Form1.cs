using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Threading;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        int Herramienta=1;//empieza con linea seleccionada
        bool loaded = false;
        bool clickIzq = false;
        Bitmap Data;//variable donde guardamos lo que tiene la pantalla dibujado
        int xini=0,yini=0,lados=3;
        int puntox, puntoy;
        double r = 0,a;
        int xc = 0, yc = 0, xa = 0, ya = 0;
        int dx, dy;
        int rx, ry;
        int[] Puntos;//arreglo que sustituye a la estructura punto
        byte[] colorPrincipal,colorSecundario,colorFondo;//colores utilizados
        
        int X0, Y0, X1, Y1, X2, Y2, X3, Y3,nclicks=0;//curva y clicks
        Random random = new Random();//randoms
        int pincel=1;//tamaño pincel

        int[] Punto,Aux;
        public Form1()
        {
            InitializeComponent();
        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            Data = new Bitmap(glControl1.Width, glControl1.Height);
            loaded = true;
            GL.ClearColor(0f,0f,0f,1.0f);
            SetupViewport();
            colorPrincipal = new byte[3];
            colorPrincipal[0] = 0;
            colorPrincipal[1] = 255;
            colorPrincipal[2] = 0;
            colorSecundario = new byte[3];
            colorSecundario[0] = 0;
            colorSecundario[1] = 0;
            colorSecundario[2] = 0;
            GL.Color3(colorPrincipal[0], colorPrincipal[1], colorPrincipal[2]);
        }

        private void SetupViewport()
        {
            int w = glControl1.Width;
            int h = glControl1.Height;
            glControl1.CreateGraphics();
            GL.Ortho(0, w, 0, h, -1, 1);
            GL.Viewport(0, 0, w, h);
            
        }

        private void glControl1_Resize(object sender, EventArgs e)
        {
            if (!loaded)
                return;
            SetupViewport();
            glControl1.Invalidate();
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (!loaded)
                return;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            glControl1.SwapBuffers();
        }

        private void pixel(int x, int y)
        {
            if (Herramienta != 7)
            {
                    GL.Begin(BeginMode.Points);
                    GL.Vertex2(x, y);
                    GL.End();
            }
            else
            {
                GL.Color3(colorSecundario[0], colorSecundario[1], colorSecundario[2]);
                for (int i = -5 *pincel; i < 5* pincel; i++)
                    for (int j = -5* pincel; j < 5 * pincel; j++)
                    {
                        GL.Begin(BeginMode.Points);
                        GL.Vertex2(i + x, j + y);
                        GL.End();
                    }
            }
        }


        private void LineaBres(int xa, int ya, int xb, int yb)
        {
            int dx, dy, x, y, Fin, p, incX=0, incY=0;
            dx = Math.Abs(xa - xb);
            dy = Math.Abs(ya - yb);
            if (xa < xb)
            {
                incX = 1;
            }
            if (xa > xb)
            {
                incX = -1;
            }
            if (xa == xb)
            {
                incX = 0;
            }
            if (ya < yb)
            {
                incY = 1;
            }
            if (ya > yb)
            {
                incY = -1;
            }
            if (ya == yb)
            {
                incY = 0;
            }
            x = xa;
            y = ya;
            pixel(x, y);
            if (dx > dy)
            {
                Fin = dx;
                p = 2 * (dy - dx);
                while (Fin > 0)
                {
                    x = x + incX;
                    if (p < 0)
                        p = p + 2 * dy;
                    else
                    {
                        y = y + incY;
                        p = p + 2 * (dy - dx);
                    }
                    pixel(x, y);
                    Fin--;
                }
            }
            else
            {
                Fin = dy;
                p = 2 * (dx - dy);
                pixel(x, y);
                while (Fin > 0)
                {
                    y = y + incY;
                    if (p < 0)
                        p = p + 2 * dx;
                    else
                    {
                        x = x + incX;
                        p = p + 2 * (dx - dy);
                    }
                    pixel(x, y);
                    Fin--;
                }
            }
        }


        private void polar(int xc, int yc, int r, double a)
        {
            double radian;
            radian = 180 / 3.14159265358979323846;
            puntox = xc + Convert.ToInt32(Math.Ceiling(r * Math.Cos(a / radian)));
            puntoy = yc + Convert.ToInt32(Math.Ceiling(r * Math.Sin(a / radian)));
        }

        void circulo(int x, int y, int r)
        {
            int i;
            for (i = 0; i < 360; i++)
            {
                polar(x, y, r, i);
                pixel(puntox, puntoy);
            }
        }

        private void puntosCirculo(int xc, int yc, int x, int y)
        {
            pixel(xc + x, yc + y);
            pixel(xc - x, yc + y);
            pixel(xc + x, yc - y);
            pixel(xc - x, yc - y);

            pixel(xc + y, yc + x);
            pixel(xc - y, yc + x);
            pixel(xc + y, yc - x);
            pixel(xc - y, yc - x);
        }

        private void circulo2(int xc, int yc, int radio)
        {
            int x, y, p;
            x = 0;
            y = radio;
            puntosCirculo(xc, yc, x, y);
            p = 1 - radio;
            while (x < y)
            {
                if (p < 0)
                {
                    x++;
                    p += 2 * x + 1;
                }
                else
                {
                    x++;
                    y--;
                    p += 2 * (x - y) + 1;

                }
                puntosCirculo(xc, yc, x, y);
            }

        }

        void elipse(int xc, int yc, int rx, int ry)
        {
            double q = (2 * Math.PI) / 360;
            for (double i = 0; i < 2 * Math.PI; i += q)
            {

                pixel(Convert.ToInt32(Math.Ceiling(xc + (rx * Math.Cos(i)))), Convert.ToInt32(yc + Math.Ceiling(ry * Math.Sin(i))));
                pixel(Convert.ToInt32(Math.Ceiling(xc + (-rx * Math.Cos(i)))), Convert.ToInt32(yc + Math.Ceiling(ry * Math.Sin(i))));
                pixel(Convert.ToInt32(Math.Ceiling(xc + (rx * Math.Cos(i)))), Convert.ToInt32(yc + Math.Ceiling(-ry * Math.Sin(i))));
                pixel(Convert.ToInt32(Math.Ceiling(xc + (-rx * Math.Cos(i)))), Convert.ToInt32(yc + Math.Ceiling(-ry * Math.Sin(i))));
            }
        }

        void puntosElipse(int xc, int yc, int x, int y)
        {
            pixel(xc + x, yc + y);
            pixel(xc - x, yc + y);
            pixel(xc + x, yc - y);
            pixel(xc - x, yc - y);
        }

        void elipseCerrada(int xc, int yc, int rx, int ry)
        {
            double p, px, py, x, y, ry2, rx2, tworx2, twory2;
            ry2 = ry * ry;
            rx2 = rx * rx;
            twory2 = 2 * ry2;
            tworx2 = 2 * rx2;
            //region 1
            x = 0;
            y = ry;
            puntosElipse(xc, yc, Convert.ToInt32(Math.Ceiling(x)), Convert.ToInt32(Math.Ceiling(y)));
            p = Math.Ceiling(ry2 - rx2 * ry + (0.25 * rx2));
            px = 0;
            py = tworx2 * y;
            while (px < py)
            {
                x++;
                px += twory2;
                if (p >= 0)
                {
                    y--;
                    py -= tworx2;
                }
                if (p < 0)
                {
                    p += ry2 + px;
                }
                else
                {
                    p += ry2 + px - py;
                }
                puntosElipse(xc, yc, Convert.ToInt32(Math.Ceiling(x)), Convert.ToInt32(Math.Ceiling(y)));
            }
            //region 2
            p = Math.Ceiling(ry2 * (x + 0.5) * (x + 0.5) + rx2 * (y - 1) * (y - 1) - rx2 * ry2);
            while (y > 0)
            {
                y--;
                py -= tworx2;
                if (p <= 0)
                {
                    x++;
                    px += twory2;
                }
                if (p > 0)
                {
                    p += rx2 - py;
                }
                else
                {
                    p += rx2 - py + px;
                }
                puntosElipse(xc, yc, Convert.ToInt32(Math.Ceiling(x)), Convert.ToInt32(Math.Ceiling(y)));
            }
        }

        void solidos(int xc, int yc, int radio, double a, int lados)
        {
            int ang = 360 / lados;
            int l;
            Puntos = new int[20];
            for (l = 0; l/2 < lados; l+=2)
            {
                polar(xc, yc, radio, a);
                a += ang;
                Puntos[l] = puntox;
                Puntos[l+1] = puntoy;
            }
            for (l = 0; l/2 < lados - 1; l+=2)
                LineaBres(Puntos[l], Puntos[l+1], Puntos[l + 2], Puntos[l + 3]);
            LineaBres(Puntos[l], Puntos[l+1], Puntos[0], Puntos[1]);

        }

        void curva(int x0, int y0, int x1, int y1, int x2, int y2, int x3, int y3)
        {
            double time;
            int x, y;
            for (time = 0; time <= 1.0; time += 0.001)
            {
                x = Convert.ToInt32(Math.Pow(1 - time, 3) * x0 + 3 * time * Math.Pow(1 - time, 2) * x1 + 3 * Math.Pow(time, 2) * (1 - time) * x2 + Math.Pow(time, 3) * x3);
                y = Convert.ToInt32(Math.Pow(1 - time, 3) * y0 + 3 * time * Math.Pow(1 - time, 2) * y1 + 3 * Math.Pow(time, 2) * (1 - time) * y2 + Math.Pow(time, 3) * y3);
                pixel(x, y);
            }
        }

        private bool compara(byte[] A, byte[] B)
        {
            if((A[0] == B[0]&&A[1] == B[1]&&A[2] == B[2]))
                        return true;
            return false;
        }

        private void inunda(int x, int y)
        {
             Queue<int> queue = new Queue<int>();
             var Fondo = new byte[4];
             var Pixel = new byte[4];
             Punto = new int[2];
             Aux = new int[2];
             int C=0;
             GL.ReadPixels(x, y, 1, 1, PixelFormat.Rgb, PixelType.UnsignedByte, Fondo);
             GL.Color3(colorSecundario[0], colorSecundario[1], colorSecundario[2]);
             if (!compara(Fondo, colorSecundario))
             {
                 Punto[0] = x;
                 Punto[1] = y;
                 queue.Enqueue(Punto[0]);
                 queue.Enqueue(Punto[1]);
             }
             else
                 return;
            
             do
             {
                 Punto[0] = queue.Dequeue();
                 Punto[1] = queue.Dequeue();
                 GL.ReadPixels(Punto[0], Punto[1], 1, 1, PixelFormat.Rgb, PixelType.UnsignedByte, Pixel);
                 if (compara(Pixel, Fondo) && !compara(Pixel, colorSecundario) && Punto[0] <= glControl1.Width && Punto[1] <= glControl1.Height && Punto[0] >= 0 && Punto[1] >= 0)
                 {
                     pixel(Punto[0], Punto[1]);
                     Aux[0] = Punto[0] + 1;
                     Aux[1] = Punto[1];
                     queue.Enqueue(Aux[0]);
                     queue.Enqueue(Aux[1]);
                     Aux[0] = Punto[0] - 1;
                     Aux[1] = Punto[1];
                     queue.Enqueue(Aux[0]);
                     queue.Enqueue(Aux[1]);
                     Aux[0] = Punto[0];
                     Aux[1] = Punto[1] + 1;
                     queue.Enqueue(Aux[0]);
                     queue.Enqueue(Aux[1]);
                     Aux[0] = Punto[0];
                     Aux[1] = Punto[1] - 1;
                     queue.Enqueue(Aux[0]);
                     queue.Enqueue(Aux[1]);
                     C++;
                     if (C == 1024)
                     {
                         GrabScreen();
                         GL.Flush();
                         glControl1.SwapBuffers();
                         PutScreen();
                         
                         C = 0;
                     }
                 }
                 
                 
             } while (queue.Count != 0);
             GrabScreen();
             GL.Flush();
             glControl1.SwapBuffers();
             PutScreen();
        }


        private void glControl1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            
            if (clickIzq)
            {
                switch (Herramienta)
                {
                    case 1://linea
                        GL.Flush();
                        PutScreen();
                        LineaBres(xini, yini, e.X, Math.Abs(e.Y-glControl1.Height));
                        glControl1.SwapBuffers();
                        break;
                    case 2://circulo
                        dx = Math.Abs(xc - e.X);
                        dy = Math.Abs(yc - Math.Abs(e.Y - glControl1.Height));
                        r = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
                        GL.Flush();
                        PutScreen();
                        circulo(xc, yc, Convert.ToInt32(r));
                        glControl1.SwapBuffers();
                        break;
                    case 3:
                        rx = Math.Abs(xc - e.X);
                        ry = Math.Abs(yc - Math.Abs(e.Y - glControl1.Height));
                        GL.Flush();
                        PutScreen();
                        elipse(xc, yc, rx, ry);
                        glControl1.SwapBuffers();
                        break;
                    case 4: 
                        dx = xc - e.X;
                        dy = yc - Math.Abs(e.Y - glControl1.Height);
                        r = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
                        a = (180 / Math.PI) * (Math.Atan2(yc - Math.Abs(e.Y - glControl1.Height), (xc - e.X)));
                        GL.Flush();
                        PutScreen();
                        solidos(xc, yc, Convert.ToInt32(r), a, lados);
                        glControl1.SwapBuffers();
                        break;
                    case 5:
                        switch (nclicks)
                        {
                            case 0:
                                X1 = X2 = X3 = e.X;
                                Y1 = Y2 = Y3 = Math.Abs(e.Y - glControl1.Height);
                                break;
                            case 1:
                                X1 = X2 = e.X;
                                Y1 = Y2 = Math.Abs(e.Y - glControl1.Height);
                                break;
                            case 2:
                                X2 = e.X;
                                Y2 = Math.Abs(e.Y - glControl1.Height);
                                break;
                        }
                        GL.Flush();
                        PutScreen();
                        curva(X0, Y0, X1, Y1, X2, Y2, X3, Y3);
                        glControl1.SwapBuffers();
                        break;
                    case 6:
                        GL.Flush();
                        PutScreen();
                        LineaBres(xini, yini, e.X, Math.Abs(e.Y - glControl1.Height));
                        GrabScreen();
		                xini=e.X;
                        yini = Math.Abs(e.Y - glControl1.Height);
                        glControl1.SwapBuffers();
                        break;
                    case 7:
                        GL.Flush();
                        PutScreen();
                        LineaBres(xini, yini, e.X, Math.Abs(e.Y - glControl1.Height));
                        GrabScreen();
		                xini=e.X;
                        yini = Math.Abs(e.Y - glControl1.Height);
                        glControl1.SwapBuffers();
                        break;
                    case 8:

                        xini=e.X;
                        yini = Math.Abs(e.Y - glControl1.Height);
                        break;

                    default:
                        break;
                }
            }

        }

        private void glControl1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    switch (Herramienta)
                    {
                        case 1: 
                            xini = e.X;
                            yini = Math.Abs(e.Y - glControl1.Height);
                            clickIzq = true;
                            break;
                        case 2:
                            xc = e.X;
                            yc = Math.Abs(e.Y - glControl1.Height);
                            clickIzq = true;
                            break;
                        case 3:
                            xc = e.X;
                            yc = Math.Abs(e.Y - glControl1.Height);
                            clickIzq = true;
                            break;
                        case 4:
                            xc = e.X;
                            yc = Math.Abs(e.Y - glControl1.Height);
                            clickIzq = true;
                            break;
                        case 5:
                            switch (nclicks)
                            {
                                case 0:
                                    X0 = e.X;
                                    Y0 = Math.Abs(e.Y - glControl1.Height);
                                    break;
                                case 1:
                                    X1 = e.X;
                                    Y1 = Math.Abs(e.Y - glControl1.Height);
                                    break;
                                case 2:
                                    X2 = e.X;
                                    Y2 = Math.Abs(e.Y - glControl1.Height);
                                    break;
                            }
                            clickIzq = true;
                            break;//case 5
                        case 6:
                            xini=e.X;
                            yini = Math.Abs(e.Y - glControl1.Height);
		                    
                            clickIzq = true;
                            break;
                        case 7:
                            xini = e.X;
                            yini = Math.Abs(e.Y - glControl1.Height);
                            clickIzq = true;
                            break;
                        case 8:
                            xini = e.X;
                            yini = Math.Abs(e.Y - glControl1.Height);
                            timer1.Enabled = true;
                            clickIzq = true;
                            break;
                        case 9:
                            GL.Flush();
                            PutScreen();
                            glControl1.SwapBuffers();
                            inunda(e.X, Math.Abs(e.Y - glControl1.Height));
                            break;
                        default:
                            break;
                    }
                    break;//case click left
                default:
                    break;
            }
        }

        private void glControl1_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    switch (Herramienta)
                    {
                        case 1:
                            clickIzq = false;
                            GrabScreen();
                            GL.Flush();
                            PutScreen();
                            glControl1.SwapBuffers();
                            break;
                        case 2:
                            clickIzq = false;
                            GL.Flush();
                            PutScreen();
                            circulo2(xc, yc, Convert.ToInt32(r));
                            GrabScreen();
                            glControl1.SwapBuffers();
                            break;
                        case 3:
                            clickIzq = false;
                            GL.Flush();
                            PutScreen();
                            elipseCerrada(xc, yc, rx, ry);
                            GrabScreen();
                            glControl1.SwapBuffers();
                            break;
                        case 4:
                            clickIzq = false;
                            GL.Flush();
                            PutScreen();
                            solidos(xc, yc, Convert.ToInt32(r), a, lados);
                            GrabScreen();
                            glControl1.SwapBuffers();
                            break;
                        case 5:
                            clickIzq = false;
                            if (nclicks != 2){
                                nclicks++;
                            }
                            else
                            {
                                nclicks = 0;
                                GL.Flush();
                                PutScreen();
                                curva(X0, Y0, X1, Y1, X2, Y2, X3, Y3);
                                GrabScreen();
                            }
                            glControl1.SwapBuffers();
                            break;
                        case 6:
                            clickIzq = false;
                            GL.Flush();
                            PutScreen();
                            pixel(xini, yini);
                            GrabScreen();
                            glControl1.SwapBuffers();
                            break;
                        case 7:
                            clickIzq = false;
                            GL.Flush();
                            PutScreen();
                            pixel(xini, yini);
                            GrabScreen();
                            glControl1.SwapBuffers();
                            break;
                        case 8:
                            clickIzq = false;
                            timer1.Enabled = false;
                            break;
                    }
                    break;//case boton left
                default:
                    break;
            }
        }
        private void GrabScreen()
        {
            if (GraphicsContext.CurrentContext == null)
                throw new GraphicsContextMissingException();

            System.Drawing.Imaging.BitmapData data = Data.LockBits(glControl1.ClientRectangle, System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            GL.ReadPixels(0, 0, glControl1.Width, glControl1.Height, PixelFormat.Rgb, PixelType.UnsignedByte, data.Scan0);
            Data.UnlockBits(data);
            
            //bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
        }
        private void PutScreen()
        {
            System.Drawing.Imaging.BitmapData data = Data.LockBits(glControl1.ClientRectangle, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            GL.DrawPixels(glControl1.Width, glControl1.Height, PixelFormat.Rgb, PixelType.UnsignedByte, data.Scan0);
            Data.UnlockBits(data);
        }

        private void panel1_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    panel23.BackColor = Color.FromArgb(255, 255, 255);
                    colorPrincipal[0] = 255;
                    colorPrincipal[1] = 255;
                    colorPrincipal[2] = 255;
                    break;
                case MouseButtons.Right:
                    panel24.BackColor = Color.FromArgb(255, 255, 255);
                    colorSecundario[0] = 255;
                    colorSecundario[1] = 255;
                    colorSecundario[2] = 255;
                    break;

                default:
                    break;
            }
            if (Herramienta != 7)
                GL.Color3(colorPrincipal[0], colorPrincipal[1], colorPrincipal[2]);
            else
                GL.Color3(colorSecundario[0], colorSecundario[1], colorSecundario[2]);
        }

        private void panel2_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
                switch (e.Button)
            {
                case MouseButtons.Left:
                    panel23.BackColor = Color.FromArgb(255, 192, 192);
                    colorPrincipal[0] = 255;
                    colorPrincipal[1] = 192;
                    colorPrincipal[2] = 192;
                    break;
                case MouseButtons.Right:
                    panel24.BackColor = Color.FromArgb(255, 192, 192);
                    colorSecundario[0] = 255;
                    colorSecundario[1] = 192;
                    colorSecundario[2] = 192;
                    break;
                default:
                    break;
            }
                if (Herramienta != 7)
                    GL.Color3(colorPrincipal[0], colorPrincipal[1], colorPrincipal[2]);
                else
                    GL.Color3(colorSecundario[0], colorSecundario[1], colorSecundario[2]);
        }

        private void panel3_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
                switch (e.Button)
            {
                case MouseButtons.Left:
                    panel23.BackColor = Color.FromArgb(255, 255, 192);
                    colorPrincipal[0] = 255;
                    colorPrincipal[1] = 255;
                    colorPrincipal[2] = 192;
                    break;
                case MouseButtons.Right:
                    panel24.BackColor = Color.FromArgb(255, 255, 192);
                    colorSecundario[0] = 255;
                    colorSecundario[1] = 255;
                    colorSecundario[2] = 192;
                    break;
                default:
                    break;
            }
                if (Herramienta != 7)
                    GL.Color3(colorPrincipal[0], colorPrincipal[1], colorPrincipal[2]);
                else
                    GL.Color3(colorSecundario[0], colorSecundario[1], colorSecundario[2]);
        }

        private void panel4_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    panel23.BackColor = Color.FromArgb(192, 255, 192);
                    colorPrincipal[0] = 192;
                    colorPrincipal[1] = 255;
                    colorPrincipal[2] = 192;
                    break;
                case MouseButtons.Right:
                    panel24.BackColor = Color.FromArgb(192, 255, 192);
                    colorSecundario[0] = 192;
                    colorSecundario[1] = 255;
                    colorSecundario[2] = 192;
                    break;
                default:
                    break;
            }
            if (Herramienta != 7)
                GL.Color3(colorPrincipal[0], colorPrincipal[1], colorPrincipal[2]);
            else
                GL.Color3(colorSecundario[0], colorSecundario[1], colorSecundario[2]);
        }

        private void panel5_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    panel23.BackColor = Color.FromArgb(192, 192, 255);
                    colorPrincipal[0] = 192;
                    colorPrincipal[1] = 192;
                    colorPrincipal[2] = 255;
                    break;
                case MouseButtons.Right:
                    panel24.BackColor = Color.FromArgb(192, 192, 255);
                    colorSecundario[0] = 192;
                    colorSecundario[1] = 192;
                    colorSecundario[2] = 255;
                    break;
                default:
                    break;
            }
            if (Herramienta != 7)
                GL.Color3(colorPrincipal[0], colorPrincipal[1], colorPrincipal[2]);
            else
                GL.Color3(colorSecundario[0], colorSecundario[1], colorSecundario[2]);
        }

        private void panel6_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    panel23.BackColor = Color.FromArgb(255, 255, 128);
                    colorPrincipal[0] = 255;
                    colorPrincipal[1] = 255;
                    colorPrincipal[2] = 128;
                    break;
                case MouseButtons.Right:
                    panel24.BackColor = Color.FromArgb(255, 255, 128);
                    colorSecundario[0] = 255;
                    colorSecundario[1] = 255;
                    colorSecundario[2] = 128;
                    break;
                default:
                    break;
            }
            if (Herramienta != 7)
                GL.Color3(colorPrincipal[0], colorPrincipal[1], colorPrincipal[2]);
            else
                GL.Color3(colorSecundario[0], colorSecundario[1], colorSecundario[2]);
        }

        private void panel7_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    panel23.BackColor = Color.FromArgb(128, 255, 128);
                    colorPrincipal[0] = 128;
                    colorPrincipal[1] = 255;
                    colorPrincipal[2] = 128;
                    break;
                case MouseButtons.Right:
                    panel24.BackColor = Color.FromArgb(128, 255, 128);
                    colorSecundario[0] = 128;
                    colorSecundario[1] = 255;
                    colorSecundario[2] = 128;
                    break;
                default:
                    break;
            }
            if (Herramienta != 7)
                GL.Color3(colorPrincipal[0], colorPrincipal[1], colorPrincipal[2]);
            else
                GL.Color3(colorSecundario[0], colorSecundario[1], colorSecundario[2]);
        }

        private void panel8_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    panel23.BackColor = Color.FromArgb(128, 255, 255);
                    colorPrincipal[0] = 128;
                    colorPrincipal[1] = 255;
                    colorPrincipal[2] = 255;
                    break;
                case MouseButtons.Right:
                    panel24.BackColor = Color.FromArgb(128, 255, 255);
                    colorSecundario[0] = 128;
                    colorSecundario[1] = 255;
                    colorSecundario[2] = 255;
                    break;
                default:
                    break;
            }
            if (Herramienta != 7)
                GL.Color3(colorPrincipal[0], colorPrincipal[1], colorPrincipal[2]);
            else
                GL.Color3(colorSecundario[0], colorSecundario[1], colorSecundario[2]);
        }

        private void panel9_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    panel23.BackColor = Color.FromArgb(128, 128, 255);
                    colorPrincipal[0] = 128;
                    colorPrincipal[1] = 128;
                    colorPrincipal[2] = 255;
                    break;
                case MouseButtons.Right:
                    panel24.BackColor = Color.FromArgb(128, 128, 255);
                    colorSecundario[0] = 128;
                    colorSecundario[1] = 128;
                    colorSecundario[2] = 255;
                    break;
                default:
                    break;
            }
            if (Herramienta != 7)
                GL.Color3(colorPrincipal[0], colorPrincipal[1], colorPrincipal[2]);
            else
                GL.Color3(colorSecundario[0], colorSecundario[1], colorSecundario[2]);
        }

        private void panel10_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    panel23.BackColor = Color.FromArgb(255, 128, 255);
                    colorPrincipal[0] = 255;
                    colorPrincipal[1] = 128;
                    colorPrincipal[2] = 255;
                    break;
                case MouseButtons.Right:
                    panel24.BackColor = Color.FromArgb(255, 128, 255);
                    colorSecundario[0] = 255;
                    colorSecundario[1] = 128;
                    colorSecundario[2] = 255;
                    break;
                default:
                    break;
            }
            if (Herramienta != 7)
                GL.Color3(colorPrincipal[0], colorPrincipal[1], colorPrincipal[2]);
            else
                GL.Color3(colorSecundario[0], colorSecundario[1], colorSecundario[2]);
        }

        private void panel11_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    panel23.BackColor = Color.Red;
                    colorPrincipal[0] = 255;
                    colorPrincipal[1] = 0;
                    colorPrincipal[2] = 0;
                    break;
                case MouseButtons.Right:
                    panel24.BackColor = Color.Red;
                    colorSecundario[0] = 255;
                    colorSecundario[1] = 0;
                    colorSecundario[2] = 0;
                    break;
                default:
                    break;
            }
            if (Herramienta != 7)
                GL.Color3(colorPrincipal[0], colorPrincipal[1], colorPrincipal[2]);
            else
                GL.Color3(colorSecundario[0], colorSecundario[1], colorSecundario[2]);
        }

        private void panel12_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    panel23.BackColor = Color.FromArgb(255, 128, 0);
                    colorPrincipal[0] = 255;
                    colorPrincipal[1] = 128;
                    colorPrincipal[2] = 0;
                    break;
                case MouseButtons.Right:
                    panel24.BackColor = Color.FromArgb(255, 128, 0);
                    colorSecundario[0] = 255;
                    colorSecundario[1] = 128;
                    colorSecundario[2] = 0;
                    break;
                default:
                    break;
            }
            if (Herramienta != 7)
                GL.Color3(colorPrincipal[0], colorPrincipal[1], colorPrincipal[2]);
            else
                GL.Color3(colorSecundario[0], colorSecundario[1], colorSecundario[2]);
        }

        private void panel13_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    panel23.BackColor = Color.Yellow;
                    colorPrincipal[0] = 255;
                    colorPrincipal[1] = 255;
                    colorPrincipal[2] = 0;
                    break;
                case MouseButtons.Right:
                    panel24.BackColor = Color.FromArgb(255, 255, 0);
                    colorSecundario[0] = 255;
                    colorSecundario[1] = 255;
                    colorSecundario[2] = 0;
                    break;
                default:
                    break;
            }
            if (Herramienta != 7)
                GL.Color3(colorPrincipal[0], colorPrincipal[1], colorPrincipal[2]);
            else
                GL.Color3(colorSecundario[0], colorSecundario[1], colorSecundario[2]);
        }

        private void panel14_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    panel23.BackColor = Color.Lime;
                    colorPrincipal[0] = 0;
                    colorPrincipal[1] = 255;
                    colorPrincipal[2] = 0;
                    break;
                case MouseButtons.Right:
                    panel24.BackColor = Color.FromArgb(0, 255, 0);
                    colorSecundario[0] = 0;
                    colorSecundario[1] = 255;
                    colorSecundario[2] = 0;
                    break;
                default:
                    break;
            }
            if (Herramienta != 7)
                GL.Color3(colorPrincipal[0], colorPrincipal[1], colorPrincipal[2]);
            else
                GL.Color3(colorSecundario[0], colorSecundario[1], colorSecundario[2]);
        }

        private void panel15_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    panel23.BackColor = Color.Aqua;
                    colorPrincipal[0] = 0;
                    colorPrincipal[1] = 255;
                    colorPrincipal[2] = 255;
                    break;
                case MouseButtons.Right:
                    panel24.BackColor = Color.FromArgb(0, 255, 255);
                    colorSecundario[0] = 0;
                    colorSecundario[1] = 255;
                    colorSecundario[2] = 255;
                    break;
                default:
                    break;
            }
            if (Herramienta != 7)
                GL.Color3(colorPrincipal[0], colorPrincipal[1], colorPrincipal[2]);
            else
                GL.Color3(colorSecundario[0], colorSecundario[1], colorSecundario[2]);
        }

        private void panel16_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    panel23.BackColor = Color.Blue;
                    colorPrincipal[0] = 0;
                    colorPrincipal[1] = 0;
                    colorPrincipal[2] = 255;
                    break;
                case MouseButtons.Right:
                    panel24.BackColor = Color.FromArgb(0, 0, 255);
                    colorSecundario[0] = 0;
                    colorSecundario[1] = 0;
                    colorSecundario[2] = 255;
                    break;
                default:
                    break;
            }
            if (Herramienta != 7)
                GL.Color3(colorPrincipal[0], colorPrincipal[1], colorPrincipal[2]);
            else
                GL.Color3(colorSecundario[0], colorSecundario[1], colorSecundario[2]);
        }

        private void panel17_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    panel23.BackColor = Color.Fuchsia;
                    colorPrincipal[0] = 255;
                    colorPrincipal[1] = 0;
                    colorPrincipal[2] = 255;
                    break;
                case MouseButtons.Right:
                    panel24.BackColor = Color.FromArgb(255, 0, 255);
                    colorSecundario[0] = 255;
                    colorSecundario[1] = 0;
                    colorSecundario[2] = 255;
                    break;
                default:
                    break;
            }
            if (Herramienta != 7)
                GL.Color3(colorPrincipal[0], colorPrincipal[1], colorPrincipal[2]);
            else
                GL.Color3(colorSecundario[0], colorSecundario[1], colorSecundario[2]);
        }

        private void panel18_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    GL.Color3(Color.Maroon);
                    panel23.BackColor = Color.Maroon;
                    colorPrincipal[0] = 128;
                    colorPrincipal[1] = 0;
                    colorPrincipal[2] = 0;
                    break;
                case MouseButtons.Right:
                    panel24.BackColor = Color.FromArgb(128, 0, 0);
                    colorSecundario[0] = 128;
                    colorSecundario[1] = 0;
                    colorSecundario[2] = 0;
                    break;
                default:
                    break;
            }
            if (Herramienta != 7)
                GL.Color3(colorPrincipal[0], colorPrincipal[1], colorPrincipal[2]);
            else
                GL.Color3(colorSecundario[0], colorSecundario[1], colorSecundario[2]);
        }

        private void panel19_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:                    
                    panel23.BackColor = Color.Olive;
                    colorPrincipal[0] = 128;
                    colorPrincipal[1] = 128;
                    colorPrincipal[2] = 0;
                    break;
                case MouseButtons.Right:
                    panel24.BackColor = Color.FromArgb(128, 128, 0);
                    colorSecundario[0] = 128;
                    colorSecundario[1] = 128;
                    colorSecundario[2] = 0;
                    break;
                default:
                    break;
            }
            if (Herramienta != 7)
                GL.Color3(colorPrincipal[0], colorPrincipal[1], colorPrincipal[2]);
            else
                GL.Color3(colorSecundario[0], colorSecundario[1], colorSecundario[2]);
        }

        private void panel20_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    panel23.BackColor = Color.Green;
                    colorPrincipal[0] = 0;
                    colorPrincipal[1] = 128;
                    colorPrincipal[2] = 0;
                    break;
                case MouseButtons.Right:
                    panel24.BackColor = Color.FromArgb(0, 128, 0);
                    colorSecundario[0] = 0;
                    colorSecundario[1] = 128;
                    colorSecundario[2] = 0;
                    break;
                default:
                    break;
            }
            if (Herramienta != 7)
                GL.Color3(colorPrincipal[0], colorPrincipal[1], colorPrincipal[2]);
            else
                GL.Color3(colorSecundario[0], colorSecundario[1], colorSecundario[2]);
        }

        private void panel21_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    panel23.BackColor = Color.Navy;
                    colorPrincipal[0] = 0;
                    colorPrincipal[1] = 0;
                    colorPrincipal[2] = 128;
                    break;
                case MouseButtons.Right:
                    panel24.BackColor = Color.FromArgb(0, 0, 128);
                    colorSecundario[0] = 0;
                    colorSecundario[1] = 0;
                    colorSecundario[2] = 128;
                    break;
                default:
                    break;
            }
            if (Herramienta != 7)
                GL.Color3(colorPrincipal[0], colorPrincipal[1], colorPrincipal[2]);
            else
                GL.Color3(colorSecundario[0], colorSecundario[1], colorSecundario[2]);
        }

        private void panel22_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    panel23.BackColor=Color.Black;
                    colorPrincipal[0] = 0;
                    colorPrincipal[1] = 0;
                    colorPrincipal[2] = 0;
                    break;
                case MouseButtons.Right:
                    panel24.BackColor = Color.FromArgb(0, 0, 0);
                    colorSecundario[0] = 0;
                    colorSecundario[1] = 0;
                    colorSecundario[2] = 0;
                    break;
                default:
                    break;
            }
            if (Herramienta != 7)
                GL.Color3(colorPrincipal[0], colorPrincipal[1], colorPrincipal[2]);
            else
                GL.Color3(colorSecundario[0], colorSecundario[1], colorSecundario[2]);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Herramienta = 1;
            GL.PointSize(trackBar2.Value);
            trackBar1.Enabled = false;
            GL.Color3(colorPrincipal[0], colorPrincipal[1], colorPrincipal[2]);
            if (nclicks != 0)
            {
                DialogResult result2 = MessageBox.Show("El trazado de la curva aún no termina. Si continua la curva sera borrada.\n¿Desea Continuar?","Curva", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result2 != DialogResult.No)
                {
                    PutScreen();
                    nclicks = 0;
                    glControl1.SwapBuffers();
                }
                else
                    Herramienta = 5;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Herramienta = 2;
            GL.PointSize(trackBar2.Value);
            trackBar1.Enabled = false;
            GL.Color3(colorPrincipal[0], colorPrincipal[1], colorPrincipal[2]);
            if (nclicks != 0)
            {
                DialogResult result2 = MessageBox.Show("El trazado de la curva aún no termina. Si continua la curva sera borrada.\n¿Desea Continuar?", "Curva", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result2 != DialogResult.No)
                {
                    PutScreen();
                    nclicks = 0;
                    glControl1.SwapBuffers();
                }
                else
                    Herramienta = 5;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Herramienta = 3;
            GL.PointSize(trackBar2.Value);
            trackBar1.Enabled = false;
            GL.Color3(colorPrincipal[0], colorPrincipal[1], colorPrincipal[2]);
            if (nclicks != 0)
            {
                DialogResult result2 = MessageBox.Show("El trazado de la curva aún no termina. Si continua la curva sera borrada.\n¿Desea Continuar?", "Curva", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result2 != DialogResult.No)
                {
                    PutScreen();
                    nclicks = 0;
                    glControl1.SwapBuffers();
                }
                else
                    Herramienta = 5;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Herramienta = 4;
            GL.PointSize(trackBar2.Value);
            trackBar1.Enabled = true;
            label4.Text = "Lados: " + trackBar1.Value;
            GL.Color3(colorPrincipal[0], colorPrincipal[1], colorPrincipal[2]);
            if (nclicks != 0)
            {
                DialogResult result2 = MessageBox.Show("El trazado de la curva aún no termina. Si continua la curva sera borrada.\n¿Desea Continuar?", "Curva", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result2 != DialogResult.No)
                {
                    PutScreen();
                    nclicks = 0;
                    glControl1.SwapBuffers();
                }
                else
                {
                    Herramienta = 5;
                    trackBar1.Enabled = false;
                }
            }
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            lados = trackBar1.Value;
            label4.Text = "Lados: " + trackBar1.Value;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Herramienta = 5;
            GL.PointSize(trackBar2.Value);
            trackBar1.Enabled = false;
            GL.Color3(colorPrincipal[0], colorPrincipal[1], colorPrincipal[2]);
            if (nclicks != 0)
            {
                DialogResult result2 = MessageBox.Show("El trazado de la curva aún no termina. Si continua la curva sera borrada.\n¿Desea Continuar?", "Curva", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result2 != DialogResult.No)
                {
                    nclicks = 0;
                    PutScreen();
                    glControl1.SwapBuffers();
                }
                else
                    Herramienta = 5;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Herramienta = 6;
            GL.PointSize(trackBar2.Value);
            trackBar1.Enabled = false;
            GL.Color3(colorPrincipal[0], colorPrincipal[1], colorPrincipal[2]);
            if (nclicks != 0)
            {
                DialogResult result2 = MessageBox.Show("El trazado de la curva aún no termina. Si continua la curva sera borrada.\n¿Desea Continuar?", "Curva", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result2 != DialogResult.No)
                {
                    nclicks = 0;
                    PutScreen();
                    glControl1.SwapBuffers();
                }
                else
                    Herramienta = 5;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Herramienta = 7;
            GL.PointSize(trackBar2.Value);
            trackBar1.Enabled = false;
            GL.Color3(colorSecundario[0], colorSecundario[1], colorSecundario[2]);
            if (nclicks != 0)
            {
                DialogResult result2 = MessageBox.Show("El trazado de la curva aún no termina. Si continua la curva sera borrada.\n¿Desea Continuar?", "Curva", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result2 != DialogResult.No)
                {
                    nclicks = 0;
                    PutScreen();
                    glControl1.SwapBuffers();
                }
                else
                    Herramienta = 5;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Herramienta = 8;
            GL.PointSize(1f);
            trackBar1.Enabled = false;
            GL.Color3(colorPrincipal[0], colorPrincipal[1], colorPrincipal[2]);
            if (nclicks != 0)
            {
                DialogResult result2 = MessageBox.Show("El trazado de la curva aún no termina. Si continua la curva sera borrada.\n¿Desea Continuar?", "Curva", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result2 != DialogResult.No)
                {
                    nclicks = 0;
                    PutScreen();
                    glControl1.SwapBuffers();
                }
                else
                    Herramienta = 5;
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Herramienta = 9;
            GL.PointSize(1f);
            trackBar1.Enabled = false;
            GL.Color3(colorPrincipal[0], colorPrincipal[1], colorPrincipal[2]);
            if (nclicks != 0)
            {
                DialogResult result2 = MessageBox.Show("El trazado de la curva aún no termina. Si continua la curva sera borrada.\n¿Desea Continuar?", "Curva", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result2 != DialogResult.No)
                {
                    nclicks = 0;
                    PutScreen();
                    glControl1.SwapBuffers();
                }
                else
                    Herramienta = 5;
            }
        }

        private void Spray(object sender, EventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            PutScreen();
            pixel(xini + random.Next(-10 * pincel, 11 * pincel), yini + random.Next(-10 * pincel, 11 * pincel));
            pixel(xini + random.Next(-10 * pincel, 11 * pincel), yini + random.Next(-10 * pincel, 11 * pincel));
            //pixel(xini + random.Next(-20, 21), yini + random.Next(-20, 21));
            GrabScreen();
            glControl1.SwapBuffers();
        }

        private void button11_Click(object sender, EventArgs e)//Boton Guardar
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                String name = saveFileDialog1.FileName;
                saveBMP(name);

            }
        }

        private void saveBMP(String file)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            PutScreen();
            glControl1.SwapBuffers();
            GrabScreen();
            Bitmap bmp = new Bitmap(glControl1.Width, glControl1.Height);
            System.Drawing.Imaging.BitmapData data = bmp.LockBits(glControl1.ClientRectangle, System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            GL.ReadPixels(0, 0, glControl1.Width, glControl1.Height, PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
            bmp.UnlockBits(data);

            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            bmp.Save(file);
        }

        private void openBMP(String file)
        {
            try
            {
                Bitmap bmp = new Bitmap(file);//se abre el archivo de imagen
                bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
                System.Drawing.Imaging.BitmapData data = bmp.LockBits(glControl1.ClientRectangle, System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                GL.DrawPixels(glControl1.Width, glControl1.Height, PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
                GrabScreen();
                glControl1.SwapBuffers();
                PutScreen();
                glControl1.SwapBuffers();
                bmp.UnlockBits(data);
                
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("Tamaño de imágen no valido para la visualización en pantalla");
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                String name = openFileDialog1.FileName;
                openBMP(name);
                

            }
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            if (Herramienta != 9 && Herramienta != 8)
                GL.PointSize((float) trackBar2.Value);
            label7.Text = "Tamaño del Pincel: "+trackBar2.Value;
            pincel = trackBar2.Value;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            
            DialogResult boton = MessageBox.Show("Esta operación borrara todo lo que este dibujado.\n¿Desea continuar?", "Nuevo", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (boton == DialogResult.OK)
            {
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                GrabScreen();
                glControl1.SwapBuffers();
            }
            
        }


        private void button13_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                panel23.BackColor = colorDialog1.Color;
                colorPrincipal[0] = colorDialog1.Color.R;
                colorPrincipal[1] = colorDialog1.Color.G;
                colorPrincipal[2] = colorDialog1.Color.B;

            }
            if (Herramienta != 7)
                GL.Color3(colorPrincipal[0], colorPrincipal[1], colorPrincipal[2]);
            else
                GL.Color3(colorSecundario[0], colorSecundario[1], colorSecundario[2]);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                panel24.BackColor = colorDialog1.Color;
                colorSecundario[0] = colorDialog1.Color.R;
                colorSecundario[1] = colorDialog1.Color.G;
                colorSecundario[2] = colorDialog1.Color.B;
            }
            if (Herramienta != 7)
                GL.Color3(colorPrincipal[0], colorPrincipal[1], colorPrincipal[2]);
            else
                GL.Color3(colorSecundario[0], colorSecundario[1], colorSecundario[2]);
        }


    }
}
