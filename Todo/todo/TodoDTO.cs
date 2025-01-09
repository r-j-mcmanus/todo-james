
public class TodoDTO{

    public TodoDTO() {}

    public TodoDTO(Todo todo) {
        Id = todo.Id;
        Name = todo.Name;
        IsComplete = todo.IsComplete;
    }

     public int Id { get; set; }
     public string? Name { get; set; }
     public bool IsComplete { get; set; } 
};
