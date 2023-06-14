using cleanA.Application.Common.Mappings;
using cleanA.Domain.Entities;

namespace cleanA.Application.TodoLists.Queries.ExportTodos;

public class TodoItemRecord : IMapFrom<TodoItem>
{
    public string? Title { get; set; }

    public bool Done { get; set; }
}
