using AutoFixture;

namespace Testing;
public class CustomFixture
{
    public static Fixture Create()
    {
        var fixture = new Fixture();
        fixture.Customize<DateOnly>(composer => composer.FromFactory<DateTime>(DateOnly.FromDateTime));
        return fixture;
    }
}