ALTER TABLE [dbo].[form_fields] ADD 
[MaxFilesizeInKb] [int] NULL CONSTRAINT [DF_form_fields_MaxFilesizeInKb] DEFAULT ((5000)),
[ValidFileExtensions] [nvarchar] (50) NULL,
[MinFilesizeInKb] [int] NULL CONSTRAINT [DF_form_fields_MinFilesizeInKb] DEFAULT ((50))
GO
ALTER TABLE [dbo].[form] ADD 
[Theme] [nvarchar] (50) NULL,
[NotificationEmail] [nvarchar] (50) NULL
GO
