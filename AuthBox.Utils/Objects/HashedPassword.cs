namespace AuthBox.Utils.Objects;
public class HashedPassword
{
    public required string Hash { get; set; }
    public required byte[] Salt { get; set; }
}
