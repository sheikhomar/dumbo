#include <stdio.h>
#include <stdlib.h>
#include <string.h>

//LHC Type
typedef struct Text{
    int Length;
    char *Text;
} Text;

Text *CreateText(char *inputText)
{
    Text *retVal = (Text*)malloc(sizeof(Text));
    retVal->Text = inputText;
    retVal->Length = strlen(inputText);
    return retVal;
}

Text* ConcatTextAndNumber(Text *text, double number)
{
    int MAX_SIZE = 50;
    char *output = (char*)malloc(MAX_SIZE+1);
    sprintf(output, "%lf", number);
    
    Text *numberAsText = CreateText(output);

    char *tempBuffer = (char*)malloc(text->Length + numberAsText->Length);
    strcpy(tempBuffer, text->Text);
    strcat(tempBuffer, numberAsText->Text);
    return CreateText(tempBuffer);
}

int main()
{
    Text *t0 = CreateText("My number = ");

    Text *t1 = ConcatTextAndNumber(t0, 1.0/7.0);

    printf("'%s' (Length=%d)\n", t1->Text, t1->Length);

	return 1;
}