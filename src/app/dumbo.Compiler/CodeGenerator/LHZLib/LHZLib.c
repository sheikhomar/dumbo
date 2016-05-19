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
//LHZ Types
typedef enum {t_Number, t_Text, t_Boolean} Type;
typedef enum { false, true } Boolean;

typedef struct Text {
	int Length;
	char *Value;
} Text; 

//Array indexing for declaring an array, just like C (ie [1,2,3] results in 6 entries)
typedef struct DeclIndex {
	int *indices;
	int numberOfDims;
} DeclIndex;

//Array structure, wordsize denotes the entires size in bytes
typedef struct Array {
	void *arr;
	Type type;
	int wordsize;
	int entries;
	DeclIndex *maxIndex;
} Array;
/********************************************************
Function Declarations
/********************************************************/
//LHZ HelperFunctions
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

Text* ConvertNumberToText(double number);
Text* ConvertBooleanToText(Boolean boolean);

DeclIndex *CreateDeclIndex(int *indices, int numberOfDims);
Array *CreateArray(DeclIndex *maxIndex, Type type);
void IntArrayCopy(int *src, int *dest, int size);
int CalculateArrayOffset(int *actualIndex, DeclIndex *maxIndex);
int RecCalculateArrayOffset(int *actualIndex, DeclIndex *maxIndex, int currentIndex);
int CalculateNumberOfArrayEntries(DeclIndex *index);
void UpdateArrayIndexValue(Array *a, int offset, void* input);
void ReadArrayIndexValue(Array *a, int offset, void* output);
char *FetchArrayCellAddress(Array *a, int offset);
void CheckArrayBound(Array *a, int offset);
int GetArrayWordSize(Type type);
void InitArray(Array *a);
void UpdateNumberArrayIndexViaOffset(Array *a, int offset, double input);
void UpdateTextArrayIndexViaOffset(Array *a, int offset, Text *input);
void UpdateBooleanArrayIndexViaOffset(Array *a, int offset, Boolean input);
void UpdateNumberArrayIndexViaIndex(Array *a, int *offset, double input);
void UpdateTextArrayIndexViaIndex(Array *a, int *offset, Text *input);
void UpdateBooleanArrayIndexViaIndex(Array *a, int *offset, Boolean input);
double ReadNumberArrayIndex(Array *a, int *offset);
Text *ReadTextArrayIndex(Array *a, int *offset);
Boolean ReadBooleanArrayIndex(Array *a, int *offset);
int GetArrayDimSize(Array *a, int dimNumber);
int *ReduceThisIdexByOne(int *indices, int dims);
Array *ArrayDup(Array *arr);
void CheckReturnArraySize(Array *formal, Array *ret);

// Built-in functions
Text* ReadText();
double ReadNumber();
void Write(Text *input);
double Ceiling(double input);
double Floor(double input);
Boolean IsTextAndTextEqual(Text *t1, Text *t2);
double Random(double range_lower, double range_upper);
double Modulo(double n, double d);
double Div(double n, double d);

/********************************************************
Function:	Text									
Version: 	v1.5							
/********************************************************/
//Prints a Text's content
void TextPrint(Text *input) {
    	printf("%s\r\n", input->Value);
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
/********************************************************
Function:	Array
Version: 	v1.9
Uses:		Throw, Text, Boolean
/********************************************************/
//**Creation of Arrays**
// Creating an Index with the index' sizes and the number of dimensions
DeclIndex *CreateDeclIndex(int *externalIndices, int numberOfDims) {
	DeclIndex *output = (DeclIndex *)calloc(1, sizeof(DeclIndex));
	int *indices = (int *)calloc(numberOfDims, sizeof(int));

	//Duplicate the indices
	IntArrayCopy(externalIndices, indices, numberOfDims);

	//Assign to the Index
	output->indices = indices;
	output->numberOfDims = numberOfDims;
	return output;
}

//Copies the content of one IntArray to another
void IntArrayCopy(int *src, int *dest, int size) {
	int i;

	for (i = 0; i < size; i++)
	{
		*(dest + i) = *(src + i);
	}
}

//Creating an Array given the Index size and the size of the type
Array *CreateArray(DeclIndex *externalMaxIndex, Type type) {
	Array *output = (Array *)calloc(1, sizeof(Array));
	DeclIndex *maxIndex = CreateDeclIndex(externalMaxIndex->indices, externalMaxIndex->numberOfDims);
	int entries = CalculateNumberOfArrayEntries(maxIndex);
	int wordSize = GetArrayWordSize(type);

	output->arr = malloc(entries * wordSize);
	output->wordsize = wordSize;
	output->type = type;
	output->maxIndex = maxIndex;
	output->entries = entries;

	InitArray(output);

	return output;
}

//Calculate the number of entires for a given index by utalising the ArrayOffset calculation
int CalculateNumberOfArrayEntries(DeclIndex *index) {
	int dims = index->numberOfDims;
	int *indices = (int *)calloc(dims, sizeof(int));

	//Reduce the indices by one - this compensates for the difference between a declIndex and the actual indes (array[1,2,3] has the last element at [0,1,2])
	DeclIndex * copiedIndex = CreateDeclIndex(index->indices, index->numberOfDims);
	ReduceThisIdexByOne(copiedIndex->indices, copiedIndex->numberOfDims);

	//This is needed to reuse the 'CalculateArrayOffset* algorithm
	return CalculateArrayOffset(copiedIndex->indices, index) + 1;
}

//Reduces all entries in the given int array by one. Returns the same pointer as given
int *ReduceThisIdexByOne(int *indices, int dims) {
	int i;

	for (i = 0; i < dims; i++)
		*(indices + i) = *(indices + i) - 1;

	return indices;
}

//Find the wordSize of a given type
int GetArrayWordSize(Type type) {
	switch (type)
	{
	case t_Number:
		return sizeof(double);
	case t_Text:
		return sizeof(Text *);
	case t_Boolean:
		return sizeof(Boolean);
	default:
		return 1;
	}
}

//Initialises all array entires to their default value
void InitArray(Array *a) {
	int i, arrEntries = a->entries;

	switch (a->type)
	{
	case t_Number:
		for (i = 0; i < arrEntries; i++)
			*((double *)a->arr + i) = 0;
		break;

	case t_Boolean:
		for (i = 0; i < arrEntries; i++)
			*((Boolean *)a->arr + i) = false;
		break;
	case t_Text:
		for (i = 0; i < arrEntries; i++)
			*((Text **)a->arr + i) = CreateText("");
		break;
	default:
		for (i = 0; i < arrEntries; i++)
			*((int *)a->arr + i) = 0;
		break;
	}
}

//Duplicates the input Array and returs the copy as a Array *
Array *ArrayDup(Array *arr) {
	Array *arrCopy = CreateArray(arr->maxIndex, arr->type);
	int i, arrEntries = arr->entries;

	switch (arr->type)
	{
	case t_Number:
		for (i = 0; i < arrEntries; i++)
			*((double *)arrCopy->arr + i) = *((double *)arr->arr + i);
		break;

	case t_Boolean:
		for (i = 0; i < arrEntries; i++)
			*((Boolean *)arrCopy->arr + i) = *((Boolean *)arr->arr + i);
		break;
	case t_Text:
		for (i = 0; i < arrEntries; i++)
			*((Text **)arrCopy->arr + i) = CreateText((*((Text **)arr->arr + i))->Value);
		break;
	default:
		for (i = 0; i < arrEntries; i++)
			*((int *)arrCopy->arr + i) = *((int *)arr->arr + i);
		break;
	}

	return arrCopy;
}

//**Get ArrayOffset for a given index**
//Recrusive call of offset calculation (based on https://en.wikipedia.org/wiki/Row-major_order)
int RecCalculateArrayOffset(int *actualIndex, DeclIndex *maxIndex, int currentIndex)
{
	//Base - we're at the outermost dim (ie 1) and only need to add it's actual offset
	if (currentIndex == 0)
		return *((actualIndex));

	//Recursion | nd+Nd*(d-1..1)  where n is actual and N is max value for a given dim
	return *(actualIndex + currentIndex) + *((maxIndex->indices) + currentIndex)
		* RecCalculateArrayOffset(actualIndex, maxIndex, currentIndex - 1);
}

//Calculates the offset in a given array in row-major ordre
int CalculateArrayOffset(int *actualIndex, DeclIndex *maxIndex) {
	return RecCalculateArrayOffset(actualIndex, maxIndex, maxIndex->numberOfDims - 1);
}

//**Get/set values in array - base**
//Copies the given value to a specific index in the array
void UpdateArrayIndexValue(Array *a, int offset, void* input) {
	char * cellAddress = FetchArrayCellAddress(a, offset);

	CheckArrayBound(a, offset);
	memcpy(cellAddress, input, a->wordsize);

	return;
}

//Reads the given value from a specific index in the given array
void ReadArrayIndexValue(Array *a, int offset, void* output) {
	char * cellAddress = FetchArrayCellAddress(a, offset);

	CheckArrayBound(a, offset);
	memcpy(output, cellAddress, a->wordsize);

	return;
}

//Finds the given cellAdress
char *FetchArrayCellAddress(Array *a, int offset) {
	int byteOffset = offset * a->wordsize;
	return ((char *)a->arr) + byteOffset; //char in c is equal to one byte.
}

void CheckArrayBound(Array *a, int offset) {
	if (offset < 0 || offset > a->entries) {
		char message[50];
		sprintf(message, "Array out of bound: Tried to access element number %d\r\n", offset);

		Throw(message);
	}
}

//**Get/set values in array - extension for LHZ**
//Copies the given TYPE value to a specific index in the array
void UpdateNumberArrayIndexViaOffset(Array *a, int offset, double input) {
	UpdateArrayIndexValue(a, offset, &input);
}
void UpdateTextArrayIndexViaOffset(Array *a, int offset, Text *input) {
	UpdateArrayIndexValue(a, offset, &input);
}
void UpdateBooleanArrayIndexViaOffset(Array *a, int offset, Boolean input) {
	UpdateArrayIndexValue(a, offset, &input);
}
void UpdateNumberArrayIndexViaIndex(Array *a, int *index, double input) {
	UpdateArrayIndexValue(a, CalculateArrayOffset(ReduceThisIdexByOne(index, a->maxIndex->numberOfDims), a->maxIndex), &input);
}
void UpdateTextArrayIndexViaIndex(Array *a, int *index, Text *input) {
	UpdateArrayIndexValue(a, CalculateArrayOffset(ReduceThisIdexByOne(index, a->maxIndex->numberOfDims), a->maxIndex), &input);
}
void UpdateBooleanArrayIndexViaIndex(Array *a, int *index, Boolean input) {
	UpdateArrayIndexValue(a, CalculateArrayOffset(ReduceThisIdexByOne(index, a->maxIndex->numberOfDims), a->maxIndex), &input);
}

//Reads the given TYPE value from a specific index in the given array
double ReadNumberArrayIndex(Array *a, int *index) {
	double ret;
	ReadArrayIndexValue(a, CalculateArrayOffset(ReduceThisIdexByOne(index, a->maxIndex->numberOfDims), a->maxIndex), &ret);
	return ret;
}
Text *ReadTextArrayIndex(Array *a, int *index) {
	Text *ret;
	ReadArrayIndexValue(a, CalculateArrayOffset(ReduceThisIdexByOne(index, a->maxIndex->numberOfDims), a->maxIndex), &ret);
	return TextDup(ret);
}
Boolean ReadBooleanArrayIndex(Array *a, int *index) {
	Boolean ret;
	ReadArrayIndexValue(a, CalculateArrayOffset(ReduceThisIdexByOne(index, a->maxIndex->numberOfDims), a->maxIndex), &ret);
	return ret;
}

// Get the size of a specific dimension
int GetArrayDimSize(Array *a, int dimNumber) {
	if (dimNumber < 0 || dimNumber > a->maxIndex->numberOfDims - 1)
		Throw("Requested dimension is out of range");

	return *(((int*)(a->maxIndex->indices)) + dimNumber);
}

void CheckReturnArraySize(Array *formal, Array *ret) {
	if (formal->entries != ret->entries)
		Throw("Mismatch in entries for (one of) the input array(s) and reutrn array(s)");
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
Version: 	v2
/********************************************************/
Boolean IsTextAndTextEqual(Text *t1, Text *t2)
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
Version: 	v1.2 						
/********************************************************/
Text* ConvertNumberToText(double number)
{
	int MAX_SIZE = 50;
	char *output = (char*)malloc(MAX_SIZE + 1);
	sprintf(output, "%lf", number);

	return CreateText(output);
}

Text* ConvertBooleanToText(Boolean boolean)
{
	return CreateText(boolean == true ? "true" : "false");
}

/********************************************************
User area:
/********************************************************/