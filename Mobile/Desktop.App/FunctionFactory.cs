using Microsoft.FSharp.Core;
using static Nikeza.Common;
using static Nikeza.DataTransfer;
using static Nikeza.Mobile.Profile.Registration;
using static Nikeza.Mobile.UILogic.TestAPI;
using static Nikeza.Mobile.Profile.Events;

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
    }
}