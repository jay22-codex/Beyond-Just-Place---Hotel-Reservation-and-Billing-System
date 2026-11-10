CREATE TABLE [dbo].[Payments]
(
    [PaymentId] INT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Payments] PRIMARY KEY,
    [ReservationId] INT NOT NULL,
    [PaymentAmount] DECIMAL(18,2) NOT NULL,
    [PaymentMethod] NVARCHAR(30) NOT NULL,
    [PaymentStatus] NVARCHAR(30) NOT NULL,
    [AmountReceived] DECIMAL(18,2) NULL,
    [ChangeAmount] DECIMAL(18,2) NULL,
    [ProcessedBy] NVARCHAR(100) NULL,
    [PaymentDate] DATETIME2 NOT NULL CONSTRAINT [DF_Payments_Date] DEFAULT SYSDATETIME(),
    CONSTRAINT [FK_Payments_Reservations] FOREIGN KEY ([ReservationId])
        REFERENCES [dbo].[Reservations]([ReservationId])
);
GO

CREATE UNIQUE INDEX [UX_Payments_PaidReservation]
    ON [dbo].[Payments]([ReservationId])
    WHERE [PaymentStatus] = N'Paid';
