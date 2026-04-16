

CREATE TABLE [dbo].[courses](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](100) NOT NULL,
	[level] [nvarchar](500) NOT NULL,
	[Duration] [int] NOT NULL,
	[DepartmentId] [int] NOT NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[CreatedAt] [datetime] NULL,
	[UpdatedBy] [nvarchar](100) NULL,
	[UpdatedAt] [datetime] NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY];


ALTER TABLE [dbo].[courses] ADD  DEFAULT ((0)) FOR [IsDeleted];


ALTER TABLE [dbo].[courses] ADD  DEFAULT ((1)) FOR [IsActive];


ALTER TABLE [dbo].[courses]  WITH CHECK ADD  CONSTRAINT [Fk_Courses_Dept] FOREIGN KEY([DepartmentId])
REFERENCES [dbo].[Departments] ([Id]);


ALTER TABLE [dbo].[courses] CHECK CONSTRAINT [Fk_Courses_Dept];


