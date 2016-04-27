/********************************************************
Function:	Random									
Version: 	v1.0
Uses:		Modulo, Throw 							
/********************************************************/
#include <stdio.h>
#include <stdlib.h>
#include <time.h>
#include <math.h>

//Extra helper functions
double modulo(double n, double d);
void throw(char* message);
void randomTestHelper(double r1, double r2);
void multipleRanTest(int repeat);

//Helper function
double random(double range_lower, double range_upper);


int main()
{
	srand(time(NULL));
	randomTestHelper(0,15);
	randomTestHelper(5,15);
	randomTestHelper(-5,17);
	randomTestHelper(5,-17);
	randomTestHelper(17,5);
	randomTestHelper(-17,0);
	//randomTestHelper(17,17);
	randomTestHelper(0,1);
	randomTestHelper(1,0);
	randomTestHelper(32767,0);
	randomTestHelper(32765,2);
	//randomTestHelper(32768,0);
	multipleRanTest(1000);
	
	printf("Finished testing\r\n");
	
	return 0;
}

void multipleRanTest(int repeat)
{
	int zero = 0, one = 0, two = 0, three = 0, four = 0, unclass = 0;
	int i;
	double numb;
	
	for(i=0;i<repeat;i++)
	{
		numb = random(0,4);
		
		switch((int)numb)
		{
			case 0 :
			zero++; break;
			case 1 :
			one++; break;
			case 2 : 
			two++; break;
			case 3 :
			three++; break;
			case 4 :
			four++; break;
			default :
			unclass++; break;
		}
	}
	printf("\r\nOver %d repeats we got the following in index 0-4\r\n",repeat);
	printf("zero %d, one %d, two %d, three %d, four %d | %d were unclassified\r\n",
	zero,one,two,three,four,unclass);
}


void randomTestHelper(double r1, double r2)
{
	double result = random(r1,r2);
	
	printf("In range %f - %f: we got %f\r\n",r1,r2,random(r1,r2));
}

double random(double range1, double range2)
{
	double number,range_lower,range_upper;
	double range;

	//Find upper and lower
	if(range1 > range2)
	{
		range_lower = range2;
		range_upper = range1;
	}
	else 
	{
		range_lower = range1;
		range_upper = range2;
	}
	
	range = range_upper - range_lower;
	
	//Error handle
	if(range < 1)
		throw("Random's range must use two DIFFERNT Numbers.");

	if(range > 32767)
		throw("Random's actual range must be less than 32768. The actual range is argument2 - argument1");

	
	//Calculate the random number and fit to range
	number = modulo(((double)rand()),(range+1)); // 1 % 2 =0,1 in a range 0..1
	
	//Return the random number, adding range offset from 0
	return number + range_lower;
}


//Modulo code copy
double modulo(double n, double d)
{	
	if(d == 0)
		throw("Cannot do modulo when 2nd argument is zero.");
	
	return n - d*(floor((n/d)));
}

void throw(char* message){
	printf("Program ended unexpectedly:\r\n%s\r\n",message);
	exit(1);
}