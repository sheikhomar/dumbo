/********************************************************
Function:	Text									
Version: 	v1.1 (with mem leaks)							
Uses:			
/********************************************************/
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

//LHC Type
typedef struct Text{
    int Length;
    char *Value;
} Text;


//LHC HelperFunctions
void TextPrint(Text *input);
void UpdateText(char *inputText, int Length, Text *text);
void ConcatText(Text *inputText1, Text *inputText2, Text *resText);
void RemoveText(Text *input);
Text *CreateText(char *input);

int main()
{
	int i = 0;
	Text *myText = NULL;
	Text *hey = NULL;

	for (i = 0; i < 5; i++)
	{
		myText = CreateText("Hans");
		hey = CreateText("Valueer");
		UpdateText("Hello", 5, myText);
		TextPrint(myText);
		RemoveText(myText);
		UpdateText("Hello", 5, hey);
		TextPrint(hey);
		RemoveText(hey);
	}
	
    return 0;
}

Text *CreateText(char *input){
	Text *newText = (Text*)calloc(1,sizeof(Text));
	
	(*newText).Length = 0;
	(*newText).Value = "";
	
	UpdateText(input, strlen(input),newText);
	
	return newText;
}

void TextPrint(Text *input) {
	int i = 0;

	for (i = 0; i<(*input).Length; i++)
		printf("%c", *((*input).Value + i));
	printf("\n");
}

void UpdateText(char *inputText, int length, Text *text) {
	char *textContent = (char*)calloc(length, sizeof(char));
	int i = 0;

	strcpy(textContent, inputText); //handle unsucessfull copy
									//Disassemble the old Text
	RemoveText(text);

	//Create the new Text
	text->Length = length;
	text->Value = textContent;
}

void ConcatText(Text *inputText1, Text *inputText2, Text *resText) {
	int	size1 = (*inputText1).Length;
	int size2 = (*inputText2).Length;
	char *text1 = (*inputText1).Value;
	char *text2 = (*inputText2).Value;
	char *combinedText = (char*)malloc(size1 + size2);

	//Combine the two Texts
	strcpy(combinedText, text1);
	strcpy((combinedText + size1), text2);

	//Disassemble the old Text
	RemoveText(resText);

	//Create the new Text
	(*resText).Length = size1 + size2;;
	(*resText).Value = combinedText;
}

void RemoveText(Text *input) {
	return ;
	
	if (input != NULL) {
		free((*input).Value);
		(*input).Value = NULL;
		free(input);
	}
}