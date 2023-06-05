# AuraKingdomHzUnfucker
Stops Aura Kingdom (or anything else) from modifying monitor refresh rate. 

## Usage

The exe can be ran by double clicking it or by running it from the command line.

### Command Line Flags

When running from the command line you have the option to use the following flags:

```
-k or --keep-open: Keep the application running, by default the application closes after it blocks its first frequency change.
-h or --hide: Hide the application on startup, if keep-open is enabled you will need to kill the application through the task manager.
-m or --minimize: Minimize the application console on startup.
-a or --all: Poll all monitors, by default only the default monitor is polled.
-d:<DOUBLE (e.g. 5.0)>: Poll refresh rate every x seconds. If not set refresh rate will be polled every second.
```

## Compiling

Can be compiled to native code using the following command:
`dotnet publish -c Release -r <RUNTIME_IDENTIFIER>`
