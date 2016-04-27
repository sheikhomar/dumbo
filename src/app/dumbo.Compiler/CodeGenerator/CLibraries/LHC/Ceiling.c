#include <stdio.h>
#include <stdlib.h>
#include <math.h>

double Ceiling(double input)
{
    return ceil(input);
}

int main()
{
    printf("Ceiling(-1.3): %lf \n", Ceiling(-1.34));
    printf("Ceiling(4.23): %lf \n", Ceiling(4.23));
    printf("Ceiling(5.67): %lf \n", Ceiling(5.67));
    printf("Ceiling(0.99): %lf \n", Ceiling(0.99));

	return 1;
}