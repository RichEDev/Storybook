CREATE TABLE [dbo].[hotels] (
    [hotelid]    INT            IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [hotelname]  NVARCHAR (250) COLLATE Latin1_General_CI_AS NOT NULL,
    [address1]   NVARCHAR (100) COLLATE Latin1_General_CI_AS NULL,
    [address2]   NVARCHAR (100) COLLATE Latin1_General_CI_AS NULL,
    [city]       NVARCHAR (100) COLLATE Latin1_General_CI_AS NULL,
    [county]     NVARCHAR (100) COLLATE Latin1_General_CI_AS NULL,
    [postcode]   NVARCHAR (100) COLLATE Latin1_General_CI_AS NULL,
    [country]    NVARCHAR (100) COLLATE Latin1_General_CI_AS NULL,
    [rating]     TINYINT        NOT NULL,
    [telno]      NVARCHAR (50)  COLLATE Latin1_General_CI_AS NULL,
    [email]      NVARCHAR (500) COLLATE Latin1_General_CI_AS NULL,
    [CreatedOn]  DATETIME       NULL,
    [CreatedBy]  INT            NULL,
    [ModifiedOn] DATETIME       NULL,
    [ModifiedBy] INT            NULL
);

