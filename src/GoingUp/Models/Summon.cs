namespace GoingUp.Models
{
    /// <summary>
    /// A summons for the elevator from a certain level, going in a certain direction.
    /// </summary>
    internal record struct Summon(Level level, Direction direction);
}
