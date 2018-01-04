module Nikeza.Mobile.UILogic.Adapter

open Nikeza.Mobile.Profile.Registration

type Form = {
    Email:string
    Password:string
    Confirm:string
}

type UIForm = Form
type DomainForm = Nikeza.Mobile.Profile.Registration.Form

let ofUnvalidated (form:UIForm) : UnvalidatedForm = {
    UnvalidatedForm.Form= { Email =    Email    form.Email
                            Password = Password form.Password
                            Confirm =  Password form.Password
                          }
}