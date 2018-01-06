module Nikeza.Mobile.UILogic.Pages

open Nikeza.Common
open Nikeza
open Nikeza.Mobile.Profile.Registration

type UnvalidatedForm = Nikeza.Mobile.Profile.Registration.UnvalidatedForm

type PageRequested =
    | Registration
    | RegistrationError of ValidatedForm
    | Portal            of DataTransfer.Profile
    | EditProfile       of UnvalidatedForm
                        
    | Latest            of DataTransfer.Profile
    | Subscriptions     of DataTransfer.Profile
    | Followers         of DataTransfer.Profile
                        
    | Portfolio         of ProviderId
    | Videos            of ProviderId
    | Articles          of ProviderId
    | Answers           of ProviderId
    | Podcasts          of ProviderId
                        
    | TopicLinks        of Profile