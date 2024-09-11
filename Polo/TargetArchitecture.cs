namespace Polo;

public record TargetArchitecture(string Name, bool QbeSupport, int CharSize, int IntSize,
    int FloatSize, int PointerSize, bool IsLittleEndian)
{
    public static TargetArchitecture Amd64 =
        new("x86_64", true, 1, 4, 4, 8, true);
    public static TargetArchitecture Arm64 =
        new("aarch64", true, 1, 4, 4, 8, true);
    public static TargetArchitecture RiscV64 =
        new("riscv64", true, 1, 4, 4, 8, true);
}
