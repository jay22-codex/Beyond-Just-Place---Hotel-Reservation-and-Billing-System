CREATE TABLE [dbo].[Guests]
(
    [GuestId] INT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Guests] PRIMARY KEY,
    [GuestName] NVARCHAR(255) NOT NULL,
    [ContactNumber] NVARCHAR(50) NOT NULL,
    [Email] NVARCHAR(254) NULL,
    [HomeAddress] NVARCHAR(500) NULL,
    [IdType] NVARCHAR(50) NULL,
    [IdNumber] NVARCHAR(100) NULL,
    [IsActive] BIT NOT NULL CONSTRAINT [DF_Guests_IsActive] DEFAULT 1,
    [IdVerified] BIT NOT NULL CONSTRAINT [DF_Guests_IdVerified] DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_Guests_CreatedAt] DEFAULT SYSDATETIME()
);
