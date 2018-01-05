module Nikeza.Mobile.UILogic.Pages

open Nikeza.DataTransfer
open Nikeza.Common

type UnvalidatedForm = Nikeza.Mobile.Profile.Registration.UnvalidatedForm

type PageRequested =
    | Registration
    | RegistrationError of Profile
    | Portal            of Profile
    | EditProfile       of UnvalidatedForm
                        
    | Latest            of Profile
    | Subscriptions     of Profile
    | Followers         of Profile
                        
    | Portfolio         of ProviderId
    | Videos            of ProviderId
    | Articles          of ProviderId
    | Answers           of ProviderId
    | Podcasts          of ProviderId
                        
    | TopicLinks        of Profile