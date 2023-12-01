#include "./generated/computer.h"
#include <stdio.h>

int32_t exports_example_calculator_operations_add(int32_t left, int32_t right) {
    return left+right+1;
}

void exports_example_calculator_operations_to_upper(computer_string_t *input, computer_string_t *ret) {

}
