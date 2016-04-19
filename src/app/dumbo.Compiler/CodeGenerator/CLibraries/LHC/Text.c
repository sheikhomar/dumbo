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
void UpdateText(char *inputText, int Length, Text *text);
void ConcatText(Text *inputText1, Text *inputText2, Text *resText);
void RemoveText(Text *input);

int main()
{
	struct Text myText, anotherText;
	
	UpdateText("Hello",5,&myText);
	UpdateText(" world",6,&anotherText);
	
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

void UpdateText(char *inputText, int length, Text *text){
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

void ConcatText(Text *inputText1, Text *inputText2, Text *resText){
	int i, j, size1 = (*inputText1).Length, size2 = (*inputText2).Length;
	char *text1 = (*inputText1).Text, *text2 = (*inputText2).Text;
	char *combinedText = (char*) malloc(size1+size2);
	
	//Combine the two Texts
	for(i=0;i<size1;i++)
		*(combinedText+i) = *(text1+i);
	for(j=0;j<size2;j++)
		*(combinedText+(i+j)) = *(text2+j);
	
	//Disassemble the old Text
	RemoveText(resText);
	
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