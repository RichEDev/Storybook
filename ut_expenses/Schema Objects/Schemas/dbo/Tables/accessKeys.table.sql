CREATE TABLE [dbo].[accessKeys] (
    [KeyID]      INT            IDENTITY (1, 1) NOT NULL,
    [Key]        NVARCHAR (18)  NOT NULL,
    [EmployeeID] INT            NOT NULL,
    [DeviceID]   NVARCHAR (MAX) NULL,
    [Active]     BIT            NOT NULL,
    [CreatedOn]  DATE           NULL,
    [CreatedBy]  INT            NULL,
    [ModifiedOn] DATE           NULL,
    [ModifiedBy] INT            NULL
);

