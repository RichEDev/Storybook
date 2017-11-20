CREATE TABLE [dbo].[hotel_reviews] (
    [reviewid]         INT             IDENTITY (79, 1) NOT FOR REPLICATION NOT NULL,
    [hotelid]          INT             NOT NULL,
    [rating]           TINYINT         NOT NULL,
    [review]           NVARCHAR (4000) COLLATE Latin1_General_CI_AS NULL,
    [employeeid]       INT             NULL,
    [displaytype]      TINYINT         NOT NULL,
    [amountpaid]       MONEY           NULL,
    [reviewdate]       DATETIME        CONSTRAINT [DF_hotel_review_reviewdate] DEFAULT (getdate()) NOT NULL,
    [standardrooms]    TINYINT         NULL,
    [hotelfacilities]  TINYINT         NULL,
    [valuemoney]       TINYINT         NULL,
    [performancestaff] TINYINT         NULL,
    [location]         TINYINT         NULL,
    CONSTRAINT [PK_hotel_review] PRIMARY KEY CLUSTERED ([reviewid] ASC),
    CONSTRAINT [FK_hotel_reviews_hotels] FOREIGN KEY ([hotelid]) REFERENCES [dbo].[hotels] ([hotelid]) ON DELETE CASCADE ON UPDATE CASCADE NOT FOR REPLICATION
);



