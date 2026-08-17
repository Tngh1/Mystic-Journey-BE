using System;

namespace BLL.Exceptions
{
    // Executes invalid operation exception operation.
    public sealed class InventoryCapacityExceededException : InvalidOperationException
    {
        // Initializes a new default instance of the InventoryCapacityExceededException class.
        public InventoryCapacityExceededException()
            : base("Inventory does not have enough capacity for the complete reward.")
        {
        }
    }
}
