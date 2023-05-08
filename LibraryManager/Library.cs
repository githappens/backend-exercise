using System.Diagnostics.CodeAnalysis;
public class Library
{
    public List<Room> Rooms { get; set; } = new List<Room>();
    public List<Row> Rows { get; set; } = new List<Row>();
    public List<BookShelf> BookShelves { get; set; } = new List<BookShelf>();
    public List<Book> Books { get; set; } = new List<Book>();

    public void AddBook(Book book, int roomNumber, int rowNumber, int shelfNumber)
    {
        var room = Rooms.FirstOrDefault(r => r.RoomNumber == roomNumber);
        if (room is null)
        {
            room = new Room { RoomNumber = roomNumber };
            Rooms.Add(room);
        }

        var row = Rows.FirstOrDefault(r => r.RowNumber == rowNumber && r.RoomID == room.RoomID);
        if (row is null)
        {
            row = new Row { RowNumber = rowNumber, RoomID = room.RoomID };
            Rows.Add(row);
        }

        var shelf = BookShelves.FirstOrDefault(s => s.ShelfNumber == shelfNumber && s.RowID == row.RowID);
        if (shelf is null)
        {
            shelf = new BookShelf { ShelfNumber = shelfNumber, RowID = row.RowID };
            BookShelves.Add(shelf);
        }

        book.ShelfID = shelf.ShelfID;
        Books.Add(book);
    }

    [return: MaybeNull]
    public Book FindBookByISBN(string isbn)
    {
        return Books.FirstOrDefault(book => book.ISBN == isbn);
    }

    public List<Book> GetInventoryList(int roomNumber, int? rowNumber = null, int? shelfNumber = null)
{
    if (shelfNumber.HasValue && !rowNumber.HasValue)
        throw new ArgumentException("When shelfNumber is provided, rowNumber must also be provided.");

    return Books
        .Join(BookShelves, b => b.ShelfID, bs => bs.ShelfID, (b, bs) => new { Book = b, BookShelf = bs })
        .Join(Rows, b_bs => b_bs.BookShelf.RowID, r => r.RowID, (b_bs, r) => new { b_bs.Book, b_bs.BookShelf, Row = r })
        .Join(Rooms, b_bs_r => b_bs_r.Row.RoomID, rm => rm.RoomID, (b_bs_r, rm) => new { b_bs_r.Book, b_bs_r.BookShelf, b_bs_r.Row, Room = rm })
        .Where(b_bs_r_rm => b_bs_r_rm.Room.RoomNumber == roomNumber
            && (!rowNumber.HasValue || b_bs_r_rm.Row.RowNumber == rowNumber.Value)
            && (!shelfNumber.HasValue || b_bs_r_rm.BookShelf.ShelfNumber == shelfNumber.Value))
        .Select(b_bs_r_rm => b_bs_r_rm.Book)
        .Distinct()
        .ToList();
}

}