namespace DbCleaner;

public interface ICleaner
{
    public Task ClearSoftDeletedWorldScans();
}