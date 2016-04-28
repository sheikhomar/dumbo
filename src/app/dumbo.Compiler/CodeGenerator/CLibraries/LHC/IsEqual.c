/********************************************************
Function:	IsEqual									
Version: 	v1.0 		
Uses:		Text, Boolean					
/********************************************************/
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

//LHC Type
typedef struct Text{
    int Length;
    char *Text;
} Text;

//LHC Type
typedef enum { false, true } Boolean;

Text *CreateText(char *inputText)
{
    Text *retVal = (Text*)malloc(sizeof(Text));
    retVal->Text = inputText;
    retVal->Length = strlen(inputText);
    return retVal;
}

Boolean IsEqual(Text *t1, Text *t2)
{
    if (t1->Length != t2->Length)
        return false;

    return strcmp(t1->Text, t2->Text) == 0;
}

char *TestEqual(char *t1, char *t2) 
{
    Boolean q = IsEqual(CreateText(t1), CreateText(t2));
    return q == 1 ? "true" : "false";
}

int main()
{
    printf("'a'='a': %s  \n", TestEqual("a", "a"));
    printf("'a'='b': %s  \n", TestEqual("a", "b"));
    printf("'ab'='b': %s  \n", TestEqual("ab", "b"));
    printf("'bb'='bb': %s  \n", TestEqual("bb", "bb"));
	return 1;
}