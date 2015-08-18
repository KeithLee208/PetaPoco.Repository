# PetaPoco.Repository
Simple but powerful PetaPoco repository with strongly typed filters.

## Getting started
```c#
using (var repo = RepositoryFactory.Create<Person>())
{
}
```

Custom repository Type:

```c#
using (var repo = RepositoryFactory.Create<MyRepository<Person>, Person>())
{
}
```

## Insert

```c#
Person entity = new Person
{ 
	Name = "John"
	Age = 22,
};

repo.Save(entity);
```

## Update

```c#
var person = repo.GetSingle(f => f.Where(c => c.Name == "John"));
person.Age = 18;
repo.Save(person);
```

## Retrieving Data

```c#
var person = repo.GetSingle(f => f.Where(c => c.Age == 22));
```

```c#
var people = repo.GetList(f => f.Where(c => c.Age >= 18).And(c => c.Age < 80));
```

## Delete

```c#
repo.Delete(person); // single
repo.Delete(people); // list
repo.Delete(f => f.Where(c => c.Age == 22)); // filter
```

## Sort

```c#
var people = repo.GetList(f => f.Where(c => c.Age >= 18).And(c => c.Age < 80).Order().By(c => c.Age).By(c => c.Name, false));
```
