# TaskQueuer

Centralized task queuing & execution library in C#

Stores a list of generic handlers to be executed sequentially or in parallel. It allows configuration of timeouts and thread count for concurrent processing.

Supports begin/end/error events that can be used for custom actions and logging purposes.

It can be supplied with all data arguments in case they are known at initialization time, or fed dynamically on demand by writing code that queries a database. A sample client console is included as a separate project with an example of both options.
