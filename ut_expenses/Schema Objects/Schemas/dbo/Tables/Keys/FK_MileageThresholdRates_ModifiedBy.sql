ALTER TABLE dbo.MileageThresholdRates
    ADD CONSTRAINT FK_MileageThresholdRates_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES dbo.employees (employeeid);
