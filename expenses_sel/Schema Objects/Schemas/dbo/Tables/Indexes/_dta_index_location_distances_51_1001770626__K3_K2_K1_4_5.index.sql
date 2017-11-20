CREATE NONCLUSTERED INDEX [_dta_index_location_distances_51_1001770626__K3_K2_K1_4_5]
    ON [dbo].[location_distances]([locationb] ASC, [locationa] ASC, [distanceid] ASC)
    INCLUDE([distance], [postcodeanywheredistance]) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];

