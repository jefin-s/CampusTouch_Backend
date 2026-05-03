IF OBJECT_ID('dbo.Staff', 'U') IS NULL
BEGIN
CREATE TABLE [dbo].[staff](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[userId] [nvarchar](450) NOT NULL,
	[FirstName] [nvarchar](100) NOT NULL,
	[LastName] [nvarchar](100) NULL,
	[Email] [nvarchar](150) NOT NULL,
	[PhoneNumber] [nvarchar](15) NOT NULL,
	[DepartmentId] [int] NOT NULL,
	[Designation] [nvarchar](100) NULL,
	[JoiningDate] [datetime] NOT NULL,
	[IsActive] [bit] NULL,
	[CreatedAt] [datetime] NOT NULL,
	[Createdby] [nvarchar](50) NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [nvarchar](50) NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBuy] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY];


ALTER TABLE [dbo].[staff] ADD  DEFAULT ((1)) FOR [IsActive];


ALTER TABLE [dbo].[staff]  WITH CHECK ADD FOREIGN KEY([DepartmentId])
REFERENCES [dbo].[Departments] ([Id]);

END

