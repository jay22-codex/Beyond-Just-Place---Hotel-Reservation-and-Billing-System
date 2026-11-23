CREATE TABLE [dbo].[ReservationServices]
(
    [ServiceId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ReservationId] INT NOT NULL REFERENCES [dbo].[Reservations]([ReservationId]),
    [ServiceName] NVARCHAR(30) NOT NULL,
    [Quantity] INT NOT NULL,
    [UnitPrice] DECIMAL(18,2) NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT SYSDATETIME()
);
