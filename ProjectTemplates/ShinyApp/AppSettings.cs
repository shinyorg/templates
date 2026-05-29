namespace ShinyApp;

// Mark partial properties with [Bind] and the source generator persists them to a key/value store.
// [Bind] uses the default platform store; [Bind("secure")] uses the secure (encrypted) store.
// Every get rehydrates from the store, so values survive app restarts - no INotifyPropertyChanged needed.
[Singleton]
public partial class AppSettings
{
    // [Bind]
    // public partial string MySetting { get; set; }
}