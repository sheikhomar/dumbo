/********************************************************
Function:	MyFunc									
Version: 	v1.0 			
Uses:		Text, ??				
/********************************************************/
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

//LHC Type
typedef struct Text {
    int Length;
    char *Text;
} Text;



void RemoveText(Text *input) {
    if (input != NULL) {
        free((*input).Text);
        (*input).Text = NULL;
    }
}

void UpdateText(char *inputText, int length, Text *text) {
    char *textContent = (char*)malloc(length);
    int i;

    for (i = 0; i<length; i++)
        *(textContent + i) = *(inputText + i);

    //Disassemble the old Text
    RemoveText(text);

    //Create the new Text
    (*text).Length = length;
    (*text).Text = textContent;
}



void _write(Text *input) {
    int i = 0;

    for (i = 0; i<(*input).Length; i++)
        printf("%c", *((*input).Text + i));
    printf("\n");
}


void _myfunc(Text *_ret1, Text *_ret2, Text *_ret3)
{

    Text *

    //Return
    {
        UpdateText("Fuck", 4, _ret1);
        UpdateText("Fisk", 4, _ret2);
        UpdateText("Phase", 5, _ret3);
        return;
    }
}

int main()
{
    Text a;
    Text b;
    Text c;
    _myfunc(&a, &b, &c);
    _write(&a);
    _write(&b);
    _write(&c);
    return 0;
}
