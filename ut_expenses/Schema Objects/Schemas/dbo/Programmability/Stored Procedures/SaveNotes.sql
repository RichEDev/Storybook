CREATE PROCEDURE [dbo].[SaveNotes]
	@employeeid int,
	 @note ntext
AS
	BEGIN
	insert into notes (employeeid, note) values (@employeeid, @note)
END
