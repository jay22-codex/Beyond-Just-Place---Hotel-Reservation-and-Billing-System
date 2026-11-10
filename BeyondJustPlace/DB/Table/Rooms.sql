CREATE TABLE [dbo].[Rooms]
(
    [RoomId] INT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Rooms] PRIMARY KEY,
    [RoomNumber] NVARCHAR(20) NOT NULL,
    [RoomType] NVARCHAR(100) NOT NULL,
    [RoomRate] DECIMAL(18,2) NOT NULL,
    [RoomStatus] NVARCHAR(30) NOT NULL CONSTRAINT [DF_Rooms_Status] DEFAULT N'Available',
    [RoomCapacity] INT NULL,
    [ReservationFee] DECIMAL(18,2) NULL,
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_Rooms_CreatedAt] DEFAULT SYSDATETIME()
);
GO

CREATE UNIQUE INDEX [UX_Rooms_RoomNumber] ON [dbo].[Rooms]([RoomNumber]);
