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
void PrintIndex(Index *index);
void PrintArray(Array *array);
int RecCalculateArrayOffset(Index *actualIndex, Index *maxIndex, int currentIndex);
int CalculateArrayOffset(Index *actualIndex, Index *maxIndex);

//HelperFunctions
void TestArrayOffsetStart();
void TestArrayOffset(int a1, int a2, int a3, int m1, int m2, int m3, int expectedOffset);
void Throw(char* message);

int main(){
	int numbDims = 3;
	int *inputIndices = malloc(sizeof(int)*numbDims);
	inputIndices[0] = 3;
	inputIndices[1] = 5;
	inputIndices[2] = 7;
	
	Index *maxindex = CreateIndex(inputIndices, numbDims);
	PrintIndex(maxindex);
	printf("\n");
	
	Array *a = CreateArray(maxindex, sizeof(double));
	PrintArray(a);
	
	printf("I made it all the way down here!\n");
	
	TestArrayOffsetStart();
	
	return 0;
}

//Array Functions//
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




//************Helper Functions***********//
void TestArrayOffsetStart(){
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

	return ;
}

//Calculates the offset on a three dimentional array based on 3 x actual and max index values
void TestArrayOffset(int a1, int a2, int a3, int m1, int m2, int m3, int expectedOffset){
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

void Throw(char* message) {
	printf("Program ended unexpectedly:\r\n%s\r\n", message);
	exit(1);
}