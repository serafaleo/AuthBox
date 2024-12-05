namespace AuthBox.Api.Repositories;

public class BaseRepository
{
    protected readonly DatabaseContext _db;

    public BaseRepository(DatabaseContext db)
    {
        _db = db;
    }
}
