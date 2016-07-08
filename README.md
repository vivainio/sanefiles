# Sanefiles

Utility to recursively sanity check source files for bad line endings (mixed tabs + BOM checks coming)

Currently optimized for Windows, i.e. it ensures source files have CR LF line endings.
Bad lines are displayed on screen.

## Installation

Just unpack it, or use [zippim](https://github.com/vivainio/zippim):

```
zippim get https://github.com/vivainio/sanefiles/releases/download/v0.1/sanefiles.0.1.zip
```

Usage: safefiles <DIRECTORY>

Options:

```
OPTIONS:

    --full, -f            Show all conflicting lines
    --all, -a             Check all files regardless of extension
    --help                display this list of options.
```

License: MIT.
