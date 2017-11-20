ALTER TABLE dbo.VehicleEngineTypes
    ADD CONSTRAINT FK_VehicleEngineTypes_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.employees (employeeid);
    
