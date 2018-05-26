module Nikeza.Mobile.UILogic.Adapter

open Nikeza.Access.Specification

type Form = {
    Email:string
    Password:string
    Confirm:string
}

let ofUnvalidated (form:Form) : UnvalidatedForm = {
    UnvalidatedForm.Form= { Email =    Email    form.Email
                            Password = Password form.Password
                            Confirm =  Password form.Confirm
    }
}