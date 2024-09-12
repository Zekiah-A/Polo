#!/bin/bash
printf "\x1b[33mUsage tip:\x1b[0m\n"
printf "\x1b[36mRun the script with the desired sample name as the first argument (default is 'operators').\x1b[0m\n"

sample_name="${1:-operators}"

dotnet build
printf "\n\x1b[36m------- Polo QBE IL Output -------\x1b[0m\n"
dotnet run  -- compile --file Samples/"$sample_name".mt 2>&1 > >(tee test.ssa)
if qbe_output=$(qbe -o - test.ssa); then
    printf "\n\x1b[36m-------   QBE ASM Output   -------\x1b[0m\n"
    printf "%s" "$qbe_output" | tee test.s
    printf "\n\x1b[36m-------  Assembler Output  -------\x1b[0m\n"
    cc test.s -o test
    printf "\n\x1b[36m-------   Program output   -------\x1b[0m\n"
    ./test
else
    printf "\x1b[31mQBE IL compilation failed. Exiting.\x1b[0m\n"
    exit 1
fi