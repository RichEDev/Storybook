CREATE PROCEDURE [dbo].[saveEmployee]
	@employeeid int,
	@username nvarchar(50),
	@title nvarchar(20),
	@firstname nvarchar(50),
	@surname nvarchar(50),
	@mileagetotal int,
	@mileagetotaldate datetime,
	@email nvarchar(500),
	@address1 nvarchar(50),
	@address2 nvarchar(50),
	@city nvarchar(50),
	@county nvarchar(50),
	@postcode nvarchar(50),
	@payroll nvarchar(50),
	@position nvarchar(50),
	@telno nvarchar(50),
	@creditor nvarchar(50),
	@groupid int,
	@extension nvarchar(50),
	@pagerno nvarchar(50),
	@mobileno nvarchar(50),
	@faxno nvarchar(50),
	@homeemail nvarchar(500),
	@linemanager int,
	@advancegroup int,
	@primarycountry int,
	@primarycurrency int,
	@licenceexpiry DateTime,
	@active bit,
	@licencelastchecked datetime,
	@licencecheckedby int,
	@licencenumber nvarchar(50),
	@groupidcc int,
	@groupidpc int,
	@country nvarchar(100),
	@ninumber nvarchar(50),
	@maidenname nvarchar(50),
	@middlenames nvarchar(50),
	@gender nvarchar(6),
	@dateofbirth datetime,
	@hiredate datetime,
	@terminationdate datetime,
	@name nvarchar(50),
	@accountnumber nvarchar(50),
	@accounttype nvarchar(50),
	@sortcode nvarchar(50),
	@reference nvarchar(50),
	@localeID int,
	@NHSTrustID int,
	@date datetime,
	@userid int,
	@CUemployeeID INT,
	@CUdelegateID INT,
	@defaultSubAccountId int,
	@CreationMethod tinyint
AS
BEGIN
declare @count int;

	if @employeeid = 0
		begin
			set @count = (select count(*) from employees where username = @username)
			if @count > 0
				return -1

			insert into  employees (username, title, firstname, surname, mileagetotal, mileagetotaldate, address1, address2, city, county, postcode, telno, email, creditor, payroll, position, groupid, extension, pagerno, faxno, mobileno, homeemail, linemanager, advancegroupid, primarycountry, primarycurrency, verified, active, licencelastchecked, licenceexpiry, licencecheckedby, licencenumber, groupidcc, groupidpc, ninumber, middlenames, maidenname, gender, dateofbirth, hiredate, terminationdate, country, [name], accountnumber, accounttype, sortcode, reference, createdon, createdby, localeID, NHSTrustID, defaultSubAccountId, CreationMethod) values (@username,@title,@firstname,@surname,@mileagetotal,@mileagetotaldate,@address1,@address2,@city,@county,@postcode,@telno,@email,@creditor,@payroll,@position,@groupid, @extension, @pagerno, @faxno, @mobileno, @homeemail, @linemanager, @advancegroup, @primarycountry, @primarycurrency, 1, @active, @licencelastchecked, @licenceexpiry, @licencecheckedby, @licencenumber, @groupidcc, @groupidpc, @ninumber, @middlenames, @maidenname, @gender, @dateofbirth, @hiredate, @terminationdate, @country, @name, @accountnumber, @accounttype, @sortcode, @reference, @date, @userid, @localeID, @NHSTrustID, @defaultSubAccountId, @CreationMethod);
			set @employeeid = scope_identity();
			
			if @CUemployeeID = -1
				begin
					declare @label nvarchar(2000);
					set @label = (select 'Self-registration of ' + @username);
					exec addInsertEntryToAuditLog @employeeid, @CUdelegateID, 25, @employeeid, @username, null;
				end
			else
				if @CUemployeeID > 0
				begin
					exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, @username, null;
				end
		end
	else
		begin
			set @count = (select count(*) from employees where username = @username and employeeid <> @employeeid)
			if @count > 0
				return -1;
			
			declare @oldgroupid int;
			select @oldgroupid = groupid from employees where employeeid = @employeeid;
			if @oldgroupid <> @groupid
				begin
					set @count = (select count(*) from claims where employeeid = @employeeid and submitted = 1 and paid = 0)
					if @count > 0
						return -2;
				end

			declare @oldusername nvarchar(50);
			declare @oldtitle nvarchar(20);
			declare @oldfirstname nvarchar(50);
			declare @oldsurname nvarchar(50);
			declare @oldmileagetotal int;
			declare @oldmileagetotaldate datetime;
			declare @oldemail nvarchar(500);
			declare @oldaddress1 nvarchar(50);
			declare @oldaddress2 nvarchar(50);
			declare @oldcity nvarchar(50);
			declare @oldcounty nvarchar(50);
			declare @oldpostcode nvarchar(50);
			declare @oldpayroll nvarchar(50);
			declare @oldposition nvarchar(50);
			declare @oldtelno nvarchar(50);
			declare @oldcreditor nvarchar(50);
			declare @oldextension nvarchar(50);
			declare @oldpagerno nvarchar(50);
			declare @oldmobileno nvarchar(50);
			declare @oldfaxno nvarchar(50);
			declare @oldhomeemail nvarchar(500);
			declare @oldlinemanager int;
			declare @oldadvancegroup int;
			declare @oldprimarycountry int;
			declare @oldprimarycurrency int;
			declare @oldlicenceexpiry DateTime;
			declare @oldlicencelastchecked datetime;
			declare @oldlicencecheckedby int;
			declare @oldlicencenumber nvarchar(50);
			declare @oldgroupidcc int;
			declare @oldgroupidpc int;
			declare @oldcountry nvarchar(100);
			declare @oldninumber nvarchar(50);
			declare @oldmaidenname nvarchar(50);
			declare @oldmiddlenames nvarchar(50);
			declare @oldgender nvarchar(6);
			declare @olddateofbirth datetime;
			declare @oldhiredate datetime;
			declare @oldterminationdate datetime;
			declare @oldname nvarchar(50);
			declare @oldaccountnumber nvarchar(50);
			declare @oldaccounttype nvarchar(50);
			declare @oldsortcode nvarchar(50);
			declare @oldreference nvarchar(50);
			declare @oldlocaleID int;
			declare @oldNHSTrustID int;
			declare @oldDefaultSubAccountId int;
			declare @oldCreationMethod tinyint;
			
			select @oldusername = username, @oldtitle = title, @oldfirstname = firstname, @oldsurname = surname, @oldmileagetotal = mileagetotal, @oldmileagetotaldate = mileagetotaldate, @oldemail = email, @oldaddress1 = address1, @oldaddress2 = address2, @oldcity = city, @oldcounty = county, @oldpostcode = postcode, @oldpayroll = payroll, @oldposition = position, @oldtelno = telno, @oldcreditor = creditor, @oldgroupid = groupid, @oldextension = extension, @oldpagerno = pagerno, @oldmobileno = mobileno, @oldfaxno = faxno, @oldhomeemail = homeemail, @oldlinemanager = linemanager, @oldadvancegroup = advancegroupid, @oldprimarycountry = primarycountry, @oldprimarycurrency = primarycurrency, @oldlicenceexpiry = licenceexpiry, @oldlicencelastchecked = licencelastchecked, @oldlicencecheckedby = licencecheckedby, @oldlicencenumber = licencenumber, @oldgroupidcc = groupidcc, @oldgroupidpc = groupidpc, @oldcountry = country, @oldninumber = ninumber, @oldmaidenname = maidenname, @oldmiddlenames = middlenames, @oldgender = gender, @olddateofbirth = dateofbirth, @oldhiredate = hiredate, @oldterminationdate = terminationdate, @oldname = [name], @oldaccountnumber = accountnumber, @oldaccounttype = accounttype, @oldsortcode = sortcode, @oldreference = reference, @oldlocaleID = localeID, @oldNHSTrustID = NHSTrustID, @oldDefaultSubAccountId = defaultSubAccountId, @oldCreationMethod = creationMethod from employees where employeeid = @employeeid;

			update employees set title = @title, firstname = @firstname, surname = @surname, address1 = @address1, address2 = @address2, city = @city, county = @county, postcode = @postcode, telno = @telno, email = @email, creditor = @creditor, payroll = @payroll, position = @position, groupid = @groupid, mileagetotal = @mileagetotal, mileagetotaldate = @mileagetotaldate, homeemail = @homeemail, pagerno = @pagerno, mobileno = @mobileno, faxno = @faxno, extension = @extension, linemanager = @linemanager, advancegroupid = @advancegroup, primarycountry = @primarycountry, primarycurrency = @primarycurrency, licenceexpiry = @licenceexpiry, licencelastchecked = @licencelastchecked, licencecheckedby = @licencecheckedby, licencenumber = @licencenumber, groupidcc = @groupidcc, groupidpc = @groupidpc, ninumber = @ninumber, middlenames = @middlenames, active=@active, maidenname = @maidenname, gender = @gender, dateofbirth = @dateofbirth, hiredate = @hiredate, terminationdate = @terminationdate, country = @country, username = @username, [name] = @name, accountnumber = @accountnumber, accounttype = @accounttype, sortcode = @sortcode, reference = @reference, modifiedon = @date, modifiedby = @userid, localeID = @localeID, NHSTrustID = @NHSTrustID, defaultSubAccountId = @defaultSubAccountId, creationMethod = @CreationMethod, CacheExpiry = GETDATE() where employeeid = @employeeid;

			if @CUemployeeID > 0
			BEGIN
				if @oldusername <> @username
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '1c45b860-ddaa-47da-9eec-981f59cce795', @oldusername, @username, @username, null;
				if @oldtitle <> @title
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '28471060-247d-461c-abf6-234bcb4698aa', @oldtitle, @title, @username, null;
				if @oldfirstname <> @firstname
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '6614acad-0a43-4e30-90ec-84de0792b1d6', @oldfirstname, @firstname, @username, null;
				if @oldsurname <> @surname
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '9d70d151-5905-4a67-944f-1ad6d22cd931', @oldsurname, @surname, @username, null;
				if @oldmileagetotal <> @mileagetotal
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '81D172A8-91F4-4BD8-A9F1-C846F284D3B8', @oldmileagetotal, @mileagetotal, @username, null;
				if @oldmileagetotaldate <> @mileagetotaldate
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '9DCC6C76-09DA-415A-8B15-7C7F7978BC30', @oldmileagetotaldate, @mileagetotaldate, @username, null;
				if @oldemail <> @email
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '0f951c3e-29d1-49f0-ac13-4cfcabf21fda', @oldemail, @email, @username, null;
				if @oldaddress1 <> @address1
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, 'a0891ce2-d0c2-4b5b-9a78-7aaa3aaa87c1', @oldaddress1, @address1, @username, null;
				if @oldaddress2 <> @address2
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, 'ece5aeb6-2e2e-428f-bb17-253c91ad85b2', @oldaddress2, @address2, @username, null;
				if @oldcity <> @city
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '0330c639-1524-402b-b7bc-04d26bfc05a1', @oldcity, @city, @username, null;
				if @oldcounty <> @county
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '7204491c-63c3-47e8-8ee6-03cf1c464e4d', @oldcounty, @county, @username, null;
				if @oldpostcode <> @postcode
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '9c9f07dd-a9d0-4ccf-9231-dd3c10d491b8', @oldpostcode, @postcode, @username, null;
				if @oldpayroll <> @payroll
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '6a76898b-4052-416c-b870-61479ca15ac1', @oldpayroll, @payroll, @username, null;
				if @oldposition <> @position
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '5f4a4551-1c05-4c85-b6d9-06d036bc327e', @oldposition, @position, @username, null;
				if @oldtelno <> @telno
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, 'd22c2d3f-7b33-4744-a1cf-c55296161cc3', @oldtelno, @telno, @username, null;
				if @oldcreditor <> @creditor
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '25860cd6-8026-4f86-b5e7-5436d25ab244', @oldcreditor, @creditor, @username, null;
				if @oldgroupid <> @groupid
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '4f615406-8d1f-47b3-821b-88bade48205e', @oldgroupid, @groupid, @username, null;
				if @oldextension <> @extension
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '4f1c1895-a6b0-4eb3-a470-b34e43f07ef8', @oldextension, @extension, @username, null;
				if @oldpagerno <> @pagerno
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, 'fb07bc07-897d-438c-818f-5f5f0a0b7769', @oldpagerno, @pagerno, @username, null;
				if @oldmobileno <> @mobileno
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '0548653d-3255-4404-8bfb-20e5b83f7e45', @oldmobileno, @mobileno, @username, null;
				if @oldfaxno <> @faxno
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '79258e43-f986-4706-9289-18f8b9e43499', @oldfaxno, @faxno, @username, null;
				if @oldhomeemail <> @homeemail
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, 'cde508f2-f2b2-439c-8a1c-b70ee65e2abc', @oldhomeemail, @homeemail, @username, null;
				if @oldlinemanager <> @linemanager
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '96f11c6d-7615-4abd-94ec-0e4d34e187a0', @oldlinemanager, @linemanager, @username, null;
				if @oldadvancegroup <> @advancegroup
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, 'daff7a05-3489-4af0-92c2-e7d4f98691cd', @oldadvancegroup, @advancegroup, @username, null;
				if @oldprimarycountry <> @primarycountry
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '031cf9c5-ffff-4a7a-ab98-54617a91766a', @oldprimarycountry, @primarycountry, @username, null;
				if @oldprimarycurrency <> @primarycurrency
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '026cc190-20d8-427e-9ae2-200747f45670', @oldprimarycurrency, @primarycurrency, @username, null;
				if @oldlicenceexpiry <> @licenceexpiry
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '1a982e2e-f0e6-4e71-b768-b7e1329b5dda', @oldlicenceexpiry, @licenceexpiry, @username, null;
				if @oldlicencelastchecked <> @licencelastchecked
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '3f457c3f-995c-4da4-ad24-10301518ab1e', @oldlicencelastchecked, @licencelastchecked, @username, null;
				if @oldlicencecheckedby <> @licencecheckedby
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, 'fe106aa5-469f-488d-b9bc-b12cd06c1124', @oldlicencecheckedby, @licencecheckedby, @username, null;
				if @oldlicencenumber <> @licencenumber
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '79b7287b-9d74-404f-941d-6c15ffc9ea71', @oldlicencenumber, @licencenumber, @username, null;
				if @oldgroupidcc <> @groupidcc
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, 'dd8b8da9-57d9-449d-9fb1-57356b6d3e50', @oldgroupidcc, @groupidcc, @username, null;
				if @oldgroupidpc <> @groupidpc
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, 'da00f527-705c-4731-82b5-287be3df7e5b', @oldgroupidpc, @groupidpc, @username, null;
				if @oldcountry <> @country
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '8816caec-b520-4223-b738-47d2f22f3e1a', @oldcountry, @country, @username, null;
				if @oldninumber <> @ninumber
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '74391669-0070-4d19-af73-3ab4ea4d55bb', @oldninumber, @ninumber, @username, null;
				if @oldmaidenname <> @maidenname
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '075fda2d-efd4-461f-928f-9ff582a8b6ac', @oldmaidenname, @maidenname, @username, null;
				if @oldmiddlenames <> @middlenames
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, 'b3caf703-e72b-4eb8-9d5c-b389e16c8c43', @oldmiddlenames, @middlenames, @username, null;
				if @oldgender <> @gender
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, 'a4546216-b8ea-4ddf-b218-1a8c0493274c', @oldgender, @gender, @username, null;
				if @olddateofbirth <> @dateofbirth
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '486554c0-75ed-4718-ba32-3c11eaa5ee79', @olddateofbirth, @dateofbirth, @username, null;
				if @oldhiredate <> @hiredate
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '76473c0a-df08-40f9-8de0-632d0111a912', @oldhiredate, @hiredate, @username, null;
				if @oldterminationdate <> @terminationdate
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, 'b7cbf994-4a23-4405-93df-d66d4c3ed2a3', @oldterminationdate, @terminationdate, @username, null;
				if @oldname <> @name
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, null, @oldname, @name, @username, null;
				if @oldaccountnumber <> @accountnumber
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, null, @oldaccountnumber, @accountnumber, @username, null;
				if @oldaccounttype <> @accounttype
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, null, @oldaccounttype, @accounttype, @username, null;
				if @oldsortcode <> @sortcode
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, null, @oldsortcode, @sortcode, @username, null;
				if @oldreference <> @reference
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, null, @oldreference, @reference, @username, null;
				if @oldlocaleID <> @localeID
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, null, @oldlocaleID, @localeID, @username, null;
				if @oldNHSTrustID <> @NHSTrustID
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '9573ed0b-814b-4cb0-916a-8ce25893617d', @oldNHSTrustID, @NHSTrustID, @username, null;
				if @oldDefaultSubAccountId <> @defaultSubAccountId
						exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, null, @oldDefaultSubAccountId, @defaultSubAccountId, @username, null;
				if @oldCreationMethod <> @CreationMethod
					exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '473bf9a3-4d7d-4993-aab4-096fee8002b4', @oldCreationMethod, @CreationMethod, @username, null;
			END
		end

	return @employeeid
END
