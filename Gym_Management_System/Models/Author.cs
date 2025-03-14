// Author.cs
public class Author
{
  public int AuthorId { get; set; }
  public string Name { get; set; } = string.Empty;
  //  An Author has more than one Books
  public List<Book> Books { get; set; } = new List<Book>();
}
