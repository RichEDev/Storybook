ALTER TABLE [dbo].[reportcolumns_flatfile]
    ADD CONSTRAINT [PK_reportcolumns_flatfile_1] PRIMARY KEY CLUSTERED ([employeeid] ASC, [reportcolumnid] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

