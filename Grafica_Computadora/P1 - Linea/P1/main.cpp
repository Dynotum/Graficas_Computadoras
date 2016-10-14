#include <windows.h>
#include <GL/GLU.h>
#include <GL/glut.h>
#include <math.h>
#include <stdio.h>
#include <stdlib.h>

void init(void){
    glClearColor(0.0, 0.0, 0.0, 0.0); // se establece el color de la ventana de visualización
    glMatrixMode(GL_PROJECTION); //proyección ortogonal en una zona rectangular bidimensional
    gluOrtho2D(0.0, 200.0, 0.0, 150.0); //Sistema de coordenas de referencia de 0 a 200 para x, 0 a 150 para y
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
	//estrella
    LineaBres(100,20,90,65);
	LineaBres(90,65,35,75);
	LineaBres(35,75,90,85);
	LineaBres(90,85,100,130);
	LineaBres(100,130,110,85);
	LineaBres(110,85,165,75);
	LineaBres(165,75,110,65);
	LineaBres(110,65,100,20);
	//cruz
	LineaBres(100,20,100,130);
	LineaBres(35,75,165,75);
	//x central
	LineaBres(90,65,110,85);
	LineaBres(110,65,90,85);
    //marco
    LineaBres(10,10,10,140);
    LineaBres(10,140,190,140);
    LineaBres(190,140,190,10);
    LineaBres(190,10,10,10);
    glFlush(); // procesar función
 }

 int main(int argc, char** argv){
    glutInit(&argc, argv); //inicialización de GLUT
    glutInitDisplayMode(GLUT_SINGLE|GLUT_RGB);// único búfer de refresco en la ventana de visualización y el modo de color RGB
	glutInitWindowPosition(50, 100); //posición inicial de la ventana, esquina superior izquierda
    glutInitWindowSize(500, 400); //alto y ancho en pixeles
    glutCreateWindow("Estrella"); //creación de ventana de visualización y asigna el título
    init();
    glutDisplayFunc(lineSegment); //muestra la línea en la ventana de visualización
    glutMainLoop(); // bucle infinito que comprueba entrada de dispositivos
    return EXIT_SUCCESS;
 }
