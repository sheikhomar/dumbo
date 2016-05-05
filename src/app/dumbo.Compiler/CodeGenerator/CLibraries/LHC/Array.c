#include <stdlib.h>
#include <stdio.h>

typedef struct Index {
	int *indices;
	int numberOfDims;
} Index;

typedef struct Array {
	void *arr;
	int wordsize;
	Index *maxIndex;
} Array;

Index *CreateIndex(int *indices, int numberOfDims);
Array *CreateArray(Index *maxIndex, int wordsize);
void PrintIndex(Index *index);
void PrintArray(Array *array);

int CalculateArrayOffset(int wordsize, Index actualIndex, Index maxIndex);

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
	return 0;
}

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