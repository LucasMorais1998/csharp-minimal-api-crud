namespace ApiCrud.Students;

public class Student
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; private set; } = null!;

    public bool IsActive { get; private set; } = true;

    public Student(string name)
    {
        Name = name;
    }

    public void UpdateName(string name)
    {
        Name = name;
    }

    public void DisableStudent()
    {
        IsActive = false;
    }
}

