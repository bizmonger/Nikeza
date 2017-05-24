module Domain.Core exposing (..)


type alias Credentials =
    { username : String, password : String }


type Submitter
    = Submitter String


type Title
    = Title String


type Url
    = Url String


type Video
    = Video Post


type Article
    = Article Post


type alias Post =
    { submitter : Submitter, title : Title, url : Url }
