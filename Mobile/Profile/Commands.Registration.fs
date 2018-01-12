module Nikeza.Mobile.Profile.Commands.Registration

//module Registration =
    type Validate =  Execute of UnvalidatedForm
    type Command =   Execute of ValidatedForm

    module Validate =
        module ResultOf = type Validate = Executed of Result<ValidatedForm, UnvalidatedForm>

    module Submit =
        module ResultOf = type Submit = Executed of Result<Nikeza.DataTransfer.Profile, ValidatedForm>