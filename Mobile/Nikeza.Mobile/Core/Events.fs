module Events

open Model

type TopicEvents = TopicsFeatured of FeatureTopicsrequest

type NotificationEvents =
    | ContentDiscovered of Provider
    | NewSubscriber     of Profile

type LinkEvents =
    | LinkFeatured   of FeatureLinkRequest
    | LinkUnfeatured of FeatureLinkRequest

type RegistrationEvents =
    | FormValidated         of Registration.UnvalidatedForm
    | FormNotValidated      of Registration.UnvalidatedForm
    | FormSubmitted         of Registration.ValidatedForm
    | RegistrationSucceeded of Profile
    | RegistrationFailed    of Registration.ValidatedForm
    | LoginRequested        of Id

type AuthenticationEvents =
    | LoggedIn  of Profile
    | LoggedOut of Profile

type ProfileEvents =
    | ProfileRequested of Id
    | ProfileSaved     of ProfileForm.Validated
    | Subscribed       of Id
    | Unsubscribed     of Id

type PageRequested =
    | Portal           of Profile
    | EditProfile      of ProfileForm.Unvalidated
    | Registration
    | Latest           of Profile
    | Subscriptions    of Profile
    | Followers        of Profile
    | Portfolio        of Profile
    | ContentTypeLinks of Profile
    | TopicLinks       of Profile