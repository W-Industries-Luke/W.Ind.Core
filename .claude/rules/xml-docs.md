---
paths:
  - "**/*.cs"
---

# XML documentation

The project generates a documentation file that ships in the package, so these comments are what consumers see in IntelliSense.

- Document the longest form of a generic family in full. Shorter forms are usually left undocumented.
- `<summary>`: one sentence saying what the type or member is.
- `<remarks>`: how to use or extend it. Separate points go in their own `<para>`.
- Wrap C# keywords in `<see langword="..."/>`: `<see langword="abstract"/> <see langword="class"/>`, `<see langword="static"/>`, `<see langword="string"/>`, `<see langword="null"/>`.
- Link types and members with `<see cref="..."/>`. Refer to parameters with `<paramref>` and type parameters with `<typeparamref>`.
- Give every type parameter a `<typeparam>` and every parameter a `<param>`.
- `<returns>` describes the value. For a `void`-like `Task`, write "Treat as `void`".
- List each exception the member can throw with `<exception cref="...">` and the condition.
- Usage examples go in `<example><code>` inside `<remarks>`, with identifiers linked through `<see cref>`.
- Use `<c>` for literal values and inline code.
