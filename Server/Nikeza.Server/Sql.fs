module Nikeza.Server.Sql
open System.IO
open Literals

let connectionString = 
      "Data Source=DESKTOP-GE7O8JT\\SQLEXPRESS;Initial Catalog=Nikeza;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"
      //File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(),KeyFile_SqlConnection))

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

let deleteSourceLinkSql = @"DELETE FROM SourceLink WHERE SourceId = @SourceId"

let followSql = @"INSERT INTO Subscription (SubscriberId,ProfileId)

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

let unfeatureTopicSql = @"DELETE 
                          FROM  [dbo].[FeaturedTopic]
                          WHERE  ProfileId = @ProfileId AND TopicId = @TopicId"

let clearFeaturedTopicsSql = @"DELETE 
                               FROM  [dbo].[FeaturedTopic]
                               WHERE  ProfileId = @ProfileId"
                               
let getFeaturedTopicsSql = @"SELECT      Topic.Id, FeaturedTopic.ProfileId, Topic.Name
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

                   

let getLinkTopicsSql = "SELECT       Topic.Id, 
                                     Topic.Name, 
                                     isnull( (select top (1) 1 from FeaturedTopic where TopicId = Topic.Id),0) as IsFeatured 

                        FROM         Topic
                        INNER JOIN   LinkTopic
                                        ON   LinkTopic.TopicId = Topic.Id
                        INNER JOIN   Link
                                        ON   LinkTopic.LinkId =  Link.Id
                        WHERE        Link.Id = @LinkId"
                        

let deleteLinkTopicSql = "Delete     LinkTopic
                          FROM       LinkTopic
                          INNER JOIN Topic
                                        ON   LinkTopic.TopicId = Topic.Id
                          INNER JOIN Link
                                        ON   LinkTopic.LinkId = Link.Id
                          WHERE  Link.Id = @LinkId"

let getProviderTopicsSql = "SELECT     Topic.Id, Topic.Name
                            FROM       Topic
                            INNER JOIN LinkTopic
                                  ON   Topic.Id = LinkTopic.TopicId 
                            INNER JOIN Link
                                  ON Link.Id = LinkTopic.LinkId
                            WHERE    Link.ProfileId = @ProfileId"
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

let getSourceLinksBySourceIdSql = 
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
                   
                   WHERE        Source.Id = @SourceId"



let deleteLinksFromSourceSql = 
                  "DELETE       Link
                   FROM         Link
                   INNER JOIN   SourceLinks
                   ON           SourceLinks.LinkId   = Link.Id 
                   WHERE        SourceLinks.SourceId = @SourceId"

let deleteSourceLinksSql = 
                  "DELETE SourceLinks
                   WHERE  SourceLinks.SourceId = @SourceId"

let getRecentSql = @"SELECT Top (3) Link.Id, 
                                    Link.ProfileId, 
                                    Link.Title, 
                                    Link.Description, 
                                    Link.Url, 
                                    Link.ContentTypeId, 
                                    Link.IsFeatured, 
                                    Link.Created

                     FROM           Link
                     INNER JOIN     Subscription
                           ON       Subscription.SubscriberId = @SubscriberId  AND
                                    Subscription.ProfileId =    Link.ProfileId

                     WHERE          Link.Id NOT IN 
                                           (SELECT ObservedLinks.LinkId
					                        FROM   ObservedLinks
                                            WHERE  ObservedLinks.SubscriberId = @SubscriberId)"
                                     
let getLatestLinksSql = @"SELECT TOP (3) 
                                 Link.Id, 
                                 Link.ProfileId, 
                                 Link.Title, 
                                 Link.Description, 
                                 Link.Url, 
                                 Link.ContentTypeId, 
                                 Link.IsFeatured, 
                                 Link.Created
                                 
                          FROM      Link

                          WHERE     Link.ProfileId = @ProfileId
                          ORDER BY  Link.Created DESC"
                                     

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

let lastSynchedSql = @"Select SourceId,
                              LastSynched
                       FROM   SyncHistory
                       WHERE  SourceId = @SourceId"

let updateSyncHistorySql = @"Update SyncHistory
                             Set    LastSynched = CURRENT_TIMESTAMP
                             WHERE   SourceId = @SourceId"

let addSyncHistorySql = @"INSERT INTO SyncHistory
                          OUTPUT INSERTED.ID
                          VALUES      (SourceId, CURRENT_TIMESTAMP)"

let getSyncHistorySql = @"SELECT LastSynched
                          FROM   SyncHistory
                          WHERE  SourceId = @SourceId"

let getPlatformsSql = @"SELECT Name FROM Platform"

let getUsernameToIdSql = @"SELECT Id FROM Profile WHERE Email = @Email"