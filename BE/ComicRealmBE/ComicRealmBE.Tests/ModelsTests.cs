using System.ComponentModel.DataAnnotations;
using ComicRealmBE.Models;
using ComicRealmBE.Models.Enums;
using FluentAssertions;

namespace ComicRealmBE.Tests.Models;

public class ModelValidationTests
{
    private static IList<ValidationResult> Validate(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model, null, null);

        Validator.TryValidateObject(model, context, results, validateAllProperties: true);

        return results;
    }

    [Fact]
    public void Comic_ShouldBeValid_WhenRequiredFieldsAreSet()
    {
        var comic = new Comic
        {
            ComicId = 1,
            Serie = "Batman",
            Number = "1",
            Title = "Year One",
            CreatedBy = 1
        };

        var results = Validate(comic);

        results.Should().BeEmpty();
    }

    [Fact]
    public void Comic_ShouldBeInvalid_WhenSerieMissing()
    {
        var comic = new Comic
        {
            Number = "1",
            Title = "Year One",
            CreatedBy = 1
        };

        var results = Validate(comic);

        results.Should().Contain(r => r.MemberNames.Contains(nameof(Comic.Serie)));
    }

    [Fact]
    public void Comic_ShouldBeInvalid_WhenTitleTooLong()
    {
        var comic = new Comic
        {
            Serie = "Batman",
            Number = "1",
            Title = new string('A', 501),
            CreatedBy = 1
        };

        var results = Validate(comic);

        results.Should().Contain(r => r.MemberNames.Contains(nameof(Comic.Title)));
    }

    [Fact]
    public void User_ShouldBeValid_WhenRequiredFieldsAreSet()
    {
        var user = new User
        {
            UserId = 1,
            Email = "admin@example.com",
            PasswordHash = "hash",
            Role = UserRole.Admin,
            IsActive = true
        };

        var results = Validate(user);

        results.Should().BeEmpty();
    }

    [Fact]
    public void User_ShouldBeInvalid_WhenEmailIsMalformed()
    {
        var user = new User
        {
            Email = "superadmin@comicreal m.local",
            PasswordHash = "hash",
            Role = UserRole.SuperAdmin
        };

        var results = Validate(user);

        results.Should().Contain(r => r.MemberNames.Contains(nameof(User.Email)));
    }

    [Fact]
    public void User_ShouldBeInvalid_WhenPasswordHashMissing()
    {
        var user = new User
        {
            Email = "friend@example.com",
            Role = UserRole.Friend
        };

        var results = Validate(user);

        results.Should().Contain(r => r.MemberNames.Contains(nameof(User.PasswordHash)));
    }
}