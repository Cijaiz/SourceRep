/****** Object:  Table [dbo].[UserNotification]    Script Date: 07/17/2013 16:05:23 ******/
/****** Object:  Table [dbo].[UserLog]    Script Date: 07/17/2013 16:05:23 ******/
/****** Object:  Table [dbo].[UserGroup]    Script Date: 07/17/2013 16:05:23 ******/
SET IDENTITY_INSERT [dbo].[UserGroup] ON
INSERT [dbo].[UserGroup] ([Id], [UserId], [GroupId], [CreatedOn]) VALUES (1, 1, 1, CAST(0x0000A1A800A93189 AS DateTime))
SET IDENTITY_INSERT [dbo].[UserGroup] OFF
/****** Object:  Table [dbo].[UserDetail]    Script Date: 07/17/2013 16:05:23 ******/
INSERT [dbo].[UserDetail] ([UserId], [DisplayName], [Email], [PrivacyStatus]) VALUES (1, N'SiteAdmin', N'hr@cognizant.com', 2)
