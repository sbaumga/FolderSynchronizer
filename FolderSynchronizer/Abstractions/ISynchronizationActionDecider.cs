using FolderSynchronizer.Data;
using FolderSynchronizer.Enums;

namespace FolderSynchronizer.Abstractions
{
    public interface ISynchronizationActionDecider
    {
        FileSynchronizationAction GetNeededActionForFile(FileSynchronizationStatusData file);
    }
}