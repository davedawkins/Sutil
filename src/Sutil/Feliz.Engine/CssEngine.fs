namespace Feliz.Styles

type IBackgroundRepeat = abstract AsString: string
type IBorderStyle = abstract AsString: string
type ICssUnit = abstract AsString: string
type IGridSpan = abstract AsString: string
type IGridTemplateItem = abstract AsString: string
type ITextDecoration = abstract AsString: string
type ITextDecorationLine = abstract AsString: string
type ITransitionProperty = abstract AsString: string
type ITransformProperty = abstract AsString: string
// type ITextAlignment = interface end
// type IVisibility = interface end
// type IPosition = interface end
// type IAlignContent = interface end
// type IAlignItems = interface end
// type IAlignSelf = interface end
// type IDisplay = interface end
// type IFontStyle = interface end
// type IFontVariant = interface end
// type IFontWeight = interface end
// type IFontStretch = interface end
// type IFontKerning = interface end
// type IOverflow = interface end
// type IWordWrap = interface end
// type IBackgroundClip = interface end

namespace Feliz

open System
open Feliz.Styles

type internal Util =
    static member inline asString(x: string): string = x
    static member inline asString(x: int): string = string x
    static member inline asString(x: int option): string =
        match x with Some x -> Util.asString x | None -> ""
    static member inline asString(x: float): string = string x
    static member inline asString(x: Guid): string = string x
    static member inline asString< ^t when ^t : (member AsString: string)> (x: ^t option): string =
        match x with Some x -> Util.asString x | None -> ""
    static member inline asString< ^t when ^t : (member AsString: string)> (x: ^t): string =
#if FABLE_COMPILER
        unbox x
#else
        (^t : (member AsString: string) (x))
#endif

    static member inline newCssUnit(x: string): ICssUnit =
#if FABLE_COMPILER
        unbox x
#else
        { new ICssUnit with member _.AsString = x }
#endif

    static member inline newBorderStyle(x: string): IBorderStyle =
#if FABLE_COMPILER
        unbox x
#else
        { new IBorderStyle with member _.AsString = x }
#endif

    static member inline newGridSpan(x: string): IGridSpan =
#if FABLE_COMPILER
        unbox x
#else
        { new IGridSpan with member _.AsString = x }
#endif

    static member inline newGridTemplateItem(x: string): IGridTemplateItem =
#if FABLE_COMPILER
        unbox x
#else
        { new IGridTemplateItem with member _.AsString = x }
#endif

    static member inline newTextDecorationLine(x: string): ITextDecorationLine =
#if FABLE_COMPILER
        unbox x
#else
        { new ITextDecorationLine with member _.AsString = x }
#endif

    static member inline newTextDecoration(x: string): ITextDecoration =
#if FABLE_COMPILER
        unbox x
#else
        { new ITextDecoration with member _.AsString = x }
#endif

    static member inline newTransformProperty(x: string): ITransformProperty =
#if FABLE_COMPILER
        unbox x
#else
        { new ITransformProperty with member _.AsString = x }
#endif

    static member inline newTransitionProperty(x: string): ITransitionProperty =
#if FABLE_COMPILER
        unbox x
#else
        { new ITransitionProperty with member _.AsString = x }
#endif

open type Util

type CssHelper<'Style> =
    abstract MakeStyle: key: string * value: string -> 'Style

type CssEngine<'Style>(h: CssHelper<'Style>) =
    new (f: string -> string -> 'Style) =
        CssEngine { new CssHelper<'Style> with
                        member _.MakeStyle(k, v) = f k v }

    /// Define a custom property
    member _.custom(key: string, value: string) = h.MakeStyle(key, value)

    /// Specifies that all the element's properties should be changed to their initial values.
    member _.allInitial = h.MakeStyle("all", "initial")
    /// Specifies that all the element's properties should be changed to their inherited values.
    member _.allInherit = h.MakeStyle("all", "inherit")
    /// Specifies that all the element's properties should be changed to their inherited values if they inherit by default, or to their initial values if not.
    member _.allUnset = h.MakeStyle("all", "unset")
    /// Specifies behavior that depends on the stylesheet origin to which the declaration belongs:
    ///
    /// User-agent origin
    ///     Equivalent to unset.
    /// User origin
    ///     Rolls back the cascade to the user-agent level, so that the specified values are calculated as if no author-level or user-level rules were specified for the element.
    /// Author origin
    ///     Rolls back the cascade to the user level, so that the specified values are calculated as if no author-level rules were specified for the element. For purposes of revert, the Author origin includes the Override and Animation origins.
    member _.allRevert = h.MakeStyle("all", "revert")

    member _.boxShadow(value: string) = h.MakeStyle("box-shadow", value)

    member _.boxShadow(horizontalOffset: int, verticalOffset: int, color: string) =
        h.MakeStyle("box-shadow",
            (asString horizontalOffset) + "px " +
            (asString verticalOffset) + "px " +
            color
        )
    member _.boxShadow(horizontalOffset: int, verticalOffset: int, blur: int, color: string) =
        h.MakeStyle("box-shadow",
            (asString horizontalOffset) + "px " +
            (asString verticalOffset) + "px " +
            (asString blur) + "px " +
            color
        )
    member _.boxShadow(horizontalOffset: int, verticalOffset: int, blur: int, spread: int, color: string) =
        h.MakeStyle("box-shadow",
            (asString horizontalOffset) + "px " +
            (asString verticalOffset) + "px " +
            (asString blur) + "px " +
            (asString spread) + "px " +
            color
        )
    member _.boxShadowNone = h.MakeStyle("box-shadow", "none")
    /// Inherits this property from its parent element.
    member _.boxShadowInheritFromParent = h.MakeStyle("box-shadow", "inherit")

    member _.height(value: int) = h.MakeStyle("height", asString value + "px")
    member _.height(value: ICssUnit) = h.MakeStyle("height", asString value)
    /// Inherits this property from its parent element.
    member _.heightInheritFromParent = h.MakeStyle("height", "inherit")
    /// Sets this property to its default value.
    member _.heightInitial = h.MakeStyle("height", "initial")
    /// The intrinsic preferred height.
    member _.heightMaxContent = h.MakeStyle("height", "max-content")
    /// The intrinsic minimum height.
    member _.heightMinContent = h.MakeStyle("height", "min-content")

    member _.maxHeight(value: int) = h.MakeStyle("max-height", asString value + "px")
    member _.maxHeight(value: ICssUnit) = h.MakeStyle("max-height", asString value)
    /// Inherits this property from its parent element.
    member _.maxHeightInheritFromParent = h.MakeStyle("max-height", "inherit")
    /// Sets this property to its default value.
    member _.maxHeightInitial = h.MakeStyle("max-height", "initial")
    /// The intrinsic preferred height.
    member _.maxHeightMaxContent = h.MakeStyle("height", "max-content")
    /// The intrinsic minimum height.
    member _.maxHeightMinContent = h.MakeStyle("height", "min-content")

    member _.minHeight(value: int) = h.MakeStyle("min-height", asString value + "px")
    member _.minHeight(value: ICssUnit) = h.MakeStyle("min-height", asString value)
    /// Inherits this property from its parent element.
    member _.minHeightInheritFromParent = h.MakeStyle("min-height", "inherit")
    /// Sets this property to its default value.
    member _.minHeightInitial = h.MakeStyle("min-height", "initial")
    /// The intrinsic preferred height.
    member _.minHeightMaxContent = h.MakeStyle("height", "max-content")
    /// The intrinsic minimum height.
    member _.minHeightMinContent = h.MakeStyle("height", "min-content")

    /// The browser determines the justification algorithm
    member _.textJustifyAuto = h.MakeStyle("text-justify", "auto")
    /// Increases/Decreases the space between words
    member _.textJustifyInterWord = h.MakeStyle("text-justify", "inter-word")
    /// Increases/Decreases the space between characters
    member _.textJustifyInterCharacter = h.MakeStyle("text-justify", "inter-character")
    /// Disables justification methods
    member _.textJustifyNone = h.MakeStyle("text-justify", "none")
    member _.textJustifyInitial = h.MakeStyle("text-justify", "initial")
    /// Inherits this property from its parent element.
    member _.textJustifyInheritFromParent = h.MakeStyle("text-justify", "inherit")

    /// Sequences of whitespace will collapse into a single whitespace. Text will wrap when necessary. This is default.
    member _. whiteSpaceNormal = h.MakeStyle("white-space", "normal")
    /// Sequences of whitespace will collapse into a single whitespace. Text will never wrap to the next line.
    /// The text continues on the same line until a `<br> ` tag is encountered.
    member _. whiteSpaceNowrap = h.MakeStyle("white-space", "nowrap")
    /// Whitespace is preserved by the browser. Text will only wrap on line breaks. Acts like the <pre> tag in HTML.
    member _. whiteSpacePre = h.MakeStyle("white-space", "pre")
    /// Sequences of whitespace will collapse into a single whitespace. Text will wrap when necessary, and on line breaks
    member _. whiteSpacePreLine = h.MakeStyle("white-space", "pre-line")
    /// Whitespace is preserved by the browser. Text will wrap when necessary, and on line breaks
    member _. whiteSpacePreWrap = h.MakeStyle("white-space", "pre-wrap")
    /// Sets this property to its default value.
    member _. whiteSpaceInitial = h.MakeStyle("white-space", "initial")
    /// Inherits this property from its parent element.
    member _. whiteSpaceInheritFromParent = h.MakeStyle("white-space", "inherit")

    /// Default value. Uses default line break rules.
    member _.wordbreakNormal = h.MakeStyle("word-break", "normal")
    /// To prevent overflow, word may be broken at any character
    member _.wordbreakBreakAll = h.MakeStyle("word-break", "break-all")
    /// Word breaks should not be used for Chinese/Japanese/Korean (CJK) text. Non-CJK text behavior is the same as value "normal"
    member _.wordbreakKeepAll = h.MakeStyle("word-break", "keep-all")
    /// To prevent overflow, word may be broken at arbitrary points.
    member _.wordbreakBreakWord = h.MakeStyle("word-break", "break-word")
    /// Sets this property to its default value.
    member _.wordbreakInitial = h.MakeStyle("word-break", "initial")
    /// Inherits this property from its parent element.
    member _.wordbreakInheritFromParent = h.MakeStyle("word-break", "inherit")

    /// Allows a straight jump "scroll effect" between elements within the scrolling box. This is default
    member _.scrollBehaviorAuto = h.MakeStyle("scroll-behavior", "auto")
    /// Allows a smooth animated "scroll effect" between elements within the scrolling box.
    member _.scrollBehaviorSmooth = h.MakeStyle("scroll-behavior", "smooth")
    /// Sets this property to its default value.
    member _.scrollBehaviorInitial = h.MakeStyle("scroll-behavior", "initial")
    /// Inherits this property from its parent element.
    member _.scrollBehaviorInheritFromParent = h.MakeStyle("scroll-behavior", "inherit")

    /// The content is not clipped, and it may be rendered outside the left and right edges. This is default.
    member _.overflowVisible = h.MakeStyle("overflow", "visibile")
    /// The content is clipped - and no scrolling mechanism is provided.
    member _.overflowHidden = h.MakeStyle("overflow", "hidden")
    /// The content is clipped and a scrolling mechanism is provided.
    member _.overflowScroll = h.MakeStyle("overflow", "scroll")
    /// Should cause a scrolling mechanism to be provided for overflowing boxes
    member _.overflowAuto = h.MakeStyle("overflow", "auto")
    /// Sets this property to its default value.
    member _.overflowInitial = h.MakeStyle("overflow", "initial")
    /// Inherits this property from its parent element.
    member _.overflowInheritFromParent = h.MakeStyle("overflow", "inherit")

    /// The content is not clipped, and it may be rendered outside the left and right edges. This is default.
    member _.overflowXVisible = h.MakeStyle("overflow-x", "visibile")
    /// The content is clipped - and no scrolling mechanism is provided.
    member _.overflowXHidden = h.MakeStyle("overflow-x", "hidden")
    /// The content is clipped and a scrolling mechanism is provided.
    member _.overflowXScroll = h.MakeStyle("overflow-x", "scroll")
    /// Should cause a scrolling mechanism to be provided for overflowing boxes
    member _.overflowXAuto = h.MakeStyle("overflow-x", "auto")
    /// Sets this property to its default value.
    member _.overflowXInitial = h.MakeStyle("overflow-x", "initial")
    /// Inherits this property from its parent element.
    member _.overflowXInheritFromParent = h.MakeStyle("overflow-x", "inherit")

    /// The content is not clipped, and it may be rendered outside the left and right edges. This is default.
    member _.overflowYVisible = h.MakeStyle("overflow-y", "visibile")
    /// The content is clipped - and no scrolling mechanism is provided.
    member _.overflowYHidden = h.MakeStyle("overflow-y", "hidden")
    /// The content is clipped and a scrolling mechanism is provided.
    member _.overflowYScroll = h.MakeStyle("overflow-y", "scroll")
    /// Should cause a scrolling mechanism to be provided for overflowing boxes
    member _.overflowYAuto = h.MakeStyle("overflow-y", "auto")
    /// Sets this property to its default value.
    member _.overflowYInitial = h.MakeStyle("overflow-y", "initial")
    /// Inherits this property from its parent element.
    member _.overflowYInheritFromParent = h.MakeStyle("overflow-y", "inherit")

    /// The element is hidden (but still takes up space).
    member _.visibilityHidden = h.MakeStyle("visibility", "hidden")
    /// Default value. The element is visible.
    member _.visibilityVisible = h.MakeStyle("visibility", "visible")
    /// Only for table rows (`<tr> `), row groups (`<tbody> `), columns (`<col> `), column groups
    /// (`<colgroup> `). This value removes a row or column, but it does not affect the table layout.
    /// The space taken up by the row or column will be available for other content.
    ///
    /// If collapse is used on other elements, it renders as "hidden"
    member _.visibilityCollapse = h.MakeStyle("visibility", "collapse")
    /// Sets this property to its default value.
    member _.visibilityInitial = h.MakeStyle("visibility", "initial")
    /// Inherits this property from its parent element.
    member _.visibilityInheritFromParent = h.MakeStyle("visibility", "inherit")

    /// Default value. The length is equal to the length of the flexible item. If the item has
    /// no length specified, the length will be according to its content.
    member _.flexBasisAuto = h.MakeStyle("flex-basis", "auto")
    /// Sets this property to its default value.
    member _.flexBasisInitial = h.MakeStyle("flex-basis", "initial")
    /// Inherits this property from its parent element.
    member _.flexBasisInheritFromParent = h.MakeStyle("flex-basis", "inherit")

    /// Default value. The flexible items are displayed horizontally, as a row
    member _.flexDirectionRow = h.MakeStyle("flex-direction", "row")
    /// Same as row, but in reverse order.
    member _.flexDirectionRowReverse = h.MakeStyle("flex-direction", "row-reverse")
    /// The flexible items are displayed vertically, as a column
    member _.flexDirectionColumn = h.MakeStyle("flex-direction", "column")
    /// Same as column, but in reverse order
    member _.flexDirectionColumnReverse = h.MakeStyle("flex-direction", "column-reverse")
    /// Sets this property to its default value.
    member _.flexDirectionInitial = h.MakeStyle("flex-basis", "initial")
    /// Inherits this property from its parent element.
    member _.flexDirectionInheritFromParent = h.MakeStyle("flex-basis", "inherit")

    /// Default value. Specifies that the flexible items will not wrap.
    member _.flexWrapNowrap = h.MakeStyle("flex-wrap", "nowrap")
    /// Specifies that the flexible items will wrap if necessary
    member _.flexWrapWrap = h.MakeStyle("flex-wrap", "wrap")
    /// Specifies that the flexible items will wrap, if necessary, in reverse order
    member _.flexWrapWrapReverse = h.MakeStyle("flex-wrap", "wrap-reverse")
    /// Sets this property to its default value.
    member _.flexWrapInitial = h.MakeStyle("flex-wrap", "initial")
    /// Inherits this property from its parent element.
    member _.flexWrapInheritFromParent = h.MakeStyle("flex-wrap", "inherit")

    /// The element must float on the left side of its containing block.
    member _.floatLeft = h.MakeStyle("float", "left")
    /// The element must float on the right side of its containing block.
    member _.floatRight = h.MakeStyle("float", "right")
    /// The element must not float.
    member _.floatNone = h.MakeStyle("float", "none")

    /// The font display strategy is defined by the user agent.
    ///
    /// Default value
    member _.fontDisplayAuto = h.MakeStyle("font-display", "auto")
    /// Gives the font face a short block period and an infinite swap period.
    member _.fontDisplayBlock = h.MakeStyle("font-display", "block")
    /// Gives the font face an extremely small block period and an infinite swap period.
    member _.fontDisplaySwap = h.MakeStyle("font-display", "swap")
    /// Gives the font face an extremely small block period and a short swap period.
    member _.fontDisplayFallback = h.MakeStyle("font-display", "fallback")
    /// Gives the font face an extremely small block period and no swap period.
    member _.fontDisplayOptional = h.MakeStyle("font-display", "optional")

    /// Default. The browser determines whether font kerning should be applied or not
    member _.fontKerningAuto = h.MakeStyle("font-kerning", "auto")
    /// Specifies that font kerning is applied
    member _.fontKerningNormal = h.MakeStyle("font-kerning", "normal")
    /// Specifies that font kerning is not applied
    member _.fontKerningNone = h.MakeStyle("font-kerning", "none")

    /// Defines from thin to thick characters. 400 is the same as normal, and 700 is the same as bold.
    /// Possible values are [100, 200, 300, 400, 500, 600, 700, 800, 900]
    member _.fontWeight(weight: int) = h.MakeStyle("font-weight", asString weight)
    /// Defines normal characters. This is default.
    member _.fontWeightNormal = h.MakeStyle("font-weight", "normal")
    /// Defines thick characters.
    member _.fontWeightBold = h.MakeStyle("font-weight", "bold")
    /// Defines thicker characters
    member _.fontWeightBolder = h.MakeStyle("font-weight", "bolder")
    /// Defines lighter characters.
    member _.fontWeightLighter = h.MakeStyle("font-weight", "lighter")
    /// Sets this property to its default value.
    member _.fontWeightInitial = h.MakeStyle("font-weight", "initial")
    /// Inherits this property from its parent element.
    member _.fontWeightInheritFromParent = h.MakeStyle("font-weight", "inherit")

    /// The browser displays a normal font style. This is defaut.
    member _.fontStyleNormal = h.MakeStyle("font-style", "normal")
    /// The browser displays an italic font style.
    member _.fontStyleItalic = h.MakeStyle("font-style", "italic")
    /// The browser displays an oblique font style.
    member _.fontStyleOblique = h.MakeStyle("font-style", "oblique")
    /// Sets this property to its default value.
    member _.fontStyleInitial = h.MakeStyle("font-style", "initial")
    /// Inherits this property from its parent element.
    member _.fontStyleInheritFromParent = h.MakeStyle("font-style", "inherit")

    /// The browser displays a normal font. This is default
    member _.fontVariantNormal = h.MakeStyle("font-variant", "normal")
    /// The browser displays a small-caps font
    member _.fontVariantSmallCaps = h.MakeStyle("font-variant", "small-caps")
    /// Sets this property to its default value.
    member _.fontVariantInitial = h.MakeStyle("font-variant", "initial")
    /// Inherits this property from its parent element.
    member _.fontVariantInheritFromParent = h.MakeStyle("font-variant", "inherit")

    /// Break words only at allowed break points
    member _.wordWrapNormal = h.MakeStyle("word-wrap", "normal")
    /// Allows unbreakable words to be broken
    member _.wordWrapBreakWord = h.MakeStyle("word-wrap", "break-word")
    /// Sets this property to its default value.
    member _.wordWrapInitial = h.MakeStyle("word-wrap", "initial")
    /// Inherits this property from its parent element.
    member _.wordWrapInheritFromParent = h.MakeStyle("word-wrap", "inherit")

    /// Default. The element inherits its parent container's align-items property, or "stretch" if it has no parent container.
    member _.alignSelfAuto = h.MakeStyle("align-self", "auto")
    /// The element is positioned to fit the container
    member _.alignSelfStretch = h.MakeStyle("align-self", "stretch")
    /// The element is positioned at the center of the container
    member _.alignSelfCenter = h.MakeStyle("align-self", "center")
    /// The element is positioned at the beginning of the container
    member _.alignSelfFlexStart = h.MakeStyle("align-self", "flex-start")
    /// The element is positioned at the end of the container
    member _.alignSelfFlexEnd = h.MakeStyle("align-self", "flex-end")
    /// The element is positioned at the baseline of the container
    member _.alignSelfBaseline = h.MakeStyle("align-self", "baseline")
    /// Sets this property to its default value
    member _.alignSelfInitial = h.MakeStyle("align-self", "initial")
    /// Inherits this property from its parent element
    member _.alignSelfInheritFromParent = h.MakeStyle("align-self", "inherit")

    /// Default. Items are stretched to fit the container
    member _.alignItemsStretch = h.MakeStyle("align-items", "stretch")
    /// Items are positioned at the center of the container
    member _.alignItemsCenter = h.MakeStyle("align-items", "center")
    /// Items are positioned at the beginning of the container
    member _.alignItemsFlexStart = h.MakeStyle("align-items", "flex-start")
    /// Items are positioned at the end of the container
    member _.alignItemsFlexEnd = h.MakeStyle("align-items", "flex-end")
    /// Items are positioned at the baseline of the container
    member _.alignItemsBaseline = h.MakeStyle("align-items", "baseline")
    /// Sets this property to its default value
    member _.alignItemsInitial = h.MakeStyle("align-items", "initial")
    /// Inherits this property from its parent element
    member _.alignItemsInheritFromParent = h.MakeStyle("align-items", "inherit")

    /// Default value. Lines stretch to take up the remaining space.
    member _.alignContentStretch = h.MakeStyle("align-content", "stretch")
    /// Lines are packed toward the center of the flex container.
    member _.alignContentCenter = h.MakeStyle("align-content", "center")
    /// Lines are packed toward the start of the flex container.
    member _.alignContentFlexStart = h.MakeStyle("align-content", "flex-start")
    /// Lines are packed toward the end of the flex container.
    member _.alignContentFlexEnd = h.MakeStyle("align-content", "flex-end")
    /// Lines are evenly distributed in the flex container.
    member _.alignContentSpaceBetween = h.MakeStyle("align-content", "space-between")
    /// Lines are evenly distributed in the flex container, with half-size spaces on either end.
    member _.alignContentSpaceAround = h.MakeStyle("align-content", "space-around")
    member _.alignContentInitial = h.MakeStyle("align-content", "initial")
    member _.alignContentInheritFromParent = h.MakeStyle("align-content", "inherit")

    /// Default value. Items are positioned at the beginning of the container.
    member _.justifyContentFlexStart = h.MakeStyle("justify-content", "flex-start")
    /// Items are positioned at the end of the container.
    member _.justifyContentFlexEnd = h.MakeStyle("justify-content", "flex-end")
    /// Items are positioned at the center of the container
    member _.justifyContentCenter = h.MakeStyle("justify-content", "center")
    /// Items are positioned with space between the lines
    member _.justifyContentSpaceBetween = h.MakeStyle("justify-content", "space-between")
    /// Items are positioned with space before, between, and after the lines.
    member _.justifyContentSpaceAround = h.MakeStyle("justify-content", "space-around")
    /// Sets this property to its default value.
    member _.justifyContentInitial = h.MakeStyle("justify-content", "initial")
    /// Inherits this property from its parent element.
    member _.justifyContentInheritFromParent = h.MakeStyle("justify-content", "inherit")

    member _.outlineWidth(width: int) = h.MakeStyle("outline-width", asString width + "px")
    member _.outlineWidth(width: ICssUnit) = h.MakeStyle("outline-width", asString width)
    /// Specifies a medium outline. This is default.
    member _.outlineWidthMedium = h.MakeStyle("outline-width", "medium")
    /// Specifies a thin outline.
    member _.outlineWidthThin = h.MakeStyle("outline-width", "thin")
    /// Specifies a thick outline.
    member _.outlineWidthThick = h.MakeStyle("outline-width", "thick")
    /// Sets this property to its default value
    member _.outlineWidthInitial = h.MakeStyle("outline-width", "initial")
    /// Inherits this property from its parent element
    member _.outlineWidthInheritFromParent = h.MakeStyle("outline-width", "inherit")

    /// Default value. The marker is a filled circle
    member _.listStyleTypeDisc = h.MakeStyle("list-style-type", "disc")
    /// The marker is traditional Armenian numbering
    member _.listStyleTypeArmenian = h.MakeStyle("list-style-type", "armenian")
    /// The marker is a circle
    member _.listStyleTypeCircle = h.MakeStyle("list-style-type", "circle")
    /// The marker is plain ideographic numbers
    member _.listStyleTypeCjkIdeographic = h.MakeStyle("list-style-type", "cjk-ideographic")
    /// The marker is a number
    member _.listStyleTypeDecimal = h.MakeStyle("list-style-type", "decimal")
    /// The marker is a number with leading zeros (01, 02, 03, etc.)
    member _.listStyleTypeDecimalLeadingZero = h.MakeStyle("list-style-type", "decimal-leading-zero")
    /// The marker is traditional Georgian numbering
    member _.listStyleTypeGeorgian = h.MakeStyle("list-style-type", "georgian")
    /// The marker is traditional Hebrew numbering
    member _.listStyleTypeHebrew = h.MakeStyle("list-style-type", "hebrew")
    /// The marker is traditional Hiragana numbering
    member _.listStyleTypeHiragana = h.MakeStyle("list-style-type", "hiragana")
    /// The marker is traditional Hiragana iroha numbering
    member _.listStyleTypeHiraganaIroha = h.MakeStyle("list-style-type", "hiragana-iroha")
    /// The marker is traditional Katakana numbering
    member _.listStyleTypeKatakana = h.MakeStyle("list-style-type", "katakana")
    /// The marker is traditional Katakana iroha numbering
    member _.listStyleTypeKatakanaIroha = h.MakeStyle("list-style-type", "katakana-iroha")
    /// The marker is lower-alpha (a, b, c, d, e, etc.)
    member _.listStyleTypeLowerAlpha = h.MakeStyle("list-style-type", "lower-alpha")
    /// The marker is lower-greek
    member _.listStyleTypeLowerGreek = h.MakeStyle("list-style-type", "lower-greek")
    /// The marker is lower-latin (a, b, c, d, e, etc.)
    member _.listStyleTypeLowerLatin = h.MakeStyle("list-style-type", "lower-latin")
    /// The marker is lower-roman (i, ii, iii, iv, v, etc.)
    member _.listStyleTypeLowerRoman = h.MakeStyle("list-style-type", "lower-roman")
    /// No marker is shown
    member _.listStyleTypeNone = h.MakeStyle("list-style-type", "none")
    /// The marker is a square
    member _.listStyleTypeSquare = h.MakeStyle("list-style-type", "square")
    /// The marker is upper-alpha (A, B, C, D, E, etc.)
    member _.listStyleTypeUpperAlpha = h.MakeStyle("list-style-type", "upper-alpha")
    /// The marker is upper-greek
    member _.listStyleTypeUpperGreek = h.MakeStyle("list-style-type", "upper-greek")
    /// The marker is upper-latin (A, B, C, D, E, etc.)
    member _.listStyleTypeUpperLatin = h.MakeStyle("list-style-type", "upper-latin")
    /// The marker is upper-roman (I, II, III, IV, V, etc.)
    member _.listStyleTypeUpperRoman = h.MakeStyle("list-style-type", "upper-roman")
    /// Sets this property to its default value.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_text-align&preval=initial
    member _.listStyleTypeInitial = h.MakeStyle("list-style-type", "initial")
    /// Inherits this property from its parent element.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_text-align&preval=initial
    member _.listStyleTypeInheritFromParent = h.MakeStyle("list-style-type", "inherit")

    member _.propertyNone = h.MakeStyle("list-style-image", "none")
    /// The path to the image to be used as a list-item marker
    member _.propertyUrl (path: string) = h.MakeStyle("list-style-image", "url('" + path + "')")
    /// Sets this property to its default value.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_text-align&preval=initial
    member _.propertyInitial = h.MakeStyle("list-style-image", "initial")
    /// Inherits this property from its parent element.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_text-align&preval=initial
    member _.propertyInheritFromParent = h.MakeStyle("list-style-image", "inherit")

    /// The bullet points will be inside the list item
    member _.listStylePositionInside = h.MakeStyle("list-style-position", "inside")
    /// The bullet points will be outside the list item. This is default
    member _.listStylePositionOutside = h.MakeStyle("list-style-position", "outside")
    /// Sets this property to its default value.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_text-align&preval=initial
    member _.listStylePositionInitial = h.MakeStyle("list-style-position", "initial")
    /// Inherits this property from its parent element.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_text-align&preval=initial
    member _.listStylePositionInheritFromParent = h.MakeStyle("list-style-position", "inherit")

    member _.textDecorationLine(line: ITextDecorationLine) = h.MakeStyle("text-decoration-line", asString line)
    member _.textDecorationLineNone = h.MakeStyle("text-decoration-line", "none")
    member _.textDecorationLineUnderline = h.MakeStyle("text-decoration-line", "underline")
    member _.textDecorationLineOverline = h.MakeStyle("text-decoration-line", "overline")
    member _.textDecorationLineLineThrough = h.MakeStyle("text-decoration-line", "line-through")
    member _.textDecorationLineInitial = h.MakeStyle("text-decoration-line", "initial")
    /// Inherits this property from its parent element.
    member _.textDecorationLineInheritFromParent = h.MakeStyle("text-decoration-line", "inherit")

    member _.textDecoration(line: ITextDecorationLine) = h.MakeStyle("text-decoration", asString line)
    member _.textDecoration(bottom: ITextDecorationLine, top: ITextDecorationLine) =
        h.MakeStyle("text-decoration", (asString bottom) + " " + (asString top))
    member _.textDecoration(bottom: ITextDecorationLine, top: ITextDecorationLine, style: ITextDecoration) =
        h.MakeStyle("text-decoration", (asString bottom) + " " + (asString top) + " " + (asString style))
    member _.textDecoration(bottom: ITextDecorationLine, top: ITextDecorationLine, style: ITextDecoration, color: string) =
        h.MakeStyle("text-decoration", (asString bottom) + " " + (asString top) + " " + (asString style) + " " + color)
    member _.textDecorationNone = h.MakeStyle("text-decoration", "none")
    member _.textDecorationUnderline = h.MakeStyle("text-decoration", "underline")
    member _.textDecorationOverline = h.MakeStyle("text-decoration", "overline")
    member _.textDecorationLineThrough = h.MakeStyle("text-decoration", "line-through")
    member _.textDecorationInitial = h.MakeStyle("text-decoration", "initial")
    /// Inherits this property from its parent element.
    member _.textDecorationInheritFromParent = h.MakeStyle("text-decoration", "inherit")

    /// Specifies that child elements will NOT preserve its 3D position. This is default.
    member _.transformStyleFlat = h.MakeStyle("transform-style", "flat")
    /// Specifies that child elements will preserve its 3D position
    member _.transformStylePreserve3D = h.MakeStyle("transform-style", "preserve-3d")
    member _.transformStyleInitial = h.MakeStyle("transform-style", "initial")
    /// Inherits this property from its parent element.
    member _.transformStyleInheritFromParent = h.MakeStyle("transform-style", "inherit")

    /// No capitalization. The text renders as it is. This is default.
    member _.textTransformNone = h.MakeStyle("text-transform", "none")
    /// Transforms the first character of each word to uppercase.
    member _.textTransformCapitalize = h.MakeStyle("text-transform", "capitalize")
    /// Transforms all characters to uppercase.
    member _.textTransformUppercase = h.MakeStyle("text-transform", "uppercase")
    /// Transforms all characters to lowercase.
    member _.textTransformLowercase = h.MakeStyle("text-transform", "lowercase")
    member _.textTransformInitial = h.MakeStyle("text-transform", "initial")
    /// Inherits this property from its parent element.
    member _.textTransformInheritFromParent = h.MakeStyle("text-transform", "inherit")

    /// Default value. The text is clipped and not accessible.
    member _.textOverflowClip = h.MakeStyle("text-overflow", "clip")
    /// Render an ellipsis ("...") to represent the clipped text.
    member _.textOverflowEllipsis = h.MakeStyle("text-overflow", "ellipsis")
    /// Render the given asString to represent the clipped text.
    member _.textOverflowInitial = h.MakeStyle("text-overflow", "initial")
    /// Inherits this property from its parent element.
    member _.textOverflowInheritFromParent = h.MakeStyle("text-overflow", "inherit")

    /// Default value. Specifies no effects.
    member _.filterNone = h.MakeStyle("filter", "none")
    /// Applies a blur effect to the elemeen. A larger value will create more blur.
    ///
    /// This overload takes an integer that represents a percentage from 0 to 100.
    member _.filterBlur(value: int) = h.MakeStyle("filter", "blur(" + (asString value) + "%)")
    /// Applies a blur effect to the elemeen. A larger value will create more blur.
    ///
    /// This overload takes a floating number that goes from 0 to 1,
    member _.filterBlur(value: double) = h.MakeStyle("filter", "blur(" + (asString value) + ")")
    /// Adjusts the brightness of the elemeen
    ///
    /// This overload takes an integer that represents a percentage from 0 to 100.
    ///
    /// Values over 100% will provide brighter results.
    member _.filterBrightness(value: int) = h.MakeStyle("filter", "brightness(" + (asString value) + "%)")
    /// Adjusts the brightness of the elemeen. A larger value will create more blur.
    ///
    /// This overload takes a floating number that goes from 0 to 1,
    member _.filterBrightness(value: double) = h.MakeStyle("filter", "brightness(" + (asString value) + ")")
    /// Adjusts the contrast of the element.
    ///
    /// This overload takes an integer that represents a percentage from 0 to 100.
    member _.filterContrast(value: int) = h.MakeStyle("filter", "contrast(" + (asString value) + "%)")
    /// Adjusts the contrast of the element. A larger value will create more contrast.
    ///
    /// This overload takes a floating number that goes from 0 to 1
    member _.filterContrast(value: double) = h.MakeStyle("filter", "contrast(" + (asString value) + ")")
    /// Applies a drop shadow effect.
    member _.filterDropShadow(horizontalOffset: int, verticalOffset: int, blur: int, spread: int,  color: string) = h.MakeStyle("filter", "drop-shadow(" + (asString horizontalOffset) + "px " + (asString verticalOffset) + "px " + (asString blur) + "px " + (asString spread) + "px " + color + ")")
    /// Applies a drop shadow effect.
    member _.filterDropShadow(horizontalOffset: int, verticalOffset: int, blur: int, color: string) = h.MakeStyle("filter", "drop-shadow(" + (asString horizontalOffset) + "px " + (asString verticalOffset) + "px " + (asString blur) + "px " + color + ")")
    /// Applies a drop shadow effect.
    member _.filterDropShadow(horizontalOffset: int, verticalOffset: int, color: string) = h.MakeStyle("filter", "drop-shadow(" + (asString horizontalOffset) + "px " + (asString verticalOffset) + "px " + color + ")")
    /// Converts the image to grayscale
    ///
    /// This overload takes an integer that represents a percentage from 0 to 100.
    member _.filterGrayscale(value: int) = h.MakeStyle("filter", "grayscale(" + (asString value) + "%)")
    /// Converts the image to grayscale
    ///
    /// This overload takes a floating number that goes from 0 to 1
    member _.filterGrayscale(value: double) = h.MakeStyle("filter", "grayscale(" + (asString value) + ")")
    /// Applies a hue rotation on the image. The value defines the number of degrees around the color circle the image
    /// samples will be adjusted. 0deg is default, and represents the original image.
    ///
    /// **Note**: Maximum value is 360
    member _.filterHueRotate(degrees: int) = h.MakeStyle("filter", "hue-rotate(" + (asString degrees) + "deg)")
    /// Inverts the element.
    ///
    /// This overload takes an integer that represents a percentage from 0 to 100.
    member _.filterInvert(value: int) = h.MakeStyle("filter", "invert(" + (asString value) + "%)")
    /// Inverts the element.
    ///
    /// This overload takes a floating number that goes from 0 to 1
    member _.filterInvert(value: double) = h.MakeStyle("filter", "invert(" + (asString value) + ")")
    /// Sets the opacity of the element.
    ///
    /// This overload takes an integer that represents a percentage from 0 to 100.
    member _.filterOpacity(value: int) = h.MakeStyle("filter", "opacity(" + (asString value) + "%)")
    /// Sets the opacity of the element.
    ///
    /// This overload takes a floating number that goes from 0 to 1
    member _.filterOpacity(value: double) = h.MakeStyle("filter", "opacity(" + (asString value) + ")")
    /// Sets the saturation of the element.
    ///
    /// This overload takes an integer that represents a percentage from 0 to 100.
    member _.filterSaturate(value: int) = h.MakeStyle("filter", "saturate(" + (asString value) + "%)")
    /// Sets the saturation of the element.
    ///
    /// This overload takes a floating number that goes from 0 to 1
    member _.filterSaturate(value: double) = h.MakeStyle("filter", "saturate(" + (asString value) + ")")
    /// Applies Sepia filter to the element.
    ///
    /// This overload takes an integer that represents a percentage from 0 to 100.
    member _.filterSepia(value: int) = h.MakeStyle("filter", "sepia(" + (asString value) + "%)")
    /// Applies Sepia filter to the element.
    ///
    /// This overload takes a floating number that goes from 0 to 1
    member _.filterSepia(value: double) = h.MakeStyle("filter", "sepia(" + (asString value) + ")")
    /// The url() function takes the location of an XML file that specifies an SVG filter, and may include an anchor to a specific filter element.
    ///
    /// Example: `filter: url(svg-url#element-id)`
    member _.filterUrl(value: string) = h.MakeStyle("filter", "url(" + value + ")")
    /// Sets this property to its default value.
    member _.filterInitial = h.MakeStyle("filter", "initial")
    /// Inherits this property from its parent element.
    member _.filterInheritFromParent = h.MakeStyle("filter", "inherit")

    /// Sets whether table borders should collapse into a single border or be separated as in standard HTML.
    /// Borders are separated; each cell will display its own borders. This is default.
    member _.borderCollapseSeparate = h.MakeStyle("border-collapse", "separate")
    /// Borders are collapsed into a single border when possible (border-spacing and empty-cells properties have no effect)
    member _.borderCollapseCollapse = h.MakeStyle("border-collapse", "collapse")
    /// Sets this property to its default value
    member _.borderCollapseInitial = h.MakeStyle("border-collapse", "initial")
    /// Inherits this property from its parent element.
    member _.borderCollapseInheritFromParent = h.MakeStyle("border-collapse", "inherit")

    /// Sets the distance between the borders of adjacent <table> cells. Applies only when border-collapse is separate.
    member _.borderSpacing(horizontal: ICssUnit, ?vertical: ICssUnit) =
        h.MakeStyle("border-spacing", asString horizontal + " " + asString vertical)
    /// Sets this property to its default value
    member _.borderSpacingInitial = h.MakeStyle("border-spacing", "initial")
    /// Inherits this property from its parent element.
    member _.borderSpacingInheritFromParent = h.MakeStyle("border-spacing", "inherit")

    /// Sets the size of the element's background image.
    ///
    /// The image can be left to its natural size, stretched, or constrained to fit the available space.
    member _.backgroundSize(value: string) = h.MakeStyle("background-size", asString value)
    /// Sets the size of the element's background image.
    ///
    /// The image can be left to its natural size, stretched, or constrained to fit the available space.
    member _.backgroundSize(value: ICssUnit) = h.MakeStyle("background-size", asString value)
    /// Sets the size of the element's background image.
    ///
    /// The image can be left to its natural size, stretched, or constrained to fit the available space.
    member _.backgroundSize(width: ICssUnit, height: ICssUnit) =
        h.MakeStyle("background-size",
            asString width
            + " " +
            asString height
        )
    /// Default value. The background image is displayed in its original size
    ///
    /// See [example here](https://www.w3schools.com/cssref/playit.asp?filename=playcss_background-size&preval=auto)
    member _.backgroundSizeAuto = h.MakeStyle("background-size", "auto")
    /// Resize the background image to cover the entire container, even if it has to stretch the image or cut a little bit off one of the edges.
    ///
    /// See [example here](https://www.w3schools.com/cssref/playit.asp?filename=playcss_background-size&preval=cover)
    member _.backgroundSizeCover = h.MakeStyle("background-size", "cover")
    /// Resize the background image to make sure the image is fully visible
    ///
    /// See [example here](https://www.w3schools.com/cssref/playit.asp?filename=playcss_background-size&preval=contain)
    member _.backgroundSizeContain = h.MakeStyle("background-size", "contain")
    /// Sets this property to its default value.
    member _.backgroundSizeInitial = h.MakeStyle("background-size", "initial")
    /// Inherits this property from its parent element.
    member _.backgroundSizeInheritFromParent = h.MakeStyle("background-size", "inherit")

    /// Default value. The line will display as a single line.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_text-decoration-style&preval=solid
    member _.textDecorationStyleSolid = h.MakeStyle("text-decoration-style", "solid")
    /// The line will display as a double line.
    ///
    /// https://www.w3schools.com/cssref/playit.asp?filename=playcss_text-decoration-style&preval=double
    member _.textDecorationStyleDouble = h.MakeStyle("text-decoration-style", "double")
    /// The line will display as a dotted line.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_text-decoration-style&preval=dotted
    member _.textDecorationStyleDotted = h.MakeStyle("text-decoration-style", "dotted")
    /// The line will display as a dashed line.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_text-decoration-style&preval=dashed
    member _.textDecorationStyleDashed = h.MakeStyle("text-decoration-style", "dashed")
    /// The line will display as a wavy line.
    ///
    /// https://www.w3schools.com/cssref/playit.asp?filename=playcss_text-decoration-style&preval=wavy
    member _.textDecorationStyleWavy = h.MakeStyle("text-decoration-style", "wavy")
    /// Sets this property to its default value.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_text-decoration-style&preval=initial
    member _.textDecorationStyleInitial = h.MakeStyle("text-decoration-style", "initial")
    /// Inherits this property from its parent element.
    member _.textDecorationStyleInheritFromParent = h.MakeStyle("text-decoration-style", "inherit")

    /// Makes the text as narrow as it gets.
    member _.fontStretchUltraCondensed = h.MakeStyle("font-stretch", "ultra-condensed")
    /// Makes the text narrower than condensed, but not as narrow as ultra-condensed
    member _.fontStretchExtraCondensed = h.MakeStyle("font-stretch", "extra-condensed")
    /// Makes the text narrower than semi-condensed, but not as narrow as extra-condensed.
    member _.fontStretchCondensed = h.MakeStyle("font-stretch", "condensed")
    /// Makes the text narrower than normal, but not as narrow as condensed.
    member _.fontStretchSemiCondensed = h.MakeStyle("font-stretch", "semi-condensed")
    /// Default value. No font stretching
    member _.fontStretchNormal = h.MakeStyle("font-stretch", "normal")
    /// Makes the text wider than normal, but not as wide as expanded
    member _.fontStretchSemiExpanded = h.MakeStyle("font-stretch", "semi-expanded")
    /// Makes the text wider than semi-expanded, but not as wide as extra-expanded
    member _.fontStretchExpanded = h.MakeStyle("font-stretch", "expanded")
    /// Makes the text wider than expanded, but not as wide as ultra-expanded
    member _.fontStretchExtraExpanded = h.MakeStyle("font-stretch", "extra-expanded")
    /// Makes the text as wide as it gets.
    member _.fontStretchUltraExpanded = h.MakeStyle("font-stretch", "ultra-expanded")
    /// Sets this property to its default value.
    member _.fontStretchInitial = h.MakeStyle("font-stretch", "initial")
    /// Inherits this property from its parent element.
    member _.fontStretchInheritFromParent = h.MakeStyle("font-stretch", "inherit")

    /// The element does not float, (will be displayed just where it occurs in the text). This is default
    member _.floatStyleNone = h.MakeStyle("float", "none")
    member _.floatStyleLeft = h.MakeStyle("float", "left")
    member _.floatStyleRight = h.MakeStyle("float", "right")
    /// Sets this property to its default value.
    member _.floatStyleInitial = h.MakeStyle("float", "initial")
    /// Inherits this property from its parent element.
    member _.floatStyleInheritFromParent = h.MakeStyle("float", "inherit")

    /// The element is aligned with the baseline of the parent. This is default.
    member _.verticalAlignBaseline = h.MakeStyle("vertical-align", "baseline")
    /// The element is aligned with the subscript baseline of the parent
    member _.verticalAlignSub = h.MakeStyle("vertical-align", "sup")
    /// The element is aligned with the superscript baseline of the parent.
    member _.verticalAlignSuper = h.MakeStyle("vertical-align", "super")
    /// The element is aligned with the top of the tallest element on the line.
    member _.verticalAlignTop = h.MakeStyle("vertical-align", "top")
    /// The element is aligned with the top of the parent element's font.
    member _.verticalAlignTextTop = h.MakeStyle("vertical-align", "text-top")
    /// The element is placed in the middle of the parent element.
    member _.verticalAlignMiddle = h.MakeStyle("vertical-align", "middle")
    /// The element is aligned with the lowest element on the line.
    member _.verticalAlignBottom = h.MakeStyle("vertical-align", "bottom")
    /// The element is aligned with the bottom of the parent element's font
    member _.verticalAlignTextBottom = h.MakeStyle("vertical-align", "text-bottom")
    /// Sets this property to its default value.
    member _.verticalAlignInitial = h.MakeStyle("vertical-align", "initial")
    /// Inherits this property from its parent element.
    member _.verticalAlignInheritFromParent = h.MakeStyle("vertical-align", "inherit")

    /// Let the content flow horizontally from left to right, vertically from top to bottom
    member _.writingModeHorizontalTopBottom = h.MakeStyle("writing-mode", "horizontal-tb")
    /// Let the content flow vertically from top to bottom, horizontally from right to left
    member _.writingModeVerticalRightLeft = h.MakeStyle("writing-mode", "vertical-rl")
    /// Let the content flow vertically from top to bottom, horizontally from left to right
    member _.writingModeVerticalLeftRight = h.MakeStyle("writing-mode", "vertical-lr")
    /// Sets this property to its default value.
    member _.writingModeInitial = h.MakeStyle("writing-mode", "initial")
    /// Inherits this property from its parent element.
    member _.writingModeInheritFromParent = h.MakeStyle("writing-mode", "inherit")

    /// Default value. Specifies a animation effect with a slow start, then fast, then end slowly (equivalent to cubic-bezier(0.25,0.1,0.25,1)).
    member _.animationTimingFunctionEase = h.MakeStyle("animation-timing-function", "ease")
    /// Specifies a animation effect with the same speed from start to end (equivalent to cubic-bezier(0,0,1,1))
    member _.animationTimingFunctionLinear = h.MakeStyle("animation-timing-function", "linear")
    /// Specifies a animation effect with a slow start (equivalent to cubic-bezier(0.42,0,1,1)).
    member _.animationTimingFunctionEaseIn = h.MakeStyle("animation-timing-function", "ease-in")
    /// Specifies a animation effect with a slow end (equivalent to cubic-bezier(0,0,0.58,1)).
    member _.animationTimingFunctionEaseOut = h.MakeStyle("animation-timing-function", "ease-out")
    /// Specifies a animation effect with a slow start and end (equivalent to cubic-bezier(0.42,0,0.58,1))
    member _.animationTimingFunctionEaseInOut = h.MakeStyle("animation-timing-function", "ease-in-out")
    /// Define your own values in the cubic-bezier function. Possible values are numeric values from 0 to 1
    member _.animationTimingFunctionCubicBezier(n1: float, n2: float, n3: float, n4: float) = h.MakeStyle("animation-timing-function", "cubic-bezier(" + (asString n1) + "," + (asString n2) + "," + (asString n3) + ", " + (asString n4) + ")")
    /// Sets this property to its default value
    member _.animationTimingFunctionInitial = h.MakeStyle("animation-timing-function", "initial")
    /// Inherits this property from its parent element.
    member _.animationTimingFunctionInheritFromParent = h.MakeStyle("animation-timing-function", "inherit")

    /// Default value. Specifies a transition effect with a slow start, then fast, then end slowly (equivalent to cubic-bezier(0.25,0.1,0.25,1)).
    member _.transitionTimingFunctionEase = h.MakeStyle("transition-timing-function", "ease")
    /// Specifies a transition effect with the same speed from start to end (equivalent to cubic-bezier(0,0,1,1))
    member _.transitionTimingFunctionLinear = h.MakeStyle("transition-timing-function", "linear")
    /// Specifies a transition effect with a slow start (equivalent to cubic-bezier(0.42,0,1,1)).
    member _.transitionTimingFunctionEaseIn = h.MakeStyle("transition-timing-function", "ease-in")
    /// Specifies a transition effect with a slow end (equivalent to cubic-bezier(0,0,0.58,1)).
    member _.transitionTimingFunctionEaseOut = h.MakeStyle("transition-timing-function", "ease-out")
    /// Specifies a transition effect with a slow start and end (equivalent to cubic-bezier(0.42,0,0.58,1))
    member _.transitionTimingFunctionEaseInOut = h.MakeStyle("transition-timing-function", "ease-in-out")
    /// Equivalent to steps(1, start)
    member _.transitionTimingFunctionStepStart = h.MakeStyle("transition-timing-function", "step-start")
    /// Equivalent to steps(1, end)
    member _.transitionTimingFunctionStepEnd = h.MakeStyle("transition-timing-function", "step-end")
    /// Define your own values in the cubic-bezier function. Possible values are numeric values from 0 to 1
    member _.transitionTimingFunctionCubicBezier(n1: float, n2: float, n3: float, n4: float) = h.MakeStyle("transition-timing-function", "cubic-bezier(" + (asString n1) + "," + (asString n2) + "," + (asString n3) + ", " + (asString n4) + ")")
    /// Sets this property to its default value
    member _.transitionTimingFunctionInitial = h.MakeStyle("transition-timing-function", "initial")
    /// Inherits this property from its parent element.
    member _.transitionTimingFunctionInheritFromParent = h.MakeStyle("transition-timing-function", "inherit")

    /// Default. Text can be selected if the browser allows it.
    member _.userSelectAuto = h.MakeStyle("user-select", "auto")
    /// Prevents text selection.
    member _.userSelectNone = h.MakeStyle("user-select", "none")
    /// The text can be selected by the user.
    member _.userSelectText = h.MakeStyle("user-select", "text")
    /// Text selection is made with one click instead of a double-click.
    member _.userSelectAll = h.MakeStyle("user-select", "all")
    /// Sets this property to its default value.
    member _.userSelectInitial = h.MakeStyle("user-select", "initial")
    /// Inherits this property from its parent element.
    member _.userSelectInheritFromParent = h.MakeStyle("user-select", "inherit")

    /// Sets the line style for all four sides of an element's border.
    member _.borderStyle(style: IBorderStyle) = h.MakeStyle("border-style", asString style)
    /// Sets the line style for all four sides of an element's border.
    member _.borderStyle(vertical: IBorderStyle, horizontal: IBorderStyle)  =
        h.MakeStyle("border-style", (asString vertical) + " " + (asString horizontal))
    /// Sets the line style for all four sides of an element's border.
    member _.borderStyle(top: IBorderStyle, right: IBorderStyle, bottom: IBorderStyle, left: IBorderStyle) =
        h.MakeStyle("border-style", (asString top) + " " + (asString right) + " " + (asString bottom) + " " +  (asString left))
    /// Specifies a dotted border.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_border-style&preval=dotted
    member _.borderStyleDotted = h.MakeStyle("border-style", "dotted")
    /// Specifies a dashed border.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_border-style&preval=dotted
    member _.borderStyleDashed = h.MakeStyle("border-style", "dashed")
    /// Specifies a solid border.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_border-style&preval=dotted
    member _.borderStyleSolid = h.MakeStyle("border-style", "solid")
    /// Specifies a double border.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_border-style&preval=dotted
    member _.borderStyleDouble = h.MakeStyle("border-style", "double")
    /// Specifies a 3D grooved border. The effect depends on the border-color value.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_border-style&preval=dotted
    member _.borderStyleGroove = h.MakeStyle("border-style", "groove")
    /// Specifies a 3D ridged border. The effect depends on the border-color value.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_border-style&preval=dotted
    member _.borderStyleRidge = h.MakeStyle("border-style", "ridge")
    /// Specifies a 3D inset border. The effect depends on the border-color value.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_border-style&preval=dotted
    member _.borderStyleInset = h.MakeStyle("border-style", "inset")
    /// Specifies a 3D outset border. The effect depends on the border-color value.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_border-style&preval=dotted
    member _.borderStyleOutset = h.MakeStyle("border-style", "outset")
    /// Default value. Specifies no border.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_border-style&preval=dotted
    member _.borderStyleNone = h.MakeStyle("border-style", "none")
    /// The same as "none", except in border conflict resolution for table elements.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_border-style&preval=hidden
    member _.borderStyleHidden = h.MakeStyle("border-style", "hidden")
    /// Sets this property to its default value.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_border-style&preval=hidden
    ///
    /// Read about initial value https://www.w3schools.com/cssref/css_initial.asp
    member _.borderStyleInitial = h.MakeStyle("border-style", "initial")
    /// Inherits this property from its parent element.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_border-style&preval=hidden
    ///
    /// Read about inherit https://www.w3schools.com/cssref/css_inherit.asp
    member _.borderStyleInheritFromParent = h.MakeStyle("border-style", "inherit")

    /// Browsers use an automatic table layout algorithm. The column width is set by the widest unbreakable
    /// content in the cells. The content will dictate the layout
    member _.tableLayoutAuto = h.MakeStyle("table-layout", "auto")
    /// Sets a fixed table layout algorithm. The table and column widths are set by the widths of table and col
    /// or by the width of the first row of cells. Cells in other rows do not affect column widths. If no widths
    /// are present on the first row, the column widths are divided equally across the table, regardless of content
    /// inside the cells
    member _.tableLayoutFixed' = h.MakeStyle("table-layout", "fixed")
    /// Sets this property to its default value.
    member _.tableLayoutInitial = h.MakeStyle("table-layout", "initial")
    /// Inherits this property from its parent element.
    member _.tableLayoutInheritFromParent = h.MakeStyle("table-layout", "inherit")

    member _.cursor(value: string) = h.MakeStyle("cursor", asString value)
    /// The User Agent will determine the cursor to display based on the current context. E.g., equivalent to text when hovering text.
    member _.cursorAuto = h.MakeStyle("cursor", "auto")
    /// The cursor indicates an alias of something is to be created
    member _.cursorAlias = h.MakeStyle("cursor", "alias")
    /// The platform-dependent default cursor. Typically an arrow.
    member _.cursorDefaultCursor = h.MakeStyle("cursor", "default")
    /// No cursor is rendered.
    member _.cursorNone = h.MakeStyle("cursor", "none")
    /// A context menu is available.
    member _.cursorContextMenu = h.MakeStyle("cursor", "context-menu")
    /// Help information is available.
    member _.cursorHelp = h.MakeStyle("cursor", "help")
    /// The cursor is a pointer that indicates a link. Typically an image of a pointing hand.
    member _.cursorPointer = h.MakeStyle("cursor", "pointer")
    /// The program is busy in the background, but the user can still interact with the interface (in contrast to `wait`).
    member _.cursorProgress = h.MakeStyle("cursor", "progress")
    /// The program is busy, and the user can't interact with the interface (in contrast to progress). Sometimes an image of an hourglass or a watch.
    member _.cursorWait = h.MakeStyle("cursor", "wait")
    /// The table cell or set of cells can be selected.
    member _.cursorCell = h.MakeStyle("cursor", "cell")
    /// Cross cursor, often used to indicate selection in a bitmap.
    member _.cursorCrosshair = h.MakeStyle("cursor", "crosshair")
    /// The text can be selected. Typically the shape of an I-beam.
    member _.cursorText = h.MakeStyle("cursor", "text")
    /// The vertical text can be selected. Typically the shape of a sideways I-beam.
    member _.cursorVerticalText = h.MakeStyle("cursor", "vertical-text")
    /// Something is to be copied.
    member _.cursorCopy = h.MakeStyle("cursor", "copy")
    /// Something is to be moved.
    member _.cursorMove = h.MakeStyle("cursor", "move")
    /// An item may not be dropped at the current location. On Windows and Mac OS X, `no-drop` is the same as `not-allowed`.
    member _.cursorNoDrop = h.MakeStyle("cursor", "no-drop")
    /// The requested action will not be carried out.
    member _.cursorNotAllowed = h.MakeStyle("cursor", "not-allowed")
    /// Something can be grabbed (dragged to be moved).
    member _.cursorGrab = h.MakeStyle("cursor", "grab")
    /// Something is being grabbed (dragged to be moved).
    member _.cursorGrabbing = h.MakeStyle("cursor", "grabbing")
    /// Something can be scrolled in any direction (panned).
    member _.cursorAllScroll = h.MakeStyle("cursor", "all-scroll")
    /// The item/column can be resized horizontally. Often rendered as arrows pointing left and right with a vertical bar separating them.
    member _.cursorColumnResize = h.MakeStyle("cursor", "col-resize")
    /// The item/row can be resized vertically. Often rendered as arrows pointing up and down with a horizontal bar separating them.
    member _.cursorRowResize = h.MakeStyle("cursor", "row-resize")
    /// Directional resize arrow
    member _.cursorNorthResize = h.MakeStyle("cursor", "n-resize")
    /// Directional resize arrow
    member _.cursorEastResize = h.MakeStyle("cursor", "e-resize")
    /// Directional resize arrow
    member _.cursorSouthResize = h.MakeStyle("cursor", "s-resize")
    /// Directional resize arrow
    member _.cursorWestResize = h.MakeStyle("cursor", "w-resize")
    /// Directional resize arrow
    member _.cursorNorthEastResize = h.MakeStyle("cursor", "ne-resize")
    /// Directional resize arrow
    member _.cursorNorthWestResize = h.MakeStyle("cursor", "nw-resize")
    /// Directional resize arrow
    member _.cursorSouthEastResize = h.MakeStyle("cursor", "se-resize")
    /// Directional resize arrow
    member _.cursorSouthWestResize = h.MakeStyle("cursor", "sw-resize")
    /// Directional resize arrow
    member _.cursorEastWestResize = h.MakeStyle("cursor", "ew-resize")
    /// Directional resize arrow
    member _.cursorNorthSouthResize = h.MakeStyle("cursor", "ns-resize")
    /// Directional resize arrow
    member _.cursorNorthEastSouthWestResize = h.MakeStyle("cursor", "nesw-resize")
    /// Directional resize arrow
    member _.cursorNorthWestSouthEastResize = h.MakeStyle("cursor", "nwse-resize")
    /// Something can be zoomed (magnified) in
    member _.cursorZoomIn = h.MakeStyle("cursor", "zoom-in")
    /// Something can be zoomed out
    member _.cursorZoomOut = h.MakeStyle("cursor", "zoom-out")

    /// Permits the user agent to render a custom outline style.
    member _.outlineStyleAuto = h.MakeStyle("outline-style", "auto")
    /// Specifies no outline. This is default.
    member _.outlineStyleNone = h.MakeStyle("outline-style", "none")
    /// Specifies a hidden outline
    member _.outlineStyleHidden = h.MakeStyle("outline-style", "hidden")
    /// Specifies a dotted outline
    member _.outlineStyleDotted = h.MakeStyle("outline-style", "dotted")
    /// Specifies a dashed outline
    member _.outlineStyleDashed = h.MakeStyle("outline-style", "dashed")
    /// Specifies a solid outline
    member _.outlineStyleSolid = h.MakeStyle("outline-style", "solid")
    /// Specifies a double outliner
    member _.outlineStyleDouble = h.MakeStyle("outline-style", "double")
    /// Specifies a 3D grooved outline. The effect depends on the outline-color value
    member _.outlineStyleGroove = h.MakeStyle("outline-style", "groove")
    /// Specifies a 3D ridged outline. The effect depends on the outline-color value
    member _.outlineStyleRidge = h.MakeStyle("outline-style", "ridge")
    /// Specifies a 3D inset  outline. The effect depends on the outline-color value
    member _.outlineStyleInset = h.MakeStyle("outline-style", "inset")
    /// Specifies a 3D outset outline. The effect depends on the outline-color value
    member _.outlineStyleOutset = h.MakeStyle("outline-style", "outset")
    /// Sets this property to its default value
    member _.outlineStyleInitial = h.MakeStyle("outline-style", "initial")
    /// Inherits this property from its parent element
    member _.outlineStyleInheritFromParent = h.MakeStyle("outline-style", "inherit")

    /// Sets the initial position for each background image.
    ///
    /// The position is relative to the position layer set by background-origin.
    member _.backgroundPosition(position: string) = h.MakeStyle("background-position", position)
    /// The background image will scroll with the page. This is default.
    member _.backgroundPositionScroll = h.MakeStyle("background-position", "scroll")
    /// The background image will not scroll with the page.
    member _.backgroundPositionFixedNoScroll = h.MakeStyle("background-position", "fixed")
    /// The background image will scroll with the element's contents.
    member _.backgroundPositionLocal = h.MakeStyle("background-position", "local")
    /// Sets this property to its default value.
    member _.backgroundPositionInitial = h.MakeStyle("background-position", "initial")
    /// Inherits this property from its parent element.
    member _.backgroundPositionInheritFromParent = h.MakeStyle("background-position", "inherit")

    /// This is default. Sets the blending mode to normal.
    member _.backgroundBlendModeNormal = h.MakeStyle("background-blend-mode", "normal")
    /// Sets the blending mode to screen
    member _.backgroundBlendModeScreen = h.MakeStyle("background-blend-mode", "screen")
    /// Sets the blending mode to overlay
    member _.backgroundBlendModeOverlay = h.MakeStyle("background-blend-mode", "overlay")
    /// Sets the blending mode to darken
    member _.backgroundBlendModeDarken = h.MakeStyle("background-blend-mode", "darken")
    /// Sets the blending mode to multiply
    member _.backgroundBlendModeLighten = h.MakeStyle("background-blend-mode", "lighten")
    /// Sets the blending mode to color-dodge
    member _.backgroundBlendModeCollorDodge = h.MakeStyle("background-blend-mode", "color-dodge")
    /// Sets the blending mode to saturation
    member _.backgroundBlendModeSaturation = h.MakeStyle("background-blend-mode", "saturation")
    /// Sets the blending mode to color
    member _.backgroundBlendModeColor = h.MakeStyle("background-blend-mode", "color")
    /// Sets the blending mode to luminosity
    member _.backgroundBlendModeLuminosity = h.MakeStyle("background-blend-mode", "luminosity")

    /// Default value. The background extends behind the border.
    member _.backgroundClipBorderBox = h.MakeStyle("background-clip", "border-box")
    /// The background extends to the inside edge of the border.
    member _.backgroundClipPaddingBox = h.MakeStyle("background-clip", "padding-box")
    /// The background extends to the edge of the content box.
    member _.backgroundClipContentBox = h.MakeStyle("background-clip", "content-box")
    /// Sets this property to its default value.
    member _.backgroundClipInitial = h.MakeStyle("background-clip", "initial")
    /// Inherits this property from its parent element.
    member _.backgroundClipInheritFromParent = h.MakeStyle("background-clip", "inherit")

    member _.transform(transformation: ITransformProperty) =
        h.MakeStyle("transform", asString transformation)
    member _.transform(transformations: ITransformProperty list) =
        h.MakeStyle("transform", String.concat " " (transformations |> List.map asString))
    /// Defines that there should be no transformation.
    member _.transformNone = h.MakeStyle("transform", "none")
    /// Defines a 2D transformation, using a matrix of six values.
    member _.transformMatrix(x1: int, y1: int, z1: int, x2: int, y2: int, z2: int) = h.MakeStyle("transform", "matrix(" + (asString x1) + "," + (asString y1) + "," + (asString z1) + "," + (asString x2) + "," + (asString y2) + ", " + (asString z2) + ")")
    /// Defines a 2D translation.
    member _.transformTranslate(x: int, y: int) = h.MakeStyle("transform", "translate(" + (asString x) + "px," + (asString y) + "px)")
    /// Defines a 2D translation.
    member _.transformTranslate(x: ICssUnit, y: ICssUnit) = h.MakeStyle("transform", "translate(" + (asString x) + ", " + (asString y) + ")")
    /// Defines a 3D translation.
    member _.transformTranslate3D(x: int, y: int, z: int) = h.MakeStyle("transform", "translate3d(" + (asString x) + "px," + (asString y) + "px," + (asString z) + "px)")
    /// Defines a 3D translation.
    member _.transformTranslate3D(x: ICssUnit, y: ICssUnit, z: ICssUnit) = h.MakeStyle("transform", "translate3d(" + (asString x) + "," + (asString y) + ", " + (asString z) + ")")
    /// Defines a translation, using only the value for the X-axis.
    member _.transformTranslateX(x: int) = h.MakeStyle("transform", "translateX(" + (asString x) + "px)")
    /// Defines a translation, using only the value for the X-axis.
    member _.transformTranslateX(x: ICssUnit) = h.MakeStyle("transform", "translateX(" + (asString x) + ")")
    /// Defines a translation, using only the value for the Y-axis
    member _.transformTranslateY(y: int) = h.MakeStyle("transform", "translateY(" + (asString y) + "px)")
    /// Defines a translation, using only the value for the Y-axis
    member _.transformTranslateY(y: ICssUnit) = h.MakeStyle("transform", "translateY(" + (asString y) + ")")
    /// Defines a 3D translation, using only the value for the Z-axis
    /// Defines a 3D translation, using only the value for the Z-axis
    member _.transformTranslateZ(z: ICssUnit) = h.MakeStyle("transform", "translateZ(" + (asString z) + ")")
    /// Defines a 2D scale transformation.
    member _.transformScale(x: int, y: int) = h.MakeStyle("transform", "scale(" + (asString x) + ", " + (asString y) + ")")
    /// Defines a scale transformation.
    /// Defines a scale transformation.
    member _.transformScale(n: float) = h.MakeStyle("transform", "scale(" + (asString n) + ")")
    /// Defines a 3D scale transformation
    member _.transformScale3D(x: int, y: int, z: int) = h.MakeStyle("transform", "scale3d(" + (asString x) + "," + (asString y) + ", " + (asString z) + ")")
    /// Defines a scale transformation by giving a value for the X-axis.
    member _.transformScaleX(x: int) = h.MakeStyle("transform", "scaleX(" + (asString x) + ")")
    /// Defines a scale transformation by giving a value for the Y-axis.
    member _.transformScaleY(y: int) = h.MakeStyle("transform", "scaleY(" + (asString y) + ")")
    /// Defines a 3D translation, using only the value for the Z-axis
    member _.transformScaleZ(z: int) = h.MakeStyle("transform", "scaleZ(" + (asString z) + ")")
    /// Defines a 2D rotation, the angle is specified in the parameter.
    member _.transformRotate(deg: int) = h.MakeStyle("transform", "rotate(" + (asString deg) + "deg)")
    /// Defines a 2D rotation, the angle is specified in the parameter.
    member _.transformRotate(deg: float) = h.MakeStyle("transform", "rotate(" + (asString deg) + "deg)")
    /// Defines a 3D rotation along the X-axis.
    member _.transformRotateX(deg: float) = h.MakeStyle("transform", "rotateX(" + (asString deg) + "deg)")
    /// Defines a 3D rotation along the X-axis.
    member _.transformRotateX(deg: int) = h.MakeStyle("transform", "rotateX(" + (asString deg) + "deg)")
    /// Defines a 3D rotation along the Y-axis
    member _.transformRotateY(deg: float) = h.MakeStyle("transform", "rotateY(" + (asString deg) + "deg)")
    /// Defines a 3D rotation along the Y-axis
    member _.transformRotateY(deg: int) = h.MakeStyle("transform", "rotateY(" + (asString deg) + "deg)")
    /// Defines a 3D rotation along the Z-axis
    member _.transformRotateZ(deg: float) = h.MakeStyle("transform", "rotateZ(" + (asString deg) + "deg)")
    /// Defines a 3D rotation along the Z-axis
    member _.transformRotateZ(deg: int) = h.MakeStyle("transform", "rotateZ(" + (asString deg) + "deg)")
    /// Defines a 2D skew transformation along the X- and the Y-axis.
    member _.transformSkew(xAngle: int, yAngle: int) = h.MakeStyle("transform", "skew(" + (asString xAngle) + "deg," + (asString yAngle) + "deg)")
    /// Defines a 2D skew transformation along the X- and the Y-axis.
    member _.transformSkew(xAngle: float, yAngle: float) = h.MakeStyle("transform", "skew(" + (asString xAngle) + "deg," + (asString yAngle) + "deg)")
    /// Defines a 2D skew transformation along the X-axis
    member _.transformSkewX(xAngle: int) = h.MakeStyle("transform", "skewX(" + (asString xAngle) + "deg)")
    /// Defines a 2D skew transformation along the X-axis
    member _.transformSkewX(xAngle: float) = h.MakeStyle("transform", "skewX(" + (asString xAngle) + "deg)")
    /// Defines a 2D skew transformation along the Y-axis
    member _.transformSkewY(xAngle: int) = h.MakeStyle("transform", "skewY(" + (asString xAngle) + "deg)")
    /// Defines a 2D skew transformation along the Y-axis
    member _.transformSkewY(xAngle: float) = h.MakeStyle("transform", "skewY(" + (asString xAngle) + "deg)")
    /// Defines a perspective view for a 3D transformed element
    member _.transformPerspective(n: int) = h.MakeStyle("transform", "perspective(" + (asString n) + ")")
    /// Sets this property to its default value.
    member _.transformInitial = h.MakeStyle("transform", "initial")
    /// Inherits this property from its parent element.
    member _.transformInheritFromParent = h.MakeStyle("transform", "inherit")

    /// Text direction goes from right-to-left
    member _.directionRightToLeft = h.MakeStyle("direction", "rtl")
    /// Text direction goes from left-to-right. This is default
    member _.directionLeftToRight = h.MakeStyle("direction", "ltr")
    /// Sets this property to its default value.
    member _.directionInitial = h.MakeStyle("direction", "initial")
    /// Inherits this property from its parent element.
    member _.directionInheritFromParent = h.MakeStyle("direction", "inherit")

    /// Display borders on empty cells. This is default
    member _.emptyCellsShow = h.MakeStyle("empty-cells", "show")
    /// Hide borders on empty cells
    member _.emptyCellsHide = h.MakeStyle("empty-cells", "hide")
    /// Sets this property to its default value
    member _.emptyCellsInitial = h.MakeStyle("empty-cells", "initial")
    /// Inherits this property from its parent element
    member _.emptyCellsInheritFromParent = h.MakeStyle("empty-cells", "inherit")

    /// Default value. The animation should be played as normal
    member _.animationDirectionNormal = h.MakeStyle("animation-direction", "normal")
    /// The animation should play in reverse direction
    member _.animationDirectionReverse = h.MakeStyle("animation-direction", "reverse")
    /// The animation will be played as normal every odd time (1, 3, 5, etc..) and in reverse direction every even time (2, 4, 6, etc...).
    member _.animationDirectionAlternate = h.MakeStyle("animation-direction", "alternate")
    /// The animation will be played in reverse direction every odd time (1, 3, 5, etc..) and in a normal direction every even time (2,4,6,etc...)
    member _.animationDirectionAlternateReverse = h.MakeStyle("animation-direction", "alternate-reverse")
    /// Sets this property to its default value
    member _.animationDirectionInitial = h.MakeStyle("animation-direction", "initial")
    /// Inherits this property from its parent element.
    member _.animationDirectionInheritFromParent = h.MakeStyle("animation-direction", "inherit")

    /// Default value. Specifies that the animation is running.
    member _.animationPlayStateRunning = h.MakeStyle("animation-play-state", "running")
    /// Specifies that the animation is paused
    member _.animationPlayStatePaused = h.MakeStyle("animation-play-state", "paused")
    /// Sets this property to its default value
    member _.animationPlayStateInitial = h.MakeStyle("animation-play-state", "initial")
    /// Inherits this property from its parent element.
    member _.animationPlayStateInheritFromParent = h.MakeStyle("animation-play-state", "inherit")

    /// Specifies that the animation should be played infinite times (forever)
    member _.animationIterationCountInfinite = h.MakeStyle("animation-iteration-count", "infinite")
    /// Sets this property to its default value
    member _.animationIterationCountInitial = h.MakeStyle("animation-iteration-count", "initial")
    /// Inherits this property from its parent element.
    member _.animationIterationCountInheritFromParent = h.MakeStyle("animation-iteration-count", "inherit")

    /// Default value. Animation will not apply any styles to the element before or after it is executing
    member _.animationFillModeNone = h.MakeStyle("animation-fill-mode", "none")
    /// The element will retain the style values that is set by the last keyframe (depends on animation-direction and animation-iteration-count).
    member _.animationFillModeForwards = h.MakeStyle("animation-fill-mode", "forwards")
    /// The element will get the style values that is set by the first keyframe (depends on animation-direction), and retain this during the animation-delay period
    member _.animationFillModeBackwards = h.MakeStyle("animation-fill-mode", "backwards")
    /// The animation will follow the rules for both forwards and backwards, extending the animation properties in both directions
    member _.animationFillModeBoth = h.MakeStyle("animation-fill-mode", "both")
    /// Sets this property to its default value
    member _.animationFillModeInitial = h.MakeStyle("animation-fill-mode", "initial")
    /// Inherits this property from its parent element
    member _.animationFillModeInheritFromParent = h.MakeStyle("animation-fill-mode", "inherit")

    /// Sets how background images are repeated.
    ///
    /// A background image can be repeated along the horizontal and vertical axes, or not repeated at all.
    member _.backgroundRepeat(repeat: IBackgroundRepeat) = h.MakeStyle("background-repeat", asString repeat)
    /// The background image is repeated both vertically and horizontally. This is default.
    member _.backgroundRepeatRepeat = h.MakeStyle("background-repeat", "repeat")
    /// The background image is only repeated horizontally.
    member _.backgroundRepeatRepeatX = h.MakeStyle("background-repeat", "repeat-x")
    /// The background image is only repeated vertically.
    member _.backgroundRepeatRepeatY = h.MakeStyle("background-repeat", "repeat-y")
    /// The background-image is not repeated.
    member _.backgroundRepeatNoRepeat = h.MakeStyle("background-repeat", "no-repeat")
    /// Sets this property to its default value.
    member _.backgroundRepeatInitial = h.MakeStyle("background-repeat", "initial")
    /// Inherits this property from its parent element.
    member _.backgroundRepeatInheritFromParent = h.MakeStyle("background-repeat", "inherit")

    /// Default value. Elements render in order, as they appear in the document flow.
    member _.positionDefaultStatic = h.MakeStyle("position", "static")
    /// The element is positioned relative to its first positioned (not static) ancestor element.
    member _.positionAbsolute = h.MakeStyle("position", "absolute")
    /// The element is positioned relative to the browser window
    member _.positionFixed = h.MakeStyle("position", "fixed")
    /// The element is positioned relative to its normal position, so "left:20px" adds 20 pixels to the element's LEFT position.
    member _.positionRelative = h.MakeStyle("position", "relative")
    /// The element is positioned based on the user's scroll position
    ///
    /// A sticky element toggles between relative and fixed, depending on the scroll position. It is positioned relative until a given offset position is met in the viewport - then it "sticks" in place (like position:fixed).
    ///
    /// Note: Not supported in IE/Edge 15 or earlier. Supported in Safari from version 6.1 with a -webkit- prefix.
    member _.positionSticky = h.MakeStyle("position", "sticky")
    member _.positionInitial = h.MakeStyle("position", "initial")
    /// Inherits this property from its parent element.
    member _.positionInheritFromParent = h.MakeStyle("position", "inherit")

    /// Default value. The width and height properties include the content, but does not include the padding, border, or margin.
    member _.boxSizingContentBox = h.MakeStyle("box-sizing", "content-box")
    /// The width and height properties include the content, padding, and border, but do not include the margin. Note that padding and border will be inside of the box.
    member _.boxSizingBorderBox = h.MakeStyle("box-sizing", "border-box")
    /// Sets this property to its default value.
    member _.boxSizingInitial = h.MakeStyle("box-sizing", "initial")
    /// Inherits this property from its parent element.
    member _.boxSizingInheritFromParent = h.MakeStyle("box-sizing", "inherit")

    /// Default value. The element offers no user-controllable method for resizing it.
    member _.resizeNone = h.MakeStyle("resize", "none")
    /// The element displays a mechanism for allowing the user to resize it, which may be resized both horizontally and vertically.
    member _.resizeBoth = h.MakeStyle("resize", "both")
    /// The element displays a mechanism for allowing the user to resize it in the horizontal direction.
    member _.resizeHorizontal = h.MakeStyle("resize", "horizontal")
    /// The element displays a mechanism for allowing the user to resize it in the vertical direction.
    member _.resizeVertical = h.MakeStyle("resize", "vertical")
    /// The element displays a mechanism for allowing the user to resize it in the block direction (either horizontally or vertically, depending on the writing-mode and direction value).
    member _.resizeBlock = h.MakeStyle("resize", "block")
    /// The element displays a mechanism for allowing the user to resize it in the inline direction (either horizontally or vertically, depending on the writing-mode and direction value).
    member _.resizeInline' = h.MakeStyle("resize", "inline")
    /// Sets this property to its default value.
    member _.resizeInitial = h.MakeStyle("resize", "initial")
    /// Inherits this property from its parent element.
    member _.resizeInheritFromParent = h.MakeStyle("resize", "inherit")

    /// Aligns the text to the left.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_text-align
    member _.textAlignLeft = h.MakeStyle("text-align", "left")
    /// Aligns the text to the right.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_text-align&preval=right
    member _.textAlignRight = h.MakeStyle("text-align", "right")
    /// Centers the text.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_text-align&preval=center
    member _.textAlignCenter = h.MakeStyle("text-align", "center")
    /// Stretches the lines so that each line has equal width (like in newspapers and magazines).
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_text-align&preval=justify
    member _.textAlignJustify = h.MakeStyle("text-align", "justify")
    /// Sets this property to its default value.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_text-align&preval=initial
    member _.textAlignInitial = h.MakeStyle("text-align", "initial")
    /// Inherits this property from its parent element.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_text-align&preval=initial
    member _.textAlignInheritFromParent = h.MakeStyle("text-align", "inherit")

    /// Displays an element as an inline element (like `<span> `). Any height and width properties will have no effect.
    member _.displayInlineElement = h.MakeStyle("display", "inline")
    /// Displays an element as a block element (like `<p> `). It starts on a new line, and takes up the whole width.
    member _.displayBlock = h.MakeStyle("display", "block")
    /// Makes the container disappear, making the child elements children of the element the next level up in the DOM.
    member _.displayContents = h.MakeStyle("display", "contents")
    /// Displays an element as a block-level flex container.
    member _.displayFlex = h.MakeStyle("display", "flex")
    /// Displays an element as a block container box, and lays out its contents using flow layout.
    ///
    /// It always establishes a new block formatting context for its contents.
    member _.displayFlowRoot = h.MakeStyle("display", "flow-root")
    /// Displays an element as a block-level grid container.
    member _.displayGrid = h.MakeStyle("display", "grid")
    /// Displays an element as an inline-level block container. The element itself is formatted as an inline element, but you can apply height and width values.
    member _.displayInlineBlock = h.MakeStyle("display", "inline-block")
    /// Displays an element as an inline-level flex container.
    member _.displayInlineFlex = h.MakeStyle("display", "inline-flex")
    /// Displays an element as an inline-level grid container
    member _.displayInlineGrid = h.MakeStyle("display", "inline-grid")
    /// The element is displayed as an inline-level table.
    member _.displayInlineTable = h.MakeStyle("display", "inline-table")
    /// Let the element behave like a `<li> ` element
    member _.displayListItem = h.MakeStyle("display", "list-item")
    /// Displays an element as either block or inline, depending on context.
    member _.displayRunIn = h.MakeStyle("display", "run-in")
    /// Let the element behave like a `<table> ` element.
    member _.displayTable = h.MakeStyle("display", "table")
    /// Let the element behave like a <caption> element.
    member _.displayTableCaption = h.MakeStyle("display", "table-caption")
    /// Let the element behave like a <colgroup> element.
    member _.displayTableColumnGroup = h.MakeStyle("display", "table-column-group")
    /// Let the element behave like a <thead> element.
    member _.displayTableHeaderGroup = h.MakeStyle("display", "table-header-group")
    /// Let the element behave like a <tfoot> element.
    member _.displayTableFooterGroup = h.MakeStyle("display", "table-footer-group")
    /// Let the element behave like a <tbody> element.
    member _.displayTableRowGroup = h.MakeStyle("display", "table-row-group")
    /// Let the element behave like a <td> element.
    member _.displayTableCell = h.MakeStyle("display", "table-cell")
    /// Let the element behave like a <col> element.
    member _.displayTableColumn = h.MakeStyle("display", "table-column")
    /// Let the element behave like a <tr> element.
    member _.displayTableRow = h.MakeStyle("display", "table-row")
    /// The element is completely removed.
    member _.displayNone = h.MakeStyle("display", "none")
    /// Sets this property to its default value.
    member _.displayInitial = h.MakeStyle("display", "initial")
    /// Inherits this property from its parent element.
    member _.displayInheritFromParent = h.MakeStyle("display", "inherit")

    /// The zIndex property sets or returns the stack order of a positioned element.
    ///
    /// An element with greater stack order (1) is always in front of another element with lower stack order (0).
    ///
    /// **Tip**: A positioned element is an element with the position property set to: relative, absolute, or fixed.
    ///
    /// **Tip**: This property is useful if you want to create overlapping elements.
    member _.zIndex(value: int) = h.MakeStyle("z-index", asString value)

    /// Sets the margin area on all four sides of an element. It is a shorthand for margin-top, margin-right,
    /// margin-bottom, and margin-left.
    member _.margin(value: int) = h.MakeStyle("margin", asString value + "px")
    /// Sets the margin area on all four sides of an element. It is a shorthand for margin-top, margin-right,
    /// margin-bottom, and margin-left.
    member _.margin(value: ICssUnit) = h.MakeStyle("margin", asString value)
    /// Sets the margin area on the vertical and horizontal axis.
    member _.margin(vertical: int, horizonal: int) =
        h.MakeStyle("margin",
            (asString vertical) + "px " +
            (asString horizonal) + "px"
        )
    /// Sets the margin area on the vertical and horizontal axis.
    member _.margin(vertical: ICssUnit, horizonal: ICssUnit) =
        h.MakeStyle("margin",
            (asString vertical) + " " +
            (asString horizonal)
        )
    /// Sets the margin area on all four sides of an element. It is a shorthand for margin-top, margin-right,
    /// margin-bottom, and margin-left.
    member _.margin(top: int, right: int, bottom: int, left: int) =
        h.MakeStyle("margin",
            (asString top) + "px " +
            (asString right) + "px " +
            (asString bottom) + "px " +
            (asString left) + "px"
        )
    /// Sets the margin area on all four sides of an element. It is a shorthand for margin-top, margin-right,
    /// margin-bottom, and margin-left.
    member _.margin(top: ICssUnit, right: ICssUnit, bottom: ICssUnit, left: ICssUnit) =
        h.MakeStyle("margin",
            (asString top) + " " +
            (asString right) + " " +
            (asString bottom) + " " +
            (asString left)
        )
    /// Sets the margin area on the left side of an element. A positive value places it farther from its
    /// neighbors, while a negative value places it closer.
    member _.marginLeft(value: int) = h.MakeStyle("margin-left", asString value + "px")
    /// Sets the margin area on the left side of an element. A positive value places it farther from its
    /// neighbors, while a negative value places it closer.
    member _.marginLeft(value: ICssUnit) = h.MakeStyle("margin-left", asString value)
    /// sets the margin area on the right side of an element. A positive value places it farther from its
    /// neighbors, while a negative value places it closer.
    member _.marginRight(value: int) = h.MakeStyle("margin-right", asString value + "px")
    /// sets the margin area on the right side of an element. A positive value places it farther from its
    /// neighbors, while a negative value places it closer.
    member _.marginRight(value: ICssUnit) = h.MakeStyle("margin-right", asString value)
    /// Sets the margin area on the top of an element. A positive value places it farther from its
    /// neighbors, while a negative value places it closer.
    member _.marginTop(value: int) = h.MakeStyle("margin-top", asString value + "px")
    /// Sets the margin area on the top of an element. A positive value places it farther from its
    /// neighbors, while a negative value places it closer.
    member _.marginTop(value: ICssUnit) = h.MakeStyle("margin-top", asString value)
    /// Sets the margin area on the bottom of an element. A positive value places it farther from its
    /// neighbors, while a negative value places it closer.
    member _.marginBottom(value: int) = h.MakeStyle("margin-bottom", asString value + "px")
    /// Sets the margin area on the bottom of an element. A positive value places it farther from its
    /// neighbors, while a negative value places it closer.
    member _.marginBottom(value: ICssUnit) = h.MakeStyle("margin-bottom", asString value)

    /// Sets the padding area on all four sides of an element. It is a shorthand for padding-top,
    /// padding-right, padding-bottom, and padding-left.
    member _.padding(value: int) = h.MakeStyle("padding", asString value + "px")
    /// Sets the padding area on all four sides of an element. It is a shorthand for padding-top,
    /// padding-right, padding-bottom, and padding-left.
    member _.padding(value: ICssUnit) = h.MakeStyle("padding", asString value)
    /// Sets the padding area for vertical and horizontal axis.
    member _.padding(vertical: ICssUnit, horizontal: ICssUnit) =
        h.MakeStyle("padding",
            (asString vertical) + " " +
            (asString horizontal)
        )
    /// Sets the padding area on all four sides of an element. It is a shorthand for padding-top,
    /// padding-right, padding-bottom, and padding-left.
    member _.padding(top: ICssUnit, right: ICssUnit, bottom: ICssUnit, left: ICssUnit) =
        h.MakeStyle("padding",
            (asString top) + " " +
            (asString right) + " " +
            (asString bottom) + " " +
            (asString left)
        )
    /// Sets the height of the padding area on the bottom of an element.
    member _.paddingBottom(value: int) = h.MakeStyle("padding-bottom", asString value + "px")
    /// Sets the height of the padding area on the bottom of an element.
    member _.paddingBottom(value: ICssUnit) = h.MakeStyle("padding-bottom", asString value)
    /// Sets the width of the padding area to the left of an element.
    member _.paddingLeft(value: int) = h.MakeStyle("padding-left", asString value + "px")
    /// Sets the width of the padding area to the left of an element.
    member _.paddingLeft(value: ICssUnit) = h.MakeStyle("padding-left", asString value)
    /// Sets the width of the padding area on the right of an element.
    member _.paddingRight(value: int) = h.MakeStyle("padding-right", asString value + "px")
    /// Sets the width of the padding area on the right of an element.
    member _.paddingRight(value: ICssUnit) = h.MakeStyle("padding-right", asString value)
    /// Sets the height of the padding area on the top of an element.
    member _.paddingTop(value: int) = h.MakeStyle("padding-top", asString value + "px")
    /// Sets the height of the padding area on the top of an element.
    member _.paddingTop(value: ICssUnit) = h.MakeStyle("padding-top", asString value)

    /// Sets the flex shrink factor of a flex item. If the size of all flex items is larger than
    /// the flex container, items shrink to fit according to flex-shrink.
    member _.flexShrink(value: int) = h.MakeStyle("flex-shrink", asString value)
    /// Sets the initial main size of a flex item. It sets the size of the content box unless
    /// otherwise set with box-sizing.
    member _.flexBasis (value: int) = h.MakeStyle("flex-basis", asString value + "px")
    /// Sets the initial main size of a flex item. It sets the size of the content box unless
    /// otherwise set with box-sizing.
    member _.flexBasis (value: ICssUnit) = h.MakeStyle("flex-basis", asString value)
    /// Sets the flex grow factor of a flex item main size. It specifies how much of the remaining
    /// space in the flex container should be assigned to the item (the flex grow factor).
    member _.flexGrow (value: int) = h.MakeStyle("flex-grow", asString value)
    /// Shorthand of flex-grow, flex-shrink and flex-basis
    member _.flex (grow: int, ?shrink: int, ?basis: ICssUnit) = h.MakeStyle("flex", asString grow + " " + asString shrink + " " + asString basis)
    /// Shorthand of flex-grow, flex-shrink and flex-basis
    member _.flex (value: string) = h.MakeStyle("flex", value)

    /// Sets the width of each individual grid column in pixels.
    ///
    /// **CSS**
    /// ```css
    /// grid-template-columns: 100px 200px 100px;
    /// ```
    /// **F#**
    /// ```f#
    /// gridTemplateColumns: [100; 200; 100]
    /// ```
    member _.gridTemplateColumns(value: int seq) =
        let addPixels = fun x -> x + "px"
        h.MakeStyle("grid-template-columns", value |> Seq.map (asString >> addPixels) |> String.concat " ")
    /// Sets the width of each individual grid column.
    ///
    /// **CSS**
    /// ```css
    /// grid-template-columns: 1fr 1fr 2fr;
    /// ```
    /// **F#**
    /// ```f#
    /// gridTemplateColumns: [length.fr 1; length.fr 1; length.fr 2]
    /// ```
    member _.gridTemplateColumns(value: ICssUnit seq) =
        h.MakeStyle("grid-template-columns", value |> Seq.map asString |> String.concat " ")
    /// Sets the width of each individual grid column. It can also name the lines between them
    /// There can be multiple names for the same line
    ///
    /// **CSS**
    /// ```css
    /// grid-template-columns: [first-line] auto [first-line-end second-line-start] 100px [second-line-end];
    /// ```
    /// **F#**
    /// ```f#
    /// style.gridTemplateColumns [
    ///     grid.namedLine "first-line"
    ///     grid.templateWidth length.auto
    ///     grid.namedLines ["first-line-end second-line-start"]
    ///     grid.templateWidth 100
    ///     grid.namedLine "second-line-end"
    /// ]
    /// ```
    member _.gridTemplateColumns(value: IGridTemplateItem seq) =
        h.MakeStyle("grid-template-columns", value |> Seq.map asString |> String.concat " ")
    /// Sets the width of a number of grid columns to the defined width, as well as naming the lines between them
    ///
    /// **CSS**
    /// ```css
    /// grid-template-columns: repeat(3, 1fr [col-start]);
    /// ```
    /// **F#**
    /// ```f#
    /// style.gridTemplateColumns (3, length.fr 1, "col-start")
    /// ```
    member _.gridTemplateColumns(count: int, size: ICssUnit, ?areaName: string) =
        let areaName = match areaName with Some n -> " [" + n + "]" | None -> ""
        h.MakeStyle("grid-template-columns",
            "repeat(" +
            (asString count) + ", " +
            (asString size) + areaName + ")"
        )
    /// Sets the width of a number of grid rows to the defined width
    ///
    /// **CSS**
    /// ```css
    /// grid-template-rows: 100px 200px 100px;
    /// ```
    /// **F#**
    /// ```f#
    /// style.gridTemplateRows [100, 200, 100]
    /// ```
    member _.gridTemplateRows(value: int seq) =
        let addPixels = (fun x -> x + "px")
        h.MakeStyle("grid-template-rows", value |> Seq.map (asString >> addPixels) |> String.concat " ")
    /// Sets the width of a number of grid rows to the defined width
    ///
    /// **CSS**
    /// ```css
    /// grid-template-rows: 1fr 10% 250px auto;
    /// ```
    /// **F#**
    /// ```f#
    /// style.gridTemplateRows [length.fr 1; length.percent 10; length.px 250; length.auto]
    /// ```
    member _.gridTemplateRows(value: ICssUnit seq) =
        h.MakeStyle("grid-template-rows", value |> Seq.map asString |> String.concat " ")
    /// Sets the width of a number of grid rows to the defined width as well as naming the spaces between
    ///
    /// **CSS**
    /// ```css
    /// grid-template-rows: [row-1-start] 1fr [row-1-end row-2-start] 1fr [row-2-end];
    /// ```
    /// **F#**
    /// ```f#
    /// style.gridTemplateRows [
    ///     grid.namedLine "row-1-start"
    ///     grid.templateWidth (length.fr 1)
    ///     grid.namedLines ["row-1-end"; "row-2-start"]
    ///     grid.templateWidth (length.fr 1)
    ///     grid.namedLine "row-2-end"
    /// ]
    /// ```
    member _.gridTemplateRows(value: IGridTemplateItem seq) =
        h.MakeStyle("grid-template-rows", value |> Seq.map asString |> String.concat " ")
    /// Sets the width of a number of grid rows to the defined width
    ///
    /// **CSS**
    /// ```css
    /// grid-template-rows: repeat(3, 10%);
    /// ```
    /// **F#**
    /// ```f#
    /// style.gridTemplateRows (3, length.percent 10)
    /// ```
    member _.gridTemplateRows(count: int, size: ICssUnit, ?areaName: string) =
        let areaName = match areaName with Some n -> " [" + n + "]" | None -> ""
        h.MakeStyle("grid-template-rows",
            "repeat("+
            (asString count) + ", " +
            (asString size) + areaName + ")"
        )
    /// 2D representation of grid layout as blocks with names
    ///
    /// **CSS**
    /// ```css
    /// grid-template-areas:
    ///     'header header header header'
    ///     'nav nav . sidebar'
    ///     'footer footer footer footer';
    /// ```
    /// **F#**
    /// ```f#
    /// style.gridTemplateAreas [
    ///     ["header"; "header"; "header"; "header" ]
    ///     ["nav"   ; "nav"   ; "."     ; "sidebar"]
    ///     ["footer"; "footer"; "footer"; "footer" ]
    /// ]
    /// ```
    member _.gridTemplateAreas(value: string list list) =
        let wrapLine = (fun x -> "'" + x + "'")
        let lines = List.map (String.concat " " >> wrapLine) value
        let block = String.concat "\n" lines
        h.MakeStyle("grid-template-areas", block)
    /// 2D representation of grid layout as blocks with names
    ///
    /// **CSS**
    /// ```css
    /// grid-template-areas:
    ///     'header header header header'
    ///     'nav nav . sidebar'
    ///     'footer footer footer footer';
    /// ```
    /// **F#**
    /// ```f#
    /// style.gridTemplateAreas [|
    ///     [|"header"; "header"; "header"; "header" |]
    ///     [|"nav"   ; "nav"   ; "."     ; "sidebar"|]
    ///     [|"footer"; "footer"; "footer"; "footer" |]
    /// |]
    /// ```
    member _.gridTemplateAreas(value: string[][]) =
        let wrapLine = (fun x -> "'" + x + "'")
        let lines = Array.map (String.concat " " >> wrapLine) value
        let block = String.concat "\n" lines
        h.MakeStyle("grid-template-areas", block)
    /// One-dimensional alternative to the nested list. For column-based layouts
    ///
    /// **CSS**
    /// ```css
    /// grid-template-areas: 'first second third fourth';
    /// ```
    /// **F#**
    /// ```f#
    /// style.gridTemplateAreas ["first"; "second"; "third"; "fourth"]
    /// ```
    member _.gridTemplateAreas(value: string seq) =
        let block = String.concat " " value
        h.MakeStyle("grid-template-areas", "'" + block + "'")
    /// Specifies the size of the grid lines. You can think of it like
    /// setting the width of the gutters between the columns.
    ///
    /// **CSS**
    /// ```css
    /// column-gap: 10px;
    /// ```
    /// **F#**
    /// ```f#
    /// style.columnGap 10
    /// ```
    member _.columnGap(value: int) =
        h.MakeStyle("column-gap", asString value + "px")
    /// Specifies the size of the grid lines. You can think of it like
    /// setting the width of the gutters between the columns.
    ///
    /// **CSS**
    /// ```css
    /// column-gap: 1em;
    /// ```
    /// **F#**
    /// ```f#
    /// style.columnGap (length.em 1)
    /// ```
    member _.columnGap(value: ICssUnit) =
        h.MakeStyle("column-gap", asString value)
    /// Specifies the size of the grid lines. You can think of it like
    /// setting the width of the gutters between the rows.
    ///
    /// **CSS**
    /// ```css
    /// row-gap: 10px;
    /// ```
    /// **F#**
    /// ```f#
    /// style.rowGap 10
    /// ```
    member _.rowGap(value: int) =
        h.MakeStyle("row-gap", asString value + "px")
    /// Specifies the size of the grid lines. You can think of it like
    /// setting the width of the gutters between the rows.
    ///
    /// **CSS**
    /// ```css
    /// row-gap: 1em;
    /// ```
    /// **F#**
    /// ```f#
    /// style.rowGap (length.em 1)
    /// ```
    member _.rowGap(value: ICssUnit) =
        h.MakeStyle("row-gap", asString value)
    /// Specifies the size of the grid lines. You can think of it like
    /// setting the width of the gutters between the rows/columns.
    ///
    /// _Shorthand for `rowGap` and `columnGap`_
    ///
    /// **CSS**
    /// ```css
    /// gap: 1em 2em;
    /// ```
    /// **F#**
    /// ```f#
    /// style.gap (length.em 1, length.em 2)
    /// ```
    member _.gap(rowGap: ICssUnit, columnGap: ICssUnit) =
        h.MakeStyle("gap",
            (asString rowGap) + " " +
            (asString columnGap)
        )
    member _.gap(rowColumnGap: ICssUnit) =
        h.MakeStyle("gap",
            (asString rowColumnGap) + " " +
            (asString rowColumnGap)
        )
    /// Sets where an item in the grid starts
    /// The value can be one of the following options:
    /// - a named line
    /// - a numbered line
    /// - span until a named line was hit
    /// - span over a specified number of lines
    ///
    ///
    /// When there are multiple named lines with the same name, you can specify which one by count
    ///
    /// **CSS**
    /// ```css
    /// grid-column-start: col 2;
    /// ```
    /// **F#**
    /// ```f#
    /// style.gridColumnStart ("col", 2)
    /// ```
    member _.gridColumnStart(value: string, ?count: int) =
        h.MakeStyle("grid-column-start", asString value + " " + (asString count))
    /// Sets where an item in the grid starts
    /// The value can be one of the following options:
    /// - a named line
    /// - a numbered line
    /// - span until a named line was hit
    /// - span over a specified number of lines
    ///
    ///
    /// **CSS**
    /// ```css
    /// grid-column-start: 2;
    /// ```
    /// **F#**
    /// ```f#
    /// style.gridColumnStart 2
    /// ```
    member _.gridColumnStart(value: int) = h.MakeStyle("grid-column-start", asString value)
    /// Sets where an item in the grid starts
    /// The value can be one of the following options:
    /// - a named line
    /// - a numbered line
    /// - span until a named line was hit
    /// - span over a specified number of lines
    ///
    ///
    /// **CSS**
    /// ```css
    /// grid-column-start: span odd-col;
    /// ```
    /// **F#**
    /// ```f#
    /// style.gridColumnStart (gridColumn.span "odd-col")
    /// ```
    member _.gridColumnStart(value: IGridSpan) = h.MakeStyle("grid-column-start", asString value)
    /// Sets where an item in the grid ends
    /// The value can be one of the following options:
    /// - a named line
    /// - a numbered line
    /// - span until a named line was hit
    /// - span over a specified number of lines
    ///
    ///
    /// _When there are multiple named lines with the same name, you can specify which one by count_
    ///
    /// **CSS**
    /// ```css
    /// grid-column-end: odd-col 2;
    /// ```
    /// **F#**
    /// ```f#
    /// style.gridColumnEnd ("odd-col", 2)
    /// ```
    member _.gridColumnEnd(value: string, ?count: int) =
        h.MakeStyle("grid-column-end", asString value + " " + (asString count))
    /// Sets where an item in the grid ends
    /// The value can be one of the following options:
    /// - a named line
    /// - a numbered line
    /// - span until a named line was hit
    /// - span over a specified number of lines
    ///
    ///
    /// **CSS**
    /// ```css
    /// grid-column-end: 2;
    /// ```
    /// **F#**
    /// ```f#
    /// style.gridColumnEnd 2
    /// ```
    member _.gridColumnEnd(value: int) = h.MakeStyle("grid-column-end", asString value)
    /// Sets where an item in the grid ends
    /// The value can be one of the following options:
    /// - a named line
    /// - a numbered line
    /// - span until a named line was hit
    /// - span over a specified number of lines
    ///
    ///
    /// **CSS**
    /// ```css
    /// grid-column-end: span 2;
    /// ```
    /// **F#**
    /// ```f#
    /// style.gridColumnEnd (gridColumn.span 2)
    /// ```
    member _.gridColumnEnd(value: IGridSpan) = h.MakeStyle("grid-column-end", asString value)
    /// Sets where an item in the grid starts
    /// The value can be one of the following options:
    /// - a named line
    /// - a numbered line
    /// - span until a named line was hit
    /// - span over a specified number of lines
    ///
    ///
    /// **CSS**
    /// ```css
    /// grid-row-start: col 2;
    /// ```
    /// **F#**
    /// ```f#
    /// style.gridRowStart ("col", 2)
    /// ```
    member _.gridRowStart(value: string, ?count: int) =
        h.MakeStyle("grid-row-start", asString value + " " + (asString count))
    /// Sets where an item in the grid starts
    /// The value can be one of the following options:
    /// - a named line
    /// - a numbered line
    /// - span until a named line was hit
    /// - span over a specified number of lines
    ///
    ///
    /// **CSS**
    /// ```css
    /// grid-row-start: 2;
    /// ```
    /// **F#**
    /// ```f#
    /// style.gridRowStart 2
    /// ```
    member _.gridRowStart(value: int) = h.MakeStyle("grid-row-start", asString value)
    /// Sets where an item in the grid starts
    /// The value can be one of the following options:
    /// - a named line
    /// - a numbered line
    /// - span until a named line was hit
    /// - span over a specified number of lines
    ///
    ///
    /// **CSS**
    /// ```css
    /// grid-row-start: span odd-col;
    /// ```
    /// **F#**
    /// ```f#
    /// style.gridRowStart (gridRow.span "odd-col")
    /// ```
    member _.gridRowStart(value: IGridSpan) = h.MakeStyle("grid-row-start", asString value)
    /// Sets where an item in the grid ends
    /// The value can be one of the following options:
    /// - a named line
    /// - a numbered line
    /// - span until a named line was hit
    /// - span over a specified number of lines
    ///
    ///
    /// _When there are multiple named lines with the same name, you can specify which one by count_
    ///
    /// **CSS**
    /// ```css
    /// grid-row-end: odd-col 2;
    /// ```
    /// **F#**
    /// ```f#
    /// style.gridRowEnd ("odd-col", 2)
    /// ```
    member _.gridRowEnd(value: string, ?count: int) =
        h.MakeStyle("grid-row-end", asString value + " " + (asString count))
    /// Sets where an item in the grid ends
    /// The value can be one of the following options:
    /// - a named line
    /// - a numbered line
    /// - span until a named line was hit
    /// - span over a specified number of lines
    ///
    ///
    /// **CSS**
    /// ```css
    /// grid-row-end: 2;
    /// ```
    /// **F#**
    /// ```f#
    /// style.gridRowEnd 2
    /// ```
    member _.gridRowEnd(value: int) = h.MakeStyle("grid-row-end", asString value)
    /// Sets where an item in the grid ends
    /// The value can be one of the following options:
    /// - a named line
    /// - a numbered line
    /// - span until a named line was hit
    /// - span over a specified number of lines
    ///
    ///
    /// **CSS**
    /// ```css
    /// grid-row-end: span 2;
    /// ```
    /// **F#**
    /// ```f#
    /// style.gridRowEnd (gridRow.span 2)
    /// ```
    member _.gridRowEnd(value: IGridSpan) = h.MakeStyle("grid-row-end", asString value)
    /// Determines a grid item’s location within the grid by referring to specific grid lines.
    /// start is the line where the item begins, end' is the line where it ends.
    /// They can be one of the following options:
    /// - a named line
    /// - a numbered line
    /// - span until a named line was hit
    /// - span over a specified number of lines
    ///
    ///
    /// _Shorthand for `gridColumnStart` and `gridColumnEnds`_
    ///
    /// **CSS**
    /// ```css
    /// grid-column: col-2 / col-4;
    /// ```
    /// **F#**
    /// ```f#
    /// style.gridColumn ("col-2", "col-4")
    /// ```
    member _.gridColumn(start: string, end': string) =
        h.MakeStyle("grid-column", start + " / " + end')
    /// Determines a grid item’s location within the grid by referring to specific grid lines.
    /// start is the line where the item begins, end' is the line where it ends.
    /// They can be one of the following options:
    /// - a named line
    /// - a numbered line
    /// - span until a named line was hit
    /// - span over a specified number of lines
    ///
    ///
    /// _Shorthand for `gridColumnStart` and `gridColumnEnds`_
    ///
    /// **CSS**
    /// ```css
    /// grid-column: 1 / 3;
    /// ```
    /// **F#**
    /// ```f#
    /// style.gridColumn (1, 3)
    /// ```
    member _.gridColumn(start: int, end': int) =
        h.MakeStyle("grid-column", (asString start) + " / " + (asString end'))
    /// Determines a grid item’s location within the grid by referring to specific grid lines.
    /// start is the line where the item begins, end' is the line where it ends.
    /// They can be one of the following options:
    /// - a named line
    /// - a numbered line
    /// - span until a named line was hit
    /// - span over a specified number of lines
    ///
    ///
    /// _Shorthand for `gridColumnStart` and `gridColumnEnds`_
    ///
    /// **CSS**
    /// ```css
    /// grid-column: span 2 / span 3;
    /// ```
    /// **F#**
    /// ```f#
    /// style.gridColumn (gridColumn.span 2, gridColumn.span 3)
    /// ```
    member _.gridColumn(start: IGridSpan, end': IGridSpan) =
        h.MakeStyle("grid-column", (asString start) + " / " + (asString end'))
    /// Determines a grid item’s location within the grid by referring to specific grid lines.
    /// start is the line where the item begins, end' is the line where it ends.
    /// They can be one of the following options:
    /// - a named line
    /// - a numbered line
    /// - span until a named line was hit
    /// - span over a specified number of lines
    ///
    ///
    /// _Shorthand for `gridRowStart` and `gridRowEnds`_
    ///
    /// **CSS**
    /// ```css
    /// grid-row: row-2 / row-4;
    /// ```
    /// **F#**
    /// ```f#
    /// style.gridRow ("row-2", "row-4")
    /// ```
    member _.gridRow(start: string, end': string) =
        h.MakeStyle("grid-row", start + " / " + end')
    /// Determines a grid item’s location within the grid by referring to specific grid lines.
    /// start is the line where the item begins, end' is the line where it ends.
    /// They can be one of the following options:
    /// - a named line
    /// - a numbered line
    /// - span until a named line was hit
    /// - span over a specified number of lines
    ///
    ///
    /// _Shorthand for `gridRowStart` and `gridRowEnds`_
    ///
    /// **CSS**
    /// ```css
    /// grid-row: 2 / 4;
    /// ```
    /// **F#**
    /// ```f#
    /// style.gridRow (2, 4)
    /// ```
    member _.gridRow(start: int, end': int) =
        h.MakeStyle("grid-row", (asString start) + " / " + (asString end'))
    /// Determines a grid item’s location within the grid by referring to specific grid lines.
    /// start is the line where the item begins, end' is the line where it ends.
    /// They can be one of the following options:
    /// - a named line
    /// - a numbered line
    /// - span until a named line was hit
    /// - span over a specified number of lines
    ///
    ///
    /// _Shorthand for `gridRowStart` and `gridRowEnds`_
    ///
    /// **CSS**
    /// ```css
    /// grid-row: span 2 / span 3;
    /// ```
    /// **F#**
    /// ```f#
    /// style.gridRow (gridRow.span 2, gridRow.span 3)
    /// ```
    member _.gridRow(start: IGridSpan, end': IGridSpan) =
        h.MakeStyle("grid-row", (asString start) + " / " + (asString end'))
    /// Sets the named grid area the item is placed in
    ///
    /// **CSS**
    /// ```css
    /// grid-area: header;
    /// ```
    /// **F#**
    /// ```f#
    /// style.gridArea "header"
    /// ```
    member _.gridArea(value: string) =
        h.MakeStyle("grid-area", asString value)
    /// Shorthand for `grid-template-areas`, `grid-template-columns` and `grid-template-rows`.
    ///
    /// Documentation: https://developer.mozilla.org/en-US/docs/Web/CSS/grid-template
    ///
    /// **CSS**
    /// ```css
    /// grid-template:  [header-top] 'a a a'      [header-bottom]
    ///                   [main-top] 'b b b' 1fr  [main-bottom]
    ///                              / auto 1fr auto;
    /// ```
    /// **F#**
    /// ```f#
    /// style.gridTemplate "[header-top] 'a a a'      [header-bottom] " +
    ///                      "[main-top] 'b b b' 1fr  [main-bottom] " +
    ///                                "/ auto 1fr auto"
    /// ```
    member _.gridTemplate(value: string) =
        h.MakeStyle("grid-template", asString value)
    member _.transition(value: string) =
        h.MakeStyle("transition", value)
    /// Sets the length of time a transition animation should take to complete. By default, the
    /// value is 0s, meaning that no animation will occur.
    member _.transitionDuration(timespan: TimeSpan) =
        h.MakeStyle("transition-duration", asString timespan.TotalMilliseconds + "ms")
    /// Sets the length of time a transition animation should take to complete. By default, the
    /// value is 0s, meaning that no animation will occur.
    member _.transitionDurationSeconds(n: float) =
        h.MakeStyle("transition-duration", (asString n) + "s")
    /// Sets the length of time a transition animation should take to complete. By default, the
    /// value is 0s, meaning that no animation will occur.
    member _.transitionDurationMilliseconds(n: float) =
        h.MakeStyle("transition-duration", (asString n) + "ms")
    /// Sets the length of time a transition animation should take to complete. By default, the
    /// value is 0s, meaning that no animation will occur.
    member _.transitionDurationSeconds(n: int) =
        h.MakeStyle("transition-duration", (asString n) + "s")
    /// Sets the length of time a transition animation should take to complete. By default, the
    /// value is 0s, meaning that no animation will occur.
    member _.transitionDurationMilliseconds(n: int) =
        h.MakeStyle("transition-duration", (asString n) + "ms")
    /// Specifies the duration to wait before starting a property's transition effect when its value changes.
    member _.transitionDelay(timespan: TimeSpan) =
        h.MakeStyle("transition-delay", asString timespan.TotalMilliseconds + "ms")
    /// Specifies the duration to wait before starting a property's transition effect when its value changes.
    member _.transitionDelaySeconds(n: float) =
        h.MakeStyle("transition-delay", (asString n) + "s")
    /// Specifies the duration to wait before starting a property's transition effect when its value changes.
    member _.transitionDelayMilliseconds(n: float) =
        h.MakeStyle("transition-delay", (asString n) + "ms")
    /// Specifies the duration to wait before starting a property's transition effect when its value changes.
    member _.transitionDelaySeconds(n: int) =
        h.MakeStyle("transition-delay", (asString n) + "s")
    /// Specifies the duration to wait before starting a property's transition effect when its value changes.
    member _.transitionDelayMilliseconds(n: int) =
        h.MakeStyle("transition-delay", (asString n) + "ms")
    /// Sets the CSS properties to which a transition effect should be applied.
    member _.transitionProperty ([<ParamArray>] properties: ITransitionProperty[]) =
        h.MakeStyle("transition-property", String.concat "," (properties |> Array.map asString))
    /// Sets the CSS properties to which a transition effect should be applied.
    member _.transitionProperty (properties: ITransitionProperty list) =
        h.MakeStyle("transition-property", String.concat "," (properties |> List.map asString))
    /// Sets the CSS properties to which a transition effect should be applied.
    member _.transitionProperty (property: ITransitionProperty) =
        h.MakeStyle("transition-property", asString property)
    /// Sets the CSS properties to which a transition effect should be applied.
    member _.transitionProperty (property: string) =
        h.MakeStyle("transition-property", property)

    /// Sets the size of the font.
    ///
    /// This property is also used to compute the size of em, ex, and other relative <length> units.
    member _.fontSize(size: int) = h.MakeStyle("font-size", asString size + "px")
    /// Sets the size of the font.
    ///
    /// This property is also used to compute the size of em, ex, and other relative <length> units.
    member _.fontSize(size: ICssUnit) = h.MakeStyle("font-size", asString size)
    /// Specifies the height of a text lines.
    ///
    /// This property is also used to compute the size of em, ex, and other relative <length> units.
    ///
    /// Note: Negative values are not allowed.
    member _.lineHeight(size: int) = h.MakeStyle("line-height", asString size + "px")
    /// Specifies the height of a text lines.
    ///
    /// This property is also used to compute the size of em, ex, and other relative <length> units.
    ///
    /// Note: Negative values are not allowed.
    member _.lineHeight(size: ICssUnit) = h.MakeStyle("line-height", asString size)
    /// Sets the background color of an element.
    member _.backgroundColor (color: string) = h.MakeStyle("background-color", color)
    /// Sets the color of the insertion caret, the visible marker where the next character typed will be inserted.
    ///
    /// This is sometimes referred to as the text input cursor. The caret appears in elements such as <input> or
    /// those with the contenteditable attribute. The caret is typically a thin vertical line that flashes to
    /// help make it more noticeable. By default, it is black, but its color can be altered with this property.
    member _.caretColor (color: string) = h.MakeStyle("caret-color", color)
    /// Sets the foreground color value of an element's text and text decorations, and sets the
    /// `currentcolor` value. `currentcolor` may be used as an indirect value on other properties
    /// and is the default for other color properties, such as border-color.
    member _.color (color: string) = h.MakeStyle("color", color)
    /// Specifies the vertical position of a positioned element. It has no effect on non-positioned elements.
    member _.top(value: int) = h.MakeStyle("top", asString value + "px")
    /// Specifies the vertical position of a positioned element. It has no effect on non-positioned elements.
    member _.top(value: ICssUnit) = h.MakeStyle("top", asString value)
    /// Specifies the vertical position of a positioned element. It has no effect on non-positioned elements.
    member _.bottom(value: int) = h.MakeStyle("bottom", asString value + "px")
    /// Specifies the vertical position of a positioned element. It has no effect on non-positioned elements.
    member _.bottom(value: ICssUnit) = h.MakeStyle("bottom", asString value)
    /// Specifies the horizontal position of a positioned element. It has no effect on non-positioned elements.
    member _.left(value: int) = h.MakeStyle("left", asString value + "px")
    /// Specifies the horizontal position of a positioned element. It has no effect on non-positioned elements.
    member _.left(value: ICssUnit) = h.MakeStyle("left", asString value)
    /// Specifies the horizontal position of a positioned element. It has no effect on non-positioned elements.
    member _.right(value: int) = h.MakeStyle("right", asString value + "px")
    /// Specifies the horizontal position of a positioned element. It has no effect on non-positioned elements.
    member _.right(value: ICssUnit) = h.MakeStyle("right", asString value)

    /// Sets an element's bottom border. It sets the values of border-bottom-width,
    /// border-bottom-style and border-bottom-color.
    member _.borderBottom(width: ICssUnit, style: IBorderStyle, color: string) =
        h.MakeStyle("border-bottom",
            (asString width) + " " +
            (asString style) + " " +
            color
        )

    /// The outline-offset property adds space between an outline and the edge or border of an element.
    ///
    /// The space between an element and its outline is transparent.
    ///
    /// Outlines differ from borders in three ways:
    ///
    ///  - An outline is a line drawn around elements, outside the border edge
    ///  - An outline does not take up space
    ///  - An outline may be non-rectangular
    ///
    member _.outlineOffset (offset:int) =
        h.MakeStyle("outline-width", (asString offset) + "px")

    /// The outline-offset property adds space between an outline and the edge or border of an element.
    ///
    /// The space between an element and its outline is transparent.
    ///
    /// Outlines differ from borders in three ways:
    ///
    ///  - An outline is a line drawn around elements, outside the border edge
    ///  - An outline does not take up space
    ///  - An outline may be non-rectangular
    ///
    member _.outlineOffset (offset: ICssUnit) =
        h.MakeStyle("outline-width", asString offset)

    /// An outline is a line that is drawn around elements (outside the borders) to make the element "stand out".
    ///
    /// The `outline-color` property specifies the color of an outline.

    /// **Note**: Always declare the outline-style property before the outline-color property. An element must have an outline before you change the color of it.
    member _.outlineColor (color: string) =
        h.MakeStyle("outline-color", color)

    /// Set an element's left border.
    member _.borderLeft(width: ICssUnit, style: IBorderStyle, color: string) =
        h.MakeStyle("border-left",
            (asString width) + " " +
            (asString style) + " " +
            color
        )
    /// Set an element's right border.
    member _.borderRight(width: ICssUnit, style: IBorderStyle, color: string) =
        h.MakeStyle("border-right",
            (asString width) + " " +
            (asString style) + " " +
            color
        )
    /// Set an element's top border.
    member _.borderTop(width: ICssUnit, style: IBorderStyle, color: string) =
        h.MakeStyle("border-top",
            (asString width) + " " +
            (asString style) + " " +
            color
        )
    /// Sets the line style of an element's bottom border.
    member _.borderBottomStyle(style: IBorderStyle) = h.MakeStyle("border-bottom-style", asString style)
    /// Sets the width of the bottom border of an element.
    member _.borderBottomWidth (width: int) = h.MakeStyle("border-bottom-width", asString width + "px")
    /// Sets the width of the bottom border of an element.
    member _.borderBottomWidth (width: ICssUnit) = h.MakeStyle("border-bottom-width", asString width)
    /// Sets the color of an element's bottom border.
    ///
    /// It can also be set with the shorthand CSS properties border-color or border-bottom.
    member _.borderBottomColor (color: string) = h.MakeStyle("border-bottom-color", color)
    /// Sets the line style of an element's top border.
    member _.borderTopStyle(style: IBorderStyle) = h.MakeStyle("border-top-style", asString style)
    /// Sets the width of the top border of an element.
    member _.borderTopWidth (width: int) = h.MakeStyle("border-top-width", asString width + "px")
    /// Sets the width of the top border of an element.
    member _.borderTopWidth (width: ICssUnit) = h.MakeStyle("border-top-width", asString width)
    /// Sets the color of an element's top border.
    ///
    /// It can also be set with the shorthand CSS properties border-color or border-bottom.
    member _.borderTopColor (color: string) = h.MakeStyle("border-top-color", color)
    /// Sets the line style of an element's right border.
    member _.borderRightStyle(style: IBorderStyle) = h.MakeStyle("border-right-style", asString style)
    /// Sets the width of the right border of an element.
    member _.borderRightWidth (width: int) = h.MakeStyle("border-right-width", asString width + "px")
    /// Sets the width of the right border of an element.
    member _.borderRightWidth (width: ICssUnit) = h.MakeStyle("border-right-width", asString width)
    /// Sets the color of an element's right border.
    ///
    /// It can also be set with the shorthand CSS properties border-color or border-bottom.
    member _.borderRightColor (color: string) = h.MakeStyle("border-right-color", color)
    /// Sets the line style of an element's left border.
    member _.borderLeftStyle(style: IBorderStyle) = h.MakeStyle("border-left-style", asString style)
    /// Sets the width of the left border of an element.
    member _.borderLeftWidth (width: int) = h.MakeStyle("border-left-width", asString width + "px")
    /// Sets the width of the left border of an element.
    member _.borderLeftWidth (width: ICssUnit) = h.MakeStyle("border-left-width", asString width)
    /// Sets the color of an element's left border.
    ///
    /// It can also be set with the shorthand CSS properties border-color or border-bottom.
    member _.borderLeftColor (color: string) = h.MakeStyle("border-left-color", color)
    /// Sets an element's border.
    ///
    /// It sets the values of border-width, border-style, and border-color.
    member _.border(width: ICssUnit, style: IBorderStyle, color: string) =
        h.MakeStyle("border",
            (asString width) + " " +
            (asString style) + " " +
            color
        )
    /// Sets an element's border.
    ///
    /// It sets the values of border-width, border-style, and border-color.
    member _.border(width: string, style: IBorderStyle, color: string) =
        h.MakeStyle("border",
            width + " " +
            (asString style) + " " +
            color
        )
    /// Sets the color of an element's border.
    member _.borderColor (color: string) = h.MakeStyle("border-color", color)
    /// Rounds the corners of an element's outer border edge. You can set a single radius to make
    /// circular corners, or two radii to make elliptical corners.
    member _.borderRadius (radius: int) = h.MakeStyle("border-radius", asString radius + "px")
    /// Rounds the corners of an element's outer border edge. You can set a single radius to make
    /// circular corners, or two radii to make elliptical corners.
    member _.borderRadius (radius: ICssUnit) = h.MakeStyle("border-radius", asString radius)
    /// Sets the width of an element's border.
    member _.borderWidth (width: int) = h.MakeStyle("border-width", asString width + "px")
    /// Sets the width of an element's border.
    member _.borderWidth (top: ICssUnit, ?right: ICssUnit) =
        h.MakeStyle("border-width",
            asString top + (match right with Some x -> " " + asString x | None -> ""))
    /// Sets the width of an element's border.
    member _.borderWidth (top: ICssUnit, right: ICssUnit, bottom: ICssUnit, ?left: ICssUnit) =
        h.MakeStyle("border-width",
            (asString top) + " " +
            (asString right) + " " +
            (asString bottom) +
            (match left with Some x -> " " + asString x | None -> ""))
    /// Sets one or more animations to apply to an element. Each name is an @keyframes at-rule that
    /// sets the property values for the animation sequence.
    member _.animationName(keyframeName: string) = h.MakeStyle("animation-name", keyframeName)
    /// Sets the length of time that an animation takes to complete one cycle.
    member _.animationDuration(timespan: TimeSpan) = h.MakeStyle("animation-duration", (asString timespan.TotalMilliseconds) + "ms")
    /// Sets the length of time that an animation takes to complete one cycle.
    member _.animationDuration(seconds: int) = h.MakeStyle("animation-duration", (asString seconds) + "s")
    /// Sets when an animation starts.
    ///
    /// The animation can start later, immediately from its beginning, or immediately and partway through the animation.
    member _.animationDelay(timespan: TimeSpan) = h.MakeStyle("animation-delay", (asString timespan.TotalMilliseconds) + "ms")
    /// Sets when an animation starts.
    ///
    /// The animation can start later, immediately from its beginning, or immediately and partway through the animation.
    member _.animationDelay(seconds: int) = h.MakeStyle("animation-delay", (asString seconds) + "s")
    /// The number of times the animation runs.
    member _.animationDurationCount(count: int) = h.MakeStyle("animation-duration-count", asString count)
    /// Sets the font family for the font specified in a @font-face rule.
    member _.fontFamily (family: string) = h.MakeStyle("font-family", family)
    /// Sets the color of decorations added to text by text-decoration-line.
    member _.textDecorationColor(color: string) = h.MakeStyle("text-decoration-color", color)
    /// Sets the length of empty space (indentation) that is put before lines of text in a block.
    member _.textIndent(value: int) = h.MakeStyle("text-indent", asString value)
    /// Sets the length of empty space (indentation) that is put before lines of text in a block.
    member _.textIndent(value: string) = h.MakeStyle("text-indent", asString value)
    /// Sets the opacity of an element.
    ///
    /// Opacity is the degree to which content behind an element is hidden, and is the opposite of transparency.
    member _.opacity(value: double) = h.MakeStyle("opacity", asString value)
    /// Sets the minimum width of an element.
    ///
    /// It prevents the used value of the width property from becoming smaller than the value specified for min-width.
    member _.minWidth (value: int) = h.MakeStyle("min-width", asString value + "px")
    /// Sets the minimum width of an element.
    ///
    /// It prevents the used value of the width property from becoming smaller than the value specified for min-width.
    member _.minWidth (value: ICssUnit) = h.MakeStyle("min-width", asString value)
    /// Sets the minimum width of an element.
    ///
    /// It prevents the used value of the width property from becoming smaller than the value specified for min-width.
    member _.minWidth (value: string) = h.MakeStyle("min-width", asString value)
    /// Sets the maximum width of an element.
    ///
    /// It prevents the used value of the width property from becoming larger than the value specified by max-width.
    member _.maxWidth (value: int) = h.MakeStyle("max-width", asString value + "px")
    /// Sets the maximum width of an element.
    ///
    /// It prevents the used value of the width property from becoming larger than the value specified by max-width.
    member _.maxWidth (value: ICssUnit) = h.MakeStyle("max-width", asString value)
    /// Sets the width of an element.
    ///
    /// By default, the property defines the width of the content area.
    member _.width (value: int) = h.MakeStyle("width", asString value + "px")
    /// Sets the width of an element.
    ///
    /// By default, the property defines the width of the content area.
    member _.width (value: ICssUnit) = h.MakeStyle("width", asString value)

    /// Sets one or more background images on an element.
    member _.backgroundImage (value: string) = h.MakeStyle("background-image", asString value)
    /// Short-hand for `style.backgroundImage(sprintf "url('%s')" value)` to set the backround image using a url.
    member _.backgroundImageUrl (value: string) = h.MakeStyle("background-image", "url('" + value + "')")

    /// Sets the color of an SVG shape.
    member _.fill (color: string) = h.MakeStyle("fill", color)

/// Contains a list of HTML5 colors from https://htmlcolorcodes.com/color-names/
module color =
    /// Creates a color from components [hue](https://en.wikipedia.org/wiki/Hue), [saturation](https://en.wikipedia.org/wiki/Colorfulness) and [lightness](https://en.wikipedia.org/wiki/Lightness) where hue is a number that goes from 0 to 360 and both
    /// the `saturation` and `lightness` go from 0 to 100 as they are percentages.
    let hsl (hue: float, saturation: float, lightness: float) =
        "hsl(" + (string hue) + "," + (string saturation) + "%," + (string lightness) + "%)"
    let rgb (r: int, g: int, b: int) =
        "rgb(" + (string r) + "," + (string g) + "," + (string b) + ")"
    let rgba (r: int, g: int, b: int, a: float) =
        "rgba(" + (string r) + "," + (string g) + "," + (string b) + "," + (string a) + ")"
    let [<Literal>] indianRed = "#CD5C5C"
    let [<Literal>] lightCoral = "#F08080"
    let [<Literal>] salmon = "#FA8072"
    let [<Literal>] darkSalmon = "#E9967A"
    let [<Literal>] lightSalmon = "#FFA07A"
    let [<Literal>] crimson = "#DC143C"
    let [<Literal>] red = "#FF0000"
    let [<Literal>] fireBrick = "#B22222"
    let [<Literal>] darkRed = "#8B0000"
    let [<Literal>] pink = "#FFC0CB"
    let [<Literal>] lightPink = "#FFB6C1"
    let [<Literal>] hotPink = "#FF69B4"
    let [<Literal>] deepPink = "#FF1493"
    let [<Literal>] mediumVioletRed = "#C71585"
    let [<Literal>] paleVioletRed = "#DB7093"
    let [<Literal>] coral = "#FF7F50"
    let [<Literal>] tomato = "#FF6347"
    let [<Literal>] orangeRed = "#FF4500"
    let [<Literal>] darkOrange = "#FF8C00"
    let [<Literal>] orange = "#FFA500"
    let [<Literal>] gold = "#FFD700"
    let [<Literal>] yellow = "#FFFF00"
    let [<Literal>] lightYellow = "#FFFFE0"
    let [<Literal>] limonChiffon = "#FFFACD"
    let [<Literal>] lightGoldenRodYellow = "#FAFAD2"
    let [<Literal>] papayaWhip = "#FFEFD5"
    let [<Literal>] moccasin = "#FFE4B5"
    let [<Literal>] peachPuff = "#FFDAB9"
    let [<Literal>] paleGoldenRod = "#EEE8AA"
    let [<Literal>] khaki = "#F0E68C"
    let [<Literal>] darkKhaki = "#BDB76B"
    let [<Literal>] lavender = "#E6E6FA"
    let [<Literal>] thistle = "#D8BFD8"
    let [<Literal>] plum = "#DDA0DD"
    let [<Literal>] violet = "#EE82EE"
    let [<Literal>] orchid = "#DA70D6"
    let [<Literal>] fuchsia = "#FF00FF"
    let [<Literal>] magenta = "#FF00FF"
    let [<Literal>] mediumOrchid = "#BA55D3"
    let [<Literal>] mediumPurple = "#9370DB"
    let [<Literal>] rebeccaPurple = "#663399"
    let [<Literal>] blueViolet = "#8A2BE2"
    let [<Literal>] darkViolet = "#9400D3"
    let [<Literal>] darkOrchid = "#9932CC"
    let [<Literal>] darkMagenta = "#8B008B"
    let [<Literal>] purple = "#800080"
    let [<Literal>] indigo = "#4B0082"
    let [<Literal>] slateBlue = "#6A5ACD"
    let [<Literal>] darkSlateBlue = "#483D8B"
    let [<Literal>] mediumSlateBlue = "#7B68EE"
    let [<Literal>] greenYellow = "#ADFF2F"
    let [<Literal>] chartreuse = "#7FFF00"
    let [<Literal>] lawnGreen = "#7CFC00"
    let [<Literal>] lime = "#00FF00"
    let [<Literal>] limeGreen = "#32CD32"
    let [<Literal>] paleGreen = "#98FB98"
    let [<Literal>] lightGreen = "#90EE90"
    let [<Literal>] mediumSpringGreen = "#00FA9A"
    let [<Literal>] springGreen = "#00FF7F"
    let [<Literal>] mediumSeaGreen = "#3CB371"
    let [<Literal>] seaGreen = "#2E8B57"
    let [<Literal>] forestGreen = "#228B22"
    let [<Literal>] green = "#008000"
    let [<Literal>] darkGreen = "#006400"
    let [<Literal>] yellowGreen = "#9ACD32"
    let [<Literal>] oliveDrab = "#6B8E23"
    let [<Literal>] olive = "#808000"
    let [<Literal>] darkOliveGreen = "#556B2F"
    let [<Literal>] mediumAquamarine = "#66CDAA"
    let [<Literal>] darkSeaGreen = "#8FBC8B"
    let [<Literal>] lightSeaGreen = "#20B2AA"
    let [<Literal>] darkCyan = "#008B8B"
    let [<Literal>] teal = "#008080"
    let [<Literal>] aqua = "#00FFFF"
    let [<Literal>] cyan = "#00FFFF"
    let [<Literal>] lightCyan = "#E0FFFF"
    let [<Literal>] paleTurqouise = "#AFEEEE"
    let [<Literal>] aquaMarine = "#7FFFD4"
    let [<Literal>] turqouise = "#AFEEEE"
    let [<Literal>] mediumTurqouise = "#48D1CC"
    let [<Literal>] darkTurqouise = "#00CED1"
    let [<Literal>] cadetBlue = "#5F9EA0"
    let [<Literal>] steelBlue = "#4682B4"
    let [<Literal>] lightSteelBlue = "#B0C4DE"
    let [<Literal>] powederBlue = "#B0E0E6"
    let [<Literal>] lightBlue = "#ADD8E6"
    let [<Literal>] skyBlue = "#87CEEB"
    let [<Literal>] lightSkyBlue = "#87CEFA"
    let [<Literal>] deepSkyBlue = "#00BFFF"
    let [<Literal>] dodgerBlue = "#1E90FF"
    let [<Literal>] cornFlowerBlue = "#6495ED"
    let [<Literal>] royalBlue = "#4169E1"
    let [<Literal>] blue = "#0000FF"
    let [<Literal>] mediumBlue = "#0000CD"
    let [<Literal>] darkBlue = "#00008B"
    let [<Literal>] navy = "#000080"
    let [<Literal>] midnightBlue = "#191970"
    let [<Literal>] cornSilk = "#FFF8DC"
    let [<Literal>] blanchedAlmond = "#FFEBCD"
    let [<Literal>] bisque = "#FFE4C4"
    let [<Literal>] navajoWhite = "#FFDEAD"
    let [<Literal>] wheat = "#F5DEB3"
    let [<Literal>] burlyWood = "#DEB887"
    let [<Literal>] tan = "#D2B48C"
    let [<Literal>] rosyBrown = "#BC8F8F"
    let [<Literal>] sandyBrown = "#F4A460"
    let [<Literal>] goldenRod = "#DAA520"
    let [<Literal>] darkGoldenRod = "#B8860B"
    let [<Literal>] peru = "#CD853F"
    let [<Literal>] chocolate = "#D2691E"
    let [<Literal>] saddleBrown = "#8B4513"
    let [<Literal>] sienna = "#A0522D"
    let [<Literal>] brown = "#A52A2A"
    let [<Literal>] maroon = "#A52A2A"
    let [<Literal>] white = "#FFFFFF"
    let [<Literal>] snow = "#FFFAFA"
    let [<Literal>] honeyDew = "#F0FFF0"
    let [<Literal>] mintCream = "#F5FFFA"
    let [<Literal>] azure = "#F0FFFF"
    let [<Literal>] aliceBlue = "#F0F8FF"
    let [<Literal>] ghostWhite = "#F8F8FF"
    let [<Literal>] whiteSmoke = "#F5F5F5"
    let [<Literal>] seaShell = "#FFF5EE"
    let [<Literal>] beige = "#F5F5DC"
    let [<Literal>] oldLace = "#FDF5E6"
    let [<Literal>] floralWhite = "#FFFAF0"
    let [<Literal>] ivory = "#FFFFF0"
    let [<Literal>] antiqueWhite = "#FAEBD7"
    let [<Literal>] linen = "#FAF0E6"
    let [<Literal>] lavenderBlush = "#FFF0F5"
    let [<Literal>] mistyRose = "#FFE4E1"
    let [<Literal>] gainsBoro = "#DCDCDC"
    let [<Literal>] lightGray = "#D3D3D3"
    let [<Literal>] silver = "#C0C0C0"
    let [<Literal>] darkGray = "#A9A9A9"
    let [<Literal>] gray = "#808080"
    let [<Literal>] dimGray = "#696969"
    let [<Literal>] lightSlateGray = "#778899"
    let [<Literal>] slateGray = "#708090"
    let [<Literal>] darkSlateGray = "#2F4F4F"
    let [<Literal>] black = "#000000"
    let [<Literal>] transparent = "transparent"

/// Contains a list of CSS Fonts from https://www.tutorialbrain.com/css_tutorial/css_font_family_list/
module font =
    let [<Literal>] abadiMTCondensedLight = "Abadi MT Condensed Light"
    let [<Literal>] aharoni = "Aharoni"
    let [<Literal>] aharoniBold = "Aharoni Bold"
    let [<Literal>] aldhabi = "Aldhabi"
    let [<Literal>] alternateGothic2BT = "AlternateGothic2 BT"
    let [<Literal>] andaleMono = "Andale Mono"
    let [<Literal>] andalus = "Andalus"
    let [<Literal>] angsanaNew = "Angsana New"
    let [<Literal>] angsanaUPC = "AngsanaUPC"
    let [<Literal>] aparajita = "Aparajita"
    let [<Literal>] appleChancery = "Apple Chancery"
    let [<Literal>] arabicTypesetting = "Arabic Typesetting"
    let [<Literal>] arial = "Arial"
    let [<Literal>] arialBlack = "Arial Black"
    let [<Literal>] arialNarrow = "Arial narrow"
    let [<Literal>] arialNova = "Arial Nova"
    let [<Literal>] arialRoundedMTBold = "Arial Rounded MT Bold"
    let [<Literal>] arnoldboecklin = "Arnoldboecklin"
    let [<Literal>] avantaGarde = "Avanta Garde"
    let [<Literal>] bahnschrift = "Bahnschrift"
    let [<Literal>] bahnschriftLight = "Bahnschrift Light"
    let [<Literal>] bahnschriftSemiBold = "Bahnschrift SemiBold"
    let [<Literal>] bahnschriftSemiLight = "Bahnschrift SemiLight"
    let [<Literal>] baskerville = "Baskerville"
    let [<Literal>] batang = "Batang"
    let [<Literal>] batangChe = "BatangChe"
    let [<Literal>] bigCaslon = "Big Caslon"
    let [<Literal>] bizUDGothic = "BIZ UDGothic"
    let [<Literal>] bizUDMinchoMedium = "BIZ UDMincho Medium"
    let [<Literal>] blippo = "Blippo"
    let [<Literal>] bodoniMT = "Bodoni MT"
    let [<Literal>] bookAntiqua = "Book Antiqua"
    let [<Literal>] Bookman = "Bookman"
    let [<Literal>] bradlyHand = "Bradley Hand"
    let [<Literal>] browalliaNew = "Browallia New"
    let [<Literal>] browalliaUPC = "BrowalliaUPC"
    let [<Literal>] brushScriptMT = "Brush Script MT"
    let [<Literal>] brushScriptStd = "Brush Script Std"
    let [<Literal>] brushStroke = "Brushstroke"
    let [<Literal>] calibri = "Calibri"
    let [<Literal>] calibriLight = "Calibri Light"
    let [<Literal>] calistoMT = "Calisto MT"
    let [<Literal>] cambodian = "Cambodian"
    let [<Literal>] cambria = "Cambria"
    let [<Literal>] cambriaMath = "Cambria Math"
    let [<Literal>] candara = "Candara"
    let [<Literal>] centuryGothic = "Century Gothic"
    let [<Literal>] chalkDuster = "Chalkduster"
    let [<Literal>] cherokee = "Cherokee"
    let [<Literal>] comicSans = "Comic Sans"
    let [<Literal>] comicSansMS = "Comic Sans MS"
    let [<Literal>] consolas = "Consolas"
    let [<Literal>] constantia = "Constantia"
    let [<Literal>] copperPlate = "Copperplate"
    let [<Literal>] copperPlateGothicLight = "Copperplate Gothic Light"
    let [<Literal>] copperPlateGothicBold = "Copperplate Gothic Bold"
    let [<Literal>] corbel = "Corbel"
    let [<Literal>] cordiaNew = "Cordia New"
    let [<Literal>] cordiaUPC = "CordiaUPC"
    let [<Literal>] coronetScript = "Coronet script"
    let [<Literal>] courier = "Courier"
    let [<Literal>] courierNew = "Courier New"
    let [<Literal>] daunPenh = "DaunPenh"
    let [<Literal>] david = "David"
    let [<Literal>] dengXian = "DengXian"
    let [<Literal>] dfKaiSB = "DFKai-SB"
    let [<Literal>] didot = "Didot"
    let [<Literal>] dilleniaUPC = "DilleniaUPC"
    let [<Literal>] dokChampa = "DokChampa"
    let [<Literal>] dotum = "Dotum"
    let [<Literal>] dotumChe = "DotumChe"
    let [<Literal>] ebrima = "Ebrima"
    let [<Literal>] estrangeloEdessa = "Estrangelo Edessa"
    let [<Literal>] eucrosiaUPC = "EucrosiaUPC"
    let [<Literal>] euphemia = "Euphemia"
    let [<Literal>] fangSong = "FangSong"
    let [<Literal>] florence = "Florence"
    let [<Literal>] franklinGothicMedium = "Franklin Gothic Medium"
    let [<Literal>] frankRuehl = "FrankRuehl"
    let [<Literal>] fressiaUPC = "FressiaUPC"
    let [<Literal>] futara = "Futara"
    let [<Literal>] gabriola = "Gabriola"
    let [<Literal>] garamond = "Garamond"
    let [<Literal>] gautami = "Gautami"
    let [<Literal>] geneva = "Geneva"
    let [<Literal>] georgia = "Georgia"
    let [<Literal>] georgiaPro = "Georgia Pro"
    let [<Literal>] gillSans = "Gill Sans"
    let [<Literal>] gillSansNova = "Gill Sans Nova"
    let [<Literal>] gisha = "Gisha"
    let [<Literal>] goudyOldStyle = "Goudy Old Style"
    let [<Literal>] gulim = "Gulim"
    let [<Literal>] gulimChe = "GulimChe"
    let [<Literal>] gungsuh = "Gungsuh"
    let [<Literal>] gungsuhChe = "GungsuhChe"
    let [<Literal>] hebrew = "Hebrew"
    let [<Literal>] helvetica = "Helvetica"
    let [<Literal>] hoeflerText = "Hoefler Text"
    let [<Literal>] holoLensMDL2Assets = "HoloLens MDL2 Assets"
    let [<Literal>] impact = "Impact"
    let [<Literal>] inkFree = "Ink Free"
    let [<Literal>] irisUPC = "IrisUPC"
    let [<Literal>] iskoolaPota = "Iskoola Pota"
    let [<Literal>] japanese = "Japanese"
    let [<Literal>] jasmineUPC = "JasmineUPC"
    let [<Literal>] javaneseText = "Javanese Text"
    let [<Literal>] jazzLET = "Jazz LET"
    let [<Literal>] kaiTi = "KaiTi"
    let [<Literal>] kalinga = "Kalinga"
    let [<Literal>] kartika = "Kartika"
    let [<Literal>] khmerUI = "Khmer UI"
    let [<Literal>] kodchiangUPC = "KodchiangUPC"
    let [<Literal>] kokila = "Kokila"
    let [<Literal>] korean = "Korean"
    let [<Literal>] lao = "Lao"
    let [<Literal>] laoUI = "Lao UI"
    let [<Literal>] latha = "Latha"
    let [<Literal>] leelawadee = "Leelawadee"
    let [<Literal>] leelawadeeUI = "Leelawadee UI"
    let [<Literal>] leelawadeeUISemilight = "Leelawadee UI Semilight"
    let [<Literal>] levenimMT = "Levenim MT"
    let [<Literal>] lilyUPC = "LilyUPC"
    let [<Literal>] lucidaBright = "Lucida Bright"
    let [<Literal>] lucidaConsole = "Lucida Console"
    let [<Literal>] lucidaHandwriting = "Lucida Handwriting"
    let [<Literal>] lucidaSans = "Lucida Sans"
    let [<Literal>] lucidaSansTypewriter = "Lucida Sans Typewriter"
    let [<Literal>] lucidaSansUnicode = "Lucida Sans Unicode"
    let [<Literal>] lucidaTypewriter = "Lucidatypewriter"
    let [<Literal>] luminari = "Luminari"
    let [<Literal>] malgunGothic = "Malgun Gothic"
    let [<Literal>] malgunGothicSemilight = "Malgun Gothic Semilight"
    let [<Literal>] mangal = "Mangal"
    let [<Literal>] markerFelt = "Marker Felt"
    let [<Literal>] marlett = "Marlett"
    let [<Literal>] meiryo = "Meiryo"
    let [<Literal>] meiryoUI = "Meiryo UI"
    let [<Literal>] microsoftHimalaya = "Microsoft Himalaya"
    let [<Literal>] microsoftJhengHei = "Microsoft JhengHei"
    let [<Literal>] microsoftJhengHeiUI = "Microsoft JhengHei UI"
    let [<Literal>] microsoftNewTaiLue = "Microsoft New Tai Lue"
    let [<Literal>] microsoftPhagsPa = "Microsoft PhagsPa"
    let [<Literal>] microsoftSansSerif = "Microsoft Sans Serif"
    let [<Literal>] microsoftTaiLe = "Microsoft Tai Le"
    let [<Literal>] microsoftUighur = "Microsoft Uighur"
    let [<Literal>] microsoftYaHei = "Microsoft YaHei"
    let [<Literal>] microsoftYaHeiUI = "Microsoft YaHei UI"
    let [<Literal>] microsoftYiBaiti = "Microsoft Yi Baiti"
    let [<Literal>] mingLiU = "MingLiU"
    let [<Literal>] mingLiUHKSCS = "MingLiU_HKSCS"
    let [<Literal>] mingLiUHKSCSExtB = "MingLiU_HKSCS-ExtB"
    let [<Literal>] mingLiUExtB = "MingLiU-ExtB"
    let [<Literal>] miriam = "Miriam"
    let [<Literal>] monaco = "Monaco"
    let [<Literal>] mongolianBaiti = "Mongolian Baiti"
    let [<Literal>] moolBoran = "MoolBoran"
    let [<Literal>] msGothic = "MS Gothic"
    let [<Literal>] msMincho = "MS Mincho"
    let [<Literal>] msPGothic = "MS PGothic"
    let [<Literal>] msPMincho = "MS PMincho"
    let [<Literal>] msUIGothic = "MS UI Gothic"
    let [<Literal>] mvBoli = "MV Boli"
    let [<Literal>] myanmarText = "Myanmar Text"
    let [<Literal>] narkisim = "Narkisim"
    let [<Literal>] neueHaasGroteskTextPro = "Neue Haas Grotesk Text Pro"
    let [<Literal>] newCenturySchoolbook = "New Century Schoolbook"
    let [<Literal>] newsGothicMT = "News Gothic MT"
    let [<Literal>] nirmalaUI = "Nirmala UI"
    let [<Literal>] noAutoLanguageAssoc = "No automatic language associations"
    let [<Literal>] noto = "Noto"
    let [<Literal>] nSimSun = "NSimSun"
    let [<Literal>] nyala = "Nyala"
    let [<Literal>] oldTown = "Oldtown"
    let [<Literal>] optima = "Optima"
    let [<Literal>] palatino = "Palatino"
    let [<Literal>] palatinoLinotype = "Palatino Linotype"
    let [<Literal>] papyrus = "papyrus"
    let [<Literal>] parkAvenue = "Parkavenue"
    let [<Literal>] perpetua = "Perpetua"
    let [<Literal>] plantagenetCherokee = "Plantagenet Cherokee"
    let [<Literal>] PMingLiU = "PMingLiU"
    let [<Literal>] raavi = "Raavi"
    let [<Literal>] rockwell = "Rockwell"
    let [<Literal>] rockwellExtraBold = "Rockwell Extra Bold"
    let [<Literal>] rockwellNova = "Rockwell Nova"
    let [<Literal>] rockwellNovaCond = "Rockwell Nova Cond"
    let [<Literal>] rockwellNovaExtraBold = "Rockwell Nova Extra Bold"
    let [<Literal>] rod = "Rod"
    let [<Literal>] sakkalMajalla = "Sakkal Majalla"
    let [<Literal>] sanskritText = "Sanskrit Text"
    let [<Literal>] segoeMDL2Assets = "segoeMDL2Assets"
    let [<Literal>] segoePrint = "Segoe Print"
    let [<Literal>] segoeScript = "Segoe Script"
    let [<Literal>] segoeUI = "Segoe UI"
    let [<Literal>] segoeUIEmoji = "Segoe UI Emoji"
    let [<Literal>] segoeUIHistoric = "Segoe UI Historic"
    let [<Literal>] segoeUISymbol = "Segoe UI Symbol"
    let [<Literal>] shonarBangla = "Shonar Bangla"
    let [<Literal>] shruti = "Shruti"
    let [<Literal>] simHei = "SimHei"
    let [<Literal>] simKai = "SimKai"
    let [<Literal>] simplifiedArabic = "Simplified Arabic"
    let [<Literal>] simplifiedChinese = "Simplified Chinese"
    let [<Literal>] simSun = "SimSun"
    let [<Literal>] simSunExtB = "SimSun-ExtB"
    let [<Literal>] sitka = "Sitka"
    let [<Literal>] snellRoundhan = "Snell Roundhan"
    let [<Literal>] stencilStd = "Stencil Std"
    let [<Literal>] sylfaen = "Sylfaen"
    let [<Literal>] symbol = "Symbol"
    let [<Literal>] tahoma = "Tahoma"
    let [<Literal>] thai = "Thai"
    let [<Literal>] timesNewRoman = "Times New Roman"
    let [<Literal>] traditionalArabic = "Traditional Arabic"
    let [<Literal>] traditionalChinese = "Traditional Chinese"
    let [<Literal>] trattatello = "Trattatello"
    let [<Literal>] trebuchetMS = "Trebuchet MS"
    let [<Literal>] udDigiKyokasho = "UD Digi Kyokasho"
    let [<Literal>] udDigiKyokashoNKR = "UD Digi Kyokasho NK-R"
    let [<Literal>] udDigiKyokashoNPR = "UD Digi Kyokasho NP-R"
    let [<Literal>] udDigiKyokashoNR = "UD Digi Kyokasho N-R"
    let [<Literal>] urduTypesetting = "Urdu Typesetting"
    let [<Literal>] urwChancery = "URW Chancery"
    let [<Literal>] utsaah = "Utsaah"
    let [<Literal>] vani = "Vani"
    let [<Literal>] verdana = "Verdana"
    let [<Literal>] verdanaPro = "Verdana Pro"
    let [<Literal>] vijaya = "Vijaya"
    let [<Literal>] vrinda = "Vrinda"
    let [<Literal>] Webdings = "Webdings"
    let [<Literal>] westminster = "Westminster"
    let [<Literal>] wingdings = "Wingdings"
    let [<Literal>] yuGothic = "Yu Gothic"
    let [<Literal>] yuGothicUI = "Yu Gothic UI"
    let [<Literal>] yuMincho = "Yu Mincho"
    let [<Literal>] zapfChancery = "Zapf Chancery"

/// Specifies a number of specialized CSS units
type length =
    static member inline zero : ICssUnit = newCssUnit "0"
    /// Pixels are (1px = 1/96th of 1in).
    ///
    /// **Note**: Pixels (px) are relative to the viewing device. For low-dpi devices, 1px is one device pixel (dot) of the display. For printers and high resolution screens 1px implies multiple device pixels.
    static member inline px(value: int) : ICssUnit = newCssUnit (string value + "px")
    /// Pixels are (1px = 1/96th of 1in).
    ///
    /// **Note**: Pixels (px) are relative to the viewing device. For low-dpi devices, 1px is one device pixel (dot) of the display. For printers and high resolution screens 1px implies multiple device pixels.
    static member inline px(value: double) : ICssUnit = newCssUnit (string value + "px")
    /// Centimeters
    static member inline cm(value: int) : ICssUnit = newCssUnit (string value + "cm")
    /// Centimeters
    static member inline cm(value: double) : ICssUnit = newCssUnit (string value + "cm")
    /// Millimeters
    static member inline mm(value: int) : ICssUnit = newCssUnit (string value + "mm")
    /// Millimeters
    static member inline mm(value: double) : ICssUnit = newCssUnit (string value + "mm")
    /// Inches (1in = 96px = 2.54cm)
    static member inline inch(value: int) : ICssUnit = newCssUnit (string value + "in")
    /// Inches (1in = 96px = 2.54cm)
    static member inline inch(value: double) : ICssUnit = newCssUnit (string value + "in")
    /// Points (1pt = 1/72 of 1in)
    static member inline pt(value: int) : ICssUnit = newCssUnit (string value + "pt")
    /// Points (1pt = 1/72 of 1in)
    static member inline pt(value: double) : ICssUnit = newCssUnit (string value + "pt")
    /// Picas (1pc = 12 pt)
    static member inline pc(value: int) : ICssUnit = newCssUnit (string value + "pc")
    /// Picas (1pc = 12 pt)
    static member inline pc(value: double) : ICssUnit = newCssUnit (string value + "pc")
    /// Relative to the font-size of the element (2em means 2 times the size of the current font
    static member inline em(value: int) : ICssUnit = newCssUnit (string value + "em")
    /// Relative to the font-size of the element (2em means 2 times the size of the current font
    static member inline em(value: double) : ICssUnit = newCssUnit (string value + "em")
    /// Relative to the x-height of the current font (rarely used)
    static member inline ex(value: int) : ICssUnit = newCssUnit (string value + "ex")
    /// Relative to the x-height of the current font (rarely used)
    static member inline ex(value: double) : ICssUnit = newCssUnit (string value + "ex")
    /// Relative to width of the "0" (zero)
    static member inline ch(value: int) : ICssUnit = newCssUnit (string value + "ch")
    /// Relative to font-size of the root element
    static member inline rem(value: double) : ICssUnit = newCssUnit (string value + "rem")
    /// Relative to font-size of the root element
    static member inline rem(value: int) : ICssUnit = newCssUnit (string value + "rem")
    /// Relative to 1% of the height of the viewport*
    ///
    /// **Viewport** = the browser window size. If the viewport is 50cm wide, 1vw = 0.5cm.
    static member inline vh(value: int) : ICssUnit = newCssUnit (string value + "vh")
    /// Relative to 1% of the height of the viewport*
    ///
    /// **Viewport** = the browser window size. If the viewport is 50cm wide, 1vw = 0.5cm.
    static member inline vh(value: double) : ICssUnit = newCssUnit (string value + "vh")
    /// Relative to 1% of the width of the viewport*
    ///
    /// **Viewport** = the browser window size. If the viewport is 50cm wide, 1vw = 0.5cm.
    static member inline vw(value: int) : ICssUnit = newCssUnit (string value + "vw")
    /// Relative to 1% of the width of the viewport*
    ///
    /// **Viewport** = the browser window size. If the viewport is 50cm wide, 1vw = 0.5cm.
    static member inline vw(value: double) : ICssUnit = newCssUnit (string value + "vw")
    /// Relative to 1% of viewport's smaller dimension
    static member inline vmin(value: double) : ICssUnit = newCssUnit (string value + "vmin")
    /// Relative to 1% of viewport's smaller dimension
    static member inline vmin(value: int) : ICssUnit = newCssUnit (string value + "vmin")
    /// Relative to 1% of viewport's larger dimension
    static member inline vmax(value: double) : ICssUnit = newCssUnit (string value + "vmax")
    /// Relative to 1% of viewport's* larger dimension
    static member inline vmax(value: int) : ICssUnit = newCssUnit (string value + "vmax")
    /// Relative to the parent element
    static member inline perc(value: int) : ICssUnit = newCssUnit (string value + "%")
    /// Relative to the parent element
    static member inline perc(value: double) : ICssUnit = newCssUnit (string value + "%")
    /// Relative to the parent element
    static member inline percent(value: int) : ICssUnit = newCssUnit (string value + "%")
    /// Relative to the parent element
    static member inline percent(value: double) : ICssUnit = newCssUnit (string value + "%")
    /// The browser calculates the length.
    static member inline auto : ICssUnit = newCssUnit "auto"
    /// calculated length, frequency, angle, time, percentage, number or integer
    static member inline calc(value: string) : ICssUnit = newCssUnit ("calc(" + value + ")")
    /// Relative to width of the grid layout in correlation with the other fr's in the grid
    static member inline fr(value: int) : ICssUnit = newCssUnit (string value + "fr")

type borderStyle =
    /// Specifies a dashed border.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_border-style&preval=dashed
    static member inline dashed : IBorderStyle = newBorderStyle "dashed"
    /// Specifies a dotted border.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_border-style&preval=dotted
    static member inline dotted : IBorderStyle = newBorderStyle "dotted"
    /// Specifies a double border.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_border-style&preval=double
    static member inline double : IBorderStyle = newBorderStyle "double"
    /// Specifies a 3D grooved border. The effect depends on the border-color value.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_border-style&preval=groove
    static member inline groove : IBorderStyle = newBorderStyle "groove"
    /// The same as "none", except in border conflict resolution for table elements.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_border-style&preval=hidden
    static member inline hidden : IBorderStyle = newBorderStyle "hidden"
    /// Inherits this property from its parent element.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_border-style&preval=inherit
    ///
    /// Read about inherit https://www.w3schools.com/cssref/css_inherit.asp
    static member inline inheritFromParent : IBorderStyle = newBorderStyle "inherit"
    /// Sets this property to its default value.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_border-style&preval=initial
    ///
    /// Read about initial value https://www.w3schools.com/cssref/css_initial.asp
    static member inline initial : IBorderStyle = newBorderStyle "initial"
    /// Specifies a 3D inset border. The effect depends on the border-color value.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_border-style&preval=inset
    static member inline inset : IBorderStyle = newBorderStyle "inset"
    /// Default value. Specifies no border.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_border-style&preval=none
    static member inline none : IBorderStyle = newBorderStyle "none"
    /// Specifies a 3D outset border. The effect depends on the border-color value.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_border-style&preval=outset
    static member inline outset : IBorderStyle = newBorderStyle "outset"
    /// Specifies a 3D ridged border. The effect depends on the border-color value.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_border-style&preval=ridge
    static member inline ridge : IBorderStyle = newBorderStyle "ridge"
    /// Specifies a solid border.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_border-style&preval=solid
    static member inline solid : IBorderStyle = newBorderStyle "solid"

type gridColumn =
    static member inline span(value: string) : IGridSpan = newGridSpan("span " + value)
    static member inline span(value: string, count: int) : IGridSpan = newGridSpan("span " + value + " " + (string count))
    static member inline span(value: int) : IGridSpan = newGridSpan("span " + (string value))

type gridRow =
    static member inline span(value: string) : IGridSpan = newGridSpan("span " + value)
    static member inline span(value: string, count: int) : IGridSpan = newGridSpan("span " + value + " " + (string count))
    static member inline span(value: int) : IGridSpan = newGridSpan("span " + (string value))

type grid =
    static member inline namedLine(value: string) : IGridTemplateItem = newGridTemplateItem ("[" + value + "]")
    static member inline namedLines(value: string[]) : IGridTemplateItem = newGridTemplateItem ("[" + (String.concat " " value) + "]")
    static member inline namedLines(value: string list) : IGridTemplateItem = newGridTemplateItem ("[" + (String.concat " " value) + "]")
    static member inline templateWidth(value: ICssUnit) : IGridTemplateItem = newGridTemplateItem(asString value)
    static member inline templateWidth(value: int) : IGridTemplateItem = newGridTemplateItem ((string value) + "px")
    static member inline templateWidth(value: float) : IGridTemplateItem = newGridTemplateItem ((string value) + "px")

type textDecorationLine =
    static member inline none : ITextDecorationLine = newTextDecorationLine "none"
    static member inline underline : ITextDecorationLine = newTextDecorationLine "underline"
    static member inline overline : ITextDecorationLine = newTextDecorationLine "overline"
    static member inline lineThrough : ITextDecorationLine = newTextDecorationLine "line-through"
    static member inline initial : ITextDecorationLine = newTextDecorationLine "initial"
    static member inline inheritFromParent : ITextDecorationLine = newTextDecorationLine "inherit"

type textDecorationStyle =
    /// Default value. The line will display as a single line.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_text-decoration-style&preval=solid
    static member inline solid : ITextDecoration = newTextDecoration "solid"
    /// The line will display as a double line.
    ///
    /// https://www.w3schools.com/cssref/playit.asp?filename=playcss_text-decoration-style&preval=double
    static member inline double : ITextDecoration = newTextDecoration "double"
    /// The line will display as a dotted line.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_text-decoration-style&preval=dotted
    static member inline dotted : ITextDecoration = newTextDecoration "dotted"
    /// The line will display as a dashed line.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_text-decoration-style&preval=dashed
    static member inline dashed : ITextDecoration = newTextDecoration "dashed"
    /// The line will display as a wavy line.
    ///
    /// https://www.w3schools.com/cssref/playit.asp?filename=playcss_text-decoration-style&preval=wavy
    static member inline wavy : ITextDecoration = newTextDecoration "wavy"
    /// Sets this property to its default value.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_text-decoration-style&preval=initial
    static member inline initial : ITextDecoration = newTextDecoration "initial"
    /// Inherits this property from its parent element.
    static member inline inheritFromParent : ITextDecoration = newTextDecoration "inherit"

/// The transform CSS property lets you rotate, scale, skew, or translate an element.
/// It modifies the coordinate space of the CSS [visual formatting model](https://developer.mozilla.org/en-US/docs/Web/CSS/Visual_formatting_model)
type transform =
    /// Defines that there should be no transformation.
    static member inline none = newTransformProperty "none"
    /// Defines a 2D transformation, using a matrix of six values.
    static member inline matrix(x1: int, y1: int, z1: int, x2: int, y2: int, z2: int) =
        newTransformProperty (
            "matrix(" +
            (asString x1) + "," +
            (asString y1) + "," +
            (asString z1) + "," +
            (asString x2) + "," +
            (asString y2) + "," +
            (asString z2) + ")"
        )

    /// Defines a 2D translation.
    static member inline translate(x: int, y: int) =
        newTransformProperty (
            "translate(" + (asString x) + "px," + (asString y) + "px)"
        )
    /// Defines a 2D translation.
    static member inline translate(x: ICssUnit, y: ICssUnit) =
        newTransformProperty (
            "translate(" + (asString x) + "," + (asString y) + ")"
        )

    /// Defines a 3D translation.
    static member inline translate3D(x: int, y: int, z: int) =
        newTransformProperty (
            "translate3d(" + (asString x) + "px," + (asString y) + "px," + (asString z) + "px)"
        )
    /// Defines a 3D translation.
    static member inline translate3D(x: ICssUnit, y: ICssUnit, z: ICssUnit) =
        newTransformProperty (
            "translate3d(" + (asString x) + "," + (asString y) + "," + (asString z) + ")"
        )

    /// Defines a translation, using only the value for the X-axis.
    static member inline translateX(x: int) =
        newTransformProperty ("translateX(" + (asString x) + "px)")
    /// Defines a translation, using only the value for the X-axis.
    static member inline translateX(x: ICssUnit) =
        newTransformProperty ("translateX(" + (asString x) + ")")
    /// Defines a translation, using only the value for the Y-axis
    static member inline translateY(y: int) =
        newTransformProperty ("translateY(" + (asString y) + "px)")
    /// Defines a translation, using only the value for the Y-axis
    static member inline translateY(y: ICssUnit) =
        newTransformProperty ("translateY(" + (asString y) + ")")
    /// Defines a 3D translation, using only the value for the Z-axis
    static member inline translateZ(z: int) =
        newTransformProperty ("translateZ(" + (asString z) + "px)")
    /// Defines a 3D translation, using only the value for the Z-axis
    static member inline translateZ(z: ICssUnit) =
        newTransformProperty ("translateZ(" + (asString z) + ")")

    /// Defines a 2D scale transformation.
    static member inline scale(x: int, y: int) =
        newTransformProperty (
            "scale(" + (asString x) + "," + (asString y) + ")"
        )
    /// Defines a 2D scale transformation.
    static member inline scale(x: float, y: float) =
        newTransformProperty (
            "scale(" + (asString x) + "," + (asString y) + ")"
        )

    /// Defines a scale transformation.
    static member inline scale(n: int) =
        newTransformProperty (
            "scale(" + (asString n) + ")"
        )

    /// Defines a scale transformation.
    static member inline scale(n: float) =
        newTransformProperty (
            "scale(" + (asString n) + ")"
        )

    /// Defines a 3D scale transformation
    static member inline scale3D(x: int, y: int, z: int) =
        newTransformProperty (
            "scale3d(" + (asString x) + "," + (asString y) + "," + (asString z) + ")"
        )
    /// Defines a 3D scale transformation
    static member inline scale3D(x: float, y: float, z: float) =
        newTransformProperty (
            "scale3d(" + (asString x) + "," + (asString y) + "," + (asString z) + ")"
        )

    /// Defines a scale transformation by giving a value for the X-axis.
    static member inline scaleX(x: int) =
        newTransformProperty ("scaleX(" + (asString x) + ")")

    /// Defines a scale transformation by giving a value for the X-axis.
    static member inline scaleX(x: float) =
        newTransformProperty ("scaleX(" + (asString x) + ")")
    /// Defines a scale transformation by giving a value for the Y-axis.
    static member inline scaleY(y: int) =
        newTransformProperty ("scaleY(" + (asString y) + ")")
    /// Defines a scale transformation by giving a value for the Y-axis.
    static member inline scaleY(y: float) =
        newTransformProperty ("scaleY(" + (asString y) + ")")
    /// Defines a 3D translation, using only the value for the Z-axis
    static member inline scaleZ(z: int) =
        newTransformProperty ("scaleZ(" + (asString z) + ")")
    /// Defines a 3D translation, using only the value for the Z-axis
    static member inline scaleZ(z: float) =
        newTransformProperty ("scaleZ(" + (asString z) + ")")
    /// Defines a 2D rotation, the angle is specified in the parameter.
    static member inline rotate(deg: int) =
        newTransformProperty ("rotate(" + (asString deg) + "deg)")
    /// Defines a 2D rotation, the angle is specified in the parameter.
    static member inline rotate(deg: float) =
        newTransformProperty ("rotate(" + (asString deg) + "deg)")
    /// Defines a 3D rotation along the X-axis.
    static member inline rotateX(deg: float) =
        newTransformProperty ("rotateX(" + (asString deg) + "deg)")
    /// Defines a 3D rotation along the X-axis.
    static member inline rotateX(deg: int) =
        newTransformProperty ("rotateX(" + (asString deg) + "deg)")
    /// Defines a 3D rotation along the Y-axis
    static member inline rotateY(deg: float) =
        newTransformProperty ("rotateY(" + (asString deg) + "deg)")
    /// Defines a 3D rotation along the Y-axis
    static member inline rotateY(deg: int) =
        newTransformProperty ("rotateY(" + (asString deg) + "deg)")
    /// Defines a 3D rotation along the Z-axis
    static member inline rotateZ(deg: float) =
        newTransformProperty ("rotateZ(" + (asString deg) + "deg)")
    /// Defines a 3D rotation along the Z-axis
    static member inline rotateZ(deg: int) =
        newTransformProperty ("rotateZ(" + (asString deg) + "deg)")
    /// Defines a 2D skew transformation along the X- and the Y-axis.
    static member inline skew(xAngle: int, yAngle: int) =
        newTransformProperty ("skew(" + (asString xAngle) + "deg," + (asString yAngle) + "deg)")
    /// Defines a 2D skew transformation along the X- and the Y-axis.
    static member inline skew(xAngle: float, yAngle: float) =
        newTransformProperty ("skew(" + (asString xAngle) + "deg," + (asString yAngle) + "deg)")
    /// Defines a 2D skew transformation along the X-axis
    static member inline skewX(xAngle: int) =
        newTransformProperty ("skewX(" + (asString xAngle) + "deg)")
    /// Defines a 2D skew transformation along the X-axis
    static member inline skewX(xAngle: float) =
        newTransformProperty ("skewX(" + (asString xAngle) + "deg)")
    /// Defines a 2D skew transformation along the Y-axis
    static member inline skewY(xAngle: int) =
        newTransformProperty ("skewY(" + (asString xAngle) + "deg)")
    /// Defines a 2D skew transformation along the Y-axis
    static member inline skewY(xAngle: float) =
        newTransformProperty ("skewY(" + (asString xAngle) + "deg)")
    /// Defines a perspective view for a 3D transformed element
    static member inline perspective(n: int) =
        newTransformProperty ("perspective(" + (asString n) + ")")

type transitionProperty =
    static member inline all = newTransitionProperty "all"
    static member inline backdropFilter = newTransitionProperty "backdrop-filter"
    static member inline background = newTransitionProperty "background"
    static member inline backgroundColor = newTransitionProperty "background-color"
    static member inline backgroundPosition = newTransitionProperty "background-position"
    static member inline backgroundSize = newTransitionProperty "background-size"
    static member inline border = newTransitionProperty "border"
    static member inline borderBottom = newTransitionProperty "border-bottom"
    static member inline borderBottomColor = newTransitionProperty "border-bottom-color"
    static member inline borderBottomLeftRadius = newTransitionProperty "border-bottom-left-radius"
    static member inline borderBottomRightRadius = newTransitionProperty "border-bottom-right-radius"
    static member inline borderBottomWidth = newTransitionProperty "border-bottom-width"
    static member inline borderColor = newTransitionProperty "border-color"
    static member inline borderEndEndRadius = newTransitionProperty "border-end-end-radius"
    static member inline borderEndStartRadius = newTransitionProperty "border-end-start-radius"
    static member inline borderLeft = newTransitionProperty "border-left"
    static member inline borderLeftColor = newTransitionProperty "border-left-color"
    static member inline borderLeftWidth = newTransitionProperty "border-left-width"
    static member inline borderRadius = newTransitionProperty "border-radius"
    static member inline borderRight = newTransitionProperty "border-right"
    static member inline borderRightColor = newTransitionProperty "border-right-color"
    static member inline borderRightWidth = newTransitionProperty "border-right-width"
    static member inline borderStartEndRadius = newTransitionProperty "border-start-end-radius"
    static member inline borderStartStartRadius = newTransitionProperty "border-start-start-radius"
    static member inline borderTop = newTransitionProperty "border-top"
    static member inline borderTopColor = newTransitionProperty "border-top-color"
    static member inline borderTopLeftRadius = newTransitionProperty "border-top-left-radius"
    static member inline borderTopRightRadius = newTransitionProperty "border-top-right-radius"
    static member inline borderTopWidth = newTransitionProperty "border-top-width"
    static member inline borderWidth = newTransitionProperty "border-width"
    static member inline bottom = newTransitionProperty "bottom"
    static member inline boxShadow = newTransitionProperty "box-shadow"
    static member inline caretColor = newTransitionProperty "caret-color"
    static member inline clip = newTransitionProperty "clip"
    static member inline clipPath = newTransitionProperty "clip-path"
    static member inline color = newTransitionProperty "color"
    static member inline columnCount = newTransitionProperty "column-count"
    static member inline columnGap = newTransitionProperty "column-gap"
    static member inline columnRule = newTransitionProperty "column-rule"
    static member inline columnRuleColor = newTransitionProperty "column-rule-color"
    static member inline columnRuleWidth = newTransitionProperty "column-rule-width"
    static member inline columnWidth = newTransitionProperty "column-width"
    static member inline columns = newTransitionProperty "columns"
    static member inline filter = newTransitionProperty "filter"
    static member inline flex = newTransitionProperty "flex"
    static member inline flexBasis = newTransitionProperty "flex-basis"
    static member inline flexGrow = newTransitionProperty "flex-grow"
    static member inline flexShrink = newTransitionProperty "flex-shrink"
    static member inline font = newTransitionProperty "font"
    static member inline fontSize = newTransitionProperty "font-size"
    static member inline fontSizeAdjust = newTransitionProperty "font-size-adjust"
    static member inline fontStretch = newTransitionProperty "font-stretch"
    static member inline fontVariationSettings = newTransitionProperty "font-variation-settings"
    static member inline fontWeight = newTransitionProperty "font-weight"
    static member inline gap = newTransitionProperty "gap"
    static member inline gridColumnGap = newTransitionProperty "grid-column-gap"
    static member inline gridGap = newTransitionProperty "grid-gap"
    static member inline gridRowGap = newTransitionProperty "grid-row-gap"
    static member inline gridTemplateColumns = newTransitionProperty "grid-template-columns"
    static member inline gridTemplateRows = newTransitionProperty "grid-template-rows"
    static member inline height = newTransitionProperty "height"
    static member inline inset = newTransitionProperty "inset"
    static member inline insetBlock = newTransitionProperty "inset-block"
    static member inline insetBlockEnd = newTransitionProperty "inset-block-end"
    static member inline insetBlockStart = newTransitionProperty "inset-block-start"
    static member inline insetInline = newTransitionProperty "inset-inline"
    static member inline insetInlineEnd = newTransitionProperty "inset-inline-end"
    static member inline insetInlineStart = newTransitionProperty "inset-inline-start"
    static member inline left = newTransitionProperty "left"
    static member inline letterSpacing = newTransitionProperty "letter-spacing"
    static member inline lineClamp = newTransitionProperty "line-clamp"
    static member inline lineHeight = newTransitionProperty "line-height"
    static member inline margin = newTransitionProperty "margin"
    static member inline marginBottom = newTransitionProperty "margin-bottom"
    static member inline marginLeft = newTransitionProperty "margin-left"
    static member inline marginRight = newTransitionProperty "margin-right"
    static member inline marginTop = newTransitionProperty "margin-top"
    static member inline mask = newTransitionProperty "mask"
    static member inline maskBorder = newTransitionProperty "mask-border"
    static member inline maskPosition = newTransitionProperty "mask-position"
    static member inline maskSize = newTransitionProperty "mask-size"
    static member inline maxHeight = newTransitionProperty "max-height"
    static member inline maxLines = newTransitionProperty "max-lines"
    static member inline maxWidth = newTransitionProperty "max-width"
    static member inline minHeight = newTransitionProperty "min-height"
    static member inline minWidth = newTransitionProperty "min-width"
    static member inline objectPosition = newTransitionProperty "object-position"
    static member inline offset = newTransitionProperty "offset"
    static member inline offsetAnchor = newTransitionProperty "offset-anchor"
    static member inline offsetDistance = newTransitionProperty "offset-distance"
    static member inline offsetPath = newTransitionProperty "offset-path"
    static member inline offsetPosition = newTransitionProperty "offset-position"
    static member inline offsetRotate = newTransitionProperty "offset-rotate"
    static member inline opacity = newTransitionProperty "opacity"
    static member inline order = newTransitionProperty "order"
    static member inline outline = newTransitionProperty "outline"
    static member inline outlineColor = newTransitionProperty "outline-color"
    static member inline outlineOffset = newTransitionProperty "outline-offset"
    static member inline outlineWidth = newTransitionProperty "outline-width"
    static member inline padding = newTransitionProperty "padding"
    static member inline paddingBottom = newTransitionProperty "padding-bottom"
    static member inline paddingLeft = newTransitionProperty "padding-left"
    static member inline paddingRight = newTransitionProperty "padding-right"
    static member inline paddingTop = newTransitionProperty "padding-top"
    static member inline perspective = newTransitionProperty "perspective"
    static member inline perspectiveOrigin = newTransitionProperty "perspective-origin"
    static member inline right = newTransitionProperty "right"
    static member inline rotate = newTransitionProperty "rotate"
    static member inline rowGap = newTransitionProperty "row-gap"
    static member inline scale = newTransitionProperty "scale"
    static member inline scrollMargin = newTransitionProperty "scroll-margin"
    static member inline scrollMarginBlock = newTransitionProperty "scroll-margin-block"
    static member inline scrollMarginBlockEnd = newTransitionProperty "scroll-margin-block-end"
    static member inline scrollMarginBlockStart = newTransitionProperty "scroll-margin-block-start"
    static member inline scrollMarginBottom = newTransitionProperty "scroll-margin-bottom"
    static member inline scrollMarginInline = newTransitionProperty "scroll-margin-inline"
    static member inline scrollMarginInlineEnd = newTransitionProperty "scroll-margin-inline-end"
    static member inline scrollMarginInlineStart = newTransitionProperty "scroll-margin-inline-start"
    static member inline scrollMarginLeft = newTransitionProperty "scroll-margin-left"
    static member inline scrollMarginRight = newTransitionProperty "scroll-margin-right"
    static member inline scrollMarginTop = newTransitionProperty "scroll-margin-top"
    static member inline scrollPadding = newTransitionProperty "scroll-padding"
    static member inline scrollPaddingBlock = newTransitionProperty "scroll-padding-block"
    static member inline scrollPaddingBlockEnd = newTransitionProperty "scroll-padding-block-end"
    static member inline scrollPaddingBlockStart = newTransitionProperty "scroll-padding-block-start"
    static member inline scrollPaddingBottom = newTransitionProperty "scroll-padding-bottom"
    static member inline scrollPaddingInline = newTransitionProperty "scroll-padding-inline"
    static member inline scrollPaddingInlineEnd = newTransitionProperty "scroll-padding-inline-end"
    static member inline scrollPaddingInlineStart = newTransitionProperty "scroll-padding-inline-start"
    static member inline scrollPaddingLeft = newTransitionProperty "scroll-padding-left"
    static member inline scrollPaddingRight = newTransitionProperty "scroll-padding-right"
    static member inline scrollPaddingTop = newTransitionProperty "scroll-padding-top"
    static member inline scrollSnapCoordinate = newTransitionProperty "scroll-snap-coordinate"
    static member inline scrollSnapDestination = newTransitionProperty "scroll-snap-destination"
    static member inline scrollbarColor = newTransitionProperty "scrollbar-color"
    static member inline shapeImageThreshold = newTransitionProperty "shape-image-threshold"
    static member inline shapeMargin = newTransitionProperty "shape-margin"
    static member inline shapeOutside = newTransitionProperty "shape-outside"
    static member inline tabSize = newTransitionProperty "tab-size"
    static member inline textDecoration = newTransitionProperty "text-decoration"
    static member inline textDecorationColor = newTransitionProperty "text-decoration-color"
    static member inline textEmphasis = newTransitionProperty "text-emphasis"
    static member inline textEmphasisColor = newTransitionProperty "text-emphasis-color"
    static member inline textIndent = newTransitionProperty "text-indent"
    static member inline textShadow = newTransitionProperty "text-shadow"
    static member inline top = newTransitionProperty "top"
    static member inline transform = newTransitionProperty "transform"
    static member inline transformOrigin = newTransitionProperty "transform-origin"
    static member inline translate = newTransitionProperty "translate"
    static member inline verticalAlign = newTransitionProperty "vertical-align"
    static member inline visibility = newTransitionProperty "visibility"
    static member inline width = newTransitionProperty "width"
    static member inline wordSpacing = newTransitionProperty "word-spacing"
    static member inline zIndex = newTransitionProperty "z-index"
    static member inline zoom = newTransitionProperty "zoom"