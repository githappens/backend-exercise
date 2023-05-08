using System;
using System.Text.RegularExpressions;
using System.Reflection;

public static class Parser
{
    private const char LineDelimiter = '\n';
    private const char KeyValueDelimiter = ':';
    private const string QueryDelimiter = " & ";

    public static List<Book> ReadBooks(string input)
    {
        var books = new List<Book>();
        var lines = input.Split(LineDelimiter);
        Book currentBook = null;

        foreach (var line in lines)
        {
            if (line.StartsWith("Book"))
            {
                if (currentBook != null)
                {
                    books.Add(currentBook);
                }
                currentBook = new Book();
                continue;
            }

            if (currentBook is null)
            {
                throw new ArgumentException("Each entry should start with \"Book\"!");
            }
            
            var keyValue = line.Split(KeyValueDelimiter);
            var key = keyValue[0].Trim();
            var value = keyValue[1].Trim();

            var propertyInfo = typeof(Book).GetProperty(key);

            if (propertyInfo != null)
            {
                SetValue(ref currentBook, propertyInfo, value);
                continue;
            }
            
            var propertiesWithAttribute = typeof(Book).GetProperties()
                .Where(prop => Attribute.IsDefined(prop, typeof(FieldNameAttribute)))
                .ToArray();

            var propertyFound = false;
            foreach (var prop in propertiesWithAttribute)
            {
                var attribute = prop.GetCustomAttribute<FieldNameAttribute>();
                if (attribute.Value == key)
                {
                    SetValue(ref currentBook, prop, value);
                    propertyFound = true;
                    break;
                }
            }

            if(!propertyFound)
                throw new ArgumentException($"Property with key {key} could not be parsed.");
        }

        if (currentBook != null)
        {
            books.Add(currentBook);
        }

        return books;
    }

private static void SetValue(ref Book book, PropertyInfo propertyInfo, string value)
{
    switch (propertyInfo.PropertyType)
    {
        case Type t when t == typeof(int):
            propertyInfo.SetValue(book, int.Parse(value));
            break;
        case Type t when t == typeof(List<string>):
            ((List<string>)propertyInfo.GetValue(book)).Add(value);
            break;
        default:
            propertyInfo.SetValue(book, value);
            break;
    }
}

public static List<Book> FindBooks(this List<Book> books, string searchString)
{
    var foundBooks = new List<Book>();
    var andQueries = searchString.Split(new[] { QueryDelimiter }, StringSplitOptions.None);

    foreach (Book book in books)
    {
        var allQueriesMatch = true;

        foreach (string query in andQueries)
        {
            var substrings = query.Trim('*').Split('*');

            var queryMatch =
            substrings.All(substring =>
            {
                var pattern = Regex.Escape(substring).Replace("\\*", ".*");
                return
                    Regex.IsMatch(book.Title, pattern, RegexOptions.IgnoreCase) ||
                    Regex.IsMatch(book.Publisher, pattern, RegexOptions.IgnoreCase) ||
                    book.Authors.Any(author => Regex.IsMatch(author, pattern, RegexOptions.IgnoreCase)) ||
                    Regex.IsMatch(book.PublicationYear.ToString(), pattern, RegexOptions.IgnoreCase);
           });

            if (!queryMatch)
            {
                allQueriesMatch = false;
                break;
            }
        }

        if (allQueriesMatch)
        {
            foundBooks.Add(book);
        }
    }

    return foundBooks;
}

}