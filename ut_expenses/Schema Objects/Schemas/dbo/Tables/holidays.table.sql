CREATE TABLE [dbo].[holidays] (
    [holidayid]  INT      IDENTITY (1, 1) NOT NULL,
    [employeeid] INT      CONSTRAINT [DF_holidays_employeeid] DEFAULT (0) NOT NULL,
    [startdate]  DATETIME NOT NULL,
    [enddate]    DATETIME NOT NULL,
    [CreatedOn]  DATETIME NULL,
    [CreatedBy]  INT      NULL,
    [ModifiedOn] DATETIME NULL,
    [ModifiedBy] INT      NULL,
    PRIMARY KEY CLUSTERED ([holidayid] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_holidays_employees] FOREIGN KEY ([employeeid]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE CASCADE ON UPDATE CASCADE
);



