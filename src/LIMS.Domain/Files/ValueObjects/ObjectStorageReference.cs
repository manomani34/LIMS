using LIMS.Domain.Common;

namespace LIMS.Domain.Files.ValueObjects;

public sealed class ObjectStorageReference : ValueObject
{
    public string Provider { get; }
    public string Bucket { get; }
    public string ObjectKey { get; }

    private ObjectStorageReference(
        string provider,
        string bucket,
        string objectKey)
    {
        Provider = provider;
        Bucket = bucket;
        ObjectKey = objectKey;
    }

    public static ObjectStorageReference Create(
        string provider,
        string bucket,
        string objectKey)
    {
        if (string.IsNullOrWhiteSpace(provider))
            throw new FileDomainException("Storage provider is required.");

        if (string.IsNullOrWhiteSpace(bucket))
            throw new FileDomainException("Storage bucket is required.");

        if (string.IsNullOrWhiteSpace(objectKey))
            throw new FileDomainException("Storage object key is required.");

        if (provider.Length > 100)
            throw new FileDomainException("Storage provider cannot exceed 100 characters.");

        if (bucket.Length > 255)
            throw new FileDomainException("Storage bucket cannot exceed 255 characters.");

        if (objectKey.Length > 1000)
            throw new FileDomainException("Storage object key cannot exceed 1000 characters.");

        return new ObjectStorageReference(
            provider.Trim(),
            bucket.Trim(),
            objectKey.Trim());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Provider;
        yield return Bucket;
        yield return ObjectKey;
    }
}
