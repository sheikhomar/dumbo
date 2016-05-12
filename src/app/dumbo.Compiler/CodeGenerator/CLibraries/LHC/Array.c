/********************************************************
Function:	Array
Version: 	v1.5
Uses:		Throw, Text, Boolean
/********************************************************/
#include <stdlib.h>
#include <stdio.h>
#include <string.h>

//*****External LHZ lib*****//
//Types
typedef enum { false, true } Boolean;

typedef struct Text {
	int Length;
	char *Value;
} Text;
void Throw(char* message);

//Functions
void UpdateText(Text *sourceText, Text *destText);
Text *ConcatText(Text *inputText1, Text *inputText2);
void RemoveText(Text *input);
void RemoveTextValue(Text *input);
Text *CreateText(char *input);
void CopyToText(char *inputText, int length, Text *destText);
Text * TextDup(Text *input);


//*****Array LHZ Types*****//
//LHZ Types
typedef enum {t_Number, t_Text, t_Boolean} Type;

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

//*****Array LHZ Functions*****//
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

//HelperFunctions
void DebugPrintDeclIndex(DeclIndex *index);
void DebugPrintArray(Array *array);
void TestArrayCreate();
void TestArrayOffsetStart();
void TestArrayOffset(int a1, int a2, int a3, int m1, int m2, int m3, int expectedOffset);
void TestArraySetReadValue();
void TestArraySerReadLHZValues();

int main() {

	TestArrayCreate();
	TestArrayOffsetStart();
	TestArraySetReadValue();
	TestArraySerReadLHZValues();

	return 0;
}

//*****Array Functions*****//
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
	int i, *indices = (int *)calloc(dims, sizeof(int));

	//Reduce the indices by one - this compensates for the difference between a declIndex and the actual indes (array[1,2,3] has the last element at [0,1,2])
	//This is needed to reuse the 'CalculateArrayOffset* algorithm
	for (i = 0; i < dims; i++) {
		*(indices + i) = *(index->indices + i) - 1;
	}

	return CalculateArrayOffset(indices, index) + 1;
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
	UpdateArrayIndexValue(a, CalculateArrayOffset(index, a->maxIndex), &input);
}
void UpdateTextArrayIndexViaIndex(Array *a, int *index, Text *input) {
	UpdateArrayIndexValue(a, CalculateArrayOffset(index, a->maxIndex), &input);
}
void UpdateBooleanArrayIndexViaIndex(Array *a, int *index, Boolean input) {
	UpdateArrayIndexValue(a, CalculateArrayOffset(index, a->maxIndex), &input);
}

//Reads the given TYPE value from a specific index in the given array
double ReadNumberArrayIndex(Array *a, int *index) {
	double ret;
	ReadArrayIndexValue(a, CalculateArrayOffset(index, a->maxIndex), &ret);
	return ret;
}
Text *ReadTextArrayIndex(Array *a, int *index) {
	Text *ret;
	ReadArrayIndexValue(a, CalculateArrayOffset(index, a->maxIndex), &ret);
	return TextDup(ret);
}
Boolean ReadBooleanArrayIndex(Array *a, int *index) {
	Boolean ret;
	ReadArrayIndexValue(a, CalculateArrayOffset(index, a->maxIndex), &ret);
	return ret;
}


//************Helper Functions***********//
//Test the creating of Array - does nothing but running the actual code atm
void TestArrayCreate() {
	int numbDims = 3;
	int inputIndices[] = { 3,5,7 };

	printf("Beginning Array Create Test\r\n");
	DeclIndex *maxindex = CreateDeclIndex(inputIndices, numbDims);
	Array *a = CreateArray(maxindex, t_Number);
	printf("Finished Array Create Test\r\n");
	//DebugPrintArray(a);
	return;
}

//Starts the ArrayOffset test
void TestArrayOffsetStart() {
	printf("Beginning Array Offset Test\r\n");
	TestArrayOffset(0, 0, 0, 1, 1, 1, 0);
	TestArrayOffset(0, 0, 0, 1, 2, 3, 0);
	TestArrayOffset(0, 0, 1, 1, 2, 3, 1);
	TestArrayOffset(0, 1, 0, 1, 2, 3, 3);
	TestArrayOffset(1, 0, 0, 1, 2, 3, 6);
	TestArrayOffset(1, 1, 1, 1, 2, 3, 10);
	TestArrayOffset(1, 2, 3, 1, 2, 3, 15);
	TestArrayOffset(0, 0, 0, 3, 3, 3, 0);
	TestArrayOffset(3, 2, 1, 3, 3, 3, 34);
	TestArrayOffset(3, 3, 3, 3, 3, 3, 39);
	printf("Finished Array Offset Test\r\n");

	return;
}

//Calculates the offset on a three dimentional array based on 3 x actual and max index values
void TestArrayOffset(int a1, int a2, int a3, int m1, int m2, int m3, int expectedOffset) {
	int actual[] = { a1,a2,a3 };
	int max[] = { m1,m2,m3 };

	DeclIndex maxIndex;
	maxIndex.indices = max;
	maxIndex.numberOfDims = 3;

	int offset = CalculateArrayOffset(actual, &maxIndex);

	if (expectedOffset != offset) {
		printf("ERROR: Test with %d, %d, %d, %d, %d, %d failed\r\nExpected offset was %d, calculated was %d\r\n", a1, a2, a3, m1, m2, m3, expectedOffset, offset);
	}
	return;
}

void TestArraySetReadValue() {

	printf("Beginning Array Set/Read Test\r\n");

	//Create Array
	int indexIntArr[] = { 1,2,3 };
	DeclIndex *decl = CreateDeclIndex(indexIntArr, 3);
	Array *a = CreateArray(decl, t_Number);

	//Assign values
	double val1 = 5, val2 = 10;
	int in1Arr[] = { 0,0,0 }, in2Arr[] = { 0,1,2 }, in3Arr[] = { 1,0,0 };
	int offset1 = CalculateArrayOffset(in1Arr, decl);
	int offset2 = CalculateArrayOffset(in2Arr, decl);
	int offset3 = CalculateArrayOffset(in3Arr, decl);

	//Set the values
	UpdateArrayIndexValue(a, offset1, &val1);
	UpdateArrayIndexValue(a, offset2, &val2);

	//Read the set values
	double ret1 = 0, ret2 = 0;
	ReadArrayIndexValue(a, offset1, &ret1);
	ReadArrayIndexValue(a, offset2, &ret2);

	if (ret1 != val1 || ret2 != val2)
		printf("Error: sat values %f and %f, but read %f and %f\r\n", val1, val2, ret1, ret2);

	//Bound check
	//ReadArrayIndexValue(a, offset3, &ret1);

	printf("Finished Array Set/Read Test\r\n");

	return;
}

void TestArraySerReadLHZValues() {

	printf("Beginning Array Set/Read LHZ Value Test\r\n");

	//Numbers
	{
		//Create array
		int indexIntArr[] = { 1,2,3 };
		DeclIndex *decl = CreateDeclIndex(indexIntArr, 3);
		Array *a = CreateArray(decl, t_Number);

		//Set values
		double val1 = 20, val2 = 40;
		int index[] = { 0,0,0 };
		UpdateNumberArrayIndexViaIndex(a, index, val1);
		UpdateNumberArrayIndexViaOffset(a, CalculateNumberOfArrayEntries(a->maxIndex)-1, val2);

		//Read values
		int read1[] = { 0,0,0 };
		int read2[] = { 0,1,2 };
		double ret1 = ReadNumberArrayIndex(a, read1);
		double ret2 = ReadNumberArrayIndex(a, read2);

		if (val1 != ret1 || val2 != ret2)
			printf("Error: sat values %f and %f, but read %f and %f\r\n", val1, val2, ret1, ret2);
	}

	//Booleans
	{
		//Create array
		int indexIntArr[] = { 1,2,3 };
		DeclIndex *decl = CreateDeclIndex(indexIntArr, 3);
		Array *a = CreateArray(decl, t_Boolean);

		//Set values
		Boolean val1 = true, val2 = false;
		int index[] = { 0,0,0 };
		UpdateBooleanArrayIndexViaIndex(a, index, val1);
		UpdateBooleanArrayIndexViaOffset(a, CalculateNumberOfArrayEntries(a->maxIndex)-1, val2);

		//Read values
		int read1[] = { 0,0,0 };
		int read2[] = { 0,1,2 };
		Boolean ret1 = ReadBooleanArrayIndex(a, read1);
		Boolean ret2 = ReadBooleanArrayIndex(a, read2);

		if (val1 != ret1 || val2 != ret2)
			printf("Error: sat values %d and %d, but read %d and %d\r\n", val1, val2, ret1, ret2);
	}

	//Text
	{
		//Create array
		int indexIntArr[] = { 1,2,3 };
		DeclIndex *decl = CreateDeclIndex(indexIntArr, 3);
		Array *a = CreateArray(decl, t_Text);

		//Set values
		Text *val1 = CreateText("Hello"), *val2 = CreateText("World");
		int index[] = { 0,0,0 };
		UpdateTextArrayIndexViaIndex(a, index, val1);
		UpdateTextArrayIndexViaOffset(a, CalculateNumberOfArrayEntries(a->maxIndex)-1, val2);

		//Read values
		int read1[] = { 0,0,0 };
		int read2[] = { 0,1,2 };
		Text *ret1 = ReadTextArrayIndex(a, read1);
		Text *ret2 = ReadTextArrayIndex(a, read2);

		if (strcmp(val1->Value, ret1->Value) || strcmp(val2->Value, ret2->Value))
			printf("Error: sat values %s and %s, but read %s and %s\r\n", val1->Value, val2->Value, ret1->Value, ret2->Value);
	}

	printf("Finished Array Set/Read Test\r\n");

	return;
}


void DebugPrintDeclIndex(DeclIndex *index) {
	int i;
	printf("Number of dims: %d\n", index->numberOfDims);
	for (i = 0; i < index->numberOfDims; i++) {
		printf("The index of slot %d is: %d\n", i, index->indices[i]);
	}
}

void DebugPrintArray(Array *array) {
	printf("The data starts at: %x\n", (unsigned int)(array->arr));
	printf("The wordsize is: %d\n", array->wordsize);
	DebugPrintDeclIndex(array->maxIndex);
}


//*****LHZ Functions*****
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

void Throw(char* message) {
	printf("Program ended unexpectedly:\r\n%s\r\n", message);
	exit(1);
}