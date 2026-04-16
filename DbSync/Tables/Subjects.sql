

CREATE TABLE [dbo].[Subjects](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Code] [nvarchar](20) NOT NULL,
	[Credits] [int] NOT NULL,
	[SemesterId] [int] NOT NULL,
	[IsActive] [bit] NULL,
	[CreatedAt] [datetime] NULL,
	[UpdatedAt] [datetime] NULL,
	[CreatedBy] [nvarchar](50) NULL,
	[deletedAt] [datetime] NULL,
	[UpdatedBy] [nvarchar](50) NULL,
	[DeletedBy] [nvarchar](50) NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Semester_Subject_Name] UNIQUE NONCLUSTERED 
(
	[SemesterId] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_subject_code] UNIQUE NONCLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY];

ALTER TABLE [dbo].[Subjects] ADD  DEFAULT ((1)) FOR [IsActive];


ALTER TABLE [dbo].[Subjects] ADD  DEFAULT (getdate()) FOR [CreatedAt];


ALTER TABLE [dbo].[Subjects] ADD  DEFAULT ((0)) FOR [IsDeleted];


ALTER TABLE [dbo].[Subjects]  WITH CHECK ADD  CONSTRAINT [FK_Subject_Semester] FOREIGN KEY([SemesterId])
REFERENCES [dbo].[Semester] ([id])
ON DELETE CASCADE;


ALTER TABLE [dbo].[Subjects] CHECK CONSTRAINT [FK_Subject_Semester];


