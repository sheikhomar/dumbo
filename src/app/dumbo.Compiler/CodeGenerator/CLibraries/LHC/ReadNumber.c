#include <stdio.h>
#include <stdlib.h>

int ReadNumber();

int main()
{
    printf("Please enter a number:\n");

	printf("Entered Number: %d\n", ReadNumber());
    
    printf("Please enter a number:\n");
    printf("Entered Number: %d\n", ReadNumber());

    printf("Please enter a number:\n");
    printf("Entered Number: %d\n", ReadNumber());

    printf("Please enter a number:\n");
    printf("Entered Number: %d\n", ReadNumber());
	
	return 0;
}

int ReadNumber(){
    int retValue = 0;
    int result = scanf("%d", &retValue);
    if (result == 0)
        while (fgetc(stdin) != '\n');
    return retValue;
}

