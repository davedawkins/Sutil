
type CssWordWrapEngine<'Style>(h: CssHelper<'Style>) =
    /// Break words only at allowed break points
    member _.normal = h.MakeStyle("word-wrap", "normal")
    /// Allows unbreakable words to be broken
    member _.breakWord = h.MakeStyle("word-wrap", "break-word")
    /// Sets this property to its default value.
    member _.initial = h.MakeStyle("word-wrap", "initial")
    /// Inherits this property from its parent element.
    member _.inheritFromParent = h.MakeStyle("word-wrap", "inherit")

type CssAlignSelfEngine<'Style>(h: CssHelper<'Style>) =
    /// Default. The element inherits its parent container's align-items property, or "stretch" if it has no parent container.
    member _.auto = h.MakeStyle("align-self", "auto")
    /// The element is positioned to fit the container
    member _.stretch = h.MakeStyle("align-self", "stretch")
    /// The element is positioned at the center of the container
    member _.center = h.MakeStyle("align-self", "center")
    /// The element is positioned at the beginning of the container
    member _.flexStart = h.MakeStyle("align-self", "flex-start")
    /// The element is positioned at the end of the container
    member _.flexEnd = h.MakeStyle("align-self", "flex-end")
    /// The element is positioned at the baseline of the container
    member _.baseline = h.MakeStyle("align-self", "baseline")
    /// Sets this property to its default value
    member _.initial = h.MakeStyle("align-self", "initial")
    /// Inherits this property from its parent element
    member _.inheritFromParent = h.MakeStyle("align-self", "inherit")

type CssAlignItemsEngine<'Style>(h: CssHelper<'Style>) =
    /// Default. Items are stretched to fit the container
    member _.stretch = h.MakeStyle("align-items", "stretch")
    /// Items are positioned at the center of the container
    member _.center = h.MakeStyle("align-items", "center")
    /// Items are positioned at the beginning of the container
    member _.flexStart = h.MakeStyle("align-items", "flex-start")
    /// Items are positioned at the end of the container
    member _.flexEnd = h.MakeStyle("align-items", "flex-end")
    /// Items are positioned at the baseline of the container
    member _.baseline = h.MakeStyle("align-items", "baseline")
    /// Sets this property to its default value
    member _.initial = h.MakeStyle("align-items", "initial")
    /// Inherits this property from its parent element
    member _.inheritFromParent = h.MakeStyle("align-items", "inherit")

type CssAlignContentEngine<'Style>(h: CssHelper<'Style>) =
    /// The `align-content` property modifies the behavior of the `flex-wrap` property.
    /// It is similar to align-items, but instead of aligning flex items, it aligns flex lines.
    ///
    /// **Note**: There must be multiple lines of items for this property to have any effect!
    ///
    /// **Tip**: Use the justify-content property to align the items on the main-axis (horizontally).
    /// Default value. Lines stretch to take up the remaining space.
    member _.stretch = h.MakeStyle("align-content", "stretch")
    /// Lines are packed toward the center of the flex container.
    member _.center = h.MakeStyle("align-content", "center")
    /// Lines are packed toward the start of the flex container.
    member _.flexStart = h.MakeStyle("align-content", "flex-start")
    /// Lines are packed toward the end of the flex container.
    member _.flexEnd = h.MakeStyle("align-content", "flex-end")
    /// Lines are evenly distributed in the flex container.
    member _.spaceBetween = h.MakeStyle("align-content", "space-between")
    /// Lines are evenly distributed in the flex container, with half-size spaces on either end.
    member _.spaceAround = h.MakeStyle("align-content", "space-around")
    member _.initial = h.MakeStyle("align-content", "initial")
    member _.inheritFromParent = h.MakeStyle("align-content", "inherit")

type CssJustifyContentEngine<'Style>(h: CssHelper<'Style>) =
    /// The justify-content property aligns the flexible container's items when the items do not use all available space on the main-axis (horizontally).
    ///
    /// See https://www.w3schools.com/cssref/css3_pr_justify-content.asp for reference.
    ///
    /// **Tip**: Use the align-items property to align the items vertically.
    /// Default value. Items are positioned at the beginning of the container.
    member _.flexStart = h.MakeStyle("justify-content", "flex-start")
    /// Items are positioned at the end of the container.
    member _.flexEnd = h.MakeStyle("justify-content", "flex-end")
    /// Items are positioned at the center of the container
    member _.center = h.MakeStyle("justify-content", "center")
    /// Items are positioned with space between the lines
    member _.spaceBetween = h.MakeStyle("justify-content", "space-between")
    /// Items are positioned with space before, between, and after the lines.
    member _.spaceAround = h.MakeStyle("justify-content", "space-around")
    /// Sets this property to its default value.
    member _.initial = h.MakeStyle("justify-content", "initial")
    /// Inherits this property from its parent element.
    member _.inheritFromParent = h.MakeStyle("justify-content", "inherit")

type CssOutlineWidthEngine<'Style>(h: CssHelper<'Style>) =
    /// An outline is a line around an element.
    /// It is displayed around the margin of the element. However, it is different from the border property.
    /// The outline is not a part of the element's dimensions, therefore the element's width and height properties do not contain the width of the outline.
    /// Specifies a medium outline. This is default.
    member _.medium = h.MakeStyle("outline-width", "medium")
    /// Specifies a thin outline.
    member _.thin = h.MakeStyle("outline-width", "thin")
    /// Specifies a thick outline.
    member _.thick = h.MakeStyle("outline-width", "thick")
    /// Sets this property to its default value
    member _.initial = h.MakeStyle("outline-width", "initial")
    /// Inherits this property from its parent element
    member _.inheritFromParent = h.MakeStyle("outline-width", "inherit")

type CssListStyleTypeEngine<'Style>(h: CssHelper<'Style>) =
    /// Default value. The marker is a filled circle
    member _.disc = h.MakeStyle("list-style-type", "disc")
    /// The marker is traditional Armenian numbering
    member _.armenian = h.MakeStyle("list-style-type", "armenian")
    /// The marker is a circle
    member _.circle = h.MakeStyle("list-style-type", "circle")
    /// The marker is plain ideographic numbers
    member _.cjkIdeographic = h.MakeStyle("list-style-type", "cjk-ideographic")
    /// The marker is a number
    member _.decimal = h.MakeStyle("list-style-type", "decimal")
    /// The marker is a number with leading zeros (01, 02, 03, etc.)
    member _.decimalLeadingZero = h.MakeStyle("list-style-type", "decimal-leading-zero")
    /// The marker is traditional Georgian numbering
    member _.georgian = h.MakeStyle("list-style-type", "georgian")
    /// The marker is traditional Hebrew numbering
    member _.hebrew = h.MakeStyle("list-style-type", "hebrew")
    /// The marker is traditional Hiragana numbering
    member _.hiragana = h.MakeStyle("list-style-type", "hiragana")
    /// The marker is traditional Hiragana iroha numbering
    member _.hiraganaIroha = h.MakeStyle("list-style-type", "hiragana-iroha")
    /// The marker is traditional Katakana numbering
    member _.katakana = h.MakeStyle("list-style-type", "katakana")
    /// The marker is traditional Katakana iroha numbering
    member _.katakanaIroha = h.MakeStyle("list-style-type", "katakana-iroha")
    /// The marker is lower-alpha (a, b, c, d, e, etc.)
    member _.lowerAlpha = h.MakeStyle("list-style-type", "lower-alpha")
    /// The marker is lower-greek
    member _.lowerGreek = h.MakeStyle("list-style-type", "lower-greek")
    /// The marker is lower-latin (a, b, c, d, e, etc.)
    member _.lowerLatin = h.MakeStyle("list-style-type", "lower-latin")
    /// The marker is lower-roman (i, ii, iii, iv, v, etc.)
    member _.lowerRoman = h.MakeStyle("list-style-type", "lower-roman")
    /// No marker is shown
    member _.none = h.MakeStyle("list-style-type", "none")
    /// The marker is a square
    member _.square = h.MakeStyle("list-style-type", "square")
    /// The marker is upper-alpha (A, B, C, D, E, etc.)
    member _.upperAlpha = h.MakeStyle("list-style-type", "upper-alpha")
    /// The marker is upper-greek
    member _.upperGreek = h.MakeStyle("list-style-type", "upper-greek")
    /// The marker is upper-latin (A, B, C, D, E, etc.)
    member _.upperLatin = h.MakeStyle("list-style-type", "upper-latin")
    /// The marker is upper-roman (I, II, III, IV, V, etc.)
    member _.upperRoman = h.MakeStyle("list-style-type", "upper-roman")
    /// Sets this property to its default value.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_text-align&preval=initial
    member _.initial = h.MakeStyle("list-style-type", "initial")
    /// Inherits this property from its parent element.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_text-align&preval=initial
    member _.inheritFromParent = h.MakeStyle("list-style-type", "inherit")

type CssPropertyEngine<'Style>(h: CssHelper<'Style>) =
    member _.none = h.MakeStyle("list-style-image", "none")
    /// The path to the image to be used as a list-item marker
    member _.url (path: string) = h.MakeStyle("list-style-image", "url('" + path + "')")
    /// Sets this property to its default value.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_text-align&preval=initial
    member _.initial = h.MakeStyle("list-style-image", "initial")
    /// Inherits this property from its parent element.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_text-align&preval=initial
    member _.inheritFromParent = h.MakeStyle("list-style-image", "inherit")

type CssListStylePositionEngine<'Style>(h: CssHelper<'Style>) =
    /// The bullet points will be inside the list item
    member _.inside = h.MakeStyle("list-style-position", "inside")
    /// The bullet points will be outside the list item. This is default
    member _.outside = h.MakeStyle("list-style-position", "outside")
    /// Sets this property to its default value.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_text-align&preval=initial
    member _.initial = h.MakeStyle("list-style-position", "initial")
    /// Inherits this property from its parent element.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_text-align&preval=initial
    member _.inheritFromParent = h.MakeStyle("list-style-position", "inherit")

type CssTextDecorationLineEngine<'Style>(h: CssHelper<'Style>) =
    member _.none = h.MakeStyle("text-decoration-line", "none")
    member _.underline = h.MakeStyle("text-decoration-line", "underline")
    member _.overline = h.MakeStyle("text-decoration-line", "overline")
    member _.lineThrough = h.MakeStyle("text-decoration-line", "line-through")
    member _.initial = h.MakeStyle("text-decoration-line", "initial")
    /// Inherits this property from its parent element.
    member _.inheritFromParent = h.MakeStyle("text-decoration-line", "inherit")

type CssTextDecorationEngine<'Style>(h: CssHelper<'Style>) =
    member _.none = h.MakeStyle("text-decoration", "none")
    member _.underline = h.MakeStyle("text-decoration", "underline")
    member _.overline = h.MakeStyle("text-decoration", "overline")
    member _.lineThrough = h.MakeStyle("text-decoration", "line-through")
    member _.initial = h.MakeStyle("text-decoration", "initial")
    /// Inherits this property from its parent element.
    member _.inheritFromParent = h.MakeStyle("text-decoration", "inherit")

type CssTransformStyleEngine<'Style>(h: CssHelper<'Style>) =
    /// The `transform-style` property specifies how nested elements are rendered in 3D space.
    /// Specifies that child elements will NOT preserve its 3D position. This is default.
    member _.flat = h.MakeStyle("transform-style", "flat")
    /// Specifies that child elements will preserve its 3D position
    member _.preserve3D = h.MakeStyle("transform-style", "preserve-3d")
    member _.initial = h.MakeStyle("transform-style", "initial")
    /// Inherits this property from its parent element.
    member _.inheritFromParent = h.MakeStyle("transform-style", "inherit")

type CssTextTransformEngine<'Style>(h: CssHelper<'Style>) =
    /// No capitalization. The text renders as it is. This is default.
    member _.none = h.MakeStyle("text-transform", "none")
    /// Transforms the first character of each word to uppercase.
    member _.capitalize = h.MakeStyle("text-transform", "capitalize")
    /// Transforms all characters to uppercase.
    member _.uppercase = h.MakeStyle("text-transform", "uppercase")
    /// Transforms all characters to lowercase.
    member _.lowercase = h.MakeStyle("text-transform", "lowercase")
    member _.initial = h.MakeStyle("text-transform", "initial")
    /// Inherits this property from its parent element.
    member _.inheritFromParent = h.MakeStyle("text-transform", "inherit")

type CssTextOverflowEngine<'Style>(h: CssHelper<'Style>) =
    /// Default value. The text is clipped and not accessible.
    member _.clip = h.MakeStyle("text-overflow", "clip")
    /// Render an ellipsis ("...") to represent the clipped text.
    member _.ellipsis = h.MakeStyle("text-overflow", "ellipsis")
    /// Render the given asString to represent the clipped text.
    member _.initial = h.MakeStyle("text-overflow", "initial")
    /// Inherits this property from its parent element.
    member _.inheritFromParent = h.MakeStyle("text-overflow", "inherit")

type CssFilterEngine<'Style>(h: CssHelper<'Style>) =
    /// Defines visual effects like blur and saturation to an element.
    /// Default value. Specifies no effects.
    member _.none = h.MakeStyle("filter", "none")
    /// Applies a blur effect to the elemeen. A larger value will create more blur.
    ///
    /// This overload takes an integer that represents a percentage from 0 to 100.
    member _.blur(value: int) = h.MakeStyle("filter", "blur(" + (asString value) + "%)")
    /// Applies a blur effect to the elemeen. A larger value will create more blur.
    ///
    /// This overload takes a floating number that goes from 0 to 1,
    member _.blur(value: double) = h.MakeStyle("filter", "blur(" + (asString value) + ")")
    /// Adjusts the brightness of the elemeen
    ///
    /// This overload takes an integer that represents a percentage from 0 to 100.
    ///
    /// Values over 100% will provide brighter results.
    member _.brightness(value: int) = h.MakeStyle("filter", "brightness(" + (asString value) + "%)")
    /// Adjusts the brightness of the elemeen. A larger value will create more blur.
    ///
    /// This overload takes a floating number that goes from 0 to 1,
    member _.brightness(value: double) = h.MakeStyle("filter", "brightness(" + (asString value) + ")")
    /// Adjusts the contrast of the element.
    ///
    /// This overload takes an integer that represents a percentage from 0 to 100.
    member _.contrast(value: int) = h.MakeStyle("filter", "contrast(" + (asString value) + "%)")
    /// Adjusts the contrast of the element. A larger value will create more contrast.
    ///
    /// This overload takes a floating number that goes from 0 to 1
    member _.contrast(value: double) = h.MakeStyle("filter", "contrast(" + (asString value) + ")")
    /// Applies a drop shadow effect.
    member _.dropShadow(horizontalOffset: int, verticalOffset: int, blur: int, spread: int,  color: string) = h.MakeStyle("filter", "drop-shadow(" + (asString horizontalOffset) + "px " + (asString verticalOffset) + "px " + (asString blur) + "px " + (asString spread) + "px " + color + ")")
    /// Applies a drop shadow effect.
    member _.dropShadow(horizontalOffset: int, verticalOffset: int, blur: int, color: string) = h.MakeStyle("filter", "drop-shadow(" + (asString horizontalOffset) + "px " + (asString verticalOffset) + "px " + (asString blur) + "px " + color + ")")
    /// Applies a drop shadow effect.
    member _.dropShadow(horizontalOffset: int, verticalOffset: int, color: string) = h.MakeStyle("filter", "drop-shadow(" + (asString horizontalOffset) + "px " + (asString verticalOffset) + "px " + color + ")")
    /// Converts the image to grayscale
    ///
    /// This overload takes an integer that represents a percentage from 0 to 100.
    member _.grayscale(value: int) = h.MakeStyle("filter", "grayscale(" + (asString value) + "%)")
    /// Converts the image to grayscale
    ///
    /// This overload takes a floating number that goes from 0 to 1
    member _.grayscale(value: double) = h.MakeStyle("filter", "grayscale(" + (asString value) + ")")
    /// Applies a hue rotation on the image. The value defines the number of degrees around the color circle the image
    /// samples will be adjusted. 0deg is default, and represents the original image.
    ///
    /// **Note**: Maximum value is 360
    member _.hueRotate(degrees: int) = h.MakeStyle("filter", "hue-rotate(" + (asString degrees) + "deg)")
    /// Inverts the element.
    ///
    /// This overload takes an integer that represents a percentage from 0 to 100.
    member _.invert(value: int) = h.MakeStyle("filter", "invert(" + (asString value) + "%)")
    /// Inverts the element.
    ///
    /// This overload takes a floating number that goes from 0 to 1
    member _.invert(value: double) = h.MakeStyle("filter", "invert(" + (asString value) + ")")
    /// Sets the opacity of the element.
    ///
    /// This overload takes an integer that represents a percentage from 0 to 100.
    member _.opacity(value: int) = h.MakeStyle("filter", "opacity(" + (asString value) + "%)")
    /// Sets the opacity of the element.
    ///
    /// This overload takes a floating number that goes from 0 to 1
    member _.opacity(value: double) = h.MakeStyle("filter", "opacity(" + (asString value) + ")")
    /// Sets the saturation of the element.
    ///
    /// This overload takes an integer that represents a percentage from 0 to 100.
    member _.saturate(value: int) = h.MakeStyle("filter", "saturate(" + (asString value) + "%)")
    /// Sets the saturation of the element.
    ///
    /// This overload takes a floating number that goes from 0 to 1
    member _.saturate(value: double) = h.MakeStyle("filter", "saturate(" + (asString value) + ")")
    /// Applies Sepia filter to the element.
    ///
    /// This overload takes an integer that represents a percentage from 0 to 100.
    member _.sepia(value: int) = h.MakeStyle("filter", "sepia(" + (asString value) + "%)")
    /// Applies Sepia filter to the element.
    ///
    /// This overload takes a floating number that goes from 0 to 1
    member _.sepia(value: double) = h.MakeStyle("filter", "sepia(" + (asString value) + ")")
    /// The url() function takes the location of an XML file that specifies an SVG filter, and may include an anchor to a specific filter element.
    ///
    /// Example: `filter: url(svg-url#element-id)`
    member _.url(value: string) = h.MakeStyle("filter", "url(" + value + ")")
    /// Sets this property to its default value.
    member _.initial = h.MakeStyle("filter", "initial")
    /// Inherits this property from its parent element.
    member _.inheritFromParent = h.MakeStyle("filter", "inherit")

type CssBorderCollapseEngine<'Style>(h: CssHelper<'Style>) =
    /// Sets whether table borders should collapse into a single border or be separated as in standard HTML.
    /// Borders are separated; each cell will display its own borders. This is default.
    member _.separate = h.MakeStyle("border-collapse", "separate")
    /// Borders are collapsed into a single border when possible (border-spacing and empty-cells properties have no effect)
    member _.collapse = h.MakeStyle("border-collapse", "collapse")
    /// Sets this property to its default value
    member _.initial = h.MakeStyle("border-collapse", "initial")
    /// Inherits this property from its parent element.
    member _.inheritFromParent = h.MakeStyle("border-collapse", "inherit")

type CssBackgroundSizeEngine<'Style>(h: CssHelper<'Style>) =
    /// Specifies the size of the background images
    /// Default value. The background image is displayed in its original size
    ///
    /// See [example here](https://www.w3schools.com/cssref/playit.asp?filename=playcss_background-size&preval=auto)
    member _.auto = h.MakeStyle("background-size", "auto")
    /// Resize the background image to cover the entire container, even if it has to stretch the image or cut a little bit off one of the edges.
    ///
    /// See [example here](https://www.w3schools.com/cssref/playit.asp?filename=playcss_background-size&preval=cover)
    member _.cover = h.MakeStyle("background-size", "cover")
    /// Resize the background image to make sure the image is fully visible
    ///
    /// See [example here](https://www.w3schools.com/cssref/playit.asp?filename=playcss_background-size&preval=contain)
    member _.contain = h.MakeStyle("background-size", "contain")
    /// Sets this property to its default value.
    member _.initial = h.MakeStyle("background-size", "initial")
    /// Inherits this property from its parent element.
    member _.inheritFromParent = h.MakeStyle("background-size", "inherit")

type CssTextDecorationStyleEngine<'Style>(h: CssHelper<'Style>) =
    /// Default value. The line will display as a single line.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_text-decoration-style&preval=solid
    member _.solid = h.MakeStyle("text-decoration-style", "solid")
    /// The line will display as a double line.
    ///
    /// https://www.w3schools.com/cssref/playit.asp?filename=playcss_text-decoration-style&preval=double
    member _.double = h.MakeStyle("text-decoration-style", "double")
    /// The line will display as a dotted line.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_text-decoration-style&preval=dotted
    member _.dotted = h.MakeStyle("text-decoration-style", "dotted")
    /// The line will display as a dashed line.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_text-decoration-style&preval=dashed
    member _.dashed = h.MakeStyle("text-decoration-style", "dashed")
    /// The line will display as a wavy line.
    ///
    /// https://www.w3schools.com/cssref/playit.asp?filename=playcss_text-decoration-style&preval=wavy
    member _.wavy = h.MakeStyle("text-decoration-style", "wavy")
    /// Sets this property to its default value.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_text-decoration-style&preval=initial
    member _.initial = h.MakeStyle("text-decoration-style", "initial")
    /// Inherits this property from its parent element.
    member _.inheritFromParent = h.MakeStyle("text-decoration-style", "inherit")

type CssFontStretchEngine<'Style>(h: CssHelper<'Style>) =
    /// Makes the text as narrow as it gets.
    member _.ultraCondensed = h.MakeStyle("font-stretch", "ultra-condensed")
    /// Makes the text narrower than condensed, but not as narrow as ultra-condensed
    member _.extraCondensed = h.MakeStyle("font-stretch", "extra-condensed")
    /// Makes the text narrower than semi-condensed, but not as narrow as extra-condensed.
    member _.condensed = h.MakeStyle("font-stretch", "condensed")
    /// Makes the text narrower than normal, but not as narrow as condensed.
    member _.semiCondensed = h.MakeStyle("font-stretch", "semi-condensed")
    /// Default value. No font stretching
    member _.normal = h.MakeStyle("font-stretch", "normal")
    /// Makes the text wider than normal, but not as wide as expanded
    member _.semiExpanded = h.MakeStyle("font-stretch", "semi-expanded")
    /// Makes the text wider than semi-expanded, but not as wide as extra-expanded
    member _.expanded = h.MakeStyle("font-stretch", "expanded")
    /// Makes the text wider than expanded, but not as wide as ultra-expanded
    member _.extraExpanded = h.MakeStyle("font-stretch", "extra-expanded")
    /// Makes the text as wide as it gets.
    member _.ultraExpanded = h.MakeStyle("font-stretch", "ultra-expanded")
    /// Sets this property to its default value.
    member _.initial = h.MakeStyle("font-stretch", "initial")
    /// Inherits this property from its parent element.
    member _.inheritFromParent = h.MakeStyle("font-stretch", "inherit")

type CssFloatStyleEngine<'Style>(h: CssHelper<'Style>) =
    /// Specifies how an element should float.
    ///
    /// **Note**: Absolutely positioned elements ignores the float property!
    /// The element does not float, (will be displayed just where it occurs in the text). This is default
    member _.none = h.MakeStyle("float", "none")
    member _.left = h.MakeStyle("float", "left")
    member _.right = h.MakeStyle("float", "right")
    /// Sets this property to its default value.
    member _.initial = h.MakeStyle("float", "initial")
    /// Inherits this property from its parent element.
    member _.inheritFromParent = h.MakeStyle("float", "inherit")

type CssVerticalAlignEngine<'Style>(h: CssHelper<'Style>) =
    /// The element is aligned with the baseline of the parent. This is default.
    member _.baseline = h.MakeStyle("vertical-align", "baseline")
    /// The element is aligned with the subscript baseline of the parent
    member _.sub = h.MakeStyle("vertical-align", "sup")
    /// The element is aligned with the superscript baseline of the parent.
    member _.super = h.MakeStyle("vertical-align", "super")
    /// The element is aligned with the top of the tallest element on the line.
    member _.top = h.MakeStyle("vertical-align", "top")
    /// The element is aligned with the top of the parent element's font.
    member _.textTop = h.MakeStyle("vertical-align", "text-top")
    /// The element is placed in the middle of the parent element.
    member _.middle = h.MakeStyle("vertical-align", "middle")
    /// The element is aligned with the lowest element on the line.
    member _.bottom = h.MakeStyle("vertical-align", "bottom")
    /// The element is aligned with the bottom of the parent element's font
    member _.textBottom = h.MakeStyle("vertical-align", "text-bottom")
    /// Sets this property to its default value.
    member _.initial = h.MakeStyle("vertical-align", "initial")
    /// Inherits this property from its parent element.
    member _.inheritFromParent = h.MakeStyle("vertical-align", "inherit")

type CssWritingModeEngine<'Style>(h: CssHelper<'Style>) =
    /// Specifies whether lines of text are laid out horizontally or vertically.
    /// Let the content flow horizontally from left to right, vertically from top to bottom
    member _.horizontalTopBottom = h.MakeStyle("writing-mode", "horizontal-tb")
    /// Let the content flow vertically from top to bottom, horizontally from right to left
    member _.verticalRightLeft = h.MakeStyle("writing-mode", "vertical-rl")
    /// Let the content flow vertically from top to bottom, horizontally from left to right
    member _.verticalLeftRight = h.MakeStyle("writing-mode", "vertical-lr")
    /// Sets this property to its default value.
    member _.initial = h.MakeStyle("writing-mode", "initial")
    /// Inherits this property from its parent element.
    member _.inheritFromParent = h.MakeStyle("writing-mode", "inherit")

type CssAnimationTimingFunctionEngine<'Style>(h: CssHelper<'Style>) =
    /// Default value. Specifies a animation effect with a slow start, then fast, then end slowly (equivalent to cubic-bezier(0.25,0.1,0.25,1)).
    member _.ease = h.MakeStyle("animation-timing-function", "ease")
    /// Specifies a animation effect with the same speed from start to end (equivalent to cubic-bezier(0,0,1,1))
    member _.linear = h.MakeStyle("animation-timing-function", "linear")
    /// Specifies a animation effect with a slow start (equivalent to cubic-bezier(0.42,0,1,1)).
    member _.easeIn = h.MakeStyle("animation-timing-function", "ease-in")
    /// Specifies a animation effect with a slow end (equivalent to cubic-bezier(0,0,0.58,1)).
    member _.easeOut = h.MakeStyle("animation-timing-function", "ease-out")
    /// Specifies a animation effect with a slow start and end (equivalent to cubic-bezier(0.42,0,0.58,1))
    member _.easeInOut = h.MakeStyle("animation-timing-function", "ease-in-out")
    /// Define your own values in the cubic-bezier function. Possible values are numeric values from 0 to 1
    member _.cubicBezier(n1: float, n2: float, n3: float, n4: float) = h.MakeStyle("animation-timing-function", "cubic-bezier(" + (asString n1) + "," + (asString n2) + "," + (asString n3) + ", " + (asString n4) + ")")
    /// Sets this property to its default value
    member _.initial = h.MakeStyle("animation-timing-function", "initial")
    /// Inherits this property from its parent element.
    member _.inheritFromParent = h.MakeStyle("animation-timing-function", "inherit")

type CssTransitionTimingFunctionEngine<'Style>(h: CssHelper<'Style>) =
    /// Default value. Specifies a transition effect with a slow start, then fast, then end slowly (equivalent to cubic-bezier(0.25,0.1,0.25,1)).
    member _.ease = h.MakeStyle("transition-timing-function", "ease")
    /// Specifies a transition effect with the same speed from start to end (equivalent to cubic-bezier(0,0,1,1))
    member _.linear = h.MakeStyle("transition-timing-function", "linear")
    /// Specifies a transition effect with a slow start (equivalent to cubic-bezier(0.42,0,1,1)).
    member _.easeIn = h.MakeStyle("transition-timing-function", "ease-in")
    /// Specifies a transition effect with a slow end (equivalent to cubic-bezier(0,0,0.58,1)).
    member _.easeOut = h.MakeStyle("transition-timing-function", "ease-out")
    /// Specifies a transition effect with a slow start and end (equivalent to cubic-bezier(0.42,0,0.58,1))
    member _.easeInOut = h.MakeStyle("transition-timing-function", "ease-in-out")
    /// Equivalent to steps(1, start)
    member _.stepStart = h.MakeStyle("transition-timing-function", "step-start")
    /// Equivalent to steps(1, end)
    member _.stepEnd = h.MakeStyle("transition-timing-function", "step-end")
    /// Define your own values in the cubic-bezier function. Possible values are numeric values from 0 to 1
    member _.cubicBezier(n1: float, n2: float, n3: float, n4: float) = h.MakeStyle("transition-timing-function", "cubic-bezier(" + (asString n1) + "," + (asString n2) + "," + (asString n3) + ", " + (asString n4) + ")")
    /// Sets this property to its default value
    member _.initial = h.MakeStyle("transition-timing-function", "initial")
    /// Inherits this property from its parent element.
    member _.inheritFromParent = h.MakeStyle("transition-timing-function", "inherit")

type CssUserSelectEngine<'Style>(h: CssHelper<'Style>) =
    /// Default. Text can be selected if the browser allows it.
    member _.auto = h.MakeStyle("user-select", "auto")
    /// Prevents text selection.
    member _.none = h.MakeStyle("user-select", "none")
    /// The text can be selected by the user.
    member _.text = h.MakeStyle("user-select", "text")
    /// Text selection is made with one click instead of a double-click.
    member _.all = h.MakeStyle("user-select", "all")
    /// Sets this property to its default value.
    member _.initial = h.MakeStyle("user-select", "initial")
    /// Inherits this property from its parent element.
    member _.inheritFromParent = h.MakeStyle("user-select", "inherit")

type CssBorderStyleEngine<'Style>(h: CssHelper<'Style>) =
    /// Specifies a dotted border.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_border-style&preval=dotted
    member _.dotted = h.MakeStyle("border-style", "dotted")
    /// Specifies a dashed border.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_border-style&preval=dotted
    member _.dashed = h.MakeStyle("border-style", "dashed")
    /// Specifies a solid border.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_border-style&preval=dotted
    member _.solid = h.MakeStyle("border-style", "solid")
    /// Specifies a double border.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_border-style&preval=dotted
    member _.double = h.MakeStyle("border-style", "double")
    /// Specifies a 3D grooved border. The effect depends on the border-color value.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_border-style&preval=dotted
    member _.groove = h.MakeStyle("border-style", "groove")
    /// Specifies a 3D ridged border. The effect depends on the border-color value.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_border-style&preval=dotted
    member _.ridge = h.MakeStyle("border-style", "ridge")
    /// Specifies a 3D inset border. The effect depends on the border-color value.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_border-style&preval=dotted
    member _.inset = h.MakeStyle("border-style", "inset")
    /// Specifies a 3D outset border. The effect depends on the border-color value.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_border-style&preval=dotted
    member _.outset = h.MakeStyle("border-style", "outset")
    /// Default value. Specifies no border.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_border-style&preval=dotted
    member _.none = h.MakeStyle("border-style", "none")
    /// The same as "none", except in border conflict resolution for table elements.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_border-style&preval=hidden
    member _.hidden = h.MakeStyle("border-style", "hidden")
    /// Sets this property to its default value.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_border-style&preval=hidden
    ///
    /// Read about initial value https://www.w3schools.com/cssref/css_initial.asp
    member _.initial = h.MakeStyle("border-style", "initial")
    /// Inherits this property from its parent element.
    ///
    /// See example https://www.w3schools.com/cssref/playit.asp?filename=playcss_border-style&preval=hidden
    ///
    /// Read about inherit https://www.w3schools.com/cssref/css_inherit.asp
    member _.inheritFromParent = h.MakeStyle("border-style", "inherit")

type CssTableLayoutEngine<'Style>(h: CssHelper<'Style>) =
    /// Defines the algorithm used to lay out table cells, rows, and columns.
    ///
    /// **Tip:** The main benefit of table-layout: fixed; is that the table renders much faster. On large tables,
    /// users will not see any part of the table until the browser has rendered the whole table. So, if you use
    /// table-layout: fixed, users will see the top of the table while the browser loads and renders rest of the
    /// table. This gives the impression that the page loads a lot quicker!
    /// Browsers use an automatic table layout algorithm. The column width is set by the widest unbreakable
    /// content in the cells. The content will dictate the layout
    member _.auto = h.MakeStyle("table-layout", "auto")
    /// Sets a fixed table layout algorithm. The table and column widths are set by the widths of table and col
    /// or by the width of the first row of cells. Cells in other rows do not affect column widths. If no widths
    /// are present on the first row, the column widths are divided equally across the table, regardless of content
    /// inside the cells
    member _.fixed' = h.MakeStyle("table-layout", "fixed")
    /// Sets this property to its default value.
    member _.initial = h.MakeStyle("table-layout", "initial")
    /// Inherits this property from its parent element.
    member _.inheritFromParent = h.MakeStyle("table-layout", "inherit")

type CssCursorEngine<'Style>(h: CssHelper<'Style>) =
    /// See documentation at https://developer.mozilla.org/en-US/docs/Web/CSS/cursor
    /// The User Agent will determine the cursor to display based on the current context. E.g., equivalent to text when hovering text.
    member _.auto = h.MakeStyle("cursor", "auto")
    /// The cursor indicates an alias of something is to be created
    member _.alias = h.MakeStyle("cursor", "alias")
    /// The platform-dependent default cursor. Typically an arrow.
    member _.defaultCursor = h.MakeStyle("cursor", "default")
    /// No cursor is rendered.
    member _.none = h.MakeStyle("cursor", "none")
    /// A context menu is available.
    member _.contextMenu = h.MakeStyle("cursor", "context-menu")
    /// Help information is available.
    member _.help = h.MakeStyle("cursor", "help")
    /// The cursor is a pointer that indicates a link. Typically an image of a pointing hand.
    member _.pointer = h.MakeStyle("cursor", "pointer")
    /// The program is busy in the background, but the user can still interact with the interface (in contrast to `wait`).
    member _.progress = h.MakeStyle("cursor", "progress")
    /// The program is busy, and the user can't interact with the interface (in contrast to progress). Sometimes an image of an hourglass or a watch.
    member _.wait = h.MakeStyle("cursor", "wait")
    /// The table cell or set of cells can be selected.
    member _.cell = h.MakeStyle("cursor", "cell")
    /// Cross cursor, often used to indicate selection in a bitmap.
    member _.crosshair = h.MakeStyle("cursor", "crosshair")
    /// The text can be selected. Typically the shape of an I-beam.
    member _.text = h.MakeStyle("cursor", "text")
    /// The vertical text can be selected. Typically the shape of a sideways I-beam.
    member _.verticalText = h.MakeStyle("cursor", "vertical-text")
    /// Something is to be copied.
    member _.copy = h.MakeStyle("cursor", "copy")
    /// Something is to be moved.
    member _.move = h.MakeStyle("cursor", "move")
    /// An item may not be dropped at the current location. On Windows and Mac OS X, `no-drop` is the same as `not-allowed`.
    member _.noDrop = h.MakeStyle("cursor", "no-drop")
    /// The requested action will not be carried out.
    member _.notAllowed = h.MakeStyle("cursor", "not-allowed")
    /// Something can be grabbed (dragged to be moved).
    member _.grab = h.MakeStyle("cursor", "grab")
    /// Something is being grabbed (dragged to be moved).
    member _.grabbing = h.MakeStyle("cursor", "grabbing")
    /// Something can be scrolled in any direction (panned).
    member _.allScroll = h.MakeStyle("cursor", "all-scroll")
    /// The item/column can be resized horizontally. Often rendered as arrows pointing left and right with a vertical bar separating them.
    member _.columnResize = h.MakeStyle("cursor", "col-resize")
    /// The item/row can be resized vertically. Often rendered as arrows pointing up and down with a horizontal bar separating them.
    member _.rowResize = h.MakeStyle("cursor", "row-resize")
    /// Directional resize arrow
    member _.northResize = h.MakeStyle("cursor", "n-resize")
    /// Directional resize arrow
    member _.eastResize = h.MakeStyle("cursor", "e-resize")
    /// Directional resize arrow
    member _.southResize = h.MakeStyle("cursor", "s-resize")
    /// Directional resize arrow
    member _.westResize = h.MakeStyle("cursor", "w-resize")
    /// Directional resize arrow
    member _.northEastResize = h.MakeStyle("cursor", "ne-resize")
    /// Directional resize arrow
    member _.northWestResize = h.MakeStyle("cursor", "nw-resize")
    /// Directional resize arrow
    member _.southEastResize = h.MakeStyle("cursor", "se-resize")
    /// Directional resize arrow
    member _.southWestResize = h.MakeStyle("cursor", "sw-resize")
    /// Directional resize arrow
    member _.eastWestResize = h.MakeStyle("cursor", "ew-resize")
    /// Directional resize arrow
    member _.northSouthResize = h.MakeStyle("cursor", "ns-resize")
    /// Directional resize arrow
    member _.northEastSouthWestResize = h.MakeStyle("cursor", "nesw-resize")
    /// Directional resize arrow
    member _.northWestSouthEastResize = h.MakeStyle("cursor", "nwse-resize")
    /// Something can be zoomed (magnified) in
    member _.zoomIn = h.MakeStyle("cursor", "zoom-in")
    /// Something can be zoomed out
    member _.zoomOut = h.MakeStyle("cursor", "zoom-out")

type CssOutlineStyleEngine<'Style>(h: CssHelper<'Style>) =
    /// An outline is a line that is drawn around elements (outside the borders) to make the element "stand out".
    ///
    /// The outline-style property specifies the style of an outline.
    ///
    /// An outline is a line around an element. It is displayed around the margin of the element. However, it is different from the border property.
    ///
    /// The outline is not a part of the element's dimensions, therefore the element's width and height properties do not contain the width of the outline.
    /// Permits the user agent to render a custom outline style.
    member _.auto = h.MakeStyle("outline-style", "auto")
    /// Specifies no outline. This is default.
    member _.none = h.MakeStyle("outline-style", "none")
    /// Specifies a hidden outline
    member _.hidden = h.MakeStyle("outline-style", "hidden")
    /// Specifies a dotted outline
    member _.dotted = h.MakeStyle("outline-style", "dotted")
    /// Specifies a dashed outline
    member _.dashed = h.MakeStyle("outline-style", "dashed")
    /// Specifies a solid outline
    member _.solid = h.MakeStyle("outline-style", "solid")
    /// Specifies a double outliner
    member _.double = h.MakeStyle("outline-style", "double")
    /// Specifies a 3D grooved outline. The effect depends on the outline-color value
    member _.groove = h.MakeStyle("outline-style", "groove")
    /// Specifies a 3D ridged outline. The effect depends on the outline-color value
    member _.ridge = h.MakeStyle("outline-style", "ridge")
    /// Specifies a 3D inset  outline. The effect depends on the outline-color value
    member _.inset = h.MakeStyle("outline-style", "inset")
    /// Specifies a 3D outset outline. The effect depends on the outline-color value
    member _.outset = h.MakeStyle("outline-style", "outset")
    /// Sets this property to its default value
    member _.initial = h.MakeStyle("outline-style", "initial")
    /// Inherits this property from its parent element
    member _.inheritFromParent = h.MakeStyle("outline-style", "inherit")
    /// Resets to its inherited value if the property naturally inherits from its parent,
    /// and to its initial value if not.
    member _.unset = h.MakeStyle("outline-style", "unset")

type CssBackgroundPositionEngine<'Style>(h: CssHelper<'Style>) =
    /// The background image will scroll with the page. This is default.
    member _.scroll = h.MakeStyle("background-position", "scroll")
    /// The background image will not scroll with the page.
    member _.fixedNoScroll = h.MakeStyle("background-position", "fixed")
    /// The background image will scroll with the element's contents.
    member _.local = h.MakeStyle("background-position", "local")
    /// Sets this property to its default value.
    member _.initial = h.MakeStyle("background-position", "initial")
    /// Inherits this property from its parent element.
    member _.inheritFromParent = h.MakeStyle("background-position", "inherit")

type CssBackgroundBlendModeEngine<'Style>(h: CssHelper<'Style>) =
    /// This property defines the blending mode of each background layer (color and/or image).
    /// This is default. Sets the blending mode to normal.
    member _.normal = h.MakeStyle("background-blend-mode", "normal")
    /// Sets the blending mode to screen
    member _.screen = h.MakeStyle("background-blend-mode", "screen")
    /// Sets the blending mode to overlay
    member _.overlay = h.MakeStyle("background-blend-mode", "overlay")
    /// Sets the blending mode to darken
    member _.darken = h.MakeStyle("background-blend-mode", "darken")
    /// Sets the blending mode to multiply
    member _.lighten = h.MakeStyle("background-blend-mode", "lighten")
    /// Sets the blending mode to color-dodge
    member _.collorDodge = h.MakeStyle("background-blend-mode", "color-dodge")
    /// Sets the blending mode to saturation
    member _.saturation = h.MakeStyle("background-blend-mode", "saturation")
    /// Sets the blending mode to color
    member _.color = h.MakeStyle("background-blend-mode", "color")
    /// Sets the blending mode to luminosity
    member _.luminosity = h.MakeStyle("background-blend-mode", "luminosity")

type CssBackgroundClipEngine<'Style>(h: CssHelper<'Style>) =
    /// Defines how far the background (color or image) should extend within an element.
    /// Default value. The background extends behind the border.
    member _.borderBox = h.MakeStyle("background-clip", "border-box")
    /// The background extends to the inside edge of the border.
    member _.paddingBox = h.MakeStyle("background-clip", "padding-box")
    /// The background extends to the edge of the content box.
    member _.contentBox = h.MakeStyle("background-clip", "content-box")
    /// Sets this property to its default value.
    member _.initial = h.MakeStyle("background-clip", "initial")
    /// Inherits this property from its parent element.
    member _.inheritFromParent = h.MakeStyle("background-clip", "inherit")

type CssTransformEngine<'Style>(h: CssHelper<'Style>) =
    /// Defines that there should be no transformation.
    member _.none = h.MakeStyle("transform", "none")
    /// Defines a 2D transformation, using a matrix of six values.
    member _.matrix(x1: int, y1: int, z1: int, x2: int, y2: int, z2: int) = h.MakeStyle("transform", "matrix(" + (asString x1) + "," + (asString y1) + "," + (asString z1) + "," + (asString x2) + "," + (asString y2) + ", " + (asString z2) + ")")
    /// Defines a 2D translation.
    member _.translate(x: int, y: int) = h.MakeStyle("transform", "translate(" + (asString x) + "px," + (asString y) + "px)")
    /// Defines a 2D translation.
    member _.translate(x: ICssUnit, y: ICssUnit) = h.MakeStyle("transform", "translate(" + (asString x) + ", " + (asString y) + ")")
    /// Defines a 3D translation.
    member _.translate3D(x: int, y: int, z: int) = h.MakeStyle("transform", "translate3d(" + (asString x) + "px," + (asString y) + "px," + (asString z) + "px)")
    /// Defines a 3D translation.
    member _.translate3D(x: ICssUnit, y: ICssUnit, z: ICssUnit) = h.MakeStyle("transform", "translate3d(" + (asString x) + "," + (asString y) + ", " + (asString z) + ")")
    /// Defines a translation, using only the value for the X-axis.
    member _.translateX(x: int) = h.MakeStyle("transform", "translateX(" + (asString x) + "px)")
    /// Defines a translation, using only the value for the X-axis.
    member _.translateX(x: ICssUnit) = h.MakeStyle("transform", "translateX(" + (asString x) + ")")
    /// Defines a translation, using only the value for the Y-axis
    member _.translateY(y: int) = h.MakeStyle("transform", "translateY(" + (asString y) + "px)")
    /// Defines a translation, using only the value for the Y-axis
    member _.translateY(y: ICssUnit) = h.MakeStyle("transform", "translateY(" + (asString y) + ")")
    /// Defines a 3D translation, using only the value for the Z-axis
    /// Defines a 3D translation, using only the value for the Z-axis
    member _.translateZ(z: ICssUnit) = h.MakeStyle("transform", "translateZ(" + (asString z) + ")")
    /// Defines a 2D scale transformation.
    member _.scale(x: int, y: int) = h.MakeStyle("transform", "scale(" + (asString x) + ", " + (asString y) + ")")
    /// Defines a scale transformation.
    /// Defines a scale transformation.
    member _.scale(n: float) = h.MakeStyle("transform", "scale(" + (asString n) + ")")
    /// Defines a 3D scale transformation
    member _.scale3D(x: int, y: int, z: int) = h.MakeStyle("transform", "scale3d(" + (asString x) + "," + (asString y) + ", " + (asString z) + ")")
    /// Defines a scale transformation by giving a value for the X-axis.
    member _.scaleX(x: int) = h.MakeStyle("transform", "scaleX(" + (asString x) + ")")
    /// Defines a scale transformation by giving a value for the Y-axis.
    member _.scaleY(y: int) = h.MakeStyle("transform", "scaleY(" + (asString y) + ")")
    /// Defines a 3D translation, using only the value for the Z-axis
    member _.scaleZ(z: int) = h.MakeStyle("transform", "scaleZ(" + (asString z) + ")")
    /// Defines a 2D rotation, the angle is specified in the parameter.
    member _.rotate(deg: int) = h.MakeStyle("transform", "rotate(" + (asString deg) + "deg)")
    /// Defines a 2D rotation, the angle is specified in the parameter.
    member _.rotate(deg: float) = h.MakeStyle("transform", "rotate(" + (asString deg) + "deg)")
    /// Defines a 3D rotation along the X-axis.
    member _.rotateX(deg: float) = h.MakeStyle("transform", "rotateX(" + (asString deg) + "deg)")
    /// Defines a 3D rotation along the X-axis.
    member _.rotateX(deg: int) = h.MakeStyle("transform", "rotateX(" + (asString deg) + "deg)")
    /// Defines a 3D rotation along the Y-axis
    member _.rotateY(deg: float) = h.MakeStyle("transform", "rotateY(" + (asString deg) + "deg)")
    /// Defines a 3D rotation along the Y-axis
    member _.rotateY(deg: int) = h.MakeStyle("transform", "rotateY(" + (asString deg) + "deg)")
    /// Defines a 3D rotation along the Z-axis
    member _.rotateZ(deg: float) = h.MakeStyle("transform", "rotateZ(" + (asString deg) + "deg)")
    /// Defines a 3D rotation along the Z-axis
    member _.rotateZ(deg: int) = h.MakeStyle("transform", "rotateZ(" + (asString deg) + "deg)")
    /// Defines a 2D skew transformation along the X- and the Y-axis.
    member _.skew(xAngle: int, yAngle: int) = h.MakeStyle("transform", "skew(" + (asString xAngle) + "deg," + (asString yAngle) + "deg)")
    /// Defines a 2D skew transformation along the X- and the Y-axis.
    member _.skew(xAngle: float, yAngle: float) = h.MakeStyle("transform", "skew(" + (asString xAngle) + "deg," + (asString yAngle) + "deg)")
    /// Defines a 2D skew transformation along the X-axis
    member _.skewX(xAngle: int) = h.MakeStyle("transform", "skewX(" + (asString xAngle) + "deg)")
    /// Defines a 2D skew transformation along the X-axis
    member _.skewX(xAngle: float) = h.MakeStyle("transform", "skewX(" + (asString xAngle) + "deg)")
    /// Defines a 2D skew transformation along the Y-axis
    member _.skewY(xAngle: int) = h.MakeStyle("transform", "skewY(" + (asString xAngle) + "deg)")
    /// Defines a 2D skew transformation along the Y-axis
    member _.skewY(xAngle: float) = h.MakeStyle("transform", "skewY(" + (asString xAngle) + "deg)")
    /// Defines a perspective view for a 3D transformed element
    member _.perspective(n: int) = h.MakeStyle("transform", "perspective(" + (asString n) + ")")
    /// Sets this property to its default value.
    member _.initial = h.MakeStyle("transform", "initial")
    /// Inherits this property from its parent element.
    member _.inheritFromParent = h.MakeStyle("transform", "inherit")

type CssMarginEngine<'Style>(h: CssHelper<'Style>) =
    member _.auto = h.MakeStyle("margin", "auto")

type CssDirectionEngine<'Style>(h: CssHelper<'Style>) =
    /// The direction property specifies the text direction/writing direction within a block-level element.
    /// Text direction goes from right-to-left
    member _.rightToLeft = h.MakeStyle("direction", "rtl")
    /// Text direction goes from left-to-right. This is default
    member _.leftToRight = h.MakeStyle("direction", "ltr")
    /// Sets this property to its default value.
    member _.initial = h.MakeStyle("direction", "initial")
    /// Inherits this property from its parent element.
    member _.inheritFromParent = h.MakeStyle("direction", "inherit")

type CssEmptyCellsEngine<'Style>(h: CssHelper<'Style>) =
    /// Sets whether or not to display borders on empty cells in a table.
    /// Display borders on empty cells. This is default
    member _.show = h.MakeStyle("empty-cells", "show")
    /// Hide borders on empty cells
    member _.hide = h.MakeStyle("empty-cells", "hide")
    /// Sets this property to its default value
    member _.initial = h.MakeStyle("empty-cells", "initial")
    /// Inherits this property from its parent element
    member _.inheritFromParent = h.MakeStyle("empty-cells", "inherit")

type CssAnimationDirectionEngine<'Style>(h: CssHelper<'Style>) =
    /// Sets whether or not the animation should play in reverse on alternate cycles.
    /// Default value. The animation should be played as normal
    member _.normal = h.MakeStyle("animation-direction", "normal")
    /// The animation should play in reverse direction
    member _.reverse = h.MakeStyle("animation-direction", "reverse")
    /// The animation will be played as normal every odd time (1, 3, 5, etc..) and in reverse direction every even time (2, 4, 6, etc...).
    member _.alternate = h.MakeStyle("animation-direction", "alternate")
    /// The animation will be played in reverse direction every odd time (1, 3, 5, etc..) and in a normal direction every even time (2,4,6,etc...)
    member _.alternateReverse = h.MakeStyle("animation-direction", "alternate-reverse")
    /// Sets this property to its default value
    member _.initial = h.MakeStyle("animation-direction", "initial")
    /// Inherits this property from its parent element.
    member _.inheritFromParent = h.MakeStyle("animation-direction", "inherit")

type CssAnimationPlayStateEngine<'Style>(h: CssHelper<'Style>) =
    /// Default value. Specifies that the animation is running.
    member _.running = h.MakeStyle("animation-play-state", "running")
    /// Specifies that the animation is paused
    member _.paused = h.MakeStyle("animation-play-state", "paused")
    /// Sets this property to its default value
    member _.initial = h.MakeStyle("animation-play-state", "initial")
    /// Inherits this property from its parent element.
    member _.inheritFromParent = h.MakeStyle("animation-play-state", "inherit")

type CssAnimationIterationCountEngine<'Style>(h: CssHelper<'Style>) =
    /// Specifies that the animation should be played infinite times (forever)
    member _.infinite = h.MakeStyle("animation-iteration-count", "infinite")
    /// Sets this property to its default value
    member _.initial = h.MakeStyle("animation-iteration-count", "initial")
    /// Inherits this property from its parent element.
    member _.inheritFromParent = h.MakeStyle("animation-iteration-count", "inherit")

type CssAnimationFillModeEngine<'Style>(h: CssHelper<'Style>) =
    /// Specifies a style for the element when the animation is not playing (before it starts, after it ends, or both).
    /// Default value. Animation will not apply any styles to the element before or after it is executing
    member _.none = h.MakeStyle("animation-fill-mode", "none")
    /// The element will retain the style values that is set by the last keyframe (depends on animation-direction and animation-iteration-count).
    member _.forwards = h.MakeStyle("animation-fill-mode", "forwards")
    /// The element will get the style values that is set by the first keyframe (depends on animation-direction), and retain this during the animation-delay period
    member _.backwards = h.MakeStyle("animation-fill-mode", "backwards")
    /// The animation will follow the rules for both forwards and backwards, extending the animation properties in both directions
    member _.both = h.MakeStyle("animation-fill-mode", "both")
    /// Sets this property to its default value
    member _.initial = h.MakeStyle("animation-fill-mode", "initial")
    /// Inherits this property from its parent element
    member _.inheritFromParent = h.MakeStyle("animation-fill-mode", "inherit")

type CssBackgroundRepeatEngine<'Style>(h: CssHelper<'Style>) =
    /// The background image is repeated both vertically and horizontally. This is default.
    member _.repeat = h.MakeStyle("background-repeat", "repeat")
    /// The background image is only repeated horizontally.
    member _.repeatX = h.MakeStyle("background-repeat", "repeat-x")
    /// The background image is only repeated vertically.
    member _.repeatY = h.MakeStyle("background-repeat", "repeat-y")
    /// The background-image is not repeated.
    member _.noRepeat = h.MakeStyle("background-repeat", "no-repeat")
    /// Sets this property to its default value.
    member _.initial = h.MakeStyle("background-repeat", "initial")
    /// Inherits this property from its parent element.
    member _.inheritFromParent = h.MakeStyle("background-repeat", "inherit")

type CssPositionEngine<'Style>(h: CssHelper<'Style>) =
    /// Default value. Elements render in order, as they appear in the document flow.
    member _.defaultStatic = h.MakeStyle("position", "static")
    /// The element is positioned relative to its first positioned (not static) ancestor element.
    member _.absolute = h.MakeStyle("position", "absolute")
    /// The element is positioned relative to the browser window
    member _.fixedRelativeToWindow = h.MakeStyle("position", "fixed")
    /// The element is positioned relative to its normal position, so "left:20px" adds 20 pixels to the element's LEFT position.
    member _.relative = h.MakeStyle("position", "relative")
    /// The element is positioned based on the user's scroll position
    ///
    /// A sticky element toggles between relative and fixed, depending on the scroll position. It is positioned relative until a given offset position is met in the viewport - then it "sticks" in place (like position:fixed).
    ///
    /// Note: Not supported in IE/Edge 15 or earlier. Supported in Safari from version 6.1 with a -webkit- prefix.
    member _.sticky = h.MakeStyle("position", "sticky")
    member _.initial = h.MakeStyle("position", "initial")
    /// Inherits this property from its parent element.
    member _.inheritFromParent = h.MakeStyle("position", "inherit")

type CssBoxSizingEngine<'Style>(h: CssHelper<'Style>) =
    /// Sets how the total width and height of an element is calculated.
    /// Default value. The width and height properties include the content, but does not include the padding, border, or margin.
    member _.contentBox = h.MakeStyle("box-sizing", "content-box")
    /// The width and height properties include the content, padding, and border, but do not include the margin. Note that padding and border will be inside of the box.
    member _.borderBox = h.MakeStyle("box-sizing", "border-box")
    /// Sets this property to its default value.
    member _.initial = h.MakeStyle("box-sizing", "initial")
    /// Inherits this property from its parent element.
    member _.inheritFromParent = h.MakeStyle("box-sizing", "inherit")

type CssResizeEngine<'Style>(h: CssHelper<'Style>) =
    /// Sets whether an element is resizable, and if so, in which directions.
    /// Default value. The element offers no user-controllable method for resizing it.
    member _.none = h.MakeStyle("resize", "none")
    /// The element displays a mechanism for allowing the user to resize it, which may be resized both horizontally and vertically.
    member _.both = h.MakeStyle("resize", "both")
    /// The element displays a mechanism for allowing the user to resize it in the horizontal direction.
    member _.horizontal = h.MakeStyle("resize", "horizontal")
    /// The element displays a mechanism for allowing the user to resize it in the vertical direction.
    member _.vertical = h.MakeStyle("resize", "vertical")
    /// The element displays a mechanism for allowing the user to resize it in the block direction (either horizontally or vertically, depending on the writing-mode and direction value).
    member _.block = h.MakeStyle("resize", "block")
    /// The element displays a mechanism for allowing the user to resize it in the inline direction (either horizontally or vertically, depending on the writing-mode and direction value).
    member _.inline' = h.MakeStyle("resize", "inline")
    /// Sets this property to its default value.
    member _.initial = h.MakeStyle("resize", "initial")
    /// Inherits this property from its parent element.
    member _.inheritFromParent = h.MakeStyle("resize", "inherit")

    let _wordWrap = CssWordWrapEngine(h)
    let _alignSelf = CssAlignSelfEngine(h)
    let _alignItems = CssAlignItemsEngine(h)
    let _alignContent = CssAlignContentEngine(h)
    let _justifyContent = CssJustifyContentEngine(h)
    let _outlineWidth = CssOutlineWidthEngine(h)
    let _listStyleType = CssListStyleTypeEngine(h)
    let _property = CssPropertyEngine(h)
    let _listStylePosition = CssListStylePositionEngine(h)
    let _textDecorationLine = CssTextDecorationLineEngine(h)
    let _textDecoration = CssTextDecorationEngine(h)
    let _transformStyle = CssTransformStyleEngine(h)
    let _textTransform = CssTextTransformEngine(h)
    let _textOverflow = CssTextOverflowEngine(h)
    let _filter = CssFilterEngine(h)
    let _borderCollapse = CssBorderCollapseEngine(h)
    let _backgroundSize = CssBackgroundSizeEngine(h)
    let _textDecorationStyle = CssTextDecorationStyleEngine(h)
    let _fontStretch = CssFontStretchEngine(h)
    let _floatStyle = CssFloatStyleEngine(h)
    let _verticalAlign = CssVerticalAlignEngine(h)
    let _writingMode = CssWritingModeEngine(h)
    let _animationTimingFunction = CssAnimationTimingFunctionEngine(h)
    let _transitionTimingFunction = CssTransitionTimingFunctionEngine(h)
    let _userSelect = CssUserSelectEngine(h)
    let _borderStyle = CssBorderStyleEngine(h)
    let _tableLayout = CssTableLayoutEngine(h)
    let _cursor = CssCursorEngine(h)
    let _outlineStyle = CssOutlineStyleEngine(h)
    let _backgroundPosition = CssBackgroundPositionEngine(h)
    let _backgroundBlendMode = CssBackgroundBlendModeEngine(h)
    let _backgroundClip = CssBackgroundClipEngine(h)
    let _transform = CssTransformEngine(h)
    let _margin = CssMarginEngine(h)
    let _direction = CssDirectionEngine(h)
    let _emptyCells = CssEmptyCellsEngine(h)
    let _animationDirection = CssAnimationDirectionEngine(h)
    let _animationPlayState = CssAnimationPlayStateEngine(h)
    let _animationIterationCount = CssAnimationIterationCountEngine(h)
    let _animationFillMode = CssAnimationFillModeEngine(h)
    let _backgroundRepeat = CssBackgroundRepeatEngine(h)
    let _position = CssPositionEngine(h)
    let _boxSizing = CssBoxSizingEngine(h)
    let _resize = CssResizeEngine(h)

    member _.wordWrap = _wordWrap
    member _.alignSelf = _alignSelf
    member _.alignItems = _alignItems
    member _.alignContent = _alignContent
    member _.justifyContent = _justifyContent
    member _.outlineWidth = _outlineWidth
    member _.listStyleType = _listStyleType
    member _.property = _property
    member _.listStylePosition = _listStylePosition
    member _.textDecorationLine = _textDecorationLine
    member _.textDecoration = _textDecoration
    member _.transformStyle = _transformStyle
    member _.textTransform = _textTransform
    member _.textOverflow = _textOverflow
    member _.filter = _filter
    member _.borderCollapse = _borderCollapse
    member _.backgroundSize = _backgroundSize
    member _.textDecorationStyle = _textDecorationStyle
    member _.fontStretch = _fontStretch
    member _.floatStyle = _floatStyle
    member _.verticalAlign = _verticalAlign
    member _.writingMode = _writingMode
    member _.animationTimingFunction = _animationTimingFunction
    member _.transitionTimingFunction = _transitionTimingFunction
    member _.userSelect = _userSelect
    member _.borderStyle = _borderStyle
    member _.tableLayout = _tableLayout
    member _.cursor = _cursor
    member _.outlineStyle = _outlineStyle
    member _.backgroundPosition = _backgroundPosition
    member _.backgroundBlendMode = _backgroundBlendMode
    member _.backgroundClip = _backgroundClip
    member _.transform = _transform
    member _.margin = _margin
    member _.direction = _direction
    member _.emptyCells = _emptyCells
    member _.animationDirection = _animationDirection
    member _.animationPlayState = _animationPlayState
    member _.animationIterationCount = _animationIterationCount
    member _.animationFillMode = _animationFillMode
    member _.backgroundRepeat = _backgroundRepeat
    member _.position = _position
    member _.boxSizing = _boxSizing
    member _.resize = _resize
