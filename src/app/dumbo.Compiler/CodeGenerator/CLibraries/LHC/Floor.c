#include <stdio.h>
#include <stdlib.h>
#include <math.h>

double Floor(double input)
{
    return floor(input);
}

int main()
{
    printf("Floor(-1.3): %lf \n", Floor(-1.34));
    printf("Floor(4.23): %lf \n", Floor(4.23));
    printf("Floor(5.67): %lf \n", Floor(5.67));
    printf("Floor(0.99): %lf \n", Floor(0.99));

	return 1;
}