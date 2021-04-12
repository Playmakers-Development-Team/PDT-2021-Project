using Commands;
using Units;

public class MoveCommand : Command
{
    public MoveCommand(IUnit unit) : base(unit) {}

    public override void Queue() {}

    public override void Execute() {}

    public override void Undo() {}

    // Start is called before the first frame update
    void Start() {}

    // Update is called once per frame
    void Update() {}
}
