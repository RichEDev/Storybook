CREATE NONCLUSTERED INDEX [_dta_index_cars_66_2105058535___cars]
    ON [dbo].[cars]([employeeid] ASC, [carid] ASC)
    INCLUDE([startdate], [enddate], [make], [model], [registration], [cartypeid], [active], [odometer], [fuelcard], [endodometer], [taxexpiry], [taxlastchecked], [taxcheckedby], [mottestnumber], [motlastchecked], [motcheckedby], [motexpiry], [insurancenumber], [insuranceexpiry], [insurancelastchecked], [insurancecheckedby], [serviceexpiry], [servicelastchecked], [servicecheckedby], [CreatedOn], [CreatedBy], [ModifiedOn], [ModifiedBy], [default_unit], [enginesize], [approved], [exemptFromHomeToOffice], [taxAttachID], [MOTAttachID], [insuranceAttachID], [serviceAttachID]) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF, ONLINE = OFF, MAXDOP = 0)
    ON [PRIMARY];

