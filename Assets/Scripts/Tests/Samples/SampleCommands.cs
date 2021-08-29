using Commands;

namespace Tests.Samples
{
    [CommandFinishAt(typeof(SomeOperationFinishedCommand))]
    public class SomeOperationCommand : Command {}

    
    public class SomeOperationFinishedCommand : Command {}
}
