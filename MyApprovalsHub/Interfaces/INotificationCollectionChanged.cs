using System.Collections.Specialized;

namespace MyApprovalsHub.Interfaces
{
    public interface INotificationCollectionChanged
    {
        event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}
