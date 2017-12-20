module Model

open Nikeza.Common

type Email =      Email      of string
type Password =   Password   of string
type EntityName = EntityName of string
type Id =         Id         of string
type Url =        Url        of string
type FirstName =  FirstName  of string
type LastName =   LastName   of string

type FeatureLinkRequest = { 
    LinkId:     Id
    isFeatured: bool 
}

type FeatureTopicsrequest = { 
    ProfileId: Id
    Names:     string list
}

type Profile = {
    Id :         Id
    Email :      Email
    EntityName : EntityName
}

type Provider = {
    Profile:     Profile
    RecentLinks: Link list
}

    module Registration =
        type Form = {
            Email:    Email
            Password: Password
            Confirm:  Password
        }

        type UnvalidatedForm = { Form : Form }
        type ValidatedForm =   { Form : Form }

    module ProfileForm =
        type Form = {
            Email:       Email
            EntityName : EntityName
            ImageUrl :   Url
            FirstName :  FirstName option
            LastName :   LastName  option
        }

        type Unvalidated = { Form : Form }
        type Validated =   { Form : Form }