USE [master]
GO

/****** Object:  Database [Nikeza]    Script Date: 8/5/2017 7:56:58 AM ******/
DROP DATABASE [Nikeza]
GO

/****** Object:  Database [Nikeza]    Script Date: 8/5/2017 7:56:58 AM ******/
CREATE DATABASE [Nikeza]
GO

USE [Nikeza]
GO
/****** Object:  Table [dbo].[ContentType]    Script Date: 15-Aug-17 5:25:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ContentType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Type] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_ContentType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Link]    Script Date: 15-Aug-17 5:25:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Link](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProviderId] [int] NOT NULL,
	[Title] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](500) NOT NULL,
	[Url] [nvarchar](100) NOT NULL,
	[ContentTypeId] [int] NOT NULL,
	[IsFeatured] [bit] NOT NULL,
	[Created] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Link] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Profile]    Script Date: 15-Aug-17 5:25:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Profile](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](20) NOT NULL,
	[LastName] [nvarchar](20) NOT NULL,
	[Email] [nvarchar](50) NOT NULL,
	[ImgUrl] [nvarchar](50) NOT NULL,
	[Bio] [nvarchar](1000) NOT NULL,
	[PasswordHash] [nvarchar](100) NOT NULL,
	[Created] [datetime2](7) NOT NULL,
	[Salt] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_Profile] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProfileLinks]    Script Date: 15-Aug-17 5:25:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProfileLinks](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProfileId] [int] NOT NULL,
	[LinkId] [int] NOT NULL,
 CONSTRAINT [PK_ProfileLinks] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProfileTopics]    Script Date: 15-Aug-17 5:25:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProfileTopics](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProfileId] [int] NOT NULL,
	[TopicId] [int] NOT NULL,
 CONSTRAINT [PK_ProfileTopics] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProviderSources]    Script Date: 15-Aug-17 5:25:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProviderSources](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProfileId] [int] NOT NULL,
	[SourceId] [int] NOT NULL,
 CONSTRAINT [PK_ProviderSources] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Source]    Script Date: 15-Aug-17 5:25:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Source](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProfileId] [int] NOT NULL,
	[Platform] [nvarchar](50) NOT NULL,
	[UserName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Source] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Subscription]    Script Date: 15-Aug-17 5:25:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Subscription](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProfileId] [int] NOT NULL,
	[ProviderId] [int] NOT NULL,
 CONSTRAINT [PK_Subscription] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Topic]    Script Date: 15-Aug-17 5:25:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Topic](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_Topic] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[ProfileLinks]  WITH CHECK ADD  CONSTRAINT [FK_ProfileLinks_Link] FOREIGN KEY([LinkId])
REFERENCES [dbo].[Link] ([Id])
GO
ALTER TABLE [dbo].[ProfileLinks] CHECK CONSTRAINT [FK_ProfileLinks_Link]
GO
ALTER TABLE [dbo].[ProfileLinks]  WITH CHECK ADD  CONSTRAINT [FK_ProfileLinks_Profile] FOREIGN KEY([ProfileId])
REFERENCES [dbo].[Profile] ([Id])
GO
ALTER TABLE [dbo].[ProfileLinks] CHECK CONSTRAINT [FK_ProfileLinks_Profile]
GO
ALTER TABLE [dbo].[ProfileTopics]  WITH CHECK ADD  CONSTRAINT [FK_ProfileTopics_Profile] FOREIGN KEY([ProfileId])
REFERENCES [dbo].[Profile] ([Id])
GO
ALTER TABLE [dbo].[ProfileTopics] CHECK CONSTRAINT [FK_ProfileTopics_Profile]
GO
ALTER TABLE [dbo].[ProfileTopics]  WITH CHECK ADD  CONSTRAINT [FK_ProfileTopics_Topic] FOREIGN KEY([TopicId])
REFERENCES [dbo].[Topic] ([Id])
GO
ALTER TABLE [dbo].[ProfileTopics] CHECK CONSTRAINT [FK_ProfileTopics_Topic]
GO
ALTER TABLE [dbo].[ProviderSources]  WITH CHECK ADD  CONSTRAINT [FK_ProviderSources_Profile] FOREIGN KEY([ProfileId])
REFERENCES [dbo].[Profile] ([Id])
GO
ALTER TABLE [dbo].[ProviderSources] CHECK CONSTRAINT [FK_ProviderSources_Profile]
GO
ALTER TABLE [dbo].[ProviderSources]  WITH CHECK ADD  CONSTRAINT [FK_ProviderSources_Source] FOREIGN KEY([SourceId])
REFERENCES [dbo].[Source] ([Id])
GO
ALTER TABLE [dbo].[ProviderSources] CHECK CONSTRAINT [FK_ProviderSources_Source]
GO
ALTER TABLE [dbo].[Source]  WITH CHECK ADD  CONSTRAINT [FK_Source_Source] FOREIGN KEY([ProfileId])
REFERENCES [dbo].[Profile] ([Id])
GO
ALTER TABLE [dbo].[Source] CHECK CONSTRAINT [FK_Source_Source]
GO
ALTER TABLE [dbo].[Subscription]  WITH CHECK ADD  CONSTRAINT [FK_Subscription_Profile] FOREIGN KEY([ProfileId])
REFERENCES [dbo].[Profile] ([Id])
GO
ALTER TABLE [dbo].[Subscription] CHECK CONSTRAINT [FK_Subscription_Profile]
GO
