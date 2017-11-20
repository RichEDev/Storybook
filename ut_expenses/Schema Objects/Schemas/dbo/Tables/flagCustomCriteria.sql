-- Columns

-- Columns

CREATE TABLE [dbo].[flagCustomCriteria]
(
[criteriaId] [uniqueidentifier] NOT NULL CONSTRAINT [DF_flagCustomCriteria_criteriaId] DEFAULT (newid()),
[flagId] [int] NOT NULL,
[condition] [tinyint] NOT NULL,
[value1] [nvarchar] (1000) COLLATE Latin1_General_CI_AS NULL,
[value2] [nvarchar] (1000) COLLATE Latin1_General_CI_AS NULL,
[andor] [tinyint] NOT NULL,
[order] [int] NOT NULL,
[groupnumber] [tinyint] NULL,
[fieldID] [uniqueidentifier] NOT NULL,
[joinViaID] [int] NULL
)
GO
-- Foreign Keys

-- Foreign Keys

ALTER TABLE [dbo].[flagCustomCriteria] ADD CONSTRAINT [FK_flagCustomCriteria_flags] FOREIGN KEY ([flagId]) REFERENCES [dbo].[flags] ([flagID]) ON DELETE CASCADE
GO
-- Constraints and Indexes

-- Constraints and Indexes

ALTER TABLE [dbo].[flagCustomCriteria] ADD CONSTRAINT [PK_flagCustomCriteria_1] PRIMARY KEY CLUSTERED  ([criteriaId])