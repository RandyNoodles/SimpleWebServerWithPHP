# Simple Web Server With PHP

## HTTP Class
- Uses helper classes RequestParser and ResponseBuilder to keep code easier to read & modular
- "Supported headers" are stored as static types
- "Unsupported headers" are stored in a dictionary for potential expansion or flexibility

## Shutdown Management
Implemented using dependency injection.
Goal of this class is to provide a way to track active threads, and provide a single `CancellationTokenSource` for graceful shutdown.
`ITaskShutdown` specifies methods to:
- Register Task
- Remove Task upon completion
- Check ShutdownStatus
- Get a cancellation token

## Logger
Implemented using dependency injection to allow for easy switching between console, file I/O or other logging methods.

## PHP CGI
Runs PHP scripts and returns results to requestor using CGI
- Env variables and stdin/stdout are used to communicate with `php_cgi.exe`

## TCP/IP listener
Async listener, spawns new threads to handle incoming requests.
