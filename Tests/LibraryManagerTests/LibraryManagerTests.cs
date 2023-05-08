using Xunit;
using FluentAssertions;
using System.Collections.Generic;

public class LibraryManagerTests
{
    private static List<Book> TestBooks => new List<Book>
    {
        new Book { ISBN = "978-0-1234-5678-1", Title = "Book 1", Authors = new List<string> { "Author A" }, Publisher = "Publisher 1", PublicationYear = 2010, NumberOfPages = 250 },
        new Book { ISBN = "978-0-1234-5678-2", Title = "Book 2", Authors = new List<string> { "Author B" }, Publisher = "Publisher 1", PublicationYear = 2011, NumberOfPages = 260 },
        new Book { ISBN = "978-0-1234-5678-3", Title = "Book 3", Authors = new List<string> { "Author C" }, Publisher = "Publisher 2", PublicationYear = 2012, NumberOfPages = 270 },
        new Book { ISBN = "978-0-1234-5678-4", Title = "Book 4", Authors = new List<string> { "Author D" }, Publisher = "Publisher 2", PublicationYear = 2013, NumberOfPages = 280 }
    };

    private readonly Library _library;

    public LibraryManagerTests()
    {
        _library = new Library();

        _library.Rooms.Add(new Room { RoomID = 1, RoomNumber = 1 });
        _library.Rooms.Add(new Room { RoomID = 2, RoomNumber = 2 });

        _library.Rows.Add(new Row { RowID = 1, RowNumber = 1, RoomID = 1 });
        _library.Rows.Add(new Row { RowID = 2, RowNumber = 2, RoomID = 1 });
        _library.Rows.Add(new Row { RowID = 3, RowNumber = 1, RoomID = 2 });
        _library.Rows.Add(new Row { RowID = 4, RowNumber = 2, RoomID = 2 });

        _library.BookShelves.Add(new BookShelf { ShelfID = 1, ShelfNumber = 1, RowID = 1 });
        _library.BookShelves.Add(new BookShelf { ShelfID = 2, ShelfNumber = 2, RowID = 1 });
        _library.BookShelves.Add(new BookShelf { ShelfID = 3, ShelfNumber = 1, RowID = 2 });
        _library.BookShelves.Add(new BookShelf { ShelfID = 4, ShelfNumber = 2, RowID = 2 });

        foreach (var book in TestBooks)
        {
            int roomNumber = book.PublicationYear % 2 == 0 ? 1 : 2; // book 1 & 3 : 1, book 2 & 4: 2
            int rowNumber = book.NumberOfPages > 250 ? 1 : 2; // book 2,3,4 : 1 : book 1 : 2
            int shelfNumber = book.Publisher == "Publisher 1" ? 1 : 2; // book 1 & 2 : 1, book 3 & 4 : 2

            _library.AddBook(book, roomNumber, rowNumber, shelfNumber);
        }
    }

    [Fact]
    public void FindBookByISBN_FindsCorrectBook()
    {
        // Act
        var foundBook = _library.FindBookByISBN("978-0-1234-5678-3");

        // Assert
        foundBook.Should().NotBeNull();
        foundBook.Title.Should().Be("Book 3");
    }

    [Fact]
    public void GetInventoryList_FilteredByRoomNumber_ReturnsCorrectBooks()
    {
        // Act
        var inventoryList = _library.GetInventoryList(roomNumber: 1);

        // Assert
        inventoryList.Count.Should().Be(2);
        inventoryList.Should().Contain(book => book.Title == "Book 1");
        inventoryList.Should().Contain(book => book.Title == "Book 3");
    }

    [Fact]
    public void GetInventoryList_FilteredByRoomNumber_ReturnsCorrectBooks2()
    {
        // Act
        var inventoryList = _library.GetInventoryList(roomNumber: 2);

        // Assert
        inventoryList.Count.Should().Be(2);
        inventoryList.Should().Contain(book => book.Title == "Book 2");
        inventoryList.Should().Contain(book => book.Title == "Book 4");
    }

    [Fact]
    public void GetInventoryList_FilteredByRowNumber_ReturnsCorrectBooks()
    {
        // Act
        var inventoryList = _library.GetInventoryList(roomNumber: 1, rowNumber: 2);

        // Assert
        inventoryList.Count.Should().Be(1);
        inventoryList.Should().Contain(book => book.Title == "Book 1");
    }

    [Fact]
    public void GetInventoryList_FilteredByShelfNumber_ReturnsCorrectBooks()
    {
        // Act
        var inventoryList = _library.GetInventoryList(roomNumber: 1, rowNumber: 2, shelfNumber: 1);

        // Assert
        inventoryList.Count.Should().Be(1);
        inventoryList.Should().Contain(book => book.Title == "Book 1");
    }

    [Fact]
    public void GetInventoryList_FilteredByRoomNumberAndRowNumber_ReturnsCorrectBooks()
    {
        // Act
        var inventoryList = _library.GetInventoryList(roomNumber: 2, rowNumber: 1);

        // Assert
        inventoryList.Count.Should().Be(2);
        inventoryList.Should().Contain(book => book.Title == "Book 2");
        inventoryList.Should().Contain(book => book.Title == "Book 4");
    }

    [Fact]
    public void GetInventoryList_FilteredByRoomNumberAndShelfNumber_ThrowsException()
    {
        // Arrange 
        Exception thrownException = null;
        
        // Act
        try
        {
            var inventoryList = _library.GetInventoryList(roomNumber: 1, shelfNumber: 2);
        }
        catch(Exception ex)
        {
            thrownException = ex;
        }
        
        // Assert
        thrownException.Should().NotBeNull();
        thrownException.Should().BeOfType<ArgumentException>();
        thrownException.Message.Should().Be("When shelfNumber is provided, rowNumber must also be provided.");
    }
}