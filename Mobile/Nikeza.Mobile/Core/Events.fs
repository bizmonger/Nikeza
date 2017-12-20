module Events

open Model

type TopicEvents = TopicsFeatured of FeatureTopicsrequest

type NotificationEvents =
    | ContentDiscovered of Provider
    | NewSubscriber

type LinkEvents =
    | LinkFeatured   of FeatureLinkRequest
    | LinkUnfeatured of FeatureLinkRequest

type RegistrationEvents =
    | FormValidated         of Registration.UnvalidatedForm
    | FormSubmitted         of Registration.ValidatedForm
    | RegistrationSucceeded of Profile
    | RegistrationFailed    of Registration.ValidatedForm
    | LoginRequested        of Id
    
type AuthenticationEvents =
    | LoggedIn
    | LoggedOut

type AppEvents =
    | AppLoaded
    | AppSuspended
    | AppExiting
    | UnhandledException

type ProfileEvents =
    | ProfileRequested of Id
    | ProfileSaved     of ProfileForm.Validated
    | Subscribed       of Id
    | Unsubscribed     of Id

type PageRequested =
    | Portal
    | EditProfile of ProfileForm.Unvalidated
    | Registration
    | Latest
    | Subscriptions
    | Followers
    | Portfolio
    | ContentTypeLinks
    | TopicLinks