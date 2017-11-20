CREATE TABLE [customEntityViewFormSelectionMappings] (
	[viewFormSelectionMappingId] INT NOT NULL IDENTITY(1,1) CONSTRAINT [PK_customEntityViewFormSelectionMappings] PRIMARY KEY,
	[viewId] INT NOT NULL CONSTRAINT [FK_customEntityViewFormSelectionMappings_customEntityViews] FOREIGN KEY REFERENCES [customEntityViews] ([viewId]) ON DELETE CASCADE ON UPDATE NO ACTION,
	[isAdd] BIT NOT NULL CONSTRAINT [DF_customEntityViewFormSelectionMappings_isAdd] DEFAULT ((0)),
	[formId] INT NOT NULL CONSTRAINT [FK_customEntityViewFormSelectionMappings_customEntityForms] FOREIGN KEY REFERENCES [customEntityForms] ([formId]) ON DELETE NO ACTION ON UPDATE NO ACTION,
	[listValue] INT NULL,
	[textValue] NVARCHAR(4000) NULL,
	CONSTRAINT [CK_customEntityViewFormSelectionMappings] CHECK (([listValue] = NULL AND [textValue] IS NOT NULL) OR ([listValue] IS NOT NULL AND [textValue] = NULL))
);