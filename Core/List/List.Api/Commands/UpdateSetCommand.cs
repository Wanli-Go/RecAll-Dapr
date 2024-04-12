using MediatR;
using TheSalLab.GeneralReturnValues;

namespace RecAll.Core.List.Api.Commands;

public class UpdateSetCommand : IRequest<ServiceResult>
{
    public int Id { get; set; }

    public string Name { get; set; }

    public UpdateSetCommand(int id, string name)
    {
        Id = id;
        Name = name;
    }
}