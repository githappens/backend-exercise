using System;

public class Book : LibraryItem
{
    [FieldName("Author")]
    public List<string> Authors { get; set; } = new List<string>();
    public int NumberOfPages { get; set; }
    public string Publisher { get; set; }
    [FieldName("Published")]
    public int PublicationYear { get; set; }
    public int ShelfID { get; set; }
}