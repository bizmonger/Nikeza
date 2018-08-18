module internal Attempt

open Nikeza.Common

type private TryFeatureLink =   LinkId       -> Result<LinkId, LinkId>
type private TryUnfeatureLink = LinkId       -> Result<LinkId, LinkId>
type private TryFeatureTopics = TopicId list -> Result<TopicId list, TopicId list>

let featureLink : TryFeatureLink =
    fun linkId -> Error linkId

let unfeatureLink : TryUnfeatureLink =
    fun linkId -> Error linkId

let featureTopics : TryFeatureTopics =
    fun topicIds -> Error topicIds