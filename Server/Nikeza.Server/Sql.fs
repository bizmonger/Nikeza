module Nikeza.Server.Sql

let connectionString = "Data Source=DESKTOP-GE7O8JT\\SQLEXPRESS;Initial Catalog=Nikeza;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"

let findUserByEmailSql = "SELECT * FROM Profile Where Email = @Email"

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

let addLinkTopicSql = @"INSERT INTO [dbo].LinkTopic
                        (LinkId , TopicId)
                        OUTPUT INSERTED.ID
                        VALUES (@LinkId, @TopicId)"

let addTopicSql = @"INSERT INTO [dbo].Topic
                        (Name)
                        OUTPUT INSERTED.ID
                        VALUES (@Name)"

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
                               ,AccessId)
                    
                          OUTPUT INSERTED.ID

                          VALUES
                                (@ProfileId
                                ,@Platform
                                ,@AccessId)"

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
let observeLinkSql = @"INSERT INTO [dbo].[ObservedLinks]
                      (SubscriberId
                      ,LinkId)

                OUTPUT INSERTED.ID

                VALUES
                       ( @SubscriberId 
                       , @LinkId
                       )"
let featureLinkSql = @"UPDATE Link
                       SET    IsFeatured = @IsFeatured
                       WHERE  Id = @Id"
let featureTopicSql = @"INSERT INTO [dbo].[FeaturedTopic]
                       (ProfileId, TopicId)

                       OUTPUT INSERTED.ID

                       VALUES
                        (@ProfileId, @TopicId)"

let unfeatureTopicSql = @"DELETE FROM [dbo].[FeaturedTopic]
                         WHERE ProfileId = @ProfileId AND TopicId = @TopicId"

let getFeaturedTopicsSql = @"SELECT      FeaturedTopic.TopicId, FeaturedTopic.ProfileId, Topic.Name
                             FROM        FeaturedTopic
                             INNER JOIN  Topic
                                   ON    Topic.Id = FeaturedTopic.TopicId
                             WHERE       FeaturedTopic.ProfileId = @ProfileId"

let updateProfileSql = @"UPDATE [dbo].[Profile]
                        SET     [FirstName] = @FirstName,
                                [LastName] =  @LastName,
                                [Bio] =       @bio,
                                [Email] =     @email
                        WHERE   Id =          @Id"
let updateThumbnailSql = @"UPDATE [dbo].[Profile]
                           SET     [ImageUrl] =  @ImageUrl
                           WHERE   Id =          @ProfileId"
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

let getTopicSql = "SELECT Id, Name
                   FROM   [dbo].[Topic]
                   WHERE  Name = @Name"
let getSourceLinksSql = 
                  "SELECT       Link.Id,
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

let getRecentSql = @"SELECT     Link.Id, 
                                Link.ProfileId, 
                                Link.Title, 
                                Link.Description, 
                                Link.Url, 
                                Link.ContentTypeId, 
                                Link.IsFeatured, 
                                Link.Created
                     FROM       Link
                     WHERE Link.Id NOT IN 
                                  (SELECT LinkId
					               FROM   ObservedLinks
                                   WHERE  ObservedLinks.SubscriberId = SubscriberId)"
                                     

let getFollowersSql = @"SELECT Profile.Id,
                               Profile.FirstName,
                               Profile.LastName,
                               Profile.Email,
                               Profile.ImageUrl,
                               Profile.Bio,
                               PasswordHash,
                               Salt,         
                               Created

                       FROM       Profile
                       INNER JOIN Subscription
                       ON         Subscription.SubscriberId = Profile.Id
                       WHERE      Subscription.ProfileId = @ProfileId"

let getSubscriptionsSql = @"SELECT Profile.Id,
                                   Profile.FirstName,
                                   Profile.LastName,
                                   Profile.Email,
                                   Profile.ImageUrl,
                                   Profile.Bio,
                                   PasswordHash,
                                   Salt,         
                                   Created

                            FROM       Profile
                            INNER JOIN Subscription
                            ON         Subscription.ProfileId =   Profile.Id
                            WHERE      Subscription.SubscriberId = @SubscriberId"

let filterOnProfileId = "WHERE [Profile].Id = @ProfileId"

let getProfilesSql = @"SELECT  Id,
                               FirstName,
                               LastName,
                               Email,
                               ImageUrl,
                               Bio,
                               PasswordHash,
                               Salt,         
                               Created

                       FROM    Profile"

let getProfileSql = @"SELECT  Id,
                              FirstName,
                              LastName,
                              Email,
                              ImageUrl,
                              Bio,
                              PasswordHash,
                              Salt,         
                              Created
                              
                       FROM   Profile
                       WHERE  Id = @ProfileId"
let getSourcesSql = @"SELECT Id,
                             ProfileId,
                             Platform,
                             AccessId
                      FROM   Source
                      WHERE  ProfileId = @ProfileId"
let getSourceSql = @"SELECT  Source.Id,
                             Source.ProfileId,
                             Source.Platform,
                             Source.AccessId
                      FROM   Source
                      WHERE  Id = @SourceId"

let getPlatformsSql = @"SELECT Name FROM Platform"

let getUsernameToIdSql = @"SELECT Id FROM Profile WHERE Email = @Email"