// CME.cpp : Defines the entry point for the console application.
//
#include "stdafx.h"
#include <conio.h>

/********************************************************
Function:	Array
Version: 	v1.3
Uses:		Throw, Text, Boolean
/********************************************************/
#include <stdlib.h>
#include <stdio.h>
#include <string.h>

//LHZ Types//
//Array indexing for declaring an array, just like C (ie [1,2,3] results in 6 entries)

typedef struct DeclIndex {
	int *indices;
	int numberOfDims;
} DeclIndex;

//Array index assuming 0-indexing, just like C (ie accessing a[1,2,3] last evelemtn would be [0,1,2])
typedef struct LookupIndex {
	int *indices;
	int numberOfDims;
} LookupIndex;

//Array structure, wordsize denotes the entires size in bytes
typedef struct Array {
	void *arr;
	int wordsize;
	int lastIndex;
	DeclIndex *maxIndex;
} Array;


//LHC helper Type
//typedef enum { false, true } Boolean;
typedef enum Boolean{ff, tt} Boolean;

typedef struct Text {
	int Length;
	char *Value;
} Text;

//LHZ Functions
DeclIndex *CreateDeclIndex(int *indices, int numberOfDims);
LookupIndex *CreateLookupIndex(int *indices, int numberOfDims);
Array *CreateArray(DeclIndex *maxIndex, int wordsize);
void *IntArrayCopy(int *src, int *dest, int size);
int CalculateArrayOffset(LookupIndex *actualIndex, DeclIndex *maxIndex);
int RecCalculateArrayOffset(LookupIndex *actualIndex, DeclIndex *maxIndex, int currentIndex);
void UpdateArrayIndexValue(Array *a, LookupIndex * index, void* input);
void ReadArrayIndexValue(Array *a, LookupIndex * index, void* output);
char *FetchArrayCellAddress(Array *a, LookupIndex *index);
void CheckArrayBound(Array *a, LookupIndex *index);
LookupIndex *ConvertDeclToLookupIndexMethod(DeclIndex *inpt);

void UpdateNumberArrayIndexViaOffset(Array *a, int offset, double input);
void UpdateTextArrayIndexViaOffset(Array *a, int offset, Text *input);
void UpdateBooleanArrayIndexViaOffset(Array *a, int offset, Boolean input);
void UpdateNumberArrayIndexViaIndex(Array *a, int *offset, double input);
void UpdateTextArrayIndexViaIndex(Array *a, int *offset, Text *input);
void UpdateBooleanArrayIndexViaIndex(Array *a, int *offset, Boolean input);
double ReadNumberArrayIndex(Array *a, int offset);
Text *ReadTextArrayIndex(Array *a, int *offset);
Boolean ReadBooleanArrayIndex(Array *a, int *offset);



//HelperFunctions
void DebugPrintDeclIndex(DeclIndex *index);
void DebugPrintLookupIndex(LookupIndex *index);
void DebugPrintArray(Array *array);
void TestArrayCreate();
void TestArrayOffsetStart();
void TestArrayOffset(int a1, int a2, int a3, int m1, int m2, int m3, int expectedOffset);
void TestArraySetReadValue();
void Throw(char* message);

int main() {

	TestArrayCreate();
	TestArrayOffsetStart();
	TestArraySetReadValue();

	_getch();
	return 0;
}

//Array Functions//
//**Creation of Arrays**
// Creating an Index with the index' sizes and the number of dimensions
DeclIndex *CreateDeclIndex(int *externalIndices, int numberOfDims) {
	DeclIndex *output = (DeclIndex *)calloc(1, sizeof(DeclIndex));
	int *indices = (int *)calloc(numberOfDims, sizeof(int));
	int offset;

	//Duplicate the indices
	IntArrayCopy(externalIndices, indices, numberOfDims);

	//Assign to the Index
	output->indices = indices;
	output->numberOfDims = numberOfDims;
	return output;
}

//Create an Index with the index' sizes and the number of dimensions
LookupIndex *CreateLookupIndex(int *externalIndices, int numberOfDims) {
	LookupIndex *output = (LookupIndex *)calloc(1, sizeof(LookupIndex));
	int *indices = (int *)calloc(numberOfDims, sizeof(int));
	int offset;

	//Dup the content of external Indices
	IntArrayCopy(externalIndices, indices, numberOfDims);

	//Assign to the Index
	output->indices = indices;
	output->numberOfDims = numberOfDims;
	return output;
}

//Converts a DeclIndex to a LookupIndex
LookupIndex *ConvertDeclToLookupIndexMethod(DeclIndex *inpt) {
	int i, dims = inpt->numberOfDims;
	int *indices = (int *)calloc(dims, sizeof(int));

	IntArrayCopy(inpt->indices, indices, dims);

	return CreateLookupIndex(indices, dims);
}

//Copies the content of one IntArray to another
void *IntArrayCopy(int *src, int *dest, int size) {
	int i;
	
	for (i = 0; i < size; i++)
	{
		*(dest + i) = *(src + i);
	}
}

// Creating an Array given the Index size and the size of the type
Array *CreateArray(DeclIndex *externalMaxIndex, int wordsize) {
	Array *output = (Array *)calloc(1, sizeof(Array));
	DeclIndex *maxIndex = CreateDeclIndex(externalMaxIndex->indices, externalMaxIndex->numberOfDims);
	int entries = CalculateArrayOffset(ConvertDeclToLookupIndexMethod(maxIndex), maxIndex);

	output->arr = calloc(entries, wordsize);
	output->wordsize = wordsize;
	output->maxIndex = maxIndex;
	output->lastIndex = entries;
	return output;
}

//**Get ArrayOffset for a given index**
//Recrusive call of offset calculation (based on https://en.wikipedia.org/wiki/Row-major_order)
int RecCalculateArrayOffset(LookupIndex *actualIndex, DeclIndex *maxIndex, int currentIndex)
{
	//Base - we're at the outermost dim (ie 1) and only need to add it's actual offset
	if (currentIndex == 0)
		return *((actualIndex->indices));

	//Recursion | nd+Nd*(d-1..1)  where n is actual and N is max value for a given dim
	return *((actualIndex->indices) + currentIndex) + *((maxIndex->indices) + currentIndex)
		* RecCalculateArrayOffset(actualIndex, maxIndex, currentIndex - 1);
}

//Calculates the offset in a given array in row-major ordre
int CalculateArrayOffset(LookupIndex *actualIndex, DeclIndex *maxIndex) {
	return RecCalculateArrayOffset(actualIndex, maxIndex, maxIndex->numberOfDims - 1);
}

//**Get/set values in array - base**
//Copies the given value to a specific index in the array
void UpdateArrayIndexValue(Array *a, LookupIndex * index, void* input) {
	char * cellAddress = FetchArrayCellAddress(a, index);

	CheckArrayBound(a, index);
	memcpy(cellAddress, input, a->wordsize);

	return;
}

//Reads the given value from a specific index in the given array
void ReadArrayIndexValue(Array *a, LookupIndex * index, void* output) {
	char * cellAddress = FetchArrayCellAddress(a, index);

	CheckArrayBound(a, index);
	memcpy(output, cellAddress, a->wordsize);

	return;
}

//Finds the given cellAdress
char *FetchArrayCellAddress(Array *a, LookupIndex *index) {
	int byteOffset = CalculateArrayOffset(index, a->maxIndex) * a->wordsize;
	return ((char *)a->arr) + byteOffset; //char in c is equal to one byte.
}

void CheckArrayBound(Array *a, LookupIndex *index) {
	int curOffset = CalculateArrayOffset(index, a->maxIndex);

	if (curOffset < 0 || curOffset > a->lastIndex) {
		char message[50];
		sprintf(message, "Array out of bound: Tried to access element number %d\r\n", curOffset);

		Throw(message);
	}
}

//**Get/set values in array - extension for LHZ**
//Copies the given TYPE value to a specific index in the array
void UpdateNumberArrayIndexViaOffset(Array *a, int offset, double input) {}
void UpdateTextArrayIndexViaOffset(Array *a, int offset, Text *input) {}
void UpdateBooleanArrayIndexViaOffset(Array *a, int offset, Boolean input) {}
void UpdateNumberArrayIndexViaIndex(Array *a, int *offset, double input) {}
void UpdateTextArrayIndexViaIndex(Array *a, int *offset, Text *input) {}
void UpdateBooleanArrayIndexViaIndex(Array *a, int *offset, Boolean input) {}

//Reads the given  TYPE value from a specific index in the given array
double ReadNumberArrayIndex(Array *a, int offset) {}
Text *ReadTextArrayIndex(Array *a, int *offset) {}
Boolean ReadBooleanArrayIndex(Array *a, int *offset) {}



//************Helper Functions***********//
//Test the creating of Array - does nothing but running the actual code atm
void TestArrayCreate() {
	int numbDims = 3;
	int inputIndices[] = { 3,5,7 };

	printf("Beginning Array Create Test\r\n");
	DeclIndex *maxindex = CreateDeclIndex(inputIndices, numbDims);
	Array *a = CreateArray(maxindex, sizeof(double));
	printf("Finished Array Create Test\r\n");
	//DebugPrintArray(a);
	return;
}


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

	LookupIndex actualIndex;
	actualIndex.indices = actual;
	actualIndex.numberOfDims = 3;

	DeclIndex maxIndex;
	maxIndex.indices = max;
	maxIndex.numberOfDims = 3;

	int offset = CalculateArrayOffset(&actualIndex, &maxIndex);

	if (expectedOffset != offset) {
		printf("ERROR: Test with %d, %d, %d, %d, %d, %d failed\r\nExpected offset was %d, calculated was %d\r\n", a1, a2, a3, m1, m2, m3, expectedOffset, offset);
	}
	return;
}

void TestArraySetReadValue() {
	//Set-up
	int indexIntArr[] = { 1,2,3 };
	Array * a = CreateArray(CreateDeclIndex(indexIntArr, 3), sizeof(double));

	printf("Beginning Array Set/Read Test\r\n");

	//Assign values
	double val1 = 5, val2 = 10;
	int in1Arr[] = { 0,0,0 }, in2Arr[] = { 0,1,2 }, in3Arr[] = { 1,0,0 };
	LookupIndex *in1 = CreateLookupIndex(in1Arr, 3);
	LookupIndex *in2 = CreateLookupIndex(in2Arr, 3);
	LookupIndex *in3 = CreateLookupIndex(in3Arr, 3);

	//Set the values
	UpdateArrayIndexValue(a, in1, &val1);
	UpdateArrayIndexValue(a, in2, &val2);

	//Read the set values
	double ret1 = 0, ret2 = 0;
	ReadArrayIndexValue(a, in1, &ret1);
	ReadArrayIndexValue(a, in2, &ret2);

	if (ret1 != val1 || ret2 != val2)
		printf("Error: sat values %f and %f, but read %f and %f\r\n", val1, val2, ret1, ret2);

	//Bound check
	//ReadArrayIndexValue(a, in3, &ret1);

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

void DebugPrintLookupIndex(LookupIndex *index) {
	int i;
	printf("Number of dims: %d\n", index->numberOfDims);
	for (i = 0; i < index->numberOfDims; i++) {
		printf("The index of slot %d is: %d\n", i, index->indices[i]);
	}
}

void DebugPrintArray(Array *array) {
	printf("The data starts at: %d\n", array->arr);
	printf("The wordsize is: %d\n", array->wordsize);
	DebugPrintDeclIndex(array->maxIndex);
}

void Throw(char* message) {
	printf("Program ended unexpectedly:\r\n%s\r\n", message);
	exit(1);
}