dotnet build
printf "\n\x1b[36m------- Polo QBE IL Output -------\x1b[0m\n"
dotnet run --no-build -- --compile Samples/condition.mt |& tee test.ssa
printf "\n\x1b[36m-------     ASM Output     -------\x1b[0m\n"
qbe -o - test.ssa |& tee test.s
cc test.s -o test
./test