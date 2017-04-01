-- sp_GetChildren
EXEC [dbo].[sp_GetChildren] 2

-- Test sp_GetJoinedDate
DECLARE @JoinedDate datetime
EXEC [dbo].[sp_GetJoinedDate] 
    @StaffID = 2,
	@JoinedDate = @JoinedDate OUTPUT
	
SELECT @JoinedDate as N'@JoinedDate'

-- sp_InsertEmp
DECLARE @ID int
EXEC [dbo].[sp_InsertEmp] 
	@ID = @ID OUTPUT,
    @Name = N'Jack Corolis',
	@Title = 'HR Manager',
	@Address = 'Emerald Hill',
	@Salary = 3600.0,
	@JoinedDate = '2011-05-06',
	@Children = 1
	
SELECT @ID as N'@ID'
