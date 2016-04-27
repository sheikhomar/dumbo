#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stdio.h>
#include <math.h>
#include <time.h>

//LHC Type
typedef struct Text {
	int Length;
	char *Text;
} Text;

typedef enum { false, true } Boolean;


//LHC HelperFunctions
void TextPrint(Text *input);
void UpdateText(char *inputText, int Length, Text *text);
void ConcatText(Text *inputText1, Text *inputText2, Text *resText);
void RemoveText(Text *input);
void BooleanPrint(Boolean *input);

Text* ConcatTextAndBoolean(Text *text, Boolean boolean);
Text* ConcatTextAndNumber(Text *text, double number);

// Built-in functions
Text* ReadText();
double ReadNumber();
void Write(Text *input);
double Ceiling(double input);
double Floor(double input);
Boolean IsEqual(Text *t1, Text *t2);
double random(double range_lower, double range_upper);

// Text functions //
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


// Boolean functions //
// Printing the value of a Boolean given with a pointer
void BooleanPrint(Boolean *input) {
	if ((*input) == true)
		printf("true");
	else
		printf("false");
}

// From: http://stackoverflow.com/questions/314401/how-to-read-a-line-from-the-console-in-c
char * getline(void) {
    char * line = malloc(100), *linep = line;
    size_t lenmax = 100, len = lenmax;
    int c;

    if (line == NULL)
        return NULL;

    for (;;) {
        c = fgetc(stdin);
        if (c == EOF || c == '\n')
            break;

        if (--len == 0) {
            len = lenmax;
            char * linen = realloc(linep, lenmax *= 2);

            if (linen == NULL) {
                free(linep);
                return NULL;
            }
            line = linen + (line - linep);
            linep = linen;
        }

        *line++ = c;
    }
    *line = '\0';
    return linep;
}

Text* ReadText() {
    char* text = getline();
    int length = strlen(text);

    Text *retVal = (Text*)malloc(sizeof(Text));
    retVal->Text = text;
    retVal->Length = length;

    return retVal;
}

double ReadNumber() {
    double retValue = 0;
    int result = scanf("%lf", &retValue);
    if (result == 0)
        while (fgetc(stdin) != '\n');
    return retValue;
}

double Floor(double input)
{
    return floor(input);
}

double Ceiling(double input)
{
    return ceil(input);
}

void Write(Text *input)
{
    printf("%s\n", input->Text);
}

Boolean IsEqual(Text *t1, Text *t2)
{
    if (t1->Length != t2->Length)
        return false;

    return strcmp(t1->Text, t2->Text) == 0;
}


Text* ConcatTextAndNumber(Text *text, double number)
{
    int MAX_SIZE = 50;
    char *output = (char*)malloc(MAX_SIZE + 1);
    sprintf(output, "%lf", number);

    Text *numberAsText = CreateText(output);

    char *tempBuffer = (char*)malloc(text->Length + numberAsText->Length);
    strcpy(tempBuffer, text->Text);
    strcat(tempBuffer, numberAsText->Text);
    return CreateText(tempBuffer);
}


Text* ConcatTextAndBoolean(Text *text, Boolean boolean)
{
    char *tempBuffer = (char*)malloc(text->Length + 6);
    strcpy(tempBuffer, text->Text);
    strcat(tempBuffer, boolean == true ? "true" : "false");

    return CreateText(tempBuffer);
}

/********************************************************
Function:	Random									
Version: 	v1.0 							
/********************************************************/
double random(double range1, double range2)
{
	double number,range_lower,range_upper;
	double range;

	//Find upper and lower
	if(range1 > range2)
	{
		range_lower = range2;
		range_upper = range1;
	}
	else 
	{
		range_lower = range1;
		range_upper = range2;
	}
	
	range = range_upper - range_lower;
	
	//Error handle
	if(range < 1)
		throw("Random's range must use two DIFFERNT Numbers.");

	if(range > 32767)
		throw("Random's actual range must be less than 32768. The actual range is argument2 - argument1");

	
	//Calculate the random number and fit to range
	number = modulo(((double)rand()),(range+1)); // 1 % 2 =0,1 in a range 0..1
	
	//Return the random number, adding range offset from 0
	return number + range_lower;
}



