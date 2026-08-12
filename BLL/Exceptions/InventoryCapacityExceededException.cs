using System;

namespace BLL.Exceptions
{
    public sealed class InventoryCapacityExceededException : InvalidOperationException
    {
        public InventoryCapacityExceededException()
            : base("Inventory does not have enough capacity for the complete reward.")
        {
        }
    }
}
