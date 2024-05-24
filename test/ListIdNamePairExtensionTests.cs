using Soenneker.Tests.FixturedUnit;
using Xunit;
using Xunit.Abstractions;

namespace Soenneker.Extensions.List.IdNamePair.Tests;

[Collection("Collection")]
public class ListIdNamePairExtensionTests : FixturedUnitTest
{
    public ListIdNamePairExtensionTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
    }
}
