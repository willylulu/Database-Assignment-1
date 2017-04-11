SELECT bookId, title, pages, name
FROM Book, Author
WHERE Book.authorId = Author.authorId and 1;