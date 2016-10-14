#include <windows.h>
#include <GL/GLU.h>
#include <GL/glut.h>
#include <math.h>
#include <stdio.h>
#include <stdlib.h>

struct punto
{
    int x,y;
};

struct color{
unsigned char red,green,blue;
};

int cont=0;

color color_fondo={0,0,0};
color color_actual;
color color_inunda={255,0,0};
color verde={0,255,0};
punto ini;

void pixel(int x,int y,color a){
	glColor3ub((int)a.red,(int)a.green,(int)a.blue);
	glBegin(GL_POINTS);
	glVertex2i(x,y);
	glEnd();
}

/*                Estructura de los nodos de la cola
------------------------------------------------------------------------*/
struct nodo
	{
		punto P;
		struct nodo *sgte;
	};


/*                      Estructura de la cola
------------------------------------------------------------------------*/
struct cola
	{
		nodo *delante;
		nodo *atras  ;
	};


/*                        Encolar elemento
------------------------------------------------------------------------*/
void encolar( struct cola &q, int x,int y )
{
	struct nodo *aux = new(struct nodo);

	aux->P.x = x;
	aux->P.y = y;
	aux->sgte = NULL;

	if( q.delante == NULL)
		q.delante = aux;   // encola el primero elemento
	else
		(q.atras)->sgte = aux;

	q.atras = aux;        // puntero que siempre apunta al ultimo elemento

}

/*                        Desencolar elemento
------------------------------------------------------------------------*/
punto desencolar( struct cola &q )
{
	punto PA ;
	struct nodo *aux ;

	aux = q.delante;      // aux apunta al inicio de la cola
	PA.x = aux->P.x;
	PA.y = aux->P.y;
	q.delante = (q.delante)->sgte;
	delete(aux);          // libera memoria a donde apuntaba aux

	return PA;
}


/*              Eliminar todos los elementos de la Cola
------------------------------------------------------------------------*/
void vaciaCola( struct cola &q)
{
	struct nodo *aux;

	while( q.delante != NULL)
	{
		aux = q.delante;
		q.delante = aux->sgte;
		delete(aux);
	}
	q.delante = NULL;
	q.atras   = NULL;

}

int vacia(struct cola &q){
	return (q.delante==NULL);
}







bool compara(color colorA,color colorF){
return ((int)colorA.red==(int)colorF.red&&(int)colorA.green==(int)colorF.green&&(int)colorA.blue==(int)colorF.blue);
}

void inunda(int x, int y){
 
 //---->Cola
 struct cola q;
 q.delante = NULL;
 q.atras   = NULL;
 punto Aux;
 glReadPixels(Aux.x,Aux.y,1,1,GL_RGB,GL_UNSIGNED_BYTE,&color_actual);
 if(compara(color_actual,color_fondo))
	 encolar(q,x,y);
 else
	 return;
 while(!vacia(q)){
	 Aux=desencolar(q);
	 pixel(Aux.x,Aux.y,color_inunda);
	 //printf("x: %d, y: %d\n",Aux.x,Aux.y);
	 glReadPixels(Aux.x+1,Aux.y,1,1,GL_RGB,GL_UNSIGNED_BYTE,&color_actual);
	 if(compara(color_actual,color_fondo))
		 encolar(q,Aux.x+1,Aux.y);
	 glReadPixels(Aux.x-1,Aux.y,1,1,GL_RGB,GL_UNSIGNED_BYTE,&color_actual);
	 if(compara(color_actual,color_fondo))
		 encolar(q,Aux.x-1,Aux.y);
	 glReadPixels(Aux.x,Aux.y+1,1,1,GL_RGB,GL_UNSIGNED_BYTE,&color_actual);
	 if(compara(color_actual,color_fondo))
		 encolar(q,Aux.x,Aux.y+1);
	 glReadPixels(Aux.x,Aux.y-1,1,1,GL_RGB,GL_UNSIGNED_BYTE,&color_actual);
	 if(compara(color_actual,color_fondo))
		 encolar(q,Aux.x,Aux.y-1);
	 cont++;
	 if(cont==2500){
		 glFlush();
		 cont=0;
	 }
 }
 //---->Cola
 

 /*//---->Recursivo
	glReadPixels(x,y,1,1,GL_RGB,GL_UNSIGNED_BYTE,&color_actual);
 if(compara(color_actual,color_fondo) && !compara(color_actual,color_inunda)&&x<=400&&y<=400){
	 //printf("x: %d, y: %d\n",x,y);
    pixel(x,y,color_inunda);
    inunda(x+1,y);
    inunda(x-1,y);
    inunda(x,y+1);
    inunda(x,y-1);
 }

 cont++;
 if(cont==20){
	 glFlush();
	 cont=0;
 }

 //--> Recursivo
*/
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
	pixel(x,y,verde);
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
			pixel(x,y,verde);
			Fin--;
		}
	}
	else{
		Fin=dy;
		p=2*(dx-dy);
		pixel(x,y,verde);
		while(Fin>0){
			y=y+incY;
			if(p<0)
				p=p+2*dx;
			else{
				x=x+incX;
				p=p+2*(dx-dy);
			}
			pixel(x,y,verde);
			Fin--;
		}
	}
}//LineaBres

void lineSegment(void){
   // glClear(GL_COLOR_BUFFER_BIT); // visualización del color asignado a la ventana
    glColor3f(0.0, 1.0, 0.0); // color de lalínea
	//glClear(GL_COLOR_BUFFER_BIT| GL_DEPTH_BUFFER_BIT);
	glFlush();
	glutSwapBuffers();
 }

 void init(void){
    glClearColor(0.0, 0.0, 0.0, 0.0); // se establece el color de la ventana de visualización
    glMatrixMode(GL_PROJECTION); //proyección ortogonal en una zona rectangular bidimensional
    gluOrtho2D(0.0, 400.0, 0.0, 400.0); //Sistema de coordenas de referencia de 0 a 200 para x, 0 a 150 para y
 }

void onMouse(int button, int state, int x, int y) {
    if ( (button == GLUT_LEFT_BUTTON) && (state == GLUT_DOWN)) {
		ini.x=x;
		ini.y=abs(400-y);
		pixel(ini.x,ini.y,verde);
    }
    if ( (button == GLUT_RIGHT_BUTTON) && (state == GLUT_DOWN)) {
        inunda(x,abs(400-y));
		//printf("x: %d, y: %d\n",x,abs(400-y));
    }
    glFlush();
    glutSwapBuffers();
 }

void onMotion(int x, int y) {
	LineaBres(ini.x,ini.y,x,abs(400-y));
	ini.x=x;
	ini.y=abs(400-y);
    glFlush();
    glutSwapBuffers();
	}

int main(int argc, char** argv){
    glutInit(&argc, argv); //inicialización de GLUT
    glutInitDisplayMode(GLUT_SINGLE|GLUT_RGB|GLUT_DEPTH);// único búfer de refresco en la ventana de visualización y el modo de color RGB
	glutInitWindowPosition(50, 100); //posición inicial de la ventana, esquina superior izquierda
    glutInitWindowSize(400, 400); //alto y ancho en pixeles
    glutCreateWindow("Relleno"); //creación de ventana de visualización y asigna el título
    init();
    glutDisplayFunc(lineSegment); //muestra la línea en la ventana de visualización
    glutMouseFunc(onMouse);
    glutMotionFunc(onMotion);
    glutMainLoop(); // bucle infinito que comprueba entrada de dispositivos
    return EXIT_SUCCESS;
 }
