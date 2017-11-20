ALTER TABLE dbo.VehicleEngineTypes
    ADD CONSTRAINT FK_VehicleEngineTypes_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES dbo.employees (employeeid);
