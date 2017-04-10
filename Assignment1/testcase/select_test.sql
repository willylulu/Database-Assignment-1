SELECT title, a.name
FROM Book, Author AS a
WHERE Book.pages > 200