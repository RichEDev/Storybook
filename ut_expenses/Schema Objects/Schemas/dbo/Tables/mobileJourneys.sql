CREATE TABLE [dbo].[MobileJourneys]
(
	[JourneyID] INT identity(1,1) NOT NULL,
	[EmployeeID] INT NOT NULL,  
	[SubcatID] INT NOT NULL, 
	[DeviceTypeID] INT NULL, 
    [JourneyJSON] NVARCHAR(MAX) NOT NULL, 
    [JourneyDate] DATETIME NOT NULL, 
    [JourneyStartTime] DATETIME NOT NULL, 
    [JourneyEndTime] DATETIME NOT NULL, 
    [CreatedBy] INT NOT NULL, 
    [CreatedOn] DATETIME NOT NULL, 
    [Active] BIT NULL, 
    CONSTRAINT [PK_MobileJourneys] PRIMARY KEY CLUSTERED 
(
	[JourneyID] ASC
),
    CONSTRAINT [FK_mobileJourneys_ToEmployees] FOREIGN KEY ([EmployeeID]) REFERENCES [Employees]([EmployeeID]) ON DELETE CASCADE, 
    CONSTRAINT [FK_MobileJourneys_ToSubcats] FOREIGN KEY ([SubcatID]) REFERENCES [Subcats]([SubcatID]) ON DELETE CASCADE
)
