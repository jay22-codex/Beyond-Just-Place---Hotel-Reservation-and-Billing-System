CREATE TABLE [dbo].[Users]
(
    [UserId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Username] VARCHAR(50) NOT NULL,
    [Password] VARCHAR(50) NOT NULL,
    [Role] VARCHAR(20) NOT NULL
);