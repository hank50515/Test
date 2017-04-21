CREATE TABLE [dbo].[Employee](
[EmployeeID] [int] IDENTITY(1,1) NOT NULL,
[FirstName] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[LastName] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[Address] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[Phone] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[DepartmentID] [bigint] NOT NULL,
[JoiningDate] [datetime] NOT NULL,
[LeavingDate] [datetime] NULL,
[DesignationID] [bigint] NOT NULL,
[PFID] [bigint] NOT NULL,
CONSTRAINT [PK_Employee] PRIMARY KEY CLUSTERED
(
[EmployeeID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[Department](
[DepartmentID] [bigint] NOT NULL,
[DepartmentName] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[DepartmentHead] [bigint] NOT NULL,
CONSTRAINT [PK_Department] PRIMARY KEY CLUSTERED
(
[DepartmentID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[Designation](
[DesignationID] [bigint] NOT NULL,
[DesignationName] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
CONSTRAINT [PK_Designation] PRIMARY KEY CLUSTERED
(
[DesignationID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[Salary](
[SalaryID] [bigint] NOT NULL,
[EmployeeID] [bigint] NOT NULL,
[Basic] [money] NOT NULL,
[Allowance] [money] NOT NULL,
[PFID] [bigint] NULL,
[Tax] [money] NOT NULL,
[GrossSalary] [money] NOT NULL,
[NetSalary] [money] NOT NULL,
CONSTRAINT [PK_Salary] PRIMARY KEY CLUSTERED
(
[SalaryID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[ProvidentFund](
[PFID] [bigint] NOT NULL,
[EmployeeID] [bigint] NOT NULL,
[PFAmount] [money] NOT NULL,
CONSTRAINT [PK_ProvidentFund] PRIMARY KEY CLUSTERED
(
[PFID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

Go

Create Procedure Employee_Insert
@FirstName varchar(50), @LastName varchar(50), @Address varchar(50), @Phone varchar(50), @DepartmentID int, @DesignationID int,
@JoiningDate datetime, @LeavingDate datetime
as
Insert into Employee(DepartmentID, DesignationID, JoiningDate, LeavingDate) values (@DepartmentID, @DesignationID, @JoiningDate,
@LeavingDate)
Go

Create Procedure Employee_Update
@EmployeeID int, @FirstName varchar(50), @LastName varchar(50),
@Address varchar(50), @Phone varchar(50), @DepartmentID int,
@DesignationID int
as
Update Employee Set DepartmentID = @DepartmentID, DesignationID =
@DesignationID Where Employee.EmployeeID = @EmployeeID

Go
Create Procedure Employee_Delete
@EmployeeID int
as
Delete from Employee where Employee.EmployeeID = @EmployeeID
Go

Create Procedure Department_Insert
@DepartmentName varchar(50)
as
Insert into Department (DepartmentName) values (@DepartmentName)
Go

Create Procedure Department_Update
@DepartmentID int,@DepartmentName varchar(50)
as
Update Department Set DepartmentName = @DepartmentName where DepartmentID = @DepartmentID
Go

Create Procedure Department_Delete
@DepartmentID int
as
Delete from Department where DepartmentID = @DepartmentID
Go

Create Procedure Designation_Insert
@DesignationName varchar(50)
as
Insert into Designation (DesignationName) values (@DesignationName)
Go

Create Procedure Designation_Update
@DesignationID int, @DesignationName varchar(50)
as
Update Designation Set DesignationName = @DesignationName where DesignationID=@DesignationID
Go
Create Procedure Designation_Delete
@DesignationID int
as
Delete from Designation where DesignationID=@DesignationID
Go

Create Procedure ProvidentFund_Insert
@EmployeeID int, @PFAmount money
as
Insert into ProvidentFund (EmployeeID, PFAmount) values (@EmployeeID,
@PFAmount)
Go
Create Procedure ProvidentFund_Delete
@PFID int
as
Delete from ProvidentFund where PFID = @PFID
Go
Create Procedure Salary_Insert
@EmployeeID int, @PFID int, @Basic money, @Allowance money,
@Tax money, @GrossSalary money, @NetSalary money
as
Insert into Salary(EmployeeID, PFID, Basic, Allowance, Tax, GrossSalary, NetSalary) values (@EmployeeID, @PFID, @Basic,
@Allowance, @Tax, @GrossSalary, @NetSalary)
Go
Create Procedure Salary_Update
@SalaryID int, @EmployeeID int, @PFID int, @Basic money, @Allowance money, @Tax money, @GrossSalary money, @NetSalary money
as
Update Salary set EmployeeID = @EmployeeID, PFID = @PFID, Basic =
@Basic, Allowance = @Allowance, Tax = @Tax, GrossSalary = @GrossSalary, NetSalary = @NetSalary where SalaryID = @SalaryID
Go
Create Procedure Salary_Delete
@SalaryID int
as
Delete from Salary where SalaryID = @SalaryID

Go