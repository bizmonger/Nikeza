module Domain.Core exposing (..)


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
