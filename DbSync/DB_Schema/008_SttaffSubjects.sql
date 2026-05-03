IF OBJECT_ID('dbo.StaffSubject', 'U') IS NULL
BEGIN

CREATE TABLE [dbo].[StaffSubject](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StaffId] [int] NOT NULL,
	[SubjectID] [int] NOT NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [nvarchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY];


ALTER TABLE [dbo].[StaffSubject]  WITH CHECK ADD FOREIGN KEY([StaffId])
REFERENCES [dbo].[staff] ([id]);


ALTER TABLE [dbo].[StaffSubject]  WITH CHECK ADD FOREIGN KEY([SubjectID])
REFERENCES [dbo].[Subjects] ([Id]);


END