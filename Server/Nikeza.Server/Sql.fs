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
                          (ProviderId
                          ,Title
                          ,Description
                          ,Url
                          ,ContentTypeId
                          ,IsFeatured
                          ,Created)
                    
                    OUTPUT INSERTED.ID

                    VALUES
                          (@ProviderId
                          ,@Title
                          ,@Description
                          ,@Url
                          ,@ContentTypeId
                          ,@IsFeatured
                          ,@Created)"

let deleteLinkSql = @"DELETE FROM Link WHERE Id = @LinkId"

let addSourceSql = @"INSERT INTO [dbo].[Source]
                          (ProfileId
                          ,Platform,
                           Username)
                    
                    OUTPUT INSERTED.ID

                    VALUES
                          (@ProfileId
                          ,@Platform
                          ,@Username)"

let deleteSourceSql = @"DELETE FROM Source WHERE Id = @Id"

let followSql = @"INSERT INTO [dbo].[Subscription]
                      (SubscriberId
                      ,ProviderId)

                OUTPUT INSERTED.ID

                VALUES
                       ( @SubscriberId 
                       , @ProviderId
                       )"

let unsubscribeSql = @"DELETE FROM [dbo].[Subscription]
                       WHERE SubscriberId  = @SubscriberId AND 
                             ProviderId =    @ProviderId"

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
                          ProviderId, 
                          Title, 
                          Description, 
                          Url, 
                          ContentTypeId, 
                          IsFeatured, 
                          Created

                   FROM   [dbo].[Link]
                   WHERE  ProviderId = @ProviderId"

let getFollowersSql = @"SELECT Profile.Id,
                               Profile.FirstName,
                               Profile.LastName,
                               Profile.Email,
                               Profile.ImageUrl,
                               Profile.Bio

                       FROM       Profile
                       INNER JOIN Subscription
                       ON         Subscription.SubscriberId = Profile.Id
                       WHERE      Subscription.ProviderId = @ProviderId"

let getSubscriptionsSql = @"SELECT Profile.Id,
                                   Profile.FirstName,
                                   Profile.LastName,
                                   Profile.Email,
                                   Profile.ImageUrl,
                                   Profile.
                                   FROM       Profile
                                   INNER JOIN Subscription
                                   ON         Subscription.ProviderId =   Profile.Id
                                   WHERE      Subscription.SubscriberId = @SubscriberId"

let getProvidersSql = @"SELECT Id,
                               FirstName,
                               LastName,
                               Email,
                               ImageUrl,
                               Bio

                       FROM    Profile"

let getProviderSql = @"SELECT Id,
                              FirstName,
                              LastName,
                              Email,
                              ImageUrl,
                              Bio
                       FROM   Profile
                       WHERE  Id = @ProviderId"

let getSourcesSql = @"SELECT ProfileId,
                             Platform,
                             Username
                      FROM   Source
                      WHERE  ProfileId = @ProfileId"

let getPlatformsSql = @"SELECT Name FROM Platform"

let getUsernameToIdSql = @"SELECT Id FROM Profile WHERE Email = @Email"