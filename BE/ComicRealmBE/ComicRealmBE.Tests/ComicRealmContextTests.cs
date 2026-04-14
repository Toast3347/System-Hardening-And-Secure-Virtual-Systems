using ComicRealmBE.Data;
using ComicRealmBE.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ComicRealmBE.Tests.Data;

public class ComicRealmContextTests
{
    private static ComicRealmContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ComicRealmContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ComicRealmContext(options);
    }

    [Fact]
    public void UserEntity_ShouldMapToUsersTable()
    {
        using var context = CreateContext();

        var entityType = context.Model.FindEntityType(typeof(User));

        entityType.Should().NotBeNull();
        entityType!.GetTableName().Should().Be("users");
    }

    [Fact]
    public void ComicEntity_ShouldMapToComicsTable()
    {
        using var context = CreateContext();

        var entityType = context.Model.FindEntityType(typeof(Comic));

        entityType.Should().NotBeNull();
        entityType!.GetTableName().Should().Be("comics");
    }

    [Fact]
    public void UserEntity_ShouldHaveUniqueIndexOnEmail()
    {
        using var context = CreateContext();

        var entityType = context.Model.FindEntityType(typeof(User))!;
        var emailIndex = entityType.GetIndexes()
            .SingleOrDefault(i => i.Properties.Select(p => p.Name).SequenceEqual(new[] { nameof(User.Email) }));

        emailIndex.Should().NotBeNull();
        emailIndex!.IsUnique.Should().BeTrue();
    }

    [Fact]
    public void UserEntity_ShouldHaveIndexOnIsActive()
    {
        using var context = CreateContext();

        var entityType = context.Model.FindEntityType(typeof(User))!;
        var isActiveIndex = entityType.GetIndexes()
            .SingleOrDefault(i => i.Properties.Select(p => p.Name).SequenceEqual(new[] { nameof(User.IsActive) }));

        isActiveIndex.Should().NotBeNull();
    }

    [Fact]
    public void ComicEntity_ShouldHaveCompositeIndexOnSerieAndNumber()
    {
        using var context = CreateContext();

        var entityType = context.Model.FindEntityType(typeof(Comic))!;
        var index = entityType.GetIndexes()
            .SingleOrDefault(i => i.Properties.Select(p => p.Name)
                .SequenceEqual(new[] { nameof(Comic.Serie), nameof(Comic.Number) }));

        index.Should().NotBeNull();
    }

    [Fact]
    public void ComicCreatedByRelationship_ShouldBeRequired_AndRestrictDelete()
    {
        using var context = CreateContext();

        var entityType = context.Model.FindEntityType(typeof(Comic))!;
        var fk = entityType.GetForeignKeys()
            .Single(f => f.Properties.Any(p => p.Name == nameof(Comic.CreatedBy)));

        fk.PrincipalEntityType.ClrType.Should().Be(typeof(User));
        fk.IsRequired.Should().BeTrue();
        fk.DeleteBehavior.Should().Be(DeleteBehavior.Restrict);
    }

    [Fact]
    public void UserSelfReferenceRelationship_ShouldSetNullOnDelete()
    {
        using var context = CreateContext();

        var entityType = context.Model.FindEntityType(typeof(User))!;
        var fk = entityType.GetForeignKeys()
            .Single(f => f.Properties.Any(p => p.Name == nameof(User.CreatedBy)));

        fk.PrincipalEntityType.ClrType.Should().Be(typeof(User));
        fk.IsRequired.Should().BeFalse();
        fk.DeleteBehavior.Should().Be(DeleteBehavior.SetNull);
    }
}