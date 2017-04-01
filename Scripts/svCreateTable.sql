SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Employee](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](30) NOT NULL,
	[Title] [varchar](20) NOT NULL,
	[Address] [varchar](30) NOT NULL,
	[Salary] [money] NOT NULL,
	[JoinedDate] [datetime] NOT NULL,
	[Children] [tinyint] NULL,
 CONSTRAINT [PK_Employee] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


INSERT INTO [dbo].[Employee] ([Name], [Title], [Address], [Salary], [JoinedDate], [Children]) VALUES ('Steven Jacob', 'Account Manager', 'Orchard Road', 5260.0, '2005-10-19', 3)
GO
INSERT INTO [dbo].[Employee] ([Name], [Title], [Address], [Salary], [JoinedDate], [Children]) VALUES ('Mandy Dell', 'Finance Manager', 'Cherry Garden', 4500.0, '2009-03-28', 2)
GO
INSERT INTO [dbo].[Employee] ([Name], [Title], [Address], [Salary], [JoinedDate]) VALUES ('Boris Russ', 'IT Manager', 'Lavender Villa', 3200.0, '2010-12-03')
GO

CREATE PROCEDURE [dbo].[sp_GetAllEmployee]
AS
	SELECT * FROM [dbo].[Employee];
GO


CREATE PROCEDURE [dbo].[sp_GetChildren](
	@StaffID int )
AS
	SELECT Children FROM [dbo].[Employee] WHERE ID = @StaffID;
GO

CREATE PROCEDURE [dbo].[sp_GetJoinedDate](
	@StaffID int,
	@JoinedDate datetime output
	)
AS
	SELECT @JoinedDate = JoinedDate FROM [dbo].[Employee] WHERE ID = @StaffID;
GO

CREATE PROCEDURE [dbo].[sp_InsertEmp](
	@ID int OUTPUT,
	@Name nvarchar(30),
	@Title varchar(20),
	@Address varchar(30),
	@Salary money,
	@JoinedDate datetime,
	@Children tinyint)
AS
	INSERT INTO [dbo].[Employee] ([Name], [Title], [Address], [Salary], [JoinedDate], [Children]) VALUES (@Name, @Title, @Address, @Salary, @JoinedDate, @Children);
	Select @ID = Scope_Identity();
GO

CREATE PROCEDURE [dbo].[sp_GetNum]
AS
	DECLARE @num AS INT;
	SET @num = 1000;
	RETURN @num;
GO

CREATE TYPE EmpType AS TABLE
(
	Name [nvarchar](30) NOT NULL,
	Title [varchar](20) NOT NULL,
	Address [varchar](30) NOT NULL,
	Salary [money] NOT NULL,
	JoinedDate [datetime] NOT NULL,
	Children [tinyint] NULL
);

GO

CREATE PROCEDURE [dbo].[sp_InsertManyEmp](
	@RowsInserted int OUTPUT,
	@Employees EmpType READONLY)
AS
	INSERT INTO [dbo].[Employee] ([Name], [Title], [Address], [Salary], [JoinedDate], [Children]) 
	SELECT [Name], [Title], [Address], [Salary], [JoinedDate], [Children] FROM @Employees;
	Set @RowsInserted = @@ROWCOUNT;
GO
