CREATE TABLE Book (
    bookId int,
    title varchar(30),
    pages int,
    authorId int,
    editorial varchar(30)
);

CREATE TABLE Author (
    authorId int,
    name varchar(30),
    nationality varchar(30)
);

CREATE TABLE Student (
    studentId int,
    name varchar(30),
    age int,
    code varchar(10),
    credits int
);