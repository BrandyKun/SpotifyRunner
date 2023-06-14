using cleanA.Application.TodoLists.Queries.ExportTodos;

namespace cleanA.Application.Common.Interfaces;

public interface ICsvFileBuilder
{
    byte[] BuildTodoItemsFile(IEnumerable<TodoItemRecord> records);
}
