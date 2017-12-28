using Recipe.NetCore.Enum;
using System;

namespace Recipe.NetCore.Attribute
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class AuditOperation : System.Attribute
    {
        public AuditOperation(OperationType operationType)
        {
            this.OperationType = operationType;
        }

        public OperationType OperationType { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class AuditOperationIgnore : System.Attribute
    {
        public AuditOperationIgnore()
        {
        }
    }
}