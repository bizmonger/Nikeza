module Nikeza.Mobile.AppLogic.Pages

open Nikeza.DataTransfer

type UnvalidatedForm = Nikeza.Mobile.Profile.Registration.UnvalidatedForm

type PageRequested =
    | ErrorRegistering of Profile
    | Portal           of Profile
    | EditProfile      of UnvalidatedForm
    | Registration
    | Latest           of Profile
    | Subscriptions    of Profile
    | Followers        of Profile
    | Portfolio        of Profile
    | ContentTypeLinks of Profile
    | TopicLinks       of Profile