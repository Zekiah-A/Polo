# Important notes for ILConverter.cs
 - Literals don't count as expressions due to the inability of assigning to a literal in QBE IR, like "%a =w 10",
 hence literals must treat themselves differently, such as creating a context statement out of their definition
 `%b = 77 add 0`, then returning their variable identifier.
`%b` as an expression.

 - As many expressions may be used inline, they are _NOT_ expected to append their context before themselves as it could risk
 messing up where they are used, and should not try to incorporate the context they receive into their source. Instead they
 should pass down their context until the control flow reaches a statement (should have it's own free line to play with and),
 be essentially independent of other statements (a statement is usually not interpolated within another) which can then
 prepend all the accumulated context before itself.

 - Statements should be responsible for appending newlines after themselves, expressions needn't do this as statements
  can be made up of expressions hence meaning newlines within an expression would have the possibility of causing issues
  when inserted into a statement.