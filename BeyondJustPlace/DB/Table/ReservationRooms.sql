CREATE TABLE [dbo].[ReservationRooms]
(
    [ReservationRoomId] INT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ReservationRooms] PRIMARY KEY,
    [ReservationId] INT NOT NULL,
    [RoomId] INT NOT NULL,
    [RoomRateSnapshot] DECIMAL(18,2) NOT NULL,
    [ReservationFeeSnapshot] DECIMAL(18,2) NULL,
    CONSTRAINT [FK_ReservationRooms_Reservations]
        FOREIGN KEY ([ReservationId]) REFERENCES [dbo].[Reservations]([ReservationId]),
    CONSTRAINT [FK_ReservationRooms_Rooms]
        FOREIGN KEY ([RoomId]) REFERENCES [dbo].[Rooms]([RoomId])
);
GO

CREATE UNIQUE INDEX [UX_ReservationRooms_ReservationRoom]
    ON [dbo].[ReservationRooms]([ReservationId], [RoomId]);
GO

CREATE INDEX [IX_ReservationRooms_RoomReservation]
    ON [dbo].[ReservationRooms]([RoomId], [ReservationId]);
