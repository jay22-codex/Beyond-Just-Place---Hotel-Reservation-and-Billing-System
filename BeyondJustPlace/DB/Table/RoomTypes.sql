CREATE TABLE [dbo].[RoomTypes]
(
    [RoomTypeId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [TypeName] NVARCHAR(100) NOT NULL UNIQUE,
    [Rate] DECIMAL(18,2) NOT NULL,
    [Capacity] INT NOT NULL,
    [BedCount] INT NOT NULL,
    [SizeSqm] INT NOT NULL,
    [ReservationFee] DECIMAL(18,2) NOT NULL,
    [Description] NVARCHAR(500) NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1
);
