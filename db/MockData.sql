DROP TABLE IF EXISTS Comment;
DROP TABLE IF EXISTS Log;
DROP TABLE IF EXISTS Users;

-- CREATE TABLE Log (
--     logId INT NOT NULL AUTO_INCREMENT,
--     username TINYTEXT,
--     title TINYTEXT,
--     date DATETIME,
--     content TEXT,
--     PRIMARY KEY (logId)
-- );

CREATE TABLE Log (
    logId SERIAL PRIMARY KEY,
    username TEXT,
    title TEXT,
    date TIMESTAMP,
    content TEXT
);

-- CREATE TABLE Comment (
--     commentId INT NOT NULL AUTO_INCREMENT,
--     logId INT,
--     username TINYTEXT,
--     date DATETIME,
--     content TEXT,
--     PRIMARY KEY (commentId),
--     FOREIGN KEY (logId) REFERENCES Log(logId)
-- );

CREATE TABLE Comment (
    commentId SERIAL PRIMARY KEY,
    logId INT,
    parent_commentId INT,
    username TEXT,
    date TIMESTAMP,
    content TEXT,
    FOREIGN KEY (logId) REFERENCES Log(logId)
);

-- CREATE TABLE User (
--     userId INT NOT NULL AUTO_INCREMENT,
--     username TINYTEXT,
--     date DATETIME,
--     password TEXT,
--     PRIMARY KEY (userId)
-- );

CREATE TABLE Users (
    userId SERIAL PRIMARY KEY,
    username TEXT,
    date TIMESTAMP,
    password TEXT
);


INSERT INTO Log (username, title, date, content) VALUES ('chris', 'test', '2025-05-25 08:22:40', 'hello world');

INSERT INTO Comment (logId, parent_commentId, username, date, content) VALUES (1, NULL, 're', '2025-05-25 08:25:40', 'hello');

INSERT INTO Users (username, date, password) VALUES ('chris', '2025-05-21 08:25:40', 'test');
