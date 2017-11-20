CREATE PROCEDURE [dbo].[APIsaveEmployee]
	      @employeeid int out,
           @username nvarchar(50),
           @password nvarchar(250),
           @title nvarchar(30),
           @firstname nvarchar(150),
           @surname nvarchar(150),
           @mileagetotal int,
           @email nvarchar(250),
           @currefnum int,
           @curclaimno int,
           @speedo int,
           @payroll nvarchar(50),
           @position nvarchar(250),
           @telno nvarchar(50),
           @creditor nvarchar(50),
           @archived bit,
           @groupid int,
           @roleid int,
           @hint nvarchar(1000),
           @lastchange smalldatetime,
           @additems int,
           @cardnum nvarchar(50),
           @userole bit,
           @costcodeid int,
           @departmentid int,
           @extension nvarchar(50),
           @pagerno nvarchar(50),
           @mobileno nvarchar(50),
           @faxno nvarchar(50),
           @homeemail nvarchar(250),
           @linemanager int,
           @advancegroupid int,
           @mileage int,
           @mileageprev int,
           @customiseditems bit,
           @active bit,
           @primarycountry int,
           @primarycurrency int,
           @verified bit,
           @licenceexpiry datetime,
           @licencelastchecked datetime,
           @licencecheckedby int,
           @licencenumber nvarchar(50),
           @groupidcc int,
           @groupidpc int,
           @CreatedOn datetime,
           @CreatedBy int,
           @ModifiedOn datetime,
           @ModifiedBy int,
           @country nvarchar(100),
           @ninumber nvarchar(50),
           @maidenname nvarchar(150),
           @middlenames nvarchar(150),
           @gender nvarchar(6),
           @dateofbirth datetime,
           @hiredate datetime,
           @terminationdate datetime,
           @homelocationid int,
           @officelocationid int,
           @applicantnumber nvarchar(50),
           @applicantactivestatusflag bit,
           @passwordMethod tinyint,
           @name nvarchar(50),
           @accountnumber nvarchar(50),
           @accounttype nvarchar(50),
           @sortcode nvarchar(50),
           @reference nvarchar(50),
           @localeID int,
           @NHSTrustID int,
           @logonCount int,
           @retryCount int,
           @firstLogon bit,
           @licenceAttachID int,
           @defaultSubAccountId int,
           @CacheExpiry datetime,
           @supportPortalAccountID int,
           @supportPortalPassword nvarchar(250),
           @CreationMethod tinyint,
           @mileagetotaldate datetime,
           @adminonly bit,
           @locked bit,
           @ESRPersonId bigint,
     @preferredname nvarchar(150),
     @employeenumber nvarchar(30),
     @esrpersontype nvarchar(80),
     @nhsuniqueid nvarchar(15),
     @esreffectivestartdate datetime,
     @esreffectiveenddate datetime
AS
BEGIN
declare @count int;
 if @employeeid = 0
  begin
   set @count = (select count(*) from employees where username = @username)
   if @count > 0
    return -1
   SELECT TOP 1 @defaultSubAccountId = subAccountID FROM accountsSubAccounts
   INSERT INTO [dbo].[employees]
           ([username]
           ,[password]
           ,[title]
           ,[firstname]
           ,[surname]
           ,[mileagetotal]
           ,[email]
           ,[currefnum]
           ,[curclaimno]
           ,[speedo]
           ,[payroll]
           ,[position]
           ,[telno]
           ,[creditor]
           ,[archived]
           ,[groupid]
           ,[roleid]
           ,[hint]
           ,[lastchange]
           ,[additems]
           ,[cardnum]
           ,[userole]
           ,[costcodeid]
           ,[departmentid]
           ,[extension]
           ,[pagerno]
           ,[mobileno]
           ,[faxno]
           ,[homeemail]
           ,[linemanager]
           ,[advancegroupid]
           ,[mileage]
           ,[mileageprev]
           ,[customiseditems]
           ,[active]
           ,[primarycountry]
           ,[primarycurrency]
           ,[verified]
           ,[licenceexpiry]
           ,[licencelastchecked]
           ,[licencecheckedby]
           ,[licencenumber]
           ,[groupidcc]
           ,[groupidpc]
           ,[CreatedOn]
           ,[CreatedBy]
           ,[ModifiedOn]
           ,[ModifiedBy]
           ,[country]
           ,[ninumber]
           ,[maidenname]
           ,[middlenames]
           ,[gender]
           ,[dateofbirth]
           ,[hiredate]
           ,[terminationdate]
           ,[homelocationid]
           ,[officelocationid]
           ,[applicantnumber]
           ,[applicantactivestatusflag]
           ,[passwordMethod]
           ,[name]
           ,[accountnumber]
           ,[accounttype]
           ,[sortcode]
           ,[reference]
           ,[localeID]
           ,[NHSTrustID]
           ,[logonCount]
           ,[retryCount]
           ,[firstLogon]
           ,[licenceAttachID]
           ,[defaultSubAccountId]
           ,[CacheExpiry]
           ,[supportPortalAccountID]
           ,[supportPortalPassword]
           ,[CreationMethod]
           ,[mileagetotaldate]
           ,[adminonly]
           ,[locked]
           ,[ESRPersonId]
     ,[PreferredName]
     ,[EmployeeNumber]
     ,[NHSUniqueId]
     ,[ESRPersonType]
     ,[ESREffectiveStartDate]
     ,[ESREffectiveEndDate])
     VALUES
           (@username
           ,@password
           ,@title
           ,@firstname
           ,@surname
           ,@mileagetotal
           ,@email
           ,@currefnum
           ,@curclaimno
           ,@speedo
           ,@payroll
           ,@position
           ,@telno
           ,@creditor
           ,@archived
           ,@groupid
           ,@roleid
           ,@hint
           ,@lastchange
           ,@additems
           ,@cardnum
           ,@userole
           ,@costcodeid
           ,@departmentid
           ,@extension
           ,@pagerno
           ,@mobileno
           ,@faxno
           ,@homeemail
           ,@linemanager
           ,@advancegroupid
           ,@mileage
           ,@mileageprev
           ,@customiseditems
           ,@active
           ,@primarycountry
           ,@primarycurrency
           ,@verified
           ,@licenceexpiry
           ,@licencelastchecked
           ,@licencecheckedby
           ,@licencenumber
           ,@groupidcc
           ,@groupidpc
           ,@CreatedOn
           ,@CreatedBy
           ,@ModifiedOn
           ,@ModifiedBy
           ,@country
           ,@ninumber
           ,@maidenname
           ,@middlenames
           ,@gender
           ,@dateofbirth
           ,@hiredate
           ,@terminationdate
           ,@homelocationid
           ,@officelocationid
           ,@applicantnumber
           ,@applicantactivestatusflag
           ,@passwordMethod
           ,@name
           ,@accountnumber
           ,@accounttype
           ,@sortcode
           ,@reference
           ,@localeID
           ,@NHSTrustID
           ,@logonCount
           ,@retryCount
           ,@firstLogon
           ,@licenceAttachID
           ,@defaultSubAccountId
           ,@CacheExpiry
           ,@supportPortalAccountID
           ,@supportPortalPassword
           ,@CreationMethod
           ,@mileagetotaldate
           ,@adminonly
           ,@locked
           ,@ESRPersonId
		   ,@preferredname
		   ,@employeenumber
		   ,@nhsuniqueid
		   ,@esrpersontype
		   ,@esreffectivestartdate
		   ,@esreffectiveenddate)
   set @employeeid = scope_identity();
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
   UPDATE [dbo].[employees]
   SET [username] = @username
  ,[password] = @password
  ,[title] = @title
  ,[firstname] = @firstname
  ,[surname] = @surname
  ,[mileagetotal] = @mileagetotal
  ,[email] = @email
  ,[currefnum] = @currefnum
  ,[curclaimno] = @curclaimno
  ,[speedo] = @speedo
  ,[payroll] = @payroll
  ,[position] = @position
  ,[telno] = @telno
  ,[creditor] = @creditor
  ,[archived] = @archived
  ,[groupid] = @groupid
  ,[roleid] = @roleid
  ,[hint] = @hint
  ,[lastchange] = @lastchange
  ,[additems] = @additems
  ,[cardnum] = @cardnum
  ,[userole] = @userole
  ,[costcodeid] = @costcodeid
  ,[departmentid] = @departmentid
  ,[extension] = @extension
  ,[pagerno] = @pagerno
  ,[mobileno] = @mobileno
  ,[faxno] = @faxno
  ,[homeemail] = @homeemail
  ,[linemanager] = @linemanager
  ,[advancegroupid] = @advancegroupid
  ,[mileage] = @mileage
  ,[mileageprev] = @mileageprev
  ,[customiseditems] = @customiseditems
  ,[active] = @active
  ,[primarycountry] = @primarycountry
  ,[primarycurrency] = @primarycurrency
  ,[verified] = @verified
  ,[licenceexpiry] = @licenceexpiry
  ,[licencelastchecked] = @licencelastchecked
  ,[licencecheckedby] = @licencecheckedby
  ,[licencenumber] = @licencenumber
  ,[groupidcc] = @groupidcc
  ,[groupidpc] = @groupidpc
  ,[CreatedOn] = @CreatedOn
  ,[CreatedBy] = @CreatedBy
  ,[ModifiedOn] = @ModifiedOn
  ,[ModifiedBy] = @ModifiedBy
  ,[country] = @country
  ,[ninumber] = @ninumber
  ,[maidenname] = @maidenname
  ,[middlenames] = @middlenames
  ,[gender] = @gender
  ,[dateofbirth] = @dateofbirth
  ,[hiredate] = @hiredate
  ,[terminationdate] = @terminationdate
  ,[homelocationid] = @homelocationid
  ,[officelocationid] = @officelocationid
  ,[applicantnumber] = @applicantnumber
  ,[applicantactivestatusflag] = @applicantactivestatusflag
  ,[passwordMethod] = @passwordMethod
  ,[name] = @name
  ,[accountnumber] = @accountnumber
  ,[accounttype] = @accounttype
  ,[sortcode] = @sortcode
  ,[reference] = @reference
  ,[localeID] = @localeID
  ,[NHSTrustID] = @NHSTrustID
  ,[logonCount] = @logonCount
  ,[retryCount] = @retryCount
  ,[firstLogon] = @firstLogon
  ,[licenceAttachID] = @licenceAttachID
  ,[defaultSubAccountId] = @defaultSubAccountId
  ,[CacheExpiry] = @CacheExpiry
  ,[supportPortalAccountID] = @supportPortalAccountID
  ,[supportPortalPassword] = @supportPortalPassword
  ,[CreationMethod] = @CreationMethod
  ,[mileagetotaldate] = @mileagetotaldate
  ,[adminonly] = @adminonly
  ,[locked] = @locked
  ,[ESRPersonId] = @ESRPersonId
  ,[PreferredName] = @preferredname
  ,[EmployeeNumber] = @employeenumber
  ,[NHSUniqueId] = @nhsuniqueid
  ,[ESRPersonType] = @esrpersontype
  ,[ESREffectiveStartDate] = @esreffectivestartdate
  ,[ESREffectiveEndDate] = @esreffectiveenddate
 WHERE [employeeid] = @employeeid
End
END