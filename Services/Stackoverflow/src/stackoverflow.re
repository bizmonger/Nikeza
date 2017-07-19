open Axios;

type tag = {
  has_synonyms: bool,
  is_moderator_only: bool,
  is_required: bool,
  count: int,
  name: string
};

type items = array tag;
type responce = {items, has_more: bool, quota_max: int, quota_remaining: int};

let tagEndpoint page => "https://api.stackexchange.com/2.2/tags?" ^ "page=" ^ (string_of_int page)  ^ "&pagesize=100&order=desc&sort=popular&site=stackoverflow";


type responcePromise = Js.Promise.t responce;
let tagsRequest page : responcePromise =>
  Js.Promise.(
    get (tagEndpoint page)
    |> then_ (fun resp => resp##data) 
    |> then_ (fun data => resolve ({
        items: data##items,
        has_more: data##has_more,
        quota_max: data##quota_max,
        quota_remaining: data##quota_remaining
      })
    )
  );

  

let rec getAllTag hasMore page acc => {
  if(not hasMore) {
    Js.Promise.resolve(acc)
  } else {
      Js.Promise.(
        tagsRequest page
        |> then_ (fun resp => {
          Js.log resp;
          getAllTag resp.has_more (page + 1) (Array.append acc resp.items);
        })
      )
  };
};

getAllTag true 1 [||]