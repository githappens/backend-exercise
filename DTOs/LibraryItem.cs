using System;

public abstract class LibraryItem
{
    public Guid ID { get; set; }
    public string Title { get; set; }
    public string ISBN { get; set; }
}