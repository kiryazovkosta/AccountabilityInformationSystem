namespace AccountabilityInformationSystem.ConsoleSeeder;

internal class HttpClientException : Exception
{
    public HttpClientException()
    {
    }

    public HttpClientException(string? message) : base(message)
    {
    }
}
