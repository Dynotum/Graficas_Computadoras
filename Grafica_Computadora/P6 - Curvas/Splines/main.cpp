#include <windows.h>
#include <GL/GLU.h>
#include <GL/glut.h>
#include <math.h>
#include <stdio.h>
#include <stdlib.h>

int colorArray[400][400];
int X0,Y0,X1,Y1,X2,Y2,X3,Y3;
int band=0;

struct punto
{
    int x,y;
};

punto punto;

void pixel(int x,int y)
{
    glBegin(GL_POINTS);
    glVertex2i(x,y);
    glEnd();
}

void curva (int x0, int y0,int x1, int y1,int x2, int y2,int x3, int y3)
{
    double time;
    int x,y;
    for (time=0; time<=1.0; time+=0.001)
    {
        x=pow(1-time,3)*x0+
          3*time*pow(1-time,2)*x1+
          3*pow(time,2)*(1-time)*x2+
          pow(time,3)*x3;
        y=pow(1-time,3)*y0+
          3*time*pow(1-time,2)*y1+
          3*pow(time,2)*(1-time)*y2+
          pow(time,3)*y3;
        pixel(x,y);
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
    //glPointSize(PZ);
    glClear(GL_COLOR_BUFFER_BIT| GL_DEPTH_BUFFER_BIT);
    glDrawPixels(400,400,GL_RGB,GL_UNSIGNED_BYTE,colorArray);
    curva(X0,Y0,X1,Y1,X2,Y2,X3,Y3);
    glFlush();
    glutSwapBuffers();
}

void onMouse(int button, int state, int x, int y)
{
    if ( (button==GLUT_LEFT_BUTTON) & (state==GLUT_DOWN))
    {
        switch(band)
        {
        case 0:
            X0=x;
            Y0=abs(400-y);
            break;
        case 1:
            X1=x;
            Y1=abs(400-y);
            break;
        case 2:
            X2=x;
            Y2=abs(400-y);
            break;
        }


    }
    if ( (button==GLUT_LEFT_BUTTON) & (state==GLUT_UP))
    {
        if(band!=2)
            band++;
        else
        {
            band=0;
			glutSwapBuffers();
            glReadPixels( 0, 0,400,400,GL_RGB,GL_UNSIGNED_BYTE,colorArray);
        }
    }



    glutSwapBuffers();
}

void onMotion(int x, int y)
{
    switch(band)
    {
    case 0:
        X1=X2=X3=x;
        Y1=Y2=Y3=abs(400-y);
        break;
    case 1:
        X1=X2=x;
        Y1=Y2=abs(400-y);
        break;
    case 2:
        X2=x;
        Y2=abs(400-y);
        break;
    }

    glutPostRedisplay();
}

int main(int argc, char** argv)
{
    glutInit(&argc, argv); //inicialización de GLUT
    glutInitDisplayMode(GLUT_DOUBLE|GLUT_RGB|GLUT_DEPTH);// único búfer de refresco en la ventana de visualización y el modo de color RGB
    glutInitWindowPosition(50, 100); //posición inicial de la ventana, esquina superior izquierda
    glutInitWindowSize(400, 400); //alto y ancho en pixeles
    glutCreateWindow("Spline"); //creación de ventana de visualización y asigna el título
    init();
    glutDisplayFunc(lineSegment); //muestra la línea en la ventana de visualización
    glutMouseFunc(onMouse);
    glutMotionFunc(onMotion);
    glutMainLoop(); // bucle infinito que comprueba entrada de dispositivos
    return EXIT_SUCCESS;
}
