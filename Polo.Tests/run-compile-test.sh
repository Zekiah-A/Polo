#!/bin/bash
printf "\n\x1b[33mUsage tip:\x1b[0m\n"
printf "\x1b[36mRun the script with the desired sample name as the first argument (default is 'add').\x1b[0m\n"

sample_name="${1:-add}"
dotnet build
printf "\n\x1b[36m------- Polo QBE IL Output -------\x1b[0m\n"
dotnet run --no-build -- --compile Samples/"$sample_name".mt |& tee test.ssa
printf "\n\x1b[36m-------     ASM Output     -------\x1b[0m\n"
qbe -o - test.ssa |& tee test.s
cc test.s -o test
printf "\n\x1b[36m-------   Program output   -------\x1b[0m\n"
./test