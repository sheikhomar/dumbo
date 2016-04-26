#include <stdio.h>
#include <stdlib.h>

//LHC Type
typedef struct Text{
    int Length;
    char *Text;
} Text;

void write(Text *input)
{
	printf("%s\n", input->Text);
}

int main()
{
	return 1;
}