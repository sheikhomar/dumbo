/********************************************************
Function:	Throw									
Version: 	v1.0	
Uses:				
/********************************************************/
#include <stdio.h>
#include <stdlib.h>
#include <time.h>
#include <math.h>

void Throw(char* message);
void modTestHelper(double para1, double para2, double res);

int main()
{
	Throw("Hello World");
	printf("End of program!");
	
	return 0;
}

void Throw(char* message){
	printf("Program ended unexpectedly:\r\n%s\r\n",message);
	exit(1);
}
