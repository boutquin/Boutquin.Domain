# Class Name: GuardCondition

The GuardCondition class is a helper class used to chain the Guard.Against method with the With&lt;TException&gt; method.

## Constructor

### `GuardCondition(bool condition)`

Initializes a new instance of the GuardCondition class. The constructor is marked as internal to prevent direct instantiation and is only used by the Guard class.

#### Parameters

- `condition` (bool): The condition to check.

## Methods

### `With<TException>() where TException : Exception, new()`

Throws the specified exception if the condition is true.

#### Type Parameters

- `TException` (Exception): The type of the exception to throw.

#### Exceptions

- `TException`: Thrown when the condition is true.

#### Example Usage

```csharp
public void SetQuantity(int quantity)
{
    Guard.Against(quantity <= 0).With<ArgumentException>();
    Console.WriteLine($"Quantity is set to: {quantity}");
}
```

### `With<TException>(string exceptionMessage) where TException : Exception`

Throws the specified exception with the provided message if the condition is true.

#### Type Parameters

- `TException` (Exception): The type of the exception to throw.

#### Parameters

- `exceptionMessage` (string): The message for the exception.

#### Exceptions

- `TException`: Thrown when the condition is true, with the provided exception message.
- `ArgumentException`: Thrown when the exceptionMessage is null, empty, or contains only whitespace characters.
- `InvalidOperationException`: Thrown when the specified exception type doesn't have a constructor that accepts a single string parameter.

#### Example Usage

```csharp
public void SetQuantity(int quantity)
{
    Guard.Against(quantity <= 0).With<ArgumentException>("Quantity must be greater than zero.");
    Console.WriteLine($"Quantity is set to: {quantity}");
}
```

### `With<TException>(string exceptionMessage, Exception innerException) where TException : Exception`

Throws the specified exception with the provided message and inner exception if the condition is true.

#### Type Parameters

- `TException` (Exception): The type of the exception to throw.

#### Parameters

- `exceptionMessage` (string): The message for the exception.
- `innerException` (Exception): The inner exception to be included.

#### Exceptions

- `TException`: Thrown when the condition is true, with the provided exception message and inner exception.
- `ArgumentException`: Thrown when the exceptionMessage is null, empty, or contains only whitespace characters.
- `ArgumentNullException`: Thrown when the innerException is null.
- `InvalidOperationException`: Thrown when the specified exception type doesn't have a constructor that accepts a single string parameter and an Exception parameter.

#### Example Usage

```csharp
public void SetQuantity(int quantity)
{
    try
    {
        // Some code that might throw an exception
    }
    catch (Exception ex)
    {
        Guard.Against(quantity <= 0).With<ArgumentException>("Quantity must be greater than zero.", ex);
    }
    Console.WriteLine($"Quantity is set to: {quantity}");
}
```