

CREATE TABLE [dbo].[classes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[DepartmentId] [int] NOT NULL,
	[CourseId] [int] NOT NULL,
	[year] [int] NOT NULL,
	[semester] [int] NOT NULL,
	[createdAt] [datetime] NULL,
	[isActive] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Class] UNIQUE NONCLUSTERED 
(
	[DepartmentId] ASC,
	[CourseId] ASC,
	[semester] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY];


ALTER TABLE [dbo].[classes] ADD  DEFAULT (getdate()) FOR [createdAt];


ALTER TABLE [dbo].[classes] ADD  DEFAULT ((1)) FOR [isActive];


ALTER TABLE [dbo].[classes]  WITH CHECK ADD  CONSTRAINT [FK_Classes_Course] FOREIGN KEY([CourseId])
REFERENCES [dbo].[courses] ([id]);


ALTER TABLE [dbo].[classes] CHECK CONSTRAINT [FK_Classes_Course];


ALTER TABLE [dbo].[classes]  WITH CHECK ADD  CONSTRAINT [FK_Classes_Department] FOREIGN KEY([DepartmentId])
REFERENCES [dbo].[Departments] ([Id]);


ALTER TABLE [dbo].[classes] CHECK CONSTRAINT [FK_Classes_Department];


ALTER TABLE [dbo].[classes]  WITH CHECK ADD  CONSTRAINT [FK_Classes_Semester] FOREIGN KEY([semester])
REFERENCES [dbo].[Semester] ([id]);


ALTER TABLE [dbo].[classes] CHECK CONSTRAINT [FK_Classes_Semester];


