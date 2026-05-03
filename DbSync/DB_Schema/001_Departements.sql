IF OBJECT_ID('dbo.Departments', 'U') IS NULL
BEGIN

CREATE TABLE [dbo].[Departments](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[IsActive] [bit] NULL,
	[CreatedAt] [datetime2](7) NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[Description] [nvarchar](500) NULL,
	[CreatedBy] [nvarchar](50) NULL,
	[UpdatedBy] [nvarchar](50) NULL,
	[DeletedBy] [nvarchar](50) NULL,
	[DeletedAt] [nvarchar](50) NULL,
	[IsDeleted] [bit] NOT NULL,
	[Code] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY];


ALTER TABLE [dbo].[Departments] ADD  DEFAULT ((1)) FOR [IsActive];


ALTER TABLE [dbo].[Departments] ADD  DEFAULT (getutcdate()) FOR [CreatedAt];


ALTER TABLE [dbo].[Departments] ADD  DEFAULT ((0)) FOR [IsDeleted];

END
