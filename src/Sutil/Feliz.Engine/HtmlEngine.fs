namespace Feliz

open System

type HtmlHelper<'Node> =
    abstract MakeNode: string * 'Node seq -> 'Node
    abstract StringToNode: string -> 'Node
    abstract EmptyNode: 'Node

module internal HtmlHelperExtensions =
    type HtmlHelper<'Node> with
        member this.IntToNode(v: int) =
            this.StringToNode(Util.asString v)
        member this.FloatToNode(v: float) =
            this.StringToNode(Util.asString v)
        member this.MakeNode(tag, child: 'Node) =
            this.MakeNode(tag, [child])
        member this.MakeNode(tag, v) =
            this.MakeNode(tag, [this.StringToNode v])
        member this.MakeNode(tag, v) =
            this.MakeNode(tag, [this.IntToNode v])
        member this.MakeNode(tag, v) =
            this.MakeNode(tag, [this.FloatToNode v])

open HtmlHelperExtensions

type HtmlEngine<'Node>(h: HtmlHelper<'Node>) =
    /// Create a custom element
    ///
    /// You generally shouldn't need to use this, if you notice an element missing please submit an issue.
    member _.custom (key: string, children: seq<'Node>) = h.MakeNode(key, children)
    /// The empty element, renders nothing on screen
    member _.none : 'Node = h.EmptyNode

    member _.a (children: seq<'Node>) = h.MakeNode("a", children)

    member _.abbr (value: float) = h.MakeNode("abbr", value)
    member _.abbr (value: int) = h.MakeNode("abbr", value)
    member _.abbr (value: 'Node) = h.MakeNode("abbr", value)
    member _.abbr (value: string) = h.MakeNode("abbr", value)
    member _.abbr (children: seq<'Node>) = h.MakeNode("abbr", children)

    member _.address (value: float) = h.MakeNode("address", value)
    member _.address (value: int) = h.MakeNode("address", value)
    member _.address (value: 'Node) = h.MakeNode("address", value)
    member _.address (value: string) = h.MakeNode("address", value)
    member _.address (children: seq<'Node>) = h.MakeNode("address", children)

    member _.anchor (children: seq<'Node>) = h.MakeNode("a", children)

    member _.area (children: seq<'Node>) = h.MakeNode("area", children)

    member _.article (children: seq<'Node>) = h.MakeNode("article", children)

    member _.aside (children: seq<'Node>) = h.MakeNode("aside", children)

    member _.audio (children: seq<'Node>) = h.MakeNode("audio", children)

    member _.b (value: float) = h.MakeNode("b", value)
    member _.b (value: int) = h.MakeNode("b", value)
    member _.b (value: 'Node) = h.MakeNode("b", value)
    member _.b (value: string) = h.MakeNode("b", value)
    member _.b (children: seq<'Node>) = h.MakeNode("b", children)

    member _.base' (children: seq<'Node>) = h.MakeNode("base", children)

    member _.bdi (value: float) = h.MakeNode("bdi", value)
    member _.bdi (value: int) = h.MakeNode("bdi", value)
    member _.bdi (value: 'Node) = h.MakeNode("bdi", value)
    member _.bdi (value: string) = h.MakeNode("bdi", value)
    member _.bdi (children: seq<'Node>) = h.MakeNode("bdi", children)

    member _.bdo (value: float) = h.MakeNode("bdo", value)
    member _.bdo (value: int) = h.MakeNode("bdo", value)
    member _.bdo (value: 'Node) = h.MakeNode("bdo", value)
    member _.bdo (value: string) = h.MakeNode("bdo", value)
    member _.bdo (children: seq<'Node>) = h.MakeNode("bdo", children)

    member _.blockquote (value: float) = h.MakeNode("blockquote", value)
    member _.blockquote (value: int) = h.MakeNode("blockquote", value)
    member _.blockquote (value: 'Node) = h.MakeNode("blockquote", value)
    member _.blockquote (value: string) = h.MakeNode("blockquote", value)
    member _.blockquote (children: seq<'Node>) = h.MakeNode("blockquote", children)

    member _.body (value: float) = h.MakeNode("body", value)
    member _.body (value: int) = h.MakeNode("body", value)
    member _.body (value: 'Node) = h.MakeNode("body", value)
    member _.body (value: string) = h.MakeNode("body", value)
    member _.body (children: seq<'Node>) = h.MakeNode("body", children)

    member _.br (children: seq<'Node>) = h.MakeNode("br", children)

    member _.button (children: seq<'Node>) = h.MakeNode("button", children)

    member _.canvas (children: seq<'Node>) = h.MakeNode("canvas", children)

    member _.caption (value: float) = h.MakeNode("caption", value)
    member _.caption (value: int) = h.MakeNode("caption", value)
    member _.caption (value: 'Node) = h.MakeNode("caption", value)
    member _.caption (value: string) = h.MakeNode("caption", value)
    member _.caption (children: seq<'Node>) = h.MakeNode("caption", children)

    member _.cite (value: float) = h.MakeNode("cite", value)
    member _.cite (value: int) = h.MakeNode("cite", value)
    member _.cite (value: 'Node) = h.MakeNode("cite", value)
    member _.cite (value: string) = h.MakeNode("cite", value)
    member _.cite (children: seq<'Node>) = h.MakeNode("cite", children)

    // member _.code (value: bool) = h.MakeNode("code", value)
    member _.code (value: float) = h.MakeNode("code", value)
    member _.code (value: int) = h.MakeNode("code", value)
    member _.code (value: 'Node) = h.MakeNode("code", value)
    member _.code (value: string) = h.MakeNode("code", value)
    member _.code (children: seq<'Node>) = h.MakeNode("code", children)

    member _.col (children: seq<'Node>) = h.MakeNode("col", children)

    member _.colgroup (children: seq<'Node>) = h.MakeNode("colgroup", children)

    member _.data (value: float) = h.MakeNode("data", value)
    member _.data (value: int) = h.MakeNode("data", value)
    member _.data (value: 'Node) = h.MakeNode("data", value)
    member _.data (value: string) = h.MakeNode("data", value)
    member _.data (children: seq<'Node>) = h.MakeNode("data", children)

    member _.datalist (value: float) = h.MakeNode("datalist", value)
    member _.datalist (value: int) = h.MakeNode("datalist", value)
    member _.datalist (value: 'Node) = h.MakeNode("datalist", value)
    member _.datalist (value: string) = h.MakeNode("datalist", value)
    member _.datalist (children: seq<'Node>) = h.MakeNode("datalist", children)

    member _.dd (value: float) = h.MakeNode("dd", value)
    member _.dd (value: int) = h.MakeNode("dd", value)
    member _.dd (value: 'Node) = h.MakeNode("dd", value)
    member _.dd (value: string) = h.MakeNode("dd", value)
    member _.dd (children: seq<'Node>) = h.MakeNode("dd", children)

    member _.del (value: float) = h.MakeNode("del", value)
    member _.del (value: int) = h.MakeNode("del", value)
    member _.del (value: 'Node) = h.MakeNode("del", value)
    member _.del (value: string) = h.MakeNode("del", value)
    member _.del (children: seq<'Node>) = h.MakeNode("del", children)

    member _.details (children: seq<'Node>) = h.MakeNode("details", children)

    member _.dfn (value: float) = h.MakeNode("dfn", value)
    member _.dfn (value: int) = h.MakeNode("dfn", value)
    member _.dfn (value: 'Node) = h.MakeNode("dfn", value)
    member _.dfn (value: string) = h.MakeNode("dfn", value)
    member _.dfn (children: seq<'Node>) = h.MakeNode("dfn", children)

    member _.dialog (value: float) = h.MakeNode("dialog", value)
    member _.dialog (value: int) = h.MakeNode("dialog", value)
    member _.dialog (value: 'Node) = h.MakeNode("dialog", value)
    member _.dialog (value: string) = h.MakeNode("dialog", value)
    member _.dialog (children: seq<'Node>) = h.MakeNode("dialog", children)

    member _.div (value: float) = h.MakeNode("div", value)
    member _.div (value: int) = h.MakeNode("div", value)
    member _.div (value: 'Node) = h.MakeNode("div", value)
    member _.div (value: string) = h.MakeNode("div", value)
    /// The `<div>` tag defines a division or a section in an HTML document
    member _.div (children: seq<'Node>) = h.MakeNode("div", children)

    member _.dl (children: seq<'Node>) = h.MakeNode("dl", children)

    member _.dt (value: float) = h.MakeNode("dt", value)
    member _.dt (value: int) = h.MakeNode("dt", value)
    member _.dt (value: 'Node) = h.MakeNode("dt", value)
    member _.dt (value: string) = h.MakeNode("dt", value)
    member _.dt (children: seq<'Node>) = h.MakeNode("dt", children)

    member _.em (value: float) = h.MakeNode("em", value)
    member _.em (value: int) = h.MakeNode("em", value)
    member _.em (value: 'Node) = h.MakeNode("em", value)
    member _.em (value: string) = h.MakeNode("em", value)
    member _.em (children: seq<'Node>) = h.MakeNode("em", children)

    member _.fieldSet (children: seq<'Node>) = h.MakeNode("fieldset", children)

    member _.figcaption (children: seq<'Node>) = h.MakeNode("figcaption", children)

    member _.figure (children: seq<'Node>) = h.MakeNode("figure", children)

    member _.footer (children: seq<'Node>) = h.MakeNode("footer", children)

    member _.form (children: seq<'Node>) = h.MakeNode("form", children)

    member _.h1 (value: float) = h.MakeNode("h1", value)
    member _.h1 (value: int) = h.MakeNode("h1", value)
    member _.h1 (value: 'Node) = h.MakeNode("h1", value)
    member _.h1 (value: string) = h.MakeNode("h1", value)
    member _.h1 (children: seq<'Node>) = h.MakeNode("h1", children)
    member _.h2 (value: float) =  h.MakeNode("h2", value)
    member _.h2 (value: int) =  h.MakeNode("h2", value)
    member _.h2 (value: 'Node) =  h.MakeNode("h2", value)
    member _.h2 (value: string) =  h.MakeNode("h2", value)
    member _.h2 (children: seq<'Node>) = h.MakeNode("h2", children)

    member _.h3 (value: float) =  h.MakeNode("h3", value)
    member _.h3 (value: int) =  h.MakeNode("h3", value)
    member _.h3 (value: 'Node) =  h.MakeNode("h3", value)
    member _.h3 (value: string) =  h.MakeNode("h3", value)
    member _.h3 (children: seq<'Node>) = h.MakeNode("h3", children)

    member _.h4 (value: float) = h.MakeNode("h4", value)
    member _.h4 (value: int) = h.MakeNode("h4", value)
    member _.h4 (value: 'Node) = h.MakeNode("h4", value)
    member _.h4 (value: string) = h.MakeNode("h4", value)
    member _.h4 (children: seq<'Node>) = h.MakeNode("h4", children)

    member _.h5 (value: float) = h.MakeNode("h5", value)
    member _.h5 (value: int) = h.MakeNode("h5", value)
    member _.h5 (value: 'Node) = h.MakeNode("h5", value)
    member _.h5 (value: string) = h.MakeNode("h5", value)
    member _.h5 (children: seq<'Node>) = h.MakeNode("h5", children)

    member _.h6 (value: float) = h.MakeNode("h6", value)
    member _.h6 (value: int) = h.MakeNode("h6", value)
    member _.h6 (value: 'Node) = h.MakeNode("h6", value)
    member _.h6 (value: string) = h.MakeNode("h6", value)
    member _.h6 (children: seq<'Node>) = h.MakeNode("h6", children)

    member _.head (children: seq<'Node>) = h.MakeNode("head", children)

    member _.header (children: seq<'Node>) = h.MakeNode("header", children)

    member _.hr (children: seq<'Node>) = h.MakeNode("hr", children)

    member _.html (children: seq<'Node>) = h.MakeNode("html", children)

    member _.i (value: float) = h.MakeNode("i", value)
    member _.i (value: int) = h.MakeNode("i", value)
    member _.i (value: 'Node) = h.MakeNode("i", value)
    member _.i (value: string) = h.MakeNode("i", value)
    member _.i (children: seq<'Node>) = h.MakeNode("i", children)

    member _.iframe (children: seq<'Node>) = h.MakeNode("iframe", children)

    member _.img (children: seq<'Node>) = h.MakeNode("img", children)

    member _.input (children: seq<'Node>) = h.MakeNode("input", children)

    member _.ins (value: float) = h.MakeNode("ins", value)
    member _.ins (value: int) = h.MakeNode("ins", value)
    member _.ins (value: 'Node) = h.MakeNode("ins", value)
    member _.ins (value: string) = h.MakeNode("ins", value)
    member _.ins (children: seq<'Node>) = h.MakeNode("ins", children)

    member _.kbd (value: float) = h.MakeNode("kbd", value)
    member _.kbd (value: int) = h.MakeNode("kbd", value)
    member _.kbd (value: 'Node) = h.MakeNode("kbd", value)
    member _.kbd (value: string) = h.MakeNode("kbd", value)
    member _.kbd (children: seq<'Node>) = h.MakeNode("kbd", children)

    member _.label (children: seq<'Node>) = h.MakeNode("label", children)

    member _.legend (value: float) = h.MakeNode("legend", value)
    member _.legend (value: int) = h.MakeNode("legend", value)
    member _.legend (value: 'Node) = h.MakeNode("legend", value)
    member _.legend (value: string) = h.MakeNode("legend", value)
    member _.legend (children: seq<'Node>) = h.MakeNode("legend", children)

    member _.li (value: float) = h.MakeNode("li", value)
    member _.li (value: int) = h.MakeNode("li", value)
    member _.li (value: 'Node) = h.MakeNode("li", value)
    member _.li (value: string) = h.MakeNode("li", value)
    member _.li (children: seq<'Node>) = h.MakeNode("li", children)

    member _.listItem (value: float) = h.MakeNode("li", value)
    member _.listItem (value: int) = h.MakeNode("li", value)
    member _.listItem (value: 'Node) = h.MakeNode("li", value)
    member _.listItem (value: string) = h.MakeNode("li", value)
    member _.listItem (children: seq<'Node>) = h.MakeNode("li", children)

    member _.main (children: seq<'Node>) = h.MakeNode("main", children)

    member _.map (children: seq<'Node>) = h.MakeNode("map", children)

    member _.mark (value: float) = h.MakeNode("mark", value)
    member _.mark (value: int) = h.MakeNode("mark", value)
    member _.mark (value: 'Node) = h.MakeNode("mark", value)
    member _.mark (value: string) = h.MakeNode("mark", value)
    member _.mark (children: seq<'Node>) = h.MakeNode("mark", children)

    member _.metadata (children: seq<'Node>) = h.MakeNode("metadata", children)

    member _.meter (value: float) = h.MakeNode("meter", value)
    member _.meter (value: int) = h.MakeNode("meter", value)
    member _.meter (value: 'Node) = h.MakeNode("meter", value)
    member _.meter (value: string) = h.MakeNode("meter", value)
    member _.meter (children: seq<'Node>) = h.MakeNode("meter", children)

    member _.nav (children: seq<'Node>) = h.MakeNode("nav", children)

    member _.noscript (children: seq<'Node>) = h.MakeNode("noscript", children)

    member _.object (children: seq<'Node>) = h.MakeNode("object", children)

    member _.ol (children: seq<'Node>) = h.MakeNode("ol", children)

    member _.option (value: float) = h.MakeNode("option", value)
    member _.option (value: int) = h.MakeNode("option", value)
    member _.option (value: 'Node) = h.MakeNode("option", value)
    member _.option (value: string) = h.MakeNode("option", value)
    member _.option (children: seq<'Node>) = h.MakeNode("option", children)

    member _.optgroup (children: seq<'Node>) = h.MakeNode("optgroup", children)

    member _.orderedList (children: seq<'Node>) = h.MakeNode("ol", children)

    member _.output (value: float) = h.MakeNode("output", value)
    member _.output (value: int) = h.MakeNode("output", value)
    member _.output (value: 'Node) = h.MakeNode("output", value)
    member _.output (value: string) = h.MakeNode("output", value)
    member _.output (children: seq<'Node>) = h.MakeNode("output", children)

    member _.p (value: float) = h.MakeNode("p", value)
    member _.p (value: int) = h.MakeNode("p", value)
    member _.p (value: 'Node) = h.MakeNode("p", value)
    member _.p (value: string) = h.MakeNode("p", value)
    member _.p (children: seq<'Node>) = h.MakeNode("p", children)

    member _.paragraph (value: float) = h.MakeNode("p", value)
    member _.paragraph (value: int) = h.MakeNode("p", value)
    member _.paragraph (value: 'Node) = h.MakeNode("p", value)
    member _.paragraph (value: string) = h.MakeNode("p", value)
    member _.paragraph (children: seq<'Node>) = h.MakeNode("p", children)

    member _.picture (children: seq<'Node>) = h.MakeNode("picture", children)

    // member _.pre (value: bool) = h.MakeNode("pre", value)
    member _.pre (value: float) = h.MakeNode("pre", value)
    member _.pre (value: int) = h.MakeNode("pre", value)
    member _.pre (value: 'Node) = h.MakeNode("pre", value)
    member _.pre (value: string) = h.MakeNode("pre", value)
    member _.pre (children: seq<'Node>) = h.MakeNode("pre", children)

    member _.progress (children: seq<'Node>) = h.MakeNode("progress", children)

    member _.q (children: seq<'Node>) = h.MakeNode("q", children)

    member _.rb (value: float) = h.MakeNode("rb", value)
    member _.rb (value: int) = h.MakeNode("rb", value)
    member _.rb (value: 'Node) = h.MakeNode("rb", value)
    member _.rb (value: string) = h.MakeNode("rb", value)
    member _.rb (children: seq<'Node>) = h.MakeNode("rb", children)

    member _.rp (value: float) = h.MakeNode("rp", value)
    member _.rp (value: int) = h.MakeNode("rp", value)
    member _.rp (value: 'Node) = h.MakeNode("rp", value)
    member _.rp (value: string) = h.MakeNode("rp", value)
    member _.rp (children: seq<'Node>) = h.MakeNode("rp", children)

    member _.rt (value: float) = h.MakeNode("rt", value)
    member _.rt (value: int) = h.MakeNode("rt", value)
    member _.rt (value: 'Node) = h.MakeNode("rt", value)
    member _.rt (value: string) = h.MakeNode("rt", value)
    member _.rt (children: seq<'Node>) = h.MakeNode("rt", children)

    member _.rtc (value: float) = h.MakeNode("rtc", value)
    member _.rtc (value: int) = h.MakeNode("rtc", value)
    member _.rtc (value: 'Node) = h.MakeNode("rtc", value)
    member _.rtc (value: string) = h.MakeNode("rtc", value)
    member _.rtc (children: seq<'Node>) = h.MakeNode("rtc", children)

    member _.ruby (value: float) = h.MakeNode("ruby", value)
    member _.ruby (value: int) = h.MakeNode("ruby", value)
    member _.ruby (value: 'Node) = h.MakeNode("ruby", value)
    member _.ruby (value: string) = h.MakeNode("ruby", value)
    member _.ruby (children: seq<'Node>) = h.MakeNode("ruby", children)

    member _.s (value: float) = h.MakeNode("s", value)
    member _.s (value: int) = h.MakeNode("s", value)
    member _.s (value: 'Node) = h.MakeNode("s", value)
    member _.s (value: string) = h.MakeNode("s", value)
    member _.s (children: seq<'Node>) = h.MakeNode("s", children)

    member _.samp (value: float) = h.MakeNode("samp", value)
    member _.samp (value: int) = h.MakeNode("samp", value)
    member _.samp (value: 'Node) = h.MakeNode("samp", value)
    member _.samp (value: string) = h.MakeNode("samp", value)
    member _.samp (children: seq<'Node>) = h.MakeNode("samp", children)

    member _.script (children: seq<'Node>) = h.MakeNode("script", children)

    member _.section (children: seq<'Node>) = h.MakeNode("section", children)

    member _.select (children: seq<'Node>) = h.MakeNode("select", children)
    member _.small (value: float) = h.MakeNode("small", value)
    member _.small (value: int) = h.MakeNode("small", value)
    member _.small (value: 'Node) = h.MakeNode("small", value)
    member _.small (value: string) = h.MakeNode("small", value)
    member _.small (children: seq<'Node>) = h.MakeNode("small", children)

    member _.source (children: seq<'Node>) = h.MakeNode("source", children)

    member _.span (value: float) = h.MakeNode("span", value)
    member _.span (value: int) = h.MakeNode("span", value)
    member _.span (value: 'Node) = h.MakeNode("span", value)
    member _.span (value: string) = h.MakeNode("span", value)
    member _.span (children: seq<'Node>) = h.MakeNode("span", children)

    member _.strong (value: float) = h.MakeNode("strong", value)
    member _.strong (value: int) = h.MakeNode("strong", value)
    member _.strong (value: 'Node) = h.MakeNode("strong", value)
    member _.strong (value: string) = h.MakeNode("strong", value)
    member _.strong (children: seq<'Node>) = h.MakeNode("strong", children)

    member _.style (value: string) = h.MakeNode("style", value)

    member _.sub (value: float) = h.MakeNode("sub", value)
    member _.sub (value: int) = h.MakeNode("sub", value)
    member _.sub (value: 'Node) = h.MakeNode("sub", value)
    member _.sub (value: string) = h.MakeNode("sub", value)
    member _.sub (children: seq<'Node>) = h.MakeNode("sub", children)

    member _.summary (value: float) = h.MakeNode("summary", value)
    member _.summary (value: int) = h.MakeNode("summary", value)
    member _.summary (value: 'Node) = h.MakeNode("summary", value)
    member _.summary (value: string) = h.MakeNode("summary", value)
    member _.summary (children: seq<'Node>) = h.MakeNode("summary", children)

    member _.sup (value: float) = h.MakeNode("sup", value)
    member _.sup (value: int) = h.MakeNode("sup", value)
    member _.sup (value: 'Node) = h.MakeNode("sup", value)
    member _.sup (value: string) = h.MakeNode("sup", value)
    member _.sup (children: seq<'Node>) = h.MakeNode("sup", children)

    member _.table (children: seq<'Node>) = h.MakeNode("table", children)

    member _.tableBody (children: seq<'Node>) = h.MakeNode("tbody", children)

    member _.tableCell (children: seq<'Node>) = h.MakeNode("td", children)

    member _.tableHeader (children: seq<'Node>) = h.MakeNode("th", children)

    member _.tableRow (children: seq<'Node>) = h.MakeNode("tr", children)

    member _.tbody (children: seq<'Node>) = h.MakeNode("tbody", children)

    member _.td (value: float) = h.MakeNode("td", value)
    member _.td (value: int) = h.MakeNode("td", value)
    member _.td (value: 'Node) = h.MakeNode("td", value)
    member _.td (value: string) = h.MakeNode("td", value)
    member _.td (children: seq<'Node>) = h.MakeNode("td", children)

    member _.template (children: seq<'Node>) = h.MakeNode("template", children)

    member _.text (value: float) : 'Node = h.FloatToNode value
    member _.text (value: int) : 'Node = h.IntToNode value
    member _.text (value: string) : 'Node = h.StringToNode value
    member _.text (value: System.Guid) : 'Node = h.StringToNode (Util.asString value)

    member this.textf fmt = Printf.kprintf this.text fmt

    member _.textarea (children: seq<'Node>) = h.MakeNode("textarea", children)

    member _.tfoot (children: seq<'Node>) = h.MakeNode("tfoot", children)

    member _.th (value: float) = h.MakeNode("th", value)
    member _.th (value: int) = h.MakeNode("th", value)
    member _.th (value: 'Node) = h.MakeNode("th", value)
    member _.th (value: string) = h.MakeNode("th", value)
    member _.th (children: seq<'Node>) = h.MakeNode("th", children)

    member _.thead (children: seq<'Node>) = h.MakeNode("thead", children)

    member _.time (children: seq<'Node>) = h.MakeNode("time", children)

    member _.tr (children: seq<'Node>) = h.MakeNode("tr", children)

    member _.track (children: seq<'Node>) = h.MakeNode("track", children)

    member _.u (value: float) = h.MakeNode("u", value)
    member _.u (value: int) = h.MakeNode("u", value)
    member _.u (value: 'Node) = h.MakeNode("u", value)
    member _.u (value: string) = h.MakeNode("u", value)
    member _.u (children: seq<'Node>) = h.MakeNode("u", children)

    member _.ul (children: seq<'Node>) = h.MakeNode("ul", children)

    member _.unorderedList (children: seq<'Node>) = h.MakeNode("ul", children)

    member _.var (value: float) = h.MakeNode("var", value)
    member _.var (value: int) = h.MakeNode("var", value)
    member _.var (value: 'Node) = h.MakeNode("var", value)
    member _.var (value: string) = h.MakeNode("var", value)
    member _.var (children: seq<'Node>) = h.MakeNode("var", children)

    member _.video (children: seq<'Node>) = h.MakeNode("video", children)

    member _.wbr (children: seq<'Node>) = h.MakeNode("wbr", children)
