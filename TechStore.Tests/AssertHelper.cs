using TechStore.Contracts.DTOs;

namespace TechStore.Tests;

public static class AssertHelper
{
    public static void Equal(
        CategoryDto expected, CategoryDto actual, bool ignoreId = false)
    {
        if (!ignoreId)
            Assert.Equal(expected.Id, actual.Id);
        Assert.Equal(expected.Key, actual.Key);
        Assert.Equal(expected.DisplayName, actual.DisplayName);
    }
}
