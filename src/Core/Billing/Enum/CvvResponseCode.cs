namespace Core.Billing.Enum
{
    public enum CvvResponseCode
    {
        SuccessfullyMatch = 'M',
        DoesNotMatch = 'N',
        NotProcessed = 'P',
        ShouldBeOnCardButNotIndicated = 'S',
        IssuerNotCertifiedOrNotProvidedEncryptionKey = 'U'
    }
}
