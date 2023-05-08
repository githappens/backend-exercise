using Xunit;
using FluentAssertions;
using System.Collections.Generic;

public class ParserTests
{
    private static readonly Book peterJohnson = new Book { Authors = new List<string> { "Peter Johnson" }, Title = "The Year 2020", Publisher = "Penguin", PublicationYear = 2020, NumberOfPages = 250 };
    private static readonly Book aliceSmith = new Book { Authors = new List<string> { "Alice Smith" }, Title = "History of 1920s", Publisher = "HarperCollins", PublicationYear = 2015, NumberOfPages = 350 };
    private static readonly Book peterBrown = new Book { Authors = new List<string> { "Peter Brown" }, Title = "The Modern Era", Publisher = "Random House", PublicationYear = 2018, NumberOfPages = 400 };
    private static readonly Book samuelJohnson = new Book { Authors = new List<string> { "Samuel Johnson" }, Title = "The 20th Century", Publisher = "Simon & Schuster", PublicationYear = 2010, NumberOfPages = 300 };
    private static readonly Book knudRasmussen1 = new Book { Authors = new List<string> { "Knud Rasmussen" }, Title = "Across Arctic America", Publisher = "G.P. Putnam's Sons", PublicationYear = 1927, NumberOfPages = 414 };
    private static readonly Book knudRasmussen2 = new Book { Authors = new List<string> { "Knud Rasmussen" }, Title = "The Netsilik Eskimos", Publisher = "Report of the Fifth Thule Expedition", PublicationYear = 1931, NumberOfPages = 276 };
    private static readonly Book knudRasmussen3 = new Book { Authors = new List<string> { "Knud Rasmussen" }, Title = "Eskimo Folk-Tales", Publisher = "Houghton Mifflin Company", PublicationYear = 1921, NumberOfPages = 295 };
    private static readonly Book knudRasmussen4 = new Book { Authors = new List<string> { "Knud Rasmussen" }, Title = "The Intellectual Culture of the Iglulik Eskimos", Publisher = "Report of the Fifth Thule Expedition", PublicationYear = 1929, NumberOfPages = 350 };
    private static readonly Book knudRasmussen5 = new Book { Authors = new List<string> { "Knud Rasmussen" }, Title = "Observations on the Intellectual Culture of the Caribou Eskimos", Publisher = "Report of the Fifth Thule Expedition", PublicationYear = 1930, NumberOfPages = 364 };
    
    private List<Book> GetTestBooks()
    {
        return new List<Book>
        {
            peterJohnson,
            aliceSmith,
            peterBrown,
            samuelJohnson,
            knudRasmussen1,
            knudRasmussen2,
            knudRasmussen3,
            knudRasmussen4,
            knudRasmussen5
        };
    }

    [Fact]
    public void FindBooks_SearchString20_ShouldReturnBooksContaining20()
    {
        // Arrange
        List<Book> books = GetTestBooks();
        string searchString = "*20*";

        // Act
        List<Book> foundBooks = books.FindBooks(searchString);

        // Assert
        foundBooks.Should().HaveCount(4);
        foundBooks[0].Should().BeEquivalentTo(books[0]);
        foundBooks[1].Should().BeEquivalentTo(books[1]);
        foundBooks[2].Should().BeEquivalentTo(books[2]);
        foundBooks[3].Should().BeEquivalentTo(books[3]);
    }

    [Fact]
    public void FindBooks_SearchString20AndPeter_ShouldReturnBooksSatisfyingBothConditions()
    {
        // Arrange
        List<Book> books = GetTestBooks();
        string searchString = "*20* & *peter*";

        // Act
        List<Book> foundBooks = books.FindBooks(searchString);

        // Assert
        foundBooks.Should().HaveCount(2);
        foundBooks[0].Should().BeEquivalentTo(books[0]);
        foundBooks[1].Should().BeEquivalentTo(books[2]);
    }

    [Fact]
    public void FindBooks_SearchingShouldBeCaseInsensitive()
    {
        // Arrange
        List<Book> books = GetTestBooks();
        string searchString = "*EsKiMo* & *kNuD*";

        // Act
        List<Book> foundBooks = books.FindBooks(searchString);

        // Assert
        foundBooks.Should().HaveCount(4);
        foundBooks[0].Should().BeEquivalentTo(books[5]);
        foundBooks[1].Should().BeEquivalentTo(books[6]);
        foundBooks[2].Should().BeEquivalentTo(books[7]);
        foundBooks[3].Should().BeEquivalentTo(books[8]);
    }

    [Fact]
    public void ReadBooks_ShouldParseBooksCorrectly()
    {
        // Arrange
        string input = @"Book:
Author: Brian Jensen
Title: Texts from Denmark
Publisher: Gyldendal
Published: 2001
NumberOfPages: 253
Book:
Author: Peter Jensen
Author: Hans Andersen
Title: Stories from abroad
Publisher: Borgen
Published: 2012
NumberOfPages: 156";

        // Act
        List<Book> books = Parser.ReadBooks(input);

        // Assert
        books.Should().HaveCount(2);

        books[0].Authors.Should().ContainSingle().Which.Should().Be("Brian Jensen");
        books[0].Title.Should().Be("Texts from Denmark");
        books[0].Publisher.Should().Be("Gyldendal");
        books[0].PublicationYear.Should().Be(2001);
        books[0].NumberOfPages.Should().Be(253);

        books[1].Authors.Should().HaveCount(2);
        books[1].Authors.Should().Contain("Peter Jensen").And.Contain("Hans Andersen");
        books[1].Title.Should().Be("Stories from abroad");
        books[1].Publisher.Should().Be("Borgen");
        books[1].PublicationYear.Should().Be(2012);
        books[1].NumberOfPages.Should().Be(156);
    }

    [Fact]
    public void ReadBooks_ExpectExceptionWhenFieldCanNotBeParsed()
    {
        // Arrange
        string input = @"Book:
SomeWeirdField: Brian Jensen
Title: Texts from Denmark
Publisher: Gyldendal
Published: 2001
NumberOfPages: 253";

        // Act
        System.Exception thrownException = null;
        try
        {
            List<Book> books = Parser.ReadBooks(input);    
        }
        catch (System.Exception ex)
        {
            thrownException = ex;
        }

        // Assert
        thrownException.Should().NotBeNull();
        thrownException.Should().BeOfType<ArgumentException>();
        thrownException.Message.Should().Be("Property with key SomeWeirdField could not be parsed.");
    }

    [Fact]
    public void ReadBooks_ExpectExceptionWhenBookPrefixIsMissing()
    {
        // Arrange
        string input = @"Author: Brian Jensen
Title: Texts from Denmark
Publisher: Gyldendal
Published: 2001
NumberOfPages: 253";

        // Act
        System.Exception thrownException = null;
        try
        {
            List<Book> books = Parser.ReadBooks(input);    
        }
        catch (System.Exception ex)
        {
            thrownException = ex;
        }

        // Assert
        thrownException.Should().NotBeNull();
        thrownException.Should().BeOfType<ArgumentException>();
        thrownException.Message.Should().Be("Each entry should start with \"Book\"!");
    }
}