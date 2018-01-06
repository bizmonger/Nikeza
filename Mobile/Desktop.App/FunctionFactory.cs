using Microsoft.FSharp.Core;
using static Nikeza.Common;
using static Nikeza.Mobile.Profile.Registration;
using static Nikeza.Mobile.AppLogic.TestAPI;
using static Nikeza.DataTransfer;

namespace Desktop.App
{
    static class FunctionFactory
    {
        internal static FSharpFunc<ValidatedForm, FSharpResult<ProfileRequest, ValidatedForm>> SubmitRegistration() =>
            FSharpFunc<ValidatedForm, FSharpResult<ProfileRequest, ValidatedForm>>.FromConverter(mockSubmit);

        internal static FSharpFunc<ValidatedProfile, FSharpResult<ProfileRequest, ValidatedProfile>> SaveProfile() =>
            FSharpFunc<ValidatedProfile, FSharpResult<ProfileRequest, ValidatedProfile>>.FromConverter(mockSave);
    }
}
