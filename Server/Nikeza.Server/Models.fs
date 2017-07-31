namespace Nikeza.Server.Models

open System

type Profile = {
    ProfileId: int
    FirstName: string
    LastName: string
    Email: string
    ImageUrl: string
    Bio: string
    Created: DateTime
}