namespace Nikeza.Access.Specification

type Email =      Email      of string
type Password =   Password   of string
type EntityName = EntityName of string
type FirstName =  FirstName  of string
type LastName =   LastName   of string

type Form = {
    Email:    Email
    Password: Password
    Confirm:  Password
}

type UnvalidatedForm = Unvalidated of Form
type ValidatedForm =   Validated   of Form