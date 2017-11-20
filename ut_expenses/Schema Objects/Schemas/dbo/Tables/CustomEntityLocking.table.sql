CREATE TABLE [CustomEntityLocking]
(
	[customEntityId] INT NOT NULL, 
    [entityId] INT NOT NULL, 
    [lockedBy] INT NOT NULL, 
    [LockedDateTime] DATETIME NOT NULL, 
    PRIMARY KEY ([customEntityId], [entityId])
)
