type tag = {
  count: int,
  name: string
};

let decodeJsonArray data => Js.Json.decodeArray @@ Js.Json.parseExn data;

let decodeTag data => {
    let name = data |> Json.Decode.(field "name" string);
    let count = data |> Json.Decode.(field "count" int);
    { name, count };
};

let decodeTagArray someArray => 
switch someArray {
    | Some array => Some (Array.to_list @@ Array.map decodeTag array)
    | _ => None
  };

type tags = array (option tag);
let data =
  Node.Fs.readFileAsUtf8Sync "data.json"
  |> decodeJsonArray
  |> decodeTagArray;

let startsWith str target => 
  Js.Re.fromStringWithFlags ("^" ^ str) flags::"i"
  |> Js.Re.test target;

let findTagByName name =>
  switch data {
    | Some lst => Some (List.filter (fun x => startsWith name x.name) lst)
    | None => None
  };

open Express;

let getDictString dict key => {
  switch (Js.Dict.get dict key) {
    | Some json => Js.Json.decodeString json
    | _ => None
  };
};

let app = express ();

App.get app path::"/tags/:tag" @@ Middleware.from(fun req res next => {
  switch (getDictString (Request.params req) "tag") {
    | Some name => name
    | _ => ""
  }
  |> (fun name => findTagByName name)
  |> (fun someTag => 
    switch someTag {
      | Some tags => {
        open Json.Encode;
        let encodeTag tag => object_ [ ("name", string tag.name), ("count", int tag.count)];
        let jsonTags = list encodeTag tags;
          
        Response.sendJson res (object_ [ ("tags", jsonTags) ] );
      }
      | None => Response.sendString res "Failed to find tag";
    }
  )
});


let onListen port e =>
  switch e {
  | exception (Js.Exn.Error e) =>
    Js.log e;
  | _ => Js.log @@ "Listening at http://127.0.0.1:" ^ (string_of_int port);
  };

 App.listen app onListen::(onListen 3000) (); 
