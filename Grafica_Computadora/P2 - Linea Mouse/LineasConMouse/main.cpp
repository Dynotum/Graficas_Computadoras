#include <windows.h>
#include <GL/GLU.h>
#include <GL/glut.h>
#include <math.h>
#include <stdio.h>
#include <stdlib.h>

int colorArray[200][200];
int xini=0,yini=0,xfin=0,yfin=0,lados;

void init(void){
    glClearColor(0.0, 0.0, 0.0, 0.0); // se establece el color de la ventana de visualización
    glMatrixMode(GL_PROJECTION); //proyección ortogonal en una zona rectangular bidimensional
    gluOrtho2D(0.0, 200.0, 0.0, 200.0); //Sistema de coordenas de referencia de 0 a 200 para x, 0 a 150 para y
 }

void pixel(int x,int y){
	glBegin(GL_POINTS);
	glVertex2i(x,y);
	glEnd();
}

 void LineaBres(int xa, int ya, int xb, int yb){
	int dx, dy,x,y,Fin,p,incX,incY;
	dx=abs(xa-xb);
	dy=abs(ya-yb);
	if(xa<xb){
		incX=1;
	}
	if(xa>xb){
		incX=-1;
	}
	if(xa==xb){
		incX=0;
	}
	if(ya<yb){
		incY=1;
	}
	if(ya>yb){
		incY=-1;
	}
	if(ya==yb){
		incY=0;
	}
	x=xa;
	y=ya;
	pixel(x,y);
	if(dx>dy){
		Fin=dx;
		p=2*(dy-dx);
		while(Fin>0){
			x=x+incX;
			if(p<0)
				p=p+2*dy;
			else{
				y=y+incY;
				p=p+2*(dy-dx);
			}
			pixel(x,y);
			Fin--;
		}
	}
	else{
		Fin=dy;
		p=2*(dx-dy);
		pixel(x,y);
		while(Fin>0){
			y=y+incY;
			if(p<0)
				p=p+2*dx;
			else{
				x=x+incX;
				p=p+2*(dx-dy);
			}
			pixel(x,y);
			Fin--;
		}
	}
}//LineaBres

 void lineSegment(void){

	glClear(GL_COLOR_BUFFER_BIT); // visualización del color asignado a la ventana
    glColor3f(0.0, 1.0, 0.0); // color de lalínea
	glClear(GL_COLOR_BUFFER_BIT| GL_DEPTH_BUFFER_BIT);
	glDrawPixels(200,200,GL_RGB,GL_UNSIGNED_BYTE,colorArray);
	LineaBres(xini,yini,xfin,yfin);
	glFlush();
	glutSwapBuffers();
 }

 void onMouse(int button, int state, int x, int y) {
    if ( (button == GLUT_LEFT_BUTTON) & (state == GLUT_DOWN) ) {
		xini=x;
		yini=abs(200-y);
    }
	if ( (button == GLUT_LEFT_BUTTON) & (state == GLUT_UP) ) {
		glReadPixels(0,0,200,200,GL_RGB,GL_UNSIGNED_BYTE,colorArray);
	}
 }

 void onMotion(int x, int y) {
	xfin=x;
	yfin=abs(200-y);
	glutPostRedisplay();
}

 int main(int argc, char** argv){
    glutInit(&argc, argv); //inicialización de GLUT
    glutInitDisplayMode(GLUT_DOUBLE|GLUT_RGB|GLUT_DEPTH);// único búfer de refresco en la ventana de visualización y el modo de color RGB
	glutInitWindowPosition(50, 100); //posición inicial de la ventana, esquina superior izquierda
    glutInitWindowSize(200, 200); //alto y ancho en pixeles
    glutCreateWindow("Lineas con el Mouse"); //creación de ventana de visualización y asigna el título
    init();
    glutDisplayFunc(lineSegment); //muestra la línea en la ventana de visualización
    glutMouseFunc(onMouse);
    glutMotionFunc(onMotion);
    glutMainLoop(); // bucle infinito que comprueba entrada de dispositivos
    return EXIT_SUCCESS;
 }
