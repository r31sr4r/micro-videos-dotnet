using Bogus;
using Xunit;
using FC.Codeflix.Catalog.Domain.Validation;
using FluentAssertions;
using FC.Codeflix.Catalog.Domain.Exceptions;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Validation;
public class DomainValidationTest
{
    private Faker Faker { get; set; } = new Faker();

    [Fact(DisplayName = nameof(NotNullOk))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void NotNullOk()
    {
        var value = Faker.Commerce.ProductName();

        Action action = () => DomainValidation.NotNull(value, "Value");

        action.Should().NotThrow();

    }

    [Fact(DisplayName = nameof(NotNullThrowWhenNull))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void NotNullThrowWhenNull()
    {
        string? value = null;
        string fieldName = Faker.Commerce.ProductName().Replace(" ", "");

        Action action = () => DomainValidation.NotNull(value, fieldName);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage($"{fieldName} cannot be null");
    }

    [Theory(DisplayName = nameof(NotNullOrEmptyThrowWhenEmpty))]
    [Trait("Domain", "DomainValidation - Validation")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void NotNullOrEmptyThrowWhenEmpty(string? target)
    {
        string fieldName = Faker.Commerce.ProductName().Replace(" ", "");

        Action action =
            () => DomainValidation.NotNullOrEmpty(target, fieldName);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage($"{fieldName} cannot be empty or null");
    }

    [Fact(DisplayName = nameof(NotNullOrEmptyOk))]
    [Trait("Domain", "DomainValidation - Validation")]
    public void NotNullOrEmptyOk()
    {
        string fieldName = Faker.Commerce.ProductName().Replace(" ", "");
        var target = Faker.Commerce.ProductName();

        Action action =
            () => DomainValidation.NotNullOrEmpty(target, fieldName);

        action.Should().NotThrow();
    }

    [Theory(DisplayName = nameof(MinLenghtThrowWhenLess))]
    [Trait("Domain", "DomainValidation - Validation")]
    [MemberData(nameof(GetValuesSmallerThanMin), parameters: 10)]
    public void MinLenghtThrowWhenLess(string target, int minLenght)
    {
        string fieldName = Faker.Commerce.ProductName().Replace(" ", "");

        Action action =
            () => DomainValidation.MinLenght(target, minLenght, fieldName);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage($"{fieldName} should be at least {minLenght} characters long");
    }

    public static IEnumerable<object[]> GetValuesSmallerThanMin(int numerOfTests = 5)
    {
        yield return new object[] { "123456", 10 };
        var faker = new Faker();
        for (int i = 0; i < (numerOfTests - 1); i++)
        {
            var example = faker.Commerce.ProductName();
            var minLenght = example.Length + (new Random().Next(1, 20));
            yield return new object[] { example, minLenght };
        }
    }

    [Theory(DisplayName = nameof(MinLenghtOk))]
    [Trait("Domain", "DomainValidation - Validation")]
    [MemberData(nameof(GetValuesGreaterThanMin), parameters: 10)]
    public void MinLenghtOk(string target, int minLenght)
    {
        string fieldName = Faker.Commerce.ProductName().Replace(" ", "");

        Action action =
            () => DomainValidation.MinLenght(target, minLenght, fieldName);

        action.Should().NotThrow();            
    }

    public static IEnumerable<object[]> GetValuesGreaterThanMin(int numerOfTests = 5)
    {
        yield return new object[] { "123456", 6 };
        var faker = new Faker();
        for (int i = 0; i < (numerOfTests - 1); i++)
        {
            var example = faker.Commerce.ProductName();
            var minLenght = example.Length - (new Random().Next(1, example.Length - 1));
            yield return new object[] { example, minLenght };
        }
    }

    [Theory(DisplayName = nameof(MaxLenghtThrowWhenGreater))]
    [Trait("Domain", "DomainValidation - Validation")]
    [MemberData(nameof(GetValuesGreaterThanMax), parameters: 10)]
    public void MaxLenghtThrowWhenGreater(string target, int maxLenght)
    {
        string fieldName = Faker.Commerce.ProductName().Replace(" ", "");

        Action action =
            () => DomainValidation.MaxLenght(target, maxLenght, fieldName);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage($"{fieldName} should not be greater than {maxLenght} characters long");
    }

    public static IEnumerable<object[]> GetValuesGreaterThanMax(int numerOfTests = 5)
    {
        yield return new object[] { "123456", 5 };
        var faker = new Faker();
        for (int i = 0; i < (numerOfTests - 1); i++)
        {
            var example = faker.Commerce.ProductName();
            var maxLenght = example.Length - (new Random().Next(1, 5));
            yield return new object[] { example, maxLenght };
        }
    }

    [Theory(DisplayName = nameof(MaxLenghtOk))]
    [Trait("Domain", "DomainValidation - Validation")]
    [MemberData(nameof(GetValuesLessThanMax), parameters: 10)]
    public void MaxLenghtOk(string target, int maxLenght)
    {
        string fieldName = Faker.Commerce.ProductName().Replace(" ", "");

        Action action =
            () => DomainValidation.MaxLenght(target, maxLenght, fieldName);

        action.Should().NotThrow();            
    }

    public static IEnumerable<object[]> GetValuesLessThanMax(int numerOfTests = 5)
    {
        yield return new object[] { "123456", 6 };
        var faker = new Faker();
        for (int i = 0; i < (numerOfTests - 1); i++)
        {
            var example = faker.Commerce.ProductName();
            var maxLenght = example.Length + (new Random().Next(0, 5));
            yield return new object[] { example, maxLenght };
        }
    }


}
