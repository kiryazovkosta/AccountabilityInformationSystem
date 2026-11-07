using AccountabilityInformationSystem.Api.Shared.Services.Tokenizing;

namespace AccountabilityInformationSystem.UnitTests.Services;

public sealed class RefreshTokenHasherTests
{
    [Fact]
    public void Hash_ReturnsKnownSha256UppercaseHex()
    {
        // SHA-256("abc") — NIST test vector
        string hash = RefreshTokenHasher.Hash("abc");

        Assert.Equal("BA7816BF8F01CFEA414140DE5DAE2223B00361A396177A9CB410FF61F20015AD", hash);
    }

    [Fact]
    public void Hash_IsDeterministic()
    {
        const string token = "dGVzdC1yZWZyZXNoLXRva2VuLXZhbHVl";

        Assert.Equal(RefreshTokenHasher.Hash(token), RefreshTokenHasher.Hash(token));
    }

    [Fact]
    public void Hash_DiffersFromInputAndHas64Chars()
    {
        const string token = "dGVzdC1yZWZyZXNoLXRva2VuLXZhbHVl";

        string hash = RefreshTokenHasher.Hash(token);

        Assert.NotEqual(token, hash);
        Assert.Equal(64, hash.Length);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Hash_ThrowsOnEmptyInput(string token)
    {
        Assert.Throws<ArgumentException>(() => RefreshTokenHasher.Hash(token));
    }
}
