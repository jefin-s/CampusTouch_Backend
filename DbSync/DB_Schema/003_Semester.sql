
IF OBJECT_ID('dbo.Semester', 'U') IS NULL
BEGIN
CREATE TABLE [dbo].[Semester](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](100) NOT NULL,
	[orderNumber] [int] NOT NULL,
	[Courseid] [int] NOT NULL,
	[isActive] [bit] NULL,
	[CreatedA] [datetime] NULL,
	[updatedAt] [datetime] NULL,
	[CreatedBy] [nvarchar](50) NULL,
	[UpdatedBy] [nvarchar](50) NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [nvarchar](50) NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Course_Semester_Order] UNIQUE NONCLUSTERED 
(
	[Courseid] ASC,
	[orderNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY];


ALTER TABLE [dbo].[Semester] ADD  DEFAULT ((1)) FOR [isActive];


ALTER TABLE [dbo].[Semester] ADD  DEFAULT (getdate()) FOR [CreatedA];


ALTER TABLE [dbo].[Semester] ADD  DEFAULT ((0)) FOR [IsDeleted];


ALTER TABLE [dbo].[Semester]  WITH CHECK ADD  CONSTRAINT [Fk_Semester_course] FOREIGN KEY([Courseid])
REFERENCES [dbo].[courses] ([id])
ON DELETE CASCADE;


ALTER TABLE [dbo].[Semester] CHECK CONSTRAINT [Fk_Semester_course];


END