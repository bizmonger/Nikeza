namespace Nikeza.Mobile.UILogic.Pages

open Nikeza.Common
open Nikeza.DataTransfer
open Nikeza.Access.Specification

type PageRequested =
    | Registration
    | RegistrationError of ValidatedForm error

    | Portal            of Profile
    | EditProfile       of Profile
                        
    | Latest            of Profile
    | Subscriptions     of Profile
    | Followers         of Profile
    | Members
                        
    | Portfolio         of Profile
    | PortfolioError    of ProviderId error

    | Videos            of ProviderId
    | Articles          of ProviderId
    | Answers           of ProviderId
    | Podcasts          of ProviderId
                        
    | TopicLinks        of Profile

    | Error             of string error