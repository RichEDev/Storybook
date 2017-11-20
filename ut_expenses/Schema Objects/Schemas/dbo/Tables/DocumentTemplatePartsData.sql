CREATE TABLE [dbo].[DocumentTemplatePartsData] (
    [DocumentPartId]   INT             IDENTITY (1, 1) NOT NULL,
    [MergeProjectId]   INT             NOT NULL,
    [GroupingId]       INT             NOT NULL,
    [DocumentData]     VARBINARY (MAX) NOT NULL,
    [DocumentPartName] NVARCHAR (255)  NOT NULL,
    CONSTRAINT [PK_DocumentTemplatePartsData] PRIMARY KEY CLUSTERED ([DocumentPartId] ASC),
    CONSTRAINT [FK_DocumentTemplatePartsData_document_template_data] FOREIGN KEY ([MergeProjectId]) REFERENCES [dbo].[document_template_data] ([documentid]) ON DELETE CASCADE
);



