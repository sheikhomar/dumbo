#include <stdio.h>
#include <stdlib.h>
#include <string.h>

//LHC Text Type
typedef struct Text {
	int Length;
	char *Text;
} Text;
//LHC Boolean Type
typedef enum { false, true } Boolean;


//LHC Text HelperFunctions
void TextPrint(Text *input);
void UpdateText(char *inputText, int Length, Text *text);
void ConcatText(Text *inputText1, Text *inputText2, Text *resText);
void RemoveText(Text *input);
//LHC Boolean HelperFunctions
void BooleanPrint(Boolean *input);
Boolean CreateTrueBoolean();
Boolean CreateFalseBoolean();


// Text functions
// Print a Text value given a pointer to a Text
void TextPrint(Text *input) {
	int i = 0;

	// Printing character by character
	for (i = 0; i<(*input).Length; i++)
		printf("%c", *((*input).Text + i));
	printf("\n");
}

// Updating a value of a Text given a char pointer of the new value, the length of the new value and the Text to be updated
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

// Copy a value of a Text given a char pointer to the old value, the length of the old value and the Text to be copied to (it's null - empty)
void CopyText(char *inputText, int length, Text *text) {
	char *textContent = (char*)malloc(length);
	int i;

	for (i = 0; i<length; i++)
		*(textContent + i) = *(inputText + i);
	
	//Copy the Text
	(*text).Length = length;
	(*text).Text = textContent;
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
	(*resText).Length = size1 + size2;;
	(*resText).Text = combinedText;
}

// Free the old memory occupied
void RemoveText(Text *input) {
	if (input != NULL) {
		free((*input).Text);
		(*input).Text = NULL;
	}
}


// Boolean functions
// Creating a Boolean with the value true
Boolean CreateTrueBoolean() {
	Boolean ret = true;
	return ret;
}

// Creating a Boolean with the value false
Boolean CreateFalseBoolean() {
	Boolean ret = false;
	return ret;
}

// Printing the value of a Boolean given with a pointer
void BooleanPrint(Boolean *input) {
	if ((*input) == true)
		printf("true");
	else
		printf("false");
}