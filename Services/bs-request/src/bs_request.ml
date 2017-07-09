type request
type response
type body = string
type error =
  < stack : string Js.undefined ;
    message : string Js.undefined ;
    name : string Js.undefined;
    fileName : string Js.undefined
  > Js.t

module Body = struct
  type t = body

  
end 

module Response = struct
  type t = response

  external statusCode: t -> int = "" [@@bs.get]
  external headers: t -> string array = "" [@@bs.get]
end

external get: string -> request = "" [@@bs.module "request"]
external post: string -> 'a -> request = "" [@@bs.module "request"]
external on: 
  (
  [ `response of response -> unit 
  | `error of error -> unit
  | `body of body -> string Js.Promise.t  
  ]
  [@bs.string])
  -> response = "" [@@bs.send.pipe: request]


let get uri = 
  get uri
  |> on (`response (fun response -> Js.log(Response.statusCode(response)) ))
  |> on (`body (fun body -> Js.Promise.resolve(body)))

let post uri data = post uri data
  