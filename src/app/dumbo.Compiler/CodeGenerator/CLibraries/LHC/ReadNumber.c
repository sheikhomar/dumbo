/********************************************************
Function:	ReadNumber									
Version: 	v1.0 			
Uses:							
/********************************************************/
#include <stdio.h>
#include <stdlib.h>

double ReadNumber();

int main()
{
    printf("Please enter a number:\n");

	printf("Entered Number: %lf\n", ReadNumber());
    
    printf("Please enter a number:\n");
    printf("Entered Number: %lf\n", ReadNumber());

    printf("Please enter a number:\n");
    printf("Entered Number: %lf\n", ReadNumber());

    printf("Please enter a number:\n");
    printf("Entered Number: %lf\n", ReadNumber());
	
	return 0;
}

double ReadNumber(){
    double retValue = 0;
    int result = scanf("%lf", &retValue);
    if (result == 0)
        while (fgetc(stdin) != '\n');
    return retValue;
}

