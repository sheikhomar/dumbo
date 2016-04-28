/********************************************************
Function:	Throw									
Version: 	v1.0	
Uses:				
/********************************************************/
#include <stdio.h>
#include <stdlib.h>
#include <time.h>
#include <math.h>

void throw(char* message);
void modTestHelper(double para1, double para2, double res);

int main()
{
	throw("Hello World");
	printf("End of program!");
	
	return 0;
}

void throw(char* message){
	printf("Program ended unexpectedly:\r\n%s\r\n",message);
	exit(1);
}
