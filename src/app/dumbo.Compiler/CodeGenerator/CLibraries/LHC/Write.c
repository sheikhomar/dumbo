/********************************************************
Function:	Write									
Version: 	v1.0 
Uses:		Text							
/********************************************************/
#include <stdio.h>
#include <stdlib.h>

//LHC Type
typedef struct Text{
    int Length;
    char *Text;
} Text;

void Write(Text *input)
{
	printf("%s\n", input->Text);
}

int main()
{
	return 1;
}