﻿using FolderSynchronizer.Data;

namespace FolderSynchronizer.Abstractions
{
    public interface IFileSyncChecker
    {
        Task<IEnumerable<FileSynchronizationStatusData>> GetSynchronizationStatusForFilesAsync();
    }
}