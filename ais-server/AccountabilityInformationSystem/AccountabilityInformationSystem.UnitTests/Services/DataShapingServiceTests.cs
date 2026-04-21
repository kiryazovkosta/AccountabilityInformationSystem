using System.Collections.Generic;
using System.Dynamic;
using AccountabilityInformationSystem.Api.Shared.Services.DataShaping;
using Microsoft.Extensions.Time.Testing;

namespace AccountabilityInformationSystem.UnitTests.Services;

public sealed class DataShapingServiceTests
{
    private sealed class TestObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    private static readonly DateTimeOffset FrozenTime = new(2025, 1, 1, 12, 0, 0, TimeSpan.Zero);

    private readonly TestObject _testObject;
    private readonly DataShapingService _service = new();

    public DataShapingServiceTests()
    {
        FakeTimeProvider timeProvider = new(FrozenTime);
        _testObject = new()
        {
            Id = 1,
            Name = "Test",
            Value = 2.5M,
            CreatedAt = timeProvider.GetUtcNow().UtcDateTime,
            IsActive = true
        };
    }

    [Fact]
    public void ShapeData_ShouldReturnAllProperties_WhenFieldsIsNull()
    {
        ExpandoObject result = _service.ShapeData(_testObject, null);

        IDictionary<string, object?> dict = result;
        Assert.Equal(1, dict["Id"]);
        Assert.Equal("Test", dict["Name"]);
        Assert.Equal(FrozenTime.UtcDateTime, dict["CreatedAt"]);
        Assert.Equal(2.5M, dict["Value"]);
        Assert.True((bool)dict["IsActive"]!);
    }

    [Fact]
    public void ShapeData_ShouldReturnOnlyRequestedFields_WhenFieldsAreSpecified()
    {
        ExpandoObject result = _service.ShapeData(_testObject, "Name,Value");

        IDictionary<string, object?> dict = result;
        Assert.True(dict.ContainsKey("Id"));
        Assert.True(dict.ContainsKey("Name"));
        Assert.True(dict.ContainsKey("Value"));
        Assert.False(dict.ContainsKey("CreatedAt"));
        Assert.False(dict.ContainsKey("IsActive"));
    }
}
