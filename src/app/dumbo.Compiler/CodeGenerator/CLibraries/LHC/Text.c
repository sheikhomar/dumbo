/********************************************************
Function:	Text									
Version: 	v1.2 (with more mem leaks)							
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
void UpdateText(Text *sourceText, Text *destText);
Text *ConcatText(Text *inputText1, Text *inputText2);
void RemoveText(Text *input);
void RemoveTextValue(Text *input);
Text *CreateText(char *input);
void CopyToText(char *inputText, int length, Text *destText);

int main()
{
	int i = 0;
	Text *myText = NULL;
	Text *hey = NULL;

	for (i = 0; i < 5; i++)
	{
		myText = CreateText("ho ");
		hey = CreateText("hi ");
		TextPrint(hey);
		UpdateText(ConcatText(hey, myText), myText);
		TextPrint(myText);
		UpdateText(ConcatText(myText, hey), hey);
		TextPrint(hey);
		RemoveText(myText);
		RemoveText(hey);
	}
	
    return 0;
}

//Prints a Text's content
void TextPrint(Text *input) {
	printf("%s\r\n",input->Value);
}

//Creates a new Text Structure and 
Text *CreateText(char *input){
	Text *newText = (Text*)calloc(1,sizeof(Text));
	
	(*newText).Length = 0;
	(*newText).Value = "";
	
	CopyToText(input, strlen(input),newText);
	
	return newText;
}

//Updates a Text with the content of another Text
void UpdateText(Text *sourceText, Text *destText){
	CopyToText(sourceText->Value, sourceText->Length, destText);
}

//Copies a char * content to a given Text
void CopyToText(char *inputText, int length, Text *destText) {
	char *textContent = (char*)calloc(length, sizeof(char));
	int i = 0;

	strcpy(textContent, inputText);
									
	RemoveTextValue(destText);

	//Create the new Text
	destText->Length = length;
	destText->Value = textContent;
}

//Concatenates  the two texts and return a new Text with the result
Text *ConcatText(Text *inputText1, Text *inputText2) {
	int	size1 = (*inputText1).Length;
	int size2 = (*inputText2).Length;
	char *text1 = (*inputText1).Value;
	char *text2 = (*inputText2).Value;
	char *combinedText = (char*)malloc(size1 + size2);

	//Combine the two Texts
	strcpy(combinedText, text1);
	strcpy((combinedText + size1), text2);

	
	//Create the new Text
	return CreateText(combinedText);
}

//Removes a given Text and it's value
void RemoveText(Text *input) {
	return ;
	
	if (input != NULL) {
		RemoveTextValue(input);
		(*input).Value = NULL;
		free(input);
	}
}

//Removes a given Text's value
void RemoveTextValue(Text *input) {
	return ;
	
	if (input != NULL) {
		free((*input).Value);
		(*input).Value = NULL;
	}
}