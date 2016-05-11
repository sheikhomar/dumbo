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


Text *CreateText(char *input);
void CopyToText(char *inputText, int length, Text *destText);
void RemoveTextValue(Text *input);
void Throw(char* message);
Text* ConcatTextAndNumber(Text *text, double number);
Text* ConcatTextAndBoolean(Text *text, Boolean boolean);
Text* ConvertNumberToText(double number);
Text* ConvertBooleanToText(Boolean boolean);

//Creates a new Text Structure and 
Text *CreateText(char *input){
	Text *newText = (Text*)calloc(1,sizeof(Text));
	
	(*newText).Length = 0;
	(*newText).Value = "";
	
	CopyToText(input, strlen(input),newText);
	
	return newText;
}

//Copies a char * content to a given Text
void CopyToText(char *inputText, int length, Text *destText) {
	if (inputText == NULL || destText == NULL)
		Throw("Cannot Copy to/from a NULL Text");
	char *textContent = (char*)calloc(length, sizeof(char));
	int i = 0;

	strcpy(textContent, inputText);
									
	RemoveTextValue(destText);

	//Create the new Text
	destText->Length = length;
	destText->Value = textContent;
}

//Removes a given Text's value
void RemoveTextValue(Text *input) {
	return ;
	
	if (input != NULL) {
		free((*input).Value);
		(*input).Value = NULL;
	}
}

//OTHER HELPER FUNCTIONS
void Throw(char* message){
	printf("Program ended unexpectedly:\r\n%s\r\n",message);
	exit(1);
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

Text* ConvertNumberToText(double number)
{
	int MAX_SIZE = 50;
    char *output = (char*)malloc(MAX_SIZE+1);
    sprintf(output, "%lf", number);
    
    return CreateText(output);
}

Text* ConvertBooleanToText(Boolean boolean)
{
    return CreateText(boolean == true ? "true" : "false");
}

int main()
{
    Text *t0 = CreateText("My text: ");
	double n0 = 11;
	Text *t1 = ConvertNumberToText(n0);
	Boolean b0 = true, b1 = false;
	Text *t2 = ConvertBooleanToText(b0);
	Text *t3 = ConvertBooleanToText(b1);
	
	printf("ConvertNumberToText		Test %f		'%s'\n", n0, t1->Value);
	printf("ConvertBooleanToText	Test true	'%s'\n", t2->Value);
	printf("ConvertBooleanToText	Test false	'%s'\n", t3->Value);
    //printf("ConcatTextAndNumber  Test %f     '%s'\n",1.0 / 7.0, ConcatTextAndNumber(t0, 1.0 / 7.0)->Value);
    //printf("ConcatTextAndBoolean Test true:  '%s'\n", ConcatTextAndBoolean(t0, true)->Value);
    //printf("ConcatTextAndBoolean Test false: '%s'\n", ConcatTextAndBoolean(t0, false)->Value);
    

	return 0;
}