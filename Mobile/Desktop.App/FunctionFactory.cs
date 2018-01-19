using Microsoft.FSharp.Core;
using static Nikeza.Common;
using static Nikeza.DataTransfer;
using static Nikeza.Mobile.Profile.Registration;
using static Nikeza.Mobile.UILogic.TestAPI;
using static Nikeza.Mobile.Profile.Events;
using static Nikeza.Mobile.Subscriptions.Events;
using System.Collections.Generic;
using Microsoft.FSharp.Collections;

namespace Desktop.App
{
    static class FunctionFactory
    {
        internal static FSharpFunc<ValidatedForm, FSharpResult<ProfileRequest, ValidatedForm>> SubmitRegistration() =>
            FSharpFunc<ValidatedForm, FSharpResult<ProfileRequest, ValidatedForm>>.FromConverter(mockSubmit);

        internal static FSharpFunc<ValidatedProfile, FSharpResult<ProfileRequest, ValidatedProfile>> SaveProfile() =>
            FSharpFunc<ValidatedProfile, FSharpResult<ProfileRequest, ValidatedProfile>>.FromConverter(mockSave);

        internal static FSharpFunc<Unit, TopicsQuery> GetTopics() =>
            FSharpFunc<Unit, TopicsQuery>.FromConverter(mockTopics);

        internal static FSharpFunc<Unit, PlatformsQuery> GetPlatforms() =>
            FSharpFunc<Unit, PlatformsQuery>.FromConverter(mockPlatforms);

        internal static FSharpFunc<ProfileId, RecentQuery> GetRecent() =>
            FSharpFunc<ProfileId, RecentQuery>.FromConverter(mockRecent);

        internal static FSharpFunc<ProviderId, Nikeza.Mobile.Portfolio.Events.Query> GetPortfolio() =>
            FSharpFunc<ProviderId, Nikeza.Mobile.Portfolio.Events.Query>.FromConverter(mockPortfolio);

        internal static FSharpFunc<FSharpList<DataSourceSubmit>, FSharpResult<ProfileRequest, FSharpList<DataSourceSubmit>>> SaveSources() =>
            FSharpFunc<FSharpList<DataSourceSubmit>, FSharpResult<ProfileRequest, FSharpList<DataSourceSubmit>>>.FromConverter(mockSaveSources);
    }
}