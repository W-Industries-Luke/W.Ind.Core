---
paths:
  - "**/*.cs"
---

# XML documentation

The project generates a documentation file that ships in the package, so these comments are what consumers see in IntelliSense.

- Every public and protected type and member has documentation. New code must not add CS1591 (missing comment) or other documentation warnings; existing gaps get filled when the file is touched.
- Document the longest form of a generic family in full. Shorter forms and interface implementations use `<inheritdoc cref="..."/>` plus a `<remarks>` line stating which defaults they fill in, rather than copying the text or leaving it blank.
- `<summary>`: one sentence saying what the type or member does, written for someone using it. Don't restate the declaration ("An abstract class that…").
- `<remarks>`: how to use or extend it. Separate points go in their own `<para>`.
- Give every type parameter a `<typeparam>` and every parameter a `<param>`, with real content. No empty tags.
- `<returns>` describes the value. Omit it for methods that return `Task` or `void`.
- `<exception cref="...">` lists exceptions callers should expect, with the condition. List what this member throws, not everything the framework might.
- Link types and members with `<see cref="..."/>`; refer to parameters with `<paramref>` and type parameters with `<typeparamref>`.
- Use `<see langword="..."/>` for `null`, `true`, `false` and for a keyword the reader needs to type. Don't wrap every occurrence of `class`, `static` or `string` in ordinary prose; existing comments overdo this.
- Use `<c>` for literal values and inline code.
- Usage examples go in `<example><code>` as plain, copy-pasteable C#. Don't thread `<see cref>` tags through code samples; existing examples do, and they render poorly and can't be pasted.
- Check spelling. Known misspellings in public names (`IEnterredDate`) stay until 2.0; don't repeat them in new names.
