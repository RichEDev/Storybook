

CREATE FUNCTION [dbo].[getUserdefinedViewGroup](@tableid uniqueidentifier, @userdefinedFieldType int) 
RETURNS uniqueidentifier
AS
BEGIN
				IF @userdefinedFieldType = 8
								BEGIN
												RETURN NULL    
								END
				-- Declare the return variable here
				DECLARE @viewgroupid uniqueidentifier;
				DECLARE @fieldid uniqueidentifier;

				--Expense Items
				IF @tableid = '65394331-792e-40b8-af8b-643505550783'
								BEGIN   
												SET @fieldid = '951135dd-8d2d-47f8-ab18-d75758558e72';
								END
                
				--Employees
				IF @tableid = '972ac42d-6646-4efc-9323-35c2c9f95b62'
								BEGIN
												SET @fieldid = 'fc73b3bb-0737-4d99-9a3b-48dee8f9e0f8';
								END
                
				--Cars
				IF @tableid = '7e9e6bee-f8ca-45d8-b914-1a9b105e47b2'
								BEGIN
												SET @fieldid = '7e380edf-688e-4a00-8f34-ddfadb277d07';
								END

				--Claims
				IF @tableid = 'f70d6e0d-8e38-4a1d-a681-cc9d310c2ae9'
								BEGIN
												SET @fieldid = '52b7fe8b-4476-4af9-98f0-48627cae1e6a';
								END       

				--Departments
				IF @tableid = '155ae388-1b60-4fb2-a1bd-c46f543fa401'
								BEGIN
												SET @fieldid = '2df90d15-237f-4bc5-834a-90e69fe7568b';
								END       

				--Subcats -Expense Item Details
				IF @tableid = '82f57980-92c9-4a4b-b76d-2e8485c0bb41'
								BEGIN
												SET @fieldid = '3cc15963-9423-4841-924f-332deaeeedd8';
								END       

				--Costcodes        
				IF @tableid = 'e4cca1ba-a065-4116-860b-abaa1e7bb2ef'
								BEGIN
												SET @fieldid = 'edd93d78-306e-47a3-b6ab-cc4573b7dbde';
								END       

				--Locations
				IF @tableid = '7d323dae-3494-4d9b-b1a0-85f5a2d69e1b'
								BEGIN
												SET @fieldid = '7b70a503-4c23-4d5e-98ff-97b5da447c29';
								END       

				--Project Codes
				IF @tableid = 'ce235f78-82c6-4ba1-8845-034a015c5dca'
								BEGIN
												SET @fieldid = '58aadfb6-ef24-4d09-b02c-2e4faadb8c17';
								END
                                
				--Contract Details
				IF @tableid = 'A5508EF4-D0EE-48B1-B9B4-19473D133F98'
								BEGIN
												SET @fieldid = '4658C119-2E03-4BB3-969D-2409C09BA5DE';
								END
                                
				--Contract Product Details
				IF @tableid = '04F351A6-F7FF-4B39-A782-1E8317567E57'
								BEGIN
												SET @fieldid = '8E2BE977-0A35-47D1-B44D-CF08518A6269';
								END
                                
				--Product Details
				IF @tableid = '9353859B-CF95-4A58-863C-5A134E16C5F5'
								BEGIN
												SET @fieldid = '0812C440-349F-4823-A410-6EAAD5439B6A';
								END
                                
				--Supplier Details
				IF @tableid = '1371141B-67D7-4D46-B9B6-8E37DEE3C1C8'
								BEGIN
												SET @fieldid = '2F78C51A-E8A7-4147-98CF-6C90CDEA4F24';
								END
                
				--Supplier Contacts
				IF @tableid = 'D731594C-FED5-49DE-98C4-650297AA7219'
								BEGIN
												SET @fieldid = '17E10368-4101-49D6-AB4E-7666D3EDA7C8';
								END

				--Invoices
								IF @tableid = '8B3750B3-7ADD-4984-9363-B22B23B3B559'
								BEGIN
												SET @fieldid = '8EB1D9D2-6B88-49DA-9FB7-063A92CC7D5E';
								END
                
				-- Product Licences
								IF @tableid = '9B38866A-6786-4064-B77C-10D3948E0994'
								BEGIN
												SET @fieldid = 'DEEDF617-6D5D-42B4-92A3-D87B3C52631A';
								END
                
				--
				SET @viewgroupid = (SELECT viewgroupid FROM [$(metabaseExpenses)].dbo.fields_base WHERE fieldid = @fieldid);
				-- Return the result of the function
				RETURN @viewgroupid;

END

