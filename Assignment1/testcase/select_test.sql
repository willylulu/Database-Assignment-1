SELECT bookId, title, pages, name
FROM Book, Author
WHERE Book.authorId = Author.authorId AND Book.pages = 555;