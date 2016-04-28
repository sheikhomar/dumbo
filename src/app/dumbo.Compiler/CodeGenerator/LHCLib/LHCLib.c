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

typedef enum { false, true } Boolean;

/********************************************************
Function Declarations																
/********************************************************/
//LHC HelperFunctions
void throw(char* message);

void TextPrint(Text *input);
void UpdateText(Text *sourceText, Text *destText);
Text *ConcatText(Text *inputText1, Text *inputText2);
void RemoveText(Text *input);
void RemoveTextValue(Text *input);
Text *CreateText(char *input);
void CopyToText(char *inputText, int length, Text *destText);

void BooleanPrint(Boolean *input);

Text* ConcatTextAndBoolean(Text *text, Boolean boolean);
Text* ConcatTextAndNumber(Text *text, double number);

// Built-in functions
Text* ReadText();
double ReadNumber();
void Write(Text *input);
double Ceiling(double input);
double Floor(double input);
Boolean IsEqual(Text *t1, Text *t2);
double random(double range_lower, double range_upper);
double modulo(double n, double d);
double Div(double n, double d);

/********************************************************
Function:	Text									
Version: 	v1.2 (with more mem leaks) 							
/********************************************************/
//Prints a Text's content
void TextPrint(Text *input) {
	printf("%s\r\n",input->Value);
}

//Creates a new Text Structure and 
Text *CreateText(char *input){
	Text *newText = (Text*)calloc(1,sizeof(Text));
	
	(*newText).Length = 0;
	(*newText).Value = "";
	
	CopyToText(input, strlen(input),newText);
	
	return newText;
}

//Updates a Text with the content of another Text
void UpdateText(Text *sourceText, Text *destText){
	CopyToText(sourceText->Value, sourceText->Length, destText);
}

//Copies a char * content to a given Text
void CopyToText(char *inputText, int length, Text *destText) {
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
	return ;
	
	if (input != NULL) {
		RemoveTextValue(input);
		(*input).Value = NULL;
		free(input);
	}
}

//Removes a given Text's value
void RemoveTextValue(Text *input) {
	return ;
	
	if (input != NULL) {
		free((*input).Value);
		(*input).Value = NULL;
	}
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
Version: 	v1.0 							
/********************************************************/
// From: http://stackoverflow.com/questions/314401/how-to-read-a-line-from-the-console-in-c
char * getline(void) {
    char * line = malloc(100), *linep = line;
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
            char * linen = realloc(linep, lenmax *= 2);

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
    char* text = getline();
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
double random(double range1, double range2)
{
	double number,range_lower,range_upper;
	double range;

	//Find upper and lower
	if(range1 > range2)
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
	if(range < 1)
		throw("Random's range must use two DIFFERNT Numbers.");

	if(range > 32767)
		throw("Random's actual range must be less than 32768. The actual range is argument2 - argument1");

	
	//Calculate the random number and fit to range
	number = modulo(((double)rand()),(range+1)); // 1 % 2 =0,1 in a range 0..1
	
	//Return the random number, adding range offset from 0
	return number + range_lower;
}
/********************************************************
Function:	Modulo									
Version: 	v1.0 							
/********************************************************/
double modulo(double n, double d)
{	
	if(d == 0)
		throw("Cannot do modulo when 2nd argument is zero.");
	
	return n - d*(floor((n/d)));
}

/********************************************************
Function:	Modulo
Version: 	v1.0
/********************************************************/
double Div(double n, double d)
{
	if (d == 0)
		throw("Cannot divide by zero.");

	return n / d;
}

/********************************************************
Function:	Throw									
Version: 	v1.0 							
/********************************************************/
void throw(char* message){
	printf("Program ended unexpectedly:\r\n%s\r\n",message);
	exit(1);
}

/********************************************************
User area:
/********************************************************/
