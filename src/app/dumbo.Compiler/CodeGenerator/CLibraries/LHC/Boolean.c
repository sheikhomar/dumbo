#include <stdio.h>
#include <stdlib.h>

//LHC Type
typedef enum {false, true} Boolean;

//LHC HelperFunctions
void BooleanPrint(Boolean *input);
Boolean CreateTrueBoolean();
Boolean CreateFalseBoolean();


int main()
{
	Boolean boolT = CreateTrueBoolean();
	Boolean boolF = CreateFalseBoolean();
	
	//Print, and or not
	
	//And test
	printf("And test | tt,ff,tf, ft\n");
	Boolean a1 = boolT && boolT;
	Boolean a2 = boolF && boolF;
	Boolean a3 = boolT && boolF;
	Boolean a4 = boolF && boolT;
	
	BooleanPrint(&a1); printf("  | true\n");
	BooleanPrint(&a2); printf(" | flase\n");
	BooleanPrint(&a3); printf(" | false\n");
	BooleanPrint(&a4); printf(" | false\n");
	
	
	//Or test
	printf("\n");
	printf("Or test | tt,ff,tf, ft\n");
	Boolean o1 = boolT || boolT;
	Boolean o2 = boolF || boolF;
	Boolean o3 = boolT || boolF;
	Boolean o4 = boolF || boolT;
	
	BooleanPrint(&o1); printf("  | true\n");
	BooleanPrint(&o2); printf(" | false\n");
	BooleanPrint(&o3); printf("  | true\n");
	BooleanPrint(&o4); printf("  | true\n");
	
	
	//Not test
	printf("\n");
	printf("Not test | t, f\n");
	Boolean n1 = ! boolT;
	Boolean n2 = ! boolF;

	BooleanPrint(&n1); printf(" | false\n");
	BooleanPrint(&n2); printf("  | true\n");
	
    return 0;
}


Boolean CreateTrueBoolean(){
	Boolean ret = true;
	return ret;
}

Boolean CreateFalseBoolean(){
	Boolean ret = false;
	return ret;
}


void BooleanPrint(Boolean *input){
	if((*input) == true)
		printf("true");
	else
		printf("false");
}