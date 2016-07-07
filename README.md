# Sanefiles

Utility to recursively sanity check source files for bad line endings (mixed tabs + BOM checks coming)

Currently optimized for Windows, i.e. it ensures source files have CR LF line endings.
Bad lines are displayed on screen.

Usage: safefiles <DIRECTORY>

Options:

```
OPTIONS:

    --full, -f            Show all conflicting lines
    --all, -a             Check all files regardless of extension
    --help                display this list of options.
```

License: MIT.