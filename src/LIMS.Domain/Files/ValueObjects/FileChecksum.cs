using LIMS.Domain.Common;

namespace LIMS.Domain.Files.ValueObjects;

public sealed class FileChecksum : ValueObject
{
    public string Algorithm { get; }
    public string Value { get; }

    private FileChecksum(string algorithm, string value)
    {
        Algorithm = algorithm;
        Value = value;
    }

    public static FileChecksum CreateSha256(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new FileDomainException("SHA-256 checksum is required.");

        var normalized = value.Trim().ToLowerInvariant();

        if (normalized.Length != 64 || normalized.Any(c => !Uri.IsHexDigit(c)))
            throw new FileDomainException("SHA-256 checksum must contain exactly 64 hexadecimal characters.");

        return new FileChecksum("SHA256", normalized);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Algorithm;
        yield return Value;
    }
}
