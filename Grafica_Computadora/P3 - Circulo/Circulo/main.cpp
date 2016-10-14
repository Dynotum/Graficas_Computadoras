#include <windows.h>
#include <GL/GLU.h>
#include <GL/glut.h>
#include <math.h>
#include <stdio.h>
#include <stdlib.h>

int colorArray[400][400];
float r=0;
int xc=0,yc=0,xa=0,ya=0;
int dx,dy;

struct punto
{
    int x,y;
} punto;

void pixel(int x,int y)
{
    glBegin(GL_POINTS);
    glVertex2i(x,y);
    glEnd();
}

void polar(int xc,int yc,int r, float a)
{
    float radian;
    radian=180/M_PI;
    punto.x=xc+ceil(r*cos(a/radian));
    punto.y=yc+ceil(r*sin(a/radian));
}
void circulo(int x,int y, int r)
{
    int i;
    for(i=0; i<360; i++)
    {
        polar(x,y,r,i);
        pixel(punto.x,punto.y);
    }
}

void puntos(int xc,int yc, int x, int y)
{
    pixel(xc+x,yc+y);
    pixel(xc-x,yc+y);
    pixel(xc+x,yc-y);
    pixel(xc-x,yc-y);

    pixel(xc+y,yc+x);
    pixel(xc-y,yc+x);
    pixel(xc+y,yc-x);
    pixel(xc-y,yc-x);
}

void circulo2(int xc,int yc, int radio)
{
    int x,y,p;
    x=0;
    y=radio;
    puntos(xc,yc,x,y);
    p=1-radio;
    while(x<y)
    {
        if(p<0)
        {
            x++;
            p+=2*x+1;
        }
        else
        {
            x++;
            y--;
            p+=2*(x-y)+1;

        }
        puntos(xc,yc,x,y);
    }

}


void init(void)
{
    glClearColor(0.0, 0.0, 0.0, 0.0); // se establece el color de la ventana de visualización
    glMatrixMode(GL_PROJECTION); //proyección ortogonal en una zona rectangular bidimensional
    gluOrtho2D(0.0, 400.0, 0.0, 400.0); //Sistema de coordenas de referencia de 0 a 200 para x, 0 a 150 para y
}

void lineSegment(void)
{
    glClear(GL_COLOR_BUFFER_BIT); // visualización del color asignado a la ventana
    glColor3f(0.0, 1.0, 0.0); // color de lalínea
    glClear(GL_COLOR_BUFFER_BIT| GL_DEPTH_BUFFER_BIT);
    glDrawPixels(400,400,GL_RGB,GL_UNSIGNED_BYTE,colorArray);
    circulo(xc,yc,r);
    glFlush();
    glutSwapBuffers();
}

void onMouse(int button, int state, int x, int y)
{
    if ( (button==GLUT_LEFT_BUTTON) & (state==GLUT_DOWN) )
    {
        xc=x;
        yc=abs(400-y);
    }
}

void onMotion(int x, int y)
{
    dx=abs(xc-x);
    dy=abs(yc-abs(400-y));
    r=sqrt(pow(dx,2)+pow(dy,2));
    glutPostRedisplay();
}

void onPassive(int x,int y)
{
    glColor3f(1.0, 1.0, 1.0);
    glDrawPixels(400,400,GL_RGB,GL_UNSIGNED_BYTE,colorArray);
    circulo2(xc,yc,r);
    glReadPixels( 0, 0,400,400,GL_RGB,GL_UNSIGNED_BYTE,colorArray);
    glColor3f(0.0, 1.0, 0.0);
    glutSwapBuffers();
}

int main(int argc, char** argv)
{
    glutInit(&argc, argv); //inicialización de GLUT
    glutInitDisplayMode(GLUT_DOUBLE|GLUT_RGB|GLUT_DEPTH);// único búfer de refresco en la ventana de visualización y el modo de color RGB
    glutInitWindowPosition(50, 100); //posición inicial de la ventana, esquina superior izquierda
    glutInitWindowSize(400, 400); //alto y ancho en pixeles
    glutCreateWindow("Circulos con el Mouse"); //creación de ventana de visualización y asigna el título
    init();
    glutDisplayFunc(lineSegment); //muestra la línea en la ventana de visualización
    glutMouseFunc(onMouse);
    glutMotionFunc(onMotion);
    glutPassiveMotionFunc(onPassive);
    glutMainLoop(); // bucle infinito que comprueba entrada de dispositivos
    return EXIT_SUCCESS;
}
