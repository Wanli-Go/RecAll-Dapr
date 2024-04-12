using MediatR;
using TheSalLab.GeneralReturnValues;

namespace RecAll.Core.List.Api.Commands
{
    public class CreateListCommand : IRequest<ServiceResult> // indicating the return should be in ServiceResult type
    {
        public string Name { get; set; }

        public int TypeId { get; set; }

        public CreateListCommand(string name, int typeId)
        {
            Name = name;
            TypeId = typeId;
        }
    }

}