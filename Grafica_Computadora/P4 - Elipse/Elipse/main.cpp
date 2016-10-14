#include <windows.h>
#include <GL/GLU.h>
#include <GL/glut.h>
#include <math.h>
#include <stdio.h>
#include <stdlib.h>

int colorArray[400][400];
float r=0;
int xc=0,yc=0,xa=0,ya=0;
int rx,ry;

void pixel(int x,int y){
	glBegin(GL_POINTS);
	glVertex2i(x,y);
	glEnd();
}

void elipse(int xc,int yc, int rx, int ry){
    float q=(2*M_PI)/360;
    for(float i=0;i<2*M_PI;i+=q){
        pixel(xc+(rx*cos(i)),yc+(ry*sin(i)));
        pixel(xc+(-rx*cos(i)),yc+(ry*sin(i)));
        pixel(xc+(rx*cos(i)),yc+(-ry*sin(i)));
        pixel(xc+(-rx*cos(i)),yc+(-ry*sin(i)));
    }
}

void puntos(int xc,int yc, int x, int y){
    pixel(xc+x,yc+y);
    pixel(xc-x,yc+y);
    pixel(xc+x,yc-y);
    pixel(xc-x,yc-y);
}

void elipseCerrada(int xc, int yc, int rx, int ry){
    float p, px, py, x, y, ry2, rx2, tworx2, twory2;
    ry2=ry*ry;
    rx2=rx*rx;
    twory2=2*ry2;
    tworx2=2*rx2;
    //region 1
    x=0;
    y=ry;
    puntos(xc,yc,x,y);
    p= ceil(ry2-rx2*ry+(0.25*rx2));
    px=0;
    py=tworx2*y;
    while(px<py){
        x++;
        px+=twory2;
        if(p>=0){
            y--;
            py-=tworx2;
        }
        if(p<0){
            p+=ry2+px;
        }
        else{
            p+=ry2+px-py;
        }
        puntos(xc,yc,x,y);
    }
    //region 2
    p=ceil(ry2*(x+0.5)*(x+0.5)+rx2*(y-1)*(y-1)-rx2*ry2);
    while(y>0){
        y--;
        py-=tworx2;
        if(p<=0){
            x++;
            px+=twory2;
        }
        if(p>0){
            p+=rx2-py;
        }
        else{
            p+=rx2-py+px;
        }
        puntos(xc,yc,x,y);
    }
}

void init(void){
    glClearColor(0.0, 0.0, 0.0, 0.0); // se establece el color de la ventana de visualización
    glMatrixMode(GL_PROJECTION); //proyección ortogonal en una zona rectangular bidimensional
    gluOrtho2D(0.0, 400.0, 0.0, 400.0); //Sistema de coordenas de referencia de 0 a 200 para x, 0 a 150 para y
 }

void lineSegment(void){
    glClear(GL_COLOR_BUFFER_BIT); // visualización del color asignado a la ventana
    glColor3f(0.0, 1.0, 0.0); // color de lalínea
	glClear(GL_COLOR_BUFFER_BIT| GL_DEPTH_BUFFER_BIT);
	glDrawPixels(400,400,GL_RGB,GL_UNSIGNED_BYTE,colorArray);
    elipse(xc,yc,rx,ry);
	glFlush();
	glutSwapBuffers();
 }

void onMouse(int button, int state, int x, int y) {
    if ( (button==GLUT_LEFT_BUTTON) & (state==GLUT_DOWN) ) {
            xc=x;
            yc=abs(400-y);
    }
 }

 void onMotion(int x, int y) {
    rx=abs(xc-x);
    ry=abs(yc-abs(400-y));
    glutPostRedisplay();
}

void onPassive(int x,int y){
    glColor3f(1.0, 1.0, 0.0);
	glDrawPixels(400,400,GL_RGB,GL_UNSIGNED_BYTE,colorArray);
	elipseCerrada(xc,yc,rx,ry);
	glReadPixels( 0, 0,400,400,GL_RGB,GL_UNSIGNED_BYTE,colorArray);
	glColor3f(0.0, 1.0, 0.0);
	glutSwapBuffers();
}

int main(int argc, char** argv){
    glutInit(&argc, argv); //inicialización de GLUT
    glutInitDisplayMode(GLUT_DOUBLE|GLUT_RGB|GLUT_DEPTH);// único búfer de refresco en la ventana de visualización y el modo de color RGB
	glutInitWindowPosition(50, 100); //posición inicial de la ventana, esquina superior izquierda
    glutInitWindowSize(400, 400); //alto y ancho en pixeles
    glutCreateWindow("Elipses con el Mouse"); //creación de ventana de visualización y asigna el título
    init();
    glutDisplayFunc(lineSegment); //muestra la línea en la ventana de visualización
    glutMouseFunc(onMouse);
    glutMotionFunc(onMotion);
    glutPassiveMotionFunc(onPassive);
    glutMainLoop(); // bucle infinito que comprueba entrada de dispositivos
    return EXIT_SUCCESS;
 }
