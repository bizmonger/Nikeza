module Nikeza.Server.Sql

let connectionString = "Data Source=DESKTOP-GE7O8JT\\SQLEXPRESS;Initial Catalog=Nikeza;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"

let findUserByEmailSql = "SELECT * FROM Profile Where Email = @email"
let registerSql = @"INSERT INTO [dbo].[Profile]
                          ( FirstName
                          , LastName
                          , Email
                          , ImageUrl
                          , Bio
                          , PasswordHash
                          , Created
                          , Salt )

                   OUTPUT INSERTED.ID

                   VALUES
                          ( @FirstName
                          , @LastName
                          , @Email
                          , @ImageUrl
                          , @Bio
                          , @PasswordHash
                          , @Created
                          , @Salt
                          )"
   
let addLinkSql = @"INSERT INTO [dbo].[Link]
                          (ProfileId
                          ,Title
                          ,Description
                          ,Url
                          ,ContentTypeId
                          ,IsFeatured
                          ,Created)
                    
                    OUTPUT INSERTED.ID

                    VALUES
                          (@ProfileId
                          ,@Title
                          ,@Description
                          ,@Url
                          ,@ContentTypeId
                          ,@IsFeatured
                          ,@Created)"

let deleteLinkSql = @"DELETE FROM Link WHERE Id = @LinkId"

let addDataSourceSql = @"INSERT INTO [dbo].[Source]
                          (ProfileId
                           ,Platform
                           ,AccessId
                           ,APIKey)
                    
                          OUTPUT INSERTED.ID

                          VALUES
                                (@ProfileId
                                ,@Platform
                                ,@AccessId
                                ,@APIKey)"

let addSourceLinkSql = @"INSERT INTO [dbo].[SourceLinks]
                          (SourceId
                           ,LinkId)
                    
                          OUTPUT INSERTED.ID

                          VALUES
                                (@SourceId
                                ,@LinkId)"

let deleteSourceSql = @"DELETE FROM Source WHERE Id = @Id"

let followSql = @"INSERT INTO [dbo].[Subscription]
                      (SubscriberId
                      ,ProfileId)

                OUTPUT INSERTED.ID

                VALUES
                       ( @SubscriberId 
                       , @ProfileId
                       )"

let unsubscribeSql = @"DELETE FROM [dbo].[Subscription]
                       WHERE SubscriberId  = @SubscriberId AND 
                             ProfileId =    @ProfileId"

let featureLinkSql = @"UPDATE Link
                       SET    IsFeatured = @IsFeatured
                       WHERE  Id = @Id"

let updateProfileSql = @"UPDATE [dbo].[Profile]
                        SET     [FirstName] = @FirstName,
                                [LastName] =  @LastName,
                                [Bio] =       @bio,
                                [Email] =     @email
                        WHERE   Id =          @Id"

let getLinksSql = "SELECT Id, 
                          ProfileId, 
                          Title, 
                          Description, 
                          Url, 
                          ContentTypeId, 
                          IsFeatured, 
                          Created

                   FROM   [dbo].[Link]
                   WHERE  ProfileId = @ProfileId"

let getSourceLinksSql = 
                  "SELECT Link.Id, 
                          Link.ProfileId, 
                          Link.Title, 
                          Link.Description, 
                          Link.Url, 
                          Link.ContentTypeId, 
                          Link.IsFeatured, 
                          Link.Created

                   FROM         Link
                   INNER JOIN   Source
                   ON           Link.ProfileId = Source.ProfileId
                   INNER JOIN   SourceLinks
                   ON           SourceLinks.LinkId   = Link.Id   AND 
                                SourceLinks.SourceId = Source.Id
                   
                   WHERE        Link.ProfileId = @ProfileId AND Source.Platform = @Platform"

let getFollowersSql = @"SELECT Profile.Id,
                               Profile.FirstName,
                               Profile.LastName,
                               Profile.Email,
                               Profile.ImageUrl,
                               Profile.Bio

                       FROM       Profile
                       INNER JOIN Subscription
                       ON         Subscription.SubscriberId = Profile.Id
                       WHERE      Subscription.ProfileId = @ProfileId"

let getSubscriptionsSql = @"SELECT Profile.Id,
                                   Profile.FirstName,
                                   Profile.LastName,
                                   Profile.Email,
                                   Profile.ImageUrl,
                                   Profile.Bio
                                   FROM       Profile
                                   INNER JOIN Subscription
                                   ON         Subscription.ProfileId =   Profile.Id
                                   WHERE      Subscription.SubscriberId = @SubscriberId"

let filterOnProfileId = "WHERE [Profile].Id = @ProfileId"

let getProvidersSql =
    @"SELECT	[Profile].Id, 
                [Profile].FirstName, 
		        [Profile].LastName, 
		        [Profile].Email, 
		        [Profile].ImageUrl, 
		        [Profile].Bio, 
		        [Topic].Name as TopicName,
		        [Link].Title as LinkTitle,
		        [Link].Url as LinkUrl,
		        [ContentType].Type as LinkContentType,
		        [Link].IsFeatured as LinkFeatured,
		        [Link].Description as LinkDescription,
		        [Link].Created as LinkPostedDate

    FROM	Profile

    INNER JOIN	ProfileTopics
			    ON [Profile].Id = [ProfileTopics].ProfileId
    INNER JOIN	Topic
			    ON [Topic].Id =   [ProfileTopics].TopicId
    INNER JOIN ContentType
			    ON [Link].ContentTypeId = [Link].ContentTypeId"

let getProviderSql = getProvidersSql + " " + filterOnProfileId

let getProfilesSql = @"SELECT  Id,
                               FirstName,
                               LastName,
                               Email,
                               ImageUrl,
                               Bio

                       FROM    Profile"

let getProfileSql = @"SELECT  Id,
                              FirstName,
                              LastName,
                              Email,
                              ImageUrl,
                              Bio
                       FROM   Profile
                       WHERE  Id = @ProfileId"

let getSourcesSql = @"SELECT Id,
                             ProfileId,
                             Platform,
                             Username
                      FROM   Source
                      WHERE  ProfileId = @ProfileId"

let getSourceSql = @"SELECT  Id,
                             ProfileId,
                             Platform,
                             Username
                      FROM   Source
                      WHERE  ProfileId = @SourceId"

let getPlatformsSql = @"SELECT Name FROM Platform"

let getUsernameToIdSql = @"SELECT Id FROM Profile WHERE Email = @Email"