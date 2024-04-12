using MediatR;
using TheSalLab.GeneralReturnValues;

namespace RecAll.Core.List.Api.Commands;

public class DeleteListCommand : IRequest<ServiceResult>
{
    public int Id { get; set; }

    public DeleteListCommand(int id)
    {
        Id = id;
    }
}