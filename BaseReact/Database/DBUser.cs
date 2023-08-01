using Microsoft.EntityFrameworkCore;

namespace BaseReact.Database;

public class DbUser
{
    public int Id { get; set; }
    public string Email { get; set; } = "";
    public string HashedPassword { get; set; }= "";
    public string Salt { get; set; }= "";
}