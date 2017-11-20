CREATE TABLE [dbo].[hotel_reviews] (
    [reviewid]         INT             IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [hotelid]          INT             NOT NULL,
    [rating]           TINYINT         NOT NULL,
    [review]           NVARCHAR (4000) COLLATE Latin1_General_CI_AS NULL,
    [employeeid]       INT             NULL,
    [displaytype]      TINYINT         NOT NULL,
    [amountpaid]       MONEY           NULL,
    [reviewdate]       DATETIME        NOT NULL,
    [standardrooms]    TINYINT         NULL,
    [hotelfacilities]  TINYINT         NULL,
    [valuemoney]       TINYINT         NULL,
    [performancestaff] TINYINT         NULL,
    [location]         TINYINT         NULL
);

