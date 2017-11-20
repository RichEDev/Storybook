
CREATE PROCEDURE [dbo].[saveEmployee]
 @employeeid int,
 @username nvarchar(50),
 @title nvarchar(20),
 @firstname nvarchar(50),
 @surname nvarchar(50),
 @mileagetotal int,
 @mileagetotaldate datetime,
 @email nvarchar(500),
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
 @active bit,
 @groupidcc int,
 @groupidpc int,
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
 @currencyid int,
 @authoriserLevelDetail int,
 @localeID int,
 @NHSTrustID int,
 @employeeNumber nvarchar(30),
 @nhsuniqueId nvarchar(15),
 @preferredName nvarchar(150),
 @date datetime,
 @userid int,
 @CUemployeeID INT,
 @CUdelegateID INT,
 @defaultSubAccountId int,
 @CreationMethod tinyint,
 @verified bit,
 @locked bit,
 @logonRetryCount int,
 @password nvarchar(max),
 @passwordMethod tinyint,
 @archived bit,
 @currclaimno int,
 @lastchange datetime,
 @countryid int,
 @notifyClaimUnsubmission BIT, 
 @DvlaConsentDate DATETIME,
 @DriverId INT,
 @ExcessMileage FLOAT 
AS
BEGIN
declare @count int;

 if @employeeid = 0
  begin
   set @count = (select count(employeeid) from employees where username = @username)
   if @count > 0
    return -1

   insert into  employees (username, title, firstname, surname, mileagetotal, mileagetotaldate, telno, email, creditor, payroll, position, groupid, extension, pagerno, faxno, mobileno, homeemail, linemanager, advancegroupid, primarycountry, primarycurrency, verified, active, groupidcc, groupidpc, ninumber, middlenames, maidenname, gender, dateofbirth, hiredate, terminationdate, createdon, createdby, localeID, NHSTrustID, employeeNumber, preferredName, NHSUniqueId, defaultSubAccountId, CreationMethod, locked, retryCount, [password], passwordMethod, archived, curclaimno, lastchange,AuthoriserLevelDetailId,NotifyClaimUnsubmission,DVLAConsentDate,DriverId, ExcessMileage) 
                   values (@username,@title,@firstname,@surname,@mileagetotal,@mileagetotaldate, @telno,@email,@creditor,@payroll,@position,@groupid, @extension, @pagerno, @faxno, @mobileno, @homeemail, @linemanager, @advancegroup, @primarycountry, @primarycurrency, @verified, @active,@groupidcc, @groupidpc, @ninumber, @middlenames, @maidenname, @gender, @dateofbirth, @hiredate, @terminationdate, @date, @userid, @localeID, @NHSTrustID, @employeeNumber, @preferredName, @nhsuniqueId, @defaultSubAccountId, @CreationMethod, @locked, @logonRetryCount, @password, @passwordMethod, @archived, @currclaimno, @lastchange, @authoriserLevelDetail,@notifyClaimUnsubmission,@DvlaConsentDate,@DriverId, @ExcessMileage);
   set @employeeid = scope_identity();

   if @accountnumber<>'' 
   begin
   INSERT INTO [dbo].[BankAccounts]
           ([EmployeeId],[AccountName],[AccountNumber],[AccountType],[SortCode],[Reference],[CurrencyId],[CreatedOn],[CreatedBy],CountryId) VALUES (@employeeid,dbo.getEncryptedValue(@name),dbo.getEncryptedValue(@accountnumber),@accounttype,dbo.getEncryptedValue(@sortcode),dbo.getEncryptedValue(@reference),@currencyid,@date,@userid,@countryid)
   end

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
   set @count = (select count(employeeid) from employees where username = @username and employeeid <> @employeeid)
   if @count > 0
    return -1;
   
   declare @oldgroupid int;
   select @oldgroupid = groupid from employees where employeeid = @employeeid;
   if @oldgroupid <> @groupid
    begin
     set @count = (select count(claimid) from claims where employeeid = @employeeid and submitted = 1 and paid = 0)
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
   declare @oldauthoriserLevelDetail int;
   
   declare @oldlocaleID int;
   declare @oldNHSTrustID int;
   declare @oldEmployeeNumber nvarchar(30);
   declare @oldNhsUniqueId nvarchar(15);
   declare @oldPreferredName nvarchar(150);
   declare @oldDefaultSubAccountId int;
   declare @oldCreationMethod tinyint;
   declare @oldverified bit;
   declare @oldlocked bit;
   declare @oldlogonRetryCount int;
   declare @oldpassword nvarchar(max);
   declare @oldpasswordMethod tinyint;
   declare @oldlastchange datetime;   
   declare @oldarchived bit;
   declare @oldNotifyClaimUnsubmission bit;
   declare @oldExcessMileage FLOAT;

   select @oldusername = username, @oldtitle = title, @oldfirstname = firstname, @oldsurname = surname, @oldmileagetotal = mileagetotal, @oldmileagetotaldate = mileagetotaldate, @oldemail = email, @oldpayroll = payroll, @oldposition = position, @oldtelno = telno, @oldcreditor = creditor, @oldgroupid = groupid, @oldextension = extension, @oldpagerno = pagerno, @oldmobileno = mobileno, @oldfaxno = faxno, @oldhomeemail = homeemail, @oldlinemanager = linemanager, @oldadvancegroup = advancegroupid, @oldprimarycountry = primarycountry, @oldprimarycurrency = primarycurrency, @oldgroupidcc = groupidcc, @oldgroupidpc = groupidpc, @oldcountry = country, @oldninumber = ninumber, @oldmaidenname = maidenname, @oldmiddlenames = middlenames, @oldgender = gender, @olddateofbirth = dateofbirth, @oldhiredate = hiredate, @oldterminationdate = terminationdate, @oldlocaleID = localeID, @oldNHSTrustID = NHSTrustID, @oldEmployeeNumber = employeeNumber, @oldPreferredName = preferredName, @oldNhsUniqueId = nhsUniqueId, @oldDefaultSubAccountId = defaultSubAccountId, @oldCreationMethod = creationMethod, @oldverified = verified, @oldlocked = locked, @oldlogonRetryCount = retryCount, @oldpassword = password, @oldpasswordMethod = passwordMethod, @oldlastchange = lastchange, @oldauthoriserLevelDetail = AuthoriserLevelDetailId, @oldarchived = archived ,@oldNotifyClaimUnsubmission=NotifyClaimUnsubmission, @oldExcessMileage = ExcessMileage from employees where employeeid = @employeeid;

   update employees set title = @title, firstname = @firstname, surname = @surname, telno = @telno, email = @email,creditor = @creditor, payroll = @payroll, position = @position, groupid = @groupid, mileagetotal = @mileagetotal,mileagetotaldate = @mileagetotaldate, homeemail = @homeemail, pagerno = @pagerno, mobileno = @mobileno, faxno = @faxno,extension = @extension, linemanager = @linemanager, advancegroupid = @advancegroup, primarycountry = @primarycountry,primarycurrency = @primarycurrency, groupidcc = @groupidcc, groupidpc = @groupidpc, ninumber = @ninumber, middlenames = @middlenames,active=@active, maidenname = @maidenname, gender = @gender, dateofbirth = @dateofbirth, hiredate = @hiredate, terminationdate = @terminationdate,username = @username, modifiedon = @date, modifiedby = @userid, localeID = @localeID, NHSTrustID = @NHSTrustID, EmployeeNumber = @employeeNumber,NHSUniqueId = @nhsuniqueId, PreferredName = @preferredName, defaultSubAccountId = @defaultSubAccountId, creationMethod = @CreationMethod, CacheExpiry = GETDATE(),verified = @verified, locked = @locked, retryCount = @logonRetryCount, [password] = @password, passwordMethod = @passwordMethod , archived = @archived,curclaimno = @currclaimno, lastchange = @lastchange, AuthoriserLevelDetailId= @authoriserLevelDetail,NotifyClaimUnsubmission=@notifyClaimUnsubmission, ExcessMileage = @ExcessMileage where employeeid = @employeeid;
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
    if @oldpayroll <> @payroll
      exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '6a76898b-4052-416c-b870-61479ca15ac1', @oldpayroll, @payroll, @username, null;
    if @oldposition <> @position
      exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '5f4a4551-1c05-4c85-b6d9-06d036bc327e', @oldposition, @position, @username, null;
    if @oldtelno <> @telno
      exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, 'd22c2d3f-7b33-4744-a1cf-c55296161cc3', @oldtelno, @telno, @username, null;
    if @oldcreditor <> @creditor
      exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '25860cd6-8026-4f86-b5e7-5436d25ab244', @oldcreditor, @creditor, @username, null;
    if @oldgroupid <> @groupid
	BEGIN
	declare @oldgroupname nvarchar(50)
	select @oldgroupname = groupname from groups where groupid = @oldgroupid
	declare @groupname nvarchar(50)
	select @groupname = groupname from groups where groupid = @groupid
      exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '4f615406-8d1f-47b3-821b-88bade48205e', @oldgroupname, @groupname, @username, null;
	END
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
	BEGIN
	declare @oldadvancegroupname nvarchar(50)
	select @oldadvancegroupname = groupname from groups where groupid = @oldadvancegroup
	declare @advancegroupname nvarchar(50)
	select @advancegroupname = groupname from groups where groupid = @advancegroup
      exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, 'daff7a05-3489-4af0-92c2-e7d4f98691cd', @oldadvancegroupname, @advancegroupname, @username, null;
	END
    if @oldprimarycountry <> @primarycountry
      exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '031cf9c5-ffff-4a7a-ab98-54617a91766a', @oldprimarycountry, @primarycountry, @username, null;
    if @oldprimarycurrency <> @primarycurrency
      exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '026cc190-20d8-427e-9ae2-200747f45670', @oldprimarycurrency, @primarycurrency, @username, null;
    if @oldgroupidcc <> @groupidcc
	BEGIN
	declare @oldgroupccname nvarchar(50)
	select @oldgroupccname = groupname from groups where groupid = @oldgroupidcc
	declare @groupccname nvarchar(50)
	select @groupccname = groupname from groups where groupid = @groupidcc
      exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, 'dd8b8da9-57d9-449d-9fb1-57356b6d3e50', @oldgroupccname, @groupccname, @username, null;
	END
    if @oldgroupidpc <> @groupidpc
	BEGIN
	declare @oldgrouppcname nvarchar(50)
	select @oldgrouppcname = groupname from groups where groupid = @oldgroupidcc
	declare @grouppcname nvarchar(50)
	select @grouppcname = groupname from groups where groupid = @groupidcc
      exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, 'da00f527-705c-4731-82b5-287be3df7e5b', @oldgrouppcname, @grouppcname, @username, null;
	END
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
	if (@oldauthoriserLevelDetail <> @authoriserLevelDetail or (@oldauthoriserLevelDetail is null and @authoriserLevelDetail is not null) or (@authoriserLevelDetail is null and @oldauthoriserLevelDetail is not null))
	  BEGIN
		  declare @authotiserrecord nvarchar(50) 
		  set @authotiserrecord = (select @username + ' (Authoriser Level)');

		  declare @oldauthoriserlevel nvarchar(50)
		  declare @authoriserlevel nvarchar(50)
		  declare @symbol nvarchar(10)

		  select @symbol = currencySymbol  from global_currencies where globalcurrencyid = (select globalcurrencyid from currencies where currencyid = @primarycurrency)
		  SET @authoriserlevel = @symbol + CAST((select amount from AuthoriserLevelDetails where AuthoriserLevelDetailId = @authoriserLevelDetail) AS nvarchar(10));
		  SET @oldauthoriserlevel = @symbol + CAST((select amount from AuthoriserLevelDetails where AuthoriserLevelDetailId = @oldauthoriserLevelDetail) AS nvarchar(10));

		  exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '78FB2B48-7462-48FE-9E1E-A0D3E4A4FB4C', @oldauthoriserlevel, @authoriserlevel, @authotiserrecord, null;  
	  END  
    if @oldlocaleID <> @localeID
      exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, null, @oldlocaleID, @localeID, @username, null;
    if @oldNHSTrustID <> @NHSTrustID
      exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '9573ed0b-814b-4cb0-916a-8ce25893617d', @oldNHSTrustID, @NHSTrustID, @username, null;
    if @oldDefaultSubAccountId <> @defaultSubAccountId
      exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, null, @oldDefaultSubAccountId, @defaultSubAccountId, @username, null;
    if @oldCreationMethod <> @CreationMethod
     exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '473bf9a3-4d7d-4993-aab4-096fee8002b4', @oldCreationMethod, @CreationMethod, @username, null;
    if @oldPreferredName <> @preferredName
     exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '3309E5E0-6105-4E2C-BE90-A8EAD03EACF2', @oldPreferredName, @preferredName, @username, null;
    if @oldNhsUniqueId <> @nhsuniqueId
     exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, 'EA2ECF94-AB38-475F-8454-C57DA22C88BA', @oldNhsUniqueId, @nhsuniqueId, @username, null;
    if @oldEmployeeNumber <> @employeeNumber
     exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, 'D7DA409B-3E6D-4C92-B737-ADB4D90C23E5', @oldEmployeeNumber, @employeeNumber, @username, null;
    if @oldverified <> @verified
     exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, 'EC8BC4EE-A235-4803-852E-3339926EB763', @oldverified, @employeeNumber, @username, null;
    if @oldlocked <> @locked
     exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, 'EE09104F-BEAD-44DA-8B05-17A6CE6BDFC5', @oldlocked, @employeeNumber, @username, null;
    if @oldpassword <> @password
     exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '669236FD-CBA3-4E80-B58D-68A52C45032B', null, @employeeNumber, @username, null;
    if @oldlastchange <> @lastchange
     exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, null, @oldlastchange, @employeeNumber, @username, null;     
	if @oldarchived <> @archived
	 BEGIN
		declare @old_archived varchar(10) = 'Live'
		declare @new_archived varchar(10) = 'Archived'
		if @oldarchived = 1
		BEGIN
			set @old_archived = 'Archived'
			set @new_archived = 'Live'
		END
		exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '3A6A93F0-9B30-4CC2-AFC4-33EC108FA77A', @old_archived, @new_archived, @username, null;
	 END
	 if @oldNotifyClaimUnsubmission <> @notifyClaimUnsubmission
     exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '6C85659D-BE65-403C-8519-EB86265024B7', @oldNotifyClaimUnsubmission, @notifyClaimUnsubmission, @username, null; 
	 if @oldExcessMileage <> @ExcessMileage
     exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 25, @employeeid, '984D6B47-666F-49CA-9304-79AAACFC1CB8', @oldExcessMileage, @ExcessMileage, @username, null;  	  	 
   END
  end

 return @employeeid
END
