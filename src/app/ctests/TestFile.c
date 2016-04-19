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
  
	printf("f32_add: %d and %d = %d\n", a.v, b.v, result.v);
	
	result = f32_sub(a, b);
	printf("f32_sub: %d and %d = %d\n", a, b, result);
	
	result = f32_mul(a, b);
	printf("f32_mul: %d and %d = %d\n", a, b, result);
	
	result = f32_mulAdd(a, b, a);
	printf("f32_mulAdd: %d and %d = %d\n", a, b, result);
	
	result = f32_div(a, b);
	printf("f32_div: %d and %d = %d\n", a, b, result);
	
	printf("Yay, success!");

    return 0;
}
