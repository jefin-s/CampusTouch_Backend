
IF OBJECT_ID('dbo.Attendance', 'U') IS NULL
BEGIN

CREATE TABLE [dbo].[Attendance](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AttendanceDate] [date] NOT NULL,
	[ClassId] [int] NOT NULL,
	[SubjectId] [int] NOT NULL,
	[StaffId] [nvarchar](450) NULL,
	[CreatedAt] [datetime] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Attendance] UNIQUE NONCLUSTERED 
(
	[AttendanceDate] ASC,
	[ClassId] ASC,
	[SubjectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY];


ALTER TABLE [dbo].[Attendance] ADD  DEFAULT (getdate()) FOR [CreatedAt];


ALTER TABLE [dbo].[Attendance] ADD  DEFAULT ((0)) FOR [IsDeleted];


ALTER TABLE [dbo].[Attendance]  WITH CHECK ADD  CONSTRAINT [FK_Attendance_Class] FOREIGN KEY([ClassId])
REFERENCES [dbo].[classes] ([Id]);


ALTER TABLE [dbo].[Attendance] CHECK CONSTRAINT [FK_Attendance_Class];


ALTER TABLE [dbo].[Attendance]  WITH CHECK ADD  CONSTRAINT [FK_Attendance_Staff] FOREIGN KEY([StaffId])
REFERENCES [dbo].[AspNetUsers] ([Id]);


ALTER TABLE [dbo].[Attendance] CHECK CONSTRAINT [FK_Attendance_Staff];


ALTER TABLE [dbo].[Attendance]  WITH CHECK ADD  CONSTRAINT [FK_Attendance_Subject] FOREIGN KEY([SubjectId])
REFERENCES [dbo].[Subjects] ([Id]);


ALTER TABLE [dbo].[Attendance] CHECK CONSTRAINT [FK_Attendance_Subject];


END