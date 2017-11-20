CREATE NONCLUSTERED INDEX [_dta_index_savedexpenses_getMileageTotalByEmployeeid]
    ON [dbo].[savedexpenses]([subcatid] ASC, [date] ASC, [claimid] ASC, [expenseid] ASC, [carid] ASC, [mileageid] ASC);
