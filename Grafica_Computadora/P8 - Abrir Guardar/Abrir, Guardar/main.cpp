//#include <windows.h>
//#include <GL/GLU.h>
#include <GL/glut.h>
#include <math.h>
#include <stdio.h>
#include <stdlib.h>

#define ALTO 400
#define ANCHO 400

int colorArray[400][400];
int band=0;//0-> lapiz, 1-> borrador, 2->Sprite
int activo=0; //estado del spray
struct punto
{
    int x,y;
};
punto ini;
punto fin;

void pixel(int x,int y)
{
	if(band==0||band==2){//lapiz o spray
		glBegin(GL_POINTS);
		glVertex2i(x,y);
		glEnd();
}
	else{//borrador
			for(int i=-3; i<3;i++)
			  for(int j=-3; j<3;j++){
					glBegin(GL_POINTS);
					glVertex2i(i+x,j+y);
					glEnd();
				}
	}
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

void spray(int value)
{
	int temp,r1,r2;
	for(temp=0;temp<10;temp++){
		r1= (rand() % 40)-20;
		r2= (rand() % 40)-20;
		pixel(ini.x+r1,ini.y+r2);
	}
	if(activo==1)
		glutTimerFunc(40,spray,0);
    glFlush();
	glutSwapBuffers();
}

void onMouse(int button, int state, int x, int y) {
    if ( (button == GLUT_LEFT_BUTTON) && (state == GLUT_DOWN) && band!=2 ) {
		ini.x=x;
		ini.y=abs(ALTO-y);
		pixel(ini.x,ini.y);
    }
	if ( (button == GLUT_LEFT_BUTTON) && (state == GLUT_DOWN) && band==2 ) {
		activo=1;
		ini.x=x;
		ini.y=abs(ALTO-y);
		spray(0);
	}

	if ( (button == GLUT_LEFT_BUTTON) && (state == GLUT_UP) && band==2 ) {
		activo=0;
	}
	glFlush();
	glutSwapBuffers();
 }

 void onMotion(int x, int y) {
	if(band!=2){//borrador o lapiz
		if(x==ini.x&&y==ini.y)//si solo es un punto
			pixel(x,y);
		else{
		LineaBres(ini.x,ini.y,x,abs(ALTO-y));
		ini.x=x;
		ini.y=abs(ALTO-y);
		}
	}
	else{//spray
		ini.x=x;
		ini.y=abs(ALTO-y);
	}
	glFlush();
	glutSwapBuffers();

}

 void lineSegment(void){
	glColor3f(0.0, 1.0, 0.0); // color de lalínea
	glFlush();
	glutSwapBuffers();
 }

 void init(void){
    glClearColor(0.0, 0.0, 0.0, 0.0); // se establece el color de la ventana de visualización
    glMatrixMode(GL_PROJECTION); //proyección ortogonal en una zona rectangular bidimensional
    gluOrtho2D(0.0, ANCHO, 0.0, ALTO); //Sistema de coordenas de referencia de 0 a 200 para x, 0 a 150 para y
 }

 void Guardar(void){

    BITMAPFILEHEADER fhead;
    //FILE HEADER
    fhead.bfType = 0x4D42; // BMP File signature -> 'BM'
    fhead.bfSize = sizeof(BITMAPFILEHEADER) + sizeof(BITMAPINFOHEADER) + sizeof(colorArray); // tamaño archivo
    fhead.bfReserved1 = 0; // reservado
    fhead.bfReserved2 = 0; // reservado
    fhead.bfOffBits = sizeof(BITMAPFILEHEADER) + sizeof(BITMAPINFOHEADER);

    BITMAPINFOHEADER ihead;
    //INFO HEADER
    ihead.biSize = sizeof(BITMAPINFOHEADER); // bytes del FILE HEADER
    ihead.biWidth = ANCHO;
    ihead.biHeight = ALTO;
    ihead.biPlanes = 1; // Numero de planos
    ihead.biBitCount = 24; // bits por píxel
    ihead.biCompression = BI_RGB; // compresion
    ihead.biSizeImage = sizeof(colorArray); // tamaño de la imagen
    ihead.biXPelsPerMeter = 2835;
    ihead.biYPelsPerMeter = 2835;
    ihead.biClrUsed = 0; // Colores de la paleta
    ihead.biClrImportant = 0; // 0 para indicar todos los colores importantes

    // ESCRITURA
    FILE* out = fopen("C:\\imagen.bmp", "w");
    fwrite(&fhead, sizeof(BITMAPFILEHEADER), 1, out); // write the BMFH into bitmap image first
    fwrite(&ihead, sizeof(BITMAPINFOHEADER), 1, out); // write the BMIH into bitmap image second
    glReadPixels(0, 0, ANCHO, ALTO, 0x80E0, GL_UNSIGNED_BYTE, colorArray); // HEX: 0x80E0 == GL_BGR. read pixels to organize by BGR format
    fwrite(&colorArray, sizeof(colorArray), 1, out);  // write the bitmap data into bitmap image third
    fclose(out); // close file
    glReadPixels(0, 0, ANCHO, ALTO, GL_RGB, GL_UNSIGNED_BYTE, colorArray); // read pixels to organize by RGB format
}

void Cargar(void){
    FILE* in;
    if(!(in = fopen("C:\\imagen.bmp", "r")))
        return;
    fseek(in, sizeof(BITMAPFILEHEADER) + sizeof(BITMAPINFOHEADER), SEEK_SET);
    fread(&colorArray, sizeof(colorArray), 1, in);
    glDrawPixels(ANCHO, ALTO, 0x80E0, GL_UNSIGNED_BYTE, colorArray); // draw bitmap pixels onto screen in BGR format
    glFlush();
}

void keyboard(unsigned char key, int x, int y){
	switch(key){
	case 76: //L o l
	case 108: band=0;//lapiz
		break;
	case 66: //B o b
	case 98: band=1;//borrador
		break;
	case 83: //S o s
	case 115: band=2;//sprite
        break;
	case 67: //C o c
	case 99: Cargar();//Cargar imagen
	    break;
    case 71: //G o g
    case 103: Guardar();//Guardar imagen
        break;

	}
}




int main(int argc, char** argv)
{
    glutInit(&argc, argv); //inicialización de GLUT
    glutInitDisplayMode(GLUT_SINGLE|GLUT_RGB|GLUT_DEPTH);// único búfer de refresco en la ventana de visualización y el modo de color RGB
    glutInitWindowPosition(50, 100); //posición inicial de la ventana, esquina superior izquierda
    glutInitWindowSize(ALTO, ANCHO); //alto y ancho en pixeles
    glutCreateWindow("Lapiz, Borrador, Spray"); //creación de ventana de visualización y asigna el título
    init();
    glutDisplayFunc(lineSegment); //muestra la línea en la ventana de visualización
    glutMouseFunc(onMouse);
    glutMotionFunc(onMotion);
	glutKeyboardFunc(keyboard);
    glutMainLoop(); // bucle infinito que comprueba entrada de dispositivos
    return EXIT_SUCCESS;
}
