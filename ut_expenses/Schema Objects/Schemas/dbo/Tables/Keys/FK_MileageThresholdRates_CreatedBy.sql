ALTER TABLE dbo.MileageThresholdRates
    ADD CONSTRAINT FK_MileageThresholdRates_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.employees (employeeid);
    
