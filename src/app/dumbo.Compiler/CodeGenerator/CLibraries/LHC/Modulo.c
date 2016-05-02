/********************************************************
Function:	Modulo									
Version: 	v1.0
Uses:		Throw 							
/********************************************************/
#include <stdio.h>
#include <stdlib.h>
#include <time.h>
#include <math.h>

double Modulo(double n, double d);
void Throw(char* message);
void modTestHelper(double para1, double para2, double res);

int main()
{
	modTestHelper(15.0,5.0,0.0);
	modTestHelper(16,5,1);
	modTestHelper(19.99,5,4.99);
	modTestHelper(19.99,4.99,0.03);
	modTestHelper(-15,5,0);
	modTestHelper(15,-5,0);
	modTestHelper(-16,5,4);
	modTestHelper(16,-5,-4);
	modTestHelper(-5,-5,0);
	modTestHelper(-6,-5,-1);
	modTestHelper(-5,-6,-5);
	modTestHelper(2,10,2);
	//modTestHelper(1,0);
	modTestHelper(0,1,0);
	modTestHelper(1,1,0);
	//modTestHelper(0,0);
	
	printf("Finished testing\r\n");
	
	return 0;
}

void modTestHelper(double para1, double para2, double res){
	int diff = Modulo(para1,para2) - res;
	
	if(diff < 0.00001 && diff > -0.00001)
		return ;
	
	printf("ERROR: %f mod %f = %f should have been %f\r\n",para1,para2,Modulo(para1,para2),res);
}

double Modulo(double n, double d)
{	
	if(d == 0)
		Throw("Cannot do modulo when 2nd argument is zero.");
	
	return n - d*(floor((n/d)));
}

void Throw(char* message){
	printf("Program ended unexpectedly:\r\n%s\r\n",message);
	exit(1);
}