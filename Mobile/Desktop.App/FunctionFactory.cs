using Microsoft.FSharp.Core;
using static Nikeza.Common;
using static Nikeza.DataTransfer;
using static Nikeza.Mobile.Profile.Registration;
using static Nikeza.Mobile.UILogic.TestAPI;
using static Nikeza.Mobile.Subscriptions.Events;
using Microsoft.FSharp.Collections;

namespace Desktop.App
{
    static class FunctionFactory
    {
        internal static FSharpFunc<ValidatedForm, FSharpResult<ProfileRequest, ValidatedForm>> SubmitRegistration() =>
            FSharpFunc<ValidatedForm, FSharpResult<ProfileRequest, ValidatedForm>>.FromConverter(mockSubmit);

        internal static FSharpFunc<ValidatedProfile, FSharpResult<ProfileRequest, ValidatedProfile>> SaveProfile() =>
            FSharpFunc<ValidatedProfile, FSharpResult<ProfileRequest, ValidatedProfile>>.FromConverter(mockSave);

        internal static FSharpFunc<Unit, FSharpResult<FSharpList<Topic>, string>> GetTopics() =>
            FSharpFunc<Unit, FSharpResult<FSharpList<Topic>, string>>.FromConverter(mockTopics);

        internal static FSharpFunc<Unit, FSharpResult<FSharpList<string>, string>> GetPlatforms() =>
            FSharpFunc<Unit, FSharpResult<FSharpList<string>, string>>.FromConverter(mockPlatforms);

        internal static FSharpFunc<ProfileId, RecentQuery> GetRecent() =>
            FSharpFunc<ProfileId, RecentQuery>.FromConverter(mockRecent);

        internal static FSharpFunc<ProviderId, FSharpResult<ProviderRequest, ProviderId>> GetPortfolio() =>
            FSharpFunc<ProviderId, FSharpResult<ProviderRequest,ProviderId>>.FromConverter(mockPortfolio);

        internal static FSharpFunc<FSharpList<DataSourceSubmit>, FSharpResult<ProfileRequest, FSharpList<DataSourceSubmit>>> SaveSources() =>
            FSharpFunc<FSharpList<DataSourceSubmit>, FSharpResult<ProfileRequest, FSharpList<DataSourceSubmit>>>.FromConverter(mockSaveSources);
    }
}