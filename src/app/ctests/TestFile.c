#include <stdio.h>
//Try include this first if a lib is missing
#include <stdint.h>
#include <stdbool.h>
//Try include this second if a lib is missing
#include <internals.h>
#include <primitives.h>
#include <primitiveTypes.h>
#include <softfloat_types.h>
#include <softfloat.h>

int main()
{
  float32_t a, b, result;
  a.v = 10.0;
  b.v = 5.0;
  result.v = 0.0;
  
	
  printf("Hello, World!\n");
	
	result = f32_add(a, b);
  
	printf("Add %f and %f = %f", a.v, b.v, result.v);
	//
	//result = f32_sub( float32_t a, float32_t b);
	////printf("Add %f and %f = %f", a, b, result);
	//
	//result = f32_mul( float32_t a, float32_t b);
	////printf("Add %f and %f = %f", a, b, result);
	//
	//result = f32_mulAdd( float32_t a, float32_t b, float32_t a);
	////printf("Add %f and %f = %f", a, b, result);
	//
	//result = f32_div( float32_t a, float32_t b);
	////printf("Add %f and %f = %f", a, b, result);
	//
	//printf("Yay, success!");

    return 0;
}
