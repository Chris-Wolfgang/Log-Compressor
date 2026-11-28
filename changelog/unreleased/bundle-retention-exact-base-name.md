type: fix

Bundle retention (`--delete-archives-older-than`) now only deletes archives whose name is the bundle's base name followed directly by the logc timestamp: a bundle named `app` no longer deletes old archives of `app-server` or any other bundle whose name starts with `app-`.
