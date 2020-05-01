using IdleFramework;

public interface CanSnapshot<T> where T:Snapshot
{
    T GetSnapshot(IdleEngine engine);
    void LoadFromSnapshot(T snapshot);
}
