CREATE TABLE [dbo].[employeeMenuFavourites] (
    [MenuFavouriteID] INT            IDENTITY (1, 1) NOT NULL,
    [EmployeeID]      INT            NOT NULL,
    [Title]           NVARCHAR (100) NOT NULL,
    [IconLocation]    NVARCHAR (150) NOT NULL,
    [OnClickUrl]      NVARCHAR (250) NOT NULL,
    [Order]           TINYINT        NOT NULL
)
