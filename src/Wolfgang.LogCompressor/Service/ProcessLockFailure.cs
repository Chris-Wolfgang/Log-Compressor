namespace Wolfgang.LogCompressor.Service;

/// <summary>
/// Why <see cref="ProcessLock.TryAcquire"/> could not take the lock.
/// </summary>
internal enum ProcessLockFailure
{
    /// <summary>No failure: the lock was acquired, or no attempt was made yet.</summary>
    None,

    /// <summary>Another process holds the lock file open (or another I/O error occurred while opening it).</summary>
    AlreadyHeld,

    /// <summary>The directory to lock does not exist.</summary>
    DirectoryNotFound,

    /// <summary>The lock file cannot be created there (access denied, read-only directory).</summary>
    CannotCreate
}
