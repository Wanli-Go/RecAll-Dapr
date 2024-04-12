using MediatR;
using TheSalLab.GeneralReturnValues;

namespace RecAll.Core.List.Api.Commands;

public class UpdateListCommand : IRequest<ServiceResult>
{
    public int Id { get; set; }

    public string Name { get; set; }

    public UpdateListCommand(int id, string name)
    {
        Id = id;
        Name = name;
    }
}