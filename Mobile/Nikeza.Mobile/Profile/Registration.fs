module Registration

open Nikeza.Common

type Email =      Email      of string
type Password =   Password   of string
type EntityName = EntityName of string
type FirstName =  FirstName  of string
type LastName =   LastName   of string

//type Profile = {
//    Id :         Id
//    Email :      Email
//    EntityName : EntityName
//}

type Form = {
    Email:    Email
    Password: Password
    Confirm:  Password
}

type UnvalidatedForm = { Form : Form }
type ValidatedForm =   { Form : Form }