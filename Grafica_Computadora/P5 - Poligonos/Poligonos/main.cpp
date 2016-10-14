#include <windows.h>
#include <GL/GLU.h>
#include <GL/glut.h>
#include <math.h>
#include <stdio.h>
#include <stdlib.h>

int colorArray[400][400];
float r=0,a;
int xc=0,yc=0,xa=0,ya=0,lados=3;
float PZ=1;
int band=1;

struct punto
{
    int x,y;
};

punto puntos[10];
punto punto;

void pixel(int x,int y)
{
    glBegin(GL_POINTS);
    glVertex2i(x,y);
    glEnd();
}

void LineaBres(int xa, int ya, int xb, int yb)
{
    int dx, dy,x,y,Fin,p,incX,incY;
    dx=abs(xa-xb);
    dy=abs(ya-yb);
    if(xa<xb)
    {
        incX=1;
    }
    if(xa>xb)
    {
        incX=-1;
    }
    if(xa==xb)
    {
        incX=0;
    }
    if(ya<yb)
    {
        incY=1;
    }
    if(ya>yb)
    {
        incY=-1;
    }
    if(ya==yb)
    {
        incY=0;
    }
    x=xa;
    y=ya;
    pixel(x,y);
    if(dx>dy)
    {
        Fin=dx;
        p=2*(dy-dx);
        while(Fin>0)
        {
            x=x+incX;
            if(p<0)
                p=p+2*dy;
            else
            {
                y=y+incY;
                p=p+2*(dy-dx);
            }
            if(band==1&&x%4==0)
                pixel(x,y);
            else if(band==0)
                pixel(x,y);
            Fin--;
        }
    }
    else
    {
        Fin=dy;
        p=2*(dx-dy);
        pixel(x,y);
        while(Fin>0)
        {
            y=y+incY;
            if(p<0)
                p=p+2*dx;
            else
            {
                x=x+incX;
                p=p+2*(dx-dy);
            }
            if(band==1&&x%4==0)
            pixel(x,y);
            else if(band==0)
                pixel(x,y);
            Fin--;
        }
    }
}//LineaBres


void polar(int xc,int yc,int r, float a)
{
    float radian;
    radian=180/M_PI;
    punto.x=xc+ceil(r*cos(a/radian));
    punto.y=yc+ceil(r*sin(a/radian));
}

void solidos(int xc,int yc,int radio, float a, int lados)
{
    int ang=360/lados;
    int l;
    for(l=0; l<lados; l++)
    {
        polar(xc,yc,radio,a);
        a+=ang;
        puntos[l]=punto;
    }
    for(l=0; l<lados-1; l++)
        LineaBres(puntos[l].x,puntos[l].y,puntos[l+1].x,puntos[l+1].y);
    LineaBres(puntos[l].x,puntos[l].y,puntos[0].x,puntos[0].y);

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
    glPointSize(PZ);
    glClear(GL_COLOR_BUFFER_BIT| GL_DEPTH_BUFFER_BIT);
    glDrawPixels(400,400,GL_RGB,GL_UNSIGNED_BYTE,colorArray);
        band=1;
    solidos(xc,yc,r,a,lados);
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
    if ( (button==GLUT_LEFT_BUTTON) & (state==GLUT_UP) ){
        glColor3f(0.0, 1.0, 0.8);
        glDrawPixels(400,400,GL_RGB,GL_UNSIGNED_BYTE,colorArray);
        band=0;
        solidos(xc,yc,r,a,lados);
        glReadPixels( 0, 0,400,400,GL_RGB,GL_UNSIGNED_BYTE,colorArray);
        glColor3f(0.0, 1.0, 0.0);
        glutSwapBuffers();
    }
}

void onMotion(int x, int y)
{
    int dX,dY;
    dX=xc-x;
    dY=yc-abs(400-y);
    r=sqrt(pow(dX,2)+pow(dY,2));
    a=(180/M_PI)*(atan2(yc-abs(400-y),(xc-x)));
    glutPostRedisplay();
}

/*
void onPassive(int x,int y)
{
    glColor3f(1.0, 1.0, 0.0);
    glDrawPixels(400,400,GL_RGB,GL_UNSIGNED_BYTE,colorArray);
    solidos(xc,yc,r,a,lados);
    glReadPixels( 0, 0,400,400,GL_RGB,GL_UNSIGNED_BYTE,colorArray);
    glColor3f(0.0, 1.0, 0.0);
    glutSwapBuffers();
}
*/

void ArrowKey(int key, int x, int y)
{
    switch (key)
    {
    case GLUT_KEY_RIGHT:
        if(lados <10&&band!=0){
            lados+=1;
        glutPostRedisplay();
        solidos(xc,yc,r,a,lados);
        }
        break;
    case GLUT_KEY_LEFT:
        if(lados >3&&band!=0){
            lados-=1;
        glutPostRedisplay();
        solidos(xc,yc,r,a,lados);
        }
        break;
    case GLUT_KEY_UP:
        if(PZ <10&&band!=0){
        PZ += 0.5;
        glutPostRedisplay();
        solidos(xc,yc,r,a,lados);
        }
    break;
    case GLUT_KEY_DOWN:
        if(PZ >0.5&&band!=0){
        PZ -= 0.5;
        glutPostRedisplay();
        solidos(xc,yc,r,a,lados);
        }
    break;
    }
}


int main(int argc, char** argv)
{
    glutInit(&argc, argv); //inicialización de GLUT
    glutInitDisplayMode(GLUT_DOUBLE|GLUT_RGB|GLUT_DEPTH);// único búfer de refresco en la ventana de visualización y el modo de color RGB
    glutInitWindowPosition(50, 100); //posición inicial de la ventana, esquina superior izquierda
    glutInitWindowSize(400, 400); //alto y ancho en pixeles
    glutCreateWindow("Poligonos"); //creación de ventana de visualización y asigna el título
    init();
    glutDisplayFunc(lineSegment); //muestra la línea en la ventana de visualización
    glutMouseFunc(onMouse);
    glutMotionFunc(onMotion);
//    glutPassiveMotionFunc(onPassive);
    glutSpecialFunc(ArrowKey);
    glutMainLoop(); // bucle infinito que comprueba entrada de dispositivos
    return EXIT_SUCCESS;
}



