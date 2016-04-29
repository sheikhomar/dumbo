/********************************************************
Function:	Convert									
Version: 	v1.1 			
Uses:		Text, Boolean				
/********************************************************/
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

//LHC Type
typedef struct Text{
    int Length;
    char *Value;
} Text;

//LHC Type
typedef enum { false, true } Boolean;


Text *CreateText(char *inputText)
{
    Text *retVal = (Text*)malloc(sizeof(Text));
    retVal->Value = inputText;
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
    strcpy(tempBuffer, text->Value);
    strcat(tempBuffer, numberAsText->Value);
    return CreateText(tempBuffer);
}


Text* ConcatTextAndBoolean(Text *text, Boolean boolean)
{
    char *tempBuffer = (char*)malloc(text->Length + 6);
    strcpy(tempBuffer, text->Value);
    strcat(tempBuffer, boolean == true ? "true" : "false");
    
    return CreateText(tempBuffer);
}

int main()
{
    Text *t0 = CreateText("My text: ");

    printf("ConcatTextAndNumber  Test %f     '%s'\n",1.0 / 7.0, ConcatTextAndNumber(t0, 1.0 / 7.0)->Value);
    printf("ConcatTextAndBoolean Test true:  '%s'\n", ConcatTextAndBoolean(t0, true)->Value);
    printf("ConcatTextAndBoolean Test false: '%s'\n", ConcatTextAndBoolean(t0, false)->Value);
    

	return 0;
}