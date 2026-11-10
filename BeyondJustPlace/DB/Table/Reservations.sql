CREATE TABLE [dbo].[Reservations]
(
    [ReservationId] INT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Reservations] PRIMARY KEY,
    [GuestId] INT NOT NULL,
    [RoomId] INT NOT NULL,
    [CheckInDate] DATE NOT NULL,
    [CheckOutDate] DATE NOT NULL,
    [ReservationStatus] NVARCHAR(30) NOT NULL,
    [NumberOfGuests] INT NOT NULL CONSTRAINT [DF_Reservations_Guests] DEFAULT 1,
    [Adults] INT NOT NULL CONSTRAINT [DF_Reservations_Adults] DEFAULT 1,
    [Children] INT NOT NULL CONSTRAINT [DF_Reservations_Children] DEFAULT 0,
    [RoomCount] INT NOT NULL CONSTRAINT [DF_Reservations_RoomCount] DEFAULT 1,
    [SpecialRequest] NVARCHAR(1000) NULL,
    [ReservationFeeAmount] DECIMAL(18,2) NULL,
    [ReservationFeePaymentMethod] NVARCHAR(30) NULL,
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_Reservations_CreatedAt] DEFAULT SYSDATETIME(),
    CONSTRAINT [FK_Reservations_Guests] FOREIGN KEY ([GuestId]) REFERENCES [dbo].[Guests]([GuestId]),
    CONSTRAINT [FK_Reservations_Rooms] FOREIGN KEY ([RoomId]) REFERENCES [dbo].[Rooms]([RoomId]),
    CONSTRAINT [CK_Reservations_Dates] CHECK ([CheckOutDate] > [CheckInDate])
);
GO

CREATE INDEX [IX_Reservations_RoomDates]
    ON [dbo].[Reservations]([RoomId], [CheckInDate], [CheckOutDate], [ReservationStatus]);
GO
CREATE INDEX [IX_Reservations_GuestStatus]
    ON [dbo].[Reservations]([GuestId], [ReservationStatus], [ReservationId]);
