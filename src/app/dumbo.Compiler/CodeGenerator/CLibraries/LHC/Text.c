/********************************************************
Function:	Text									
Version: 	v1.0 							
Uses:			
/********************************************************/
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

//LHC Type
typedef struct Text{
    int Length;
    char *Text;
} Text;


//LHC HelperFunctions
void TextPrint(Text *input);
Text CreateText(char *inputText);
void UpdateText(Text *inputText, Text *text);
Text ConcatText(Text *inputText1, Text *inputText2);
void RemoveText(Text *input);

int main()
{
	struct Text myText, anotherText;
	Text myText2 = CreateText("Test text");
	
	UpdateText("Hello",&myText);
	UpdateText(" world",&anotherText);
	
	ConcatText(&myText,&anotherText,&myText);
	
	TextPrint(&myText);
	
	RemoveText(&myText);
	
	if(myText.Text == NULL)
		printf("Successfully removed Text\n");
	else
		printf("Error - RemoveText failed (it's %c\n",*(myText.Text));
	
	
    return 0;
}

void TextPrint(Text *input){
	int i=0;
	
	for(i=0;i<(*input).Length;i++)
		printf("%c",*((*input).Text+i));
	printf("\n");
}

Text *CreateText(char *inputText)
{
    Text *retVal = (Text*)malloc(sizeof(Text));
    retVal->Text = inputText;
    retVal->Length = strlen(inputText);
	return retVal;
}

void UpdateText(Text *inputText, Text *text){//Fix this
	char *textContent = (char*) malloc(length);
	int i;
	
	for(i=0;i<length;i++)
		*(textContent+i) = *(inputText+i);
	
	//Disassemble the old Text
	RemoveText(text);
	
	//Create the new Text
	(*text).Length = length;
	(*text).Text = textContent;
}

Text ConcatText(Text *inputText1, Text *inputText2){
	Text resText;
	int i, j;
	int	size1 = (*inputText1).Length;
	int size2 = (*inputText2).Length;
	char *text1 = (*inputText1).Text;
	char *text2 = (*inputText2).Text;
	char *combinedText = (char*) malloc(size1+size2);
	
	//Combine the two Texts
	strcpy(combinedText, text1);
	strcpy((combinedText+size1), text2);
	
	//Disassemble the old Text
	//RemoveText(resText);
	
	//Create the new Text
	(*resText).Length = size1 + size2;;
	(*resText).Text = combinedText;
}

void RemoveText(Text *input){
	if(input != NULL){
		free((*input).Text);
		(*input).Text = NULL;
	}
}