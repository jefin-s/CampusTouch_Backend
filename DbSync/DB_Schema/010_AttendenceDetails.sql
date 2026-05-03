
  IF OBJECT_ID('dbo.AttendenceDetails', 'U') IS NULL
BEGIN
CREATE TABLE [dbo].[AttendenceDetails](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AttendanceId] [int] NOT NULL,
	[StudentId] [int] NOT NULL,
	[Status] [varchar](20) NOT NULL,
	[Remark] [nvarchar](255) NULL,
	[MarkedAt] [datetime] NOT NULL,
	[IsEdited] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_AttendenceDetails] UNIQUE NONCLUSTERED 
(
	[AttendanceId] ASC,
	[StudentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY];


ALTER TABLE [dbo].[AttendenceDetails] ADD  DEFAULT (getdate()) FOR [MarkedAt];


ALTER TABLE [dbo].[AttendenceDetails] ADD  DEFAULT ((0)) FOR [IsEdited];


ALTER TABLE [dbo].[AttendenceDetails]  WITH CHECK ADD  CONSTRAINT [FK_AttendanceDetails_Attendance] FOREIGN KEY([AttendanceId])
REFERENCES [dbo].[Attendance] ([Id])
ON DELETE CASCADE;


ALTER TABLE [dbo].[AttendenceDetails] CHECK CONSTRAINT [FK_AttendanceDetails_Attendance];


ALTER TABLE [dbo].[AttendenceDetails]  WITH CHECK ADD  CONSTRAINT [FK_AttendanceDetails_Student] FOREIGN KEY([StudentId])
REFERENCES [dbo].[Students] ([Id]);


ALTER TABLE [dbo].[AttendenceDetails] CHECK CONSTRAINT [FK_AttendanceDetails_Student];


END