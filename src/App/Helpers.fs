module AppHelpers

open Fetch

let fetch<'T> (url: string) (properties: RequestProperties list) =
    GlobalFetch.fetch(RequestInfo.Url url, requestProps properties)


