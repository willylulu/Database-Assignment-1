SELECT COUNT(*)
FROM Book, Author AS a
WHERE Book.authorId = a.authorId AND Book.pages > 200;