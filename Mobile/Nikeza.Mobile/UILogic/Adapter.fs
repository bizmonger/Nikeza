module Nikeza.Mobile.UILogic.Adapter

open Nikeza.Mobile.Profile.Registration

type UIForm = Nikeza.Mobile.UILogic.Registration.Types.Form
type DomainForm = Nikeza.Mobile.Profile.Registration.Form

let toDomainForm (form:UIForm) : DomainForm = { 
        Email =    Email    form.Email
        Password = Password form.Password
        Confirm =  Password form.Password
}