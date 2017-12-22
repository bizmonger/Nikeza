module Noeza.Mobile.Core.Events

open Registration

type PageRequested =
    | Portal           of Profile
    | EditProfile      of Registration.Unvalidated
    | Registration
    | Latest           of Profile
    | Subscriptions    of Profile
    | Followers        of Profile
    | Portfolio        of Profile
    | ContentTypeLinks of Profile
    | TopicLinks       of Profile