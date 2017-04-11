SELECT COUNT(*), a.pages 
FROM Author AS a, Book AS b
WHERE a.authorId = b.authorId AND a.nationality <> 'Taiwan';

