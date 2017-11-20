ALTER TABLE dbo.subcats
ADD CONSTRAINT FK_subcats_mileage_categories_PublicTransportRate FOREIGN KEY
(
     PublicTransportRate
)
REFERENCES dbo.mileage_categories
(
     mileageid
) 
ON DELETE NO ACTION