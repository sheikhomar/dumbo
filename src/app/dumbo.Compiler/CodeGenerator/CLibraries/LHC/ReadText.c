/********************************************************
Function:	ReadText									
Version: 	v1.0 	
Uses:		Text						
/********************************************************/
#include <stdio.h>
#include <stdlib.h>
#include <string.h>


//LHC Type
typedef struct Text {
    int Length;
    char *Text;
} Text;


// From: http://stackoverflow.com/questions/314401/how-to-read-a-line-from-the-console-in-c
char * Getline(void) {
    char * line = malloc(100), *linep = line;
    size_t lenmax = 100, len = lenmax;
    int c;

    if (line == NULL)
        return NULL;

    for (;;) {
        c = fgetc(stdin);
        if (c == EOF || c == '\n')
            break;

        if (--len == 0) {
            len = lenmax;
            char * linen = realloc(linep, lenmax *= 2);

            if (linen == NULL) {
                free(linep);
                return NULL;
            }
            line = linen + (line - linep);
            linep = linen;
        }

        *line++ = c;
    }
    *line = '\0';
    return linep;
}

Text* ReadText() {
    char* text = Getline();
    int length = strlen(text);

    Text *retVal = (Text*)malloc(sizeof(Text));
    retVal->Text = text;
    retVal->Length = length;

    return retVal;
}

int main()
{
    Text *buffer;
    int size = 0;

    printf("Please enter a text:\n");
	
    printf("Type something: ");
    buffer = ReadText();
    printf("You typed: '%s'\n", buffer->Text);
    printf("Size: %d\n", buffer->Length);

	return 0;
}
