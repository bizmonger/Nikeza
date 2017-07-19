type tag = {
    has_synonyms: bool,
    is_moderator_only: bool,
    is_required: bool,
    count: int,
    name: string
  };

let getAllTags: bool => int => array tag => Js.Promise.t (array tag);