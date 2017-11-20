CREATE TABLE [dbo].[moduleElementBase] (
    [moduleID]  INT CONSTRAINT [DF_module_element_base_moduleID] DEFAULT ((2)) NOT NULL,
    [elementID] INT NOT NULL,
    CONSTRAINT [PK_moduleElementBase] PRIMARY KEY CLUSTERED ([moduleID] ASC, [elementID] ASC),
    CONSTRAINT [FK_module_element_base_elements_base] FOREIGN KEY ([elementID]) REFERENCES [dbo].[elementsBase] ([elementID]) NOT FOR REPLICATION,
    CONSTRAINT [FK_module_element_base_module_base] FOREIGN KEY ([moduleID]) REFERENCES [dbo].[moduleBase] ([moduleID]) NOT FOR REPLICATION
);



