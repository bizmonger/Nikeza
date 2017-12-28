module internal IO

open Commands
open Registration

type TrySubmit = ValidatedForm -> ResultOf

let trySubmit : TrySubmit = 
    fun form -> SubmitRegistration <| Error form