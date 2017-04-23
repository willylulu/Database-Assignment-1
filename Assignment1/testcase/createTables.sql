CREATE TABLE Book (
    bookId int,
    title varchar(40),
    pages int,
    authorId int,
    editorial varchar(40)
);

CREATE TABLE Author (
    authorId int,
    name varchar(40),
    nationality varchar(40)
);

CREATE TABLE Student (
    studentId int,
    name varchar(30),
    age int,
    code varchar(10),
    credits int
);