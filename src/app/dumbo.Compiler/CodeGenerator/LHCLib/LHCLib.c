/********************************************************
External Libraries
/********************************************************/
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stdio.h>
#include <math.h>
#include <time.h>

/********************************************************
Type Declarations
/********************************************************/
typedef struct Text {
	int Length;
	char *Value;
} Text; 

typedef struct Index {
	int *indices;
	int numberOfDims;
} Index;

typedef struct Array {
	void *arr;
	int wordsize;
	Index *maxIndex;
} Array;

typedef enum { false, true } Boolean;

/********************************************************
Function Declarations
/********************************************************/
//LHC HelperFunctions
void Throw(char* message);

void TextPrint(Text *input);
void UpdateText(Text *sourceText, Text *destText);
Text *ConcatText(Text *inputText1, Text *inputText2);
void RemoveText(Text *input);
void RemoveTextValue(Text *input);
Text *CreateText(char *input);
void CopyToText(char *inputText, int length, Text *destText);
Text * TextDup(Text *input);

void BooleanPrint(Boolean *input);

Text* ConcatTextAndBoolean(Text *text, Boolean boolean);
Text* ConcatTextAndNumber(Text *text, double number);

Index *CreateIndex(int *indices, int numberOfDims);
Array *CreateArray(Index *maxIndex, int wordsize);
void PrintIndex(Index *index);
void PrintArray(Array *array);
int RecCalculateArrayOffset(Index *actualIndex, Index *maxIndex, int currentIndex);
int CalculateArrayOffset(Index *actualIndex, Index *maxIndex);

// Built-in functions
Text* ReadText();
double ReadNumber();
void Write(Text *input);
double Ceiling(double input);
double Floor(double input);
Boolean IsEqual(Text *t1, Text *t2);
double Random(double range_lower, double range_upper);
double Modulo(double n, double d);
double Div(double n, double d);

/********************************************************
Function:	Text									
Version: 	v1.4 (with more mem leaks and NULL check) ) 							
/********************************************************/
//Prints a Text's content
void TextPrint(Text *input) {
	printf("%s\r\n", input->Value);
}

//Creates a new Text Structure and 
Text *CreateText(char *input) {
	Text *newText = (Text*)calloc(1, sizeof(Text));

	(*newText).Length = 0;
	(*newText).Value = "";

	CopyToText(input, strlen(input), newText);

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
	if (inputText1 == NULL || inputText2 == NULL)
		Throw("Cannot concat to/from a NULL Text");
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
	return;

	if (input != NULL) {
		RemoveTextValue(input);
		(*input).Value = NULL;
		free(input);
	}
}

//Removes a given Text's value
void RemoveTextValue(Text *input) {
	return;

	if (input != NULL) {
		free((*input).Value);
		(*input).Value = NULL;
	}
}

//Duplicates the input Text and returs the copy as a Text *
Text * TextDup(Text *input){
	return CreateText(input->Value);
}

/********************************************************
Function:	Array
Version: 	v1.1
/********************************************************/
// Creating an Index with the indix sizes and the number of dimensions
Index *CreateIndex(int *indices, int numberOfDims){
	Index *output = (Index*)calloc(1, sizeof(Index));
	output->indices = indices;
	output->numberOfDims = numberOfDims;
	return output;
}

// Creating an Array given the Index size and the size of the type
Array *CreateArray(Index *maxIndex, int wordsize){
	int i, sum = 0;
	Array *output = (Array*)calloc(1, sizeof(Array));
	//output->arr = malloc(CalculateArrayOffset(wordsize, maxIndex, maxIndex));
	for(i = 0; i<maxIndex->numberOfDims; i++){
		sum += maxIndex->indices[i];
	}
	output->arr = malloc(wordsize*sum);
	output->wordsize = wordsize;
	output->maxIndex = maxIndex;
	return output;
}

void PrintIndex(Index *index){
	int i;
	printf("Number of dims: %d\n", index->numberOfDims);
	for(i =0; i<index->numberOfDims; i++){
		printf("The index of slot %d is: %d\n", i, index->indices[i]);
	}
}

void PrintArray(Array *array){
	printf("The data starts at: %d\n", array->arr);
	printf("The wordsize is: %d\n", array->wordsize);
	PrintIndex(array->maxIndex);
}

//Recrusive call of offset calculation (based on https://en.wikipedia.org/wiki/Row-major_order)
int RecCalculateArrayOffset(Index *actualIndex, Index *maxIndex, int currentIndex)
{
	//Base - we're at the outermost dim (ie 1) and only need to add it's actual offset
	if (currentIndex == 0)
		return *((actualIndex->indices));

	//Recursion | nd+Nd*(d-1..1)  where n is actual and N is max value for a given dim
	return *((actualIndex->indices) + currentIndex) + *((maxIndex->indices) + currentIndex)
		* RecCalculateArrayOffset(actualIndex, maxIndex, currentIndex - 1);
}


//Calculates the offset in a given array in row-major ordre
int CalculateArrayOffset(Index *actualIndex, Index *maxIndex) {
	return RecCalculateArrayOffset(actualIndex, maxIndex, maxIndex->numberOfDims - 1);
}
/********************************************************
Function:	Boolean
Version: 	v1.0
/********************************************************/
// Printing the value of a Boolean given with a pointer
void BooleanPrint(Boolean *input) {
	if ((*input) == true)
		printf("true");
	else
		printf("false");
}

/********************************************************
Function:	ReadText
Version: 	v1.1
/********************************************************/
// From: http://stackoverflow.com/questions/314401/how-to-read-a-line-from-the-console-in-c
char * Getline(void) {
	char * line = (char *)malloc(100), *linep = line;
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
			char * linen = (char *)realloc(linep, lenmax *= 2);

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
	char* text = Getline();
	int length = strlen(text);

	Text *retVal = (Text*)malloc(sizeof(Text));
	retVal->Value = text;
	retVal->Length = length;

	return retVal;
}

/********************************************************
Function:	ReadNumber
Version: 	v1.0
/********************************************************/
double ReadNumber() {
	double retValue = 0;
	int result = scanf("%lf", &retValue);
	if (result == 0)
		while (fgetc(stdin) != '\n');
	return retValue;
}

/********************************************************
Function:	Floor
Version: 	v1.0
/********************************************************/
double Floor(double input)
{
	return floor(input);
}

/********************************************************
Function:	Ceiling
Version: 	v1.0
/********************************************************/
double Ceiling(double input)
{
	return ceil(input);
}

/********************************************************
Function:	Write
Version: 	v1.0
/********************************************************/
void Write(Text *input)
{
	printf("%s\n", input->Value);
}

/********************************************************
Function:	IsEqual
Version: 	v1.0
/********************************************************/
Boolean IsEqual(Text *t1, Text *t2)
{
	if (t1->Length != t2->Length)
		return false;

	return strcmp(t1->Value, t2->Value) == 0;
}

/********************************************************
Function:	Random
Version: 	v1.0
/********************************************************/
double Random(double range1, double range2)
{
	double number, range_lower, range_upper;
	double range;

	//Find upper and lower
	if (range1 > range2)
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
	if (range < 1)
		Throw("Random's range must use two DIFFERNT Numbers.");

	if (range > 32767)
		Throw("Random's actual range must be less than 32768. The actual range is argument2 - argument1");


	//Calculate the random number and fit to range
	number = Modulo(((double)rand()), (range + 1)); // 1 % 2 =0,1 in a range 0..1

	//Return the random number, adding range offset from 0
	return number + range_lower;
}

/********************************************************
Function:	Modulo
Version: 	v1.0
/********************************************************/
double Modulo(double n, double d)
{
	if (d == 0)
		Throw("Cannot do modulo when 2nd argument is zero.");

	return n - d*(floor((n / d)));
}

/********************************************************
Function:	Div
Version: 	v1.0
/********************************************************/
double Div(double n, double d)
{
	if (d == 0)
		Throw("Cannot divide by zero.");

	return n / d;
}

/********************************************************
Function:	Throw
Version: 	v1.0
/********************************************************/
void Throw(char* message) {
	printf("Program ended unexpectedly:\r\n%s\r\n", message);
	exit(1);
}

/********************************************************
Function:	Convert									
Version: 	v1.1 						
/********************************************************/
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

/********************************************************
User area:
/********************************************************/