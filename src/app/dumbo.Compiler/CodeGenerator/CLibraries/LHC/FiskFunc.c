#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stdio.h>

//LHC Type
typedef struct Text {
    	int Length;
    	char *Text;
} Text;

void TextPrint(Text *input);
Text *CreateText(char *input);
void UpdateText(Text *input, Text *output);
void ConcatText(Text *inputText1, Text *inputText2, Text *resText);
void RemoveText(Text *input);
Text *_fiskfunc();

// Text functions //
// Print a Text value given a pointer to a Text
void TextPrint(Text *input) {
    	int i = 0;
    
    	// Printing character by character
    	for (i = 0; i<(*input).Length; i++)
    		printf("%c", *((*input).Text + i));
    	printf("\n");
}

Text *CreateText(char *input){
    	Text *output = (Text*)malloc(sizeof(Text));
		output->Text = input;
		output->Length = strlen(input);
		return output;
}

// Updating a value of a Text given a char pointer of the new value, the length of the new value and the Text to be updated
void UpdateText(Text *input, Text *output) {
    	char *textContent = (char*)malloc(input->Length);
    	int i, Length = input->Length;
    
    	for (i = 0; i<Length; i++)
    		*(textContent + i) = *(input->Text + i);
	
    	//Disassemble the old Text
    	RemoveText(output);
    
    	//Create the new Text
    	output->Length = Length;
    	output->Text = textContent;
}

// Copy a value of a Text given a char pointer to the old value, the length of the old value and the Text to be copied to (it's null - empty)
void CopyText(Text *input, Text *output) {
    	char *textContent = (char*)malloc(input->Length);
    	int i, Length = input->Length;
    
    	for (i = 0; i<Length; i++)
    		*(textContent + i) = *(input->Text + i);
    	
    	//Copy the Text
    	(*output).Length = Length;
    	(*output).Text = textContent;
}

// Concatenating to Texts' values
void ConcatText(Text *inputText1, Text *inputText2, Text *resText) {
    	int i, j, size1 = (*inputText1).Length, size2 = (*inputText2).Length;
    	char *text1 = (*inputText1).Text, *text2 = (*inputText2).Text;
    	char *combinedText = (char*)malloc(size1 + size2);
    
    	//Combine the two Texts
    	for (i = 0; i<size1; i++)
    		*(combinedText + i) = *(text1 + i);
    	for (j = 0; j<size2; j++)
    		*(combinedText + (i + j)) = *(text2 + j);
    
    	//Disassemble the old Text
    	RemoveText(resText);
    
    	//Create the new Text
    	(*resText).Length = size1 + size2;
    	(*resText).Text = combinedText;
}

// Free the old memory occupied
void RemoveText(Text *input) {
    	if (input != NULL) {
        		free(input);
    	}
}



int main()
{
    Text *name = CreateText("Torsk");
	TextPrint(name);
    UpdateText(CreateText("Rødspætte"), name);
	TextPrint(name);
    //UpdateText(_fiskfunc(), name);
	//TextPrint(name);
    return 0;
}

Text *_fiskfunc()
{
    return CreateText("Sild");
}