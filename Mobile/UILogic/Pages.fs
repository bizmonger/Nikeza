module Nikeza.Mobile.UILogic.Pages

open Nikeza.Common
open Nikeza.DataTransfer
open Nikeza.Mobile.Access

type UnvalidatedForm = Nikeza.Mobile.Access.UnvalidatedForm

type PageRequested =
    | Registration
    | RegistrationError of ValidatedForm error

    | Portal            of Profile
    | EditProfile       of UnvalidatedForm
                        
    | Latest            of ProfileId
    | Subscriptions     of ProfileId
    | Followers         of ProfileId
    | Members
                        
    | Portfolio         of Provider
    | PortfolioError    of ProviderId error

    | Videos            of ProviderId
    | Articles          of ProviderId
    | Answers           of ProviderId
    | Podcasts          of ProviderId
                        
    | TopicLinks        of Profile

    | Error             of string error