/********************************************************
Function:	Text									
Version: 	v1.5 (with more mem leaks, NULL check and \0)							
Uses:		Throw	
/********************************************************/
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

//LHC Type
typedef struct Text{
    int Length;
    char *Value;
} Text;

//Extra HelperFunctions
void Throw(char* message);
void ChangeText(Text *input);

//LHC HelperFunctions
void TextPrint(Text *input);
void UpdateText(Text *sourceText, Text *destText);
Text *ConcatText(Text *inputText1, Text *inputText2);
void RemoveText(Text *input);
void RemoveTextValue(Text *input);
Text *CreateText(char *input);
void CopyToText(char *inputText, int length, Text *destText);
Text * TextDup(Text *input);

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
	
	myText = CreateText("ho ");
	printf("DupTest \r\nBefore func: "); TextPrint(myText);
	ChangeText(myText);
	printf("After func (same): "); TextPrint(myText); 
	
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
	(*newText).Value = '\0';
	
	CopyToText(input, strlen(input),newText);
	
	return newText;
}

//Updates a Text with the content of another Text
void UpdateText(Text *sourceText, Text *destText){
	if (sourceText == NULL || destText == NULL)
		Throw("Cannot update a NULL text");
	CopyToText(sourceText->Value, sourceText->Length, destText);
}

//Copies a char * content to a given Text
void CopyToText(char *inputText, int length, Text *destText) {
	if (inputText == NULL || destText == NULL)
		Throw("Cannot Copy to/from a NULL Text");
	char *textContent = (char*)calloc(length+1, sizeof(char));
	int i = 0;

	strcpy(textContent, inputText);
									
	RemoveTextValue(destText);

	//Create the new Text
	destText->Length = length;
	destText->Value = textContent;
}

//Concatenates  the two texts and return a new Text with the result
Text *ConcatText(Text *inputText1, Text *inputText2) {
	if (inputText1 == NULL || inputText2 == NULL)
		Throw("Cannot concat to/from a NULL Text");
	int	size1 = (*inputText1).Length;
	int size2 = (*inputText2).Length;
	char *text1 = (*inputText1).Value;
	char *text2 = (*inputText2).Value;
	char *combinedText = (char*)malloc(size1 + size2 + 1);

	//Combine the two Texts
	strcpy(combinedText, text1);
	strcpy((combinedText + size1), text2);

	
	//Create the new Text
	return CreateText(combinedText);
}

//Removes a given Text and it's value
void RemoveText(Text *input) {
	return ;
}

//Removes a given Text's value
void RemoveTextValue(Text *input) {
	return ;
}

//Duplicates the input Text and returs the copy as a Text *
Text * TextDup(Text *input){
	return CreateText(input->Value);
}

//OTHER HELPER FUNCTIONS
void Throw(char* message){
	printf("Program ended unexpectedly:\r\n%s\r\n",message);
	exit(1);
}

void ChangeText(Text *input){
	input = TextDup(input);
	input = CreateText("ERROR!!");
}
