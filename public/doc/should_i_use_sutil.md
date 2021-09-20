Sutil is an F# framework for creating web applications.

Given that you've decided you need a web application framework, then you should consider using Sutil if:

- You would like no dependencies. Sutil is self-contained, and doesn't layer on top of another framework such as React. This is attractive in terms of final application size, and in terms of dependency management for your project.
- You're a fan of Svelte's programming model, or the idea of reactive programming appeals to you
- You would like to avoid a virtual DOM. Sutil uses event-drive bind expressions to regenerate DOM where needed.

Consider *any* F# framework if:

- You'd like to minimize coding mistakes. Use a framework that uses a DSL to build your DOM structure and to specify CSS styles. Sutil uses Feliz.Engine for this.
- You'd like the benefits of powerful data types. This isn't a direct benefit of Sutil itself; this results from Sutil being implemented in F#. Other F# frameworks (such as Feliz) have the same benefit
- You want to learn F#. Writing simple web applications is fun and rewarding way to learn any new language.

-----

You might want to try an alternative to Sutil if none of the above fit for you, and in addition if:

- You need something more mature. Check back in a few months. In the meantime, consider F#/React and F#/Feliz.
- You prefer the utter simplicity of F#/React. Sutil's binding approach can look a little strange, though we think that as an F# programmer you will quickly get used to it. However, the declarative nature of F#/React is very hard to improve upon.

