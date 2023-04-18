using AutoFixture;
using Testing;

namespace Showtime.Domain.Testing;
public class RegisterShowBuilder
{
    private readonly Fixture _fixture = CustomFixture.Create();

    public RegisterShowBuilder()
    {
        Id = _fixture.Create<int>();
        Name = _fixture.Create<string>();
        Language = _fixture.Create<string>();
        Premiered = DateOnly.FromDateTime(DateTime.Now);
        Genres = new List<string>().AsReadOnly();
    }

    private int Id;
    private string Name { get; init; }
    private string Language { get; init; }
    private DateOnly? Premiered { get; init; }
    private IReadOnlyCollection<string> Genres { get; init; }

    public RegisterShow Build()
    {
        return new RegisterShow()
        {
            Id = this.Id,
            Name = this.Name,
            Language = this.Language,
            Genres = this.Genres,
            Premiered = this.Premiered,
            Summary = null
        };
    }

}
