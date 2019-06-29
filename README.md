# BlazorBug04
Sample app with now fixed issue surrounding nested `@bind-Value` syntax issues.

### Description

The form on the Home page has two controls inside an `EditForm` component.

The first is an `InputText` component which is bound using `@bind-Value`

The second is a custom component, `MyControl` which also uses the same binding.
Within `MyControl` we have a nested `InputText` component which again uses
`@bind-Value`.

I've left the [demo gif](BlazorBug04.gif) here if you want to see what happens if
you don't implement the fix correctly.


### Cause of the Error

The original error in my code was to assume that the `EventCallback<T>` would be 
called by the `@bind-Value` syntax - that is not the case. The code handling the 
`Set` for the `Value` property has to invoke the `EventCallback<T>`. Invoking 
the event then results in a `StateHasChanged` event.

A *very simple* implementation might look like this:
```cs
private string _value;
[Parameter] public string Value
{
    get = _value;
    set 
    {
        _value = value;
        ValueChanged.InvokeAsync(value);
    }
}
```
Although the `InvokeAsync` is an async method, it's inside a synchronous setter, so this is 
a fire-and-forget approach.

### Solution

With thanks to @JoshLang on Gitter who proposed a much more elegant solution, using a generic `Set` 
helper method, and an extension method that swallows exceptions. 

I've put his code suggestions this into a helper class [ValueChangedHelper](https://github.com/conficient/BlazorBug04/blob/master/BlazorBug04/Shared/ValueChangedHelper.cs)
```cs
public static bool Set<T>(ref T property,
                          T value,
                          in EventCallback<T> propertyChanged,
                          IEqualityComparer<T> equalityComparer = null)
{
    if ((equalityComparer ?? EqualityComparer<T>.Default).Equals(property, value))
    {
        return false;
    }
    // set the property backing field
    property = value;
    // raise a property changed event
    propertyChanged.InvokeAsync(value).SwallowExceptions();
    return true;
}
```
That's fine for the purposes of this demo. In a real App I'd probably want 
code up generic base class to implement most of this boilerplate code. Which 
is what they do in Blazor... 

### Form Component Base

This approach can be seen in the implementations of binding for the built-in
Blazor Form controls, `InputText`, `InputDate` etc.

These all use the generic class [`InputBase`](https://github.com/aspnet/AspNetCore/blob/master/src/Components/Components/src/Forms/InputComponents/InputBase.cs)

This also has `Value` and `ValueChanged` parameters, but they do not implement the 
event triggering in the `Value` property. Instead the actual input control is bound
to a different property `CurrentValue` which does this. 
```cs
protected T CurrentValue 
{ 
    get => Value; 
    set 
    { 
        var hasChanged = !EqualityComparer<T>.Default.Equals(value, Value); 
        if (hasChanged) 
        { 
            Value = value; 
            _ = ValueChanged.InvokeAsync(value); 
            EditContext.NotifyFieldChanged(FieldIdentifier); 
        } 
    } 
} 
```
The usage in the `InputText` control is as follows:
```
 builder.AddAttribute(4, "value", BindMethods.GetValue(CurrentValue));
 builder.AddAttribute(5, "onchange", BindMethods.SetValueHandler(__value => CurrentValue = __value, CurrentValue));
```


Originally logged as an issue [#11679](https://github.com/aspnet/AspNetCore/issues/11679), now closed.
