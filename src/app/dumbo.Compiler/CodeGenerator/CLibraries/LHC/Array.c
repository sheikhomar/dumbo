/********************************************************
Function:	Array
Version: 	v1.1
Uses:		Throw
/********************************************************/
#include <stdlib.h>
#include <stdio.h>
#include <string.h>

typedef struct Index {
	int *indices;
	int numberOfDims;
} Index;

typedef struct Array {
	void *arr;
	int wordsize;
	Index *maxIndex;
} Array;


//LHZ Functions
Index *CreateIndex(int *indices, int numberOfDims);
Array *CreateArray(Index *maxIndex, int wordsize);
void DebugPrintIndex(Index *index);
void DebugPrintArray(Array *array);
int RecCalculateArrayOffset(Index *actualIndex, Index *maxIndex, int currentIndex);
int CalculateArrayOffset(Index *actualIndex, Index *maxIndex);

//HelperFunctions
void TestArrayCreate();
void TestArrayOffsetStart();
void TestArrayOffset(int a1, int a2, int a3, int m1, int m2, int m3, int expectedOffset);
void Throw(char* message);

int main() {
	TestArrayCreate();
	TestArrayOffsetStart();

	_getch();
	return 0;
}

//Array Functions//
// Creating an Index with the indix sizes and the number of dimensions
Index *CreateIndex(int *externalIndices, int numberOfDims) {
	Index *output = (Index *)calloc(1, sizeof(Index));
	int *indices = (int *)calloc(numberOfDims, sizeof(int));
	int offset;

	//Dup the content of external Indices
	for (offset = 0; offset < numberOfDims; offset++)
	{
		*(indices + offset) = *(externalIndices + offset);
	}

	//Assign to the Index
	output->indices = indices;
	output->numberOfDims = numberOfDims;
	return output;
}

// Creating an Array given the Index size and the size of the type
Array *CreateArray(Index *externalMaxIndex, int wordsize) {
	Array *output = (Array *)calloc(1, sizeof(Array));
	Index *maxIndex = CreateIndex(externalMaxIndex->indices, externalMaxIndex->numberOfDims);
	int entries = CalculateArrayOffset(externalMaxIndex, externalMaxIndex);

	output->arr = calloc(entries, wordsize);
	output->wordsize = wordsize;
	output->maxIndex = maxIndex;
	return output;
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




//************Helper Functions***********//
//Test the creating of Array - does nothing but running the actual code atm
void TestArrayCreate() {
	int numbDims = 3;
	int inputIndices[] = { 3,5,7 };

	printf("Beginning Array Create Test\r\n");
	Index *maxindex = CreateIndex(inputIndices, numbDims);
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

	Index actualIndex;
	actualIndex.indices = actual;
	actualIndex.numberOfDims = 3;

	Index maxIndex;
	maxIndex.indices = max;
	maxIndex.numberOfDims = 3;

	int offset = CalculateArrayOffset(&actualIndex, &maxIndex);

	if (expectedOffset != offset) {
		printf("ERROR: Test with %d, %d, %d, %d, %d, %d failed\r\nExpected offset was %d, calculated was %d\r\n", a1, a2, a3, m1, m2, m3, expectedOffset, offset);
	}
	return;
}


void DebugPrintIndex(Index *index) {
	int i;
	printf("Number of dims: %d\n", index->numberOfDims);
	for (i = 0; i < index->numberOfDims; i++) {
		printf("The index of slot %d is: %d\n", i, index->indices[i]);
	}
}

void DebugPrintArray(Array *array) {
	printf("The data starts at: %d\n", array->arr);
	printf("The wordsize is: %d\n", array->wordsize);
	DebugPrintIndex(array->maxIndex);
}



void Throw(char* message) {
	printf("Program ended unexpectedly:\r\n%s\r\n", message);
	exit(1);
}