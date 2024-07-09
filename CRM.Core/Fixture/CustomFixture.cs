using AutoFixture;

namespace CRM.Core.Fixture;

public class CustomFixture
{
    public const string RegexGuid = @"^[0-9A-Fa-f]{8}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{12}$";
    private readonly IFixture _fixture = new AutoFixture.Fixture()
        .Customize(new CompositeCustomization(new DateOnlyFixtureCustomization()));

    public IFixture GetFixture()
    {
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        
        return _fixture;
    }
}