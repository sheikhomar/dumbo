#include <stdio.h>
#include <stdlib.h>

//Single Return Function | Number MyFunction(Number a)
int MyFunction(void *for1);
//Multiple Return Function | Number, Text MyFunction(Text a)
void MyFunctionMul(void *for1, void *ret1,  void *ret2);

int main()
{
	int myInt = 5;
	int myInt2 = 7;
	char myChar = 'a';
	
	//Single Ret Function
	printf("Before call int is %d\n",myInt);
	printf("(%d + 42 = %d)\n",myInt,MyFunction(&myInt)+5);
	printf("After call int is %d || no change\n",myInt);
	printf("\n");
	
	//Multiple Ret Function call
	myInt = 5;
	printf("Before call int is %d and %d, char is %c\n",myInt,myInt2,myChar);
	
	//Function call
	{
		char *for1 = &myChar;
		int *ret1 = &myInt;
		int *ret2 = &myInt2;
		MyFunctionMul(for1,ret1,ret2);
	}
	
	printf("After call int is %d and %d, char is %c || int changed, char same\n",myInt,myInt2,myChar);
	
	return 0;
}

//Single return function
int MyFunction(void *for1){
	//Assignment of formal parameters
	int a = (*(int*)for1);

	//Body
	int ret = 42;
	a = 9001;
	
	//Return assignment
		return ret;
}

//Multiple return function
void MyFunctionMul(void *for1, void *ret1,void *ret2){
	//Assignment of formal parameters
	char a = (*(char*)for1);

	//Body
	int retVal = 42;
	int retVal2 = 55;
	a = 'b';
	
	//Return assignment
	{
		*(int*)ret1 = retVal; 
		*(int*)ret2 = retVal2;
		return;
	}
}