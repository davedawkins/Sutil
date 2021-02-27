namespace Feliz

open System
open Feliz.Styles

type AttrHelper<'Node> =
    abstract MakeAttr: key: string * value: string -> 'Node
    abstract MakeBooleanAttr: string * bool -> 'Node
    // abstract MakeEvent: string * (Event -> unit) -> 'Node

type AttrEngine<'Node>(h: AttrHelper<'Node>) =
    /// List of types the server accepts, typically a file type.
    member _.accept (value: string) = h.MakeAttr("accept", value)

    /// List of supported charsets.
    member _.acceptCharset (value: string) = h.MakeAttr("accept-charset", value)

    /// Defines a keyboard shortcut to activate or add focus to the element.
    member _.accessKey (value: string) = h.MakeAttr("accesskey", value)

    /// The URI of a program that processes the information submitted via the form.
    member _.action (value: string) = h.MakeAttr("action", value)

    /// Alternative text in case an image can't be displayed.
    member _.alt (value: string) = h.MakeAttr("alt", value)

    /// Controls the amplitude of the gamma function of a component transfer element when
    /// its type attribute is gamma.
    member _.amplitude (value: float) = h.MakeAttr("amplitude", Util.asString value)
    /// Controls the amplitude of the gamma function of a component transfer element when
    /// its type attribute is gamma.
    member _.amplitude (value: int) = h.MakeAttr("amplitude", Util.asString value)

    /// Identifies the currently active descendant of a `composite` widget.
    ///
    /// https://www.w3.org/WAI/PF/aria-1.1/states_and_properties#aria-activedescendant
    member _.ariaActiveDescendant (id: string) = h.MakeAttr("aria-activedescendant", id)

    /// Indicates whether assistive technologies will present all, or only parts
    /// of, the changed region based on the change notifications defined by the
    /// `aria-relevant` attribute.
    ///
    /// https://www.w3.org/WAI/PF/aria-1.1/states_and_properties#aria-atomic
    member _.ariaAtomic (value: bool) = h.MakeBooleanAttr("aria-atomic", value)

    /// Indicates whether an element, and its subtree, are currently being
    /// updated.
    ///
    /// https://www.w3.org/WAI/PF/aria-1.1/states_and_properties#aria-busy
    member _.ariaBusy (value: bool) = h.MakeBooleanAttr("aria-busy", value)

    /// Indicates the current "checked" state of checkboxes, radio buttons, and
    /// other widgets. See related `aria-pressed` and `aria-selected`.
    ///
    /// https://www.w3.org/WAI/PF/aria-1.1/states_and_properties#aria-checked
    member _.ariaChecked (value: bool) = h.MakeBooleanAttr("aria-checked", value)

    /// Identifies the element (or elements) whose contents or presence are
    /// controlled by the current element. See related `aria-owns`.
    ///
    /// https://www.w3.org/WAI/PF/aria-1.1/states_and_properties#aria-controls
    member _.ariaControls ([<ParamArray>] ids: string[]) = h.MakeAttr("aria-controls", String.concat " " ids)

    /// Specifies a URI referencing content that describes the object. See
    /// related `aria-describedby`.
    ///
    /// https://www.w3.org/WAI/PF/aria-1.1/states_and_properties#aria-describedat
    member _.ariaDescribedAt (uri: string) = h.MakeAttr("aria-describedat", uri)

    /// Identifies the element (or elements) that describes the object. See
    /// related `aria-describedat` and `aria-labelledby`.
    ///
    /// The `aria-labelledby` attribute is similar to `aria-describedby` in that
    /// both reference other elements to calculate a text alternative, but a
    /// label should be concise, where a description is intended to provide more
    /// verbose information.
    ///
    /// https://www.w3.org/WAI/PF/aria-1.1/states_and_properties#aria-describedby
    member _.ariaDescribedBy ([<ParamArray>] ids: string[]) = h.MakeAttr("aria-describedby", String.concat " " ids)

    /// Indicates that the element is perceivable but disabled, so it is not
    /// editable or otherwise operable. See related `aria-hidden` and
    /// `aria-readonly`.
    ///
    /// https://www.w3.org/WAI/PF/aria-1.1/states_and_properties#aria-disabled
    member _.ariaDisabled (value: bool) = h.MakeBooleanAttr("aria-disabled", value)

    // /// Indicates what functions can be performed when the dragged object is
    // /// released on the drop target. This allows assistive technologies to
    // /// convey the possible drag options available to users, including whether a
    // /// pop-up menu of choices is provided by the application. Typically, drop
    // /// effect functions can only be provided once an object has been grabbed
    // /// for a drag operation as the drop effect functions available are
    // /// dependent on the object being dragged.
    // ///
    // /// https://www.w3.org/WAI/PF/aria-1.1/states_and_properties#aria-dropeffect
    // member _.ariaDropEffect ([<ParamArray>] values: AriaDropEffect []) = h.MakeAttr("aria-dropeffect", values |> Array.map Util.asString |> String.concat " ")

    /// Indicates whether the element, or another grouping element it controls,
    /// is currently expanded or collapsed.
    ///
    /// https://www.w3.org/WAI/PF/aria-1.1/states_and_properties#aria-expanded
    member _.ariaExpanded (value: bool) = h.MakeBooleanAttr("aria-expanded", value)

    /// Identifies the next element (or elements) in an alternate reading order
    /// of content which, at the user's discretion, allows assistive technology
    /// to override the general default of reading in document source order.
    ///
    /// https://www.w3.org/WAI/PF/aria-1.1/states_and_properties#aria-flowto
    member _.ariaFlowTo ([<ParamArray>] ids: string[]) = h.MakeAttr("aria-flowto", String.concat " " ids)

    /// Indicates an element's "grabbed" state in a drag-and-drop operation.
    ///
    /// When it is set to true it has been selected for dragging, false
    /// indicates that the element can be grabbed for a drag-and-drop operation,
    /// but is not currently grabbed, and undefined (or no value) indicates the
    /// element cannot be grabbed (default).
    ///
    /// https://www.w3.org/WAI/PF/aria-1.1/states_and_properties#aria-grabbed
    member _.ariaGrabbed (value: bool) = h.MakeBooleanAttr("aria-grabbed", value)

    /// Indicates that the element has a popup context menu or sub-level menu.
    ///
    /// https://www.w3.org/WAI/PF/aria-1.1/states_and_properties#aria-haspopup
    member _.ariaHasPopup (value: bool) = h.MakeBooleanAttr("aria-haspopup", value)

    /// Indicates that the element and all of its descendants are not visible or
    /// perceivable to any user as implemented by the author. See related
    /// `aria-disabled`.
    ///
    /// https://www.w3.org/WAI/PF/aria-1.1/states_and_properties#aria-hidden
    member _.ariaHidden (value: bool) = h.MakeBooleanAttr("aria-hidden", value)

    /// Indicates the entered value does not conform to the format expected by
    /// the application.
    ///
    /// https://www.w3.org/WAI/PF/aria-1.1/states_and_properties#aria-invalid
    member _.ariaInvalid (value: bool) = h.MakeBooleanAttr("aria-invalid", value)

    /// Defines a Util.asString value that labels the current element. See related
    /// `aria-labelledby`.
    ///
    /// https://www.w3.org/WAI/PF/aria-1.1/states_and_properties#aria-label
    member _.ariaLabel (value: string) = h.MakeAttr("aria-label", value)

    /// Defines the hierarchical level of an element within a structure.
    ///
    /// This can be applied inside trees to tree items, to headings inside a
    /// document, to nested grids, nested tablists and to other structural items
    /// that may appear inside a container or participate in an ownership
    /// hierarchy. The value for `aria-level` is an integer greater than or
    /// equal to 1.
    ///
    /// https://www.w3.org/WAI/PF/aria-1.1/states_and_properties#aria-level
    member _.ariaLevel (value: int) = h.MakeAttr("aria-level", Util.asString value)

    /// Identifies the element (or elements) that labels the current element.
    /// See related `aria-label` and `aria-describedby`.
    ///
    /// https://www.w3.org/WAI/PF/aria-1.1/states_and_properties#aria-labelledby
    member _.ariaLabelledBy ([<ParamArray>] ids: string[]) = h.MakeAttr("aria-labelledby", String.concat " " ids)

    /// Indicates whether a text box accepts multiple lines of input or only a
    /// single line.
    ///
    /// https://www.w3.org/WAI/PF/aria-1.1/states_and_properties#aria-multiline
    member _.ariaMultiLine (value: bool) = h.MakeBooleanAttr("aria-multiline", value)

    /// Indicates that the user may select more than one item from the current
    /// selectable descendants.
    ///
    /// https://www.w3.org/WAI/PF/aria-1.1/states_and_properties#aria-multiselectable
    member _.ariaMultiSelectable (value: bool) = h.MakeBooleanAttr("aria-multiselectable", value)

    /// Identifies an element (or elements) in order to define a visual,
    /// functional, or contextual parent/child relationship between DOM elements
    /// where the DOM hierarchy cannot be used to represent the relationship.
    /// See related `aria-controls`.
    ///
    /// https://www.w3.org/WAI/PF/aria-1.1/states_and_properties#aria-owns
    member _.ariaOwns ([<ParamArray>] ids: string[]) = h.MakeAttr("aria-owns", String.concat " " ids)

    /// Indicates the current "pressed" state of toggle buttons. See related
    /// `aria-checked` and `aria-selected`.
    ///
    /// https://www.w3.org/WAI/PF/aria-1.1/states_and_properties#aria-pressed
    member _.ariaPressed (value: bool) = h.MakeBooleanAttr("aria-pressed", value)

    /// Defines an element's number or position in the current set of listitems
    /// or treeitems. Not required if all elements in the set are present in the
    /// DOM. See related `aria-setsize`.
    ///
    /// https://www.w3.org/WAI/PF/aria-1.1/states_and_properties#aria-posinset
    member _.ariaPosInSet (value: int) = h.MakeAttr("aria-posinset", Util.asString value)

    /// Indicates that the element is not editable, but is otherwise operable.
    /// See related `aria-disabled`.
    ///
    /// https://www.w3.org/WAI/PF/aria-1.1/states_and_properties#aria-readonly
    member _.ariaReadOnly (value: bool) = h.MakeBooleanAttr("aria-readonly", value)

    // /// Indicates what user agent change notifications (additions, removals,
    // /// etc.) assistive technologies will receive within a live region. See
    // /// related `aria-atomic`.
    // ///
    // /// https://www.w3.org/WAI/PF/aria-1.1/states_and_properties#aria-relevant
    // member _.ariaRelevant ([<ParamArray>] values: AriaRelevant []) = h.MakeAttr("aria-relevant", values |> Array.map Util.asString |> String.concat " ")

    /// Indicates that user input is required on the element before a form may
    /// be submitted.
    ///
    /// https://www.w3.org/WAI/PF/aria-1.1/states_and_properties#aria-required
    member _.ariaRequired (value: bool) = h.MakeBooleanAttr("aria-required", value)

    /// Indicates the current "selected" state of various widgets. See related
    /// `aria-checked` and `aria-pressed`.
    ///
    /// https://www.w3.org/WAI/PF/aria-1.1/states_and_properties#aria-selected
    member _.ariaSelected (value: bool) = h.MakeBooleanAttr("aria-selected", value)

    /// Defines the maximum allowed value for a range widget.
    ///
    /// https://www.w3.org/WAI/PF/aria-1.1/states_and_properties#aria-valuemax
    member _.ariaValueMax (value: float) = h.MakeAttr("aria-valuemax", Util.asString value)
    /// Defines the maximum allowed value for a range widget.
    ///
    /// https://www.w3.org/WAI/PF/aria-1.1/states_and_properties#aria-valuemax
    member _.ariaValueMax (value: int) = h.MakeAttr("aria-valuemax", Util.asString value)

    /// Defines the minimum allowed value for a range widget.
    ///
    /// https://www.w3.org/WAI/PF/aria-1.1/states_and_properties#aria-valuemin
    member _.ariaValueMin (value: float) = h.MakeAttr("aria-valuemin", Util.asString value)
    /// Defines the minimum allowed value for a range widget.
    ///
    /// https://www.w3.org/WAI/PF/aria-1.1/states_and_properties#aria-valuemin
    member _.ariaValueMin (value: int) = h.MakeAttr("aria-valuemin", Util.asString value)

    /// Defines the current value for a range widget. See related
    /// `aria-valuetext`.
    ///
    /// https://www.w3.org/WAI/PF/aria-1.1/states_and_properties#aria-valuenow
    member _.ariaValueNow (value: float) = h.MakeAttr("aria-valuenow", Util.asString value)
    /// Defines the current value for a range widget. See related
    /// `aria-valuetext`.
    ///
    /// https://www.w3.org/WAI/PF/aria-1.1/states_and_properties#aria-valuenow
    member _.ariaValueNow (value: int) = h.MakeAttr("aria-valuenow", Util.asString value)

    /// Defines the human readable text alternative of `aria-valuenow` for a
    /// range widget.
    ///
    /// https://www.w3.org/WAI/PF/aria-1.1/states_and_properties#aria-valuetext
    member _.ariaValueText (value: string) = h.MakeAttr("aria-valuetext", value)

    /// Defines the number of items in the current set of listitems or
    /// treeitems. Not required if all elements in the set are present in the
    /// DOM. See related `aria-posinset`.
    ///
    /// https://www.w3.org/WAI/PF/aria-1.1/states_and_properties#aria-setsize
    member _.ariaSetSize (value: int) = h.MakeAttr("aria-setsize", Util.asString value)

    /// Indicates that the script should be executed asynchronously.
    member _.async (value: bool) = h.MakeBooleanAttr("async", value)

    /// Indicates the name of the CSS property or attribute of the target element
    /// that is going to be changed during an animation.
    member _.attributeName (value: string) = h.MakeAttr("attributeName", value)

    /// Indicates whether controls in this form can by default have their values
    /// automatically completed by the browser.
    member _.autoComplete (value: string) = h.MakeAttr("autocomplete", value)

    /// The element should be automatically focused after the page loaded.
    member _.autoFocus (value: bool) = h.MakeBooleanAttr("autofocus", value)

    /// The audio or video should play as soon as possible.
    member _.autoPlay (value: bool) = h.MakeBooleanAttr("autoplay", value)

    /// Specifies the direction angle for the light source on the XY plane (clockwise),
    /// in degrees from the x axis.
    member _.azimuth (value: float) = h.MakeAttr("azimuth", Util.asString value)
    /// Specifies the direction angle for the light source on the XY plane (clockwise),
    /// in degrees from the x axis.
    member _.azimuth (value: int) = h.MakeAttr("azimuth", Util.asString value)

    /// Represents the base frequency parameter for the noise function of the
    /// <feTurbulence> filter primitive.
    member _.baseFrequency (value: float) = h.MakeAttr("baseFrequency", Util.asString value)
    /// Represents the base frequency parameter for the noise function of the
    /// <feTurbulence> filter primitive.
    member _.baseFrequency (value: int) = h.MakeAttr("baseFrequency", Util.asString value)
    /// Represents the base frequency parameter for the noise function of the
    /// <feTurbulence> filter primitive.
    member _.baseFrequency (horizontal: float, vertical: float) = h.MakeAttr("baseFrequency", Util.asString horizontal + "," + Util.asString vertical)
    /// Represents the base frequency parameter for the noise function of the
    /// <feTurbulence> filter primitive.
    member _.baseFrequency (horizontal: float, vertical: int) = h.MakeAttr("baseFrequency", Util.asString horizontal + "," + Util.asString vertical)
    /// Represents the base frequency parameter for the noise function of the
    /// <feTurbulence> filter primitive.
    member _.baseFrequency (horizontal: int, vertical: float) = h.MakeAttr("baseFrequency", Util.asString horizontal + "," + Util.asString vertical)
    /// Represents the base frequency parameter for the noise function of the
    /// <feTurbulence> filter primitive.

    /// Defines when an animation should begin or when an element should be discarded.
    member _.begin' (value: string) = h.MakeAttr("begin", value)

    /// Shifts the range of the filter. After applying the kernelMatrix of the <feConvolveMatrix>
    /// element to the input image to yield a number and applied the divisor attribute, the bias
    /// attribute is added to each component. This allows representation of values that would
    /// otherwise be clamped to 0 or 1.
    member _.bias (value: float) = h.MakeAttr("bias", Util.asString value)
    /// Shifts the range of the filter. After applying the kernelMatrix of the <feConvolveMatrix>
    /// element to the input image to yield a number and applied the divisor attribute, the bias
    /// attribute is added to each component. This allows representation of values that would
    /// otherwise be clamped to 0 or 1.
    member _.bias (value: int) = h.MakeAttr("bias", Util.asString value)

    /// Specifies a relative offset value for an attribute that will be modified during an animation.
    member _.by (value: float) = h.MakeAttr("by", Util.asString value)
    /// Specifies a relative offset value for an attribute that will be modified during an animation.
    member _.by (value: int) = h.MakeAttr("by", Util.asString value)
    /// Specifies a relative offset value for an attribute that will be modified during an animation.
    member _.by (value: string) = h.MakeAttr("by", value)

    member _.capture (value: bool) = h.MakeBooleanAttr("capture", value)

    /// This attribute declares the document's character encoding. Must be used in the meta tag.
    member _.charset (value: string) = h.MakeAttr("charset", value)

    /// A URL that designates a source document or message for the information quoted. This attribute is intended to
    /// point to information explaining the context or the reference for the quote.
    member _.cite (value: string) = h.MakeAttr("cite", value)

    /// Specifies a CSS class for this element.
    member _.className (value: string) = h.MakeAttr("class", value)
    /// Takes a `seq<string>` and joins them using a space to combine the classses into a single class property.
    ///
    /// `prop.className [ "one"; "two" ]`
    ///
    /// is the same as
    ///
    /// `prop.className "one two"`
    member _.className (names: seq<string>) = h.MakeAttr("class", String.concat " " names)

    /// Takes a `seq<string>` and joins them using a space to combine the classses into a single class property.
    ///
    /// `prop.classes [ "one"; "two" ]` => `prop.className "one two"`
    member _.classes (names: seq<string>) = h.MakeAttr("class", String.concat " " names)

    member _.classes (names: seq<bool * string>) =
        let class' = names |> Seq.choose (function false, _ -> None | true, c -> Some c) |> String.concat " "
        h.MakeAttr("class", class')

    /// Defines the number of columns in a textarea.
    member _.cols (value: int) = h.MakeAttr("cols", Util.asString value)

    /// Defines the number of columns a cell should span.
    member _.colSpan (value: int) = h.MakeAttr("colspan", Util.asString value)

    /// A value associated with http-equiv or name depending on the context.
    member _.content (value: string) = h.MakeAttr("content", value)

    /// Indicates whether the element's content is editable.
    member _.contentEditable (value: bool) = h.MakeBooleanAttr("contenteditable", value)

    /// If true, the browser will offer controls to allow the user to control video playback,
    /// including volume, seeking, and pause/resume playback.
    member _.controls (value: bool) = h.MakeBooleanAttr("controls", value)

    /// Create a custom attribute
    ///
    /// You generally shouldn't need to use this, if you notice a core React/Html attribute missing please submit an issue.
    member _.custom (key: string, value: string) = h.MakeAttr(key, value)
    /// The SVG cx attribute define the x-axis coordinate of a center point.
    ///
    /// Three elements are using this attribute: <circle>, <ellipse>, and <radialGradient>
    member _.cx (value: float) = h.MakeAttr("cx", Util.asString value)
    /// The SVG cx attribute define the x-axis coordinate of a center point.
    ///
    /// Three elements are using this attribute: <circle>, <ellipse>, and <radialGradient>
    member _.cx (value: ICssUnit) = h.MakeAttr("cx", Util.asString value)
    /// The SVG cx attribute define the x-axis coordinate of a center point.
    ///
    /// Three elements are using this attribute: <circle>, <ellipse>, and <radialGradient>
    member _.cx (value: int) = h.MakeAttr("cx", Util.asString value)

    /// The SVG cy attribute define the y-axis coordinate of a center point.
    ///
    /// Three elements are using this attribute: <circle>, <ellipse>, and <radialGradient>
    member _.cy (value: float) = h.MakeAttr("cy", Util.asString value)
    /// The SVG cy attribute define the y-axis coordinate of a center point.
    ///
    /// Three elements are using this attribute: <circle>, <ellipse>, and <radialGradient>
    member _.cy (value: ICssUnit) = h.MakeAttr("cy", Util.asString value)
    /// The SVG cy attribute define the y-axis coordinate of a center point.
    ///
    /// Three elements are using this attribute: <circle>, <ellipse>, and <radialGradient>
    member _.cy (value: int) = h.MakeAttr("cy", Util.asString value)

    // TODO
    // /// Defines a SVG path to be drawn.
    // member _.d (path: seq<char * (float list list)>) =
    //     PropHelpers.createSvgPathFloat path
    //     |> h.MakeAttr("d",)
    // /// Defines a SVG path to be drawn.
    // member _.d (path: seq<char * (int list list)>) =
    //     PropHelpers.createSvgPathInt path
    //     |> h.MakeAttr("d",)
    /// Defines a SVG path to be drawn.
    member _.d (path: string) = h.MakeAttr("d", path)

    // /// Sets the inner Html content of the element.
    // member _.dangerouslySetInnerHTML (content: string) = h.MakeAttr("dangerouslySetInnerHTML", (createObj [ "__html" ==> content ]))

    /// This attribute indicates the time and/or date of the element.
    member _.dateTime (value: string) = h.MakeAttr("datetime", value)

    // /// Sets the DOM defaultChecked value when initially rendered.
    // ///
    // /// Typically only used with uncontrolled components.
    // member _.defaultChecked (value: bool) = h.MakeBooleanAttr("defaultChecked", value)

    // /// Sets the DOM defaultValue value when initially rendered.
    // ///
    // /// Typically only used with uncontrolled components.
    // member _.defaultValue (value: bool) = h.MakeBooleanAttr("defaultValue", value)
    // /// Sets the DOM defaultValue value when initially rendered.
    // ///
    // /// Typically only used with uncontrolled components.
    // member _.defaultValue (value: float) = h.MakeAttr("defaultValue", Util.asString value)
    // /// Sets the DOM defaultValue value when initially rendered.
    // ///
    // /// Typically only used with uncontrolled components.
    // member _.defaultValue (value: int) = h.MakeAttr("defaultValue", Util.asString value)
    // /// Sets the DOM defaultValue value when initially rendered.
    // ///
    // /// Typically only used with uncontrolled components.
    // member _.defaultValue (value: string) = h.MakeAttr("defaultValue", value)
    // /// Sets the DOM defaultValue value when initially rendered.
    // ///
    // /// Typically only used with uncontrolled components.
    // member _.defaultValue (value: seq<float>) = h.MakeAttr("defaultValue", (ResizeArray value))
    // /// Sets the DOM defaultValue value when initially rendered.
    // ///
    // /// Typically only used with uncontrolled components.
    // member _.defaultValue (value: seq<int>) = h.MakeAttr("defaultValue", (ResizeArray value))
    // /// Sets the DOM defaultValue value when initially rendered.
    // ///
    // /// Typically only used with uncontrolled components.
    // member _.defaultValue (value: seq<string>) = h.MakeAttr("defaultValue", (ResizeArray value))

    /// Indicates to a browser that the script is meant to be executed after the document
    /// has been parsed, but before firing DOMContentLoaded.
    ///
    /// Scripts with the defer attribute will prevent the DOMContentLoaded event from
    /// firing until the script has loaded and finished evaluating.
    ///
    /// This attribute must not be used if the src attribute is absent (i.e. for inline scripts),
    /// in this case it would have no effect.
    member _.defer (value: bool) = h.MakeBooleanAttr("defer", value)

    /// Represents the kd value in the Phong lighting model.
    ///
    /// In SVG, this can be any non-negative number.
    member _.diffuseConstant (value: float) = h.MakeAttr("diffuseConstant", Util.asString value)
    /// Represents the kd value in the Phong lighting model.
    ///
    /// In SVG, this can be any non-negative number.
    member _.diffuseConstant (value: int) = h.MakeAttr("diffuseConstant", Util.asString value)

    /// Sets the directionality of the element.
    member _.dirName (value: string) = h.MakeAttr("dirName", value)

    /// Indicates whether the user can interact with the element.
    member _.disabled (value: bool) = h.MakeBooleanAttr("disabled", value)

    /// Specifies the value by which the resulting number of applying the kernelMatrix
    /// of a <feConvolveMatrix> element to the input image color value is divided to
    /// yield the destination color value.
    ///
    /// A divisor that is the sum of all the matrix values tends to have an evening
    /// effect on the overall color intensity of the result.
    member _.divisor (value: float) = h.MakeAttr("divisor", Util.asString value)
    /// Specifies the value by which the resulting number of applying the kernelMatrix
    /// of a <feConvolveMatrix> element to the input image color value is divided to
    /// yield the destination color value.
    ///
    /// A divisor that is the sum of all the matrix values tends to have an evening
    /// effect on the overall color intensity of the result.
    member _.divisor (value: int) = h.MakeAttr("divisor", Util.asString value)

    /// This attribute, if present, indicates that the author intends the hyperlink to be used for downloading a resource.
    member _.download (value: bool) = h.MakeBooleanAttr("download", value)

    /// Indicates whether the the element can be dragged.
    member _.draggable (value: bool) = h.MakeBooleanAttr("draggable", value)

    /// SVG attribute to indicate a shift along the x-axis on the position of an element or its content.
    member _.dx (value: float) = h.MakeAttr("dx", Util.asString value)
    /// SVG attribute to indicate a shift along the x-axis on the position of an element or its content.
    member _.dx (value: int) = h.MakeAttr("dx", Util.asString value)

    /// SVG attribute to indicate a shift along the y-axis on the position of an element or its content.
    member _.dy (value: float) = h.MakeAttr("dy", Util.asString value)
    /// SVG attribute to indicate a shift along the y-axis on the position of an element or its content.
    member _.dy (value: int) = h.MakeAttr("dy", Util.asString value)

    /// SVG attribute that specifies the direction angle for the light source from the XY plane towards
    /// the Z-axis, in degrees.
    ///
    /// Note that the positive Z-axis points towards the viewer of the content.
    member _.elevation (value: float) = h.MakeAttr("elevation", Util.asString value)
    /// SVG attribute that specifies the direction angle for the light source from the XY plane towards
    /// the Z-axis, in degrees.
    ///
    /// Note that the positive Z-axis points towards the viewer of the content.
    member _.elevation (value: int) = h.MakeAttr("elevation", Util.asString value)

    /// Defines an end value for the animation that can constrain the active duration.
    member _.end' (value: string) = h.MakeAttr("end", value)
    /// Defines an end value for the animation that can constrain the active duration.
    member _.end' (values: seq<string>) = h.MakeAttr("end", String.concat ";" values)
    /// Defines the exponent of the gamma function.
    member _.exponent (value: float) = h.MakeAttr("exponent", Util.asString value)
    /// Defines the exponent of the gamma function.
    member _.exponent (value: int) = h.MakeAttr("exponent", Util.asString value)

    // /// Defines the files that will be uploaded when using an input element of the file type.
    // member _.files (value: FileList) = h.MakeAttr("files", value)

    /// SVG attribute to define the opacity of the paint server (color, gradient, pattern, etc) applied to a shape.
    member _.fillOpacity (value: float) = h.MakeAttr("fill-opacity", Util.asString value)
    /// SVG attribute to define the opacity of the paint server (color, gradient, pattern, etc) applied to a shape.
    member _.fillOpacity (value: int) = h.MakeAttr("fill-opacity", Util.asString value)

    /// SVG attribute to define the size of the font from baseline to baseline when multiple
    /// lines of text are set solid in a multiline layout environment.
    member _.fontSize (value: float) = h.MakeAttr("font-size", Util.asString value)
    /// SVG attribute to define the size of the font from baseline to baseline when multiple
    /// lines of text are set solid in a multiline layout environment.
    member _.fontSize (value: int) = h.MakeAttr("font-size", Util.asString value)

    /// A space-separated list of other elements’ ids, indicating that those elements contributed input
    /// values to (or otherwise affected) the calculation.
    member _.for' (value: string) = h.MakeAttr("for", value)
    /// A space-separated list of other elements’ ids, indicating that those elements contributed input
    /// values to (or otherwise affected) the calculation.
    member _.for' (ids: #seq<string>) = h.MakeAttr("for", (ids |> String.concat " "))

    /// The <form> element to associate the <meter> element with (its form owner). The value of this
    /// attribute must be the id of a <form> in the same document. If this attribute is not set, the
    /// <button> is associated with its ancestor <form> element, if any. This attribute is only used
    /// if the <meter> element is being used as a form-associated element, such as one displaying a
    /// range corresponding to an <input type="number">.
    member _.form (value: string) = h.MakeAttr("form", value)

    /// Indicates the initial value of the attribute that will be modified during the animation.
    ///
    /// When used with the `to` attribute, the animation will change the modified attribute from
    /// the from value to the to value.
    ///
    /// When used with the `by` attribute, the animation will change the attribute relatively
    /// from the from value by the value specified in by.
    member _.from (value: float) = h.MakeAttr("from", Util.asString value)
    /// Indicates the initial value of the attribute that will be modified during the animation.
    ///
    /// When used with the `to` attribute, the animation will change the modified attribute from
    /// the from value to the to value.
    ///
    /// When used with the `by` attribute, the animation will change the attribute relatively
    /// from the from value by the value specified in by.
    member _.from (values: seq<float>) = h.MakeAttr("from", (values |> Seq.map Util.asString |> String.concat " "))
    /// Indicates the initial value of the attribute that will be modified during the animation.
    ///
    /// When used with the `to` attribute, the animation will change the modified attribute from
    /// the from value to the to value.
    ///
    /// When used with the `by` attribute, the animation will change the attribute relatively
    /// from the from value by the value specified in by.
    member _.from (value: int) = h.MakeAttr("from", Util.asString value)
    /// Indicates the initial value of the attribute that will be modified during the animation.
    ///
    /// When used with the `to` attribute, the animation will change the modified attribute from
    /// the from value to the to value.
    ///
    /// When used with the `by` attribute, the animation will change the attribute relatively
    /// from the from value by the value specified in by.
    member _.from (values: seq<int>) = h.MakeAttr("from", (values |> Seq.map Util.asString |> String.concat " "))
    /// Indicates the initial value of the attribute that will be modified during the animation.
    ///
    /// When used with the `to` attribute, the animation will change the modified attribute from
    /// the from value to the to value.
    ///
    /// When used with the `by` attribute, the animation will change the attribute relatively
    /// from the from value by the value specified in by.
    member _.from (value: string) = h.MakeAttr("from", value)
    /// Indicates the initial value of the attribute that will be modified during the animation.
    ///
    /// When used with the `to` attribute, the animation will change the modified attribute from
    /// the from value to the to value.
    ///
    /// When used with the `by` attribute, the animation will change the attribute relatively
    /// from the from value by the value specified in by.
    member _.from (values: seq<string>) = h.MakeAttr("from", (values |> String.concat " "))

    /// Defines the radius of the focal point for the radial gradient.
    member _.fr (value: float) = h.MakeAttr("fr", Util.asString value)
    /// Defines the radius of the focal point for the radial gradient.
    member _.fr (value: int) = h.MakeAttr("fr", Util.asString value)
    /// Defines the radius of the focal point for the radial gradient.
    member _.fr (value: ICssUnit) = h.MakeAttr("fr", Util.asString value)

    /// Defines the x-axis coordinate of the focal point for a radial gradient.
    member _.fx (value: float) = h.MakeAttr("fx", Util.asString value)
    /// Defines the x-axis coordinate of the focal point for a radial gradient.
    member _.fx (value: int) = h.MakeAttr("fx", Util.asString value)
    /// Defines the x-axis coordinate of the focal point for a radial gradient.
    member _.fx (value: ICssUnit) = h.MakeAttr("fx", Util.asString value)

    /// Defines the y-axis coordinate of the focal point for a radial gradient.
    member _.fy (value: float) = h.MakeAttr("fy", Util.asString value)
    /// Defines the y-axis coordinate of the focal point for a radial gradient.
    member _.fy (value: int) = h.MakeAttr("fy", Util.asString value)
    /// Defines the y-axis coordinate of the focal point for a radial gradient.
    member _.fy (value: ICssUnit) = h.MakeAttr("fy", Util.asString value)

    /// Defines an optional additional transformation from the gradient coordinate system
    /// onto the target coordinate system (i.e., userSpaceOnUse or objectBoundingBox).
    ///
    /// This allows for things such as skewing the gradient. This additional transformation
    /// matrix is post-multiplied to (i.e., inserted to the right of) any previously defined
    /// transformations, including the implicit transformation necessary to convert from object
    /// bounding box units to user space.
    member _.gradientTransform (transform: ITransformProperty) =
        h.MakeAttr("gradientTransform", (Util.asString transform))
    /// Defines optional additional transformation(s) from the gradient coordinate system
    /// onto the target coordinate system (i.e., userSpaceOnUse or objectBoundingBox).
    ///
    /// This allows for things such as skewing the gradient. This additional transformation
    /// matrix is post-multiplied to (i.e., inserted to the right of) any previously defined
    /// transformations, including the implicit transformation necessary to convert from object
    /// bounding box units to user space.
    member _.gradientTransform (transforms: seq<ITransformProperty>) =
        h.MakeAttr("gradientTransform", transforms |> Seq.map Util.asString |> String.concat " ")

    /// Prevents rendering of given element, while keeping child elements, e.g. script elements, active.
    member _.hidden (value: bool) = h.MakeBooleanAttr("hidden", value)

    /// Specifies the height of elements listed here. For all other elements, use the CSS height property.
    ///
    /// HTML: <canvas>, <embed>, <iframe>, <img>, <input>, <object>, <video>
    ///
    /// SVG: <feBlend>, <feColorMatrix>, <feComponentTransfer>, <feComposite>, <feConvolveMatrix>,
    /// <feDiffuseLighting>, <feDisplacementMap>, <feDropShadow>, <feFlood>, <feGaussianBlur>, <feImage>,
    /// <feMerge>, <feMorphology>, <feOffset>, <feSpecularLighting>, <feTile>, <feTurbulence>, <filter>,
    /// <mask>, <pattern>
    member _.height (value: float) = h.MakeAttr("height", Util.asString value)
    /// Specifies the height of elements listed here. For all other elements, use the CSS height property.
    ///
    /// HTML: <canvas>, <embed>, <iframe>, <img>, <input>, <object>, <video>
    ///
    /// SVG: <feBlend>, <feColorMatrix>, <feComponentTransfer>, <feComposite>, <feConvolveMatrix>,
    /// <feDiffuseLighting>, <feDisplacementMap>, <feDropShadow>, <feFlood>, <feGaussianBlur>, <feImage>,
    /// <feMerge>, <feMorphology>, <feOffset>, <feSpecularLighting>, <feTile>, <feTurbulence>, <filter>,
    /// <mask>, <pattern>
    member _.height (value: ICssUnit) = h.MakeAttr("height", Util.asString value)
    /// Specifies the height of elements listed here. For all other elements, use the CSS height property.
    ///
    /// HTML: <canvas>, <embed>, <iframe>, <img>, <input>, <object>, <video>
    ///
    /// SVG: <feBlend>, <feColorMatrix>, <feComponentTransfer>, <feComposite>, <feConvolveMatrix>,
    /// <feDiffuseLighting>, <feDisplacementMap>, <feDropShadow>, <feFlood>, <feGaussianBlur>, <feImage>,
    /// <feMerge>, <feMorphology>, <feOffset>, <feSpecularLighting>, <feTile>, <feTurbulence>, <filter>,
    /// <mask>, <pattern>
    member _.height (value: int) = h.MakeAttr("height", Util.asString value)

    /// The lower numeric bound of the high end of the measured range. This must be less than the
    /// maximum value (max attribute), and it also must be greater than the low value and minimum
    /// value (low attribute and min attribute, respectively), if any are specified. If unspecified,
    /// or if greater than the maximum value, the high value is equal to the maximum value.
    member _.high (value: float) = h.MakeAttr("high", Util.asString value)
    /// The lower numeric bound of the high end of the measured range. This must be less than the
    /// maximum value (max attribute), and it also must be greater than the low value and minimum
    /// value (low attribute and min attribute, respectively), if any are specified. If unspecified,
    /// or if greater than the maximum value, the high value is equal to the maximum value.
    member _.high (value: int) = h.MakeAttr("high", Util.asString value)

    /// The URL of a linked resource.
    member _.href (value: string) = h.MakeAttr("href", value)

    /// Indicates the language of the linked resource. Allowed values are determined by BCP47.
    ///
    /// Use this attribute only if the href attribute is present.
    member _.hrefLang (value: string) = h.MakeAttr("hreflang", value)

    member _.htmlFor (value: string) = h.MakeAttr("for", value)

    /// Often used with CSS to style a specific element. The value of this attribute must be unique.
    member _.id (value: int) = h.MakeAttr("id", (Util.asString value))
    /// Often used with CSS to style a specific element. The value of this attribute must be unique.
    member _.id (value: string) = h.MakeAttr("id", value)

    // /// Alias for `dangerouslySetInnerHTML`, sets the inner Html content of the element.
    // member _.innerHtml (content: string) = h.MakeAttr("dangerouslySetInnerHTML", (createObj [ "__html" ==> content ]))

    /// Contains inline metadata that a user agent can use to verify that a fetched resource
    /// has been delivered free of unexpected manipulation.
    member _.integrity (value: string) = h.MakeAttr("integrity", value)

    /// Defines the intercept of the linear function of color component transfers when the type
    /// attribute is set to linear.
    member _.intercept (value: float) = h.MakeAttr("intercept", Util.asString value)
    /// Defines the intercept of the linear function of color component transfers when the type
    /// attribute is set to linear.
    member _.intercept (value: int) = h.MakeAttr("intercept", Util.asString value)

    /// Sets the checked attribute for an element.
    member _.isChecked (value: bool) = h.MakeBooleanAttr("checked", value)

    /// Sets the open attribute for an element.
    member _.isOpen (value: bool) = h.MakeBooleanAttr("open", value)

    /// Defines one of the values to be used within the the arithmetic operation of the
    /// <feComposite> filter primitive.
    member _.k1 (value: float) = h.MakeAttr("k1", Util.asString value)
    /// Defines one of the values to be used within the the arithmetic operation of the
    /// <feComposite> filter primitive.
    member _.k1 (value: int) = h.MakeAttr("k1", Util.asString value)

    /// Defines one of the values to be used within the the arithmetic operation of the
    /// <feComposite> filter primitive.
    member _.k2 (value: float) = h.MakeAttr("k2", Util.asString value)
    /// Defines one of the values to be used within the the arithmetic operation of the
    /// <feComposite> filter primitive.
    member _.k2 (value: int) = h.MakeAttr("k2", Util.asString value)

    /// Defines one of the values to be used within the the arithmetic operation of the
    /// <feComposite> filter primitive.
    member _.k3 (value: float) = h.MakeAttr("k3", Util.asString value)
    /// Defines one of the values to be used within the the arithmetic operation of the
    /// <feComposite> filter primitive.
    member _.k3 (value: int) = h.MakeAttr("k3", Util.asString value)

    /// Defines one of the values to be used within the the arithmetic operation of the
    /// <feComposite> filter primitive.
    member _.k4 (value: float) = h.MakeAttr("k4", Util.asString value)
    /// Defines one of the values to be used within the the arithmetic operation of the
    /// <feComposite> filter primitive.
    member _.k4 (value: int) = h.MakeAttr("k4", Util.asString value)

    /// Defines the list of numbers that make up the kernel matrix for the
    /// <feConvolveMatrix> element.
    member _.kernelMatrix (values: seq<float>) = h.MakeAttr("kernelMatrix", (values |> Seq.map Util.asString |> String.concat " "))
    /// Defines the list of numbers that make up the kernel matrix for the
    /// <feConvolveMatrix> element.
    member _.kernelMatrix (values: seq<int>) = h.MakeAttr("kernelMatrix", (values |> Seq.map Util.asString  |> String.concat " "))

    /// Indicates the simple duration of an animation.
    member _.keyPoints (values: seq<float>) =
        h.MakeAttr("keyPoints", (values |> Seq.map Util.asString  |> String.concat ";"))

    // /// Indicates the simple duration of an animation.
    // ///
    // /// Each control point description is a set of four values: x1 y1 x2 y2, describing the Bézier
    // /// control points for one time segment.
    // ///
    // /// The keyTimes values that define the associated segment are the Bézier "anchor points",
    // /// and the keySplines values are the control points. Thus, there must be one fewer sets of
    // /// control points than there are keyTimes.
    // ///
    // /// The values of x1 y1 x2 y2 must all be in the range 0 to 1.
    // member _.keySplines (values: seq<float * float * float * float>) =
    //     PropHelpers.createKeySplines(values)
    //     |> h.MakeAttr("keySplines",)

    /// Indicates the simple duration of an animation.
    member _.keyTimes (values: seq<float>) =
        h.MakeAttr("keyTimes", (values |> Seq.map Util.asString |> String.concat ";"))

    /// Helps define the language of an element: the language that non-editable elements are
    /// written in, or the language that the editable elements should be written in by the user.
    member _.lang (value: string) = h.MakeAttr("lang", value)

    /// Defines the color of the light source for lighting filter primitives.
    member _.lightingColor (value: string) = h.MakeAttr("lighting-color", value)

    /// Represents the angle in degrees between the spot light axis (i.e. the axis between the
    /// light source and the point to which it is pointing at) and the spot light cone. So it
    /// defines a limiting cone which restricts the region where the light is projected.
    ///
    /// No light is projected outside the cone.
    member _.limitingConeAngle (value: float) = h.MakeAttr("limitingConeAngle", Util.asString value)
    /// Represents the angle in degrees between the spot light axis (i.e. the axis between the
    /// light source and the point to which it is pointing at) and the spot light cone. So it
    /// defines a limiting cone which restricts the region where the light is projected.
    ///
    /// No light is projected outside the cone.
    member _.limitingConeAngle (value: int) = h.MakeAttr("limitingConeAngle", Util.asString value)

    /// If true, the browser will automatically seek back to the start upon reaching the end of the video.
    member _.loop (value: bool) = h.MakeBooleanAttr("loop", value)

    /// The upper numeric bound of the low end of the measured range. This must be greater than
    /// the minimum value (min attribute), and it also must be less than the high value and
    /// maximum value (high attribute and max attribute, respectively), if any are specified.
    /// If unspecified, or if less than the minimum value, the low value is equal to the minimum value.
    member _.low (value: float) = h.MakeAttr("low", Util.asString value)
    /// The upper numeric bound of the low end of the measured range. This must be greater than
    /// the minimum value (min attribute), and it also must be less than the high value and
    /// maximum value (high attribute and max attribute, respectively), if any are specified.
    /// If unspecified, or if less than the minimum value, the low value is equal to the minimum value.
    member _.low (value: int) = h.MakeAttr("low", Util.asString value)
    /// Indicates the maximum value allowed.
    member _.max (value: float) = h.MakeAttr("max", Util.asString value)
    /// Indicates the maximum value allowed.
    member _.max (value: int) = h.MakeAttr("max", Util.asString value)
    /// Indicates the maximum value allowed.
    member _.max (value: DateTime) = h.MakeAttr("max", value.ToString("yyyy-MM-dd"))

    /// Defines the maximum number of characters allowed in the element.
    member _.maxLength (value: int) = h.MakeAttr("maxlength", Util.asString value)

    /// This attribute specifies the media that the linked resource applies to.
    /// Its value must be a media type / media query. This attribute is mainly useful
    /// when linking to external stylesheets — it allows the user agent to pick the
    /// best adapted one for the device it runs on.
    ///
    /// In HTML 4, this can only be a simple white-space-separated list of media
    /// description literals, i.e., media types and groups, where defined and allowed
    /// as values for this attribute, such as print, screen, aural, braille. HTML5
    /// extended this to any kind of media queries, which are a superset of the allowed
    /// values of HTML 4.
    ///
    /// Browsers not supporting CSS3 Media Queries won't necessarily recognize the adequate
    /// link; do not forget to set fallback links, the restricted set of media queries
    /// defined in HTML 4.
    member _.media (value: string) = h.MakeAttr("media", value)

    /// Defines which HTTP method to use when submitting the form. Can be GET (default) or POST.
    member _.method (value: string) = h.MakeAttr("method", value)

    /// Indicates the minimum value allowed.
    member _.min (value: float) = h.MakeAttr("min", Util.asString value)
    /// Indicates the minimum value allowed.
    member _.min (value: int) = h.MakeAttr("min", Util.asString value)
    /// Indicates the minimum value allowed.
    member _.min (value: DateTime) = h.MakeAttr("min", value.ToString("yyyy-MM-dd"))

    /// Defines the minimum number of characters allowed in the element.
    member _.minLength (value: int) = h.MakeAttr("minlength", Util.asString value)

    /// Indicates whether multiple values can be entered in an input of the type email or file.
    member _.multiple (value: bool) = h.MakeBooleanAttr("multiple", value)

    /// Indicates whether the audio will be initially silenced on page load.
    member _.muted (value: bool) = h.MakeBooleanAttr("muted", value)

    /// Name of the element.
    ///
    /// For example used by the server to identify the fields in form submits.
    member _.name (value: string) = h.MakeAttr("name", value)

    /// This Boolean attribute is set to indicate that the script should not be executed in
    /// browsers that support ES2015 modules — in effect, this can be used to serve fallback
    /// scripts to older browsers that do not support modular JavaScript code.
    member _.nomodule (value: bool) = h.MakeBooleanAttr("nomodule", value)

    /// A cryptographic nonce (number used once) to whitelist scripts in a script-src
    /// Content-Security-Policy. The server must generate a unique nonce value each time
    /// it transmits a policy. It is critical to provide a nonce that cannot be guessed
    /// as bypassing a resource's policy is otherwise trivial.
    member _.nonce (value: string) = h.MakeAttr("nonce", value)

    /// Defines the number of octaves for the noise function of the <feTurbulence> primitive.
    member _.numOctaves (value: int) = h.MakeAttr("numOctaves", Util.asString value)

    /// SVG attribute to define where the gradient color will begin or end.
    member _.offset (value: float) = h.MakeAttr("offset", Util.asString value)
    /// SVG attribute to define where the gradient color will begin or end.
    member _.offset (value: ICssUnit) = h.MakeAttr("offset", Util.asString value)
    /// SVG attribute to define where the gradient color will begin or end.
    member _.offset (value: int) = h.MakeAttr("offset", Util.asString value)

    /// This attribute indicates the optimal numeric value. It must be within the range (as defined by the min
    /// attribute and max attribute). When used with the low attribute and high attribute, it gives an indication
    /// where along the range is considered preferable. For example, if it is between the min attribute and the
    /// low attribute, then the lower range is considered preferred.
    member _.optimum (value: float) = h.MakeAttr("optimum", Util.asString value)
    /// This attribute indicates the optimal numeric value. It must be within the range (as defined by the min
    /// attribute and max attribute). When used with the low attribute and high attribute, it gives an indication
    /// where along the range is considered preferable. For example, if it is between the min attribute and the
    /// low attribute, then the lower range is considered preferred.
    member _.optimum (value: int) = h.MakeAttr("optimum", Util.asString value)

    /// Indicates the minimum value allowed.
    member _.order (value: int) = h.MakeAttr("order", Util.asString value)
    /// Indicates the minimum value allowed.
    member _.order (values: seq<int>) = h.MakeAttr("order", (values |> Seq.map Util.asString |> String.concat " "))

    /// Represents the ideal vertical position of the overline.
    ///
    /// The overline position is expressed in the font's coordinate system.
    member _.overlinePosition (value: float) = h.MakeAttr("overline-position", Util.asString value)
    /// Represents the ideal vertical position of the overline.
    ///
    /// The overline position is expressed in the font's coordinate system.
    member _.overlinePosition (value: int) = h.MakeAttr("overline-position", Util.asString value)

    /// Represents the ideal thickness of the overline.
    ///
    /// The overline thickness is expressed in the font's coordinate system.
    member _.overlineThickness (value: float) = h.MakeAttr("overline-thickness", Util.asString value)
    /// Represents the ideal thickness of the overline.
    ///
    /// The overline thickness is expressed in the font's coordinate system.
    member _.overlineThickness (value: int) = h.MakeAttr("overline-thickness", Util.asString value)

    // /// It either defines a text path along which the characters of a text are rendered, or a motion
    // /// path along which a referenced element is animated.
    // member _.path (path: seq<char * (float list list)>) =
    //     PropHelpers.createSvgPathFloat path
    //     |> h.MakeAttr("path")
    // /// It either defines a text path along which the characters of a text are rendered, or a motion
    // /// path along which a referenced element is animated.
    // member _.path (path: seq<char * (int list list)>) =
    //     PropHelpers.createSvgPathInt path
    //     |> h.MakeAttr("path")
    /// It either defines a text path along which the characters of a text are rendered, or a motion
    /// path along which a referenced element is animated.
    member _.path (path: string) = h.MakeAttr("path", path)
    /// The part global attribute contains a space-separated list of the part names of the element.
    /// Part names allows CSS to select and style specific elements in a shadow tree
    member _.part(value: string) = h.MakeAttr("part", value)
    /// The part global attribute contains a space-separated list of the part names of the element.
    /// Part names allows CSS to select and style specific elements in a shadow tree
    member _.part(values: #seq<string>) = h.MakeAttr("part", String.concat " " values)
    /// Specifies a total length for the path, in user units.
    ///
    /// This value is then used to calibrate the browser's distance calculations with those of the
    /// author, by scaling all distance computations using the ratio pathLength/(computed value of
    /// path length).
    ///
    /// This can affect the actual rendered lengths of paths; including text paths, animation paths,
    /// and various stroke operations. Basically, all computations that require the length of the path.
    member _.pathLength (value: int) = h.MakeAttr("pathLength", Util.asString value)

    /// Sets the input field allowed input.
    ///
    /// This attribute only applies when the value of the type attribute is text, search, tel, url or email.
    member _.pattern (value: string) = h.MakeAttr("pattern", value)

    /// Defines a list of transform definitions that are applied to a pattern tile.
    member _.patternTransform (transform: ITransformProperty) =
        h.MakeAttr("patternTransform", (Util.asString transform))
    /// Defines a list of transform definitions that are applied to a pattern tile.
    member _.patternTransform (transforms: seq<ITransformProperty>) =
        h.MakeAttr("patternTransform", transforms |> Seq.map Util.asString |> String.concat " ")

    /// Provides a hint to the user of what can be entered in the field.
    member _.placeholder (value: string) = h.MakeAttr("placeholder", value)

    /// Indicating that the video is to be played "inline", that is within the element's playback area.
    ///
    /// Note that the absence of this attribute does not imply that the video will always be played in fullscreen.
    member _.playsInline (value: bool) = h.MakeBooleanAttr("playsinline", value)

    /// Contains a space-separated list of URLs to which, when the hyperlink is followed,
    /// POST requests with the body PING will be sent by the browser (in the background).
    ///
    /// Typically used for tracking.
    member _.ping (value: string) = h.MakeAttr("ping", value)
    /// Contains a space-separated list of URLs to which, when the hyperlink is followed,
    /// POST requests with the body PING will be sent by the browser (in the background).
    ///
    /// Typically used for tracking.
    member _.ping (urls: #seq<string>) = h.MakeAttr("ping", (urls |> String.concat " "))

    // /// Defines a list of points.
    // ///
    // /// Each point is defined by a pair of numbers representing a X and a Y coordinate in
    // /// the user coordinate system.
    // member _.points (coordinates: seq<float * float>) =
    //     PropHelpers.createPointsFloat(coordinates)
    //     |> h.MakeAttr("points")
    // /// Defines a list of points.
    // ///
    // /// Each point is defined by a pair of numbers representing a X and a Y coordinate in
    // /// the user coordinate system.
    // member _.points (coordinates: seq<int * int>) =
    //     PropHelpers.createPointsInt(coordinates)
    //     |> h.MakeAttr("points")

    /// Defines a list of points.
    ///
    /// Each point is defined by a pair of numbers representing a X and a Y coordinate in
    /// the user coordinate system.
    member _.points (coordinates: string) = h.MakeAttr("points", coordinates)

    /// Represents the x location in the coordinate system established by attribute primitiveUnits
    /// on the <filter> element of the point at which the light source is pointing.
    member _.pointsAtX (value: float) = h.MakeAttr("pointsAtX", Util.asString value)
    /// Represents the x location in the coordinate system established by attribute primitiveUnits
    /// on the <filter> element of the point at which the light source is pointing.
    member _.pointsAtX (value: int) = h.MakeAttr("pointsAtX", Util.asString value)

    /// Represents the y location in the coordinate system established by attribute primitiveUnits
    /// on the <filter> element of the point at which the light source is pointing.
    member _.pointsAtY (value: float) = h.MakeAttr("pointsAtY", Util.asString value)
    /// Represents the y location in the coordinate system established by attribute primitiveUnits
    /// on the <filter> element of the point at which the light source is pointing.
    member _.pointsAtY (value: int) = h.MakeAttr("pointsAtY", Util.asString value)

    /// Represents the y location in the coordinate system established by attribute primitiveUnits
    /// on the <filter> element of the point at which the light source is pointing, assuming that,
    /// in the initial local coordinate system, the positive z-axis comes out towards the person
    /// viewing the content and assuming that one unit along the z-axis equals one unit in x and y.
    member _.pointsAtZ (value: float) = h.MakeAttr("pointsAtZ", Util.asString value)
    /// Represents the y location in the coordinate system established by attribute primitiveUnits
    /// on the <filter> element of the point at which the light source is pointing, assuming that,
    /// in the initial local coordinate system, the positive z-axis comes out towards the person
    /// viewing the content and assuming that one unit along the z-axis equals one unit in x and y.
    member _.pointsAtZ (value: int) = h.MakeAttr("pointsAtZ", Util.asString value)

    /// Indicates how a <feConvolveMatrix> element handles alpha transparency.
    member _.preserveAlpha (value: bool) = h.MakeBooleanAttr("preserveAlpha", value)

    /// A URL for an image to be shown while the video is downloading. If this attribute isn't specified, nothing
    /// is displayed until the first frame is available, then the first frame is shown as the poster frame.
    member _.poster (value: string) = h.MakeAttr("poster", value)

    /// SVG attribute to define the radius of a circle.
    member _.r (value: float) = h.MakeAttr("r", Util.asString value)
    /// SVG attribute to define the radius of a circle.
    member _.r (value: ICssUnit) = h.MakeAttr("r", Util.asString value)
    /// SVG attribute to define the radius of a circle.
    member _.r (value: int) = h.MakeAttr("r", Util.asString value)

    /// Represents the radius (or radii) for the operation on a given <feMorphology> filter primitive.
    member _.radius (value: float) = h.MakeAttr("radius", Util.asString value)
    /// Represents the radius (or radii) for the operation on a given <feMorphology> filter primitive.
    member _.radius (value: int) = h.MakeAttr("radius", Util.asString value)
    /// Represents the radius (or radii) for the operation on a given <feMorphology> filter primitive.
    member _.radius (xRadius: float, yRadius: float) = h.MakeAttr("radius", (Util.asString xRadius  + "," + Util.asString yRadius))
    /// Represents the radius (or radii) for the operation on a given <feMorphology> filter primitive.
    member _.radius (xRadius: float, yRadius: int) = h.MakeAttr("radius", (Util.asString xRadius  + "," + Util.asString yRadius))
    /// Represents the radius (or radii) for the operation on a given <feMorphology> filter primitive.
    member _.radius (xRadius: int, yRadius: float) = h.MakeAttr("radius", (Util.asString xRadius  + "," + Util.asString yRadius))
    /// Represents the radius (or radii) for the operation on a given <feMorphology> filter primitive.
    member _.radius (xRadius: int, yRadius: int) = h.MakeAttr("radius", (Util.asString xRadius  + "," + Util.asString yRadius))

    /// Indicates whether the element can be edited.
    member _.readOnly (value: bool) = h.MakeBooleanAttr("readOnly", value)

    // /// Used to reference a DOM element or class component from within a parent component.
    // member _.ref (handler: Element -> unit) = h.MakeAttr("ref", handler)
    // /// Used to reference a DOM element or class component from within a parent component.
    // member _.ref (ref: IRefValue<#HTMLElement option>) = h.MakeAttr("ref", ref)

    /// For anchors containing the href attribute, this attribute specifies the relationship
    /// of the target object to the link object. The value is a space-separated list of link
    /// types values. The values and their semantics will be registered by some authority that
    /// might have meaning to the document author. The default relationship, if no other is
    /// given, is void.
    ///
    /// Use this attribute only if the href attribute is present.
    member _.rel (value: string) = h.MakeAttr("rel", value)

    /// Indicates whether this element is required to fill out or not.
    member _.required (value: bool) = h.MakeBooleanAttr("required", value)

    /// Defines the assigned name for this filter primitive.
    ///
    /// If supplied, then graphics that result from processing this filter primitive can be
    /// referenced by an in attribute on a subsequent filter primitive within the same
    /// <filter> element.
    ///
    /// If no value is provided, the output will only be available for re-use as the implicit
    /// input into the next filter primitive if that filter primitive provides no value for
    /// its in attribute.
    member _.result (value: string) = h.MakeAttr("result", value)

    /// Sets the aria role
    ///
    /// https://www.w3.org/WAI/PF/aria-1.1/roles
    member _.role ([<System.ParamArray>] roles: string[]) = h.MakeAttr("role", String.concat " " roles)

    /// Defines the number of rows in a text area.
    member _.rows (value: int) = h.MakeAttr("rows", Util.asString value)

    /// Defines the number of rows a table cell should span over.
    member _.rowSpan (value: int) = h.MakeAttr("rowspan", Util.asString value)

    /// The SVG rx attribute defines a radius on the x-axis.
    ///
    /// Two elements are using this attribute: <ellipse>, and <rect>
    member _.rx (value: float) = h.MakeAttr("rx", Util.asString value)
    /// The SVG rx attribute defines a radius on the x-axis.
    ///
    /// Two elements are using this attribute: <ellipse>, and <rect>
    member _.rx (value: ICssUnit) = h.MakeAttr("rx", Util.asString value)
    /// The SVG rx attribute defines a radius on the x-axis.
    ///
    /// Two elements are using this attribute: <ellipse>, and <rect>
    member _.rx (value: int) = h.MakeAttr("rx", Util.asString value)

    /// The SVG ry attribute defines a radius on the y-axis.
    ///
    /// Two elements are using this attribute: <ellipse>, and <rect>
    member _.ry (value: float) = h.MakeAttr("ry", Util.asString value)
    /// The SVG ry attribute defines a radius on the y-axis.
    ///
    /// Two elements are using this attribute: <ellipse>, and <rect>
    member _.ry (value: ICssUnit) = h.MakeAttr("ry", Util.asString value)
    /// The SVG ry attribute defines a radius on the y-axis.
    ///
    /// Two elements are using this attribute: <ellipse>, and <rect>
    member _.ry (value: int) = h.MakeAttr("ry", Util.asString value)

    /// Applies extra restrictions to the content in the frame.
    ///
    /// The value of the attribute can either be empty to apply all restrictions,
    /// or space-separated tokens to lift particular restrictions
    member _.sandbox (values: #seq<string>) = h.MakeAttr("sandbox", (values |> String.concat " "))

    /// Defines the displacement scale factor to be used on a <feDisplacementMap> filter primitive.
    ///
    /// The amount is expressed in the coordinate system established by the primitiveUnits attribute
    /// on the <filter> element.
    member _.scale (value: float) = h.MakeAttr("scale", Util.asString value)
    /// Defines the displacement scale factor to be used on a <feDisplacementMap> filter primitive.
    ///
    /// The amount is expressed in the coordinate system established by the primitiveUnits attribute
    /// on the <filter> element.
    member _.scale (value: int) = h.MakeAttr("scale", Util.asString value)

    /// Represents the starting number for the pseudo random number generator of the <feTurbulence>
    /// filter primitive.
    member _.seed (value: float) = h.MakeAttr("seed", Util.asString value)
    /// Represents the starting number for the pseudo random number generator of the <feTurbulence>
    /// filter primitive.
    member _.seed (value: int) = h.MakeAttr("seed", Util.asString value)

    /// Defines a value which will be selected on page load.
    member _.selected (value: bool) = h.MakeBooleanAttr("selected", value)

    /// Sets the beginning index of the selected text.
    ///
    /// When nothing is selected, this returns the position of the text input cursor (caret) inside of the <input> element.
    member _.selectionStart (value: int) = h.MakeAttr("selectionStart", Util.asString value)

    /// Sets the end index of the selected text.
    ///
    /// When there's no selection, this returns the offset of the character immediately following the current text input cursor position.
    member _.selectionEnd (value: int) = h.MakeAttr("selectionStart", Util.asString value)

    /// Sets the *visual* size of the control.
    ///
    /// The value is in pixels unless the value of type is text or password, in which case, it is the number of characters.
    ///
    /// This attribute only applies when type is set to text, search, tel, url, email, or password.
    member _.size (value: int) = h.MakeAttr("size", Util.asString value)

    /// Defines the sizes of the icons for visual media contained in the resource.
    /// It must be present only if the rel contains a value of icon or a non-standard
    /// type such as Apple's apple-touch-icon.
    ///
    /// It may have the following values:
    ///
    /// `any`, meaning that the icon can be scaled to any size as it is in a vector
    /// format, like image/svg+xml.
    ///
    /// A white-space separated list of sizes, each in the format `<width in pixels>x<height in pixels>`
    /// or `<width in pixels>X<height in pixels>`. Each of these sizes must be contained in the resource.
    member _.sizes (value: string) = h.MakeAttr("sizes", value)

    /// This attribute contains a positive integer indicating the number of consecutive
    /// columns the <col> element spans. If not present, its default value is 1.
    member _.spam (value: int) = h.MakeAttr("span", Util.asString value)

    /// Defines whether the element may be checked for spelling errors.
    member _.spellcheck (value: bool) = h.MakeBooleanAttr("spellcheck", value)

    /// Controls the ratio of reflection of the specular lighting.
    ///
    /// It represents the ks value in the Phong lighting model. The bigger the value the stronger the reflection.
    member _.specularConstant (value: float) = h.MakeAttr("specularConstant", Util.asString value)
    /// Controls the ratio of reflection of the specular lighting.
    ///
    /// It represents the ks value in the Phong lighting model. The bigger the value the stronger the reflection.
    member _.specularConstant (value: int) = h.MakeAttr("specularConstant", Util.asString value)

    /// For <feSpecularLighting>, specularExponent defines the exponent value for the specular term.
    ///
    /// For <feSpotLight>, specularExponent defines the exponent value controlling the focus for the light source.
    member _.specularExponent (value: float) = h.MakeAttr("specularExponent", Util.asString value)
    /// For <feSpecularLighting>, specularExponent defines the exponent value for the specular term.
    ///
    /// For <feSpotLight>, specularExponent defines the exponent value controlling the focus for the light source.
    member _.specularExponent (value: int) = h.MakeAttr("specularExponent", Util.asString value)

    /// The URL of the embeddable content.
    member _.src (value: string) = h.MakeAttr("src", value)

    /// Language of the track text data. It must be a valid BCP 47 language tag.
    ///
    /// If the kind attribute is set to subtitles, then srclang must be defined.
    member _.srcLang (value: string) = h.MakeAttr("srclang", value)

    /// One or more responsive image candidates.
    member _.srcset (value: string) = h.MakeAttr("srcset", value)

    /// Defines the first number if other than 1.
    member _.start (value: string) = h.MakeAttr("start", value)

    /// Defines the standard deviation for the blur operation.
    member _.stdDeviation (value: float) = h.MakeAttr("stdDeviation", Util.asString value)
    /// Defines the standard deviation for the blur operation.
    member _.stdDeviation (value: int) = h.MakeAttr("stdDeviation", Util.asString value)
    /// Defines the standard deviation for the blur operation.
    member _.stdDeviation (xAxis: float, yAxis: float) = h.MakeAttr("stdDeviation", (Util.asString xAxis  + "," + Util.asString yAxis))
    /// Defines the standard deviation for the blur operation.
    member _.stdDeviation (xAxis: float, yAxis: int) = h.MakeAttr("stdDeviation", (Util.asString xAxis  + "," + Util.asString yAxis))
    /// Defines the standard deviation for the blur operation.
    member _.stdDeviation (xAxis: int, yAxis: float) = h.MakeAttr("stdDeviation", (Util.asString xAxis  + "," + Util.asString yAxis))
    /// Defines the standard deviation for the blur operation.
    member _.stdDeviation (xAxis: int, yAxis: int) = h.MakeAttr("stdDeviation", (Util.asString xAxis  + "," + Util.asString yAxis))

    /// Indicates the stepping interval.
    member _.step (value: float) = h.MakeAttr("step", Util.asString value)
    /// Indicates the stepping interval.
    member _.step (value: int) = h.MakeAttr("step", Util.asString value)
    /// The slot global attribute assigns a slot in a shadow DOM shadow tree to an element: An element with a slot attribute is assigned to the slot created by the slot element whose name attribute's value matches that slot attribute's value.
    member _.slot(value: string) = h.MakeAttr("slot", value)
    /// SVG attribute to indicate what color to use at a gradient stop.
    member _.stopColor (value: string) = h.MakeAttr("stop-color", value)

    /// SVG attribute to define the opacity of a given color gradient stop.
    member _.stopOpacity (value: float) = h.MakeAttr("stop-opacity", Util.asString value)
    /// SVG attribute to define the opacity of a given color gradient stop.
    member _.stopOpacity (value: int) = h.MakeAttr("stop-opacity", Util.asString value)

    /// Represents the ideal vertical position of the strikethrough.
    ///
    /// The strikethrough position is expressed in the font's coordinate system.
    member _.strikethroughPosition (value: float) = h.MakeAttr("strikethrough-position", Util.asString value)
    /// Represents the ideal vertical position of the strikethrough.
    ///
    /// The strikethrough position is expressed in the font's coordinate system.
    member _.strikethroughPosition (value: int) = h.MakeAttr("strikethrough-position", Util.asString value)

    /// Represents the ideal vertical position of the strikethrough.
    ///
    /// The strikethrough position is expressed in the font's coordinate system.
    member _.strikethroughThickness (value: float) = h.MakeAttr("strikethrough-thickness", Util.asString value)
    /// Represents the ideal thickness of the strikethrough.
    ///
    /// The strikethrough thickness is expressed in the font's coordinate system.
    member _.strikethroughThickness (value: int) = h.MakeAttr("strikethrough-thickness", Util.asString value)

    /// SVG attribute to define the color (or any SVG paint servers like gradients or patterns) used to paint the outline of the shape.
    member _.stroke (color: string) = h.MakeAttr("stroke", color)

    /// SVG attribute to define the width of the stroke to be applied to the shape.
    member _.strokeWidth (value: float) = h.MakeAttr("stroke-width", Util.asString value + "px")
    /// SVG attribute to define the width of the stroke to be applied to the shape.
    member _.strokeWidth (value: ICssUnit) = h.MakeAttr("stroke-width", Util.asString value)
    /// SVG attribute to define the width of the stroke to be applied to the shape.
    member _.strokeWidth (value: int) = h.MakeAttr("stroke-width", Util.asString value + "px")

    // member _.style (properties: #IStyleAttribute list) = h.MakeAttr("style", (createObj !!properties))

    /// Represents the height of the surface for a light filter primitive.
    member _.surfaceScale (value: float) = h.MakeAttr("surfaceScale", Util.asString value)
    /// Represents the height of the surface for a light filter primitive.
    member _.surfaceScale (value: int) = h.MakeAttr("surfaceScale", Util.asString value)

    /// Represents a list of supported language tags.
    ///
    /// This list is matched against the language defined in the user preferences.
    member _.systemLanguage (value: string) = h.MakeAttr("systemLanguage", value)

    /// The `tabindex` global attribute indicates that its element can be focused,
    /// and where it participates in sequential keyboard navigation (usually with the Tab key, hence the name).
    member _.tabIndex (index: int) = h.MakeAttr("tabindex", Util.asString index)

    /// Controls browser behavior when opening a link.
    member _.target (frameName: string) = h.MakeAttr("target", frameName)

    /// Determines the positioning in horizontal direction of the convolution matrix relative to a
    /// given target pixel in the input image.
    ///
    /// The leftmost column of the matrix is column number zero.
    ///
    /// The value must be such that:
    ///
    /// 0 <= targetX < orderX.
    member _.targetX (index: int) = h.MakeAttr("targetX", Util.asString index)

    /// Determines the positioning in vertical direction of the convolution matrix relative to a
    /// given target pixel in the input image.
    ///
    /// The topmost row of the matrix is row number zero.
    ///
    /// The value must be such that:
    ///
    /// 0 <= targetY < orderY.
    member _.targetY (index: int) = h.MakeAttr("targetY", Util.asString index)

    /// A shorthand for using prop.custom("data-testid", value). Useful for referencing elements when testing React code.
    member _.testId(value: string) = h.MakeAttr("data-testid", value)

    // /// Defines the text content of the element. Alias for `children [ Html.text (sprintf ...) ]`
    // member _.textf fmt = Printf.kprintf prop.text fmt

    /// Specifies the width of the space into which the text will draw.
    ///
    /// The user agent will ensure that the text does not extend farther than that distance, using the method or methods
    /// specified by the lengthAdjust attribute.
    member _.textLength (value: float) = h.MakeAttr("textLength", Util.asString value)
    /// Specifies the width of the space into which the text will draw.
    ///
    /// The user agent will ensure that the text does not extend farther than that distance, using the method or methods
    /// specified by the lengthAdjust attribute.
    member _.textLength (value: ICssUnit) = h.MakeAttr("textLength", Util.asString value)
    /// Specifies the width of the space into which the text will draw.
    ///
    /// The user agent will ensure that the text does not extend farther than that distance, using the method or methods
    /// specified by the lengthAdjust attribute.
    member _.textLength (value: int) = h.MakeAttr("textLength", Util.asString value)

    /// The title global attribute contains text representing advisory information related to the element it belongs to.
    member _.title (value: string) = h.MakeAttr("title", value)

    /// Indicates the initial value of the attribute that will be modified during the animation.
    ///
    /// When used with the `to` attribute, the animation will change the modified attribute from
    /// the from value to the to value.
    ///
    /// When used with the `by` attribute, the animation will change the attribute relatively
    /// from the from value by the value specified in by.
    member _.to' (value: float) = h.MakeAttr("to", Util.asString value)
    /// Indicates the initial value of the attribute that will be modified during the animation.
    ///
    /// When used with the `to` attribute, the animation will change the modified attribute from
    /// the from value to the to value.
    ///
    /// When used with the `by` attribute, the animation will change the attribute relatively
    /// from the from value by the value specified in by.
    member _.to' (values: seq<float>) = h.MakeAttr("to", (values |> Seq.map Util.asString |> String.concat " "))
    /// Indicates the initial value of the attribute that will be modified during the animation.
    ///
    /// When used with the `to` attribute, the animation will change the modified attribute from
    /// the from value to the to value.
    ///
    /// When used with the `by` attribute, the animation will change the attribute relatively
    /// from the from value by the value specified in by.
    member _.to' (value: int) = h.MakeAttr("to", Util.asString value)
    /// Indicates the initial value of the attribute that will be modified during the animation.
    ///
    /// When used with the `to` attribute, the animation will change the modified attribute from
    /// the from value to the to value.
    ///
    /// When used with the `by` attribute, the animation will change the attribute relatively
    /// from the from value by the value specified in by.
    member _.to' (values: seq<int>) = h.MakeAttr("to", (values |> Seq.map Util.asString |> String.concat " "))
    /// Indicates the initial value of the attribute that will be modified during the animation.
    ///
    /// When used with the `to` attribute, the animation will change the modified attribute from
    /// the from value to the to value.
    ///
    /// When used with the `by` attribute, the animation will change the attribute relatively
    /// from the from value by the value specified in by.
    member _.to' (value: string) = h.MakeAttr("to", value)
    /// Indicates the initial value of the attribute that will be modified during the animation.
    ///
    /// When used with the `to` attribute, the animation will change the modified attribute from
    /// the from value to the to value.
    ///
    /// When used with the `by` attribute, the animation will change the attribute relatively
    /// from the from value by the value specified in by.
    member _.to' (values: seq<string>) = h.MakeAttr("to", (values |> String.concat " "))

    /// Defines a list of transform definitions that are applied to an element and the element's children.
    member _.transform (transform: ITransformProperty) =
        h.MakeAttr("transform", (Util.asString transform))
    /// Defines a list of transform definitions that are applied to an element and the element's children.
    member _.transform (transforms: seq<ITransformProperty>) =
        let unitList = [ "px" ; "deg" ]
        let removeUnits (s : string) =
            List.fold (fun (ins:string) toReplace -> ins.Replace(toReplace,"")) s unitList
        h.MakeAttr("transform", transforms |> Seq.map Util.asString |> Seq.map removeUnits |> String.concat " ")

    /// Sets the `type` attribute for the element.
    member _.type' (value: string) = h.MakeAttr("type", value)

    /// Represents the ideal vertical position of the underline.
    ///
    /// The underline position is expressed in the font's coordinate system.
    member _.underlinePosition (value: float) = h.MakeAttr("underline-position", Util.asString value)
    /// Represents the ideal vertical position of the underline.
    ///
    /// The underline position is expressed in the font's coordinate system.
    member _.underlinePosition (value: int) = h.MakeAttr("underline-position", Util.asString value)

    /// Represents the ideal thickness of the underline.
    ///
    /// The underline thickness is expressed in the font's coordinate system.
    member _.underlineThickness (value: float) = h.MakeAttr("underline-thickness", Util.asString value)
    /// Represents the ideal thickness of the underline.
    ///
    /// The underline thickness is expressed in the font's coordinate system.
    member _.underlineThickness (value: int) = h.MakeAttr("underline-thickness", Util.asString value)

    /// A hash-name reference to a <map> element; that is a '#' followed by the value of a name of a map element.
    member _.usemap (value: string) = h.MakeAttr("usemap", value)

    /// Sets the value of a React controlled component.
    member _.value (value: bool) = h.MakeBooleanAttr("value", value)
    /// Sets the value of a React controlled component.
    member _.value (value: float) = h.MakeAttr("value", Util.asString value)
    /// Sets the value of a React controlled component.
    member _.value (value: Guid) = h.MakeAttr("value", (Util.asString value))
    /// Sets the value of a React controlled component.
    member _.value (value: int) = h.MakeAttr("value", Util.asString value)
    /// Sets the value of a React controlled component.
    member _.value (value: string) = h.MakeAttr("value", value)

(*
    /// Sets the value of a React controlled component.
    member _.value (value: seq<float>) = h.MakeAttr("value", ResizeArray value)
    /// Sets the value of a React controlled component.
    member _.value (value: seq<int>) = h.MakeAttr("value", ResizeArray value)
    /// Sets the value of a React controlled component.
    member _.value (value: seq<string>) = h.MakeAttr("value", ResizeArray value)

    /// The value of the element, interpreted as a date, or null if conversion is not possible.
    member _.valueAsDate (value: System.DateTime) = h.MakeAttr("valueAsDate", value)
    /// The value of the element, interpreted as a date, or null if conversion is not possible.
    member _.valueAsDate (value: System.DateTime option) = h.MakeAttr("valueAsDate", value)

    /// The value of the element, interpreted as a time value, number, or NaN if conversion is impossible.
    member _.valueAsNumber (value: float) = h.MakeAttr("valueAsNumber", Util.asString value)
    /// The value of the element, interpreted as a time value, number, or NaN if conversion is impossible.
    member _.valueAsNumber (value: float option) = h.MakeAttr("valueAsNumber", value)
    /// The value of the element, interpreted as a time value, number, or NaN if conversion is impossible.
    member _.valueAsNumber (value: int) = h.MakeAttr("valueAsNumber", Util.asString value)
    /// The value of the element, interpreted as a time value, number, or NaN if conversion is impossible.
    member _.valueAsNumber (value: int option) = h.MakeAttr("valueAsNumber", value)

    /// `prop.ref` callback that sets the value of an input after DOM element is created.
    /// Can be used instead of `prop.defaultValue` and `prop.value` props to override input value.
    member _.valueOrDefault (value: bool) =
        prop.ref (fun e -> if e |> isNull |> not && !!e?value <> !!value then e?value <- !!value)
    /// `prop.ref` callback that sets the value of an input after DOM element is created.
    /// Can be used instead of `prop.defaultValue` and `prop.value` props to override input value.
    member _.valueOrDefault (value: float) =
        prop.ref (fun e -> if e |> isNull |> not && !!e?value <> !!value then e?value <- !!value)
    /// `prop.ref` callback that sets the value of an input after DOM element is created.
    /// Can be used instead of `prop.defaultValue` and `prop.value` props to override input value.
    member _.valueOrDefault (value: Guid) =
        prop.ref (fun e -> if e |> isNull |> not && !!e?value <> !!value then e?value <- !!value)
    /// `prop.ref` callback that sets the value of an input after DOM element is created.
    /// Can be used instead of `prop.defaultValue` and `prop.value` props to override input value.
    member _.valueOrDefault (value: int) =
        prop.ref (fun e -> if e |> isNull |> not && !!e?value <> !!value then e?value <- !!value)
    /// `prop.ref` callback that sets the value of an input after DOM element is created.
    /// Can be used instead of `prop.defaultValue` and `prop.value` props to override input box value.
    member _.valueOrDefault (value: string) =
        prop.ref (fun e -> if e |> isNull |> not && !!e?value <> !!value then e?value <- !!value)
    /// `prop.ref` callback that sets the value of an input after DOM element is created.
    /// Can be used instead of `prop.defaultValue` and `prop.value` props to override input value.
    member _.valueOrDefault (value: seq<float>) =
        prop.ref (fun e -> if e |> isNull |> not && !!e?value <> !!value then e?value <- !!(ResizeArray value))
    /// `prop.ref` callback that sets the value of an input after DOM element is created.
    /// Can be used instead of `prop.defaultValue` and `prop.value` props to override input value.
    member _.valueOrDefault (value: seq<int>) =
        prop.ref (fun e -> if e |> isNull |> not && !!e?value <> !!value then e?value <- !!(ResizeArray value))
    /// `prop.ref` callback that sets the value of an input after DOM element is created.
    /// Can be used instead of `prop.defaultValue` and `prop.value` props to override input box value.
    member _.valueOrDefault (value: seq<string>) =
        prop.ref (fun e -> if e |> isNull |> not && !!e?value <> !!value then e?value <- !!(ResizeArray value))
*)
    /// The values attribute has different meanings, depending upon the context where itʼs used,
    /// either it defines a sequence of values used over the course of an animation, or itʼs a
    /// list of numbers for a color matrix, which is interpreted differently depending on the
    /// type of color change to be performed.
    member _.values (value: float) = h.MakeAttr("values", Util.asString value)
    /// The values attribute has different meanings, depending upon the context where itʼs used,
    /// either it defines a sequence of values used over the course of an animation, or itʼs a
    /// list of numbers for a color matrix, which is interpreted differently depending on the
    /// type of color change to be performed.
    member _.values (values: seq<float>) = h.MakeAttr("values", (values |> Seq.map Util.asString |> String.concat " "))
    /// The values attribute has different meanings, depending upon the context where itʼs used,
    /// either it defines a sequence of values used over the course of an animation, or itʼs a
    /// list of numbers for a color matrix, which is interpreted differently depending on the
    /// type of color change to be performed.
    member _.values (value: int) = h.MakeAttr("values", Util.asString value)
    /// The values attribute has different meanings, depending upon the context where itʼs used,
    /// either it defines a sequence of values used over the course of an animation, or itʼs a
    /// list of numbers for a color matrix, which is interpreted differently depending on the
    /// type of color change to be performed.
    member _.values (values: seq<int>) = h.MakeAttr("values", (values |> Seq.map Util.asString |> String.concat " "))
    /// The values attribute has different meanings, depending upon the context where itʼs used,
    /// either it defines a sequence of values used over the course of an animation, or itʼs a
    /// list of numbers for a color matrix, which is interpreted differently depending on the
    /// type of color change to be performed.
    member _.values (value: string) = h.MakeAttr("values", value)
    /// The values attribute has different meanings, depending upon the context where itʼs used,
    /// either it defines a sequence of values used over the course of an animation, or itʼs a
    /// list of numbers for a color matrix, which is interpreted differently depending on the
    /// type of color change to be performed.
    member _.values (values: seq<string>) = h.MakeAttr("values", (values |> String.concat " "))

    /// Specifies the width of elements listed here. For all other elements, use the CSS height property.
    ///
    /// HTML: <canvas>, <embed>, <iframe>, <img>, <input>, <object>, <video>
    ///
    /// SVG: <feBlend>, <feColorMatrix>, <feComponentTransfer>, <feComposite>, <feConvolveMatrix>,
    /// <feDiffuseLighting>, <feDisplacementMap>, <feDropShadow>, <feFlood>, <feGaussianBlur>, <feImage>,
    /// <feMerge>, <feMorphology>, <feOffset>, <feSpecularLighting>, <feTile>, <feTurbulence>, <filter>,
    /// <mask>, <pattern>
    member _.width (value: float) = h.MakeAttr("width", Util.asString value)
    /// Specifies the width of elements listed here. For all other elements, use the CSS height property.
    ///
    /// HTML: <canvas>, <embed>, <iframe>, <img>, <input>, <object>, <video>
    ///
    /// SVG: <feBlend>, <feColorMatrix>, <feComponentTransfer>, <feComposite>, <feConvolveMatrix>,
    /// <feDiffuseLighting>, <feDisplacementMap>, <feDropShadow>, <feFlood>, <feGaussianBlur>, <feImage>,
    /// <feMerge>, <feMorphology>, <feOffset>, <feSpecularLighting>, <feTile>, <feTurbulence>, <filter>,
    /// <mask>, <pattern>
    member _.width (value: ICssUnit) = h.MakeAttr("width", Util.asString value)
    /// Specifies the width of elements listed here. For all other elements, use the CSS height property.
    ///
    /// HTML: <canvas>, <embed>, <iframe>, <img>, <input>, <object>, <video>
    ///
    /// SVG: <feBlend>, <feColorMatrix>, <feComponentTransfer>, <feComposite>, <feConvolveMatrix>,
    /// <feDiffuseLighting>, <feDisplacementMap>, <feDropShadow>, <feFlood>, <feGaussianBlur>, <feImage>,
    /// <feMerge>, <feMorphology>, <feOffset>, <feSpecularLighting>, <feTile>, <feTurbulence>, <filter>,
    /// <mask>, <pattern>
    member _.width (value: int) = h.MakeAttr("width", Util.asString value)

    /// SVG attribute to define a x-axis coordinate in the user coordinate system.
    member _.x (value: float) = h.MakeAttr("x", Util.asString value)
    /// SVG attribute to define a x-axis coordinate in the user coordinate system.
    member _.x (value: ICssUnit) = h.MakeAttr("x", Util.asString value)
    /// SVG attribute to define a x-axis coordinate in the user coordinate system.
    member _.x (value: int) = h.MakeAttr("x", Util.asString value)

    /// The x1 attribute is used to specify the first x-coordinate for drawing an SVG element that
    /// requires more than one coordinate. Elements that only need one coordinate use the x attribute instead.
    ///
    /// Two elements are using this attribute: <line>, and <linearGradient>
    member _.x1 (value: float) = h.MakeAttr("x1", Util.asString value)
    /// The x1 attribute is used to specify the first x-coordinate for drawing an SVG element that
    /// requires more than one coordinate. Elements that only need one coordinate use the x attribute instead.
    ///
    /// Two elements are using this attribute: <line>, and <linearGradient>
    member _.x1 (value: ICssUnit) = h.MakeAttr("x1", Util.asString value)
    /// The x1 attribute is used to specify the first x-coordinate for drawing an SVG element that
    /// requires more than one coordinate. Elements that only need one coordinate use the x attribute instead.
    ///
    /// Two elements are using this attribute: <line>, and <linearGradient>
    member _.x1 (value: int) = h.MakeAttr("x1", Util.asString value)

    /// The x2 attribute is used to specify the second x-coordinate for drawing an SVG element that requires
    /// more than one coordinate. Elements that only need one coordinate use the x attribute instead.
    ///
    /// Two elements are using this attribute: <line>, and <linearGradient>
    member _.x2 (value: float) = h.MakeAttr("x2", Util.asString value)
    /// The x2 attribute is used to specify the second x-coordinate for drawing an SVG element that requires
    /// more than one coordinate. Elements that only need one coordinate use the x attribute instead.
    ///
    /// Two elements are using this attribute: <line>, and <linearGradient>
    member _.x2 (value: ICssUnit) = h.MakeAttr("x2", Util.asString value)
    /// The x2 attribute is used to specify the second x-coordinate for drawing an SVG element that requires
    /// more than one coordinate. Elements that only need one coordinate use the x attribute instead.
    ///
    /// Two elements are using this attribute: <line>, and <linearGradient>
    member _.x2 (value: int) = h.MakeAttr("x2", Util.asString value)

    /// Specifies the XML Namespace of the document.
    ///
    /// Default value is "http://www.w3.org/1999/xhtml".
    ///
    /// This is required in documents parsed with XML parsers, and optional in text/html documents.
    member _.xmlns (value: string) = h.MakeAttr("xmlns", value)

    /// SVG attribute to define a y-axis coordinate in the user coordinate system.
    member _.y (value: float) = h.MakeAttr("y", Util.asString value)
    /// SVG attribute to define a y-axis coordinate in the user coordinate system.
    member _.y (value: ICssUnit) = h.MakeAttr("y", Util.asString value)
    /// SVG attribute to define a y-axis coordinate in the user coordinate system.
    member _.y (value: int) = h.MakeAttr("y", Util.asString value)

    /// The y1 attribute is used to specify the first y-coordinate for drawing an SVG element that requires
    /// more than one coordinate. Elements that only need one coordinate use the y attribute instead.
    ///
    /// Two elements are using this attribute: <line>, and <linearGradient>
    member _.y1 (value: float) = h.MakeAttr("y1", Util.asString value)
    /// The y1 attribute is used to specify the first y-coordinate for drawing an SVG element that requires
    /// more than one coordinate. Elements that only need one coordinate use the y attribute instead.
    ///
    /// Two elements are using this attribute: <line>, and <linearGradient>
    member _.y1 (value: ICssUnit) = h.MakeAttr("y1", Util.asString value)
    /// The y1 attribute is used to specify the first y-coordinate for drawing an SVG element that requires
    /// more than one coordinate. Elements that only need one coordinate use the y attribute instead.
    ///
    /// Two elements are using this attribute: <line>, and <linearGradient>
    member _.y1 (value: int) = h.MakeAttr("y1", Util.asString value)

    /// The y2 attribute is used to specify the second y-coordinate for drawing an SVG element that requires
    /// more than one coordinate. Elements that only need one coordinate use the y attribute instead.
    ///
    /// Two elements are using this attribute: <line>, and <linearGradient>
    member _.y2 (value: float) = h.MakeAttr("y2", Util.asString value)
    /// The y2 attribute is used to specify the second y-coordinate for drawing an SVG element that requires
    /// more than one coordinate. Elements that only need one coordinate use the y attribute instead.
    ///
    /// Two elements are using this attribute: <line>, and <linearGradient>
    member _.y2 (value: ICssUnit) = h.MakeAttr("y2", Util.asString value)
    /// The y2 attribute is used to specify the second y-coordinate for drawing an SVG element that requires
    /// more than one coordinate. Elements that only need one coordinate use the y attribute instead.
    ///
    /// Two elements are using this attribute: <line>, and <linearGradient>
    member _.y2 (value: int) = h.MakeAttr("y2", Util.asString value)

    /// Defines the location along the z-axis for a light source in the coordinate system established by the
    /// primitiveUnits attribute on the <filter> element, assuming that, in the initial coordinate system,
    /// the positive z-axis comes out towards the person viewing the content and assuming that one unit along
    /// the z-axis equals one unit in x and y.
    member _.z (value: float) = h.MakeAttr("z", Util.asString value)
    /// Defines the location along the z-axis for a light source in the coordinate system established by the
    /// primitiveUnits attribute on the <filter> element, assuming that, in the initial coordinate system,
    /// the positive z-axis comes out towards the person viewing the content and assuming that one unit along
    /// the z-axis equals one unit in x and y.
    member _.z (value: ICssUnit) = h.MakeAttr("z", Util.asString value)
    /// Defines the location along the z-axis for a light source in the coordinate system established by the
    /// primitiveUnits attribute on the <filter> element, assuming that, in the initial coordinate system,
    /// the positive z-axis comes out towards the person viewing the content and assuming that one unit along
    /// the z-axis equals one unit in x and y.
    member _.z (value: int) = h.MakeAttr("z", Util.asString value)
